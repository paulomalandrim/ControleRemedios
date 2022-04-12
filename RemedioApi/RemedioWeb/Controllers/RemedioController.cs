using RemedioApi.EF;
using RemedioWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemedioWeb.Controllers
{
    public class RemedioController : Controller
    {
        // GET: Remedio
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EnviarInformacao(int id)
        {
            OperationResult result = new OperationResult();
            result.Message = "Ocorreu um erro";
            
            if (id >= 0 && id < 7)
            {
                try
                {
                    using (RemedioEntities objEF = new RemedioEntities())
                    {
                        var obj = objEF.tb_Leitura.Add(new tb_Leitura
                        {
                            DataInformada = DateTime.Now,
                            DiaDaSemana = id,
                        });

                        objEF.SaveChanges();

                        result.Message = "Salvo com sucesso";

                        Leitura aux = new Leitura
                        {
                            DataInformada = obj.DataInformada,
                            DiaDaSemana = obj.DiaDaSemana,
                            LeituraId = obj.LeituraId,
                        };

                        FirebaseMessageHelper
                            .SendMessage(objEF, 
                                         "Remédio Tomado", 
                                         $"O Remédio de {aux.GetDiaDaSemana} foi tomado com sucesso às {aux.DataInformada}!");
                    }
                }
                catch (Exception)
                {
                    result.Message = "Exception";
                }
            }

            return View(result);
        }

        [HttpPost]
        public ActionResult SalvarToken(string webToken)
        {
            OperationResult result = new OperationResult();
            result.Message = "Ocorreu um erro";

            try
            {
                using (RemedioEntities objEF = new RemedioEntities())
                {
                    objEF.tb_FcmToken.Add(new tb_FcmToken
                    {
                        FcmToken = webToken,
                    });

                    objEF.SaveChanges();

                    result.Message = "Salvo com sucesso";
                }
            }
            catch (Exception)
            {
                result.Message = "Exception";
            }

            return View(result);
        }

        public ActionResult LerInformacao()
        {
            LeituraContainer result = new LeituraContainer();

            try
            {
                using (RemedioEntities objEF = new RemedioEntities())
                {
                    var db = objEF.tb_Leitura;

                    foreach (var item in db)
                    {
                        result.Leituras.Add(new Leitura
                        {
                            DataInformada = item.DataInformada,
                            DiaDaSemana = item.DiaDaSemana,
                            LeituraId = item.LeituraId,
                            Paciente = item.tb_Paciente.PacienteNome
                        });
                    }

                }
            }
            catch (Exception)
            {
            }

            return View(result);
        }

        public ActionResult Melhorias()
        {
            return View();
        }
    }
}