using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IKhachHangReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<KhachHang>> GetAllCustomer(string searchString, int startRow, int maxRow);
    }
}
