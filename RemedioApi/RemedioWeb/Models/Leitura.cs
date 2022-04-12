using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemedioWeb.Models
{
    public class Leitura
    {
        public int LeituraId { get; set; }
        public int DiaDaSemana { get; set; }
        public string Paciente { get; set; }
        public DateTime DataInformada { get; set; }

        public string GetDiaDaSemana
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("pt-BR");
                DayOfWeek day = (DayOfWeek)DiaDaSemana;
                return culture.DateTimeFormat.GetDayName(day);
            }
        }
    }
}