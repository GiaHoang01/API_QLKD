﻿using API_KeoDua.Data;
using API_KeoDua.DataView;
namespace API_KeoDua.Reponsitory.Interface
{
    public interface IPhieuNhapHangReponsitory
    {
        public int TotalRows { get; set; }

        public Task<List<PhieuNhapHang>> GetAllPurchase(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRows);
        public Task<(PhieuNhapHang phieuNhap, List<CT_PhieuNhap> chiTietPhieuNhap)> GetPurchase_ByID(Guid? maPhieuNhap);
        public Task AddPurchaseOrderRequest(PhieuNhapHang phieuNhapHang, List<CT_PhieuNhap> ctPhieuNhaps);
        public Task<bool> UpdatePurchaseOrderRequest(PhieuNhapHang phieuNhapHang, List<CT_PhieuNhap> ctPhieuNhaps);
        public Task<bool> DeletePurchaseOrderRequest(Guid maPhieuNhap);
        public Task<List<PhieuNhapHang>> GetAllPurchaseRequest(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRows);
    }

}
