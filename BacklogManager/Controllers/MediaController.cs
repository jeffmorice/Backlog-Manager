using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BacklogManager.Controllers
{
    public class MediaController : Controller
    {
        public IActionResult Index()
        {
            return Content("Hello, World!");
        }
    }
}