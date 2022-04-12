using System;
using System.Collections.Generic;
using System.Text;

namespace RemedioApp.Model
{
    public class Leitura
    {
        public int LeituraId { get; set; }
        public int DiaDaSemana { get; set; }
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
