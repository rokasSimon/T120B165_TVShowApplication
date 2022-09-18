namespace TVShowApplication.Models
{
    public class Poster : User
    {
        public ICollection<Series> PostedSeries { get; set; }
    }
}
