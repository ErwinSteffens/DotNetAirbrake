using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotNetAirbrake;

namespace MvcAppSample.Controllers
{
    public class TestController : Controller
    {
        public void SendExceptionToAirbrake()
        {
            this.RaiseAnException();
        }

        public async Task<string> SendManually()
        {
            try
            {
                this.RaiseAnException();
            }
            catch (Exception exc)
            {
                await exc.SendToAirbrakeAsync(this.HttpContext);
            }
            return "Exception is send!";
        }

        public void RaiseAnException()
        {
            throw new InvalidOperationException("This is a test exception");
        }
    }
}