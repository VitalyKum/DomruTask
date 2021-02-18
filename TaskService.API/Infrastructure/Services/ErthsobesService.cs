using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TaskService.API.ViewModel;
using System.Net.Mime;
using System.Net.Http.Headers;
using System.IO;
using System.Text;
using TaskService.API.Models;

namespace TaskService.API.Infrastructure.Services
{
    public class ErthsobesService : IErthsobesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl;

        public ErthsobesService(HttpClient httpClient, IOptions<AppSettings> settings)
        {        
            _httpClient = httpClient;
            _remoteServiceBaseUrl = $"{settings.Value.ErthsobesserviceUrl}/Api";
        }
        public async Task<ObjectInfo> GetObjectInfo(string dataType)
        {
            var uri = $"{_remoteServiceBaseUrl}/GetObjectInfo?type={dataType}";
            string responseString;

            try
            {
                responseString = await _httpClient.GetStringAsync(uri);
            }
            catch (HttpRequestException ex)
            {
                return new ObjectInfo { Error = ex.Message };
            }
                        
            var objectInfo = JsonConvert.DeserializeObject<ObjectInfo>(responseString);
            
            return objectInfo;
        }
        public async Task<IActionResult> GetFile(Attachment file)
        {
            var uri = $"{_remoteServiceBaseUrl}/GetFile";

            var obj = new Dictionary<string, string>
            {
                { "id", file.Id.ToString() },
                { "hash", file.Hash }
            };
           
            var content = JsonConvert.SerializeObject(obj);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
            
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new NotFoundResult();
            
            var fileName = response.Content.Headers.ContentDisposition?.FileName;

            if (string.IsNullOrEmpty(fileName))
                return new NotFoundResult();

            var fresult = new FileStreamResult(await response.Content.ReadAsStreamAsync(), "application/octet-stream")
            {
                FileDownloadName = fileName
            };

            return fresult;
        }
    }
}
