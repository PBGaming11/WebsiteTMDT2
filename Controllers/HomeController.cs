﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebsiteTMDT.Areas.Admin.Models.EF;
using WebsiteTMDT.Data;
using WebsiteTMDT.Models;

namespace WebsiteTMDT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> sanpham = _db.Products.ToList();
            return View(sanpham); ;
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
