using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_PhieuGiaoHang")]
    public class PhieuGiaoHang
    {
        [Key]
        public Guid MaPhieuGiao { get; set; }

        public DateTime NgayTao { get; set; }
        public DateTime NgayGiao { get; set; }

        [MaxLength(50)]
        public string TrangThai { get; set; }

        public Guid MaHoaDon { get; set; }
        public Guid MaNV { get; set; }


        [ForeignKey("MaHoaDon")]
        public virtual HoaDonBanHang HoaDonBanHang { get; set; }
    }
}
