using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Data
{
    [Table("Loai")]
    public class Loai
    {
        [Key]
        public int Ma { get; set; }

        [Required]
        [MaxLength(50)]
        public string Ten { get; set; }

        public virtual ICollection<HangHoa> HangHoas { get; set; }
    }
}
