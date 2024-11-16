using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{

    [Table("tbl_Quyen")]
    public class Quyen
    {
        [Key]
        [MaxLength(10)]
        public string MaQuyen { get; set; }

        [MaxLength(50)]
        public string TenQuyen { get; set; }
    }
}
