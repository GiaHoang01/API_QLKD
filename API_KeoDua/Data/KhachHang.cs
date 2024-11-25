using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_KhachHang")]
    public class KhachHang
    {
        [Key]
        public Guid MaKhachHang { get; set; }

        [MaxLength(50)]
        public string TenKhachHang { get; set; }

        [MaxLength(10)]
        public string GioiTinh { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string? TenTaiKhoan { get; set; }

        [MaxLength(10)]
        public string MaLoaiKH { get; set; }

        [MaxLength (10)]
        public string? Sdt {  get; set; }

        [ForeignKey("TenTaiKhoan")]
        public virtual TaiKhoan TaiKhoan { get; set; }

        [ForeignKey("MaLoaiKH")]
        public virtual LoaiKhachHang loaiKhachHang { get; set; }
    }
}
