using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Gerenciador.Models;
using Gerenciador;
using Gerenciador.Services;
using System.Diagnostics;

namespace Gerenciador
{
    public partial class AddCompromissoPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        public ObservableCollection<Categoria> CategoriasCollection { get; set; }
        public int _idCategoriaSelecionada { get; set; }

        public AddCompromissoPage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gt.db3");
            _databaseService = new DatabaseService(dbPath);
            LoadCategorias();
        }
        private async  void voltarPagina_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CompromissoPage(_idCategoriaSelecionada));
        }
        private async void LoadCategorias()
        {
            try
            {
                var categorias = await _databaseService.GetCategoriasAsync();
                CategoriasCollection = new ObservableCollection<Categoria>(categorias);

                foreach (var categoria in CategoriasCollection)
                {
                    Debug.WriteLine($"Categoria: {categoria.Nome}");
                }

                categoriaPicker.ItemsSource = CategoriasCollection;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar categorias: {ex.Message}");
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

        private async void save_compromisso_Clicked(object sender, EventArgs e)
        {
            string descricao = entryDescricao.Text;
            DateTime data = datePickerData.Date;
            TimeSpan hora = timePickerHora.Time;
            string local = entryLocal.Text;
            int idCategoriaSelecionada = 0;

            // Verificar se a descrição está vazia
            if (string.IsNullOrEmpty(descricao))
            {
                await DisplayAlert("Erro", "Por favor, insira uma descrição.", "OK");
                return;
            }

            // Verificar se uma categoria foi selecionada
            if (categoriaPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Erro", "Por favor, selecione uma categoria. Se não houver categorias disponiveis adicione nas configurações do aplicativo", "OK");
                return;
            }
            else
            {
                Categoria categoriaSelecionada = (Categoria)categoriaPicker.SelectedItem;
                idCategoriaSelecionada = categoriaSelecionada.Id;
                
            }

            // Criar o compromisso
            Compromisso compromisso = new Compromisso
            {
                Descricao = descricao,
                Data = data,
                Hora = hora,
                Local = local,
                FkCategoriaId = idCategoriaSelecionada
            };

            // Adicionar o compromisso ao banco de dados e navegar para a página de compromissos
            _idCategoriaSelecionada = idCategoriaSelecionada;
            await _databaseService.AddCompromissoAsync(compromisso);
            await Navigation.PushAsync(new CompromissoPage(compromisso.FkCategoriaId));
            MessagingCenter.Send(this, "UpdateCompromissoPage", compromisso.FkCategoriaId);
        }

    }
}
