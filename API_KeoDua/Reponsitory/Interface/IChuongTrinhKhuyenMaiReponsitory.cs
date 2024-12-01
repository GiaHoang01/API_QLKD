using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IChuongTrinhKhuyenMaiReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<ChuongTrinhKhuyenMai>> GetAllPromotion(string searchString, int startRow, int maxRows);
        public Task<(ChuongTrinhKhuyenMai chuongTrinhKhuyenMai, List<ChiTietCT_KhuyenMai> chiTietKhuyenMai)> GetPromotion_ByID(Guid? maKhuyenMai);
        public Task AddPromotion(ChuongTrinhKhuyenMai chuongTrinhKhuyenMai, List<ChiTietCT_KhuyenMai> chiTietKhuyenMaiList);
        public Task<bool> UpdatePromotion(ChuongTrinhKhuyenMai chuongTrinhKhuyenMai, List<ChiTietCT_KhuyenMai> chiTietKhuyenMaiList);
        public Task<bool> DeletePromotion(Guid maKhuyenMai);
    }
}
