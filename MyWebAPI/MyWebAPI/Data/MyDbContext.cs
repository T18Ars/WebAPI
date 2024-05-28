using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Data
{
    /**
     * Entities <---> DbContext <---> Database
     * DbContext làm việc trực tiếp với database, đây là 1 lớp quan trọng trong Entity Framework, cung cấp nhiều API để Entities tương tác với Database
     * DbContext hỗ trợ truy vấn, theo dõi các thay đổi trên DB, hỗ trợ thao tác add, update, delete, cache, transaction và đặc biệt ánh xạ entities thành table, csdl thực ...
     * DbSet: map Entities thành table, đại diện cho tập các thực thể (record) của 1 Entities được s/d để thực hiện các thao tác thêm, xóa, sửa
     * Snapshot: config, hình ảnh hiện tại db của mình
     */
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }

        public DbSet<HangHoa> HangHoas { get; set; }
        public DbSet<Loai> Loais { get; set; }
    }
}
