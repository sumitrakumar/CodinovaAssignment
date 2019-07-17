using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodinovaAssignment.Helper;
using CodinovaAssignment.Model;
using CodinovaAssignment.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodinovaAssignment.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductDetailsController : Controller
    {
        private codinovaTestContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductDetailsController(codinovaTestContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("GetProduct")]
        public async Task<ActionResult> GetProduct()
        {
            var productInfo = await _dbContext.Products.ToListAsync();

            try
            {
                if (productInfo != null)
                {

                    var resultReturnModel = new ReturnModelValue(enumReturnStatus.Success, "Product_List", "Product Details", productInfo);
                    return Ok(resultReturnModel);
                }

                else
                {
                    var resultReturnModel = new ReturnModelValue(enumReturnStatus.Failed, "No Product Not Fount", " No Product Not Fount", null);
                    return Ok(resultReturnModel);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }

        }
        [HttpGet]
        [Route("GetProductById")]
        public async Task<ActionResult> GetProductById(int id)
        {
            var result = _dbContext.Products.FirstOrDefault(x => x.ProductId == id);
            try
            {
                if (result != null)
                {
                    var resultReturnModel = new ReturnModelValue(enumReturnStatus.Success, "Product Details", "Product Details", result);
                    return Ok(resultReturnModel);
                }
                else
                {
                    var resultReturnModel = new ReturnModelValue(enumReturnStatus.Failed, "Product Not Found", "Product Not Found", null);
                    return Ok(resultReturnModel);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("CreateProduct")]
        public async Task<ActionResult> CreateProduct(ProductDetails model)
        {
            try
            {
                if (model != null)
                {

                    await _dbContext.Products.AddAsync(model);
                    _dbContext.SaveChanges();
                    return new OkObjectResult(new ReturnModelValue(enumReturnStatus.Success, "Product Created", "Product Created Cuccessfully.", model));
                }

                else
                {
                    return new OkObjectResult(new ReturnModelValue(enumReturnStatus.Failed, "Please Pass Proper Product", "Product Not Created.", null));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Route("EditProduct")]
        public async Task<IActionResult> EditProduct(int id, ProductDetails model)
        {
            try
            {

                if (id != model.ProductId)
                {
                    return Ok(new ReturnModelValue(enumReturnStatus.Failed, "model_invalid", "Mandatory field are required.", model));
                }
                else
                {
                    var Data = _dbContext.Products.SingleOrDefault(x => x.ProductId == id);
                    if (Data != null)
                    {
                        Data.ProductName = model.ProductName;
                        Data.Price = model.Price;
                        Data.Description = model.Description;
                        Data.ProductImage = model.ProductImage;
                        Data.Quantity = model.Quantity;
                        _dbContext.Products.Update(Data);
                        _dbContext.SaveChanges();
                        return new OkObjectResult(new ReturnModelValue(enumReturnStatus.Success, "Product_updated", "Product updated successfully."));
                    }
                    else
                    {
                        var resultReturnModel = new ReturnModelValue(enumReturnStatus.Failed, "Product Not found", "product details not found.");
                        return Ok(resultReturnModel);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = _dbContext.Products.SingleOrDefault(x => x.ProductId == id);
                if (result == null)
                {
                    return Ok(new ReturnModelValue(enumReturnStatus.Failed, "No product Found to be deleted", "No Product found."));
                }
                else
                {
                    _dbContext.Remove(result);
                    _dbContext.SaveChanges();
                    return Ok(new ReturnModelValue(enumReturnStatus.Success, "Product Deleted", "Product deleted successfully."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}