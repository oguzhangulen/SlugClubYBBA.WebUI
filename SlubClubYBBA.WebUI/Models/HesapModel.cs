using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SlubClubYBBA.WebUI.Models
{
    public class HesapModel
    {
        public string HesapNo { get; set; }
        public int EkNo { get; set; }
        [RegularExpression(@"^[1-9]\d+$", ErrorMessage = "invalid character")]
        public decimal BakiyeBilgi { get; set; }
        public bool AktiflikDurumu { get; set; }
        public string TCKN { get; set; }
        public MusteriModel Musteri { get; set; }
    }
}