using System.Collections.Generic;

namespace Models.DTOs
{
    public class MovieResponse
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public List<string> Genres { get; set; }
        public string PosterImg { get; set; }
    }
}
