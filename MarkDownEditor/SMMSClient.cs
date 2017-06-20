using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MarkDownEditor
{
    public class SMMSClient
    {
        public static string UploadImageToSMMS(string path)
        {
            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");



                HttpContent fileStreamContent = new StreamContent(fs);
                fileStreamContent.Headers.Add("Content-Type", "multipart/form-data");
                formData.Add(fileStreamContent, "smfile", path);
                var res = client.PostAsync("https://sm.ms/api/upload", formData).Result;

                if (!res.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Net error!");
                }

                using (var streamReader = new StreamReader(res.Content.ReadAsStreamAsync().Result))
                {
                    JObject o = JObject.Parse(streamReader.ReadToEnd());
                    if (o["code"].ToString().ToLower() != "success")
                        throw new HttpRequestException(o["msg"].ToString());

                    return o["data"]["url"].ToString();
                }
            }
        }
    }
}
