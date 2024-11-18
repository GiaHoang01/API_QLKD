using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_KeoDua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangHoaController : BaseController
    {
        private readonly HangHoaContext hangHoaContext;
        private readonly IHangHoaReponsitory hangHoaReponsitory;
        public HangHoaController(HangHoaContext context, IHangHoaReponsitory hangHoaReponsitory)
        {
            this.hangHoaContext = context;
            this.hangHoaReponsitory = hangHoaReponsitory;
        }
    }
}
