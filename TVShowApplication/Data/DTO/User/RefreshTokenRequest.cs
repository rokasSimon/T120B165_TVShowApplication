using System.ComponentModel.DataAnnotations;

namespace TVShowApplication.Data.DTO.User
{
    public class RefreshTokenRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string AccessToken { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string RefreshToken { get; set; }
    }
}
