using Microsoft.EntityFrameworkCore;

namespace TVShowApplication.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Series> Videos { get; set; }
    }
}
