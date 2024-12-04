﻿using uniexetask.core.Models;

namespace uniexetask.api.Models.Request
{
    public class CreateUserModel
    {
      
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public int CampusId { get; set; }

        public int RoleId { get; set; }
    }
}
