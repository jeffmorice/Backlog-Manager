using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BacklogManager.Data;
using Microsoft.AspNetCore.Mvc;
using BacklogManager.Models;
using BacklogManager.ViewModels;

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

        public IActionResult Add()
        {
            AddMediaObjectViewModel addMediaObjectViewModel = new AddMediaObjectViewModel();

            return View(addMediaObjectViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMediaObjectViewModel addMediaObjectViewModel)
        {
            if (ModelState.IsValid)
            {
                MediaObject newMediaObject = new MediaObject
                {
                    Title = addMediaObjectViewModel.Title,
                    MediaType = addMediaObjectViewModel.MediaType,
                    DatabaseSource = addMediaObjectViewModel.DatabaseSource,
                    Started = addMediaObjectViewModel.Started,
                    DateAdded = DateTime.Now, //.ToString("yyyy-MM-dd"), //preferred format
                    RecommendSource = addMediaObjectViewModel.RecommendSource
                };

                context.MediaObjects.Add(newMediaObject);
                context.SaveChanges();

                return Redirect("/");
            }
            return View(addMediaObjectViewModel);

        }
    }
}