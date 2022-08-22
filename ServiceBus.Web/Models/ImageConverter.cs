using ServiceBus.Core;
using ServiceBus.Core.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace ServiceBus.Web.Models
{
    public class ImageConverter
    {
        /// <summary>
        /// this method returns base64 picture
        /// </summary>
        /// <param name="fileBase"></param>
        /// <returns></returns>
        public static string ConvertoBase64(HttpPostedFileBase fileBase)
        {
            try
            {
                if (fileBase == null)
                {
                    return string.Empty;
                }
                string FileExt = Path.GetExtension(fileBase.FileName).ToLower();                 
                byte[] thePictureAsBytes = new byte[fileBase.ContentLength];
                using (BinaryReader theReader = new BinaryReader(fileBase.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(fileBase.ContentLength);
                }
                string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);
                switch (FileExt)
                {
                    case ".png":
                        return $"data:image/png;base64,{thePictureDataAsString}";
                    case ".jpg":
                        return $"data:image/jpeg;base64,{thePictureDataAsString}";
                    case ".jpeg":
                        return $"data:image/jpeg;base64,{thePictureDataAsString}";
                    case ".pdf":
                        return $"data:application/pdf;base64,{thePictureDataAsString}";
                    case ".doc":
                        return $"data:application/doc;base64,{thePictureDataAsString}";
                    case ".txt":
                        return $"data:application/txt;base64,{thePictureDataAsString}";
                    default:
                       return string.Empty;
                        
                }
               
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"an error occurred in httpfilepostbased conversion {ex.Message} {ex?.InnerException?.StackTrace}");
                return string.Empty;
            }
           
        }

        /// <summary>
        /// this method returns base64 picture
        /// </summary>
        /// <param name="fileBase"></param>
        /// <returns></returns>
        public static string UploadFiletoPath(HttpPostedFileBase fileBase, string fileName, string foldername)
        {
            try
            {
                if (fileBase == null && fileBase.ContentLength > 0)
                {
                   
                    return string.Empty;
                }
                string FileExt = Path.GetExtension(fileBase.FileName).ToLower();
                string FilePath = $@"{BaseService.GetAppSetting("filePath")}{foldername}\{fileName}{FileExt}";
                fileBase.SaveAs(FilePath);
                string Url = BaseService.GetAppSetting("serverPath") + foldername +"/"+ fileName+FileExt;
                return Url;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"an error occurred in httpfilepostbased conversion {ex.Message} {ex?.InnerException?.StackTrace}");               
                return string.Empty;
            }

        }
    }


}