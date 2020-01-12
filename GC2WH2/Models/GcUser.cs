using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GC2WH2.Models
{
    public class GcUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public GcRole UserRole { get; set; }
        public GcAuthType AuthType { get; set; }
        public long TelegramUserId { get; set; }
        public List<long> TelegramChats { get; set; }

        public enum GcRole
        {
            Superadmin,
            Admin,
            Authenticated,
            Anonymous
        }

        public enum GcAuthType
        {
            None,
            Google
        }
    }
}
