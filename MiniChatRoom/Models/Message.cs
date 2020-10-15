using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace MiniChatRoom.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int RoomID { get; set; }

        [Required]
        public int MemberID { get; set; }

        [Required]
        public string MsgContent { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
