using Bank.Data;
using Bank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;

namespace Bank.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly dbcontext _context;
        public EmployeeController(dbcontext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult AddBank()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddBank(Banks banks)
        {
            _context.banks.Add(banks);
            _context.SaveChanges();
            return View();
        }
        [HttpGet]
        public IActionResult AddBranch()
        {
            ViewBag.Banks = new SelectList(_context.banks, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult AddBranch(Branch branch)
        {
            _context.Branch.Add(branch);
            _context.SaveChanges();
            return View();
        }
        [HttpGet]
        public IActionResult Add_Employee()
        {
            ViewBag.Branch = new SelectList(_context.Branch, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Add_Employee(Employees employees)
        {
            _context.Employees.Add(employees);
            _context.SaveChanges();
            return RedirectToAction("Add_Employee");
        }
        [HttpGet]
        public IActionResult ExtraEmployee()
        {
            var branchesWithMoreThanSixEmployees = _context.Employees
    .GroupBy(e => e.BranchId)
    .Where(group => group.Count() > 6)
    .Select(group => group.Key)
    .ToList();

            var employeesWithMoreThanSix = _context.Employees
                .Where(e => branchesWithMoreThanSixEmployees.Contains(e.BranchId))
                .OrderBy(e => e.BranchId)
                .ThenBy(e => e.Id)
                .Skip(6)
                .ToList();

            var employeeIdsWithMoreThanSix = employeesWithMoreThanSix.Select(e => e.Id).ToList();

            var employeesToShow = _context.Employees
                .Where(e => employeeIdsWithMoreThanSix.Contains(e.Id) &&
                            !_context.PoolStaff.Any(ps => ps.SubstituteId == e.Id))
                .OrderBy(e => e.BranchId)
                .ThenBy(e => e.Id)
                .ToList();

            // Pass data to view
            return View(employeesToShow);
            //       var branchEmployeeCounts = _context.Employees
            //.AsEnumerable()
            //.GroupBy(e => e.BranchId)
            //.ToDictionary(g => g.Key, g => g.Count());

            //       var branchesWithMoreThanSixEmployees = _context.Employees
            //.GroupBy(e => e.BranchId)
            //.Where(group => group.Count() > 6)
            //.Select(group => group.Key)
            //.ToList();

            //       var employeesToShow = _context.Employees
            //           .Where(e => branchesWithMoreThanSixEmployees.Contains(e.BranchId))
            //           .AsEnumerable() // Switch to client-side evaluation
            //           .GroupBy(e => e.BranchId)
            //           .SelectMany(group => group.OrderBy(e => e.Id).Skip(6))
            //           .ToList();
            //       // Pass data to view
            //       return View(employeesToShow);

        }
        public IActionResult Status()
        {
            var logedInId = User.FindFirstValue(ClaimTypes.Sid);
            if (Guid.TryParse(logedInId, out Guid logedInGuid))
            {
                // Successfully parsed as GUID, proceed with your logic
                var status = _context.Approved
     .Where(a => a.EmployeeID == logedInGuid)
     .Select(c => c.Status)
     .ToList();

     //           var statusStrings = status
     //.Select(s => s.HasValue ? (s.Value ? "Approved" : "Rejected") : "Unknown")
     //.ToList();

                return View(status);
            }
            else
            {
                return NoContent();
            }

        }
        [HttpGet]
        public IActionResult LeaveComplete()
        {
            return View();
                
        }
        [HttpPost]
        public IActionResult LeaveComplete(ViewModel model)
        {
            // Access model.EmployeeId and model.IsCompleted here
            var logedInId = User.FindFirstValue(ClaimTypes.Sid);

            if (Guid.TryParse(logedInId, out Guid logedInGuid))
            {

                var complete = new LeaveComplete
                {
                    IsCompleted = model.IsCompleted,
                    EmployeeId = logedInGuid,

                };
                _context.LeaveComplete.Add(complete);
                _context.SaveChanges();
                var poolStaffRecord = _context.PoolStaff.SingleOrDefault(a => a.EmployeeId == logedInGuid);

                if (poolStaffRecord != null)
                {
                    poolStaffRecord.Status = false;
                    _context.PoolStaff.Update(poolStaffRecord);
                    _context.SaveChanges();
                }
                    var SubstituteId = poolStaffRecord.SubstituteId;
                    var employeeInfo = _context.Employees
                   .Where(a => a.Id == SubstituteId)
                   .Select(a => new {
                    a.Email,
                    a.FName,
                    a.LName,
                   a.BranchId
                    })
                 .FirstOrDefault();
                var BranchName = _context.Branch.Where(a => a.Id == employeeInfo.BranchId).Select(a => a.Name).FirstOrDefault();
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("hassanjabbar2017@gmail.com", "poljeaszpdjiywtk"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("hassanjabbar2017@gmail.com"), // Replace with your Gmail email address.
                    Subject = "Tranfer",
                    Body = $"Dear {employeeInfo.FName+""+employeeInfo.LName},\n" + $"Your have to go back .\n" +
                     $"Branch: {BranchName}\n",
                    IsBodyHtml = false,
                };

                if (!string.IsNullOrEmpty(employeeInfo.Email))
                {
                    mailMessage.To.Add(employeeInfo.Email);
                }

                smtpClient.Send(mailMessage);
                return View();
            }
            else
            {
                return NotFound();
            }

        }


    }
}
