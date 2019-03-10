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
        //I imagine this class can be used as a ViewModel and pass relevant details (like IMDB ID) to new MediaObjects (MVP), but later may also be used to display details of a particular title on its individual page.

        //so far, pertinent key values common to all IMDB titles are Title, imdbID, and Type (movie, series, or episode)
        //these values can be passed to new MediaObjects (probably through the existing ViewModel)

        [Key]
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
        public string Poster { get; set; }
    }
}
