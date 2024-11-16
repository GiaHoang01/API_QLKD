using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_KeoDua.Data
{
    [Table("tbl_ChuongTrinhKhuyenMai")]
    public class ChuongTrinhKhuyenMai
    {
        [Key]
        public Guid MaKhuyenMai { get; set; }

        [MaxLength(50)]
        public string TenCTKhuyenMai { get; set; }

        [MaxLength(255)]
        public string GhiChu { get; set; }

    }
}
