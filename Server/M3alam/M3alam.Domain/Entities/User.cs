
using System;

namespace M3alam.Domain.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Token { get; set; }

        public void Login() { /* login logic */ }
        public void Logout() { /* logout logic */ }
    }
}
