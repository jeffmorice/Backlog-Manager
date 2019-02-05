using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.Models
{
    public class SubType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }

        IList<MediaObject> MediaObjects { get; set; }
    }
}
