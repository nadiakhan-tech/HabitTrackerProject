using System;
using System.ComponentModel.DataAnnotations;

namespace HabitTrackerProject.Models
{
    public class Habit
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public int Streak { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastCompleted { get; set; }

        public bool CompletedToday { get; set; } = false;

        // Link habit to user
        public string? UserId { get; set; }
    }
}