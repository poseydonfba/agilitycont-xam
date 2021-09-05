using AgilityContXam.Helpers;
using AgilityContXam.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgilityContXam.DataStore
{
    public class DsExtratoLancamento
    {
        SQLiteAsyncConnection db;

        public DsExtratoLancamento(SQLiteAsyncConnection _db)
        {
            db = _db;
        }

        public Task<int> SaveAsync(ExtratoLancamento entity)
        {
            //var inserted = await db.Table<ExtratoLancamento>()
            //    .FirstOrDefaultAsync(x => x.IdUsuario == Settings.Id && x.Ano == entity.Ano && x.Mes == entity.Mes);

            //if (inserted == null)
            //    return await db.InsertAsync(entity);
            //else
            //    return await db.UpdateAsync(entity);

            return db.InsertOrReplaceAsync(entity);
        }

        public async Task<int> SaveAllAsync(List<ExtratoLancamento> entity)
        {
            foreach (var item in entity)
                await SaveAsync(item);

            return await Task.FromResult(0);
        }

        public Task<List<ExtratoLancamento>> GetAsync(int ano)
        {
            return db.Table<ExtratoLancamento>()
                .Where(x => x.IdUsuario == Settings.Id && x.Ano == ano)
                .OrderBy(x => x.Mes).ToListAsync();
        }
    }
}
