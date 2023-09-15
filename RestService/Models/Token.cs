using System;

namespace RestService.Models
{
    public class Token
    {
        public Guid UserId { get; set; }
        public DateTime Expires { get; set; }
    }
}

