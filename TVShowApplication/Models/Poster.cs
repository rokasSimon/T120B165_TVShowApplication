namespace TVShowApplication.Models
{
    public class Poster : User
    {
        public new Role Role = Role.Poster;

        public ICollection<Series> PostedSeries { get; set; }
    }
}
