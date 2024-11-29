using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using System.Text;
namespace API_KeoDua.Reponsitory.Implement
{
    public class CT_HoaDonBanHangReponsitory:ICT_HoaDonBanHangReponsitory
    {
        private readonly CT_HoaDonBanHangContext cT_HoaDonBanHangContext;

        public CT_HoaDonBanHangReponsitory(CT_HoaDonBanHangContext cT_HoaDonBanHangContext)
        {
            this.cT_HoaDonBanHangContext = cT_HoaDonBanHangContext;
        }

        public int TotalRows { get; set; }
    }
}
