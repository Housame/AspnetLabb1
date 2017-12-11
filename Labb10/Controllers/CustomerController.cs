using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Labb10.Entities;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Labb10.Controllers
{
    [Route("api/customer")]
    public class CustomerController : Controller
    {
        private DatabaseContext databaseContext;
        private readonly string pathToCsvFile;
        IHostingEnvironment hostingEnvironment;

        public CustomerController(DatabaseContext databaseContext, IHostingEnvironment hostingEnvironment)
        {
            pathToCsvFile = Path.Combine(hostingEnvironment.WebRootPath, "csvFiles", "Customers.csv");
            this.databaseContext = databaseContext;
        }
        [HttpGet]
        public IActionResult GetCustomers()
        {
            var customerList = databaseContext.Customers;
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
        [Route("csv")]
        //public async Task<IActionResult> XLSPage(IFormFile xlsFile)
        //{
        //    var uploadsRoot = hostingEnvironment.WebRootPath;
        //    var filePath = Path.Combine(uploadsRoot, xlsFile.FileName).ToString();
        //    int logCounter = 0;
        //    if (xlsFile.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
        //    {
        //        try
        //        {
        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await xlsFile.CopyToAsync(fileStream);
        //                fileStream.Dispose();
        //                var package = new ExcelPackage(new FileInfo(filePath));

        //                ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

        //                //Get the values in the sheet
        //                object[,] valueArray = workSheet.Cells.GetValue<object[,]>();

        //                int maxRows = workSheet.Dimension.End.Row;
        //                int maxColumns = workSheet.Dimension.End.Column;
        //                List<DmgRegisterVM> claims = new List<DmgRegisterVM>();

        //                for (int col = 0; col < maxColumns; col++)
        //                {
        //                    dictionaryColIndexs.Add(valueArray[0, col].ToString(), col);
        //                }

        //                for (int row = 1; row < maxRows; row++)
        //                {
        //                    var c = new DmgRegisterVM();
        //                    c.ClientName = valueArray[row, MapCol("Client name")] != null ? (string)valueArray[row, MapCol("Client name")] : null;
        //                    c.Uwyear = valueArray[row, MapCol("UW Year")] != null ? valueArray[row, MapCol("UW Year")].ToString() : null;
        //                    c.AgreementNo = valueArray[row, MapCol("Agreement Number")] != null ? (string)valueArray[row, MapCol("Agreement Number")] : null;
        //                    c.AgreementName = valueArray[row, MapCol("Agreement name")] != null ? (string)valueArray[row, MapCol("Agreement name")] : null;
        //                    c.BusinessType = valueArray[row, MapCol("BusinessType")] != null ? (string)valueArray[row, MapCol("BusinessType")] : null;
        //                    c.TotalPaidInsurerShare = (valueArray[row, MapCol("TotalPaidInsurerShare")] != null) ? Decimal.Parse(valueArray[row, MapCol("TotalPaidInsurerShare")].ToString()) : (decimal?)null;
        //                    claims.Add(c);
        //                    logCounter++;
        //                }
        //                context.SaveRegToDb(claims);
        //                //If it's suppose to delete the excel file after importing the data from it
        //                FileInfo file = new FileInfo(Path.Combine(uploadsRoot, xlsFile.FileName));
        //                file.Delete();
        //                #endregion

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            String innerMessage = (ex.InnerException != null)
        //              ? ex.InnerException.Message
        //              : "";
        //            //var str = e.Message;
        //            TempData["XlsConvertErrorMsg"] = "Converting fail, check if your data is correct";
        //            return View();
        //        }

        //    }
        //    else
        //    {
        //        TempData["XlsUploadErrorMsg"] = "Uploadin fail, check if your file is xls";
        //        return View();
        //    }
        //    TempData["XlsUploadConvertSuccesMsg"] = "Uploading, converting of Excel succeeded!";
        //    TempData["LogMsg"] = logCounter;
        //    return PartialView(nameof(ImportExportXlsController.Index)); ;
        //}
        public IActionResult UploadCSV()
        {
            databaseContext.Customers.RemoveRange(databaseContext.Customers);

            using (var sr = System.IO.File.OpenText(pathToCsvFile))
            {
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
    }
}
