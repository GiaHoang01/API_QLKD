using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_ChiTietGioHang")]
    public class ChiTietGioHang
    {
        [Key, Column(Order = 0)]
        public Guid MaGioHang { get; set; }

        [Key, Column(Order = 1)]
        public Guid MaHangHoa { get; set; }

        [MaxLength(50)]
        public int SoLuong { get; set; }

        [MaxLength(50)]
        public decimal DonGia { get; set; }

        [MaxLength(50)]
        public decimal ThanhTien { get; set; }

        [ForeignKey("MaGioHang")]
        public virtual GioHang GioHang { get; set; }

        [ForeignKey("MaHangHoa")]
        public virtual HangHoa HangHoa { get; set; }
    }
}
