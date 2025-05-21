using Microsoft.AspNetCore.Http;
using MinimalChatApp.Business.ExceptionHandlers;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Business.Service
{
    public class GroupService : IGroupService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGroupRepository _groupRepository;
        private readonly IMessageRepository _messageRepository;

        public GroupService(IGroupRepository groupRepository, IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor)
        {
            _groupRepository = groupRepository;
            _messageRepository = messageRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateGroupResponse> CreateGroupAsync(string groupName, Guid currentUser)
        {
            bool exists = await _groupRepository.GroupExistsByNameAsync(groupName);
            if (exists)
            {
                throw new ConflictException("Group creation failed because the name is already used");
            }

            var group = new Group
            {
                Name = groupName,
                CreatedBy = currentUser
            };

            var createdGroupId = await _groupRepository.AddGroupAsync(group);

            var newMember = new GroupMember
            {
                UserId = currentUser,
                GroupId = createdGroupId,
                AccessType = MessageAccessType.All,
                Days = null,
                JoinedAt = DateTime.UtcNow
            };

            await _groupRepository.AddMemberAsync(newMember);

            return new CreateGroupResponse
            {
                GroupId = group.GroupId,
                Name = group.Name,
                CreatedBy = group.CreatedBy
            };
        }

        public async Task<UpdateGroupResponse?> UpdateGroupAsync(Guid GroupId, string NewGroupName, Guid currentUser)
        {
            List<Group> groupDetails = await _groupRepository.GetGroupsAsync();

            var group = groupDetails.FirstOrDefault(g => g.GroupId == GroupId);
            var isNameTaken = groupDetails.Any(g => g.GroupId != GroupId && g.Name.Equals(NewGroupName, StringComparison.OrdinalIgnoreCase));

            if (group == null)
                throw new NotFoundException("Group not found.");

            if (group.CreatedBy != currentUser)
                throw new UnauthorizedAccessException("You are not authorized to modify this group.");

            if (isNameTaken)
                throw new ConflictException("Group modification failed because the name is already used.");

            //Update the group details
            group.Name = NewGroupName; //update the name of group in model
            await _groupRepository.UpdateGroupDetailsAsync(group, NewGroupName);
            return new UpdateGroupResponse
            {
                GroupId = GroupId,
                GroupName = NewGroupName
            };

        }

        public async Task<bool> DeleteGroupAsync(Guid GroupId, string GroupName, Guid currentUser)
        {
            var group = await _groupRepository.GetGroupByIdAsync(GroupId);

            if (group == null)
                throw new NotFoundException("Group not found.");

            if (!group.Name.Equals(GroupName))
                throw new BadRequestException("Group name does not match.");

            if (group.CreatedBy != currentUser)
                throw new UnauthorizedAccessException("You are not authorized to delete this group.");

            //Delete the group
            await _groupRepository.DeleteGroupAsync(group);

            return true;
        }

        public async Task<AddMemberResponse> AddMemberAsync(Guid UserId, Guid groupId, Guid currentUser, MessageAccessType accessType, int? days)
        {
            var existing = await _groupRepository.GetMemberAsync(UserId, groupId);
            if (existing != null)
                throw new ConflictException("User is already a member of the group.");

            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new NotFoundException("Group does not exist.");

            // Check if current user is creator or already a member of this group

            var currentUserMembership = await _groupRepository.GetMemberAsync(currentUser, groupId);
            if (currentUserMembership == null)
                throw new UnauthorizedAccessException("Only group members or creator can add new members.");

            // Validate access type
            if (accessType == MessageAccessType.Days && (!days.HasValue || days <= 0))
                throw new ArgumentException("Access type is set to 'Days' but a valid number of days was not provided.");


            var newMember = new GroupMember
            {
                UserId = UserId,
                GroupId = groupId,
                AccessType = accessType,
                Days = accessType == MessageAccessType.Days ? days : null,
                JoinedAt = DateTime.UtcNow
            };

            var added = await _groupRepository.AddMemberAsync(newMember);

            return new AddMemberResponse
            {
                Id = added.Id,
                UserId = added.UserId,
                GroupId = added.GroupId,
                AccessType = added.AccessType,
                Days = added.Days
            };
        }


        public async Task<bool> RemoveMemberAsync(int memberId, Guid currentUser)
        {
            var member = await _groupRepository.GetGroupMemberByIdAsync(memberId);
            if (member == null)
                throw new NotFoundException("Group member not found.");

            var group = await _groupRepository.GetGroupByIdAsync(member.GroupId);
            if (group == null)
                throw new NotFoundException("Group not found.");

            // Only group creator or existing group members can remove
            if (group.CreatedBy != currentUser)
            {
                var isMember = await _groupRepository.GetMemberAsync(currentUser, group.GroupId);
                if (isMember == null)
                    throw new UnauthorizedAccessException("Only group members or the creator can remove a member.");
            }

            await _groupRepository.RemoveMemberAsync(member);
            return true;
        }

        public async Task<SendGroupMessageResponse> SendMessageToGroupAsync(Guid groupId, string content, IFormFile? Attachment, Guid? ParentMessageId, Guid senderId, string senderName)
        {
            // Check if sender is a member
            var isMember = await _groupRepository.GetMemberAsync(senderId, groupId);
            if (isMember == null)
                throw new UnauthorizedAccessException("Only group members can send messages.");

            string? fileUrl = null;
            string? fileType = null;

            if (Attachment != null)
            {
                var fileId = Guid.NewGuid();
                var fileName = $"{fileId}_{Attachment.FileName}";
                var filePath = Path.Combine("wwwroot/uploads", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using var stream = new FileStream(filePath, FileMode.Create);
                await Attachment.CopyToAsync(stream);

                var scheme = _httpContextAccessor.HttpContext?.Request.Scheme;
                var host = _httpContextAccessor.HttpContext?.Request.Host.Value;

                fileUrl = $"{scheme}://{host}/uploads/{fileName}";
                fileType = Attachment.ContentType;
            }

            // Create message
            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = senderId,
                SenderName = senderName,
                ReceiverId = groupId,
                Content = content,
                Attachment = fileUrl,
                AttachmentType = fileType,
                Timestamp = DateTime.UtcNow,
                ParentMessageId= ParentMessageId
            };
            await _messageRepository.CreateAsync(message);

            // Save GroupMessage mapping
            var groupMessage = new GroupMessage
            {
                GroupId = groupId,
                MessageId = message.MessageId
            };

            await _messageRepository.AddGroupMessageAsync(groupMessage);

            return new SendGroupMessageResponse
            {
                MessageId = message.MessageId,
                GroupId = groupId,
                SenderId = senderId,
                Content = content,
                Attachment = fileUrl,
                AttachmentType = fileType,
                Timestamp = message.Timestamp,
                ParentMessageId = ParentMessageId
            };
        }

        public async Task<List<Guid>> GetMemberUserIdsByGroupIdAsync(Guid groupId)
        {
            return await _groupRepository.GetMemberUserIdsByGroupIdAsync(groupId);
        }

        public async Task<List<Message>> GetConversationAsync(Guid currentUserId, Guid groupId, DateTime before, int count, string sort)
        {
            var member = await _groupRepository.GetMemberAsync(currentUserId, groupId);
            if (member == null)
                throw new UnauthorizedAccessException("Unauthorized to get the conversation.");
            return await _groupRepository.GetConversationAsync(groupId, before, count, sort, member);
        }

        public async Task<List<Message>> GetConversationByContentAsync(Guid currentUser, Guid groupId, string query)
        {
            var isMember = await _groupRepository.GetMemberAsync(currentUser, groupId);
            if (isMember == null)
                throw new UnauthorizedAccessException("Unauthorized to search the conversation.");

            return await _groupRepository.GetConversationByContentAsync(groupId, query);
        }
    }
}
