using AgilityContXam.Helpers;
using AgilityContXam.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgilityContXam.DataStore
{
    public class DsTransacao
    {
        SQLiteAsyncConnection db;

        public DsTransacao(SQLiteAsyncConnection _db)
        {
            db = _db;
        }

        public Task<int> SaveAsync(Transacao entity)
        {
            return db.InsertOrReplaceAsync(entity);
        }

        public Task<int> DeleteAsync(Transacao entity)
        {
            entity.Uer = Settings.Id;
            entity.Der = DateTime.Now;
            return db.InsertOrReplaceAsync(entity);
        }

        public async Task<int> SaveAllAsync(List<Transacao> entity)
        {
            //await db.DeleteAllAsync<Transacao>();
            //return await db.InsertAllAsync(entity);
            foreach (var item in entity)
                await SaveAsync(item);

            return await Task.FromResult(0);
        }

        public Task<List<Transacao>> GetAsync(int page, int pageSize)
        {
            return db.Table<Transacao>()
                .Where(x => x.IdUsuario == Settings.Id && x.Der == null)
                .OrderByDescending(x => x.DataTransacao).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public Task<List<Transacao>> GetAsync(DateTime dataInicio, DateTime dataFim, int page, int pageSize)
        {
            return db.Table<Transacao>()
                .Where(x => x.IdUsuario == Settings.Id && x.Der == null && x.DataTransacao >= dataInicio && x.DataTransacao <= dataFim)
                .OrderByDescending(x => x.DataTransacao).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await db.Table<Transacao>().CountAsync();
        }
    }
}
