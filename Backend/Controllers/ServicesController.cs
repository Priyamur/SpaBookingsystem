using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using System.Reflection;
//using Backend.DTOs;

namespace Backend.Controllers
{ 
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BackendController : ControllerBase
    {
        private readonly AppDbContext _cartdetails;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;


        public BackendController(AppDbContext cartdetails, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _cartdetails = cartdetails;
            _environment = environment;
            _configuration = configuration;
        }
        [HttpGet("{id}")]
        public async Task<Service> GetById(int id)
        {
            return await _cartdetails.Services.FindAsync(id);
        }
        [HttpGet("{id}/Image")]
        public IActionResult GetImage(int id)
        {
            var cart = _cartdetails.Services.Find(id);
            if (cart == null)
            {
                return NotFound(); // User not found
            }

            // Construct the full path to the image file
            var imagePath = Path.Combine(_environment.WebRootPath, "images", cart.UniqueFileName);

            // Check if the image file exists
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound(); // Image file not found
            }

            // Serve the image file
            return PhysicalFile(imagePath, "image/jpeg");
        }


        [HttpPost]
        public async Task<ActionResult<Service>> CreateUser([FromForm] Service cart)
        {

            // Generate a unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{cart.ServiceImage.FileName}";

            // Save the image to a designated folder (e.g., wwwroot/images)
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await cart.ServiceImage.CopyToAsync(stream);
            }

            // Store the file path in the database
            cart.UniqueFileName = uniqueFileName;

           // Stock Stock = _cartdetails.stocks.Find(cart.StockId);
            Service service = new Service()
            {
                ServiceId = cart.ServiceId,
                ServiceName = cart.ServiceName,
                ServiceDescription = cart.ServiceDescription,
                ServiceCost = cart.ServiceCost,
                ServiceImage = cart.ServiceImage,
                UniqueFileName = cart.UniqueFileName,
               


            };

            _cartdetails.Services.Add(service);
            await _cartdetails.SaveChangesAsync();
            // var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{cart.UniqueFileName}";

            // Return the image URL or any other relevant response
            return Ok();


            /*var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count == 0)
                return BadRequest("No files were uploaded.");
 
            var postedFile = httpRequest.Files[0];
            if (postedFile == null)
                return BadRequest("Invalid file upload.");
 
            // Generate a unique file name (e.g., using Guid)
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(postedFile.FileName);
            var filePath = Path.Combine(HttpContext.Current.Server.MapPath("wwwroot/images/"), fileName);
 
            // Save the file to the server
            postedFile.SaveAs(filePath);
 
            //Create an Image entity and save it to the database
            var image = new Image
            {
                FileName = fileName,
                FilePath = filePath // Store the relative path
            };
 
            _cartdetails.cartDetails.Add(cart);
            await _cartdetails.SaveChangesAsync();
 
 
            // Construct the absolute URL for the image
            var imageUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/images/{fileName}";
 
            return Ok(imageUrl);*/
        }



        [HttpGet]
        public IActionResult GetAllCart()
        {
            var carts = _cartdetails.Services.ToList();

            var cartList = new List<object>();

            foreach (var cart in carts)
            {


                // Create an object containing cart details and image URL
                var Data = new
                {
                    id = cart.ServiceId,
                    name = cart.ServiceName,
                    description = cart.ServiceDescription,
                    cost = cart.ServiceCost,
                    imageUrl = String.Format("{0}://{1}{2}/wwwroot/images/{3}", Request.Scheme, Request.Host, Request.PathBase, cart.UniqueFileName)
                };

                cartList.Add(Data);
            }

            return Ok(cartList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromForm] Service updatedService)
        {
            
            var existingService = await _cartdetails.Services.FindAsync(id);

            if (existingService == null)
            {
                return NotFound(); // Mobile not found
            }

            // Update the properties of the existing mobile with the new values
            existingService.ServiceName = updatedService.ServiceName;
            existingService.ServiceDescription = updatedService.ServiceDescription;
            existingService.ServiceCost = updatedService.ServiceCost;

            // If a new image is uploaded, update the image
            if (updatedService.ServiceImage != null)
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{updatedService.ServiceImage.FileName}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updatedService.ServiceImage.CopyToAsync(stream);
                }

                // Remove the old image file
                var oldImagePath = Path.Combine(_environment.WebRootPath, "images", existingService.UniqueFileName);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Update the unique file name and save it to the database
                existingService.UniqueFileName = uniqueFileName;
            }

            // Save changes to the database
            _cartdetails.Services.Update(existingService);
            await _cartdetails.SaveChangesAsync();

            return Ok(existingService);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceDetails(int id)
        {
            var servicedetails = _cartdetails.Services.Find(id);
            if (servicedetails == null)
            {
                return NotFound(); // PetAccessory not found
            }
            _cartdetails.Services.Remove(servicedetails);
            await _cartdetails.SaveChangesAsync();
            return NoContent(); // Successfully deleted
        }


    }
}