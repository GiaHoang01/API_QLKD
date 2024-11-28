using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IPhieuGiaoHangReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<dynamic>> GetAllShippingNote(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRow);
        public Task<dynamic> GetAllShippingNoteByID(Guid? maPhieuGiao);
        public Task<List<dynamic>> GetAllShippingNoteNewCreated(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRow);
        public Task<bool> AddShippingNote(PhieuGiaoHang phieuGiaoHang);
        public Task<bool> UpdateShippingNote(PhieuGiaoHang phieuGiaoHang);
        public Task<bool> DeleteShippingNote(Guid maPhieuGiao);
    }
}
