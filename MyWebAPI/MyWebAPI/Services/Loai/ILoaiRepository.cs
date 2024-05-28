using MyWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Services.Loai
{
    public interface ILoaiRepository
    {
        List<LoaiVM> GetAll();

        LoaiVM GetById(int id);

        LoaiVM Add(LoaiVM loai);

        void Update(LoaiVM loai);

        void Delete(int id);
    }
}
