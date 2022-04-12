using RemedioApi.EF;
using RemedioWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RemedioWeb.Controllers
{
    public class RemedioApiController : ApiController
    {
        [HttpGet]
        public IHttpActionResult EnviarInformacao(int pacienteId, int dia)
        {
            JsonReturn ret = new JsonReturn();

            if (dia >= 0 && dia < 7)
            {
                try
                {
                    using (RemedioEntities objEF = new RemedioEntities())
                    {
                        var paciente = objEF.tb_Paciente.FirstOrDefault(x => x.PacienteId == pacienteId);

                        if (paciente == null)
                        {
                            throw new Exception("Paciente não encontrado");
                        }

                        var obj = objEF.tb_Leitura.Add(new tb_Leitura
                        {
                            DataInformada = DateTime.Now,
                            DiaDaSemana = dia,
                            PacienteId = paciente.PacienteId,
                        });

                        objEF.SaveChanges();

                        Leitura aux = new Leitura
                        {
                            DataInformada = obj.DataInformada,
                            DiaDaSemana = obj.DiaDaSemana,
                            LeituraId = obj.LeituraId,
                        };

                        FirebaseMessageHelper
                            .SendMessage(objEF,
                                         "Remédio Tomado",
                                         $"O Remédio do paciente {paciente.PacienteNome} de {aux.GetDiaDaSemana} foi" +
                                         $" tomado com sucesso às {aux.DataInformada}!");

                        ret.Success = true;
                    }
                }
                catch (Exception)
                {
                    ret.Success = false;
                }
            }

            return Json(ret);
        }

        [HttpPost]
        public IHttpActionResult SalvarToken(RequestSendToken param)
        {
            JsonReturn ret = new JsonReturn();

            try
            {
                using (RemedioEntities objEF = new RemedioEntities())
                {
                    objEF.tb_FcmToken.Add(new tb_FcmToken
                    {
                        FcmToken = param.FcmToken,
                    });

                    objEF.SaveChanges();

                    ret.Success = true;
                }
            }
            catch (Exception)
            {
                ret.Success = false;
            }

            return Json(ret);
        }

        [HttpPost]
        public IHttpActionResult LerInformacao()
        {
            JsonReturn ret = new JsonReturn();
            List<Leitura> leituraList = new List<Leitura>();

            try
            {
                using (RemedioEntities objEF = new RemedioEntities())
                {
                    var db = objEF.tb_Leitura;

                    foreach (var item in db)
                    {
                        leituraList.Add(new Leitura
                        {
                            DataInformada = item.DataInformada,
                            DiaDaSemana = item.DiaDaSemana,
                            LeituraId = item.LeituraId,
                            Paciente = item.tb_Paciente.PacienteNome
                        });
                    }
                }

                ret.Tag = leituraList;
                ret.Success = true;
            }
            catch (Exception)
            {
                ret.Success = false;
            }

            return Json(ret);
        }

        [HttpGet]
        public IHttpActionResult EnviarInformacaoCaixa(int pacienteId, int dom, int seg, int ter, int qua, int qui, int sex, int sab)
        {
            JsonReturn ret = new JsonReturn();

            try
            {
                int dia = -1;

                using (RemedioEntities objEF = new RemedioEntities())
                {
                    var paciente = objEF.tb_Paciente.FirstOrDefault(x => x.PacienteId == pacienteId);

                    if (paciente == null)
                    {
                        throw new Exception("Paciente não encontrado");
                    }

                    var caixa = objEF.tb_CaixaRemedio.FirstOrDefault(x => x.PacienteId == pacienteId);

                    if (caixa == null)
                    {
                        caixa = new tb_CaixaRemedio
                        {
                            Domingo = Convert.ToBoolean(dom),
                            Segunda = Convert.ToBoolean(seg),
                            Terca = Convert.ToBoolean(ter),
                            Quarta = Convert.ToBoolean(qua),
                            Quinta = Convert.ToBoolean(qui),
                            Sexta = Convert.ToBoolean(sex),
                            Sabado = Convert.ToBoolean(sab),
                            PacienteId = paciente.PacienteId,
                        };

                        if (dom == 0)
                        {
                            dia = 0;
                        }
                        else if (seg == 0)
                        {
                            dia = 1;
                        }
                        else if (ter == 0)
                        {
                            dia = 2;
                        }
                        else if (qua == 0)
                        {
                            dia = 3;
                        }
                        else if (qui == 0)
                        {
                            dia = 4;
                        }
                        else if (sex == 0)
                        {
                            dia = 5;
                        }
                        else if (sab == 0)
                        {
                            dia = 6;
                        }

                        objEF.tb_CaixaRemedio.Add(caixa);
                        objEF.SaveChanges();
                    }
                    else
                    {
                        if (caixa.Domingo == true && dom == 0)
                        {
                            dia = 0;
                        }
                        else if (caixa.Segunda == true && seg == 0)
                        {
                            dia = 1;
                        }
                        else if (caixa.Terca == true && ter == 0)
                        {
                            dia = 2;
                        }
                        else if (caixa.Quarta == true && qua == 0)
                        {
                            dia = 3;
                        }
                        else if (caixa.Quinta == true && qui == 0)
                        {
                            dia = 4;
                        }
                        else if (caixa.Sexta == true && sex == 0)
                        {
                            dia = 5;
                        }
                        else if (caixa.Sabado == true && sab == 0)
                        {
                            dia = 6;
                        }

                        caixa.Domingo = Convert.ToBoolean(dom);
                        caixa.Segunda = Convert.ToBoolean(seg);
                        caixa.Terca = Convert.ToBoolean(ter);
                        caixa.Quarta = Convert.ToBoolean(qua);
                        caixa.Quinta = Convert.ToBoolean(qui);
                        caixa.Sexta = Convert.ToBoolean(sex);
                        caixa.Sabado = Convert.ToBoolean(sab);

                        objEF.Entry(caixa).State = System.Data.Entity.EntityState.Modified;
                        objEF.SaveChanges();
                    }

                    if (dia != -1)
                    {
                        var obj = objEF.tb_Leitura.Add(new tb_Leitura
                        {
                            DataInformada = DateTime.Now,
                            DiaDaSemana = dia,
                            PacienteId = paciente.PacienteId,
                        });

                        objEF.SaveChanges();

                        Leitura aux = new Leitura
                        {
                            DataInformada = obj.DataInformada,
                            DiaDaSemana = obj.DiaDaSemana,
                            LeituraId = obj.LeituraId,
                        };

                        FirebaseMessageHelper
                            .SendMessage(objEF,
                                         "Remédio Tomado",
                                         $"O Remédio do paciente {paciente.PacienteNome} de {aux.GetDiaDaSemana} foi tomado" +
                                         $" com sucesso às {aux.DataInformada}!");
                    }

                    ret.Success = true;
                }
            }
            catch (Exception)
            {
                ret.Success = false;
            }

            return Json(ret);
        }
    }
}
