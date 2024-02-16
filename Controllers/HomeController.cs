using DataTableServerSide_GenericSortNdSearchMethod.DbContextFile;
using DataTableServerSide_GenericSortNdSearchMethod.Enities;
using DataTableServerSide_GenericSortNdSearchMethod.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DataTableServerSide_GenericSortNdSearchMethod.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatatabelexampledbContext _context;

        public HomeController(ILogger<HomeController> logger,DatatabelexampledbContext context)             
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetEmployeeDetails()
        {
            // getting all Vteuv data                  
            //var listEmployee = new List<EmployeeTable>();
            //listEmployee = _context.EmployeeTable.ToList();

            IQueryable<EmployeeTable> listEmployee = _context.EmployeeTable.AsQueryable();
            var searchFilters = new List<string>();
            for (int i = 0; i < 26; i++)
            {
                searchFilters.Add(Request.Form[$"columns[{i}][search][value]"]);
                var value = Request.Form[$"columns[{i}][search][value]"].ToString();
                var name = Request.Form[$"columns[{i}][data]"].ToString();
                if (!String.IsNullOrEmpty(value))
                {
                    var lambda =Helper.SearchHelper.BuildLambda<EmployeeTable>(name, value);
                    listEmployee = listEmployee.Where(lambda).AsQueryable();
                }

            }

            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

            // Skip number of Rows count  
            var start = Request.Form["start"].FirstOrDefault();

            // Paging Length 10,20  
            var length = Request.Form["length"].FirstOrDefault();

            // Sort Column Direction (asc, desc)  

            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            int ColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault());
            var sortBy = Request.Form[$"columns[{ColumnIndex}][data]"];

            listEmployee = Helper.SortHelper.Sort(listEmployee, sortBy, sortColumnDirection);

            // Search Value from (Search box)  
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                listEmployee = Helper.SearchHelper.SearchInAllColumns(listEmployee, searchValue);
            }

            //Paging Size (10, 20, 50,100)  
            int pageSize = length != null ? Convert.ToInt32(length) : 0;

            int skip = start != null ? Convert.ToInt32(start) : 0;

            int recordsTotal = 0;

            //total number of rows counts   
            recordsTotal = listEmployee.Count();
            //Paging   
            var data = listEmployee.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data  
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
