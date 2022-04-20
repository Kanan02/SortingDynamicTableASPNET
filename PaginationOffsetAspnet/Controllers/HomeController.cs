using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaginationOffsetAspnet.Models;
using PaginationOffsetAspnet.Repositories;
using PaginationOffsetAspnet.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PaginationOffsetAspnet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDbConnection _connection;
        private readonly UserViewModel userViewModel = new UserViewModel();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _connection=UserContext.GetConnection();

        }

        public async Task<IActionResult> Index()
        {
            using (IDbConnection db = _connection)
            {
                string query = @"SELECT * FROM Users Order by Id
                            OFFSET 0 ROWS 
                            FETCH First 3 ROWS ONLY; ";

                IEnumerable<User> users = await db.QueryAsync<User>(query);

                userViewModel.SortedValue = "Id";
                userViewModel.Page = 1;
                userViewModel.Users = users;
                userViewModel.ValPerPage = 3;
                return View(userViewModel);
            }
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
        [HttpGet]
        public async Task<IActionResult> GetUsers(int num,int pagenum,string orderedVal)
        {
            if (pagenum<=0)
            {
                pagenum = 1;
            }
            using (IDbConnection db = _connection)
            {
                string query = @$"SELECT * FROM Users Order by {orderedVal}
                            OFFSET @OFFROWS ROWS 
                            FETCH NEXT @NUM ROWS ONLY; ";
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ORDEREDVALUE", orderedVal);
                dynamicParameters.Add("@NUM", num);
                dynamicParameters.Add("@OFFROWS", (pagenum-1)*num);
                IEnumerable<User> users=await db.QueryAsync<User>(query, dynamicParameters);
                userViewModel.Users = users;
                userViewModel.Page = pagenum;
                userViewModel.ValPerPage = num;
                userViewModel.SortedValue = orderedVal;
                return View("Index",userViewModel);
            }
        }

    }
}
