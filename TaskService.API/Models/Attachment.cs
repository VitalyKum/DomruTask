using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace TaskService.API.Models
{
    public partial class Attachment
    {
        public Attachment()
        {
            OrderInfos = new HashSet<OrderInfo>();
        }

        public long Id { get; set; }
        public string Hash { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderInfo> OrderInfos { get; set; }
    }
}
