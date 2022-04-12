using Newtonsoft.Json;
using RemedioApp.Interfaces;
using RemedioApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace RemedioApp.ViewModel
{
    public class MainPageViewModel
    {
        public MainPageViewModel()
        {
            this.Leituras = new ObservableCollection<Leitura>();
        }

        public ObservableCollection<Leitura> Leituras { get; set; }


        public async void GetLeitura()
        {
            try
            {
                this.Leituras.Clear();
                string url = "https://remediounisal.wsilveirait.com.br/api/remedioapi/lerinformacao/";
                ApiReturn apiReturn = await DependencyService.Get<ICommunication>()
                        .PostMethod(url, "", null);

                if (apiReturn != null && apiReturn.Success == true)
                {
                    WebReturn webRet = JsonConvert.DeserializeObject<WebReturn>(apiReturn.JsonObject);
                    string jsonTag = JsonConvert.SerializeObject(webRet.Tag);

                    List<Leitura> list = JsonConvert.DeserializeObject<List<Leitura>>(jsonTag);

                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            this.Leituras.Add(new Leitura
                            {
                                DataInformada = item.DataInformada,
                                DiaDaSemana = item.DiaDaSemana,
                                LeituraId = item.LeituraId,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
