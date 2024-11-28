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
    }
}
