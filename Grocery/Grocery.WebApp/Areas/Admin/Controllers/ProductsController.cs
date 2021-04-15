using Grocery.WebApp.Areas.Admin.ViewModels;
using Grocery.WebApp.Data;
using Grocery.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Grocery.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // only a user who is authenticated and authorized is allowed to access this
    public class ProductsController : Controller
    {

        private readonly ApplicationDbContext _context; // use to retrieve records pointing to table
        private readonly ILogger<ProductsController> _logger; // readonly = no other method can modify this
        private readonly UserManager<MyIdentityUser> _userManager;

        // through DI, all the objects are initialized for you
        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger, UserManager<MyIdentityUser> userManager)
        {   
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }


        // GET: ProductsController
        public ActionResult Index() //list all data from db
        {
            // give all records from the products db
            // also get the data of child records (parent-child relationships)


            // 1. LAMBDA version to extract all products using Eager Loading

            //var products = _context.Products
            //    .Include(p => p.CreatedByUser) // include createdbyuser , add include, becomes eager loading
            //    .Include(p => p.UpdatedByUser)
            //    .ToList() ;

            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.ProductID);
            //    Console.WriteLine(p.ProductName);
            //    Console.WriteLine(p.CreatedByUser.DisplayName);
            //}

            // projection done manually here as 2nd step
            // bind data of the model to the viewmodel (called model binding)

            //List<ProductViewModel> productViewModels = new List<ProductViewModel>();

            //foreach (var p in products)
            //{
            //    productViewModels.Add(new ProductViewModel
            //    {
            //        ProductID = p.ProductID,
            //        ProductName = p.ProductName,
            //        SellingPricePerUnit = p.SellingPricePerUnit,
            //        Quantity = p.Quantity,
            //        Image = p.Image,

            //        CreatedByUser = p.CreatedByUser,
            //        CreatedByUserId = p.CreatedByUserId,
            //        UpdatedByUser = p.UpdatedByUser,
            //        UpdatedByUserId = p.UpdatedByUserId
            //    });
            //}




            // 2. LINQ version to extract all products using Eager Loading
            // and project the data into the viewmodel (powerfeature of linq, all in 1 step)
            // p = the value you are pulling from your db

            var productViewModels = (from p in _context.Products.Include(p => p.CreatedByUser).Include(p => p.UpdatedByUser)
                                     select new ProductViewModel
                                     {
                                         ProductID = p.ProductID,
                                         ProductName = p.ProductName,
                                         SellingPricePerUnit = p.SellingPricePerUnit,
                                         Quantity = p.Quantity,
                                         Image = p.Image,

                                         LastUpdatedOn = p.LastUpdatedOn,
                                         CreatedByUser = p.CreatedByUser,
                                         CreatedByUserId = p.CreatedByUserId,
                                         UpdatedByUser = p.UpdatedByUser,
                                         UpdatedByUserId = p.UpdatedByUserId
                                     }).ToList();



            // return View(products);

            return View(productViewModels);
        }

        // GET: ProductsController/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }


            //initialize the  viewmodel

            ProductViewModel productViewModel = new ProductViewModel()
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                SellingPricePerUnit = product.SellingPricePerUnit,
                Quantity = product.Quantity,
                Image = product.Image
            };
                


            return View(productViewModel);
      
        }

        // GET: ProductsController/Create
        public ActionResult Create() // called when click create new listing
        {
            ProductViewModel productViewModel = new ProductViewModel();

            return View(productViewModel); // pass empty pvm to the view
        }



        // bind -> adding an attribute to productviewmodel
        // model binding, properties submitted from browser to server, I am expecting a field called productid, etc...
        // define what are the fields a user can submit to the server (define the list of fields that can be received)
        // replace IformCollection collection with [Bind("ProductID,ProductName,Quantity,SellingPricePerUnit,Image,CreatedByUserId,UpdatedByUserId,LastUpdatedOn")] Product product


        //no need to bind imagefile, it will come to us through an attachment, accessed through a different way.


        //multi-tasking multi-threaded code
        // need to wrap ActionResult in Task
        // POST: ProductsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken] //takes care of cross-side script attacks (protect from XSRF attack = hacking process), will not allow cross-side post on your server
        public async Task<ActionResult> Create([Bind("ProductName,SellingPricePerUnit,Quantity")] ProductViewModel productViewModel) //receive collection of form objects from Create.cshtml
        {   //copy paste to create block
          
            var user = await _userManager.GetUserAsync(User); // User -> pass in the current logged in user, any async method should have await

            if (user == null)
            {
                ModelState.AddModelError("Create", "User not found... please log back in...");
            }


            if (!ModelState.IsValid)
            {
                return View(productViewModel); // if invalid, go back same page with all the error msg
            }

            
            Product newProduct = new Product()
            {
                ProductID = new Guid(),
                ProductName = productViewModel.ProductName,
                SellingPricePerUnit = productViewModel.SellingPricePerUnit,
                Quantity = productViewModel.Quantity,

                LastUpdatedOn = DateTime.Now,
                CreatedByUserId = user.Id                //need to inject userManager to access user
            };
            

            //check if the file has been attached while submitting the form
            // if at least 1 file is attached, we try to access it.

            if(Request.Form.Files.Count >= 1)
            {
                //shortcut method, in some cases wont work
                //IFormFile file = productViewModel.ImageFile;

                //extract the file you want to 
                IFormFile file = Request.Form.Files.FirstOrDefault();

                //copy the file uploaded using the momorystream - into the Product.Image
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream); // copy into datastream
                    newProduct.Image = dataStream.ToArray();
                }
            }

            //update db
            try
            {
                _context.Products.Add(newProduct);
                _context.SaveChanges(); //commit changes to db
                
                //return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            catch(System.Exception exp)
            {
                ModelState.AddModelError("Create", "Unable to update the database. Contact Admin!");
                _logger.LogError($"Create product failed: {exp.Message}");
                return View(productViewModel);
            }

            
        }


        // add ? - user might not have passed anything
        // GET: ProductsController/Edit/5
        public async Task<ActionResult> Edit(Guid? id) //change id to guid, the id it is going to receive is a guid
        {

            if (id == null)
            {
                return NotFound();
            }

            // get the data from the db, search for the data in db
            //find product whose id matches, assign to producttoedit
            Product productToEdit = await _context.Products.FindAsync(id);

            if (productToEdit == null)
            {
                return NotFound();
            }

            //initialize the  viewmodel

            ProductViewModel productViewModel = new ProductViewModel()
            {
                ProductID = productToEdit.ProductID,
                ProductName = productToEdit.ProductName,
                SellingPricePerUnit = productToEdit.SellingPricePerUnit,
                Quantity = productToEdit.Quantity,
                Image = productToEdit.Image,

                LastUpdatedOn = productToEdit.LastUpdatedOn,
                CreatedByUser = productToEdit.CreatedByUser,
                CreatedByUserId = productToEdit.CreatedByUserId,
                UpdatedByUser = productToEdit.UpdatedByUser,
                UpdatedByUserId = productToEdit.UpdatedByUserId
            };

            return View(productViewModel); // render edit page, pass in productViewModel into the page to display fields
        }

        // POST: ProductsController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Guid id, IFormCollection collection)
        //{

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProductID,ProductName,Quantity,SellingPricePerUnit,Image,CreatedByUserId," +
            "UpdatedByUserId,LastUpdatedOn")] ProductViewModel productViewModel) // after form submission on edit page, goes to this function
        {
            var user = await _userManager.GetUserAsync(User); 

            if (user == null)
            {
                ModelState.AddModelError("Create", "User not found.  Please log back in!");
            }

            if (!ModelState.IsValid)
            {
                return View(productViewModel);
            }

            // find the product you want to edit
            Product editProduct = await _context.Products.FindAsync(productViewModel.ProductID);

            if (editProduct == null)
            {
                return NotFound();
            }

            //update properties of model - from the viewmodel
            editProduct.ProductName = productViewModel.ProductName;
            editProduct.SellingPricePerUnit = productViewModel.SellingPricePerUnit;
            editProduct.Quantity = productViewModel.Quantity;
            editProduct.LastUpdatedOn = DateTime.Now;
            editProduct.UpdatedByUserId = user.Id;

            //replaced with above
            //Product newProduct = new Product() //assign value from pvm
            //{
            //    ProductID = new Guid(),
            //    ProductName = productViewModel.ProductName,
            //    SellingPricePerUnit = productViewModel.SellingPricePerUnit,
            //    Quantity = productViewModel.Quantity,

            //    LastUpdatedOn = DateTime.Now,
            //    CreatedByUserId = user.Id
            //};


            // check if file has attached while submitting the Form
            if (Request.Form.Files.Count >= 1)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                // IFormFile file = productViewModel.ImageFile;

                // copy the file uploaded using the MemoryStream - into the Product.Image
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    editProduct.Image = dataStream.ToArray();
                }
            }

            // update the database
            try
            {
                _context.Products.Update(editProduct); //update command, not add command
                _context.SaveChanges();
                return RedirectToAction(nameof(Index)); //render index page
            }
            catch (System.Exception exp)
            {
                ModelState.AddModelError("Edit", "Unable to update the Database. Contact Admin!");
                _logger.LogError($"Edit Product failed: {exp.Message}");
                return View(productViewModel);
            }
        }

        // GET: ProductsController/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            Product productToDelete = await _context.Products.FindAsync(id);

            if (productToDelete == null)
            {
                return NotFound();
            }

            // do this to show the values of the fields on the delete page
            ProductViewModel productViewModel = new ProductViewModel()
            {
                ProductID = productToDelete.ProductID,
                ProductName = productToDelete.ProductName,
                SellingPricePerUnit = productToDelete.SellingPricePerUnit,
                Quantity = productToDelete.Quantity,
                Image = productToDelete.Image
            };

            //return View();
            return View(productViewModel); // render delete page, pass in productviewmodel
        }

        // POST: ProductsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(Guid id) // after form submission on delete page, code goes here
        { 

            // find the product you want to edit
            Product productToDelete = await _context.Products.FindAsync(id);

            if (productToDelete == null)
            {
                return NotFound();
            }

            try
            {
                _context.Products.Remove(productToDelete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("Delete", "Unable to delete a row in the Database. Contact Admin!");
                _logger.LogError($"Delete Product failed: {ex.Message}");
                return View();
            }
        }
    }
}
