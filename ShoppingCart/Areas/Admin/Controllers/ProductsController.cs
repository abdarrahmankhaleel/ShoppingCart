using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Infrastructure;
using ShoppingCart.Models;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ProductsController : Controller
    {
        private readonly ShopCartDbContext context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ShopCartDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 3;
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Count() / pageSize);
            var AlProducts = await context.Products.Include(x => x.Category).OrderByDescending(p => p.Id).Skip((p - 1) * pageSize).Take(pageSize).ToListAsync();
            return View(AlProducts);
        }
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model)
        { 
            if (ModelState.IsValid)
            {
                model.Slug = model.Name.ToLower().Replace(" ", "-");
                var productSame = context.Products.FirstOrDefault(x => x.Slug == model.Slug);
                if (productSame != null)
                {
                    TempData["Error"] = "the prodect alredy exists";
                    ViewBag.Categories = new SelectList(context.Categories, "Id", "Name", model.CategoryId);
                    return View(model);
                }
                if (model.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + model.ImageUpload.FileName;

                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await model.ImageUpload.CopyToAsync(fs);
                    fs.Close();

                    model.Image = imageName;
                }
                
                context.Products.Add(model);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categories = new SelectList(context.Categories, "Id", "Name",model.CategoryId);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            Product product = await context.Products.FindAsync(id);
            ViewBag.Categories = new SelectList(context.Categories, "Id", "Name",product.CategoryId);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            ViewBag.Categories = new SelectList(context.Categories, "Id", "Name", product.CategoryId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await context.Products.Where(p => p.Slug == product.Slug).ToListAsync();
                if (slug.Count > 1)
                {
                    ModelState.AddModelError("", "The product already exists.");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;

                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();

                    product.Image = imageName;
                }
                context.Products.Attach(product);
                context.Entry(product).State= EntityState.Modified;
                await context.SaveChangesAsync();
                TempData["Success"] = "The product has been edited!";
            }

            return RedirectToAction(nameof(Index));
        }
        //public async Task<IActionResult> Edit(int id,Product model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        model.Slug = model.Name.ToLower().Replace(" ", "-");
        //        var productSame = context.Products.Where(x => x.Slug == model.Slug).ToList();
        //        if (productSame.Count>1)
        //        {
        //            TempData["Error"] = "the prodect alredy exists";
        //            ViewBag.Categories = new SelectList(context.Categories, "Id", "Name", model.CategoryId);
        //            return View(model);
        //        }
        //        if (model.ImageUpload != null)
        //        {
        //            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
        //            string imageName = Guid.NewGuid().ToString() + "_" + model.ImageUpload.FileName;

        //            string filePath = Path.Combine(uploadsDir, imageName);

        //            FileStream fs = new FileStream(filePath, FileMode.Create);
        //            await model.ImageUpload.CopyToAsync(fs);
        //            fs.Close();

        //            model.Image = imageName;
        //        }

        //        context.Update(model);
        //        await context.SaveChangesAsync();
        //        TempData["Success"] = "The product has been edited!";
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        ViewBag.Categories = new SelectList(context.Categories, "Id", "Name", model.CategoryId);
        //        return View(model);
        //    }
        //}
        public async Task<IActionResult> Delete(long id)
        {
            Product product = await context.Products.FindAsync(id);
            context.Products.Remove(product);
            return RedirectToAction("Index");
        }
    }
}
        
       
    
