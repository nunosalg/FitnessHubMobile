namespace FitnessHubMobile.Models
{
    public class Workout
    {
        public string? InstructorEmail { get; set; }

        public string? InstructorName { get; set; }

        public List<Exercise>? ExerciseList { get; set; } = new List<Exercise>();

        public DateTime Date { get; set; }
    }
}
