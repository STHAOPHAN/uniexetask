﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/reqTopic")]
    [ApiController]
    public class ReqTopicController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMentorService _mentorService;
        private readonly IMapper _mapper;
        private readonly ITopicService _topicService;
        private readonly IReqTopicService _reqTopicService;
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;

        public ReqTopicController(IProjectService projectService, ITopicService userService, IReqTopicService reqTopicService, IMentorService mentorService, IMapper mapper, IGroupService groupService, IGroupMemberService groupMemberService)
        {
            _projectService = projectService;
            _topicService = userService;
            _mentorService = mentorService;
            _reqTopicService = reqTopicService;
            _mapper = mapper;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
        }   

        [HttpGet]
        public async Task<IActionResult> GetReqTopicList()
        {
            var reqTopicList = await _reqTopicService.GetAllReqTopic();
            if (reqTopicList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<RegTopicForm>> response = new ApiResponse<IEnumerable<RegTopicForm>>();
            response.Data = reqTopicList;
            return Ok(response);
        }

        [Authorize(Roles = "Mentor")]
        [HttpGet("GetGroupReqTopicList")]
        public async Task<IActionResult> GetGroupReqTopicList()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId) || string.IsNullOrEmpty(userRole) || !userRole.Equals("Mentor"))
            {
                return NotFound();
            }

            var mentor = await _mentorService.GetMentorWithGroupAsync(userId);

            if (mentor == null || !mentor.Groups.Any())
            {
                return NotFound("No groups found.");
            }

            var groupIds = mentor.Groups.Select(g => g.GroupId).ToList();

            var groups = new List<object>();

            foreach (var groupId in groupIds)
            {
                var group = await _groupService.GetGroupWithTopic(groupId);
                if (group != null)
                {
                    var regTopicForms = group.RegTopicForms
                        .Where(rt => rt.Status == true)
                        .Select(rt => new
                        {
                            regTopicId = rt.RegTopicId,
                            topicCode = rt.TopicCode,
                            topicName = rt.TopicName,
                            description = rt.Description,
                            status = rt.Status
                        }).ToList();

                    if (regTopicForms.Any()) // Chỉ thêm nhóm nếu regTopicForms không rỗng
                    {
                        var groupData = new
                        {
                            groupId = group.GroupId,
                            groupName = group.GroupName,
                            subjectId = group.SubjectId,
                            hasMentor = group.HasMentor,
                            isCurrentPeriod = group.IsCurrentPeriod,
                            status = group.Status,
                            isDeleted = group.IsDeleted,
                            regTopicForms = regTopicForms,
                            subjectCode = group.Subject.SubjectCode
                        };

                        groups.Add(groupData); // Thêm nhóm vào danh sách
                    }
                }
            }

            if (!groups.Any())
            {
                return NotFound("No groups found.");
            }

            var response = new ApiResponse<IEnumerable<object>>()
            {
                Data = groups
            };

            return Ok(response);
        }


        [Authorize(Roles = "Mentor")]
        [HttpGet("GetReqTopicList/{groupId}")]
        public async Task<IActionResult> GetReqTopicList( int groupId)
        {
            // Lấy thông tin user từ claims
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Xác thực userId và role
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId) || string.IsNullOrEmpty(userRole) || !userRole.Equals("Mentor"))
            {
                return Unauthorized("You are not authorized to access this resource.");
            }

            // Gọi service để lấy danh sách reqTopic theo groupId
            var reqTopics = await _reqTopicService.GetReqTopicByGroupId(groupId);

            // Kiểm tra nếu không có reqTopic nào được tìm thấy
            if (reqTopics == null || !reqTopics.Any())
            {
                return NotFound("No request topics found for the given group.");
            }

            // Tạo response trả về
            var response = new ApiResponse<IEnumerable<object>>
            {
                Data = reqTopics,
                Success = true
            };

            return Ok(response);
        }


        [HttpPost("ApproveTopic")]
        public async Task<IActionResult> ApproveTopic([FromBody] ReqTopicModel reqTopic)
        {
            var topic = new TopicModel
            {
                TopicCode = reqTopic.TopicCode,
                TopicName = reqTopic.TopicName,
                Description = reqTopic.Description
            };

            var objTopic = _mapper.Map<Topic>(topic);
            var objReqTopic = _mapper.Map<RegTopicForm>(reqTopic);

            var topicList = await _topicService.GetAllTopics();

            var group = await _groupService.GetGroupById(reqTopic.GroupId);

            

            if (topicList.Any(t => t.TopicCode == objTopic.TopicCode))
            {
                // Trả về thông báo lỗi nếu đã có chủ đề với TopicCode này
                return BadRequest("Topic với mã này đã tồn tại.");
            }

            var topicId = await _topicService.CreateTopic(objTopic);



            var project = new ProjectModel
            {
                GroupId = group.GroupId,
                TopicId = topicId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                SubjectId= group.SubjectId,
                IsCurrentPeriod = true,
                Status = "In_Progress",
                IsDeleted = false
            };

            var objProject = _mapper.Map<Project>(project);

            var createProject = await _projectService.CreateProject(objProject);

            var groupUpdate = await _groupService.UpdateGroupApproved(reqTopic.GroupId);

            var reqTopicList = await _reqTopicService.GetReqTopicByGroupId(reqTopic.GroupId);

            // Chạy vòng lặp để cập nhật trạng thái cho từng yêu cầu trong danh sách
            foreach (var reqTopicItem in reqTopicList)
            {
                await _reqTopicService.UpdateApproveTopic(reqTopicItem.RegTopicId);
            }

            if (topicId > 0 && createProject)  // Kiểm tra nếu topicId hợp lệ
            {
                var response = new ApiResponse<object>
                {
                    Data = new { Message = "Topic and Project created successfully!", TopicId = topicId }
                };
                return Ok(response);
            }
            else
            {
                return BadRequest("Lỗi Tạo.");
            }
        }



        [Authorize(Roles = nameof(EnumRole.Student))]
        [HttpPost]
        public async Task<IActionResult> CreateReqTopic([FromBody] ReqTopicModel reqTopic)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var role = await _groupMemberService.GetRoleByUserId(userId);

            if (role != "Leader")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You are not a leader to perform this operation." });
            }

            var topicCodeMax = await _reqTopicService.GetMaxTopicCode();
            int nextCodeNumber = 1;
            if (!string.IsNullOrEmpty(topicCodeMax) && topicCodeMax.Length > 2)
            {
                var numericPart = topicCodeMax.Substring(2);
                if (int.TryParse(numericPart, out int currentMax))
                {
                    nextCodeNumber = currentMax + 1;
                }
            }

            reqTopic.TopicCode = $"TP{nextCodeNumber:D3}";

            var groupMember = await _groupMemberService.GetGroupMemberByUserId(userId);
            var group = await _groupService.GetGroupById(groupMember.GroupId);

            if (group.Status == "Approved")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Nhóm đã Approved. Bạn không thể thêm topic." });
            }

            reqTopic.GroupId = groupMember.GroupId;
            reqTopic.Status = true;

            var obj = _mapper.Map<RegTopicForm>(reqTopic);
            var isTopicCreated = await _reqTopicService.CreateReqTopic(obj);

            if (isTopicCreated)
            {
                var response = new ApiResponse<object>
                {
                    Data = new { Message = "Topic created successfully!" }
                };
                return Ok(response);
            }
            else
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Lỗi Tạo." });
            }
        }


        [Authorize(Roles = "Student")]
        [HttpPut]
        public async Task<IActionResult> UpdateReqTopic([FromBody] UpdateRegTopicModel reqTopic)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var role = await _groupMemberService.GetRoleByUserId(userId);

            if (role != "Leader")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You are not a leader to perform this operation." });
            }

            var reqNew = await _reqTopicService.GetReqTopicById(reqTopic.RegTopicId);

            if (reqNew == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "Không tìm thấy yêu cầu với ID đã cho." });
            }

            reqNew.Description = reqTopic.Description;
            reqNew.TopicName = reqTopic.TopicName;
            var isReqUpdated = await _reqTopicService.UpdateReqTopic(reqNew);

            ApiResponse<object> response = new ApiResponse<object>
            {
                Data = new
                {
                    Description = reqNew.Description,
                    TopicName = reqNew.TopicName
                }
            };

            if (isReqUpdated)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Không thể cập nhật Description.");
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPut("DeleteReq")]
        public async Task<IActionResult> DeleteReqTopic([FromBody] int RegTopicId)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var role = await _groupMemberService.GetRoleByUserId(userId);

            if (role != "Leader")
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "You are not a leader to perform this operation." });
            }
            var reqNew = await _reqTopicService.GetReqTopicById(RegTopicId);
            reqNew.Status = false;
            var isReqUpdated = await _reqTopicService.UpdateReqTopic(reqNew);
            ApiResponse<object> response = new ApiResponse<object>
            {
                Data = new
                {
                    TopicName = reqNew.TopicName,
                    Description = reqNew.Description
                }
            };

            if (isReqUpdated)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Không thể cập nhật Description.");
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("MyTopic")]
        public async Task<IActionResult> GetMyReqTopics()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var groupMember = await _groupMemberService.GetGroupMemberByUserId(userId);
            if (groupMember == null || groupMember.GroupId == 0)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "User does not belong to any group." });
            }

            var reqTopicList = await _reqTopicService.GetAllReqTopic();
            if (reqTopicList == null || !reqTopicList.Any())
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorMessage = "No topics available." });
            }

            var responseData = reqTopicList
                .Where(reqTopic => reqTopic.Status == true && reqTopic.GroupId == groupMember.GroupId)
                .Select(reqTopic => new
                {
                    reqTopic.RegTopicId,
                    reqTopic.GroupId,
                    reqTopic.TopicCode,
                    reqTopic.TopicName,
                    reqTopic.Description,
                    reqTopic.Status,
                    GroupName = reqTopic.Group.GroupName,
                    SubjectCode = reqTopic.Group.Subject.SubjectCode
                });

            var response = new ApiResponse<IEnumerable<object>>
            {
                Success = true,
                Data = responseData
            };

            return Ok(response);
        }


    }
}
