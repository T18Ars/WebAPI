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
using System.Security.Cryptography;
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

        private TokenModel GenerateToken(NguoiDung nguoiDung)
        {
            try
            {
                #region tạo accessToken và refreshtoken
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, nguoiDung.FullName),
                        new Claim(JwtRegisteredClaimNames.Email, nguoiDung.Email),
                        new Claim(JwtRegisteredClaimNames.Sub, nguoiDung.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("UserName", nguoiDung.UserName),
                        new Claim("Id", nguoiDung.Id.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(20),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
                };
                var token = jwtTokenHandler.CreateToken(tokenDescription);
                var result = new TokenModel();
                var accessToken = jwtTokenHandler.WriteToken(token);
                var refreshToken = GenerateRefreshToken();
                #endregion

                #region lưu refreshtoken vào db
                var refreshTokenEntity = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    JwtId = token.Id,
                    Token = refreshToken,
                    ExpiredAt = DateTime.UtcNow.AddHours(24),
                    IssuedAt = DateTime.UtcNow,
                    IsRevoked = false,
                    IsUsed = false,
                    UserId = nguoiDung.Id
                };
                _dbContext.RefreshToken.Add(refreshTokenEntity);
                _dbContext.SaveChanges();
                #endregion

                return new TokenModel { 
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        [HttpPost("RenewToken")]
        public IActionResult RenewToken(TokenModel model)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);
                var tokenValidationParam = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Xác thực nhà phát hành của token
                    ValidateAudience = false, // Xác thực người nhận của token
                    ValidateIssuerSigningKey = true, // Xác thực khóa ký của nhà phát hành token
                    ValidateLifetime = false, // Có kiểm tra token đã hết hạn hay ko, ở đây để false vì chỉ cần check định dạng AccessToken mà ko quan tâm đến thời hạn AccessToken, nếu hết hạn token thì hàm ValidateToken sẽ nhảy luôn sang catch

                    ValidIssuer = "nhannv", // Nhà phát hành hợp lệ của token
                    ValidAudience = "user", // Người nhận hợp lệ của token
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes), // Khóa bí mật được sử dụng để ký token
                    ClockSkew = TimeSpan.Zero
                };

                // validate AccessToken, RefreshToken 
                #region check 1: định dạng AccessToken
                var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidationParam, out SecurityToken validatedToken);

                #endregion

                #region check 2: kiểm tra thuật toán (Algorithm)
                /*
                 * "pattern matching" (khớp mẫu) được giới thiệu từ C# 7.0, được sử dụng để kiểm tra xem một đối tượng có thuộc kiểu cụ thể nào đó hay không, và nếu đúng, nó sẽ gán đối tượng đó cho một biến mới
                 * "is" toán tử kiểm tra kiểu
                 */
                if (validatedToken is JwtSecurityToken jwtSecurityToken) // kiểm tra validatedToken có thuộc kiểu JwtSecurityToken ko, nếu đúng thì gán vào biến jwtSecurityToken
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.CurrentCultureIgnoreCase);
                    if (!result)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Định dạng AccessToken hoặc thuật toán token không đúng!"
                        });
                    }
                }
                #endregion

                #region check 3: kiểm tra AccessToken đã hết hạn chưa
                var timeExpireToken = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value;
                var utcExpireDate = long.Parse(timeExpireToken); // biến này là 1 số tính từ năm 1970

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "AccessToken chưa hết thời hạn sử dụng!"
                    });
                }
                #endregion

                #region check 4: kiểm tra RefreshToken có tồn tại trong db
                var refreshTokenInDb = _dbContext.RefreshToken.FirstOrDefault(x => x.Token == model.RefreshToken);
                if(refreshTokenInDb is null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "RefreshToken không tồn tại!"
                    });
                }
                #endregion

                #region check 5: kiểm tra RefreshToken đã sử dụng, thu hồi hay chưa
                if (refreshTokenInDb.IsUsed)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "RefreshToken đã được sử dụng để cấp AccessToken!"
                    });
                }
                if (refreshTokenInDb.IsRevoked)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "RefreshToken đã được thu hồi!"
                    });
                }
                #endregion

                #region check 6: kiểm tra id của AccessToken có nằm trong RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (refreshTokenInDb.JwtId != jti)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "AccessToken không hợp lệ với RefreshToken!"
                    });
                }
                #endregion

                // update IsUsed và IsRevoked
                refreshTokenInDb.IsUsed = true;
                refreshTokenInDb.IsRevoked = true;
                _dbContext.RefreshToken.Update(refreshTokenInDb);
                _dbContext.SaveChanges();

                // create new token
                var user = _dbContext.NguoiDungs.SingleOrDefault(x => x.Id == refreshTokenInDb.UserId);
                var token = GenerateToken(user);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Cấp mới token thành công!",
                    Data = token
                });
            }
            catch (Exception)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra sự cố!"
                });
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
            // ToUniversalTime: chuyển đổi giá trị của đối tượng DateTime hiện tại từ thời gian địa phương sang thời gian UTC => Trả về một đối tượng DateTime mới có cùng thời gian tương ứng nhưng được biểu diễn theo giờ UTC
            return dateTimeInterval;
        }
    }
}
