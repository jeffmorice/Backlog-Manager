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
        [Required]
        public string Title { get; set; }
        [Required]
        public int MediaType { get; set; }
        [Required]
        public int DatabaseSource { get; set; }
        public bool Started { get; set; }
        public string RecommendSource { get; set; }

        public AddMediaObjectViewModel() { }
    }
}
