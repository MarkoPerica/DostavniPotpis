using DostavniPotpis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Services.Database
{
    public interface IDatabaseService
    {
        Task<int> AddDokument(DocumentModel documentModel);
    }
}
