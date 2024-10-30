﻿using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class GroupMemberService : IGroupMemberService
    {
        public IUnitOfWork _unitOfWork;
        public GroupMemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<GroupMember>> GetAllGroupMember()
        {
            var groupList = await _unitOfWork.GroupMembers.GetAsync();
            return groupList;
        }

        public async Task<bool> CreateGroupMember(GroupMember groupMember)
        {
            if (groupMember != null)
            {
                await _unitOfWork.GroupMembers.InsertAsync(groupMember);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<GroupMember?> GetGroupByStudentId(int studentId)
        {
            var groupMember = (await _unitOfWork.GroupMembers.GetAsync(
                gm => gm.StudentId == studentId,
                includeProperties: "Group")) 
                .FirstOrDefault();

            return groupMember;
        }

        public async Task<GroupMember?> GetGroupMemberByUserId(int userId)
        {
            
            var student = (await _unitOfWork.Students.GetAsync(s => s.UserId == userId)).FirstOrDefault();

            if (student == null)
            {
                return null; 
            }

            var groupMember = (await _unitOfWork.GroupMembers.GetAsync(
                gm => gm.StudentId == student.StudentId,
                includeProperties: "Group")).FirstOrDefault();

            return groupMember;
        }

        public async Task<bool> CheckIfStudentInGroup(int studentId)
        {
            return await _unitOfWork.GroupMembers.AnyAsync(gm => gm.StudentId == studentId);
        }


        public async Task<bool> AddMember(GroupMember members)
        {
            if (members != null)
            {
                await _unitOfWork.GroupMembers.InsertAsync(members);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<List<User>> GetUsersByGroupId(int groupId)
        {
            var groupMembers = await _unitOfWork.GroupMembers.GetAsync(gm => gm.GroupId == groupId, includeProperties: "Student.User");
            var users = groupMembers.Select(gm => gm.Student.User).ToList();
            return users; 
        }

        public async Task<List<User>> GetUsersByUserId(int userId)
        {
            var student = await _unitOfWork.Students.GetAsync(s => s.UserId == userId, includeProperties: "GroupMembers.Group");

            var singleStudent = student.FirstOrDefault();

            if (singleStudent == null || singleStudent.GroupMembers == null || !singleStudent.GroupMembers.Any())
            {
                return new List<User>();
            }

            var groupId = singleStudent.GroupMembers.Select(gm => gm.GroupId).FirstOrDefault();

            var groupMembers = await _unitOfWork.GroupMembers.GetAsync(gm => gm.GroupId == groupId, includeProperties: "Student.User");

            var users = groupMembers.Select(gm => gm.Student.User).ToList();

            return users;
        }


    }
}
