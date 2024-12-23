﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/campus")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _campusService;

        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCampusesList()
        {
            var campusesList = await _campusService.GetAllCampus();
            if (campusesList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<Campus>> response = new ApiResponse<IEnumerable<Campus>>();
            response.Data = campusesList;
            return Ok(response);
        }
    }
}
