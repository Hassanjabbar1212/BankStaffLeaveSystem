namespace Bank.Models
{
    public class Employees
    {
        public Guid Id { get; set; }

        public string? FName { get; set; }

        public string? LName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }
        
        public string? Password { get; set; }
        public string? Designation { get; set; }

        public string? Address { get; set; }

        public Branch? Branch { get; set; } // Navigation property
        public Guid BranchId { get; set; } // Foreign key property



    }
}
