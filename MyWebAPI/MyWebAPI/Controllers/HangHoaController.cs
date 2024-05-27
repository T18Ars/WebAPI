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
                return Ok(hh);
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
                
                return Ok(hh);
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
    }
}
