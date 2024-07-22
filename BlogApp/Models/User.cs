namespace BlogApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public int RoleId { get; set; }
        public string Password { get; set; }

        public Role Role { get; set; }
        public ICollection<Article> Articles { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
