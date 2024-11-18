﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/task-detail")]
    [ApiController]
    public class TaskDetailController : ControllerBase
    {
        private readonly ITaskDetailService _taskDetailService;
        private readonly ITaskService _taskService;

        public TaskDetailController(ITaskDetailService taskDetailService, ITaskService taskService)
        {
            _taskDetailService = taskDetailService;
            _taskService = taskService;
        }

        [HttpGet("byTask/{taskId}")]
        public async Task<IActionResult> GetTaskDetailListByTaskId(int taskId)
        {
            ApiResponse<IEnumerable<TaskDetail>> response = new ApiResponse<IEnumerable<TaskDetail>>();
            try
            {
                var taskDetailList = await _taskDetailService.GetTaskDetailListByTaskId(taskId);
                if (taskDetailList == null)
                {
                    throw new Exception("TaskDetailList not found");
                }
                // Lọc bỏ những task detail có isDelete == true
                var filteredTaskDetailList = taskDetailList.Where(taskDetail => !taskDetail.IsDeleted).ToList();

                response.Data = filteredTaskDetailList;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }

        }

        [HttpDelete("{taskDetailId}")]
        public async Task<IActionResult> DeleteTaskDetail(int taskDetailId)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                // Kiểm tra giá trị taskId hợp lệ
                if (taskDetailId <= 0)
                {
                    response.Success = false;
                    response.ErrorMessage = "Invalid task detail ID.";
                    return BadRequest(response);
                }

                // Gọi phương thức DeleteTask từ TaskService
                var result = await _taskDetailService.DeleteTaskDetail(taskDetailId);
                if (result)
                {
                    response.Success = true;
                    response.Data = "Task Detail deleted successfully.";
                    return Ok(response);
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = "Task Detail not found.";
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut("updateStatus")]
        public async Task<IActionResult> UpdateStatusTaskDetail([FromBody] UpdateStatusTaskDetailModel model)
        {
            ApiResponse<TaskDetail> response = new ApiResponse<TaskDetail>();
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (!ModelState.IsValid)
                {
                    throw new Exception("Invalid request model");
                }

                // Lấy task từ database theo taskId
                var existingTaskDetail = await _taskDetailService.GetTaskDetailById(model.TaskDetailId);
                if (existingTaskDetail == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "Task not found.";
                    return NotFound(response);
                }

                // Cập nhật trạng thái của task
                existingTaskDetail.IsCompleted = model.IsCompleted;

                // Lưu thay đổi vào cơ sở dữ liệu
                var updatedTaskDetail = await _taskDetailService.UpdateTaskDetails(existingTaskDetail);
                if (!updatedTaskDetail)
                {
                    throw new Exception("Error updating task detail status");
                }

                var taskDetailList = await _taskDetailService.GetTaskDetailListByTaskId(existingTaskDetail.TaskId);
                if (taskDetailList == null || !taskDetailList.Any())
                {
                    throw new Exception("TaskDetailList not found or empty.");
                }

                // Kiểm tra nếu tất cả các TaskDetail có IsCompleted = true
                bool allTaskDetailsCompleted = taskDetailList.All(detail => detail.IsCompleted);

                // Lấy Task theo TaskId
                var existingTask = await _taskService.GetTaskById(existingTaskDetail.TaskId);
                if (existingTask == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "Task not found.";
                    return NotFound(response);
                }

                if (allTaskDetailsCompleted)
                {
                    existingTask.Status = "Completed";
                    var updatedTask = await _taskService.UpdateTask(existingTask);
                    if (!updatedTask)
                    {
                        throw new Exception("Error updating task status");
                    }
                }
                else
                {
                    existingTask.Status = "In_Progress";
                    var updatedTask = await _taskService.UpdateTask(existingTask);
                    if (!updatedTask)
                    {
                        throw new Exception("Error updating task status");
                    }
                }

                // Trả về task sau khi cập nhật
                response.Data = existingTaskDetail;
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
