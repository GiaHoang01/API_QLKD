using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [MaxLength(50)]
        public string TenTaiKhoan { get; set; }

        [MaxLength(20)]
        public string MatKhau { get; set; }

        [MaxLength(50)]
        public string TrangThai { get; set; }

        [MaxLength(10)]
        public string MaNhomQuyen { get; set; }

        [ForeignKey("MaNhomQuyen")]
        public virtual NhomQuyen NhomQuyen { get; set; }
    }
}
