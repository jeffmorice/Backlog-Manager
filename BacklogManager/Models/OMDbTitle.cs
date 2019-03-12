using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogManager.Models
{
    public class OMDbTitle
    {
        //This class is for use in deserializing JSON data from OMDb.
        //Objects of this class are not meant to be stored in the database.

        //This data is passed to the AddMediaObjectViewModel
        [Key]
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }

        //This data is for display purposes only
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Runtime { get; set; }
        public string Rated { get; set; }
        public string Genre { get; set; }
        public string Year { get; set; }
        public string totalSeasons { get; set; }
        public string Season { get; set; }
        public string Episode { get; set; }
    }
}
