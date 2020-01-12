using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;


namespace GC2WH2.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            
            return View();
        }
    }
}