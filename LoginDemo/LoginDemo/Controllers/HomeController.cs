using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginDemo.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LoginDemo.Controllers
{
    public class HomeController : Controller
    {
        private string token = RSAClass.Encrypt("token");
        private static readonly HttpClient client = new HttpClient();

        public async Task<IActionResult> Index()
        {
            HttpContext.Session.Clear();
            return View(await getAllUsers());
        }
        
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return View();
            return RedirectToAction("Details/");
        }

        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> LoginUser(User user)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                TokenProvider _tokenProvider = new TokenProvider(await getAllUsers());
                var userToken = _tokenProvider.LoginUser(user.Username.Trim(), user.Password.Trim());
                if (userToken != null)
                {
                    HttpContext.Session.SetString("JWToken", userToken);
                    return RedirectToAction("Details/");
                }
                ViewBag.Message = "Incorrect Username or Password";
                return View(user);
            }
            return RedirectToAction("Details/");
        }

        [ActionName("Logoff")]
        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return View(await getUser());
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind] User emp)
        {
            if (ModelState.IsValid)
            {
                List<User> users = await getAllUsers();
                foreach (var user in users)
                {
                    if (user.Username.ToLower().Trim() == emp.Username.ToLower().Trim())
                    {
                        ModelState.AddModelError("Username", "Username already Exists");
                        return View(emp);
                    }
                }
                //await client.GetStringAsync("https://localhost:44330/create?name="+emp.Name+"&gender="+emp.Gender+"&department="+emp.Department+"&username="+emp.Username.Trim()+"&password="+emp.Password);
                await client.PostAsJsonAsync("https://localhost:44330/api/employees?token=" + token, emp);
                return RedirectToAction("Index");
            }
            return View(emp);
        }

        //[Authorize]
        public async Task<ActionResult> Delete()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return View(await getUser());
            return RedirectToAction("Login");
        }

        //[Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmp()
        {
            //await client.GetStringAsync("https://localhost:44330/delete?data="+RSAClass.Encrypt(HttpContext.User.Identity.Name));
            await client.DeleteAsync($"https://localhost:44330/api/employees/{Int16.Parse(HttpContext.User.Identity.Name)}?token=" + token);
            return RedirectToAction("Index");
        }

        //[Authorize]
        [HttpGet]
        public async Task<ActionResult> Edit()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return View(await getUser());
            return RedirectToAction("Login");
        }

        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind] User emp)
        {
            if (ModelState.IsValid)
            {
                List<User> users = await getAllUsers();
                foreach (var user in users)
                {
                    if (user.Username.ToLower().Trim() == emp.Username.ToLower().Trim() && user.ID!= Int16.Parse(HttpContext.User.Identity.Name))
                    {
                        ModelState.AddModelError("Username", "Username already Exists");
                        return View(emp);
                    }
                }
                emp.ID = Int16.Parse(HttpContext.User.Identity.Name);
                //await client.GetStringAsync("https://localhost:44330/edit?id="+Int16.Parse(HttpContext.User.Identity.Name)+"&name="+emp.Name+"&gender="+emp.Gender+"&department="+emp.Department+"&username="+emp.Username.Trim()+"&password="+emp.Password);
                await client.PutAsJsonAsync("https://localhost:44330/api/employees?token=" + token, emp);
                return RedirectToAction("Details");
            }
            return View(emp);
        }

        public async Task<User> getUser()
        {
            //var responseString = await client.GetStringAsync("https://localhost:44330/emp?data="+ RSAClass.Encrypt(HttpContext.User.Identity.Name));
            var responseString = await client.GetStringAsync($"https://localhost:44330/api/employees/{Int16.Parse(HttpContext.User.Identity.Name)}?token="+token);
            JsonTextReader rs = new JsonTextReader(new StringReader(responseString));
            return new JsonSerializer().Deserialize(rs, typeof(User)) as User;
        }

        public async Task<List<User>> getAllUsers()
        {
            //var responseString = await client.GetStringAsync("https://localhost:44330/all?data="+RSAClass.Encrypt("all"));
            var responseString = await client.GetStringAsync("https://localhost:44330/api/employees?token="+token);
            JsonTextReader rs = new JsonTextReader(new StringReader(responseString));
            return new JsonSerializer().Deserialize(rs, typeof(List<User>)) as List<User>;
        }
    }
}