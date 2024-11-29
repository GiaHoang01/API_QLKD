using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_PhieuHuyDon")]
    public class PhieuHuyDon
    {
        [Key]
        public Guid MaPhieuHuy { get; set; }

        public DateTime NgayHuy { get; set; }

        [MaxLength(255)]
        public string LyDo { get; set; }

        public Guid MaPhieuGiao { get; set; }


        [ForeignKey("MaPhieuGiao")]
        public virtual PhieuGiaoHang PhieuGiaoHang { get; set; }
    }
}
