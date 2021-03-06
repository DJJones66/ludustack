﻿using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IBrainstormRepository : IRepository<BrainstormSession>
    {
        Task<BrainstormIdea> GetIdea(Guid ideaId);

        Task AddIdea(BrainstormIdea model);

        Task<bool> UpdateIdea(BrainstormIdea idea);

        Task<IEnumerable<BrainstormIdea>> GetIdeasBySession(Guid sessionId);

        Task<bool> AddVote(BrainstormVote model);

        Task<bool> UpdateVote(BrainstormVote model);

        Task<bool> AddComment(BrainstormComment model);
    }
}