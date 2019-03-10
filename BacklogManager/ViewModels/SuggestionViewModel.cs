using BacklogManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.ViewModels
{
    public class SuggestionViewModel
    {
        public IList<MediaObject> MediaObjects { get; set; }

        public SuggestionViewModel() { }
    }
}
