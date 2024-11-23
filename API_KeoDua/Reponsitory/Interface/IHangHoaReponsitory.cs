using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IHangHoaReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<HangHoaLichSuGia>> GetAllProduct(string searchString, int startRow, int maxRows);
        public Task AddProduct(HangHoa newProduct, decimal giaBan, string? ghiChu);
        public Task<bool> UpdateProduct(HangHoa updatedProduct, decimal giaBan, string? ghiChu);
        public Task DeleteProduct(Guid MaHangHoa);
        public Task<List<LichSuGia>> GetPriceHistoryProduct(Guid MaHangHoa);
    }
}
