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
using Microsoft.AspNetCore.Mvc.Rendering;

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

        //Debug controls
        private bool testSuggestions = false;
        private int numTest = 10000;

        //Actions

        public IActionResult Index(int subTypeId)
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);

            if (userId == null)
            {
                return Redirect("/Home/Index");
            }

            List<MediaBySubTypeViewModel> mediaBySubTypeViewModels = new List<MediaBySubTypeViewModel>();
            List<SubType> subTypes = new List<SubType>();
            int incompleteTitles = 0;

            if (subTypeId != 0)
            {
                subTypes = context.SubTypes.Where(s => s.ID == subTypeId).ToList();
            }
            else
            {
                subTypes = context.SubTypes.ToList();
            }

            foreach (SubType subType in subTypes)
            {
                List<MediaObject> mediaObjects = context.MediaObjects.
                    Include(s => s.MediaSubType).
                    Where(u => u.OwnerId == userId).
                    Where(d => d.Deleted == false).
                    Where(sti => sti.SubTypeID == subType.ID).
                    ToList();
                int incompleteBySubType = mediaObjects.Where(c => c.Completed == false).Count();
                incompleteTitles += incompleteBySubType;

                MediaBySubTypeViewModel mediaBySubTypeViewModel = new MediaBySubTypeViewModel
                {
                    MediaSubType = subType,
                    MediaObjects = mediaObjects,
                    BacklogCount = incompleteBySubType
                };

                if (mediaBySubTypeViewModel.MediaObjects.Count() != 0)
                {
                    mediaBySubTypeViewModels.Add(mediaBySubTypeViewModel);
                }
            }

            MediaIndexViewModel mediaIndexViewModel = new MediaIndexViewModel
            {
                MediaBySubTypeViewModels = mediaBySubTypeViewModels,
                UpdateMediaObjectViewModel = new UpdateMediaObjectViewModel(),
                BacklogCount = incompleteTitles
            };

            return View(mediaIndexViewModel);
        }

        //consider using a ViewModel here when more apis will be passing their own values here.
        public IActionResult Add(string imdbId, string title, string type, string image)
        {

            AddMediaObjectViewModel addMediaObjectViewModel = new AddMediaObjectViewModel(GetSelectListSubTypes());

            if (imdbId != null & title != null)
            {
                addMediaObjectViewModel.Title = title;
                addMediaObjectViewModel.ExternalId = imdbId;
                addMediaObjectViewModel.DatabaseSource = 1; //1 = IMDB
                addMediaObjectViewModel.SubTypeID = context.SubTypes.Where(t => t.Name == type).Single().ID;
                addMediaObjectViewModel.Image = image;
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
                    Interest = addMediaObjectViewModel.Interest,
                    Image = addMediaObjectViewModel.Image
                };

                context.MediaObjects.Add(newMediaObject);
                context.SaveChanges();

                return Redirect("/");
            }

            //in the event of invalid ModelState
            addMediaObjectViewModel.SubTypes = GetSelectListSubTypes();

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
        [ValidateAntiForgeryToken]
        public IActionResult Update(UpdateMediaObjectViewModel updateMediaObjectViewModel)
        {
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
                    bool countUpdate = false;
                    string userId = Common.ExtensionMethods.getUserId(this.User);
                    MediaObject updateCandidate = context.MediaObjects.
                        Where(u => u.OwnerId == userId).
                        Single(m => m.ID == ID);

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
                            //check if new value is > existing value, if not don't count the update
                            if (updateMediaObjectViewModel.Interest[i] > updateCandidate.Interest) { countUpdate = true; }
                        }
                    }
                    if (countUpdate) { updateCandidate.UpdateCount += 1; }

                    i++;
                }
            }

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

        public IActionResult SearchImdb()
        {
            List<OMDbTitle> omdbTitles = new List<OMDbTitle>();

            return View(omdbTitles);
        }
        
        public IActionResult RandomSuggestion(int id)
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);

            //query database for all undeleted, uncompleted results belonging to the user not yet suggested today.
            List<MediaObject> mediaObjects = context.MediaObjects.
                Include(s => s.MediaSubType).
                Where(u => u.OwnerId == userId).
                Where(d => d.Deleted == false).
                Where(c => c.Completed == false).
                Where(l => int.Parse(l.LastSuggested.ToString("yyyyMMdd")) < int.Parse(DateTime.Now.ToString("yyyyMMdd"))).
                ToList();
            List<MediaObject> randomMedia = new List<MediaObject>();

            if (id != 0)
            {
                mediaObjects = mediaObjects.Where(i => i.SubTypeID == id).ToList();
            }

            //SuggestionViewModel suggestionViewModel = new SuggestionViewModel(context.SubTypes.ToList());
            int numSuggestion = 3;

            //ensure the while loop does not continue needlessly when too few items in list
            if (mediaObjects.Count <= numSuggestion)
            {
                foreach (MediaObject mediaObject in mediaObjects)
                {
                    mediaObject.SuggestedCount += 1;
                    mediaObject.LastSuggested = DateTime.Now;
                }
                context.SaveChanges();

                SuggestionViewModel suggestionViewModelB = new SuggestionViewModel { MediaObjects = mediaObjects };

                return View(suggestionViewModelB);
            }
            else
            {
                while (randomMedia.Count < numSuggestion)
                {
                    Random rand = new Random();
                    int index = rand.Next(0, mediaObjects.Count);
                    MediaObject theMedia = mediaObjects[index];

                    if (randomMedia.Contains(theMedia) == false)
                    {
                        randomMedia.Add(theMedia);
                        theMedia.SuggestedCount += 1;
                        theMedia.LastSuggested = DateTime.Now;
                    }
                }
            }

            context.SaveChanges();

            SuggestionViewModel suggestionViewModelA = new SuggestionViewModel { MediaObjects = randomMedia };

            return View(suggestionViewModelA);
        }

        public IActionResult WeightedSuggestion(int id)
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);

            //query database for all undeleted, uncompleted results belonging to the user not yet suggested today.
            List<MediaObject> mediaObjects = context.MediaObjects.
                Include(s => s.MediaSubType).
                Where(u => u.OwnerId == userId).
                Where(d => d.Deleted == false).
                Where(c => c.Completed == false).
                Where(l => int.Parse(l.LastSuggested.ToString("yyyyMMdd")) < int.Parse(DateTime.Now.ToString("yyyyMMdd"))).
                ToList();
            List<MediaObject> weightedRandomMedia = new List<MediaObject>();

            if (id != 0)
            {
                mediaObjects = mediaObjects.Where(i => i.SubTypeID == id).ToList();
            }

            int numSuggestion = 3;
            bool ignoreDoubles = true;

            //debug Suggestions
            if (testSuggestions == true)
            {
                ResetSuggestedCount(mediaObjects);
                //numSuggestion = numTest;
                //ignoreDoubles = false;
            }

            int numTests = 0;
            //loop for debugging
            for (int i = 0; i < numTest; i++)
            {
                //ensure the while loop does not continue needlessly when too few items in list
                if (mediaObjects.Count <= numSuggestion)
                {
                    foreach (MediaObject mediaObject in mediaObjects)
                    {
                        mediaObject.SuggestedCount += 1;
                        mediaObject.LastSuggested = DateTime.Now;
                    }
                    context.SaveChanges();

                    SuggestionViewModel suggestionViewModelB = new SuggestionViewModel { MediaObjects = mediaObjects };

                    return View(suggestionViewModelB);
                }
                else
                {
                    while (weightedRandomMedia.Count < numSuggestion)
                    {
                        Random rand = new Random();
                        int index = rand.Next(0, mediaObjects.Count);
                        MediaObject theMedia = mediaObjects[index];

                        //ignore doubles
                        if (weightedRandomMedia.Contains(theMedia) == false || ignoreDoubles == false)
                        {
                            int interestMax = 10;
                            int updateThreshold = 3;  //caps influence of UpdateCount (measurement of engagement)

                            //compute probability range
                            int baseRange = interestMax + updateThreshold;
                            int updateFactor = theMedia.UpdateCount;
                            int suggestedFactor = theMedia.SuggestedCount / numSuggestion;

                            if (testSuggestions == true) { suggestedFactor = theMedia.SuggestedCount / 3; }

                            //control for maximum suggestedFactor
                            if (suggestedFactor > interestMax) { suggestedFactor = interestMax; }

                            //control for maximum updateFactor
                            if (updateFactor > updateThreshold) { updateFactor = updateThreshold; }

                            int newRange = baseRange - updateFactor + suggestedFactor - theMedia.SelectedCount - theMedia.Interest;

                            //if newRange is <= 1, there is a 100% probability it will be chosen
                            if (newRange <= 1)
                            {
                                weightedRandomMedia.Add(theMedia);
                                theMedia.SuggestedCount += 1;
                                theMedia.LastSuggested = DateTime.Now;
                            }
                            else
                            {
                                //run random with new range
                                Random prob = new Random();
                                int result = prob.Next(1, newRange + 1);

                                //if result == 1, then add it to list
                                if (result == 1)
                                {
                                    weightedRandomMedia.Add(theMedia);
                                    theMedia.SuggestedCount += 1;
                                    theMedia.LastSuggested = DateTime.Now;
                                }
                            }
                        }
                    }
                }

                //check if debug
                if (testSuggestions == false) { break; }
                else
                {
                    numTests += 1;
                    weightedRandomMedia = new List<MediaObject>();
                }
            }

            context.SaveChanges();

            SuggestionViewModel suggestionViewModelA = new SuggestionViewModel { MediaObjects = weightedRandomMedia };

            return View(suggestionViewModelA);
        }

        public IActionResult SearchBacklog(SearchBacklogViewModel searchTerms, int subTypeId)
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);
            searchTerms.SubTypes = GetSelectListSubTypes();
            searchTerms.SubTypes[0].Text = "All";
            List<MediaObject> searchResults = new List<MediaObject>();

            if (searchTerms.Search != null) //runs no matter what
            {
                searchResults = context.MediaObjects.
                    Where(u => u.OwnerId == userId).
                    Where(d => d.Deleted == false).
                    ToList();

                //Refine searchResults
                //Title - search for similar results
                if (searchTerms.Title != null)
                {
                    searchResults = searchResults.Where(t => t.Title.ToLower().Contains(searchTerms.Title.ToLower())).ToList();
                }
                //SubType - 0 = All
                if (searchTerms.SubTypeID != 0)
                {
                    List<int> childIds = GetChildTypesRecursive(searchTerms.SubTypeID);
                    List<MediaObject> mediaObjects = new List<MediaObject>();
                    
                    mediaObjects = searchResults.Where(t => t.SubTypeID == searchTerms.SubTypeID).ToList();

                    foreach (int childId in childIds)
                    {
                        List<MediaObject> childMediaObjects = new List<MediaObject>();
                        childMediaObjects = searchResults.Where(t => t.SubTypeID == childId).ToList();

                        foreach (MediaObject childMediaObject in childMediaObjects)
                        {
                            mediaObjects.Add(childMediaObject);
                        }
                    }
                    searchResults = mediaObjects;
                }
                //Database - 0 = All
                if (searchTerms.DatabaseSource != 0)
                {
                    //ToDo: fix hardcoded solution, whereby form value is offset for display purposes and corrected here
                    searchResults = searchResults.Where(t => t.DatabaseSource == searchTerms.DatabaseSource - 1).ToList();
                }
                //Completed - 0 = All
                if (searchTerms.Completed != 0)
                {
                    if (searchTerms.Completed == 1)
                    {
                        searchResults = searchResults.Where(t => t.Completed == true).ToList();
                    }
                    if (searchTerms.Completed == 2)
                    {
                        searchResults = searchResults.Where(t => t.Completed == false).ToList();
                    }
                }
                //Started - 0 = All
                if (searchTerms.Started != 0)
                {
                    if (searchTerms.Started == 1)
                    {
                        searchResults = searchResults.Where(t => t.Started == true).ToList();
                    }
                    if (searchTerms.Started == 2)
                    {
                        searchResults = searchResults.Where(t => t.Started == false).ToList();
                    }
                }
                ////Date Added - check if before or after (string to int, compare greater than, less than, or equal to)
                ////create property to hold this value
                //if (int.Parse(searchTerms.DateAdded.ToString("yyyyMMdd")) != 00010101)
                //{
                //    //earlier than given date
                //    searchResults = searchResults.Where(t => int.Parse(t.DateAdded.ToString("yyyyMMdd")) > int.Parse(searchTerms.DateAdded.ToString("yyyyMMdd"))).ToList();
                //}
                //Recommendation Source
                if (searchTerms.RecommendSource != null)
                {
                    //searchResults = searchResults.Where(t => t.RecommendSource.ToString().ToLower().Contains(searchTerms.RecommendSource.ToLower())).ToList();
                    List<MediaObject> refinedResults = new List<MediaObject>();
                    foreach (MediaObject mediaObject in searchResults)
                    {
                        if (mediaObject.RecommendSource != null)
                        {
                            if (mediaObject.RecommendSource.ToLower().Contains(searchTerms.RecommendSource.ToLower()))
                            {
                                refinedResults.Add(mediaObject);
                            }
                        }
                    }
                    searchResults = refinedResults;
                }
                //Interest - 0-10
                if (1 <= searchTerms.Interest & searchTerms.Interest <= 10)
                {
                    //check for equal to, greater than, less than, +or - 1
                    if (searchTerms.InterestOperator == 0)
                    {
                        searchResults = searchResults.Where(t => t.Interest == searchTerms.Interest).ToList();
                    }
                    if (searchTerms.InterestOperator == 1)
                    {
                        searchResults = searchResults.Where(t => t.Interest > searchTerms.Interest).ToList();
                    }
                    if (searchTerms.InterestOperator == 2)
                    {
                        searchResults = searchResults.Where(t => t.Interest < searchTerms.Interest).ToList();
                    }
                    if (searchTerms.InterestOperator == 3)
                    {
                        searchResults = searchResults.Where(t => t.Interest == searchTerms.Interest || t.Interest == searchTerms.Interest + 1 || t.Interest == searchTerms.Interest - 1).ToList();
                    }
                }
                ////Date Last Suggested - check if before or after
                //if (int.Parse(searchTerms.LastSuggested.ToString("yyyyMMdd")) != 00010101)
                //{
                //    searchResults = searchResults.Where(t => t.LastSuggested == searchTerms.LastSuggested).ToList();
                //}

                
            }
            searchTerms.MediaObjects = searchResults;

            return View(searchTerms);
        }

        public IActionResult SelectTitle(int ID)
        {
            //increment SelectedCount
            MediaObject selected = context.MediaObjects.Where(i => i.ID == ID).Single();
            selected.SelectedCount += 1;
            context.SaveChanges();
            
            //ToDo: redirect to details page matching given ID after selection
            return Redirect("/Media/Index");
        }

        public async Task<IActionResult> Details(int ID)
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);
            MediaObject mediaObject = context.MediaObjects.
                Include(s => s.MediaSubType).
                Where(u => u.OwnerId == userId).
                SingleOrDefault(o => o.ID == ID);

            if (mediaObject != null)
            {
                EditMediaObjectViewModel editMediaObjectViewModel = new EditMediaObjectViewModel(mediaObject, GetSelectListSubTypes());

                if (mediaObject.DatabaseSource == 1)
                {
                    OMDbTitle omdbTitle = await GetImdbData(mediaObject.ExternalId);
                    editMediaObjectViewModel.OMDbTitle = omdbTitle;
                }
                
                return View(editMediaObjectViewModel);
            }

            return Redirect("/Media/Index");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditMediaObjectViewModel editMediaObjectViewModel)
        {
            if (ModelState.IsValid)
            {
                string userId = Common.ExtensionMethods.getUserId(this.User);
                bool countUpdate = false;
                MediaObject editCandidate = context.MediaObjects.
                    Where(u => u.OwnerId == userId).
                    Where(i => i.ID == editMediaObjectViewModel.ID).
                    SingleOrDefault();

                //compare existing values to new values
                if (editCandidate.Title != editMediaObjectViewModel.Title)
                {
                    editCandidate.Title = editMediaObjectViewModel.Title;
                    countUpdate = true;
                }
                if (editCandidate.SubTypeID != editMediaObjectViewModel.SubTypeID)
                {
                    editCandidate.SubTypeID = editMediaObjectViewModel.SubTypeID;
                    countUpdate = true;
                }
                if (editCandidate.RecommendSource != editMediaObjectViewModel.RecommendSource)
                {
                    editCandidate.RecommendSource = editMediaObjectViewModel.RecommendSource;
                    countUpdate = true;
                }
                //check if Interest value has changed
                if (editCandidate.Interest != editMediaObjectViewModel.Interest)
                {
                    //check if value is in range
                    if (1 <= editMediaObjectViewModel.Interest & editMediaObjectViewModel.Interest <= 10)
                    {
                        //check if new value is > existing value, if not don't count the update
                        if (editMediaObjectViewModel.Interest > editCandidate.Interest) { countUpdate = true; }
                        editCandidate.Interest = editMediaObjectViewModel.Interest;
                    }
                }
                if (editCandidate.Image != editMediaObjectViewModel.Image)
                {
                    editCandidate.Image = editMediaObjectViewModel.Image;
                    countUpdate = true;
                }

                //strip double values
                //started
                List<bool> startedBools = StripAndConvertIntArrayToListBool(editMediaObjectViewModel.StartedValue);
                //completed
                List<bool> completedBools = StripAndConvertIntArrayToListBool(editMediaObjectViewModel.CompletedValue);

                //check if Started value has changed, then add to UpdateCount
                if (editCandidate.Started != startedBools[0])
                {
                    editCandidate.Started = startedBools[0];
                    countUpdate = true;
                }
                //check if Completed value has changed
                if (editCandidate.Completed != completedBools[0])
                {
                    editCandidate.Completed = completedBools[0];
                    countUpdate = true;
                }
                if (countUpdate) { editCandidate.UpdateCount += 1; }

                context.SaveChanges();
            }

            //Then check for deleted Id and pass any to the DeletePrompt route
            if (editMediaObjectViewModel.DeletedIDs != null)
            {
                List<MediaObject> mediaToDelete = new List<MediaObject>();
                mediaToDelete = ArrayIdsToListMediaObjects(editMediaObjectViewModel.DeletedIDs);
                return View("DeletePrompt", mediaToDelete);
            }

            return Redirect("/Media/Details?id=" + editMediaObjectViewModel.ID);
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

        private void ResetSuggestedCount(List<MediaObject> mediaObjects)
        {
            foreach (MediaObject mediaObject in mediaObjects)
            {
                mediaObject.SuggestedCount = 0;

                //Random rand = new Random();
                //mediaObject.UpdateCount = rand.Next(0, 4);
            }
            context.SaveChanges();
        }

        public async Task<OMDbTitle> GetImdbData(string externalId)
        {
            OMDbController omdbController = new OMDbController();
            OMDbTitle omdbTitle = await omdbController.GetById(externalId);

            return omdbTitle;
        }

        public List<int> GetChildTypesRecursive(int parentId, List<int> childIds)
        {
            List<SubType> childTypes = context.SubTypes.Where(s => s.ParentID == parentId).ToList();

            foreach (SubType childType in childTypes)
            {
                childIds.Add(childType.ID);
                childIds = GetChildTypesRecursive(childType.ID, childIds);
            }

            return childIds;
        }

        public List<int> GetChildTypesRecursive(int parentId)
        {
            List<SubType> childTypes = context.SubTypes.Where(s => s.ParentID == parentId).ToList();
            List<int> childIds = new List<int>();

            foreach (SubType childType in childTypes)
            {
                childIds.Add(childType.ID);
                childIds = GetChildTypesRecursive(childType.ID, childIds);
            }

            return childIds;
        }

        //Admin Debug actions

        public IActionResult ResetSuggestions()
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);

            List<MediaObject> mediaObjects = context.MediaObjects.
                Where(u => u.OwnerId == userId).
                ToList();

            foreach (MediaObject mediaObject in mediaObjects)
            {
                mediaObject.SuggestedCount = 0;
                mediaObject.LastSuggested = new DateTime(0001, 01, 01);
            }

            context.SaveChanges();

            return Redirect("/Media/Index");
        }

        public async Task<IActionResult> GetImagesImdb()
        {
            string userId = Common.ExtensionMethods.getUserId(this.User);
            OMDbController omdbController = new OMDbController();
            List<MediaObject> mediaObjects = context.MediaObjects.
                    Where(u => u.OwnerId == userId).
                    Where(d => d.DatabaseSource == 1).
                    Where(img => img.Image == null).
                    ToList();

            foreach (MediaObject mediaObject in mediaObjects)
            {
                OMDbTitle omdbTitle = await omdbController.GetById(mediaObject.ExternalId);

                if (omdbTitle.Poster != "N/A" & omdbTitle.Poster != null)
                {
                    mediaObject.Image = omdbTitle.Poster;
                }
            }

            context.SaveChanges();

            return Redirect("/Media/Index");
        }

        public List<SelectListItem> GetSelectListSubTypes()
        {
            List<SubType> subTypes = context.SubTypes.ToList();

            //this list is the accumulator and the return value
            List<SelectListItem> subTypeSelectItems = new List<SelectListItem>();
            subTypeSelectItems.Add(new SelectListItem
            {
                Value = "0",
                Text = "------"
            });

            //Make a copy of the IEnumerable as a list
            List<SubType> subTypesList = new List<SubType>(subTypes);

            //Make a copy of the complete list and use it like a de-accumulator to remove already appended IDs.
            List<SubType> remainingSubTypes = new List<SubType>(subTypes);

            //for each item in list of remaining subtypes
            //constructs select list with 3 level hierarchy: Parent > Child > GrandChild
            //ToDo: improve with recursive approach
            while (remainingSubTypes.Count > 0)
            {
                SubType subType = remainingSubTypes[0];

                //add them to the accumulator
                subTypeSelectItems.Add(new SelectListItem
                {
                    Value = subType.ID.ToString(),
                    Text = subType.Name
                });

                //remove from the de-accumulator
                remainingSubTypes.Remove(subType);

                foreach (SubType potentialChild in subTypesList)
                {
                    if (potentialChild.ParentID == subType.ID)
                    {
                        //When found:
                        //1) append subtypes to the accumulator list with some annotation like "--" to place them beneath each other.
                        subTypeSelectItems.Add(new SelectListItem
                        {
                            Value = potentialChild.ID.ToString(),
                            Text = "-- " + potentialChild.Name
                        });
                        //2) remove subtype from remaining list
                        remainingSubTypes.Remove(potentialChild);

                        foreach (SubType potentialGrandChild in subTypesList)
                        {
                            if (potentialGrandChild.ParentID == potentialChild.ID)
                            {
                                subTypeSelectItems.Add(new SelectListItem
                                {
                                    Value = potentialGrandChild.ID.ToString(),
                                    Text = "---- " + potentialGrandChild.Name
                                });

                                remainingSubTypes.Remove(potentialGrandChild);
                            }
                        }
                    }
                }
            }
            return subTypeSelectItems;
        }
    }
}