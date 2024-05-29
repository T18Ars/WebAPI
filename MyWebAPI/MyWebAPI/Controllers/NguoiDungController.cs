using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyWebAPI.Data;
using MyWebAPI.Models;
using MyWebAPI.Services.Loai;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly AppSetting _appSetting;

        // IOptionsMonitor<AppSetting> optionsMonitor: lấy các thông số đã map từ file appsetting.json (nhớ khai báo map trong file Startup trước)
        // nếu ko thích cách này có thể s/d cách sau
        // IConfiguration configuration
        // configuration["AppSettings:SecretKey"]
        public NguoiDungController(MyDbContext dbContext, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _dbContext = dbContext;
            _appSetting = optionsMonitor.CurrentValue;
        }

        [HttpPost("Login")]
        public IActionResult Add(NguoiDungVM nguoiDung)
        {
            try
            {
                var user = _dbContext.NguoiDungs.SingleOrDefault(x => x.UserName == nguoiDung.UserName && x.Password == nguoiDung.Password);
                if (user is null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Tài khoản hoặc mật khẩu không đúng!",
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Login thành công!",
                    Data = GenerateToken(user)
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private string GenerateToken(NguoiDung nguoiDung)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, nguoiDung.FullName),
                        new Claim(ClaimTypes.Email, nguoiDung.Email),
                        new Claim("UserName", nguoiDung.UserName),
                        new Claim("Password", nguoiDung.Password),
                        new Claim("TokenId", Guid.NewGuid().ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
                };
                var token = jwtTokenHandler.CreateToken(tokenDescription);
                return jwtTokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
