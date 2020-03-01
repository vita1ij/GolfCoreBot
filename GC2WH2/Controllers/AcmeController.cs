using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GC2DB.Data;
using GC2DB.Managers;
using Microsoft.AspNetCore.Mvc;

namespace GC2WH2.Controllers
{
    public class AcmeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(AcmeData? values)
        {
            if (values != null)
            {
                AcmeManager.Create(values.Key, values.Value);
            }
            return Index();
        }

        [Route("/.well-known/acme-challenge/{data?}")]
        public IActionResult Verify(string data)
        {
            return Content(AcmeManager.Get(data));
        }
    }
}