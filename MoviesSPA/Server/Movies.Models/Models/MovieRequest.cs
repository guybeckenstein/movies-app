using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Movies.Models.Models
{
    public class MovieRequest : IValidatableObject
    {
        [Required]
        public string Title { get; set; }
        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "The year must be a string with the exact length of 4")]
        public string Year { get; set; }
        [Required]
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        [Required]
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        [Required]
        public string Poster { get; set; }
        public string ImdbRating { get; set; }
        public string ImdbVotes { get; set; }
        public string ImdbID { get; set; }
        public string Type { get; set; }
        public string Response { get; set; }
        public List<string> Images { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Released.Contains(Year))
            {
                yield return new ValidationResult("Release date attribute must have same year", new[] { nameof(Released) });
            }
        }
    }
}
