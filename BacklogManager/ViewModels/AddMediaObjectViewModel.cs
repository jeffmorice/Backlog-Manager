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
        [Required(ErrorMessage = "Please enter a title.")]
        public string Title { get; set; }
        [Required]
        [Display(Name =  "Type")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a media type.")]
        public int SubTypeID { get; set; }
        [Required]
        public int DatabaseSource { get; set; }
        [Display(Name = "Have you started this title?")]
        public bool Started { get; set; }
        [Display(Name = "How did you hear about it? Was it recommended to you?")]
        public string RecommendSource { get; set; }
        public string ExternalId { get; set; }
        [Range(1, 10, ErrorMessage = "Please enter an integer from 1-10.")]
        [Display(Name = "Rate your interest from 1 to 10 (from 'If I ever get around to it' to 'I literally can't wait').")]
        public int Interest { get; set; } = 10;    //10 is the default value, here and for the creation of media objects
        public string Image { get; set; }

        //ToDo: fix bug where select list is destroyed after invalid submission
        public List<SelectListItem> SubTypes { get; set; }

        public AddMediaObjectViewModel() { }

        public AddMediaObjectViewModel(List<SelectListItem> selectListItems)
        {
            SubTypes = selectListItems;
        }
    }
}
