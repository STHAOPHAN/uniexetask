﻿using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
﻿using Microsoft.AspNetCore.Authorization;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;
using AutoMapper;
using OfficeOpenXml;
using System.IO;

namespace uniexetask.api.Controllers
{
    //[Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the list of product
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            var usersList = await _userService.GetAllUsers();
            if (usersList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<User>> response = new ApiResponse<IEnumerable<User>>();
            response.Data = usersList;
            return Ok(response);
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProductById(int userId)
        {
            var users = await _userService.GetUserById(userId);

            if (users != null)
            {
                return Ok(users);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("upload-excel")]
        public async Task<IActionResult> CreateUser(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return BadRequest("File is not selected or is empty.");
            }

            var usersList = new List<UserModel>();

            // Đọc file Excel
            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Giả sử dòng đầu tiên là tiêu đề
                    {
                        // Lấy giá trị của campus_id và role_id từ file Excel
                        var campusName = worksheet.Cells[row, 6].Text; // Cột 6: campus_id
                        var roleName = worksheet.Cells[row, 8].Text; // Cột 8: role_id

                        // Chuyển đổi campus_id từ tên sang số ID tương ứng
                        int campusId;
                        switch (campusName)
                        {
                            case "FPT-HN":
                                campusId = 1;
                                break;
                            case "FPT-HCM":
                                campusId = 2;
                                break;
                            case "FPT-DN":
                                campusId = 3;
                                break;
                            default:
                                campusId = 0; // Hoặc giá trị mặc định nếu không khớp
                                break;
                        }

                        // Chuyển đổi role_id từ tên sang số ID tương ứng
                        int roleId;
                        switch (roleName.ToLower()) // Chuyển đổi sang chữ thường để tránh vấn đề phân biệt chữ hoa
                        {
                            case "admin":
                                roleId = 1;
                                break;
                            case "manager":
                                roleId = 2;
                                break;
                            case "student":
                                roleId = 3;
                                break;
                            case "mentor":
                                roleId = 4;
                                break;
                            case "sponsor":
                                roleId = 5;
                                break;
                            default:
                                roleId = 0; 
                                break;
                        }

                        var user = new UserModel
                        {
                            FullName = worksheet.Cells[row, 2].Text,   
                            Password = worksheet.Cells[row, 3].Text,   
                            Email = worksheet.Cells[row, 4].Text,      
                            Phone = worksheet.Cells[row, 5].Text,      
                            CampusId = campusId,                       
                            Status = bool.Parse(worksheet.Cells[row, 7].Text),  
                            RoleId = roleId                           
                        };
                        usersList.Add(user);
                    }
                }
            }

            foreach (var userModel in usersList)
            {
                var userEntity = _mapper.Map<User>(userModel);
                var isUserCreated = await _userService.CreateUser(userEntity);

                if (!isUserCreated)
                {
                    return BadRequest($"Failed to create user: {userModel.Email}");
                }
            }

            return Ok("All users were successfully created.");
        }



        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserModel users)
        {
            var obj = _mapper.Map<User>(users);
            var isUserCreated = await _userService.CreateUser(obj);

            if (isUserCreated)
            {
                return Ok(isUserCreated);
            }
            else
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Update the product
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateUser(User user)
        {
            ApiResponse<bool> respone = new ApiResponse<bool>();
            var isUserUpdated = await _userService.UpdateUser(user);
            if (isUserUpdated)
            {
                respone.Data = isUserUpdated;
                return Ok(respone);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete product by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteProduct(int userId)
        {
            ApiResponse<bool> respone = new ApiResponse<bool>();
            var isUserDeleted = await _userService.DeleteUser(userId);

            if (isUserDeleted)
            {
                respone.Data = true;
                return Ok(respone);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}