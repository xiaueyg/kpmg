using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using KpmgApi.Models;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KpmgApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class KpmgController : Controller
    {
        private readonly KpmgContext _context;

        public KpmgController(KpmgContext context)
        {
            _context = context;
        }

        // GET: api/GetKpmgItem
        [HttpGet("DownloadKpmgItem")]
        public async Task<ActionResult<KpmgItem>> DownloadKpmgItem(string code)
        {
            if (KpmgItemExists(code))
            {
                return Json(new { status = "error", message = $"Nasdq data for {code} already exists" });
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri("https://data.nasdaq.com/api/v3/datasets/BOE/");

                var responseTask = client.GetAsync(code + ".json");//"XUDLADD.json" XUDLCDD
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    var jObject = JObject.Parse(readTask.Result);

                   
                    dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(readTask.Result, new ExpandoObjectConverter());

                    var dataset = config.dataset;

                    var item = new KpmgItem();
                    item.Code = dataset.dataset_code;
                    item.Name = dataset.name;
                    item.Data = new List<Rate>();
                    foreach (var data in dataset.data)
                    {
                        var rate = new Rate();
                        rate.Date = data[0];
                        rate.Number = data[1];
                        item.Data.Add(rate);
                    }

                    
                    _context.KpmgItems.Add(item);
                    _context.SaveChanges();
                    return Ok(item);
                }
                else //web api sent error response 
                {
                    return Json(new { status = "error", message = "Nasdq data link API error" });
                }
            }
        }


        // GET: api/Kpmg/XUDLADD 
        [HttpGet("GetKpmgItem/{code}")]
        public async Task<ActionResult<KpmgItem>> GetKpmgItem(string code)
        {
            var kpmgItem = await _context.KpmgItems.Include(r => r.Data).FirstOrDefaultAsync(i => i.Code == code);

            if (kpmgItem == null)
            {
                return NotFound();
            }

            return kpmgItem;
        }

        

        private bool KpmgItemExists(string code)
        {
            return _context.KpmgItems.Any(e => e.Code == code);
        }
    }
}
