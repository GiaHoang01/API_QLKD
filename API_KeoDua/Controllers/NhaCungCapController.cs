using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API_KeoDua.Controllers
{
    [Route("~/api/[controller]/[action]")]
    [ApiController]
    public class NhaCungCapController : BaseController
    {
        private readonly NhaCungCapContext nhaCungCapContext;
        private readonly INhaCungCapReponsitory nhaCungCapReponsitory;

        public NhaCungCapController(NhaCungCapContext nhaCungCapContext, INhaCungCapReponsitory nhaCungCapReponsitory)
        {
           this.nhaCungCapContext = nhaCungCapContext;
           this.nhaCungCapReponsitory= nhaCungCapReponsitory;
        }
    }
}
