using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Controllers
{
  [Route("api/[controller]")]
  public class OrderController : Controller
  {
    private readonly ApiContext _ctx;

    public OrderController(ApiContext ctx)
    {
      _ctx = ctx;
    }

    //GET api/order/pageNumber/pageSize
    [HttpGet("{pageIndex:int}/{pageSize:int}")]
    public IActionResult Get(int pageIndex, int pageSize)
    {
      var data = _ctx.Orders.Include(o => o.Customer)
          .OrderByDescending(c => c.Placed);

      var page = new PaginatedResponse<Order>(data, pageIndex, pageSize);

      var totalCount = data.Count();
      var totalPages = Math.Ceiling((double)totalCount / pageSize);

      var response = new
      {
        Page = page,
        TotalPages = totalPages
      };

      return Ok(response);
    }

    [HttpGet("ByState")]
    public IActionResult ByState()
    {
      var orders = _ctx.Orders.Include(o => o.Customer).ToList();

      var groupedResult = orders.GroupBy(o => o.Customer.State)
      .ToList()
      .Select(grp => new
      {
        State = grp.Key,
        Total = grp.Sum(x => x.Total)
      }).OrderByDescending(res => res.Total)
      .ToList();

      return Ok(groupedResult);
    }

    [HttpGet("ByCustomer/{n}")]
    public IActionResult ByCustomer(int n)
    {
      var orders = _ctx.Orders.Include(o => o.Customer).ToList();

      var groupedResult = orders.GroupBy(o => o.Customer.Id)
      .ToList()
      .Select(grp => new
      {
        Name = _ctx.Customers.Find(grp.Key).Name,
        Total = grp.Sum(x => x.Total)
      }).OrderByDescending(res => res.Total)
      .Take(n)
      .ToList();

      return Ok(groupedResult);
    }

    [HttpGet("GetOrder/{id}", Name = "GetOrder")]
    public IActionResult GetOrder(int id)
    {
      var order = _ctx.Orders.Include(o => o.Customer)
      .First(o => o.Id == id);

      return Ok(order);
    }
  }
}