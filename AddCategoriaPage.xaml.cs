using Gerenciador.Models;
using Gerenciador.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Reflection.Metadata;

namespace Gerenciador
{
    public partial class AddCategoriaPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        public ObservableCollection<Categoria> Categorias { get; set; }

        public AddCategoriaPage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gt.db3");
            _databaseService = new DatabaseService(dbPath);
            Categorias = new ObservableCollection<Categoria>();
            collectionViewCategorias.ItemsSource = Categorias;
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

        private async void add_categoria_clicked(object sender, EventArgs e)
        {
            string nome_categoria = entry_categoria.Text;

            if (!string.IsNullOrWhiteSpace(nome_categoria))
            {
                var categoria = new Categoria
                {
                    Nome = nome_categoria
                };
                await _databaseService.AddCategoriaAsync(categoria);
                entry_categoria.Text = "";
                await LoadCategorias();
            }
        }

        private async Task LoadCategorias()
        {
            Debug.WriteLine("Carregando categorias...");
            var categorias = await _databaseService.GetCategoriasAsync();
            Debug.WriteLine($"Número de categorias carregadas: {categorias.Count}");
            Categorias.Clear();
            foreach (var categoria in categorias)
            {
                Categorias.Add(categoria);
                Debug.WriteLine($"Categoria adicionada: {categoria.Nome}");
            }
            collectionViewCategorias.ItemsSource = null;
            collectionViewCategorias.ItemsSource = Categorias;
        }

        private async void delete_categoria_clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Categoria categoria)
            {
                await _databaseService.DeleteCategoriaAsync(categoria);
                await LoadCategorias();

            }
        }
    }
}
