using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class ResetPasswordDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
