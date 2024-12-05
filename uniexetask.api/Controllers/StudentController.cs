﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using uniexetask.api.Extensions;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentsService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public StudentController(IStudentService studentsService, IUserService userService, IMapper mapper, IEmailService emailService)
        {
            _studentsService = studentsService;
            _userService = userService;
            _mapper = mapper;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentList()
        {
            ApiResponse<IEnumerable<Student>> response = new ApiResponse<IEnumerable<Student>>();
            try
            {
                var studentList = await _studentsService.GetAllStudent();
                if (studentList == null)
                {
                    response.Data = studentList;
                    response.Success = true;
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Student not found");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("bystudentcode")]
        public async Task<IActionResult> GetStudentByStudentCode(string studentCode)
        {
            var student = await _studentsService.GetStudentByCode(studentCode);

            if (student != null)
            {
                return Ok(student);
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpGet("byuserid")]
        public async Task<IActionResult> GetStudentByUserId(int userId)
        {
            ApiResponse<Student> response = new ApiResponse<Student>();
            try
            {
                var student = await _studentsService.GetStudentByUserId(userId);

                if (student != null)
                {
                    response.Data = student;
                    response.Success = true;
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Student not found");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            ApiResponse<Student> response = new ApiResponse<Student>();
            try
            {
                var student = await _studentsService.GetStudentById(id);

                if (student != null)
                {
                    response.Data = student;
                    response.Success = true;
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Student not found");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateStudent(CreateStudentModel student)
        {
            bool isUserExisted = await _userService.CheckDuplicateUser(student.Email, student.Phone);
            if(isUserExisted)
                return Conflict("Email or phone has already been registered!");

            bool isStudentExisted = await _studentsService.CheckDuplicateStudentCode(student.StudentCode);
            if (isStudentExisted)
                return Conflict("Student code already exists!");

            string password = PasswordHasher.GenerateRandomPassword(10);
            var userModel = new UserModel
            {
                FullName = student.FullName,
                Password = PasswordHasher.HashPassword(password),
                Email = student.Email,
                Phone = student.Phone,
                CampusId = student.CampusId,
                IsDeleted = false,
                RoleId = 3,
                Avatar = "https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg"
            };
            var userEntity = _mapper.Map<User>(userModel);
            var isUserCreated = await _userService.CreateUser(userEntity);
            if (isUserCreated != null)
            {
                var studentModel = new StudentModel
                {
                    UserId = isUserCreated.UserId,
                    LecturerId = student.LectureId,
                    SubjectId = student.SubjectId,
                    StudentCode = student.StudentCode,
                    Major = student.Major,
                };
                var studentEntity = _mapper.Map<Student>(studentModel);
                var result = await _studentsService.CreateStudent(studentEntity);
                var userEmail = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            padding: 20px;
            background-color: #ffffff;
        }}
        h2 {{
            color: #333;
        }}
        p {{
            margin: 0 0 10px;
        }}
    </style>
</head>
<body>
    <h2>Dear {student.FullName},</h2>
    <p>We are sending you the following login account information:</p>
    <ul>
        <li><strong>Account:</strong> {student.Email}</li>
        <li><strong>Password: </strong><em>{password}</em>.</li>
    </ul>
    <p>We recommend that you change your password after logging in for the first time.</p>
    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>
    <p>This is an automated email, please do not reply to this email. If you need assistance, please contact us at unitask68@gmail.com.</p>
</body>
</html>
";
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _emailService.SendEmailAsync(student.Email, "Account UniEXETask", userEmail);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                ApiResponse<Student> respone = new ApiResponse<Student>();
                respone.Data = studentEntity;
                respone.Success = result;
                return Ok(respone);
            }

            return BadRequest();
        }
    }
}
