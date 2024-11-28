using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IKhachHangReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<KhachHang>> GetAllCustomer(string searchString, int startRow, int maxRow);
        public Task<bool> IsPhoneNumberExists(string phoneNumber);
        public Task<bool> AddCustomer(KhachHang khachHang);
        public Task<KhachHang> GetCustomerByID(Guid MaKH);
        public Task<bool> DeleteCustomer(Guid MaKH);
        public Task<bool> UpdateCustomer(KhachHang kh);
        public Task<List<KhachHang>> QuickSearchKhachHang(string searchString);
    }
}
