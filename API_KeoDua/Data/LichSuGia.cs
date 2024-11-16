using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_LichSuGia")]
    public class LichSuGia
    {
        [Key]
        public Guid MaLichSu { get; set; }

        public decimal GiaBan { get; set; }

        public DateTime NgayCapNhatGia { get; set; }

        [MaxLength(255)]
        public string GhiChu { get; set; }

        public Guid MaHangHoa { get; set; }

        [ForeignKey("MaHangHoa")]
        public virtual HangHoa HangHoa { get; set; }
    }
}
