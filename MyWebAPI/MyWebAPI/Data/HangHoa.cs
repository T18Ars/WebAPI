using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Data
{
    [Table("HangHoa")]
    public class HangHoa
    {
        [Key]
        public Guid MaHangHoa { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenHangHoa { get; set; }

        public string? MoTa { get; set; }

        [Range(0, double.MaxValue)]
        public double? DonGia { get; set; }

        public byte? GiamGia { get; set; }

        public int? MaLoai { get; set; }

        [ForeignKey("MaLoai")]
        public Loai Loai { get; set; }
    }
}
