using Renci.SshNet;
using ServicBus.Logic.Contracts;
using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServicBus.Logic.Implementations.IO
{
   public class ServiceFTP: IServiceFtp
    {

        // Logger _logger = new Logger();
        // Use FTP to transfer a file.
        /// <summary>
        /// Csv to Ftp upload
        /// </summary>
        /// <param name="text"></param>
        /// <param name="FTPUri"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void CsvFtp_Upload(string text, string FTPUri, string username, string password)
        {
            try
            {
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // Get network credentials.
                request.Credentials = new NetworkCredential(username, password);

                // Write the text's bytes into the request stream.
                request.ContentLength = text.Length;
                using (Stream request_stream = request.GetRequestStream())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    request_stream.Write(bytes, 0, text.Length);
                    request_stream.Close();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex.Message}; {ex?.StackTrace} {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        /// <summary>
        /// FTP file upload 
        /// </summary>
        /// <returns></returns>
        public Response UploadToFTP(string sftpHost, int sftpPort, string sftpUsername, string sftpPassword, string localFolder, string fileName)
        {
            try
            {

                using (SftpClient sftpClient = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
                {
                    Trace.TraceInformation("Connecting to SFTP Server");
                    sftpClient.Connect();
                    var fullPath = Path.Combine(localFolder, fileName);
                    using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                    {
                        sftpClient.BufferSize = 1024;
                        sftpClient.UploadFile(fs, Path.GetFileName(fullPath));
                    }
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                    sftpClient.Dispose();

                    return null;
                    //return Response.SetResponse(ResponseDictionary.Approved, "File upload was successful");
                }

            }
            catch (Exception ex)
            {
                //Trace.TraceInformation($"An error occurred {ex}; {ex.Message}");
                //return _logger.LogExceptionWithResponse(DataDictionary.AccountCreation, Caller.CRM, ex.Message, ex.StackTrace, ex.ToString(), "FTP File Upload");
                throw;
            }
        }

        /// <summary>
        /// FTP file upload for multiple records 
        /// </summary>
        /// <returns></returns>
        public Response UploadToFTP(string sftpHost, int sftpPort, string sftpUsername, string sftpPassword, string localFolder, List<string> fileNames)
        {
            try
            {
                using (SftpClient sftpClient = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
                {
                    Trace.TraceInformation("Connecting to SFTP Server");
                    sftpClient.Connect();
                    foreach (var fileName in fileNames)
                    {
                        var fullPath = Path.Combine(localFolder, fileName);
                        using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                        {
                            //sftpClient.BufferSize = 1024;
                            //sftpClient.UploadFile(fs, Path.GetFileName(fullPath));

                            sftpClient.BufferSize = 1024;
                            sftpClient.UploadFile(fs, "/C:/Users/eycrm/CrmAccount-Milesoft/" + Path.GetFileName(fullPath));
                        }
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }
                    }
                    sftpClient.Dispose();

                    return null;
                    //return Response.SetResponse(ResponseDictionary.Approved, "File upload was successful");
                }

            }
            catch (Exception ex)
            {
                //Trace.TraceInformation($"An error occurred {ex}; {ex.Message}");
                //return _logger.LogExceptionWithResponse(DataDictionary.AccountCreation, Caller.CRM, ex.Message, ex.StackTrace, ex.ToString(), "FTP File Upload");

                throw;
            }
        }


    }
}
