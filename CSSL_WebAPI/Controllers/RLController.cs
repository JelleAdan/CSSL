using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using CSSL.Examples.AccessController;
using CSSL.RL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSSL_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RLController : ControllerBase
    {
        public RLLayer RLLayer { get; set; }

        public RLController(RLLayer RLLayer)
        {
            this.RLLayer = RLLayer;
        }

        [HttpGet]
        [Route("Status")]
        public void Status()
        {
        }

        [HttpGet]
        [Route("Reset")]
        public string Reset()
        {
            try
            {
                return JsonSerializer.Serialize(RLLayer.Reset());
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        [HttpPost]
        public string Act(int action)
        {
            try
            {
                return JsonSerializer.Serialize(RLLayer.Act(action));
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
