using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BacklogManager.Data;
using Microsoft.AspNetCore.Mvc;
using BacklogManager.Models;
using BacklogManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BacklogManager.Common;
using System.Net.Http;

namespace BacklogManager.Controllers
{
    //[Authorize]
    public class MediaController : Controller
    {
        private readonly MediaObjectDbContext context;

        public MediaController(MediaObjectDbContext dbContext)
        {
            context = dbContext;
        }

        //Actions

        public IActionResult Index()
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);

            MediaIndexViewModel mediaIndexViewModel = new MediaIndexViewModel
            {
                MediaOjects = context.MediaObjects.
                Include(s => s.MediaSubType).
                Where(u => u.OwnerId == userId).
                Where(d => d.Deleted == false).ToList(),
                UpdateMediaObjectViewModel = new UpdateMediaObjectViewModel()
            };

            return View(mediaIndexViewModel);
        }

        //consider using a ViewModel here when more apis will be passing their own values here.
        public IActionResult Add(string imdbId, string title, string type)
        {

            AddMediaObjectViewModel addMediaObjectViewModel = new AddMediaObjectViewModel(context.SubTypes.ToList());

            //If I can't pass a whole object as a parameter here, I can send a request to the api, but that's a crappy solution.
            if (imdbId != null & title != null)
            {
                addMediaObjectViewModel.Title = title;
                addMediaObjectViewModel.ExternalId = imdbId;
                addMediaObjectViewModel.DatabaseSource = 1; //1 = IMDB

                addMediaObjectViewModel.SubTypeID = context.SubTypes.Where(t => t.Name == type).Single().ID;
            }

            return View(addMediaObjectViewModel);
        }

        [HttpPost]
        //[Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Add(AddMediaObjectViewModel addMediaObjectViewModel)
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);

            if (ModelState.IsValid)
            {
                MediaObject newMediaObject = new MediaObject
                {
                    Title = addMediaObjectViewModel.Title,
                    SubTypeID = addMediaObjectViewModel.SubTypeID,
                    MediaSubType = context.SubTypes.Single(s => s.ID == addMediaObjectViewModel.SubTypeID),
                    DatabaseSource = addMediaObjectViewModel.DatabaseSource,
                    Started = addMediaObjectViewModel.Started,
                    DateAdded = DateTime.Now, //.ToString("yyyy-MM-dd"), //preferred format
                    RecommendSource = addMediaObjectViewModel.RecommendSource,
                    OwnerId = userId,
                    ExternalId = addMediaObjectViewModel.ExternalId,
                    Interest = addMediaObjectViewModel.Interest
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
        //[Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int[] deletedIds)
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);

            foreach (int deletedId in deletedIds)
            {
                MediaObject theMedia = context.MediaObjects.Single(m => m.ID == deletedId);

                if (userId == theMedia.OwnerId)
                {
                    theMedia.Deleted = true;
                }

            }

            context.SaveChanges();

            return Redirect("/");
        }

        [HttpPost]
        public IActionResult Update(UpdateMediaObjectViewModel updateMediaObjectViewModel)
        {
            //ToDo: allow for text fields to be updated via form input.
            //ToDo: complete Update logic using ViewModel
            if (ModelState.IsValid)
            {
                int i = 0;

                //strip double values
                //started
                List<bool> startedBools = StripAndConvertIntArrayToListBool(updateMediaObjectViewModel.StartedValues);
                //completed
                List<bool> completedBools = StripAndConvertIntArrayToListBool(updateMediaObjectViewModel.CompletedValues);
                
                foreach (int ID in updateMediaObjectViewModel.MediaIDs)
                {
                    //find media object in database
                    MediaObject updateCandidate = context.MediaObjects.Single(m => m.ID == ID);
                    bool countUpdate = false;

                    string userId = Common.ExtensionMethods.getUserId(this.User);

                    if (userId == updateCandidate.OwnerId)
                    {
                        //check if Started value has changed, then add to UpdateCount
                        //compare existing value to new value
                        if (updateCandidate.Started != startedBools[i])
                        {
                            //then update
                            updateCandidate.Started = startedBools[i];
                            countUpdate = true;
                        }
                        //check if Completed value has changed
                        if (updateCandidate.Completed != completedBools[i])
                        {
                            //then update
                            updateCandidate.Completed = completedBools[i];
                            countUpdate = true;
                        }
                        //check if Interest value has changed
                        if (updateCandidate.Interest != updateMediaObjectViewModel.Interest[i])
                        {
                            //check if value is in range
                            if (1 <= updateMediaObjectViewModel.Interest[i] & updateMediaObjectViewModel.Interest[i] <= 10)
                            {
                                //then update
                                updateCandidate.Interest = updateMediaObjectViewModel.Interest[i];
                                countUpdate = true;
                            }
                        }
                        if (countUpdate)
                        {
                            updateCandidate.UpdateCount += 1;
                        }
                    }

                    i++;
                }
            }

            //Save changes
            context.SaveChanges();

            //Then check for deleted Ids and pass any to the DeletePrompt route
            if (updateMediaObjectViewModel.DeletedIDs != null)
            {
                List<MediaObject> mediaToDelete = new List<MediaObject>();
                mediaToDelete = ArrayIdsToListMediaObjects(updateMediaObjectViewModel.DeletedIDs);
                return View("DeletePrompt", mediaToDelete);
            }

            return Redirect("/");
        }
        
        //ToDo: Add Search action
        //ToDo: Modify layout Add link
        public IActionResult Search()
        {
            List<OMDbTitle> omdbTitles = new List<OMDbTitle>();

            return View(omdbTitles);
        }

        [HttpPost]
        public IActionResult Search(List<OMDbTitle> omdbTitles)
        {
            return View(omdbTitles);
        }

        public IActionResult RandomSuggestion()
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);

            //query database for all undeleted, uncompleted results belonging to the user.
            List<MediaObject> mediaObjects = context.MediaObjects.
                Where(u => u.OwnerId == userId).
                Where(d => d.Deleted == false).
                Where(c => c.Completed == false).
                ToList();
            List<MediaObject> randomMedia = new List<MediaObject>();
            int numSuggestion = 3;

            //write logic to get random object by index and add to list
            //for (int i = 0; i < numSuggestion; i++)
            while(randomMedia.Count < numSuggestion)
            {
                Random rand = new Random();
                int index = rand.Next(0, mediaObjects.Count);

                if (randomMedia.Contains(mediaObjects[index]) == false)
                {
                    randomMedia.Add(mediaObjects[index]);
                    mediaObjects[index].SuggestedCount += 1;
                }
            }

            context.SaveChanges();

            //pass objects to view
            return View(randomMedia);
        }

        public IActionResult SelectTitle(int ID)
        {
            //increment SelectedCount
            MediaObject selected = context.MediaObjects.Where(i => i.ID == ID).Single();
            selected.SelectedCount += 1;
            context.SaveChanges();

            //ToDo: add title details page
            //ToDo: redirect to details page matching given ID after selection
            return Redirect("/Media/Index");
        }

        //Methods

        public List<MediaObject> ArrayIdsToListMediaObjects(int[] arrayIds)
        {
            List<MediaObject> mediaList = new List<MediaObject>();

            foreach (int arrayId in arrayIds)
            {
                mediaList.Add(context.MediaObjects.Single(m => m.ID == arrayId));
            }
            return mediaList;
        }

        public List<int> StripDoubleValues(int[] values)
        {
            //Checkbox values come in from the form in 2 parts, a 0 for hidden input and 1 only if box is checked. A 0 followed by a 1 can be ignored. It is easier to deal with them if we strip the duplicate values.
            List<int> doubleValues = new List<int>(values);
            List<int> singleValues = new List<int>();

            int singleValueIndex = -1;

            for (int i = 0; i < doubleValues.Count; i++)
            {
                singleValues.Add(doubleValues[i]);
                singleValueIndex += 1;

                if (doubleValues[i] == 1)
                {
                    singleValues.RemoveAt(singleValueIndex - 1);
                    singleValueIndex -= 1;
                }
            }

            return singleValues;
        }

        public bool BinarytoBool(int binaryValue)
        {
            bool boolean = new bool();

            if (binaryValue == 0) { boolean = false; }
            else if (binaryValue == 1) { boolean = true; }
            else
            {
                //ToDo: write exception logic
            }

            return boolean;
        }

        public List<bool> ListBinaryToListBool(List<int> binaryValues)
        {
            List<bool> bools = new List<bool>();

            foreach (int binaryValue in binaryValues)
            {
                bools.Add(BinarytoBool(binaryValue));
            }

            return bools;
        }

        public List<bool> StripAndConvertIntArrayToListBool(int[] binaryValues)
        {
            //interprets form data
            //strip the values: outputs a list of ints
            List<int> strippedValues = StripDoubleValues(binaryValues);

            //convert to a list of bools
            List<bool> boolValues = ListBinaryToListBool(strippedValues);

            return boolValues;
        }


    }
}