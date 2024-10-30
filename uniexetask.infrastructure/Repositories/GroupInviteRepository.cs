﻿using Microsoft.EntityFrameworkCore;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;

namespace uniexetask.infrastructure.Repositories
{
    public class GroupInviteRepository : GenericRepository<GroupInvite>, IGroupInviteRepository
    {
        public GroupInviteRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<GroupInvite?> GetGroupInviteByNotificationId(int notificationId)
        {
            return await dbSet
                .Where(gi => gi.NotificationId == notificationId)
                .FirstOrDefaultAsync();
        }

        public async Task<GroupInvite?> GetPendingInvite(int receiverId, int groupId)
        {
            return await dbSet
                .Where(g => g.Status == nameof(GroupInviteStatus.Pending) && g.GroupId == groupId && g.InviteeId == receiverId)
                .FirstOrDefaultAsync();
        }
    }
}