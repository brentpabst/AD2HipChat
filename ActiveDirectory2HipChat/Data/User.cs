using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ActiveDirectory2HipChat.Data
{
    public class User
    {
        [Required, Key]
        public int Id { get; set; }
        [Required]
        public string Principal { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required, DefaultValue(false)]
        public bool IsEnabled { get; set; }
        [Required]
        public DateTime AddedOn { get; set; }
        [Required]
        public DateTime UpdatedOn { get; set; }
        public DateTime? SyncedOn { get; set; }
        [Required, DefaultValue(false)]
        public bool IsSynced { get; set; }
    }
}
