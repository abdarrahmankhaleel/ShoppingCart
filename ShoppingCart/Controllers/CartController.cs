using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Infrastructure;
using ShoppingCart.Models;
using ShoppingCart.Models.ViewModels;

namespace ShoppingCart.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopCartDbContext context;

        public CartController(ShopCartDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            List < CartItem > cart= HttpContext.Session.GetJson<List<CartItem>>("cart") ?? new List<CartItem>();

            CartViewModel cartViewModel = new()
            {
                cartItems = cart,
                GrandTotal = cart.Sum(X => X.Price*X.Quantity)
            };
            return View(cartViewModel);
        }
        public async Task<IActionResult> Add(long id)
        {
            List <CartItem> cart= HttpContext.Session.GetJson<List<CartItem>>("cart") ?? new List<CartItem>();
            Product product =await context.Products.FindAsync(id);
            var ProductInCart = cart.FirstOrDefault(x=>x.ProductId==product.Id);
            if(ProductInCart != null)
            {
                ProductInCart.Quantity++;
            }
            else
            {
                cart.Add(new CartItem(product) );
            }
            HttpContext.Session.SetJson("cart", cart);
            return RedirectToAction(nameof(Index));
            
        }
        public async Task<IActionResult> Decrease(long id)
        {
            List <CartItem> cart= HttpContext.Session.GetJson<List<CartItem>>("cart") ?? new List<CartItem>();
            Product product =await context.Products.FindAsync(id);
            var ProductInCart = cart.FirstOrDefault(x=>x.ProductId==product.Id);
            if(ProductInCart != null)
            {
                if(ProductInCart.Quantity > 1)
                    ProductInCart.Quantity--;
                else
                {
                    cart.Remove(ProductInCart);
                }
            }
            HttpContext.Session.SetJson("cart", cart);
            return RedirectToAction(nameof(Index));
            
        }
        public async Task<IActionResult> Remove(long id)
        {
            List <CartItem> cart= HttpContext.Session.GetJson<List<CartItem>>("cart") ?? new List<CartItem>();
            Product product =await context.Products.FindAsync(id);
            var ProductInCart = cart.FirstOrDefault(x=>x.ProductId==product.Id);
            if(ProductInCart != null)
                cart.Remove(ProductInCart);
            HttpContext.Session.SetJson("cart", cart);
            return RedirectToAction(nameof(Index)); 
        }
        public  IActionResult Clear()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("cart") ?? new List<CartItem>();
             cart.Clear();
            HttpContext.Session.SetJson("cart", cart);
            return RedirectToAction(nameof(Index));
        }
    }
}
