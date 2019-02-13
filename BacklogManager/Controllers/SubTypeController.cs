using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BacklogManager.Data;
using BacklogManager.Models;
using BacklogManager.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BacklogManager.Controllers
{
    [Authorize(Roles= "Administrator")]
    public class SubTypeController : Controller
    {
        private readonly MediaObjectDbContext context;

        public SubTypeController(MediaObjectDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<SubType> subTypeList = context.SubTypes.ToList();

            return View(subTypeList);
        }

        public IActionResult Add()
        {
            AddSubTypeViewModel addSubTypeViewModel = new AddSubTypeViewModel();

            return View(addSubTypeViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddSubTypeViewModel addSubTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                SubType newSubType = new SubType
                {
                    Name = addSubTypeViewModel.Name,
                    ParentID = addSubTypeViewModel.ParentID
                };

                context.SubTypes.Add(newSubType);
                context.SaveChanges();

                return Redirect("/subtype");
            }
            return View(addSubTypeViewModel);
        }

        // GET: Reference/Delete/5
        public ActionResult DeletePrompt(int id)
        {
            SubType theSubType = context.SubTypes.Single(m => m.ID == id);

            return View(theSubType);
        }

        // POST: Reference/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                SubType theMedia = context.SubTypes.Single(m => m.ID == id);

                context.SubTypes.Remove(theMedia);

                context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(id);
            }
        }

        public ActionResult Edit(int id)
        {
            EditSubTypeViewModel editSubTypeViewModel = new EditSubTypeViewModel();

            SubType theSubType = context.SubTypes.Single(m => m.ID == id);

            editSubTypeViewModel.Name = theSubType.Name;
            editSubTypeViewModel.ParentID = theSubType.ParentID; 

            return View(editSubTypeViewModel);
        }

        // POST: Reference/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditSubTypeViewModel editSubTypeViewModel)
        {
            try
            {
                SubType theSubType = context.SubTypes.Single(m => m.ID == editSubTypeViewModel.ID);

                theSubType.Name = editSubTypeViewModel.Name;
                theSubType.ParentID = editSubTypeViewModel.ParentID;

                context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(editSubTypeViewModel);
            }
        }
    }
}
