using API_KeoDua.Data;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IPhieuNhapHangReponsitory
    {
        public int TotalRows { get; set; }

        public Task<List<PhieuNhapHang>> GetAllPurchase(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRows);

    }

}
