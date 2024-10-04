﻿using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IMentorService
    {
        Task<Mentor?> GetMentorWithGroupAsync(int userId);
        Task<IEnumerable<Mentor>> GetMentorsAsync();
    }
}