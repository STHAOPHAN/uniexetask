﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class MemberScoreService : IMemberScoreService
    {
        public IUnitOfWork _unitOfWork;

        public MemberScoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async System.Threading.Tasks.Task<bool> AddMemberScore(MemberScore memberScore)
        {
            await _unitOfWork.MemberScores.InsertAsync(memberScore);
            var result = _unitOfWork.Save();
            if(result > 0)
                return true;
            return false;
        }
    }
}
