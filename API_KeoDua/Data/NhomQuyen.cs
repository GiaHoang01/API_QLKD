using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_NhomQuyen")]
    public class NhomQuyen
    {
        [Key]
        [MaxLength(10)]
        public string MaNhomQuyen { get; set; }

        [MaxLength(50)]
        public string TenNhomQuyen { get; set; }
    }
}
