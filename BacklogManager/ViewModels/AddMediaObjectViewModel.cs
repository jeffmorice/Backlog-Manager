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
        [Display(Name ="Type")]
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

        //ToDo: fix bug where select list is destroyed after invalid submission
        public List<SelectListItem> SubTypes { get; set; }

        public AddMediaObjectViewModel() { }
        
        public AddMediaObjectViewModel(IEnumerable<SubType> subTypes)
        {
            //this list is the accumulator
            SubTypes = new List<SelectListItem>();

            //Make a copy of the IEnumerable as a list
            List<SubType> subTypesList = new List<SubType>(subTypes);

            //Make a copy of the complete list and use it like a de-accumulator to remove already appended IDs.
            List<SubType> remainingSubTypes = new List<SubType>(subTypes);

            //for each item in list of remaining subtypes
            //constructs select list with 3 level hierarchy: Parent > Child > GrandChild
            while (remainingSubTypes.Count > 0)
            {
                SubType subType = remainingSubTypes[0];

                //add them to the accumulator
                SubTypes.Add(new SelectListItem
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
                        SubTypes.Add(new SelectListItem
                        {
                            Value = potentialChild.ID.ToString(),
                            Text = "--" + potentialChild.Name
                        });
                        //2) remove subtype from remaining list
                        remainingSubTypes.Remove(potentialChild);

                        foreach (SubType potentialGrandChild in subTypesList)
                        {
                            if (potentialGrandChild.ParentID == potentialChild.ID)
                            {
                                SubTypes.Add(new SelectListItem
                                {
                                    Value = potentialGrandChild.ID.ToString(),
                                    Text = "----" + potentialGrandChild.Name
                                });
                                
                                remainingSubTypes.Remove(potentialGrandChild);
                            }
                        }
                    }
                }
            }
        }
    }
}
