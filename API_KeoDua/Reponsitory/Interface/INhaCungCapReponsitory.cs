using API_KeoDua.Data;
using API_KeoDua.DataView;
namespace API_KeoDua.Reponsitory.Interface
{
    public interface INhaCungCapReponsitory
    {
        public int TotalRows { get; set; }

        public Task<List<NhaCungCap>> QuickSearchNhaCungCap(string searchString);
    }

   
}
