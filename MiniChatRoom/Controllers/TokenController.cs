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

namespace MiniChatRoom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly JwtHelpers jwt;
        private readonly MiniChatRoomContext _context;

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
                var token= jwt.GenerateToken(login.Account);
                return Ok(new { Token = token, Message = "Success" });
            }
            else
            {
                return Content("無此帳號");
            }
        }

        private bool ValidateUser(LoginViewModel login)
        {
            int checkAccount = _context.Member.Where(x => x.Account == login.Account && x.Password == login.Password).Count();
            if (checkAccount > 0)
                return true;

            return false;
        }

        //[HttpGet("~/claims")]
        //public IActionResult GetClaims()
        //{
        //    return Ok(User.Claims.Select(p => new { p.Type, p.Value }));
        //}

        [Authorize]
        [HttpGet]
        [Route("~/api/userName")]
        public IActionResult GetUserNickname()
        {
            int i = 0;
            var nickName = _context.Member.Where(x => x.Account == User.Identity.Name).Select(x => x.Nickname);

            return Ok(nickName);
        }


        [Authorize]
        [HttpGet]
        [Route("~/api/userName")]
        public IActionResult GetUserName()
        {
            int i = 0;
            return Ok(User.Identity.Name);
        }
        //[HttpGet("~/jwtid")]
        //public IActionResult GetUniqueId()
        //{
        //    var jti = User.Claims.FirstOrDefault(p => p.Type == "jti");
        //    return Ok(jti.Value);
        //}

        public class LoginViewModel
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }
    }
}
