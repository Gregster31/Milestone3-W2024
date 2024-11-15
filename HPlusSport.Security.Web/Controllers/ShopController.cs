using HPlusSport.Security.Web.Classes;
using HPlusSport.Security.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.Security.Web.Controllers;

[Authorize]
[IgnoreAntiforgeryToken(Order = 1001)]
public class ShopController : Controller
{
    private readonly ShopContext _context;

    public ShopController(ShopContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

    // GET: Shop
    public ActionResult Index()
    {
        var categories = _context.Categories.Include("Products").ToList();
        return View(categories);
    }

    // GET: Category
    public ActionResult Category(int id)
    {
        var category = _context.Categories.Include("Products").FirstOrDefault(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    // POST: AddToCart
    [HttpPost]
    public ActionResult AddToCart(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null)
        {
            ShopManager.AddToCart(product);
        }
        return Redirect(HttpContext.Request.GetTypedHeaders()?.Referer?.ToString() ?? "~/Shop");
    }

    // GET: Cart
    public ActionResult Cart()
    {
        var products = ShopManager.GetCart();
        return View(products);
    }

    // POST: Order
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Order()
    {
        var order = ShopManager.CreateOrder(_context);
        return View("ThankYou", order);
    }

    // GET: Shop/Search
    public ActionResult Search(string q)
    {
        ViewData["SearchTerm"] = q;
        return View();
    }

    // GET: Shop/AdminOrders
    public ActionResult AdminOrders()
    {
        var orders = _context.Orders.ToList();
        return View(orders);
    }

    // GET: Shop/AdminOrder/1
    public ActionResult AdminOrder(int id)
    {
        var order = _context.Orders.Include("Products").Where(o => o.Id == id).FirstOrDefault();
        if (order == null)
        {
            return NotFound();
        }

        var totalAmount = order.Products.Sum(p => p.Price);
        ViewData["TotalAmount"] = totalAmount;

        return View(order);
    }
}
