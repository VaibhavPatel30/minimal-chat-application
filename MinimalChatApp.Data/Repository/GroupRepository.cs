using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AppDbContext _context;

        public GroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> GroupExistsByNameAsync(string name)
        {
            return await _context.Groups.AnyAsync(g => g.Name.ToLower() == name.ToLower());
        }

        public async Task<Guid> AddGroupAsync(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group.GroupId;
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await _context.Groups.ToListAsync();
        }

        public async Task<Group> GetGroupByIdAsync(Guid GroupId)
        {
            return await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == GroupId);
        }

        public async Task UpdateGroupDetailsAsync(Group GroupDetails, string NewGroupName)
        {
            GroupDetails.Name = NewGroupName;
            _context.Groups.Update(GroupDetails);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(Group Group)
        {
            _context.Groups.Remove(Group);
            await _context.SaveChangesAsync();
        }

        public async Task<GroupMember?> GetMemberAsync(Guid UserId, Guid GroupId)
        {
            return await _context.GroupMembers
            .FirstOrDefaultAsync(m => m.UserId == UserId && m.GroupId == GroupId);
        }

        public async Task<GroupMember> AddMemberAsync(GroupMember member)
        {
            await _context.GroupMembers.AddAsync(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<GroupMember?> GetGroupMemberByIdAsync(int MemberId)
        {
            return await _context.GroupMembers
                         .FirstOrDefaultAsync(gm => gm.Id == MemberId);
        }

        public async Task RemoveMemberAsync(GroupMember Member)
        {
            _context.GroupMembers.Remove(Member);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Guid>> GetMemberUserIdsByGroupIdAsync(Guid groupId)
        {
            return await _context.GroupMembers
                        .Where(gm => gm.GroupId == groupId)
                        .Select(gm => gm.UserId)
                        .ToListAsync();
        }

        public async Task<List<Message>> GetConversationAsync(Guid groupId, DateTime before, int count, string sort, GroupMember member)
        {
            var query = await _context.Messages
                        .Where(m => m.ReceiverId == groupId && m.Timestamp < before).ToListAsync();

            switch (member.AccessType)
            {
                case MessageAccessType.None:
                    query = query.Where(m => m.Timestamp >= member.JoinedAt).ToList();
                    break;

                case MessageAccessType.All:
                    // No additional filter needed
                    break;

                case MessageAccessType.Days:
                    if (member.Days.HasValue)
                    {
                        var accessStartDate = member.JoinedAt.AddDays(-member.Days.Value);
                        query = query.Where(m => m.Timestamp >= accessStartDate).ToList();
                    }
                    break;
            }

            query = sort == "desc"
                ? query.OrderByDescending(m => m.Timestamp).ToList()
                : query.OrderBy(m => m.Timestamp).ToList();

            return  query.Take(count).ToList();
        }

        public async Task<List<Message>> GetConversationByContentAsync(Guid groupId, string query)
        {
            string loweredQuery = $"%{query.ToLower()}%";
            return await _context.Messages
                        .Where(m => m.ReceiverId == groupId && EF.Functions.Like(m.Content.ToLower(), loweredQuery))
                        .OrderBy(m => m.Timestamp)
                        .ToListAsync();
        }
    }
}
