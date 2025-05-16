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
    }
}
