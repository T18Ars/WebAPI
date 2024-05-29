using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Data
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public NguoiDung NguoiDung { get; set; }

        public string Token { get; set; }

        public string JwtId { get; set; }

        public bool IsUsed { get; set; } // đã được sử dụng chưa

        public bool IsRevoked { get; set; } // đã được thu hồi chưa

        public DateTime IssuedAt { get; set; } // thời gian tạo

        public DateTime ExpiredAt { get; set; } // thời gian hết hạn
    }
}
