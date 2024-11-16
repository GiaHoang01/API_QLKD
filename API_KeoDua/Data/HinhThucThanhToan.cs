using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_HinhThucThanhToan")]
    public class HinhThucThanhToan
    {
        [Key]
        [MaxLength(10)]
        public string MaHinhThuc { get; set; }

        [MaxLength(30)]
        public string TenHinhThuc { get; set; }
    }
}
