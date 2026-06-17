using HabitTrackerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HabitTrackerProject.Controllers
{
    [Authorize]
    public class HabitsController : Controller
    {
        private readonly AppDbContext _context;

        public HabitsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var habits = await _context.Habits
                .Where(h => h.UserId == userId)
                .ToListAsync();

            foreach (var habit in habits)
            {
                if (habit.LastCompleted.HasValue &&
                    habit.LastCompleted.Value.Date < DateTime.Today)
                {
                    habit.CompletedToday = false;
                }
            }
            await _context.SaveChangesAsync();
            return View(habits);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name, string Description)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var habit = new Habit
            {
                Name = Name,
                Description = Description,
                Streak = 0,
                CreatedAt = DateTime.Now,
                CompletedToday = false,
                UserId = userId
            };
            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
            if (habit == null) return NotFound();

            if (!habit.CompletedToday)
            {
                habit.CompletedToday = true;
                habit.LastCompleted = DateTime.Now;
                habit.Streak += 1;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
            if (habit != null)
            {
                _context.Habits.Remove(habit);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}