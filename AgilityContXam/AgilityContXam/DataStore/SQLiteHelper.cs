using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgilityContXam.Models;
using SQLite;

namespace AgilityContXam.DataStore
{
    public class SQLiteHelper
    {
        SQLiteAsyncConnection db;

        public SQLiteHelper(string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<Usuario>().Wait();
            db.CreateTableAsync<CentroCusto>().Wait();
            db.CreateTableAsync<FormaPagamento>().Wait();
            db.CreateTableAsync<TipoDespesa>().Wait();
            db.CreateTableAsync<TipoReceita>().Wait();
            db.CreateTableAsync<Transacao>().Wait();
            db.CreateTableAsync<ExtratoLancamento>().Wait();
        }

        #region USUARIO

        public Task<int> SaveItemAsync(Usuario entity)
        {
            return db.InsertOrReplaceAsync(entity);
        }

        public Task<int> DeleteItemAsync(Usuario entity)
        {
            return db.DeleteAsync(entity);
        }

        public Task<List<Usuario>> GetItemsAsync()
        {
            return db.Table<Usuario>().ToListAsync();
        }

        public Task<Usuario> GetItemAsync(Guid id)
        {
            return db.Table<Usuario>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        #endregion

        #region CENTRO CUSTO

        public async Task<int> SaveAllCentroCustoAsync(List<CentroCusto> entity)
        {
            await db.DeleteAllAsync<CentroCusto>();
            return await db.InsertAllAsync(entity);
        }
        public Task<List<CentroCusto>> GetCentroCustoAsync()
        {
            return db.Table<CentroCusto>().ToListAsync();
        }

        #endregion

        #region FORMA PAGAMENTO

        public async Task<int> SaveAllFormaPagamentoAsync(List<FormaPagamento> entity)
        {
            await db.DeleteAllAsync<FormaPagamento>();
            return await db.InsertAllAsync(entity);
        }
        public Task<List<FormaPagamento>> GetFormaPagamentoAsync()
        {
            return db.Table<FormaPagamento>().ToListAsync();
        }

        #endregion

        #region TIPO DESPESA

        public async Task<int> SaveAllTipoDespesaAsync(List<TipoDespesa> entity)
        {
            await db.DeleteAllAsync<TipoDespesa>();
            return await db.InsertAllAsync(entity);
        }
        public Task<List<TipoDespesa>> GetTipoDespesaAsync()
        {
            return db.Table<TipoDespesa>().ToListAsync();
        }

        #endregion

        #region TIPO RECEITA

        public async Task<int> SaveAllTipoReceitaAsync(List<TipoReceita> entity)
        {
            await db.DeleteAllAsync<TipoReceita>();
            return await db.InsertAllAsync(entity);
        }
        public Task<List<TipoReceita>> GetTipoReceitaAsync()
        {
            return db.Table<TipoReceita>().ToListAsync();
        }

        #endregion

        private DsTransacao _transacao;
        public DsTransacao Transacao
        {
            get
            {
                return _transacao ?? (_transacao = new DsTransacao(db));
            }
        }

        private DsExtratoLancamento _extratoLancamento;
        public DsExtratoLancamento ExtratoLancamento
        {
            get
            {
                return _extratoLancamento ?? (_extratoLancamento = new DsExtratoLancamento(db));
            }
        }
    }
}
