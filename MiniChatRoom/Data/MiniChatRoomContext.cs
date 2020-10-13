using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniChatRoom.Models;

namespace MiniChatRoom.Data
{
    public class MiniChatRoomContext : DbContext
    {
        public MiniChatRoomContext (DbContextOptions<MiniChatRoomContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Member { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Room> Room { get; set; }

    }
}
