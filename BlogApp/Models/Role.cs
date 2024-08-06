﻿namespace BlogApp.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
