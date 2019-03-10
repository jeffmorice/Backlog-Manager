using BacklogManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.ViewModels
{
    public class MediaBySubTypeViewModel
    {
        public SubType MediaSubType { get; set; }
        public List<MediaObject> MediaObjects { get; set; }

        public MediaBySubTypeViewModel() { }
    }
}
