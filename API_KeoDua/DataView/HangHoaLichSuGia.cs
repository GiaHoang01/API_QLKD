using API_KeoDua.Data;
using Microsoft.Identity.Client;
using System.Reflection.Metadata;
namespace API_KeoDua.DataView
{
    public class HangHoaLichSuGia
    {
        public Guid MaHangHoa {  get; set; }
        public string TenHangHoa { get; set; }
        public string MoTa {  get; set; }
        public string HinhAnh { get; set; }
        public string MaLoai { get; set; }
        public decimal GiaBan { get; set; }
    }
}
