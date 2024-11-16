using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_LoaiHangHoa")]
    public class LoaiHangHoa
    {
        [Key]
        [MaxLength(10)]
        public string MaLoai { get; set; }

        [MaxLength(50)]
        public string TenLoai { get; set; }
    }
}
