using BacklogManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.ViewModels
{
    public class MediaIndexViewModel
    {
        public IList<MediaBySubTypeViewModel> MediaBySubTypeViewModels { get; set; }
        public UpdateMediaObjectViewModel UpdateMediaObjectViewModel { get; set; }
        public int BacklogCount { get; set; }

        public MediaIndexViewModel() { }
    }
}
