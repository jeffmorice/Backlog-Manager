using BacklogManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.ViewModels
{
    public class SearchBacklogViewModel
    {
        public int ID { get; set; }

        //Search terms
        public string Title { get; set; }
        [Display(Name = "Type")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value greater than 0.")]
        public int SubTypeID { get; set; }
        [Display(Name = "Source Database")]
        public int DatabaseSource { get; set; }
        public int Started { get; set; }
        public int Completed { get; set; }
        [Display(Name = "Recommendation Source")]
        public string RecommendSource { get; set; }
        [Display(Name = "Interest Level")]
        public int Interest { get; set; }
        public int InterestOperator { get; set; }
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }
        [Display(Name = "Date Last Suggested")]
        public DateTime LastSuggested { get; set; }

        public string Search { get; set; } = null;

        public List<SelectListItem> SubTypes { get; set; }

        public List<MediaObject> MediaObjects { get; set; }

        public SearchBacklogViewModel() { }

        public SearchBacklogViewModel(List<SelectListItem> selectListItems)
        {
            SubTypes = selectListItems;
        }
    }
}
