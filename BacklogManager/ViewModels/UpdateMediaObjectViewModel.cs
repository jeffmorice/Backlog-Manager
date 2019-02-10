using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.ViewModels
{
    public class UpdateMediaObjectViewModel
    {
        public int[] MediaIDs { get; set; } //VS throws exception when rendering the index page, which doesn't reference this ViewModel, without a primary key. Likely an error. This is a dummy field.
        public int[] StartedValues { get; set; }
        public int[] CompletedValues { get; set; }
        public int[] DeletedIDs { get; set; }

        public UpdateMediaObjectViewModel() { }
    }
}
