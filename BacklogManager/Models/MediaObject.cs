using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.Models
{
    public class MediaObject
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public SubType MediaSubType { get; set; }
        [Required]
        public int SubTypeID { get; set; }
        [Required]
        public int DatabaseSource { get; set; }
        public bool Completed { get; set; } = false;
        public bool Started { get; set; }
        public bool Deleted { get; set; } = false;
        public int UpdateCount { get; set; } = 0;
        [Column(TypeName="date")]
        public DateTime DateAdded { get; set; }
        //[Column(TypeName="time")]
        //public DateTime TimeAdded { get; set; }
        public string RecommendSource { get; set; }
        public string OwnerId { get; set; }
        public string ExternalId { get; set; }
        public int SuggestedCount { get; set; }
        public int SelectedCount { get; set; }
        [Range(1,10)]
        public int Interest { get; set; } = 10;
        [Column(TypeName = "date")]
        public DateTime LastSuggested { get; set; }

        public MediaObject() { }
    }
}