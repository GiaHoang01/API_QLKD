using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_ThongTinGiaoHang")]
    public class ThongTinGiaoHang
    {
        [Key]
        public Guid MaThongTin { get; set; }

        [MaxLength(100)]
        public string SoNha { get; set; }

        [MaxLength(40)]
        public string Phuong { get; set; }

        [MaxLength(20)]
        public string Quan { get; set; }
        [MaxLength(20)]
        public string ThanhPho { get; set; }
        [MaxLength(200)]
        public string? DiaChi { get; set; }

        [MaxLength(10)]
        public string SDT { get; set; }

        public Guid MaKhachHang { get; set; }

        public bool MacDinh { get; set; }


        [ForeignKey("MaKhachHang")]
        public virtual KhachHang KhachHang { get; set; }
    }
}
