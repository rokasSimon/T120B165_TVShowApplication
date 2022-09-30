using System.ComponentModel.DataAnnotations;

namespace TVShowApplication.Data.DTO.User
{
    public class SignInRequest
    {
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
