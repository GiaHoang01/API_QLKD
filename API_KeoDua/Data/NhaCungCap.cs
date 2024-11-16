using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_NhaCungCap")]
    public class NhaCungCap
    {
        [Key]
        public Guid MaNCC { get; set; }

        [MaxLength(50)]
        public string TenNCC { get; set; }

        [MaxLength(10)]
        public string SDT { get; set; }

        public string DiaChi { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }
    }
}
