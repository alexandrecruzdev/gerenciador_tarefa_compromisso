using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Gerenciador.Models;
using Gerenciador.Services;
using System.Diagnostics;

namespace Gerenciador
{
    public partial class EditTarefaPage : ContentPage
    {
        private Tarefa _tarefa;
        private readonly DatabaseService _databaseService;
        public int _idCategoriaSelecionada { get; set; }
        public ObservableCollection<Categoria> CategoriasCollection { get; set; }

        public EditTarefaPage(Tarefa tarefa)
        {
            InitializeComponent();
            _tarefa = tarefa;

            // Inicialização do serviço de banco de dados com o caminho correto
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gt.db3");
            _databaseService = new DatabaseService(dbPath);

            LoadCategorias();
            loadData();
        }
        private void voltarPagina_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TarefaPage(_idCategoriaSelecionada));
        }
        private async void loadData()
        {
            entryDescricao.Text = _tarefa.Descricao;
            datePickerData.Date = _tarefa.Data;
            timePickerHora.Time = _tarefa.Hora;
            pickerPrioridade.SelectedItem = _tarefa.Prioridade.ToString(); // Ajustar para selecionar por valor

            // Carrega a categoria associada à tarefa
            Categoria categoria = await _databaseService.GetCategoriaByIdAsync(_tarefa.FkCategoriaId);

            // Verifica se a categoria foi encontrada na coleção
            if (categoria != null && CategoriasCollection != null)
            {
                // Procura pelo objeto Categoria na coleção com o mesmo Id
                Categoria categoriaSelecionada = CategoriasCollection.FirstOrDefault(c => c.Id == categoria.Id);

                // Verifica se a categoria foi encontrada na coleção
                if (categoriaSelecionada != null)
                {
                    // Define o objeto Categoria como SelectedItem no Picker
                    categoriaPicker.SelectedItem = categoriaSelecionada;
                }
            }

        }

        private async void LoadCategorias()
        {
            try
            {
                // Obtém todas as categorias do banco de dados
                var categorias = await _databaseService.GetCategoriasAsync();

                // Cria uma ObservableCollection para as categorias
                CategoriasCollection = new ObservableCollection<Categoria>(categorias);

                // Define o ItemsSource do Picker de categorias
                categoriaPicker.ItemsSource = CategoriasCollection;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar categorias: {ex.Message}");
            }
        }

        private async void save_tarefa_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Obtém os dados atualizados da interface
                string descricao = entryDescricao.Text;
                DateTime data = datePickerData.Date;
                TimeSpan hora = timePickerHora.Time;
                int prioridadeSelecionada = 0;
                int idCategoriaSelecionada = 0;

                // Obtém a prioridade selecionada
                if (pickerPrioridade.SelectedItem != null)
                {
                    prioridadeSelecionada = Convert.ToInt32(pickerPrioridade.SelectedItem);
                }

                // Obtém a categoria selecionada
                if (categoriaPicker.SelectedItem != null)
                {
                    Categoria categoriaSelecionada = (Categoria)categoriaPicker.SelectedItem;
                    idCategoriaSelecionada = categoriaSelecionada.Id;
                }

                // Cria uma nova instância de Tarefa com os dados atualizados
                Tarefa tarefaAtualizada = new Tarefa
                {
                    Id = _tarefa.Id,
                    Descricao = descricao,
                    Data = data,
                    Hora = hora,
                    Prioridade = prioridadeSelecionada,
                    FkCategoriaId = idCategoriaSelecionada
                };

                // Atualiza a tarefa no banco de dados
                int rowsUpdated = await _databaseService.UpdateTarefaAsync(tarefaAtualizada);

                if (rowsUpdated > 0)
                {
                    await DisplayAlert("Sucesso", "Tarefa atualizada com sucesso.", "OK");
                    await Navigation.PushAsync(new TarefaPage(tarefaAtualizada.FkCategoriaId));
                    MessagingCenter.Send(this, "UpdateTarefaPage", tarefaAtualizada.FkCategoriaId);
                }
                else
                {
                    await DisplayAlert("Erro", "Falha ao atualizar a tarefa.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar tarefa: {ex.Message}");
                await DisplayAlert("Erro", "Erro ao salvar tarefa.", "OK");
            }
        }

        private void CategoriaPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (categoriaPicker.SelectedIndex != -1)
            {
                Categoria categoriaSelecionada = (Categoria)categoriaPicker.SelectedItem;
                int idCategoriaSelecionada = categoriaSelecionada.Id;
                _idCategoriaSelecionada = categoriaSelecionada.Id;
                Debug.WriteLine($"ID da Categoria selecionada: {idCategoriaSelecionada}");
            }
        }
    }
}
