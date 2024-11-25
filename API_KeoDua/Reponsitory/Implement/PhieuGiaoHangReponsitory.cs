using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Transactions;

namespace API_KeoDua.Reponsitory.Implement
{
    public class PhieuGiaoHangReponsitory
    {
        private readonly PhieuGiaoHangContext phieuGiaoHangContext;
        private readonly CT_PhieuNhapContext ct_PhieuNhapContext;

        public PhieuGiaoHangReponsitory(PhieuGiaoHangContext phieuGiaoHangContext, CT_PhieuNhapContext ct_PhieuNhapContext)
        {
            this.phieuGiaoHangContext = phieuGiaoHangContext;
            this.ct_PhieuNhapContext = ct_PhieuNhapContext;
        }

        public int TotalRows { get; set; }

        //public async Task<List<PhieuGiaoHang>> GetAllShippingNote(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRow)
        //{
        //    try
        //    {
        //        var sqlWhere = new StringBuilder();
        //        var param = new DynamicParameters();

        //        if(!string.IsNullOrEmpty(searchString))
        //    }
        //}
    }
}
