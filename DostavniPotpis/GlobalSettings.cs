using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis
{
    public class GlobalSettings
    {
        public const string AdminUser = "admin";
        public const string AdminPassword = "bilbo";

        public const string PingUri = "/DocumentSignature/web/Ping";
        public const string LoginUri = "/DocumentSignature/web/Login";
        public const string DocumentSendUri = "/DocumentSignature/web/Send";

        public const int StatusUTijeku = 1; // "1 Isporuka u tijeku"
        public const int StatusIsporuceno = 2; // "2 Isporučeno"
        public const int StatusVraceno = 3; // "3 Vraćeno"
        public const int StatusOdbijeno = 4; // "4 Odbijeno"

        public const string StatusUTijekuColor = "#e0f72f";
        public const string StatusIsporucenoColor = "#008000";
        public const string StatusVracenoColor = "#080df7";
        public const string StatusOdbijenoColor = "#F25922";

        public const string BarcodePotpisCheckTag = "<!EP!>";
    }
}
