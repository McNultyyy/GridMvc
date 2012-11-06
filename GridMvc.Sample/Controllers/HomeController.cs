﻿using System.Linq;
using System.Web.Mvc;
using GridMvc.Sample.Models;

namespace GridMvc.Sample.Controllers
{


    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {

            var repository = new OrdersRepository();

            ViewBag.ActiveMenuTitle = "Demo";
            return View(repository.GetAll().ToList());
        }

        public ActionResult About()
        {
            ViewBag.ActiveMenuTitle = "About";
            return View();
        }

        [HttpPost]
        public JsonResult GetOrder(int id)
        {
            var repository = new OrdersRepository();
            Order order = repository.GetById(id);
            if (order == null)
                return Json(new { Status = 0, Message = "Not found" });

            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("_OrderInfo", order) });
        }
        [HttpPost]
        public JsonResult GetCustomersNames()
        {
            var repository = new CustomersRepository();
            var allItems = repository.GetAll().Select(c => c.CompanyName);
            return Json(new { Items = allItems });
        }
    }
}