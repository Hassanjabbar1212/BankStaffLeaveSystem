using Bank.Data;
using Bank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;

namespace Bank.Controllers
{
    public class LeaveController : Controller
    {
        private readonly dbcontext _context;
        public LeaveController(dbcontext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult LeaveRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LeaveRequest(Leave leave) 
        {
            var logedInId = User.FindFirstValue(ClaimTypes.Sid);
            var logedInGuid = Guid.Parse(logedInId);
            var Leave = new Leave
               {
                   EmployeeId = logedInGuid,
                   Reason = leave.Reason,
                   StartDate = leave.StartDate,
                   EndDate = leave.EndDate,

               };
                _context.Leave.Add(Leave);
                _context.SaveChanges();
            
            return View();
        }
        [HttpPost]
        public IActionResult LeaveResponce(Approved xyz)
        {
            var approvalContent = new Approved
            {
                EmployeeID = xyz.EmployeeID,
                LeaveId = xyz.LeaveId,
            };
            _context.Approved.Add(approvalContent);
            _context.SaveChanges();
            if (xyz.Status==true)
            {
                var branchesWithMoreThanSixEmployees = _context.Branch
        .Where(branch => branch.Employees.Count > 6)
        .ToList();
                foreach (var branch in branchesWithMoreThanSixEmployees)
                {
                    var random = new Random();
                    var seventhEmployee = _context.Employees
                        .Where(employee => employee.BranchId == branch.Id)
                        .Skip(6)  
                        .AsEnumerable()
                        .OrderBy(employee => random.Next())
                        .FirstOrDefault();
                    if (seventhEmployee != null)
                    {
                        var startDate = _context.Leave.Where(a=>a.EmployeeId == xyz.EmployeeID).Select(a => a.StartDate).FirstOrDefault().ToString("MM/dd/yyyy h:mm tt");
                        var EndDate = _context.Leave.Where(a => a.EmployeeId == xyz.EmployeeID).Select(a => a.EndDate).FirstOrDefault().ToString("MM/dd/yyyy h:mm tt");
                        var BranchId = _context.Employees.Where(a=>a.Id==xyz.EmployeeID).Select(a=>a.BranchId).FirstOrDefault();
                        var BranchName= _context.Branch.Where(a=>a.Id == BranchId).Select(a=>a.Name).FirstOrDefault();
                        var seventhName = seventhEmployee.FName + " " + seventhEmployee.LName;
                        var seventthemail = seventhEmployee.Email;
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
                            Body = $"Dear {seventhName},\n" + $"Your have to go .\n" +
                             $"Branch: {BranchName}\n" +
                             $"From: {startDate}\n" +
                             $"To: {EndDate}\n",
                            IsBodyHtml = false,
                        };

                        if (!string.IsNullOrEmpty(seventthemail))
                        {
                            mailMessage.To.Add(seventthemail);
                        } 

                        smtpClient.Send(mailMessage);

                        var content = new PoolStaff
                        {
                            EmployeeId = xyz.EmployeeID ?? Guid.Empty, 
                            SubstituteId = seventhEmployee.Id,
                            Status = true,
                        };
                        _context.PoolStaff.Add(content);
                        _context.SaveChanges();

                    }
                    return RedirectToAction("Index", "Home");
                }
                }
            else
            {
                return NotFound();
            }
            return View();
        }
        [HttpGet]
        public IActionResult DisplayLeave()
        {
            var leaveWithoutApprovalWithEmployeeInfo = _context.Leave
    .Include(leave => leave.Employees).ThenInclude(employee => employee.Branch)
    .Where(leave => !_context.Approved.Any(approved => approved.LeaveId == leave.Id))
    .Select(leave => new LeaveViewModel
    {
        Fname = leave.Employees != null ? leave.Employees.FName : null,
        Lname = leave.Employees != null ? leave.Employees.LName : null,
        EmployeeID = leave.Employees != null ? leave.Employees.Id : Guid.Empty,
        LeaveId = leave.Id,
        StartDate = leave.StartDate,
        EndDate = leave.EndDate,
        BranchId = leave.Employees != null && leave.Employees.BranchId != null ? leave.Employees.BranchId : Guid.Empty,
        BranchName = leave.Employees != null && leave.Employees.Branch != null ? leave.Employees.Branch.Name : null,
    })
    .ToList();

            return View(leaveWithoutApprovalWithEmployeeInfo);

        }
        [HttpGet]
        public IActionResult PoolStaff()
        {
            var poolStaff = _context.PoolStaff;
            return View();
        }

    }
}