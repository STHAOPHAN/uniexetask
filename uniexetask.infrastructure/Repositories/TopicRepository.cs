﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;

namespace uniexetask.infrastructure.Repositories
{
    public class TopicRepository : GenericRepository<Topic>, ITopicRepository
    {
        public TopicRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Topic>> GetAllTopics()
        {
            return await dbSet.ToListAsync();
        }

    }
}