using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_HangHoa")]
    public class HangHoa
    {
        [Key]
        public Guid MaHangHoa { get; set; }
       
        [MaxLength(50)]
        public string TenHangHoa { get; set; }

        [MaxLength(255)]
        public string MoTa { get; set; }

        [MaxLength(255)]
        public string HinhAnh { get; set; }

        [MaxLength(10)]
        public string MaLoai { get; set; }

        [ForeignKey("MaLoai")]
        public virtual LoaiHangHoa LoaiHangHoa { get; set; }

    }
}
