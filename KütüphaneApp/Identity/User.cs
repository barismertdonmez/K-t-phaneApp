using Microsoft.AspNetCore.Identity;

namespace KütüphaneApp.Identity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
