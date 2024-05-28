using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangHoaController : ControllerBase
    {
        public static List<HangHoa> hangHoas = new List<HangHoa>();

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(hangHoas);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        //[Authorize]
        public IActionResult Add(HangHoa hanghoa)
        {
            try
            {
                var hh = new HangHoa
                {
                    MaHangHoa = Guid.NewGuid(),
                    TenHangHoa = hanghoa.TenHangHoa,
                    DonGia = hanghoa.DonGia
                };
                hangHoas.Add(hh);
                return StatusCode(StatusCodes.Status201Created, hh);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                var hh = hangHoas.FirstOrDefault(x => x.MaHangHoa == Guid.Parse(id));
                if (hh is null)
                    return NotFound();

                return Ok(hh);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // Put: truyền field nào update field đấy, các field ko còn tồn tại hoặc giá trị default
        // Patch: truyền field nào update field đấy, các field ko ảnh hưởng, vẫn giữ giá trị cũ
        [HttpPut("{id}")]
        public IActionResult Edit(string id, HangHoa hanghoa)
        {
            try
            {
                var hh = hangHoas.FirstOrDefault(x => x.MaHangHoa == Guid.Parse(id));
                if (hh is null)
                    return NotFound();
                hh.TenHangHoa = hanghoa.TenHangHoa;
                hh.DonGia = hanghoa.DonGia;
                
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var hh = hangHoas.FirstOrDefault(x => x.MaHangHoa == Guid.Parse(id));
                if (hh is null)
                    return NotFound();
                hangHoas.Remove(hh);
                return Ok(hangHoas);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /*
         * 1xx: infomational response
         * 
         * 2xx: success
         * 200: OK - request thành công
         * 201: Created - tạo mới xong trả luôn object vừa tạo
         * 204: No Content - dùng cho api update
         * 
         * 3xx: redirection
         * 
         * 4xx: client error
         * 400: Bad request - server ko hiểu client gửi lên cái gì, lỗi gì đó
         * 401: Unauthorized - chưa chứng thực người dùng, chưa login
         * 403: Forbidden - sau khi ng dùng chứng thực, login xong rồi nhưng ko có quyền (authorized) cho api này
         * 404: Not Found - ko tìm thấy trang, ko tìm thấy id, ...
         * 405: Method Not Allowed - sai cách gọi method http (HttpGet nhưng dùng HttpPost để gọi)
         * 422: Unprocessable Entity - request gửi body, object (POST, PUT, PATCH) nhưng object thiếu cú pháp
         * 
         * 5xx: server error
         * 500: Internal Server Error
         * 503: Service Unavailable
         * 504: Gateway Timeout: vượt quá thời gian Gateway chờ trong microservice
         */
    }
}
