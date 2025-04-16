﻿using DostavniPotpis.Models;
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

        public async Task<int> AddDocument(DocumentModel documentModel)
        {
            await SetUpDb();

            await _dbConnection.InsertAsync(documentModel);
            return documentModel.Id;
        }

        public async Task<List<DocumentModel>> GetDocumentList()
        {
            await SetUpDb();
            var documentList = await _dbConnection.Table<DocumentModel>().ToListAsync();
            return documentList;
        }

        public async Task<DocumentModel> GetDocumentById(int id)
        {
            await SetUpDb();
            return await _dbConnection.FindAsync<DocumentModel>(id);
        }

        public async Task<int> UpdateDocument(DocumentModel documentModel)
        {
            await SetUpDb();

            await _dbConnection.UpdateAsync(documentModel);
            return documentModel.Id;
        }

        public async Task<int> DeleteDocument(DocumentModel documentModel)
        {
            await SetUpDb();
            return await _dbConnection.DeleteAsync(documentModel);
        }

        public async Task DeleteDocumentAll()
        {
            await _dbConnection.DeleteAllAsync<DocumentModel>();
        }
    }
}
