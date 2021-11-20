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
            DateTime dtime = Convert.ToDateTime(date);
            TimeSlot slot = dtToTs(dtime);
            List<double> sum = GetData(portfolio, owner, instrument, date);
            if (sum == null || sum.Count() == 0)
                return NotFound();
            ReturnInfo response = new ReturnInfo() { Price = sum.Average(), Date = tsToDt(slot) };
            return Json(response);
        }

        [HttpGet("benchmark")]
        public async Task<IActionResult> Benchmark(string portfolio, string date)
        {
            DateTime dtime = Convert.ToDateTime(date);
            TimeSlot slot = dtToTs(dtime);
            List<double> sum = GetData(portfolio, date).Select(x => x.Price).ToList();
            sum.Sort();
            sum = GetBenchmark(sum);
            if (sum.Count() == 0)
                return NotFound();
            ReturnInfo response = new ReturnInfo() { Price = sum.Average(), Date = tsToDt(slot) };
            return Json(response);
        }
        [HttpGet("aggregate")]
        public async Task<IActionResult> Aggregate(string portfolio, string startdate, string enddate, int intervals)
        {
            DateTime startdtime = Convert.ToDateTime(startdate);
            DateTime enddtime = Convert.ToDateTime(enddate);
            List<TimeSlot> slots = new List<TimeSlot>();
            while(startdtime < enddtime)
            {
                slots.Add(dtToTs(startdtime));
                startdtime = startdtime.AddSeconds(10000);
            }
            List<List<TimeSlot>> groups = new List<List<TimeSlot>>();
            int count = slots.Count() / intervals;
            for(int i = 0; i < intervals; i++)
            {
                List<TimeSlot> temp = new List<TimeSlot>();
                if (count * (i + 1) < slots.Count())
                {
                    for (int j = count * i; j < count * (i + 1); j++)
                        temp.Add(slots[j]);
                    groups.Add(temp);
                }
            }
            int g = 0;
            if (count * intervals < slots.Count())
                for(int i = count* intervals; i < slots.Count(); i++)
                {
                    groups[g].Add(slots[i]);
                    g++;
                }
            List<ReturnInfo> response = new List<ReturnInfo>();
            foreach(var list in groups)
            {
                List<double> sum = new List<double>();
                DateTime date = new DateTime();
                foreach (var slot in list)
                {
                    var l = GetData(portfolio, slot.Start.DateTime);
                    if (l.Count() > 0)
                    {
                        var sorted = l.OrderBy(x => x.Price).ToList();
                        date = sorted[0].Date;
                        var t = GetBenchmark(sorted.Select(x => x.Price).ToList());
                        if(t.Count() > 0)
                            sum.Add(t.Average());
                    }
                }
                response.Add(new ReturnInfo() { Price = sum.Average(), Date = Convert.ToDateTime(date) });
            }
            return Json(response);
        }
        //Вирахунок квартилів і середньої ціни для відповіді
        [NonAction]
        public List<double> GetBenchmark(List<double> list)
        {
            List<double> sum = new List<double>();
            int n = list.Count();
            int Q1 = (int)Math.Ceiling(Convert.ToDouble((n - 1) / 4));
            int Q3 = (int)Math.Ceiling(Convert.ToDouble((3 * n - 3) / 4));
            double IQR = (list[Q3] - list[Q1]);
            int price1 = (int)(list[Q1] - (1.5 * IQR));
            int price3 = (int)(list[Q3] + (1.5 * IQR));
            if (list.Count() == 1)
                sum.Add(list[0]);
            for (int i = 0; i < list.Count(); i++)
                if (list[i] > price1 && list[i] < price3)
                    sum.Add(list[i]);
            return sum;
        }

        [NonAction]
        public List<InstrumentDTO> GetData(string portfolio, string date)
        {
            //Перевіряє вірність даних
            if (portfolio == "" || portfolio == "Unknown"
                || date == "" || date == "Unknown")
                return null;
            //Вибірка по даним і якщо не знаходить, вертає NotFound
            List<InstrumentDTO> list = service.GetAll().ToList();
            list = list.Where(x => x.Portfolio == portfolio).ToList();
            if (list.Count() == 0)
                return null;
            List<InstrumentDTO> sum = new List<InstrumentDTO>();
            DateTime dtime = Convert.ToDateTime(date);
            TimeSlot slot = dtToTs(dtime);
            //Вибірка по TimeSlot
            foreach (var el in list)
            {
                if (el.Date <= Convert.ToDateTime(slot.End.DateTime) && el.Date >= tsToDt(slot))
                    sum.Add(el);
            }
            return sum;
        }
        [NonAction]
        public List<double> GetData(string portfolio, string owner, string instrument, string date)
        {
            //Перевіряє вірність даних
            if (portfolio == "" || portfolio == "Unknown"
                || owner == "" || owner == "Unknown"
                || instrument == "" || instrument == "Unknown"
                || date == "" || date == "Unknown")
                return null;
            //Вибірка по даним і якщо не знаходить, вертає NotFound
            List<InstrumentDTO> list = service.GetAll().ToList();
            list = list.Where(x => x.Portfolio == portfolio && x.Owner == owner && x.Name == instrument).ToList();
            if (list.Count() == 0)
                return null;
            List<double> sum = new List<double>();
            DateTime dtime = Convert.ToDateTime(date);
            TimeSlot slot = dtToTs(dtime);
            //Вибірка по TimeSlot
            foreach (var el in list)
            {
                if (el.Date <= Convert.ToDateTime(slot.End.DateTime) && el.Date >= tsToDt(slot))
                    sum.Add(el.Price);
            }
            return sum;
        }
        [NonAction]
        public TimeSlot dtToTs(DateTime time)
        {
            //DateTime startTime = Convert.ToDateTime("2018/01/01 00:00:00");
            //var endTime = startTime.AddSeconds(10000);
            //TimeSpan interval = time - startTime;
            //int diff = Convert.ToInt32(interval.TotalSeconds / 10000);
            //startTime = startTime.AddSeconds(diff * 10000); 
            //endTime = endTime.AddSeconds(diff * 10000);
            //return new TimeSlot()
            //{
            //    Start = new DateTimeTimeZone() { DateTime = startTime.ToString() },
            //    End = new DateTimeTimeZone() { DateTime = endTime.ToString() }
            //};
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
