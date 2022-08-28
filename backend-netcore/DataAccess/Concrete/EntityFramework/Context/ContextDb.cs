using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework.Context
{
    public class ContextDb : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-5MKMB20\SQLEXPRESS01;Database=datingapp35;Integrated Security=true");
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Photo> Photo { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>().HasKey(k => new { k.SourceUserId, k.LikedUserId });

            builder.Entity<UserLike>().HasOne(s => s.SourceUser).WithMany(l => l.LikedUsers).HasForeignKey(h => h.SourceUserId).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserLike>().HasOne(s => s.LikedUser).WithMany(l => l.LikedByUsers).HasForeignKey(h => h.LikedUserId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>().HasOne(s => s.Recipient).WithMany(l => l.MessagesReceived).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>().HasOne(s => s.Sender).WithMany(l => l.MessagesSent).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
