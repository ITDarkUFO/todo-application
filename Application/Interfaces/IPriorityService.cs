using Application.Models;
using Application.Utilities;

namespace Application.Interfaces
{
    public interface IPriorityService
    {
        Task<List<Priority>> GetPriorityListAsync();

        Task<PriorityResult> GetPriorityByIdAsync(int id);

        Task<PriorityResult> CreatePriorityAsync(Priority priority);

        Task<PriorityResult> EditPriorityAsync(Priority priority);

        Task<PriorityResult> DeletePriorityAsync(int id);
    }
}
