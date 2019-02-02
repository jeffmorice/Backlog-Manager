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

        [HttpPost]
        public IActionResult DeletePrompt(int[] mediaIds)
        {
            List<MediaObject> mediaToDelete = new List<MediaObject>();

            //get the id from the query
            foreach (int mediaId in mediaIds)
            {
                //ToDo: Verify User owns item. If they do, then proceed with adding to list for deletion.
                //Match id with media object and pass into View
                mediaToDelete.Add(context.MediaObjects.Single(m => m.ID == mediaId));
            }
            
            //Display prompt asking the user whether they would really like to delete this entry
            return View(mediaToDelete);
        }
        
        [HttpPost]
        public IActionResult Delete(int[] mediaIds)
        {
            foreach (int mediaId in mediaIds)
            {
                MediaObject theMedia = context.MediaObjects.Single(m => m.ID == mediaId);

                context.MediaObjects.Remove(theMedia);
            }

            context.SaveChanges();

            return Redirect("/");
        }
    }
}