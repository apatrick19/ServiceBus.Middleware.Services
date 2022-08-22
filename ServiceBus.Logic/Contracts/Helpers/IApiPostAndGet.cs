using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServicBus.Logic.Contracts
{
    public interface IApiPostAndGet
    {
       
        T UrlPostWithRestSharp<T>(string url, string parameters, string key);
        T UrlGetWithRestSharp<T>(string url, string parameters, string key);

        T UrlGet<T>(string url, string parameters);

        T UrlGet<T>(string url, string parameters, string headertype, string password);
       string UrlGet(string url, string parameters, string username, string password, string domain);
       string UrlGet(string url, string parameters);

       string UrlPost(string url, object theObject);

       string UrlPost(string url, object theObject, string userName, string password);

        string UrlPost(string url, object theObject, string Token);

        T UrlPost<T>(string url, object theObject, bool IsCustomKey, out string message);

        string UrlGet(string url, string parameters, string token);

        T UrlGet<T>(string url, string parameters, T newItem);

        T UrlPost<T>(string url, object theObject, T newItem);

        T UrlPost<T>(string url, object theObject, string Token, T newItem);

        T UrlPost<T>(string url, object theObject, string Token, T newItem, out string message);

        T UrlPost<T>(string url, object theObject);

        

    }
}
