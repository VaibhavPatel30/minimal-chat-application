using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data.IRepository
{
    public interface IGroupRepository
    {
        Task<bool> GroupExistsByNameAsync(string name);
        Task<Guid> AddGroupAsync(Group group);
        Task<List<Group>> GetGroupsAsync();
        Task<Group> GetGroupByIdAsync(Guid GroupId);
        Task UpdateGroupDetailsAsync(Group GroupDetails, string NewGroupName);
        Task DeleteGroupAsync(Group Group);

        Task<GroupMember?> GetMemberAsync(Guid UserId, Guid GroupId);
        Task<GroupMember> AddMemberAsync(GroupMember Member);
        Task<GroupMember?> GetGroupMemberByIdAsync(int MemberId);
        Task RemoveMemberAsync(GroupMember Member);
        Task<List<Guid>> GetMemberUserIdsByGroupIdAsync(Guid groupId);
        Task<List<Message>> GetConversationAsync(Guid groupId, DateTime before, int count, string sort, GroupMember member);
        Task<List<Message>> GetConversationByContentAsync(Guid groupId, string query);
    }
}
