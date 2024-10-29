﻿using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class UserService : IUserService
    {
        public IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateUser(User users)
        {
            if (users != null)
            {
                await _unitOfWork.Users.InsertAsync(users);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<bool> DeleteUser(int userId)
        {
            if (userId > 0)
            {
                var users = await _unitOfWork.Users.GetByIDAsync(userId);
                if (users != null)
                {
                    users.IsDeleted = true; 
                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            //var usersList = await _unitOfWork.Users.GetAsync(includeProperties: "Campus,Role");
            var usersList = await _unitOfWork.Users.GetUsersWithCampusAndRole();
            return usersList;
        }

        public async Task<User?> GetUserById(int userId)
        {
            if (userId > 0)
            {
                var users = await _unitOfWork.Users.GetByIDAsync(userId);
                if (users != null)
                {
                    return users;
                }
            }
            return null;
        }

        public async Task<IEnumerable<User>> SearchUsersByEmailAsync(string query)
        {
            return await _unitOfWork.Users.SearchUsersByEmailAsync(query);
        }
        
        public async Task<User?> GetUserByIdWithCampusAndRoleAndStudents(int userId)
        {
            if (userId > 0)
            {
                var users = await _unitOfWork.Users.GetByIDWithCampusAndRoleAndStudents(userId);
                if (users != null)
                {
                    return users;
                }
            }
            return null;
        }

        public async Task<bool> UpdateUser(User users)
        {
            if (users != null)
            {
                var user = await _unitOfWork.Users.GetByIDAsync(users.UserId);
                if (user != null)
                {
                    user.FullName = users.FullName;
                    user.Email = users.Email;
                    user.Password = users.Password;
                    user.CampusId = users.CampusId;
                    user.Phone = users.Phone;
                    user.IsDeleted = users.IsDeleted;
                    user.RoleId = users.RoleId;


                    _unitOfWork.Users.Update(user);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
    }
}
