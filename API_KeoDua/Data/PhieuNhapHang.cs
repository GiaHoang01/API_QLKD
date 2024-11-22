using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_PhieuNhapHang")]
    public class PhieuNhapHang
    {
        [Key]
        public Guid MaPhieuNhap { get; set; }

        public DateTime NgayNhap { get; set; }

        public DateTime NgayDat { get; set; }

        [MaxLength(50)]
        public string TrangThai  { get; set; }

        [MaxLength(50)]
        public decimal TongTriGia { get; set; }

        [MaxLength(255)]
        public string GhiChu { get; set; }

        public Guid MaNV { get; set; }

        public Guid MaNCC { get; set; }
    }
}
