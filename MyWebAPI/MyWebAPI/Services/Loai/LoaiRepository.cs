using Microsoft.EntityFrameworkCore;
using MyWebAPI.Data;
using MyWebAPI.Helpers;
using MyWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyWebAPI.Services.Loai
{
    public class LoaiRepository : ILoaiRepository
    {
        protected readonly MyDbContext _context;

        public LoaiRepository(MyDbContext context)
        {
            _context = context;
        }

        public LoaiVM Add(LoaiVM loai)
        {
            try
            {
                var _loai = new Data.Loai
                {
                    Ten = loai.Ten
                };
                _context.Add(_loai);
                _context.SaveChanges();
                return new LoaiVM
                {
                    Ma = _loai.Ma,
                    Ten = _loai.Ten
                };
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        public void Delete(int id)
        {
            var loai = _context.Loais.FirstOrDefault(x => x.Ma == id);
            if (loai is not null)
            {
                _context.Remove(loai);
                _context.SaveChanges();
            }
        }

        public List<LoaiVM> GetAll(int page, int page_size, string sort, dynamic filter, string search)
        {
            var query = _context.Loais.AsQueryable();

            #region xử lý đầu vào filter
            dynamic filterObj = HandlerFilterQuery.Handler(filter);
            #endregion

            #region filterObj
            if (filterObj != null)
            {
                String ten = filterObj.ten;
                if (!string.IsNullOrEmpty(ten))
                {
                    ten = ten.ToLower();
                    query = query.Where(x => x.Ten.ToLower().Contains(ten));
                }
            }
            #endregion

            if (search is not null)
            {
                query.Where(x => x.Ten.Contains(search));
            }

            #region pagination
            var result = PaginatedList<Data.Loai>.Create(query, page, page_size);
            #endregion

            return result.Select(x => new LoaiVM
            {
                Ma = x.Ma,
                Ten = x.Ten
            }).ToList();
        }

        public LoaiVM GetById(int id)
        {
            var loai = _context.Loais.FirstOrDefault(x => x.Ma == id);
            if (loai is not null)
            {
                return new LoaiVM
                {
                    Ma = loai.Ma,
                    Ten = loai.Ten
                };
            }
            else
                return null;
        }

        public void Update(LoaiVM loai)
        {
            var _loai = _context.Loais.FirstOrDefault(x => x.Ma == loai.Ma);
            if (loai is not null)
            {
                _loai.Ten = loai.Ten;
                _context.Update(_loai);
                _context.SaveChanges();
            }
        }
    }
}
