namespace Bank.Models
{
    public class Leave
    {
        public Guid Id { get; set; }

        public Guid? EmployeeId { get; set; }
        public Employees? Employees { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }
    }
}
