using BLL.DTO;
using BLL.Services;
using DAL.Context;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using SC.DevChallenge.Api.Data;
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
            if (portfolio == "" || portfolio == "Unknown"
                || owner == "" || owner == "Unknown"
                || instrument == "" || instrument == "Unknown"
                || date == "" || date == "Unknown")
                return NotFound();
            List<InstrumentDTO> list = service.GetAll().ToList();
            list = list.Where(x => x.Portfolio == portfolio && x.Owner == owner && x.Name == instrument).ToList();
            if (list.Count() == 0)
                return NotFound();
            List<double> sum = new List<double>();
            TimeSlot temp;
            DateTime dtime = Convert.ToDateTime(date);
            TimeSlot slot = dtToTs(dtime);
            foreach (var el in list)
            {
                if (el.Date <= Convert.ToDateTime(slot.End.DateTime) && el.Date >= tsToDt(slot))
                    sum.Add(el.Price);
            }
            if (sum.Count() == 0)
                return NotFound();
            ReturnInfo response = new ReturnInfo() { Price = sum.Average(), Date = tsToDt(slot) };
            return Json(response);
        }
        [NonAction]
        public TimeSlot dtToTs(DateTime time)
        {
            DateTime startTime = Convert.ToDateTime("2018/01/01 00:00:00");
            var endTime = startTime.AddSeconds(10000);
            while (endTime < time)
            {
                startTime = startTime.AddSeconds(10000);
                endTime = endTime.AddSeconds(10000);
            }
            return new TimeSlot()
            {
                Start = new DateTimeTimeZone() { DateTime = startTime.ToString() },
                End = new DateTimeTimeZone() { DateTime = endTime.ToString() }
            };
        }
        [NonAction]
        public DateTime tsToDt(TimeSlot slot)
        {
            return Convert.ToDateTime(slot.Start.DateTime);
        }
    }
}
