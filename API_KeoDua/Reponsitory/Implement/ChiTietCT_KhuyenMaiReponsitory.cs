using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using System.Text;

namespace API_KeoDua.Reponsitory.Implement
{
    public class ChiTietCT_KhuyenMaiReponsitory:IChiTietCT_KhuyenMaiReponsitory
    {
        private readonly ChiTietCT_KhuyenMaiContext chiTietCT_KhuyenMaiContext;

        public ChiTietCT_KhuyenMaiReponsitory(ChiTietCT_KhuyenMaiContext chiTietCT_KhuyenMaiContext)
        {
            this.chiTietCT_KhuyenMaiContext = chiTietCT_KhuyenMaiContext;
        }

        public int TotalRows { get; set; }
    }
}
