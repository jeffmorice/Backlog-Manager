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
        public IActionResult DeletePrompt(int[] deletedIds)
        {
            //write List<int> to List<MediaObject> method and invoke here
            List<MediaObject> mediaToDelete = new List<MediaObject>();

            mediaToDelete = ArrayIdsToListMediaObjects(deletedIds);

            //Display prompt asking the user whether they would really like to delete this entry
            return View(mediaToDelete);
        }
        
        [HttpPost]
        public IActionResult Delete(int[] deletedIds)
        {
            foreach (int deletedId in deletedIds)
            {
                MediaObject theMedia = context.MediaObjects.Single(m => m.ID == deletedId);

                context.MediaObjects.Remove(theMedia);
            }

            context.SaveChanges();

            return Redirect("/");
        }

        [HttpPost]
        public IActionResult Update(int[] mediaIds, int[] startedValues, int[] completedValues, int[] deletedIds)
        {
            //ToDo: allow for text fields to be updated via form input.
            int mediaAccumulator = 0;
            int startedAccumulator = 0;
            int completedAccumulator = 0;
            
            foreach (int mediaId in mediaIds)
            {
                MediaObject updatedMedia = context.MediaObjects.Single(m => m.ID == mediaId);

                if ((mediaAccumulator + startedAccumulator + 1) < startedValues.Length)
                {
                    if (startedValues[mediaAccumulator + startedAccumulator + 1] == 1)
                    {
                        updatedMedia.Started = true;
                        startedAccumulator += 1;
                    }
                    else
                    {
                        updatedMedia.Started = false;
                    }
                }
                else if ((mediaAccumulator + startedAccumulator + 1) == startedValues.Length)
                {
                    updatedMedia.Started = false;
                }

                if ((mediaAccumulator + completedAccumulator + 1) < completedValues.Length)
                {
                    if (completedValues[mediaAccumulator + completedAccumulator + 1] == 1)
                    {
                        updatedMedia.Completed = true;
                        completedAccumulator += 1;
                    }
                    else
                    {
                        updatedMedia.Completed = false;
                    }
                }
                else if ((mediaAccumulator + completedAccumulator + 1) == completedValues.Length)
                {
                    updatedMedia.Completed = false;
                }

                //ToDo: Add UpdateCount logic. Check if initial values matches end values. If any columns do not match, add 1.
                mediaAccumulator += 1;
            }

            //Save changes
            context.SaveChanges();

            //Then check for deleted Ids and pass any to the DeletePrompt route
            if (deletedIds.Length > 0)
            {
                List<MediaObject> mediaToDelete = new List<MediaObject>();
                mediaToDelete = ArrayIdsToListMediaObjects(deletedIds);
                return View("DeletePrompt", mediaToDelete);
            }

            return Redirect("/");
        }

        public List<MediaObject> ArrayIdsToListMediaObjects(int[] arrayIds)
        {
            List<MediaObject> mediaList = new List<MediaObject>();
            
            foreach (int arrayId in arrayIds)
            {
                mediaList.Add(context.MediaObjects.Single(m => m.ID == arrayId));
            }
            return mediaList;
        }
    }
}