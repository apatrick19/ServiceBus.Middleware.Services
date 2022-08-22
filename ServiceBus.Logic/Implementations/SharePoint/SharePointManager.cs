//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using ServicBus.Logic.Contracts;
//using ServiceBus.Core.Settings;
//using ServiceBus.Logic.Implementations;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.SharePoint.Client;
////using Microsoft.SharePoint.Client.Runtime;

//namespace ServicBus.Logic.Implementations.SharePoint
//{
//    public class SharePointManager: ISharePointManager
//    {

//        public static byte[] DownLoadFile(string Message)
//        {
//            try
//            {
//                Trace.TraceInformation($"entered the share point download manager, {Message}");
//                string message = Message;

//                int position = message.IndexOf(AppConfig.CRMBaseFolder);

//                string returnedMessage = message.Substring(position, message.Length - position);

//                string finalUrl = $"{AppConfig.SPURL}/_api/web/GetFolderByServerRelativeUrl('{returnedMessage} ')/Files";

//                Trace.TraceInformation("about calling sharepoint folder / file properties ");
//                var result = new ApiPostAndGet().UrlGet(finalUrl, "",AppConfig.SPUSerName,AppConfig.SPPassword,AppConfig.SPDomain);
//                Trace.TraceInformation($"share point result {result}");
//                var ResultObject = JsonConvert.DeserializeObject<RootObject>(result);

//                var fileName = ResultObject.d.results.FirstOrDefault().Name;
//                Trace.TraceInformation($"share point file name {fileName}");
//                string localfileName = $@"{AppConfig.FolderManager}\testresult.jpg";

//                var resultFile= new ApiPostAndGet().DownloadFile($"{AppConfig.SPURL}/_api/web/GetFolderByServerRelativeUrl('{returnedMessage} ')/Files('{fileName}')/$value", "", AppConfig.SPUSerName, AppConfig.SPPassword, AppConfig.SPDomain, localfileName);
//                if (resultFile!=null)
//                {
//                    Trace.TraceInformation($"successfully converted file int byte array , about to return");
                   
//                }
//                return resultFile;
//            }
//            catch (Exception ex)
//            {
//                Trace.TraceInformation($"returning null an error occurred for share point downlood  {ex.Message}; {ex.StackTrace.ToString()}");
//                return null;
//            }
//        }

//        public static string DownLoadFileToBase64(string Message, string FileName)
//        {
//            string Base64file = string.Empty;
//            try
//            {
//               Console.WriteLine($"entered the share point download manager, {Message}");
//                string message = Message;

//                int position = message.IndexOf(AppConfig.CRMBaseFolder);

//                string returnedMessage = message.Substring(position, message.Length - position);

//                string finalUrl = $"{AppConfig.SPURL}/_api/web/GetFolderByServerRelativeUrl('{returnedMessage} ')/Files";

//                Console.WriteLine("about calling sharepoint folder / file properties ");
//                var result = new ApiPostAndGet().UrlGet(finalUrl, "", AppConfig.SPUSerName, AppConfig.SPPassword, AppConfig.SPDomain);
//                Console.WriteLine($"share point result {result}");
//                var ResultObject = JsonConvert.DeserializeObject<RootObject>(result);

//                var fileName = ResultObject.d.results.FirstOrDefault().Name;
//                Console.WriteLine($"share point file name {fileName}");
                
//                //string localfileName = $@"{AppConfig.FolderManager}\testresult.jpg";

//                var resultFile = new ApiPostAndGet().DownloadFile($"{AppConfig.SPURL}/_api/web/GetFolderByServerRelativeUrl('{returnedMessage} ')/Files('{fileName}')/$value", "", AppConfig.SPUSerName, AppConfig.SPPassword, AppConfig.SPDomain, FileName);
//                if (resultFile != null)
//                {
//                    Console.WriteLine($"successfully converted file int byte array , about to convert to base64");
//                    Base64file= Convert.ToBase64String(resultFile);

//                }
//                return Base64file;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"returning null an error occurred for share point downlood  {ex.Message}; {ex.StackTrace.ToString()}");
//                return string.Empty;
//            }
//        }

//        //public static string DownLoadFileToBase64(string Message, string FileName)
//        //{
//        //    string Base64file = string.Empty;
//        //    try
//        //    {
//        //        Trace.TraceInformation($"entered the share point download manager, {Message}");
//        //        string message = Message;

//        //        int position = message.IndexOf(AppConfig.CRMBaseFolder);

//        //        string returnedMessage = message.Substring(position, message.Length - position);

//        //        string finalUrl = $"{AppConfig.SPURL}/_api/web/GetFolderByServerRelativeUrl('{returnedMessage} ')/Files";

//        //        Trace.TraceInformation("about calling sharepoint folder / file properties ");
//        //        var result = new ApiPostAndGet().UrlGet(finalUrl, "", AppConfig.SPUSerName, AppConfig.SPPassword, AppConfig.SPDomain);
//        //        Trace.TraceInformation($"share point result {result}");
//        //        var ResultObject = JsonConvert.DeserializeObject<RootObject>(result);

//        //        var fileName = ResultObject.d.results.FirstOrDefault().Name;
//        //        Trace.TraceInformation($"share point file name {fileName}");

//        //        //string localfileName = $@"{AppConfig.FolderManager}\testresult.jpg";

//        //        var resultFile = new ApiPostAndGet().DownloadFile($"{AppConfig.SPURL}/_api/web/GetFolderByServerRelativeUrl('{returnedMessage} ')/Files('{fileName}')/$value", "", AppConfig.SPUSerName, AppConfig.SPPassword, AppConfig.SPDomain, FileName);
//        //        if (resultFile != null)
//        //        {
//        //            Trace.TraceInformation($"successfully converted file int byte array , about to convert to base64");
//        //            Base64file = Convert.ToBase64String(resultFile);

