using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LuckyDraw.Helper
{
    public class FileHelper
    {
        public static async Task<string> Upload(HttpPostedFileBase fileData, string name)
        {
            //var extend = Path.GetExtension(fileData.FileName);
            //var name = Guid.NewGuid().ToString("N") + extend;
            var path = HttpContext.Current.Server.MapPath("~/Content/Images/") + name;

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                byte[] bytes = new byte[fileData.InputStream.Length];

                int numBytesRead = 0;
                int numBytesToRead = (int)fileData.InputStream.Length;
                fileData.InputStream.Position = 0;
                while (numBytesToRead > 0)
                {
                    var result = await fileData.InputStream.ReadAsync(bytes, 0, bytes.Length);
                    if (result <= 0)
                    {
                        break;
                    }
                    await fs.WriteAsync(bytes, numBytesRead, result);
                    numBytesRead += result;
                    numBytesToRead -= result;
                }

                fs.Close();
            }

            return "~/Content/Images/" + name;
        }

        public static void Rename(string path, string oName,string nName )
        {
            var oldName = Path.Combine(path,oName);
            var newName = Path.Combine(path, nName);
            
            Directory.Move(oldName, newName);
        }
    }
}