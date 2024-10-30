﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/reqMembers")]
    [ApiController]
    public class ReqMemberController : ControllerBase
    {
        private readonly IReqMemberService _reqMemberService;

        public ReqMemberController(IReqMemberService reqMemberService)
        {
            _reqMemberService = reqMemberService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReqMembersList()
        {
            var reqMembersList = await _reqMemberService.GetAllReqMember();
            if (reqMembersList == null || !reqMembersList.Any())
            {
                return NotFound();
            }

            var responseData = reqMembersList.Select(reqMember => new
            {
                reqMember.RegMemberId,
                reqMember.Description,
                reqMember.Status,
                GroupName = reqMember.Group.GroupName,
                GroupId = reqMember.Group.GroupId,
                LeaderName = reqMember.Group.GroupMembers
                    .Where(gm => gm.Role == nameof(GroupMemberRole.Leader)) 
                    .Select(gm => gm.Student.User.FullName)
                    .FirstOrDefault(),
                LeaderAvata = reqMember.Group.GroupMembers
                    .Where(gm => gm.Role == nameof(GroupMemberRole.Leader))
                    .Select(gm => gm.Student.User.Avatar)
                    .FirstOrDefault(),
                SubjectCode = reqMember.Group.Subject.SubjectCode,
                MemberCount = reqMember.Group.GroupMembers.Count() 
            });

            ApiResponse<IEnumerable<object>> response = new ApiResponse<IEnumerable<object>>();
            response.Data = responseData;
            return Ok(response);
        }
    }
}
