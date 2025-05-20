using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        public DbSet<MessageNotification> MessageNotifications { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Message>()
            //    .HasOne(m => m.Receiver)
            //    .WithMany(u => u.ReceivedMessages)
            //    .HasForeignKey(m => m.ReceiverId)
            //    .OnDelete(DeleteBehavior.Restrict);

            ////////////////////////Group Members/////////////////////////////

            // Group <-> User (CreatedBy)
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Creator)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey(g => g.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            // GroupMember <-> Group
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => gm.Id);

            modelBuilder.Entity<GroupMember>()
                .Property(gm => gm.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // GroupMember <-> User
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            ////////////////////////Group Messages/////////////////////////////

            // Add mapping config
            modelBuilder.Entity<GroupMessage>()
                .HasKey(gm => gm.Id);

            modelBuilder.Entity<GroupMessage>()
                .HasOne(gm => gm.Group)
                .WithMany()
                .HasForeignKey(gm => gm.GroupId);

            modelBuilder.Entity<GroupMessage>()
                .HasOne(gm => gm.Message)
                .WithMany()
                .HasForeignKey(gm => gm.MessageId);
        }
    }
}
