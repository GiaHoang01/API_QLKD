using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API_KeoDua.Controllers
{
    [Route("~/api/[controller]/[action]")]
    [ApiController]
    public class PhieuNhapHangController : BaseController
    {
        private readonly PhieuNhapHangContext phieuNhapHangContext;
        private readonly IPhieuNhapHangReponsitory phieuNhapHangReponsitory;
        private readonly CT_PhieuNhapContext cT_PhieuNhapContext;
        private readonly ICT_PhieuNhapReponsitory cT_PhieuNhapReponsitory;
        public PhieuNhapHangController(PhieuNhapHangContext phieuNhapHangContex, IPhieuNhapHangReponsitory phieuNhapHangReponsitory
            ,CT_PhieuNhapContext cT_PhieuNhapContext, ICT_PhieuNhapReponsitory cT_PhieuNhapReponsitory)
        {
            this.phieuNhapHangContext = phieuNhapHangContext;
            this.phieuNhapHangReponsitory = phieuNhapHangReponsitory;
            this.cT_PhieuNhapContext=cT_PhieuNhapContext ;
            this.cT_PhieuNhapReponsitory=  cT_PhieuNhapReponsitory ;
        }

    }
}
