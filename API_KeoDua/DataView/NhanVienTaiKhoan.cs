using API_KeoDua.Data;
namespace API_KeoDua.DataView
{
    public class NhanVienTaiKhoan
    {
        public string TenTaiKhoan { get; set; }
        public string MatKhau {  get; set; }
        public string MaNhomQuyen { get; set; }
        public string TenNV { get; set; }
        public string Email { get; set; }
        public string ChucVu { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public DateTime NgaySinh { get; set; }
        public DateTime NgayVaoLam { get; set; }
        public string GioiTinh { get; set; }
    }
}
