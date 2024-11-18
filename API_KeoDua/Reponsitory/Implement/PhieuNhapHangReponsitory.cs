using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using System.Text;
namespace API_KeoDua.Reponsitory.Implement
{
    public class PhieuNhapHangReponsitory:IPhieuNhapHangReponsitory
    {
        private readonly PhieuNhapHangContext phieuNhapHangContext;

        public PhieuNhapHangReponsitory(PhieuNhapHangContext phieuNhapHangContext)
        {
            this.phieuNhapHangContext = phieuNhapHangContext;
        }

        public int TotalRows { get; set; }
    }
}
