using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BacklogManager.Data;
using Microsoft.AspNetCore.Mvc;
using BacklogManager.Models;

namespace BacklogManager.Controllers
{
    public class MediaController : Controller
    {
        private readonly MediaObjectDbContext context;

        public MediaController(MediaObjectDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<MediaObject> mediaList = context.MediaObjects.ToList();

            return View(mediaList);
        }
    }
}