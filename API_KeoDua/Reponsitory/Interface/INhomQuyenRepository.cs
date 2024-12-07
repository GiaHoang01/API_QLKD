using API_KeoDua.Data;
using API_KeoDua.DataView;
namespace API_KeoDua.Reponsitory.Interface
{
    public interface INhomQuyenRepository
    {
        public int TotalRows { get; set; }

        public Task<List<NhomQuyen>> quickSearchNhomQuyen(string searchString);
        public Task<List<NhomQuyen>> GetNhomQuyen();
        public Task<List<Quyen>> GetQuyenByTenNhomQuyen(string TenNQ);
        public Task UpdateRole(string tenTaiKhoan, string tenNhomQuyen);
    }
}
