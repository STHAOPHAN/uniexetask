﻿using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ReqMemberService : IReqMemberService
    {
        public IUnitOfWork _unitOfWork;
        public ReqMemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<RegMemberForm>> GetAllReqMember()
        {
            var reqMemberList = await _unitOfWork.ReqMembers.GetAsync(
                includeProperties: "Group,Group.GroupMembers,Group.Subject,Group.GroupMembers.Student.User");
            return reqMemberList;
        }

    }
}
