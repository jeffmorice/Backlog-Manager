using BacklogManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.ViewModels
{
    public class AddMediaObjectViewModel
    {
        public int ID { get; set; } //VS throws exception when rendering the index page, which doesn't reference this ViewModel, without a primary key. Likely an error. This is a dummy field.
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name ="Media Sub-Type")]
        public int SubTypeID { get; set; }
        [Required]
        public int DatabaseSource { get; set; }
        public bool Started { get; set; }
        public string RecommendSource { get; set; }

        public List<SelectListItem> SubTypes { get; set; }

        public AddMediaObjectViewModel() { }

        public AddMediaObjectViewModel(IEnumerable<SubType> subTypes)
        {
            SubTypes = new List<SelectListItem>();

            //ToDo: change this code to create a more organized dropdown list
            foreach (SubType subType in subTypes)
            {
                SubTypes.Add(new SelectListItem
                {
                    Value = subType.ID.ToString(),
                    Text = subType.Name
                });
            }
        }

        
    }
}
