using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Data;
using MyWebAPI.Models;
using MyWebAPI.Services.Loai;
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
        private readonly ILoaiRepository _loaiRepository;

        public LoaiController(ILoaiRepository loaiRepository)
        {
            _loaiRepository = loaiRepository;
        }
        // GET: api/<LoaiController>
        [HttpGet]
        public IActionResult GetAll(int page = 1, int page_size = 0, string sort = null, string filter = null, string search = null)
        {
            var dsLoais = _loaiRepository.GetAll(page, page_size, sort, filter, search);
            return Ok(dsLoais);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var loai = _loaiRepository.GetById(id);
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
        public IActionResult Add(LoaiVM loai)
        {
            try
            {
                var item = _loaiRepository.Add(loai);
                return Ok(item);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Edit(LoaiVM loai)
        {
            try
            {
                _loaiRepository.Update(loai);

                return NoContent();
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
                _loaiRepository.Delete(ma);
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
