using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TaskService.API.Infrastructure;
using TaskService.API.Infrastructure.Services;
using TaskService.API.Models;
using TaskService.API.ViewModel;

namespace TaskService.API.Controllers
{
    [ApiController]
    public class TaskServicesController : ControllerBase
    {
        private readonly IErthsobesService _erthsobesSvc;
        private readonly TaskServiceContext _context;

        public TaskServicesController(IErthsobesService erthsobesSvc, TaskServiceContext context)
        {
            _erthsobesSvc = erthsobesSvc;
            _context = context;
        }

        [HttpGet("api/Orders")]
        public async Task<IActionResult> Get()
        {
            var result = await _context.OrderInfos.Include(o => o.Attachment).ToListAsync();
            return Ok(result);
        }

        [HttpGet("api/CreateObject")]
        public async Task<IActionResult> CreateObject(string name, string dataType)
        {
            return await GetObjectAsync(dataType);
        }

        [HttpPost("api/CreateObject")]
        public async Task<IActionResult> CreateObject(ObjectParam param)
        {
            return await GetObjectAsync(param.DataType);
        }

        [HttpGet("api/GetStat")]
        public IActionResult GetStat()
        {
            var dic = new Dictionary<string, string>();

            var query = _context.OrderInfos
                .AsNoTracking()
                .Where(x => x.Type == "phone");

            int phoneCount = query.Count();

            dic.Add("PhoneCount", $"{phoneCount}");
            dic.Add("PhoneCountWithFile", query.Count(x => x.AttachmentId != null).ToString());
            dic.Add("Top10Phones", string.Join(";", query
                .Select(x => x.PhoneNumber)
                .Skip(phoneCount < 10 ? 0 : phoneCount - 10)
                .Take(10)));

            query = _context.OrderInfos
                .AsNoTracking()
                .Where(x => x.Type == "email");
            
            int emailCount = query.Count();

            dic.Add("EmailCount", $"{emailCount}");
            dic.Add("EmailCountWithFile", query.Count(x => x.AttachmentId != null).ToString());
            dic.Add("Top10Emails", string.Join(";", query
                .Select(x => x.Email)
                .Skip(emailCount < 10 ? 0 : emailCount - 10)
                .Take(10)));

            query = _context.OrderInfos
                .AsNoTracking()
                .Where(x => x.Type == "other");

            dic.Add("OtherCount", query.Count().ToString());
            dic.Add("OtherCountWithFile", query.Count(x => x.AttachmentId != null).ToString());

            var stat = JsonConvert.SerializeObject(dic);

            return Ok(stat);
        }

        [HttpGet("api/GetFileById")]
        public async Task<IActionResult> GetFileById(Guid id)
        {
            var order = await _context.OrderInfos
                .AsNoTracking()
                .Include(attachment => attachment.Attachment)
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (order == null)
                return StatusCode(404, "Requested object not found");

            if (order.Attachment == null)
                return StatusCode(404, "Requested object have not attachment");

            return await _erthsobesSvc.GetFile(order.Attachment);
        }

        private async Task<IActionResult> GetObjectAsync(string dataType)
        {
            var objectInfo = await _erthsobesSvc.GetObjectInfo(dataType);

            if (objectInfo.Error == null)
            {
                try
                {
                    dynamic data = JsonConvert.DeserializeObject(objectInfo.Data);

                    if (data == null)
                        throw new InvalidCastException("Invalid cast data from responce to dynamic type");

                    var order = new OrderInfo
                    {
                        Type = objectInfo.DataType,
                        ProductId = data.Id,
                        Cost = data.Cost,
                        PhoneNumber = data.PhoneNumber,
                        Email = data.Email,
                        Value = data.Value
                    };

                    dynamic file = data.File;
                    if (file != null)
                        order.Attachment = new Attachment { Hash = file.Hash };

                    _context.OrderInfos.Add(order);
                    await _context.SaveChangesAsync();

                    return Ok(order.ProductId);
                }
                catch (InvalidCastException ex)
                {
                    objectInfo.Error = ex.Message;
                }
                catch (DbUpdateException ex)
                {
                    objectInfo.Error = $"Database error: {ex.Message}{Environment.NewLine}" +
                        $"{ex?.InnerException.Message}";
                }
                catch (Exception ex)
                {
                    objectInfo.Error = $"Unknown error:{ex.Message}{Environment.NewLine}" +
                        $"{ex?.InnerException.Message}";
                }
            }

            return StatusCode(500, objectInfo);
        }

    }
}