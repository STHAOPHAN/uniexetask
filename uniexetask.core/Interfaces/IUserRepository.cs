﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<IEnumerable<string>> GetAllEmails();
        Task<IEnumerable<User>> GetAllUsers();
        Task<User?> GetByIDAsync(int id);
        Task<User?> GetByIDWithCampusAndRoleAndStudents(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersWithCampusAndRole();
        Task<User?> GetUserWithChatGroupByUserIdAsyn(int userId);
        Task<IEnumerable<User>> SearchStudentsByStudentCodeAsync(string query);
        Task<IEnumerable<User>> SearchUsersByEmailAsync(string query);
    }
}