//        //        }
//        //        return Base64file;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Trace.TraceInformation($"returning null an error occurred for share point downlood  {ex.Message}; {ex.StackTrace.ToString()}");
//        //        return string.Empty;
//        //    }
//        //}

//        public static bool UploadFile(string FilePath, string LibraryName)
//        {
//            try
//            {
//                //string filePath = @"C:\Users\gm876xp\Downloads\ErrorDetails.txt";
//                // string LibrayName = "ESB Docs";
//                string filePath = FilePath;
//                string LibrayName = LibraryName;              

//                string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

//                using (ClientContext client = new ClientContext(AppConfig.SPURL))
//                {
//                    client.AuthenticationMode = ClientAuthenticationMode.Default;
//                    client.Credentials = new NetworkCredential(AppConfig.SPUSerName, AppConfig.SPPassword, AppConfig.SPDomain);
//                    FileCreationInformation fcinfo = new FileCreationInformation();
//                    fcinfo.Url = fileName;
//                    fcinfo.Overwrite = true;
//                    fcinfo.Content = System.IO.File.ReadAllBytes(filePath);

//                    Web myweb = client.Web;
//                    List myLibrary = myweb.Lists.GetByTitle(LibrayName);
//                    myLibrary.RootFolder.Files.Add(fcinfo);
//                    client.ExecuteQuery();
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//                Trace.TraceInformation($"An error occurred {ex.Message}---{ex}");
//                return false; 
//            }
//        }

//        public static bool UploadFilewithMetaData(string FilePath, string LibraryName)
//        {
//            try
//            {
//                string fileName = FilePath.Substring(FilePath.LastIndexOf("\\") + 1);
//                using (ClientContext client = new ClientContext(AppConfig.SPURL))
//                {
//                    client.AuthenticationMode = ClientAuthenticationMode.Default;
//                    client.Credentials = new NetworkCredential(AppConfig.SPUSerName, AppConfig.SPPassword, AppConfig.SPDomain);
//                    FileCreationInformation fcinfo = new FileCreationInformation();
//                    fcinfo.Url = fileName;
//                    fcinfo.Overwrite = true;
//                    fcinfo.Content = System.IO.File.ReadAllBytes(FilePath);

//                    Web myweb = client.Web;
//                    List myLibrary = myweb.Lists.GetByTitle(LibraryName);
//                    Microsoft.SharePoint.Client.File uploadedFile = myLibrary.RootFolder.Files.Add(fcinfo);
//                    client.Load(myLibrary.ContentTypes);
//                    client.ExecuteQuery();

//                    ContentType contentType = myLibrary.ContentTypes.Where(ct => ct.Name == "Picture").FirstOrDefault();
//                    uploadedFile.ListItemAllFields["ContentTypeId"] = contentType.Id;
//                    // uploadedFile.ListItemAllFields["Title"] = "PEN12245535_ErrorDetails";
//                    uploadedFile.ListItemAllFields["RSAPIN"] = "PEN12245535";
//                    uploadedFile.ListItemAllFields["CustomerID"] = "2CR2554";
//                    uploadedFile.ListItemAllFields["ApplicationType"] = "Pin Generation";
//                    uploadedFile.ListItemAllFields.Update();
//                    client.ExecuteQuery();
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//                Trace.TraceInformation($"An error occurred {ex.Message}---{ex}");
//                return false;
//            }
//        }

//        public static bool UploadFilewithMetaData(byte[] File, string FileName, string LibraryName, string RSAPIN, string CustomerID,string ApplicationType, string EntityName,string CreatedBy)
//        {
//            try
//            {
//                //string fileName = FilePath.Substring(FilePath.LastIndexOf("\\") + 1);
//                using (ClientContext client = new ClientContext(AppConfig.SPURL))
//                {
//                    client.AuthenticationMode = ClientAuthenticationMode.Default;
//                    client.Credentials = new NetworkCredential(AppConfig.SPUSerName, AppConfig.SPPassword, AppConfig.SPDomain);
//                    FileCreationInformation fcinfo = new FileCreationInformation();
//                    fcinfo.Url = FileName;
//                    fcinfo.Overwrite = true;
//                    fcinfo.Content = File;

//                    Web myweb = client.Web;
//                    List myLibrary = myweb.Lists.GetByTitle(LibraryName);
//                    Microsoft.SharePoint.Client.File uploadedFile = myLibrary.RootFolder.Files.Add(fcinfo);
//                    client.Load(myLibrary.ContentTypes);
//                    client.ExecuteQuery();

//                    ContentType contentType = myLibrary.ContentTypes.Where(ct => ct.Name == "Picture").FirstOrDefault();
//                    uploadedFile.ListItemAllFields["ContentTypeId"] = contentType.Id;                    
//                    uploadedFile.ListItemAllFields["RSAPIN"] = RSAPIN;
//                    uploadedFile.ListItemAllFields["CustomerID"] = CustomerID;
//                    uploadedFile.ListItemAllFields["ApplicationType"] = ApplicationType;
//                    uploadedFile.ListItemAllFields["Entity"] = EntityName;
//                    uploadedFile.ListItemAllFields["CreatedBy"] = CreatedBy;
//                    uploadedFile.ListItemAllFields.Update();
//                    client.ExecuteQuery();
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//                Trace.TraceInformation($"An error occurred {ex.Message}---{ex}");
//                return false;
//            }
//        }


//    }

    
//}
