using DostavniPotpis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Services
{
    public interface IDatabaseService
    {
        Task<int> AddDokument(DocumentModel documentModel);
    }
}
