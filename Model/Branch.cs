namespace Bank.Models
{
    public class Branch
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int? BranchCode { get; set; }

        public List<Employees>? Employees { get; set; }
        public Banks? Banks { get; set; }
        public Guid? BankId { get; set; }


    }
}
