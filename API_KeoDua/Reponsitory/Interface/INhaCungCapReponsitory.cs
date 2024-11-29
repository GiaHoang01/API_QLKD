using API_KeoDua.Data;
using API_KeoDua.DataView;
namespace API_KeoDua.Reponsitory.Interface
{
    public interface INhaCungCapReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<NhaCungCap>> GetAllVendors(string searchString, int startRow, int maxRow);
        public Task<bool> AddVendor(NhaCungCap nhaCungCap);
        public Task<NhaCungCap> GetVendorByID(Guid maNCC);
        public Task<bool> DeleteVendor(Guid MaNCC);
        public Task<bool> UpdateVendor(NhaCungCap nhaCungCap);
        public Task<List<NhaCungCap>> QuickSearchNhaCungCap(string searchString);
        public Task<string> SearchNhaCungCap_ByMaNCC(Guid? maNCC);
    }
}
