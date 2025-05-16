using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity.DTOs;

namespace MinimalChatApp.Business.IService
{
    public interface IGroupService
    {
        Task<CreateGroupResponse> CreateGroupAsync(string groupName, Guid currentUser);
        Task<UpdateGroupResponse?> UpdateGroupAsync(Guid GroupId, string NewGroupName, Guid currentUser);
        Task<bool> DeleteGroupAsync(Guid GroupId, string GroupName, Guid currentUser);
        Task<AddMemberResponse> AddMemberAsync(Guid UserId, Guid GroupId, Guid currentUser);
        Task<bool> RemoveMemberAsync(int Id, Guid currentUser);
        Task<List<Guid>> GetMemberUserIdsByGroupIdAsync(Guid groupId);
        Task<SendGroupMessageResponse> SendMessageToGroupAsync(Guid groupId, string content, Guid senderId, string senderName);
    }
}
