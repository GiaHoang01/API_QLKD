using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IHoaDonBanHangReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<HoaDonBanHang>> GetAllSaleInVoiceWithWait(string searchString, int startRow, int maxRows);
        public Task<List<HoaDonBanHang>> GetAllSaleInVoice(string searchString, int startRow, int maxRows);
    }
}
