namespace Bank.Models
{
    public class Approved
    {
        public Guid Id { get; set; }
      // public Employees? Employees { get; set; }   
        public Guid? EmployeeID { get; set; }
        public Leave? Leave { get; set; }
        public Guid? LeaveId { get; set; } 

        public bool? Status { get; set; }


    }
}
