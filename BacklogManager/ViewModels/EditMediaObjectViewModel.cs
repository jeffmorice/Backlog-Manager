using BacklogManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.ViewModels
{
    public class EditMediaObjectViewModel
    {
        public int ID { get; set; }

        //editable fields
        [Required]
        public string Title { get; set; }
        [NotMapped]
        public SubType MediaSubType { get; set; }
        [Required]
        public int SubTypeID { get; set; }
        public int[] CompletedValue { get; set; }
        public int[] StartedValue { get; set; }
        public int[] DeletedIDs { get; set; }
        public string RecommendSource { get; set; }
        [Range(1, 10)]
        public int Interest { get; set; }
        public string Image { get; set; } //allow user to force custom image

        //display only fields "stats"
        [NotMapped]
        public MediaObject MediaObject { get; set; }
        public List<SelectListItem> SubTypes { get; set; }

        public EditMediaObjectViewModel() { }

        public EditMediaObjectViewModel(MediaObject mediaObject, List<SelectListItem> selectListItems)
        {
            ID = mediaObject.ID;
            Title = mediaObject.Title;
            MediaSubType = mediaObject.MediaSubType;
            SubTypeID = mediaObject.SubTypeID;
            RecommendSource = mediaObject.RecommendSource;
            Interest = mediaObject.Interest;
            Image = mediaObject.Image;
            
            SubTypes = selectListItems;
            MediaObject = mediaObject;
        }
    }
}
