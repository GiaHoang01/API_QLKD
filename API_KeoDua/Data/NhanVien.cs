using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{

    [Table("tbl_NhanVien")]
    public class NhanVien
    {
        [Key]
        [MaxLength(50)]
        public Guid MaNV { get; set; }

        [MaxLength(50)]
        public string TenNV { get; set; }

        [MaxLength(10)]
        public string GioiTinh { get; set; }

        public string DiaChi { get; set; }

        [MaxLength(10)]
        public string SDT { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string ChucVu { get; set; }

        public DateTime NgayVaoLam { get; set; }

        [MaxLength(50)]
        public string TenTaiKhoan { get; set; }

        [MaxLength(50)]
        public DateTime NgaySinh { get; set; }

        [ForeignKey("TenTaiKhoan")]
        public virtual TaiKhoan TaiKhoan { get; set; }
    }
}
