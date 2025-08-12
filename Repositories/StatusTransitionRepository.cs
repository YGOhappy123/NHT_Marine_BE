using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Data;
using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Interfaces.Repositories;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Utilities;

namespace NHT_Marine_BE.Repositories
{
    public class StatusTransitionRepository : IStatusTransitionRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public StatusTransitionRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<StatusTransition> ApplyFilters(IQueryable<StatusTransition> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        default:
                            query = query.Where(sr => EF.Property<string>(sr, filter.Key.CapitalizeAllWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        public async Task<List<StatusTransitionGroupDto>> GetAllTransitions(BaseQueryObject queryObject)
        {
            var query = _dbContext.StatusTransitions.Include(st => st.FromStatus).Include(st => st.ToStatus).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Filter))
            {
                var parsedFilter = JsonSerializer.Deserialize<Dictionary<string, object>>(queryObject.Filter);
                query = ApplyFilters(query, parsedFilter!);
            }

            var grouped = await query
                .GroupBy(st => st.FromStatusId)
                .Select(g => new StatusTransitionGroupDto
                {
                    FromStatusId = g.Key,
                    Transitions = g.OrderByDescending(st => st.TransitionId)
                        .Select(st => new StatusTransitionDataDto
                        {
                            TransitionId = st.TransitionId,
                            ToStatusId = st.ToStatusId,
                            TransitionLabel = st.TransitionLabel,
                            ToStatus = st.ToStatus,
                        })
                        .ToList(),
                })
                .ToListAsync();

            return grouped;
        }

        public async Task<StatusTransition?> GetTransitionById(int transitionId)
        {
            return await _dbContext
                .StatusTransitions.Include(st => st.FromStatus)
                .Include(st => st.ToStatus)
                .SingleOrDefaultAsync(st => st.TransitionId == transitionId);
        }

        public async Task<StatusTransition?> GetTransitionByFromAndToStatusId(int fromStatusId, int toStatusId)
        {
            return await _dbContext
                .StatusTransitions.Where(st => st.FromStatusId == fromStatusId && st.ToStatusId == toStatusId)
                .Include(st => st.FromStatus)
                .Include(st => st.ToStatus)
                .FirstOrDefaultAsync();
        }

        public async Task AddStatusTransition(StatusTransition statusTransition)
        {
            _dbContext.StatusTransitions.Add(statusTransition);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStatusTransition(StatusTransition statusTransition)
        {
            _dbContext.StatusTransitions.Update(statusTransition);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteStatusTransition(StatusTransition statusTransition)
        {
            _dbContext.StatusTransitions.Remove(statusTransition);
            await _dbContext.SaveChangesAsync();
        }
    }
}
