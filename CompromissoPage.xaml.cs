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
    public partial class CompromissoPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        public ObservableCollection<Compromisso> CompromissosCollection { get; set; }
        public ObservableCollection<Categoria> Categorias { get; set; }
        public int _idPrimeiraCategoria { get; set; }
        public int _idCategoriaByAddPage { get; set; }



        public CompromissoPage(int idCategoria)
        {
            InitializeComponent();
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gt.db3");
            _databaseService = new DatabaseService(dbPath);
            Categorias = new ObservableCollection<Categoria>();
            CompromissosCollection = new ObservableCollection<Compromisso>();
            collectionViewCategorias.ItemsSource = Categorias;
            collectionViewtarefas.ItemsSource = CompromissosCollection;
            _idCategoriaByAddPage = idCategoria;
            LoadCategorias();

        }


        private async void voltarPagina_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
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
                    await carregaCompromisso(_idCategoriaByAddPage);
                }
                else
                {
                    var primeiraCategoria = Categorias.FirstOrDefault();
                    _idPrimeiraCategoria = primeiraCategoria?.Id ?? 0;
                    await carregaCompromisso(_idPrimeiraCategoria);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar categorias: {ex.Message}");
            }
        }

        private async Task carregaCompromisso(int categoriaId)
        {
            try
            {
                var compromissos = await _databaseService.GetCompromissosByCategoriaAsync(categoriaId);
                CompromissosCollection.Clear();
                foreach (var compromisso in compromissos.OrderBy(t => t.Data))
                {
                    CompromissosCollection.Add(compromisso);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar compromissos: {ex.Message}");
            }
        }

        private async void GotoAddCompromissoPage_clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCompromissoPage());
        }

        private async void mostrar_compromissos(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Categoria categoria)
            {
                await carregaCompromisso(categoria.Id);
            }
        }

        private async void OnEditaCompromisso(object sender, TappedEventArgs e)
        {
            var layout = (StackLayout)sender;
            var compromisso = (Compromisso)layout.BindingContext;

            await Navigation.PushAsync(new EditCompromissoPage(compromisso));
        }




        private async void delete_compromisso(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Compromisso compromisso)
            {
                await _databaseService.DeleteCompromissoAsync(compromisso);
                await carregaCompromisso(compromisso.FkCategoriaId);
            }
        }





    }
}
