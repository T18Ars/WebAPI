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
        public DbSet<DonHang> DonHangs { get; set; } // sẽ lấy DonHangs làm tên bảng nếu ko dùng fluent api
        public DbSet<DonHangChiTiet> DonHangChiTiets { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }

        // sử dụng fluent api
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NguoiDung>(e =>
            {
                e.ToTable("NguoiDung");
                e.HasKey(dh => dh.Id);
                e.Property(dh => dh.UserName).IsRequired().HasMaxLength(50);
                e.Property(dh => dh.Password).IsRequired().HasMaxLength(256);
                e.Property(dh => dh.FullName).HasMaxLength(256);
                e.Property(dh => dh.Email).HasMaxLength(256);
            });

            modelBuilder.Entity<DonHang>(e =>
            {
                e.ToTable("DonHang");
                e.HasKey(dh => dh.MaDh);
                e.Property(dh => dh.NgayDat).HasDefaultValueSql("getdate()");
                e.Property(dh => dh.NguoiNhan).IsRequired().HasMaxLength(100);
            });


            /*
             * Fluent API là để định nghĩa chi tiết các mối quan hệ phức tạp giữa các entity (các bảng) cũng như các ràng buộc dữ liệu.
             * Nếu không cần rảng buộc nhiều có thể định nghĩa trực tiếp trên entity model không cần dùng Fluent API.
             */
            modelBuilder.Entity<DonHangChiTiet>(e =>
            {
                e.ToTable("DonHangChiTiet");
                e.HasKey(entity => new { entity.MaDh, entity.MaHh});
                e.HasOne(entity => entity.DonHang)
                    .WithMany(entity => entity.DonHangChiTiets)
                    .HasForeignKey(entity => entity.MaDh)
                    .HasConstraintName("FK_DonHangCT_DonHang");

                e.HasOne(entity => entity.HangHoa)
                    .WithMany(entity => entity.DonHangChiTiets)
                    .HasForeignKey(entity => entity.MaHh)
                    .HasConstraintName("FK_DonHangCT_HangHoa");
            });
        }
    }
}
