using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_KeoDua.Data
{
    [Table("tbl_ChiTietCT_KhuyenMai")]
    public class ChiTietCT_KhuyenMai
    {
        public Guid MaKhuyenMai { get; set; }
        public Guid MaHangHoa { get; set; }
        public DateTime NgayBD { get; set; }

        public DateTime NgayKT { get; set; }

        public decimal TiLeKhuyenMai { get; set; }

        [ForeignKey("MaHangHoa")]
        public virtual HangHoa HangHoa { get; set; }

        [ForeignKey("MaKhuyenMai")]
        public virtual ChuongTrinhKhuyenMai KhuyenMai { get; set; }
    }
}
