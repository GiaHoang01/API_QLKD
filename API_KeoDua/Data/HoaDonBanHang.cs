using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_HoaDonBanHang")]
    public class HoaDonBanHang
    {
        [Key]
        public Guid MaHoaDon { get; set; }

        public DateTime NgayBan { get; set; }
        public DateTime? NgayThanhToan { get; set; } 

        [MaxLength(50)]
        public string TrangThai { get; set; }

        [MaxLength(50)]
        public decimal TongTriGia { get; set; }

        [MaxLength(255)]
        public string? GhiChu { get; set; }

        public Guid? MaNV { get; set; }

        public Guid MaKhachHang { get; set; }

        public Guid? MaGioHang { get; set; }

        [MaxLength(10)]
        public string? MaHinhThuc { get; set; }

        [ForeignKey("MaHinhThuc")]
        public virtual HinhThucThanhToan HinhThucThanhToan { get; set; }

        [ForeignKey("MaGioHang")]
        public virtual GioHang GioHang { get; set; }

        [ForeignKey("MaKhachHang")]
        public virtual KhachHang KhachHang { get; set; }

        [ForeignKey("MaNV")]
        public virtual NhanVien NhanVien { get; set; }
    }
}
