using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_KeoDua.Data
{
    [Table("tbl_CT_HoaDonBanHang")]
    public class CT_HoaDonBanHang
    {
        [Key, Column(Order = 0)]
        public Guid MaHoaDon { get; set; }

        [Key, Column(Order = 1)]
        public Guid MaHangHoa { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public decimal ThanhTien { get; set; }

    }
}
