using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.ViewModels
{
    public class EditSubTypeViewModel
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ParentID { get; set; }
    }
}
