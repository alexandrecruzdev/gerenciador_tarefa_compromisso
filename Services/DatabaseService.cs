using Gerenciador.Models;
using Gerenciador.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerenciador.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            _database.CreateTableAsync<Tarefa>().Wait();
            _database.CreateTableAsync<Compromisso>().Wait();
            _database.CreateTableAsync<Categoria>().Wait();
            //_database.CreateTableAsync<Lembrete>().Wait();
        }

        // Crud Tarefa
        public async Task<int> AddTarefaAsync(Tarefa tarefa)
        {
            return await _database.InsertAsync(tarefa);
        }

        public async Task<int> UpdateTarefaAsync(Tarefa tarefa)
        {
            return await _database.UpdateAsync(tarefa);
        }

        public async Task<int> DeleteTarefaAsync(Tarefa tarefa)
        {
            return await _database.DeleteAsync(tarefa);
        }

        public async Task<List<Tarefa>> GetTarefasAsync()
        {
            return await _database.Table<Tarefa>().OrderByDescending(t => t.Data).ToListAsync();
        }

        public async Task<List<Tarefa>> GetTarefasByCategoriaAsync(int fkCategoriaId)
        {
            return await _database.Table<Tarefa>()
                                 .Where(t => t.FkCategoriaId == fkCategoriaId)
                                 .ToListAsync();
        }

        public async Task AtualizaStatusTarefa(Tarefa tarefa, bool concluido)
        {
            tarefa.Concluido = concluido;
            tarefa.Status = concluido ? "Concluída" : "Pendente";
            await _database.UpdateAsync(tarefa);
        }


        // Crud Categoria
        public async Task<int> AddCategoriaAsync(Categoria categoria)
        {
            return await _database.InsertAsync(categoria);
        }

        public async Task<int> UpdateCategoriaAsync(Categoria categoria)
        {
            return await _database.UpdateAsync(categoria);
        }

        public async Task<int> DeleteCategoriaAsync(Categoria categoria)
        {
            return await _database.DeleteAsync(categoria);
        }

        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            return await _database.Table<Categoria>().ToListAsync();
        }

        public async Task<Categoria> GetCategoriaByIdAsync(int categoriaId)
        {
            return await _database.Table<Categoria>()
                                 .Where(c => c.Id == categoriaId)
                                 .FirstOrDefaultAsync();
        }


        // Crud Compromisso
        public async Task<int> AddCompromissoAsync(Compromisso compromisso)
        {
            return await _database.InsertAsync(compromisso);
        }

        public async Task<int> UpdateCompromissoAsync(Compromisso compromisso)
        {
            return await _database.UpdateAsync(compromisso);
        }

        public async Task<int> DeleteCompromissoAsync(Compromisso compromisso)
        {
            return await _database.DeleteAsync(compromisso);
        }

        public async Task<List<Compromisso>> GetCompromissosAsync()
        {
            return await _database.Table<Compromisso>().OrderByDescending(t => t.Data).ToListAsync();
        }

        public async Task<List<Compromisso>> GetCompromissosByCategoriaAsync(int fkCategoriaId)
        {
            return await _database.Table<Compromisso>()
                                 .Where(t => t.FkCategoriaId == fkCategoriaId)
                                 .ToListAsync();
        }

        
    }
}
