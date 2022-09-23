namespace TVShowApplication.Models
{
    public class Poster : User
    {
        public new const string Role = nameof(Poster);

        public ICollection<Series> PostedSeries { get; set; }
    }
}
