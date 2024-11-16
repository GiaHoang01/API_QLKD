using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_KeoDua.Data
{
    [Table("tbl_LoaiKhachHang")]
    public class LoaiKhachHang
    {
        [Key]
        [MaxLength(10)]
        public string MaLoaiKH { get; set; }

        [MaxLength(50)]
        public string TenLoaiKH { get; set; }

    }
}
