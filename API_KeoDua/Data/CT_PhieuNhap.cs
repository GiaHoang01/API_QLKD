using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_KeoDua.Data
{
    [Table("tbl_CT_PhieuNhap")]
    public class CT_PhieuNhap
    {

        [Key, Column(Order = 0)]
        public Guid MaPhieuNhap { get; set; }

        [Key, Column(Order = 1)]
        public Guid MaHangHoa { get; set; }

        public int SoLuong { get; set; }

        public int SoLuongDat { get; set; }

        public decimal DonGia { get; set; }

        public decimal ThanhTien { get; set; }
    }
}
