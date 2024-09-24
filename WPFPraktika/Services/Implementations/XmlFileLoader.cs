using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;
using WPFPraktika.Entities;
using WPFPraktika.Services.Interfaces;

namespace WPFPraktika.Services.Implementations
{
    public class XmlFileLoader : IFileLoader
    {
        public bool CanLoad(string filePath)
        {
            return filePath.EndsWith(".xml");
        }

        public async Task<List<TradeData>> LoadFileAsync(string filePath)
        {
            await Task.Delay(100);
            XmlDocument xmlDocument = new XmlDocument();
            XmlNodeList xmlNodeList;

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            xmlDocument.Load(fs);
            xmlNodeList = xmlDocument.GetElementsByTagName("value");
            List<TradeData> tradeDatas = new List<TradeData>();

            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                var tradeData = new TradeData()
                {
                    Open = xmlNodeList[i].Attributes["open"].Value,
                    Close = xmlNodeList[i].Attributes["close"].Value,
                    Date = xmlNodeList[i].Attributes["date"].Value,
                    Volume = xmlNodeList[i].Attributes["volume"].Value,
                    High = xmlNodeList[i].Attributes["high"].Value,
                    Low = xmlNodeList[i].Attributes["low"].Value
                };
                tradeDatas.Add(tradeData);
            }
            return tradeDatas;

        }
    }
}
