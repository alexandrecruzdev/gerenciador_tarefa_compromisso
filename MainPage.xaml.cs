using System.Collections.ObjectModel;
using System.Diagnostics;
using Gerenciador.Models;
using Gerenciador.Services;
namespace Gerenciador
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        public string nome { get; set; }
        public int _idPrimeiraCategoria { get; set; }
        public int _idCategoriaAddPageOrEdit { get; set; }
        public ObservableCollection<Categoria> Categorias { get; set; }

        public MainPage()
        {
            InitializeComponent();

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gt.db3");
            _databaseService = new DatabaseService(dbPath);
            Categorias = new ObservableCollection<Categoria>();
            LoadCategorias();
        }

        private void voltarPagina_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void LoadCategorias()
        {
            try
            {
                var categorias = await _databaseService.GetCategoriasAsync();
                Categorias.Clear();
                foreach (var categoria in categorias)
                {
                    Categorias.Add(categoria);
                }

                var primeiraCategoria = Categorias.FirstOrDefault();
                _idPrimeiraCategoria = primeiraCategoria?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar categorias: {ex.Message}");
            }
        }

        private async void GoTarefaPage(object sender, EventArgs e)
        {
            if (_idPrimeiraCategoria == 0)
            {
                LoadCategorias();
            }

            await Navigation.PushAsync(new TarefaPage(_idPrimeiraCategoria));
        }

        private async void GoCompromissoPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CompromissoPage(_idPrimeiraCategoria));
        }

        private async void GoAddCategoriaPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCategoriaPage());
        }
    }
}
