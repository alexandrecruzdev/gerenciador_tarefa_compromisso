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
    public partial class EditCompromissoPage : ContentPage
    {
        private Compromisso _compromisso;
        private readonly DatabaseService _databaseService;
        public ObservableCollection<Categoria> CategoriasCollection { get; set; }
        public int _idCategoriaSelecionada { get; set; }

        public EditCompromissoPage(Compromisso compromisso)
        {
            InitializeComponent();
            _compromisso = compromisso;

            // Inicialização do serviço de banco de dados com o caminho correto
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gt.db3");
            _databaseService = new DatabaseService(dbPath);

            LoadCategorias();
            loadData();
        }
        private void voltarPagina_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CompromissoPage(_idCategoriaSelecionada));
        }
        private async void loadData()
        {
            entryDescricao.Text = _compromisso.Descricao;
            datePickerData.Date = _compromisso.Data;
            timePickerHora.Time = _compromisso.Hora;
            entryLocal.Text = _compromisso.Local;
            

            // Carrega a categoria associada à tarefa
            Categoria categoria = await _databaseService.GetCategoriaByIdAsync(_compromisso.FkCategoriaId);

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

        private async void save_compromisso_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Obtém os dados atualizados da interface
                string descricao = entryDescricao.Text;
                DateTime data = datePickerData.Date;
                TimeSpan hora = timePickerHora.Time;
                string local = entryLocal.Text;
                int idCategoriaSelecionada = 0;

            
                // Obtém a categoria selecionada
                if (categoriaPicker.SelectedItem != null)
                {
                    Categoria categoriaSelecionada = (Categoria)categoriaPicker.SelectedItem;
                    idCategoriaSelecionada = categoriaSelecionada.Id;
                }

                // Cria uma nova instância de Tarefa com os dados atualizados
                Compromisso compromissoAtualizado = new Compromisso
                {
                    Id = _compromisso.Id,
                    Descricao = descricao,
                    Data = data,
                    Hora = hora,
                    Local = local,
                    FkCategoriaId = idCategoriaSelecionada
                };

                _idCategoriaSelecionada = idCategoriaSelecionada;

        // Atualiza a tarefa no banco de dados
        int rowsUpdated = await _databaseService.UpdateCompromissoAsync(compromissoAtualizado);

                if (rowsUpdated > 0)
                {
                    await DisplayAlert("Sucesso", "Compromisso atualizada com sucesso.", "OK");
                    await Navigation.PushAsync(new CompromissoPage(compromissoAtualizado.FkCategoriaId));
                    MessagingCenter.Send(this, "UpdateCompromissoPage", compromissoAtualizado.FkCategoriaId);
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
                Debug.WriteLine($"ID da Categoria selecionada: {idCategoriaSelecionada}");
            }
        }
    }
}

