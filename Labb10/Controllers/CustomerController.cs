using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Labb10.Entities;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Targets;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Labb10.Controllers
{
    [Route("api/customer")]
    public class CustomerController : Controller
    {
        private DatabaseContext databaseContext;
        private readonly string webRootPath;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(DatabaseContext databaseContext, ILogger<CustomerController> _logger, IHostingEnvironment hostingEnvironment)
        {
            webRootPath = Path.Combine(hostingEnvironment.WebRootPath, "csvFiles", "Customers.csv");
            this.databaseContext = databaseContext;
            this._logger = _logger;
        }
        [HttpGet]
        public IActionResult GetCustomers()
        {
            var customerList = databaseContext.Customers;
            _logger.LogInformation($"Getting all customers from database, count: {customerList.Count()}");
            return Ok(customerList);
            
        }
        [HttpPut]
        public IActionResult GetCustomerToEdit(int id)
        {
            Customer customerToEdit = databaseContext.Customers.Find(id);
            return Ok(customerToEdit);
        }
        [HttpPost]
        public IActionResult PostCustomer(Customer customer)
        {
            customer.DateOfCreation = DateTime.Now;
            databaseContext.Add(customer);
            databaseContext.SaveChanges();
            _logger.LogInformation($"{customer.FirstName}has been added");
            return Ok();
        }
        [HttpDelete]
        public IActionResult RemoveCustomer(int id)
        {
            Customer customer = databaseContext.Customers.Find(id);
            databaseContext.Remove(customer);
            databaseContext.SaveChanges();
            return Ok();
        }
        [HttpPatch]
        public IActionResult UppgradeCustomer(Customer customer)
        {
            Customer customerToEdit = databaseContext.Customers.Find(customer.Id);
            customerToEdit.Age = customer.Age;
            customerToEdit.Gender = customer.Gender;
            customerToEdit.FirstName = customer.FirstName;
            customerToEdit.LastName = customer.LastName;
            customerToEdit.Email = customer.Email;
            customerToEdit.DateOfEdit = DateTime.Now;
            databaseContext.SaveChanges();
            return Ok();
        }
        [HttpGet]
        [Route("removeAll")]
        public IActionResult RemoveData()
        {
            databaseContext.Customers.RemoveRange(databaseContext.Customers);
            databaseContext.SaveChanges();
            return Ok();
        }
        [HttpGet]
        [Route("csv")]
        public IActionResult UploadCSV()
        {
            databaseContext.Customers.RemoveRange(databaseContext.Customers);

            using (var sr = System.IO.File.OpenText(webRootPath))
            {
                string headerLine = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Trim().Split(",");
                    databaseContext.Customers.Add(new Customer
                    {
                        FirstName = line[0],
                        LastName = line[1],
                        Email = line[2],
                        Gender = line[3],
                        Age = int.Parse(line[4]),
                        DateOfCreation = DateTime.Now,
                    });
                }
            }
            databaseContext.SaveChanges();
            return Ok();
        }
        [HttpGet]
        [Route("log")]
        public IActionResult GetLogFile()
        {
            var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("ownFile-web");
            var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now};
            string fileName = fileTarget.FileName.Render(logEventInfo);
            if (!System.IO.File.Exists(fileName))
            {
                throw new Exception("Log file does not exist.");
            }
            else
            {
                return Ok(System.IO.File.ReadAllLines(fileName));
            }
        }
    }
}
