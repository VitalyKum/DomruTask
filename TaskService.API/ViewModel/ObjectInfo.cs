using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskService.API.ViewModel
{
    [NotMapped]
    public class ObjectInfo
    {
        public string DataType { get; set; }
        public string Data { get; set; }
        public string Error { get; set; }
    }
}
