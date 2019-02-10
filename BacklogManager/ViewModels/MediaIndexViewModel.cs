using BacklogManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.ViewModels
{
    public class MediaIndexViewModel
    {
        public IList<MediaObject> MediaOjects { get; set; }
        public UpdateMediaObjectViewModel UpdateMediaObjectViewModel { get; set; }
    }
}
