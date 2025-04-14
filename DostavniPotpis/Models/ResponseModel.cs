using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Models
{
    public class DocumentResponseModel
    {
        public List<int>? ReceivedDocuments { get; set; }
        public int NumErrors { get; set; }
        public string Message { get; set; } = "";
        public List<PasoeResponse>? PasoeResponses { get; set; }

        public bool HasErrors => NumErrors > 0 || (PasoeResponses != null && PasoeResponses.Any(e => e.IsError));
    }

    public class PasoeResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = "";
        public int NumErrors { get; set; } = 0;
        public string ErrorMessage { get; set; } = "";
        public int ErrorNumber { get; set; } = 0;
        public string ErrorCallStack { get; set; } = "";
        public List<PasoeResponse> PasoeResponses { get; set; } = new List<PasoeResponse>();

        public bool IsError => !string.IsNullOrWhiteSpace(ErrorMessage) || (PasoeResponses != null && PasoeResponses.Count > 0);
    }

    public class ErrorResponseModel
    {
        public string ErrorMessage { get; set; }
        public int NumErrors { get; set; }
    }
}
