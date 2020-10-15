using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniChatRoom.Helpers;
using Microsoft.AspNetCore.Authorization;
using MiniChatRoom.Models;
using MiniChatRoom.Data;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using Dapper;
using Microsoft.Data.SqlClient;

namespace MiniChatRoom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly JwtHelpers jwt;
        private readonly MiniChatRoomContext _context;
        private readonly string connectionString="Server=(localdb)\\mssqllocaldb;Database=MiniChatRoomDB;Trusted_Connection=True;MultipleActiveResultSets=true";

        public TokenController(JwtHelpers jwt, MiniChatRoomContext context)
        {
            this.jwt = jwt;
            _context = context;
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult<string> SignIn(LoginViewModel login)
        {
            if (ValidateUser(login))
            {
                var token = jwt.GenerateToken(login.Account);

                //設定HttpOnly可有效降低 XSS 的影響並提升攻擊難度
                //Response.Cookies.Append("Token",token,new CookieOptions { 
                //    HttpOnly=true,
                //});;
                return Ok(new { Token = token, Message = "Success" });
            }
            else
            {
                return Content("無此帳號或密碼錯誤");
            }
        }

        private bool ValidateUser(LoginViewModel login)
        {
            int checkAccount = _context.Member.Where(x => x.Account == login.Account && x.Password == login.Password).Count();
            if (checkAccount > 0)
                return true;

            return false;
        }


        [Authorize]
        [HttpGet]
        [Route("~/api/GetUserNickname")]
        public IActionResult GetUserNickname()
        {
            var nickName = _context.Member.Where(x => x.Account == User.Identity.Name).Select(x => x.Nickname).FirstOrDefault();

            return Ok(nickName);
        }


        [Authorize]
        [HttpGet]
        [Route("~/api/GetUserID")]
        public IActionResult GetUserID()
        {
            var userID = _context.Member.Where(x => x.Account == User.Identity.Name).Select(x => x.Id).FirstOrDefault();

            return Ok(userID);
        }

        [Authorize]
        [HttpGet]
        [Route("~/api/GetChatRoomData")]
        public IActionResult GetChatRoomData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"select Nickname,MsgContent from Message
INNER JOIN MEMBER ON MemberID=Member.Id
WHERE Message.RoomID=1";

                var result = conn.Query(sql).ToList();

                return Ok(result);
            }

        }

        [Authorize]
        [HttpPost]
        [Route("~/api/CreateMessage")]
        public async Task<IActionResult> CreateMessage(int memberID, string msgContent)
        {
            Message msg = new Message()
            {
                MemberID = memberID,
                RoomID = 1,
                MsgContent = msgContent,
                CreateTime = DateTime.Now
            };

            _context.Message.Add(msg);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public class LoginViewModel
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }
    }
}
