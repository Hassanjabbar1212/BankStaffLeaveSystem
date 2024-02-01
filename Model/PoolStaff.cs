namespace Bank.Models
{
    public class PoolStaff
    {
        public Guid Id { get; set; }

        public Employees? Employees { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid SubstituteId { get; set; }
        public bool Status { get; set; }
    }
}
