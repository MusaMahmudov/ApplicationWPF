using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPraktika.Entities;
using WPFPraktika.Services.Interfaces;

namespace WPFPraktika.Services.Implementations
{
    public class CsvFileLoader : IFileLoader
    {
        public bool CanLoad(string filePath)
        {
            return filePath.EndsWith(".csv");  
        }

        public  async Task<List<TradeData>> LoadFileAsync(string filePath)
        {
            await Task.Delay(100);
            var tradeDataList = new List<TradeData>();
            var lines =  File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var values = line.Split(';');
                var tradeData = new TradeData
                {
                    Date = values[0],
                    Open = values[1],
                    High = values[2],
                    Low = values[3],
                    Close = values[4],
                    Volume = values[5]
                };
                tradeDataList.Add(tradeData);
            }
            return tradeDataList;
        }
    }
}
