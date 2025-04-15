using DostavniPotpis.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Services
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection _dbConnection;

        private async Task SetUpDb()
        {
            if (_dbConnection == null)
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DocumentList.db3");
                _dbConnection = new SQLiteAsyncConnection(dbPath);
                await _dbConnection.CreateTableAsync<DocumentModel>();
            }
        }

        public async Task<int> AddDokument(DocumentModel documentModel)
        {
            await SetUpDb();

            await _dbConnection.InsertAsync(documentModel);
            return documentModel.Id;
        }
    }
}
