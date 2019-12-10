using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SlubClubYBBA.WebUI.Models
{
    public class LoginModel
    {
        [RegularExpression(@"^[1-9]\d+$", ErrorMessage = "invalid character")]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}