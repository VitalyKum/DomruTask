using System;
using System.Collections.Generic;

#nullable disable

namespace TaskService.API.Models
{
    public partial class OrderInfo
    {
        public long Id { get; set; }
        public Guid? ProductId { get; set; }
        public string Type { get; set; }
        public decimal? Cost { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Value { get; set; }
        public long? AttachmentId { get; set; }

        public virtual Attachment Attachment { get; set; }
    }
}
