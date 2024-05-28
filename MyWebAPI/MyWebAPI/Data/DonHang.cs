using System;
using System.Collections.Generic;

namespace MyWebAPI.Data
{
    public class DonHang
    {
        public DonHang()
        {
            //DonHangChiTiets = new List<DonHangChiTiet>();
            DonHangChiTiets = new HashSet<DonHangChiTiet>(); // khởi tạo [] tránh null
        }
        public Guid MaDh { get; set; }

        public DateTime NgayDat { get; set; }

        public DateTime? NgayGiao { get; set; }

        public TinhTrangDonHang TinhTrangDonHang { get; set; }

        public string NguoiNhan { get; set; }

        public string DiaChiGiao { get; set; }

        public string SoDienThoai { get; set; }

        public double ThanhTien { get; set; }

        public virtual ICollection<DonHangChiTiet> DonHangChiTiets { get; set; }
    }
    public enum TinhTrangDonHang
    {
        New = 1,
        Payment = 2,
        Complete = 3,
        Cancel = -1
    }
}
