using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Business.IService
{
    public interface IGroupService
    {
        Task<CreateGroupResponse> CreateGroupAsync(string groupName, Guid currentUser);
        Task<UpdateGroupResponse?> UpdateGroupAsync(Guid GroupId, string NewGroupName, Guid currentUser);
        Task<bool> DeleteGroupAsync(Guid GroupId, string GroupName, Guid currentUser);
        Task<AddMemberResponse> AddMemberAsync(Guid UserId, Guid GroupId, Guid currentUser, MessageAccessType AccessType, int? days);
        Task<bool> RemoveMemberAsync(int Id, Guid currentUser);
        Task<List<Guid>> GetMemberUserIdsByGroupIdAsync(Guid groupId);
        Task<SendGroupMessageResponse> SendMessageToGroupAsync(Guid groupId, string content, IFormFile? Attachment, Guid? ParentMessageId, Guid senderId, string senderName);
        Task<List<Message>> GetConversationAsync(Guid currentUserId, Guid groupId, DateTime before, int count, string sort);
        Task<List<Message>> GetConversationByContentAsync(Guid currentUser, Guid groupId, string query);
    }
}
