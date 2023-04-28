using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;
using TestCoreMVC.Models;

namespace TestCoreMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ProductController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public async Task<IActionResult> VerProducto()
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://localhost:7120/product");

            var prod = JsonConvert.DeserializeObject<List<Producto>>(json);
            var nombresProductos = prod.AsEnumerable().Select(p => p.NombreProd);

            return View(prod);
        }
        // GET: PRODUCTOS/Create
        public ActionResult CreateProduct()
        {
            return View();
        }

        // POST: Product/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct([Bind("IdProd,NombreProd,PrecioProd,EstadoProd")] Producto producto)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://localhost:7120/product");

            var productList = JsonConvert.DeserializeObject<List<Producto>>(json);

            if (ModelState.IsValid)
            {
                var productoExistente = productList.FirstOrDefault(p => p.IdProd == producto.IdProd);

                if (productoExistente == null)
                {
                    // Serializar el objeto producto en formato JSON y enviarlo mediante un método POST
                    var response = await httpClient.PostAsync("https://localhost:7120/product", new StringContent(JsonConvert.SerializeObject(producto), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        productList.Add(producto);
                        return RedirectToAction("VerProducto");
                    }
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        ModelState.AddModelError("", "Es el mismo ID");
                    }
                }
                else
                {
                    ModelState.AddModelError("IdProd", "Ya existe un producto con este Id.");
                }
            }
            ModelState.AddModelError("IdProd", "Ya existe un producto con este Id.");
            return RedirectToAction("CreateProduct");
        }




        //// GET: PRODUCTOS/Edit/5
        //public async Task<ActionResult> EditProduct(int? id)
        //{
        //    var httpClient = new HttpClient();
        //    var json = await httpClient.GetStringAsync("https://localhost:7120/product");
        //    var prod = JsonConvert.DeserializeObject<List<Producto>>(json);
        //    var producto = prod.FirstOrDefault(p => p.IdProd == id);

        //    if (producto == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(producto);
        //}

        // GET: Producto/Edit/5
        public async Task<ActionResult> EditProduct(int? id)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://localhost:7120/product");
            var prod = JsonConvert.DeserializeObject<List<Producto>>(json);
            var producto = prod.FirstOrDefault(p => p.IdProd == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: PRODUCTOS/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProduct([Bind("IdProd,NombreProd,PrecioProd,EstadoProd")] Producto producto)
        {
            var httpClient = new HttpClient();

            if (ModelState.IsValid)
            {
                var jsonActualizado = JsonConvert.SerializeObject(producto);
                var content = new StringContent(jsonActualizado, Encoding.UTF8, "application/json");
                var respuesta = await httpClient.PutAsync("https://localhost:7120/product/EditProduct/" + producto.IdProd, content);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Redirige a la página de visualización de productos si la actualización se realiza correctamente
                    return RedirectToAction("VerProducto");
                }
                else
                {
                    ModelState.AddModelError("", "Ha ocurrido un error al actualizar el producto.");
                }
            }
            return View(producto);
        }

        // GET: Producto/BorrarProducto/5
        public async Task<ActionResult> BorrarProducto(int? id)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://localhost:7120/product");

            var prod = JsonConvert.DeserializeObject<List<Producto>>(json);
            if (id == null)
            {
                return Ok();
            }
            var producto = prod.FirstOrDefault(p => p.IdProd == id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Producto/BorrarProducto/5
        [HttpPost, ActionName("BorrarProducto")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrarProducto(int id)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://localhost:7120/product");
            var prod = JsonConvert.DeserializeObject<List<Producto>>(json);
            var producto = prod.FirstOrDefault(p => p.IdProd == id);

            if (producto != null)
            {
                // Si se encuentra el producto, realiza una solicitud DELETE a la API para eliminarlo
                var respuesta = await httpClient.DeleteAsync("https://localhost:7120/product/BorrarProducto/" + id);
                if (respuesta.IsSuccessStatusCode)
                {
                    // Si la eliminación fue exitosa, redirige a la página Index
                    return RedirectToAction("VerProducto");
                }
                else
                {
                    // Si la eliminación falló, muestra un mensaje de error
                    ViewBag.ErrorMessage = "Hubo un problema al eliminar el producto.";
                    return View(producto);
                }
            }
            else
            {
                // Si no se encuentra el producto, muestra un mensaje de error
                ViewBag.ErrorMessage = "No se encontró el producto.";
                return View(producto);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
