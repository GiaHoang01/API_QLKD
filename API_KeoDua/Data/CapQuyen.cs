using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_CapQuyen")]
    public class CapQuyen
    {
        [MaxLength(10)]
        public string MaQuyen { get; set; }

        [MaxLength(10)]
        public string MaNhomQuyen { get; set; }

        [ForeignKey("MaQuyen")]
        public virtual Quyen Quyen { get; set; }

        [ForeignKey("MaNhomQuyen")]
        public virtual NhomQuyen NhomQuyen { get; set; }

    }
}
