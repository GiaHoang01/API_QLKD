using API_KeoDua.Data;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IPhieuHuyDonReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<PhieuHuyDon>> GetAllShippingNoteCancel(DateTime fromDate, DateTime toDate, int startRow, int maxRow);
        public Task<object> GetShippingNoteCancelByID(Guid? maPhieuHuy);
        public Task<bool> UpdateShippingNoteCancel(Guid maPhieuHuy, Guid? maPhieuGiao);
        public Task<bool> AddShippingNoteCancel(PhieuHuyDon phieuHuyDon);
        public Task<bool> DeleteShippingNoteCancel(Guid maPhieuHuy);
    }
}
