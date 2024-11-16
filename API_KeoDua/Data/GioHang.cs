using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_GioHang")]
    public class GioHang
    {
        [Key]
        public Guid MaGioHang { get; set; }

        public decimal TongTriGia { get; set; }

    }
}
