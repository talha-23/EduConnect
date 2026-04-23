namespace EduConnect.Models
{
    /// <summary>
    /// Faculty entity - inherits from Person (Liskov Substitution Principle)
    /// </summary>
    public class Faculty : Person
    {
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;

        public override string GetRole()
        {
            return "Faculty";
        }

        // Additional faculty-specific properties can be added later
        public List<Guid> AssignedCourseIds { get; set; } = new List<Guid>();
    }
}