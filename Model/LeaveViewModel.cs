namespace Bank.Models
{
    public class LeaveViewModel
    {
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public Guid LeaveId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid BranchId { get; set; }

        public string BranchName { get; set; }
        public Guid EmployeeID { get; set; }
    }
}
