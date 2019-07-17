using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodinovaAssignment.Helper;
using CodinovaAssignment.Model;
using CodinovaAssignment.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodinovaAssignment.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailsController : Controller
    {
        private codinovaTestContext _dbContext;
        public OrderDetailsController(codinovaTestContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("PlaceOrder")]
        public ActionResult PlaceOrder(SaveOrder model)
        {
            try
            {
                if (model != null)
                {
                    var UserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                    model.OrderBy = int.Parse(UserId);
                    var product = _dbContext.Products.SingleOrDefault(x => x.ProductId == model.ProductId);
                    var availableQuantity = product.Quantity;
                    var appliedQuantity = model.Quantity;

                    if (appliedQuantity > availableQuantity)
                    {
                        var resultReturnModel = new ReturnModelValue(enumReturnStatus.Failed, "There is no such quanties vailable", "Select Within a given quantity", null);
                        return Ok(resultReturnModel);
                    }

                    else if (appliedQuantity == availableQuantity)
                    {
                        _dbContext.SaveOrders.Add(model);
                        _dbContext.SaveChanges();
                        var data = _dbContext.Products.SingleOrDefault(x => x.ProductId == model.ProductId);
                        _dbContext.Remove(data);
                        _dbContext.SaveChanges();
                        var resultReturnModel = new ReturnModelValue(enumReturnStatus.Success, "Order Purchase", "Purchased", null);
                        return Ok(resultReturnModel);
                    }
                    else if (appliedQuantity < availableQuantity)
                    {
                        _dbContext.SaveOrders.Add(model);
                        _dbContext.SaveChanges();
                        var data = _dbContext.Products.SingleOrDefault(x => x.ProductId == model.ProductId);
                        var CurrentQuantity = availableQuantity - appliedQuantity;
                        data.Quantity = CurrentQuantity;
                        _dbContext.Update(data);
                        _dbContext.SaveChanges();
                        var resultReturnModel = new ReturnModelValue(enumReturnStatus.Success, "Order Purchaes", "Purchased", null);
                        return Ok(resultReturnModel);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return null;
        }

    }
}