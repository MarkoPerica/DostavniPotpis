using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Models
{
    public class LoginModel
    {
        public string ErrorMessage { get; set; } = "";
        public int NumErrors { get; set; }
        public bool HasErrors => NumErrors > 0;
    }
}
