using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using API_KeoDua.DataView;
using Microsoft.AspNetCore.Mvc;
using API_KeoDua.Reponsitory.Interface;
using API_KeoDua.Data;

namespace API_KeoDua.Reponsitory.Implement
{
    public class LichSuGiaReponsitory:ILichSuGiaReponsitory
    {
        private readonly HangHoaContext hangHoaContext;
        private readonly LichSuGiaContext lichSuGiaContext;
        public LichSuGiaReponsitory(HangHoaContext hangHoaContext, LichSuGiaContext lichSuGiaContext)
        {
            this.hangHoaContext = hangHoaContext;
            this.lichSuGiaContext = lichSuGiaContext;
        }

        public int TotalRows { get; set; }
    }
}
