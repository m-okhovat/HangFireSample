using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HangFireSample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HangFireController : ControllerBase
    {

        [HttpGet]
        [Route("Welcome")]
        public ActionResult Welcome(string name)
        {
            var jobId = BackgroundJob.Enqueue(() => SendEmail(name));
            return Ok($"JobId {jobId} completed. Mail sent.");
        }

        [HttpGet]
        [Route("DelayedWelcome")]
        public ActionResult DelayedWelcome(string name) //Send email with delay
        {
            var jobId = BackgroundJob.Schedule(() => SendEmail(name), TimeSpan.FromMinutes(2));
            return Ok($"JobId {jobId} completed. Mail sent.");
        }

        public void SendEmail(string name)
        {
            Console.WriteLine($"welcome dear {name}");
        }

        [HttpPost]
        [Route("invoice")] //Send email recurring
        public IActionResult Invoice(string userName)
        {
            RecurringJob.AddOrUpdate(() => SendInvoiceMail(userName), Cron.Monthly);
            return Ok($"Recurring Job Scheduled. Invoice will be mailed Monthly for {userName}!");
        }
        public void SendInvoiceMail(string userName)
        {
            //Logic to Mail the user
            Console.WriteLine($"Here is your invoice, {userName}");
        }

        [HttpPost]
        [Route("unsubscribe")]
        public IActionResult Unsubscribe(string userName)
        {
            var jobId = BackgroundJob.Enqueue(() => UnsubscribeUser(userName));
            BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine($"Sent Confirmation Mail to {userName}"));
            return Ok($"Unsubscribed");
        }

        public void UnsubscribeUser(string userName)
        {
            //Logic to Unsubscribe the user
            Console.WriteLine($"Unsubscribed {userName}");
        }

    }
}