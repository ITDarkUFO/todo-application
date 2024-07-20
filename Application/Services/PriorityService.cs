using Application.Data;
using Application.Interfaces;
using Application.Models;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class PriorityService(ApplicationDbContext context) : IPriorityService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Priority>> GetPriorityListAsync()
        {
            var priorities = await _context.Priorities.OrderBy(p => p.Level).ToListAsync();

            foreach (var priority in priorities)
            {
                priority.ToDoItems = await _context.ToDoItems.Where(i => i.Priority == priority.Id).ToListAsync();
            }

            return priorities;
        }

        public async Task<PriorityResult> GetPriorityByIdAsync(int id)
        {
            var priority = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == id);
            if (priority == null)
            {
                return new() { Priority = null };
            }

            priority.ToDoItems = await _context.ToDoItems.Where(i => i.Priority == priority.Id).ToListAsync();

            return new() { Priority = priority };
        }

        public async Task<PriorityResult> CreatePriorityAsync(Priority priority)
        {
            var validationResult = new ValidationResult();

            if (await _context.Priorities.AsNoTracking().AnyAsync(p => p.Level == priority.Level))
            {
                validationResult.AddError("Level", $"Уровень приоритета {priority.Level} уже существует");
            }

            if (priority.Level <= 0)
            {
                validationResult.AddError("Level", "Уровень приоритета не может быть меньше или равен нулю");
            }

            _context.Add(priority);
            await _context.SaveChangesAsync();

            return new() { Priority = priority, ValidationResult = validationResult };
        }

        public async Task<PriorityResult> EditPriorityAsync(Priority priority)
        {
            var validationResult = new ValidationResult();

            if (!await PriorityExists(priority.Id))
            {
                validationResult.AddError("Id", "Некорректный ID приоритета");
            }

            if (await _context.Priorities.AsNoTracking().AnyAsync(p => p.Level == priority.Level)
                && priority.Level != (await _context.Priorities.AsNoTracking().FirstOrDefaultAsync(p => p.Id == priority.Id))!.Level)
            {
                validationResult.AddError("Level", $"Уровень приоритета {priority.Level} уже существует");
            }

            if (priority.Level <= 0)
            {
                validationResult.AddError("Level", "Уровень приоритета не может быть меньше или равен нулю");
            }

            try
            {
                _context.Update(priority);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PriorityExists(priority.Id))
                {
                    validationResult.AddError("Id", "Некорректный ID приоритета");
                }
                else
                {
                    validationResult.AddError("", "Произошла ошибка при попытке обновить данные. Пожалуйста, попробуйте снова");
                }
            }

            return new() { Priority = priority, ValidationResult = validationResult };
        }

        public async Task<PriorityResult> DeletePriorityAsync(int id)
        {
            var priority = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == id);
            if (priority != null)
            {
                _context.Priorities.Remove(priority);
            }

            await _context.SaveChangesAsync();

            return new() { Priority = priority };
        }

        private async Task<bool> PriorityExists(int id)
        {
            return await _context.Priorities.AnyAsync(e => e.Id == id);
        }
    }
}
