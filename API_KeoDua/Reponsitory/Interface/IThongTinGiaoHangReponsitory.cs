using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IThongTinGiaoHangReponsitory
    {
        public Task<List<ThongTinGiaoHang>> GetAllInfoDelivery(Guid customerId);
        public Task<bool> AddInfoDelivery(Guid maKhachHang, ThongTinGiaoHang thongTinGiaoHang);
        public Task<bool> DeleteInfoDelivery(Guid maKhachHang, Guid maThongTin);
        public Task<bool> UpdateInfoDelivery(Guid maKhachHang, Guid maThongTin, ThongTinGiaoHang thongTinGiaoHang);
    }
}
