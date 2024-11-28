using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IPhieuGiaoHangReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<PhieuGiaoHang>> GetAllShippingNote(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRow);
    }
}
