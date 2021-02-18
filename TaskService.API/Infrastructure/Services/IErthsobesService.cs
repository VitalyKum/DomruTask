using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskService.API.Models;
using TaskService.API.ViewModel;

namespace TaskService.API.Infrastructure.Services
{
    public interface IErthsobesService
    {
        public Task<ObjectInfo> GetObjectInfo(string dataType);
        public Task<IActionResult> GetFile(Attachment file);                
    }
}
