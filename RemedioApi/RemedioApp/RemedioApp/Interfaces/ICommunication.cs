using RemedioApp.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemedioApp.Interfaces
{
    public interface ICommunication
    {
        Task<ApiReturn> GetMethod(string url);
        Task<ApiReturn> PostMethod(string url, string json, Dictionary<string, string> header);
    }
}
