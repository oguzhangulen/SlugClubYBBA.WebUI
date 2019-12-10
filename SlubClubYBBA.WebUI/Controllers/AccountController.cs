using Newtonsoft.Json;
using SlubClubYBBA.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SlubClubYBBA.WebUI.Controllers
{
    public class AccountController : Controller
    {
        string baseUrl = "https://localhost:44359/";
        // GET: Account
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel user,string ReturnUrl="")
        {
            List<MusteriModel> musteri = new List<MusteriModel>();
            using(var client=new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Musteri");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    musteri = JsonConvert.DeserializeObject<List<MusteriModel>>(EmpResponse);

                }
                //returning the employee list to view  
            }
            var UserExist = musteri.Where(s => s.TCKN == user.UserName).FirstOrDefault();
            if (UserExist != null)
            {
                if (ModelState.IsValid)
                {
                    //TODO : Kontrol Et 
                    if (string.Compare((user.Password), UserExist.Sifre) == 0)
                    {
                        string userjson = JsonConvert.SerializeObject(UserExist);
                        if (!string.IsNullOrEmpty(ReturnUrl))
                            return Redirect(ReturnUrl);
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1, UserExist.TCKN, DateTime.Now, DateTime.Now.AddMinutes(30), false, userjson
                            );
                        string enTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie("PortalCookie", enTicket);
                        Response.Cookies.Add(faCookie);
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
                        return View(user);
                    }
                }
            }
            ModelState.AddModelError("", "Kullanıcı adı veya Şifre Hatalı");
            return View(user);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            foreach (var cookie in Request.Cookies.AllKeys)
            {
                Request.Cookies.Remove(cookie);
            }
            if (Request.Cookies["PortalCookie"] != null)
            {
                var c = new HttpCookie("PortalCookie");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(MusteriModel user)
        {
            
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl+"api/Musteri/");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<MusteriModel>("Register",user);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        LoginModel us = new LoginModel();
                        us.UserName = user.TCKN;
                        us.Password = user.Sifre;
                        return RedirectToAction("Login", us);
                    }
                }
                ModelState.AddModelError(string.Empty, "Lütfen Alanları boş bırakmayınız");
                return View(user);
            }
            else
            {
                ModelState.AddModelError("", "Alanlar boş geçilemez");
                return View(user);
            }
        }
        public ActionResult SignUp()
        {
            return View();
        }
    }
}