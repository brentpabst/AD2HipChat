﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Ad2HipChat.Data
{
    public class UserHistory
    {
        [Key]
        public int Id { get; set; }
        public DateTime ChangedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsEnabled { get; set; }
    }
}