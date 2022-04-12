using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemedioWeb.Models
{
    public class LeituraContainer
    {
        public LeituraContainer()
        {
            this.Leituras = new List<Leitura>();
        }

        public List<Leitura> Leituras { get; set; }
    }
}