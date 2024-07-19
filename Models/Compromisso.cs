using SQLite;
using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Gerenciador
{
    public class Compromisso : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public string Local {  get; set; }
        public int FkCategoriaId { get; set; }







        public event PropertyChangedEventHandler PropertyChanged;

        public Compromisso()
        {
          
        }

        

 

       

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetFormattedHora()
        {
            string hora = DateTime.Today.Add(Hora).ToString("hh:mm tt");
            return $"{hora}";
        }

        public string GetLocalFormatted()
        {
            string local = Local;
            return $"{local}";
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
            DateTime compromissoDateTime = Data.Date.Add(Hora);
            TimeSpan tempoFaltante = compromissoDateTime - DateTime.Now;

            if (tempoFaltante.TotalSeconds >= 0)
            {
                if (tempoFaltante.TotalDays < 1)
                {
                    return $"Faltam {tempoFaltante.Hours} horas e {tempoFaltante.Minutes} minutos para este compromisso";
                }
                else
                {
                    return $"Faltam {tempoFaltante.Days} dias e {tempoFaltante.Hours} horas, {tempoFaltante.Minutes} minutos para este compromisso";
                }
            }
            else
            {
                TimeSpan atraso = DateTime.Now - compromissoDateTime;
                if (atraso.TotalDays < 1)
                {
                    return $"Você está atrasado {atraso.Hours} horas e {atraso.Minutes} minutos para este compromisso";
                }
                else
                {
                    return $"Você está atrasado {atraso.Days} dias e {atraso.Hours} horas, {atraso.Minutes} minutos para este compromisso";
                }
            }
        }


        public string TimeRemaining => GetTimeRemaining();
        public string LocalFormatted => GetLocalFormatted();
        public string GetPrioridadeColor()
        {
          return "#67B6FF"; // Azul para prioridade média
            
        }

        public string PrioridadeColor => GetPrioridadeColor();
    }
}
