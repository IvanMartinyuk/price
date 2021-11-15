using BLL.DTO;
using BLL.Services;
using DAL.Context;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SC.DevChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]
    public class PricesController : Controller
    {
        InstrumentService service;
        public PricesController(FinancialContext context)
        {
            service = new InstrumentService(context);
        }
        [HttpGet("average")]
        public async Task<IActionResult> Average(string portfolio, string owner, string instrument, string date)
        {
            var slot = dtToTs(Convert.ToDateTime(date));
            List<InstrumentDTO> list = service.GetAll().ToList();
            list = list.Where(x => x.Portfolio == portfolio && x.Owner == owner && x.Name == instrument).ToList();
            List<double> sum = new List<double>();
            foreach (var el in list)
            {
                if (el.Date <= Convert.ToDateTime(slot.End.DateTime) && el.Date >= Convert.ToDateTime(slot.Start.DateTime))
                    sum.Add(el.Price);
            }

            return Json(sum.Average());
        }

        public TimeSlot dtToTs(DateTime start)
        {
            var end = start.AddSeconds(10000);
            return new TimeSlot()
            {
                Start = new DateTimeTimeZone() { DateTime = start.ToString() },
                End = new DateTimeTimeZone() { DateTime = end.ToString() }
            };
        }
        public DateTime tsToDt(TimeSlot slot)
        {
            return Convert.ToDateTime(slot.Start.DateTime);
        }
    }
}
