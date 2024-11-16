using API_KeoDua.Models;
using log4net;
using log4net.Config;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace API_KeoDua.Controllers
{
    public class BaseController : Controller
    {
        internal ILog logger { get; set; }
        public BaseController()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            this.logger = this.logger ?? LogManager.GetLogger(typeof(LoggerManager));
        }
        internal async Task<ResponseModel> ResponseException()
        {
            return new ResponseModel() { status = -1, message = "Call API Exception" };
        }

        internal async Task<ResponseModel> ResponseFail()
        {
            return new ResponseModel() { status = -1 };
        }

        internal async Task<ResponseModel> ResponseSucceeded()
        {
            return new ResponseModel() { status = 1, message = "" };
        }

    }
}
