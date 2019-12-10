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

namespace SlubClubYBBA.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        string baseUrl = "https://localhost:44359/";
        public async Task<ActionResult> Index()
        {
            List<HesapModel> musteri = new List<HesapModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Hesap");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    musteri = JsonConvert.DeserializeObject<List<HesapModel>>(EmpResponse);

                }
                //returning the employee list to view  
            }
            var hesaps = musteri.Where(s => s.TCKN == HttpContext.User.Identity.Name && s.AktiflikDurumu == true).ToList();
            return View(hesaps);
        }
        public async Task<ActionResult> Create()
        {
            HesapModel hesap = new HesapModel();
            List<HesapModel> musteri = new List<HesapModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Hesap");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    musteri = JsonConvert.DeserializeObject<List<HesapModel>>(EmpResponse);

                }
                //returning the employee list to view  
            }
            var hesaps = musteri.Where(s => s.TCKN == HttpContext.User.Identity.Name).ToList();
            var x = hesaps.FirstOrDefault();
            hesap.BakiyeBilgi = 0;
            hesap.AktiflikDurumu = true;
            hesap.HesapNo = x.HesapNo;
            hesap.TCKN = x.TCKN;
            hesap.EkNo = hesaps.Count + 1000;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl + "api/Hesap/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<HesapModel>("add", hesap);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public ActionResult Delete(HesapModel hesap)
        {
            if (hesap.BakiyeBilgi != 0)
            {
                ModelState.AddModelError("", "Bakiyeniz boş değil...");
                return RedirectToAction("Index");
            }
            else
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl + "api/Hesap/");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    hesap.AktiflikDurumu = false;
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<HesapModel>("update", hesap);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
        public ActionResult ParaYatir(HesapModel hesap)
        {
            if (hesap != null)
            {
                return View(hesap);
            }
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> ParaYatirma(HesapModel hesap)
        {
            if (hesap.BakiyeBilgi < 0)
            {
                ModelState.AddModelError("", "Geçersiz değer girdiniz");
                return View("Index",hesap);
            }
            List<HesapModel> hesaps = new List<HesapModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Hesap");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    hesaps = JsonConvert.DeserializeObject<List<HesapModel>>(EmpResponse);
                }
                //returning the employee list to view  
            }
            var uhesap = hesaps.Where(a => a.HesapNo == hesap.HesapNo && a.EkNo == hesap.EkNo).FirstOrDefault();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl + "api/Hesap/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                hesap.BakiyeBilgi = uhesap.BakiyeBilgi + hesap.BakiyeBilgi;
                hesap.AktiflikDurumu = true;
                //HTTP POST
                var postTask = client.PostAsJsonAsync<HesapModel>("update", hesap);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public async Task<ActionResult> ParaCekme(HesapModel hesap)
        {
            List<HesapModel> hesaps = new List<HesapModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Hesap");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    hesaps = JsonConvert.DeserializeObject<List<HesapModel>>(EmpResponse);
                }
                //returning the employee list to view  
            }
            var uhesap = hesaps.Where(a => a.HesapNo == hesap.HesapNo && a.EkNo == hesap.EkNo).FirstOrDefault();
            if (hesap.BakiyeBilgi > uhesap.BakiyeBilgi)
            {
                ModelState.AddModelError("", "Bakiyeniz yetersiz");
                return View("ParaCek", uhesap);
            }
            if (hesap.BakiyeBilgi < 0)
            {
                ModelState.AddModelError("", "Geçersiz değer girdiniz..");
                return View("ParaCek", uhesap);
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl + "api/Hesap/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                uhesap.BakiyeBilgi = uhesap.BakiyeBilgi - hesap.BakiyeBilgi;
                uhesap.AktiflikDurumu = true;
                //HTTP POST
                var postTask = client.PostAsJsonAsync<HesapModel>("update", uhesap);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public ActionResult ParaCek(HesapModel hesap)
        {
            if (hesap != null)
            {
                return View(hesap);
            }
            return RedirectToAction("Index");
        }
    }
}