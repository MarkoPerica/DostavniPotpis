﻿using DostavniPotpis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Services
{
    public interface IApiService
    {
        Task<string> Ping();
        Task<(bool Poslano, string ResponseContent)> Login(string username, string password, string domain = "");
        Task<(bool Poslano, string ResponseContent)> PosaljiDokumentAsync(DocumentModel document, string username, string password, string domain = "");
        Task<(bool Poslano, List<int> uspjesnoPoslani, List<PasoeResponse> neuspjesniDokumenti, string ResponseContent)>
            PosaljiDokumenteAsync(List<DocumentModel> dokumenti, string username, string password, string domain = "");
    }
}
