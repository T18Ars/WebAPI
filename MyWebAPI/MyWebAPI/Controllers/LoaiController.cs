using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiController : ControllerBase
    {
        private readonly MyDbContext _context;

        public LoaiController(MyDbContext context)
        {
            _context = context;
        }
        // GET: api/<LoaiController>
        [HttpGet]
        public IActionResult GetAll()
        {
            var dsLoais = _context.Loais.ToList();
            return Ok(dsLoais);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var loai = _context.Loais.FirstOrDefault(x => x.Ma == id);
                if (loai is not null)
                    return Ok(loai);
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Add(Models.Loai loai)
        {
            try
            {
                var item = new Loai
                {
                    Ten = loai.Ten
                };
                _context.Add(item);
                _context.SaveChanges();
                return Ok(item);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("{ma}")]
        public IActionResult Edit(int ma, Models.Loai loai)
        {
            try
            {
                var item = _context.Loais.FirstOrDefault(x => x.Ma == ma);
                if (item is null)
                    return NotFound();
                item.Ten = loai.Ten;
                _context.Update(item);
                _context.SaveChanges();

                return Ok(item);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{ma}")]
        public IActionResult Delete(int ma)
        {
            try
            {
                var item = _context.Loais.FirstOrDefault(x => x.Ma == ma);
                if (item is null)
                    return NotFound();
                _context.Remove(item);
                _context.SaveChanges();
                return Ok(_context.Loais.ToList());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
