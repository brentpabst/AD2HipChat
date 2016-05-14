using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ad2HipChat.Data
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Principal { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [DefaultValue(false)]
        public bool IsEnabled { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime? SyncedOn { get; set; }
        [DefaultValue(false)]
        public bool IsSynced { get; set; }
        public virtual ICollection<UserHistory> History { get; set; }
    }
}
