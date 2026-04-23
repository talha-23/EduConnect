namespace EduConnect.Models
{
    /// <summary>
    /// Admin entity - inherits from Person (Liskov Substitution Principle)
    /// </summary>
    public class Admin : Person
    {
        public string AdminLevel { get; set; } = string.Empty;

        public override string GetRole()
        {
            return "Admin";
        }
    }
}