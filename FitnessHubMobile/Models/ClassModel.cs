namespace FitnessHubMobile.Models
{
    public class ClassModel
    {
        public int Id { get; set; }

        public string? Category { get; set; }

        public string? ClassType { get; set; }

        public string? Instructor { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public string? Location { get; set; }
    }
}
