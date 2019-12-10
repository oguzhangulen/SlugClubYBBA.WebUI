using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SlubClubYBBA.WebUI.Models
{
    public class MusteriModel
    {
        [RegularExpression(@"^[1-9]\d+$", ErrorMessage = "invalid character")]
        public string TCKN { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Sifre { get; set; }
        public string Adres { get; set; }
        [RegularExpression(@"^[1-9]\d+$", ErrorMessage = "invalid character")]
        public string TelNo { get; set; }
        public string EPosta { get; set; }
    }
}