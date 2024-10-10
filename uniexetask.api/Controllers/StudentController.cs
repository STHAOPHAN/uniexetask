﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentsService;

        public StudentController(IStudentService studentsService)
        {
            _studentsService = studentsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentList()
        {
            var studentList = await _studentsService.GetAllStudent();
            if (studentList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<Student>> response = new ApiResponse<IEnumerable<Student>>();
            response.Data = studentList;
            return Ok(response);
        }

        [HttpGet("{studentCode}")]
        public async Task<IActionResult> GetStudentByStdentCode(string studentCode)
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
    }
}
