using System.ComponentModel.DataAnnotations;

namespace API_KeoDua.DataView
{
    public class HoaDonBanHangView
    {
        public Guid MaHoaDon { get; set; }

        public DateTime NgayBan { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string TrangThai { get; set; }
        public decimal TongTriGia { get; set; }

        public string? GhiChu { get; set; }

        public Guid? MaNV { get; set; }
        public string TenNV { get; set; }

        public Guid MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }

        public Guid? MaGioHang { get; set; }
        public string? MaHinhThuc { get; set; }
    }
}
