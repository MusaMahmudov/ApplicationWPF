using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPraktika.Entities;

namespace WPFPraktika.Services.Interfaces
{
    public interface IFileLoader
    {
       bool CanLoad(string filePath);
       Task<List<TradeData>> LoadFileAsync(string filePath);

    }
}
