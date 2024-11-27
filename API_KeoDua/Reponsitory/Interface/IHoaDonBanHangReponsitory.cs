using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IHoaDonBanHangReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<HoaDonBanHangView>> GetAllSaleInVoiceWithWait(DateTime fromDate, DateTime toDate, string searchString, Guid? employeeId, Guid? cartId, Guid? customerId, string? maHinhThuc, int startRow, int maxRows);
        public Task<List<HoaDonBanHangView>> GetAllSaleInVoice(DateTime fromDate, DateTime toDate, string searchString, Guid? employeeId, Guid? cartId, Guid? customerId, string? maHinhThuc, int startRow, int maxRows);
        public Task<bool> ConfirmSaleInvoice(Guid maHoaDon, Guid maNV);
        public Task<List<object>> QuickSearchSaleInvoiceNewCreated(string searchString);
        public Task AddSaleInvoice(HoaDonBanHang hoaDonBanHang, List<CT_HoaDonBanHang> cT_HoaDonBanHangs);
        public Task<bool> UpdateSaleInvoice(HoaDonBanHang hoaDonBanHang, List<CT_HoaDonBanHang> cT_HoaDonBanHangs);
        public Task<bool> DeleteSaleInvoice(Guid maHoaDon);
        public Task<bool> CancelSaleInvoice(Guid maHoaDon, Guid maNV);
        public Task<int> TotalSalesCompletedRecords();
        public Task<decimal> TotalSalesCompletedAmount();
        public Task<decimal> TotalRevenueByYear(int year);
    }
}
