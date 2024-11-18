using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using System.Text;
namespace API_KeoDua.Reponsitory.Implement
{
    public class CT_PhieuNhapReponsitory:ICT_PhieuNhapReponsitory
    {
        private readonly CT_PhieuNhapContext cT_PhieuNhapContext;

        public CT_PhieuNhapReponsitory(CT_PhieuNhapContext cT_PhieuNhapContext)
        {
            this.cT_PhieuNhapContext = cT_PhieuNhapContext;
        }

        public int TotalRows { get; set; }
    }
}
