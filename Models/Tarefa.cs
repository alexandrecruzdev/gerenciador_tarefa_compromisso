using SQLite;
using System;
using System.ComponentModel;

namespace Gerenciador
{
    public class Tarefa : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public int FkCategoriaId { get; set; }
        public int Prioridade { get; set; }

        private bool _concluido;
        private string _status;

        private string _statusOriginal;
        private string _prioridadeColorOriginal;

        public event PropertyChangedEventHandler PropertyChanged;

        public Tarefa()
        {
            _status = GetInitialStatus(); // Define o status inicial ao criar a tarefa
        }

        private string GetInitialStatus()
        {
            if (_concluido)
            {
                return "Concluída";
            }
            else
            {
                return "Pendente";
            }
        }

        public bool Concluido
        {
            get { return _concluido; }
            set
            {
                if (_concluido != value)
                {
                    _concluido = value;

                    if (_concluido)
                    {
                        // Armazena o estado atual antes de marcar como concluída
                        _statusOriginal = _status;
                        _prioridadeColorOriginal = PrioridadeColor;

                        _status = "Concluída";
                    }
                    else
                    {
                        // Retorna ao estado original ao desmarcar a conclusão
                        _status = _statusOriginal ?? GetInitialStatus();
                    }

                    OnPropertyChanged(nameof(Concluido));
                    OnPropertyChanged(nameof(Status));
                    OnPropertyChanged(nameof(PrioridadeColor));
                    OnPropertyChanged(nameof(TimeRemaining));
                }
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetFormattedHora()
        {
            return DateTime.Today.Add(Hora).ToString("hh:mm tt");
        }

        public string FormattedHora => GetFormattedHora();

        public string FormattedData => GetFormattedData();

        public string GetFormattedData()
        {
            DateTime hoje = DateTime.Today;
            DateTime amanha = hoje.AddDays(1);
            DateTime ontem = hoje.AddDays(-1);

            if (Data.Date == hoje)
            {
                return "Hoje";
            }
            else if (Data.Date == amanha)
            {
                return "Amanhã";
            }
            else if (Data.Date == ontem)
            {
                return "Ontem";
            }
            else
            {
                return Data.ToString("dd/MM/yyyy");
            }
        }

        public string GetTimeRemaining()
        {
            if (_concluido)
            {
                return "Esta tarefa foi concluída";
            }

            DateTime tarefaDateTime = Data.Date.Add(Hora);
            TimeSpan tempoFaltante = tarefaDateTime - DateTime.Now;

            if (tempoFaltante.TotalSeconds <= 0)
            {
                return "Esta tarefa está atrasada";
            }
            else if (tempoFaltante.TotalDays < 1)
            {
                return $"Ainda restam {tempoFaltante.Hours} horas e {tempoFaltante.Minutes} minutos pra você concluir esta tarefa.";
            }
            else
            {
                return $"{tempoFaltante.Days} dias, {tempoFaltante.Hours} horas, {tempoFaltante.Minutes} minutos restantes";
            }
        }

        public string TimeRemaining => GetTimeRemaining();

        public string GetPrioridadeColor()
        {
            if (_concluido)
            {
                return "green";
            }

            switch (Prioridade)
            {
                case 1:
                    return "#E08484"; // Vermelho para alta prioridade
                case 2:
                    return "#67B6FF"; // Azul para prioridade média
                case 3:
                    return "#525BF8"; // Azul escuro para baixa prioridade
                default:
                    return "Black"; // Preto como padrão
            }
        }

        public string PrioridadeColor => GetPrioridadeColor();
    }
}
