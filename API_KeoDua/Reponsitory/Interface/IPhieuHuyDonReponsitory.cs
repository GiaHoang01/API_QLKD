using API_KeoDua.Data;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IPhieuHuyDonReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<PhieuHuyDon>> GetAllShippingNoteCancel(DateTime fromDate, DateTime toDate, int startRow, int maxRow);
        public Task<PhieuHuyDon> GetAllShippingNoteCancelByID(Guid? maPhieuHuy);
    }
}
