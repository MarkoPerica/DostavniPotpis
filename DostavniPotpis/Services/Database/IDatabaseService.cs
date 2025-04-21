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
        Task<int> AddDocument(DocumentModel documentModel);
        Task<List<DocumentModel>> GetDocumentList();
        Task<DocumentModel> GetDocumentById(int id);
        Task<int> GetDocumentByDocument(string document);
        Task<List<DocumentModel>> SearchBuyer(string filterText);
        Task<int> UpdateDocument(DocumentModel documentModel);
        Task<int> DeleteDocument(DocumentModel documentModel);
        Task DeleteDocumentAll();
    }
}
