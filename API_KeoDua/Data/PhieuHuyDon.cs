using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_PhieuHuyDon")]
    public class PhieuHuyDon
    {
        [Key]
        public Guid MaPhieuHuy { get; set; }

        public DateTime NgayHuy { get; set; }

        [MaxLength(255)]
        public string LyDo { get; set; }

        public Guid MaHoaDon { get; set; }


        [ForeignKey("MaHoaDon")]
        public virtual HoaDonBanHang HoaDonBanHang { get; set; }
    }
}
