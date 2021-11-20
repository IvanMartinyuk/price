using BLL.DTO;
using BLL.Services;
using DAL.Context;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SC.DevChallenge.Api
{
    public class CsvParser
    {
        public static async void Parse(FinancialContext context)
        {
            //InstrumentService service = new InstrumentService(context);
            string filepath = Directory.GetCurrentDirectory() + "/Input/data.csv";

            List<string> lines = File.ReadLines(filepath).ToList();

            for (int i = 1; i < lines.Count(); i++)
            {
                if (lines[0] != "")
                {
                    string[] data = lines[i].Split(new char[] { ',' });

                    Instrument inst = new Instrument()
                    {
                        Portfolio = data[0],
                        Owner = data[1],
                        Name = data[2],
                        Date = Convert.ToDateTime(data[3]),
                        Price = double.Parse(data[4], CultureInfo.InvariantCulture)
                    };

                    context.Instruments.Add(inst);
                }
            }

            context.SaveChanges();
        }
    }
}
