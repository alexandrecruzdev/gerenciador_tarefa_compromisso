using Gerenciador.Services;
using Gerenciador.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace Gerenciador
{
    public partial class TarefaPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        public ObservableCollection<Tarefa> TarefasCollection { get; set; }
        public ObservableCollection<Categoria> Categorias { get; set; }
        public int _idPrimeiraCategoria { get; set; }
        public int _idCategoriaByAddPage { get; set; }

      

        public TarefaPage(int idCategoria)
        {
            InitializeComponent();
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gt.db3");
            _databaseService = new DatabaseService(dbPath);
            Categorias = new ObservableCollection<Categoria>();
            TarefasCollection = new ObservableCollection<Tarefa>();
            collectionViewCategorias.ItemsSource = Categorias;
            collectionViewtarefas.ItemsSource = TarefasCollection;
            _idCategoriaByAddPage = idCategoria;
            LoadCategorias();
         
        }

        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var tarefa = checkBox?.BindingContext as Tarefa;
            if (tarefa != null)
            {
                Debug.WriteLine(e.Value);
                await _databaseService.AtualizaStatusTarefa(tarefa, e.Value);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCategorias();
        }

      

        private async Task LoadCategorias()
        {
            try
            {
                var categorias = await _databaseService.GetCategoriasAsync();
                Categorias.Clear();
                foreach (var categoria in categorias)
                {
                    Categorias.Add(categoria);
                }

                if (_idCategoriaByAddPage != 0)
                {
                    await carregaTarefas(_idCategoriaByAddPage);
                }
                else
                {
                    var primeiraCategoria = Categorias.FirstOrDefault();
                    _idPrimeiraCategoria = primeiraCategoria?.Id ?? 0;
                    await carregaTarefas(_idPrimeiraCategoria);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar categorias: {ex.Message}");
            }
        }

        private async Task carregaTarefas(int categoriaId)
        {
            try
            {
                var tarefas = await _databaseService.GetTarefasByCategoriaAsync(categoriaId);
                TarefasCollection.Clear();
                foreach (var tarefa in tarefas.OrderBy(t => t.Data))
                {
                    TarefasCollection.Add(tarefa);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar tarefas: {ex.Message}");
            }
        }

        private async void GotoAddTarefaPage_clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTarefaPage());
        }

        private async void mostrar_tarefas(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Categoria categoria)
            {
                await carregaTarefas(categoria.Id);
            }
        }

        private async void OnEditaTarefa(object sender, TappedEventArgs e)
        {
            var layout = (StackLayout)sender;
            var tarefa = (Tarefa)layout.BindingContext;

            await Navigation.PushAsync(new EditTarefaPage(tarefa));
        }

       
    

private async void delete_tarefa(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Tarefa tarefa)
            {
                await _databaseService.DeleteTarefaAsync(tarefa);
                await carregaTarefas(tarefa.FkCategoriaId);
            }
        }

        private async void voltarPagina_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
    }
}
