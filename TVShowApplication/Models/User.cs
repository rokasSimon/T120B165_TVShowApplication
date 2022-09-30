namespace TVShowApplication.Models
{
    public class User
    {
        public const string Role = nameof(User);

        public int Id { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
