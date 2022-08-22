using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using ServicBus.Logic.Contracts;
using CsvHelper;
using ServiceBus.Logic.Implementations.Model;
using ServiceBus.Core.Model.Generic;

namespace ServicBus.Logic.Implementations.IO.Image
{
    public class FileConverter: IFileConverter
    {
        public FileConverter()
        {

        }

        public bool WriteEmployerUpdateToCsv<T>(List<T> collection, string Path) where T : class
        {
            Trace.TraceInformation($"4mtd inside  L3 and l4 file writer method ");

            try
            {
                Trace.TraceInformation($"4mtd File stream method ");

                // Write each directory name to a file.
                using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        var csv = new CsvWriter(sw);

                        Trace.TraceInformation($"4mtd Employer File class initiated  ");
                        csv.Configuration.RegisterClassMap<EmployerUpdateMapping>();
                        csv.WriteRecords(collection);
                        csv.Flush();
                        Trace.TraceInformation($"successfully flushed and wrote employer file");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred while writing to file  see stack {ex?.Message} ; {ex?.StackTrace} ; {ex?.InnerException}");
                return false;
            }
        }

        /// <summary>
        /// Write to Csv
        /// </summary>
        /// <returns></returns>
        public bool WriteToCsv<T>(List<T> collection, string Path) where T : class
        {
            Trace.TraceInformation($"4mtd inside  L3 and l4 file writer method ");

            try
            {
                Trace.TraceInformation($"4mtd File stream method ");

                // Write each directory name to a file.
                using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        var csv = new CsvWriter(sw);
                       
                         Trace.TraceInformation($"4mtd Employer File class initiated  ");
                         csv.Configuration.RegisterClassMap<EmployerCSVMapping>();
                         csv.WriteRecords(collection);
                         csv.Flush();
                        Trace.TraceInformation($"successfully flushed and wrote employer file");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred while writing to file  see stack {ex?.Message} ; {ex?.StackTrace} ; {ex?.InnerException}");
                return false;
            }
        }

        public bool KycWriteToCsv<T>(List<T> collection, string Path) where T : class
        {
            Trace.TraceInformation($"4mtd inside  L3 and l4 file writer method ");

            try
            {
                Trace.TraceInformation($"4mtd File stream method ");

                // Write each directory name to a file.
                using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        var csv = new CsvWriter(sw);

                        Trace.TraceInformation($"Employer kyc File class initiated  ");
                        csv.Configuration.RegisterClassMap<EmployerKYCMapping>();
                        csv.WriteRecords(collection);
                        csv.Flush();
                        Trace.TraceInformation($"successfully flushed and wrote employer kyc file");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred while writing to file  see stack {ex?.Message} ; {ex?.StackTrace} ; {ex?.InnerException}");
                return false;
            }
        }
        public  byte[] ConvertImagetoByte(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Create a byte array of file stream length
                    byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                    //Read block of bytes from stream into the byte array
                    fs.Read(bytes, 0, System.Convert.ToInt32(fs.Length));
                    //Close the File Stream
                    fs.Close();
                    return bytes; //return the byte data
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see trace {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                return null;
            }
        }

        public static string ConvertFiletoBase64(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return string.Empty;
                }
                var dataBytes = File.ReadAllBytes(filePath);
                string base64String = Convert.ToBase64String(dataBytes, 0, dataBytes.Length);
                return base64String;
            }
            catch (Exception)
            {
                Trace.TraceInformation("");
                return string.Empty;
            }
        }        

        public static string ConverBase64toFile(string fileBase, string filePath)
        {
          
            try
            {
                if (string.IsNullOrEmpty(fileBase))
                {
                    return string.Empty;
                }                
                else if (fileBase.Contains("data:image/png;base64,"))
                {
                    fileBase = fileBase.Replace("data:image/png;base64,", "");
                    filePath = $"{filePath}.png";
                    File.WriteAllBytes(filePath, Convert.FromBase64String(fileBase));
                    return filePath;
                }
                else if (fileBase.Contains("data:image/jpeg;base64,"))
                {
                    fileBase =  fileBase.Replace("data:image/jpeg;base64,", "");
                    filePath = $"{filePath}.jpg";
                    File.WriteAllBytes(filePath, Convert.FromBase64String(fileBase));
                    return filePath;
                }
                else if (fileBase.Contains("data:image/jpg;base64,"))
                {
                    fileBase = fileBase.Replace("data:image/jpg;base64,", "");
                    filePath = $"{filePath}.jpg";
                    File.WriteAllBytes(filePath, Convert.FromBase64String(fileBase));
                    return filePath;
                }
                else if (fileBase.Contains("data:application/pdf;base64,"))
                {
                    fileBase = fileBase.Replace("data:application/pdf;base64,", "");
                    filePath = $"{filePath}.pdf";
                    File.WriteAllBytes(filePath, Convert.FromBase64String(fileBase));
                    return filePath;
                }
                else if (fileBase.Contains("data:application/doc;base64,"))
                {
                    fileBase = fileBase.Replace("data:application/doc;base64,", "");
                    filePath = $"{filePath}.doc";
                    File.WriteAllBytes(filePath, Convert.FromBase64String(fileBase));
                    return filePath;
                }
                else if (fileBase.Contains("data:application/txt;base64,"))
                {
                    fileBase = fileBase.Replace("data:application/txt;base64,", "");
                    filePath = $"{filePath}.txt";
                    File.WriteAllBytes(filePath, Convert.FromBase64String(fileBase));
                    return filePath;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"an error occurred in httpfilepostbased conversion {ex.Message} {ex?.InnerException?.StackTrace}");
                return string.Empty;
            }

        }

      

    }
}
