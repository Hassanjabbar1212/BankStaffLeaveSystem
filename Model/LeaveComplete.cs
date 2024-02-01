namespace Bank.Models
{
    public class LeaveComplete
    {
        public Guid Id { get; set; }

        public Employees? Employees { get; set; }

        public Guid EmployeeId { get; set; }

        public bool IsCompleted { get; set; } 
    }
}
