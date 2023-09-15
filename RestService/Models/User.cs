﻿using System;

namespace RestService.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}