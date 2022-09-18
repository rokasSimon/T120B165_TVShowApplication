namespace TVShowApplication.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Series> Videos { get; set; }
    }
}
