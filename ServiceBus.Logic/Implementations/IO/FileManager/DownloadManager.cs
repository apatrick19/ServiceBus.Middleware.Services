using ServiceBus.Core.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Http;

namespace ServicBus.Logic.Implementations.IO.Image
{
    public class DownloadManager 
    {

        //MemoryStream bookStuff;
        //string PdfFileName;
        //HttpRequestMessage httpRequestMessage;
        //HttpResponseMessage httpResponseMessage;


        //public DownloadManager(MemoryStream data, HttpRequestMessage request, string filename)
        //{
        //    bookStuff = data;
        //    httpRequestMessage = request;
        //    PdfFileName = filename;
        //}
        //public System.Threading.Tasks.Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        //{
        //    httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.OK);
        //    httpResponseMessage.Content = new StreamContent(bookStuff);
        //    //httpResponseMessage.Content = new ByteArrayContent(bookStuff.ToArray());  
        //    httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        //    httpResponseMessage.Content.Headers.ContentDisposition.FileName = PdfFileName;
        //    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        //    return System.Threading.Tasks.Task.FromResult(httpResponseMessage);
        //}
        public static string DownloadRemotesite(string Url, string LocalFilePath)
        {
            try
            {
                Trace.TraceInformation($"inside download client ");
                using (var webClient = new WebClient())
                {
                    Trace.TraceInformation("inside web client....downloading file in few seconds");
                    webClient.DownloadFile(Url, LocalFilePath);

                    Trace.TraceInformation($"inside web client....file downloaded {LocalFilePath}");
                    
                    return LocalFilePath;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return string.Empty;
            }
        }

        //public static byte[] DownloadNetworkFileAuth()
        //{
        //    try
        //    {

        //        System.IO.File.Copy(AppConfig.ConsentFormUrl, @"C:\ArmFileManager\Files\PEN100815006011_STM.pdf");

        //        var dataBytes = File.ReadAllBytes(@"C:\ArmFileManager\Files\PEN100815006011_STM.pdf");

        //        return dataBytes;

        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
        //        return null;
        //    }
        //}
    }
}
