namespace Api.DTOs
{
    public class MovieResponse
    {
        public required string Title { get; set; }
        public required string Year { get; set; }
        public required string Genres { get; set; }
        public required string Poster { get; set; }
        //public List<string> Genres { get; set; }
        //public string PosterImg { get; set; }
    }
}
