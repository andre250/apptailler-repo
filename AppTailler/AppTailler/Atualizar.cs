using System;
using AppTailler.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using Xamarin.Forms;
using Plugin.Connectivity;
using AppTailler.Menus;
using System.Linq;
using Android.OS;
using Newtonsoft.Json.Linq;

namespace AppTailler
{
    public class Atualizar
    {
        private static Page page;
        #region AtualizarTudo

        public async Task<string> AtualizarTudo(int IdUnidade, int idPessoa, Local local_pagina)
        {
            var versao = DependencyService.Get<IInfoService>().AppVersionName;
            var deviceid = Build.Serial;
            Local l = local_pagina;
            using (var dados = new DataAccess())
            {
                string url_locais = "http://tailler.ddns.net/ws/LocalVistoria/ObterLocalAuditoria?idUnidade=" +
                                     IdUnidade +
                                     "&IdPessoa=" +
                                     idPessoa +
                                     "&Versao=" +
                                     versao +
                                    "&DeviceID=" +
                                     deviceid;

                var json_locais = await GetAPI(url_locais);
                if (json_locais == "<!DOCTYPE " || String.IsNullOrEmpty(json_locais))
                { return "HTTP error"; }
                else
                {
                    var jsonSerializerSettings = new JsonSerializerSettings();
                    jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                    List<locais> locaisList = JsonConvert.DeserializeObject<List<locais>>(json_locais, jsonSerializerSettings);
                    dados.InserirAtualizarAllLocais(locaisList);
                    var cliente = dados.GetClientes(IdUnidade);
                    foreach (locais local in locaisList)
                    {
                        int LocalStatus = 0;
                        var local_base = dados.GetLocais(local.IdLocal);
                        if (local_base != null)
                        {
                            local.Assinatura = local_base.Assinatura;
                            local.AssinaturaArray = local_base.AssinaturaArray;
                            local.StatusAuditoria = local_base.StatusAuditoria;
                        }
                        #region New
                        if (local_base.IdEvento == local.IdEvento)
                        {
                            if (local.StatusAuditoria == 2 || local.StatusAuditoria == 0)
                            {
                                AuditoriaSync(local, cliente);
                            }
                            else if(local.StatusAuditoria == 1)
                            {
                                if (local_base.IdAuditoriaBase != local.IdAuditoriaBase)
                                {
                                    dados.DeleteAllAuditorias(local.IdAuditoria);
                                    AuditoriaSync(local, cliente);
                                }
                                else
                                {
                                    AuditoriaSync(local, cliente);
                                }
                            }                            
                        }
                        else if(local_base.IdEvento != local.IdEvento)
                        {
                            if (local.StatusAuditoria == 2 || local.StatusAuditoria == 0)
                            {
                                dados.DeleteAllAuditorias(local.IdAuditoria);
                                AuditoriaSync(local, cliente);
                            }
                            else if (local.StatusAuditoria == 1)
                            {
                                var page = new Page();
                                var resposta = await page.DisplayAlert("Alerta", "Você esta recebendo uma nova agenda, e consta uma autoria pendente em "+local.Nome+", deseja sobrescrever?", "Sim", "Não");
                                if (resposta)
                                {
                                    dados.DeleteAllAuditorias(local.IdAuditoria);
                                    AuditoriaSync(local, cliente);
                                }
                                else
                                {
                                    AuditoriaSync(local, cliente);
                                }
                            }
                        }
                        #endregion
                        #region Old
                        //List<auditoria> auditoria = new List<auditoria>();
                        //foreach (areas area in local.auditoria)
                        //{
                        //    foreach (itens item in area.Itens)
                        //    {
                        //        foreach (subitens subitem2 in item.SubItens)
                        //        {
                        //            auditoria audit = new auditoria();
                        //            string confirmSource = "";
                        //            string unconfirmSource = "";
                        //            string NaSource = "";
                        //            if (subitem2.SubCheck == 0)
                        //            {
                        //                confirmSource = "confirmnull.png";
                        //                unconfirmSource = "unconfirmnull.png";
                        //                NaSource = "nanull.png";
                        //            }
                        //            if (subitem2.SubCheck == 1)
                        //            {
                        //                confirmSource = "confirmon.png";
                        //                unconfirmSource = "unconfirmnull.png";
                        //                NaSource = "nanull.png";
                        //            }
                        //            if (subitem2.SubCheck == 2)
                        //            {
                        //                confirmSource = "confirmnull.png";
                        //                unconfirmSource = "unconfirmon.png";
                        //                NaSource = "nanull.png";
                        //            }
                        //            if (subitem2.SubCheck == 3)
                        //            {
                        //                LocalStatus = 1;
                        //                confirmSource = "confirmnull.png";
                        //                unconfirmSource = "unconfirmnull.png";
                        //                NaSource = "naon.png";
                        //            }
                        //            string idAuditoria = "" + cliente.IdUnidade
                        //                + local.IdLocal
                        //                + area.AreaCodigo
                        //                + item.ItemCodigo
                        //                + subitem2.SubCodigo + "";

                        //            audit = new auditoria
                        //            {
                        //                IdAuditoria = idAuditoria,
                        //                IdUnidade = cliente.IdUnidade,
                        //                DescArea = area.AreaNome,
                        //                DescItem = item.ItemDescricao,
                        //                DescSubItem = subitem2.SubDescricao,
                        //                IdArea = area.AreaCodigo,
                        //                IdItem = item.ItemCodigo,
                        //                IdLocal = local.IdLocal,
                        //                IdSubItem = subitem2.SubCodigo,
                        //                SubCheck = subitem2.SubCheck,
                        //                SubCheckConfirmSource = confirmSource,
                        //                SubCheckNaSource = NaSource,
                        //                SubCheckUnconfirmSource = unconfirmSource,
                        //                SubDescId = subitem2.SubCodigoTexto,
                        //                SubPhotoUrl = subitem2.SubPhotoUrl,
                        //                SubMotivoBytes = subitem2.SubMotivoBytes
                        //            };
                        //            auditoria.Add(audit);
                        //        }
                        //    }
                        //}
                        //string nomeResp;
                        //string telefoneResp;
                        //string emailResp;
                        //string cargoResp;
                        //if (string.IsNullOrEmpty(local.RespNome))
                        //{
                        //    nomeResp = "Não definido";
                        //}
                        //else
                        //{
                        //    nomeResp = local.RespNome;
                        //}
                        //if (string.IsNullOrEmpty(local.RespTelefone))
                        //{
                        //    telefoneResp = "Não definido";
                        //}
                        //else
                        //{
                        //    telefoneResp = local.RespTelefone;
                        //}
                        //if (string.IsNullOrEmpty(local.RespEmail))
                        //{
                        //    emailResp = "Não definido";
                        //}
                        //else
                        //{
                        //    emailResp = local.RespEmail;
                        //}
                        //if (string.IsNullOrEmpty(local.RespCargo))
                        //{
                        //    cargoResp = "Não definido";
                        //}
                        //else
                        //{
                        //    cargoResp = local.RespCargo;
                        //}
                        //vistoria_temp temp = dados.GetVistoriaTemp(local.IdLocal);
                        //if (temp == null)
                        //{
                        //    vistoria_temp vistoriaTemp = new vistoria_temp
                        //    {
                        //        LocalCodigo = local.IdLocal,
                        //        UnidadeCodigo = cliente.IdUnidade,
                        //        Auditor = cliente.idPessoa,
                        //        RespNome = nomeResp,
                        //        RespTelefone = telefoneResp,
                        //        RespEmail = emailResp,
                        //        RespCargo = cargoResp,
                        //        Assinatura = local.Assinatura
                        //    };
                        //    dados.InserirAtualizarVistoriaTemp(vistoriaTemp);
                        //    dados.AtualizarLocais(local);
                        //}
                        //else if (cliente.IdEvento != local.IdEvento)
                        //{
                        //    cliente.IdEvento = local.IdEvento;
                        //    cliente.cevento = local.cevento;
                        //    vistoria_temp vistoriaTemp = new vistoria_temp
                        //    {
                        //        LocalCodigo = local.IdLocal,
                        //        UnidadeCodigo = cliente.IdUnidade,
                        //        Auditor = cliente.idPessoa,
                        //        RespNome = nomeResp,
                        //        RespTelefone = telefoneResp,
                        //        RespEmail = emailResp,
                        //        RespCargo = cargoResp,
                        //        Assinatura = local.Assinatura
                        //    };
                        //    dados.InserirAtualizarVistoriaTemp(vistoriaTemp);
                        //    dados.AtualizarClientes(cliente);
                        //    dados.AtualizarLocais(local);
                        //}
                        //dados.InserirAtualizarAllAuditorias(auditoria);

                        //if (auditoria == null)
                        //{
                        //    return "HTTP error";
                        //}
                        //dados.AtualizarLocais(local);
                        #endregion
                    }
                }
                App.GLOBAL_SETS.lastLogin = GetTimestamp(DateTime.Now, "diahora");
                dados.AtualizarSettings(App.GLOBAL_SETS);
                dados.Dispose();

                return "Unidades atualizadas";

            }
        }

        #endregion
        //Funcionando
        #region AtualizarUsuario

        public async Task<string> AtualizarUsuarios(string username, string password)
        {
            var versao = DependencyService.Get<IInfoService>().AppVersionName;
            var deviceid = Build.Serial;
            string url = "http://tailler.ddns.net/ws/Usuario/ObterUsuario?usuUsuario=" +
                         username +
                         "&usuSenha=" +
                         password +
                        "&Versao=" +
                         versao +
                        "&DeviceID=" +
                         deviceid;
            var json = await GetAPI(url);

            var jsonSerializerSettings = new JsonSerializerSettings();

            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

            csErro errosList = new csErro();
            usuarios usuario = new usuarios();
            Boolean erro = false;
            String mensagem = "";

            try
            {
                errosList = JsonConvert.DeserializeObject<csErro>(json, jsonSerializerSettings);
                erro = errosList.TeveErro;
                mensagem = errosList.Mensagem;
            }
            catch
            {
                json = json.Replace("[", "");
                json = json.Replace("]", "");
                usuario = JsonConvert.DeserializeObject<usuarios>(json);
            }

            if (erro)
            {
                return mensagem;
            }
            else
            {
                //usuario = JsonConvert.DeserializeObject<usuarios>(json);
                if (usuario.usuStatus == 1)
                {
                    App.GLOBAL_SETS.lastLogin_user = usuario.usuUsuario;
                    App.GLOBAL_SETS.lastLogin_userId = usuario.IdPessoa;

                    using (var dados = new DataAccess())
                    {
                        dados.AtualizarSettings(App.GLOBAL_SETS);
                        usuarios us = dados.GetUsuarios(usuario.IdPessoa);
                        if (us == null)
                        {
                            dados.InserirUsuarios(usuario);
                            dados.Dispose();
                            return "Sucesso";
                        }
                        else
                        {
                            dados.AtualizarUsuarios(usuario);
                            dados.Dispose();
                            return "Sucesso";
                        }
                    }
                }
            }
            return "";
        }

        #endregion
        //Funcionando
        #region AtualizarUnidades

        public async Task<string> AtualizarUnidades(int idPessoa)
        {
            var versao = DependencyService.Get<IInfoService>().AppVersionName;
            var deviceid = Build.Serial;
            string url = "http://tailler.ddns.net/WS/Unidade/ObterUnidades?idPessoa=" +
                         idPessoa +
                         "&Versao=" +
                         versao +
                        "&DeviceID=" +
                         deviceid;

            var json = await GetAPI(url);
            //son = json.ToString();
            if (json == "<!DOCTYPE " || json == "HTTP error")
            {
                return "Ops! Houve um problema com o servidor, entre em contato com um administrador.";
            }
            else
            {
                List<clientes> clientesList = JsonConvert.DeserializeObject<List<clientes>>(json);
                List<VinculoPessoa> vinculosList = JsonConvert.DeserializeObject<List<VinculoPessoa>>(json);
                using (var dados = new DataAccess())
                {
                    dados.InserirAtualizarAllVinculos(vinculosList);
                    dados.InserirAtualizarAllClientes(clientesList);
                    //dados.Dispose();
                }
                return "Unidades atualizadas";
            }
        }

        #endregion
        //Funcionando
        #region AtualizarMotivos
        public async Task AtualizarMotivos(int idSubItem)
        {
            using (var dados = new DataAccess())
            {
                string url = "http://tailler.ddns.net/WS/Textos/ObterTexto2?idSubItem=" +
                             null;
                if (idSubItem != 0)
                {
                    url = "http://tailler.ddns.net/WS/Textos/ObterTexto2?idSubItem=" +
                                 idSubItem;
                }
                var json = await GetAPI(url);
                if (json != "HTTP error")
                {
                    List<motivos> listMotivos = JsonConvert.DeserializeObject<List<motivos>>(json);

                    dados.InserirAtualizarAllMotivos(listMotivos);
                    dados.Dispose();
                }


            }

        }
        #endregion AtualizarLocais 
        //Funcionando
        #region EnviarAuditoria
        public async Task<csErro> EnviarAuditoria(vistoria vistoria)
        {
            string vistoriaString = JsonConvert.SerializeObject(vistoria);

            string url = "http://tailler.ddns.net/ws/Auditoria/SincronizarAuditoria";

            var json = await PostAPI(url, vistoriaString);

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            csErro errosList = JsonConvert.DeserializeObject<csErro>(json, jsonSerializerSettings);
            return errosList;
        }
        #endregion
        //Enviar OS
        #region EnviarOS
        public async Task<csErro> EnviarOS(OS os)
        {
            string osString = JsonConvert.SerializeObject(os);

            var url = "http://tailler.ddns.net/ws/OS/ReceberOS";

            var json = await PostAPI(url, osString);

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            csErro errosList = JsonConvert.DeserializeObject<csErro>(json, jsonSerializerSettings);
            return errosList;
        }
        #endregion
        //Funcionando
        #region EnviarAuditoriaFila
        public async Task<string> EnviarAuditoriaFila(List<locais> filaLocais, clientes cli)
        {
            var internet = CrossConnectivity.Current.IsConnected;
            if (internet)
            {
                await App.NP.Navigation.PushAsync(new Loading("Estamos enviando sua auditoria. Aguarde..."));

                filaLocais = filaLocais.Where(p => p.Cancelado != 1).Where(p => p.StatusAuditoria == 1).ToList();
                foreach (locais loc in filaLocais)
                {
                    if (loc.Assinatura != null)
                    {
                        imagem assinatura = new imagem()
                        {
                            NomeImagem = loc.Assinatura,
                            ArrayBytes = loc.AssinaturaArray,
                            Tipo = "AssinaturaLocal"
                        };
                        var result = await EnviarImagem(assinatura);
                        if (result != "True")
                        {
                            App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                            App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                            return result;
                        }
                        using (var dados = new DataAccess())
                        {
                            //Lembrar de impossibilitar clicar nesse botao nas outras telas.
                            vistoria_temp temp = dados.GetVistoriaTemp(loc.IdLocal);
                            vistoria vistoria = new vistoria
                            {
                                LocalCodigo = temp.LocalCodigo,
                                UnidadeCodigo = temp.UnidadeCodigo,
                                Auditor = temp.Auditor,
                                Data = temp.Data,
                                HoraInicio = temp.HoraInicio,
                                HoraFim = GetTimestamp(DateTime.Now, "hora"),
                                RespNome = loc.RespNome,
                                RespTelefone = loc.RespTelefone,
                                RespEmail = loc.RespEmail,
                                RespCargo = loc.RespCargo,
                                Assinatura = loc.Assinatura,
                                IdAuditoriaBase = loc.IdAuditoriaBase,
                                Norma = loc.IdNorma,
                                TipoVistoria = loc.TipoVistoria,
                                IdEvento = cli.IdEvento,
                                Evento = cli.cevento,
                                Versao = DependencyService.Get<IInfoService>().AppVersionName,
                                DeviceID = Build.Serial
                            };
                            var listaImagens = vistoria.GetAreas(vistoria.LocalCodigo);
                            foreach (var img in listaImagens)
                            {
                                var fotoEnviada = await EnviarImagem(img);
                                if (fotoEnviada != "True")
                                {
                                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                                    return fotoEnviada;
                                }
                            }
                            var response = await EnviarAuditoria(vistoria);
                            if (!response.TeveErro)
                            {
                                loc.StatusAuditoria = 2;
                                loc.StatusAuditoriaFila = 1;
                                loc.IdAuditoria = loc.IdLocal;
                                dados.AtualizarLocais(loc);
                                cli = dados.GetClientes(cli.IdUnidade);
                                bool allLocaisConcluidos = true;
                                List<locais> list = dados.GetAllLocais(cli.IdUnidade);
                                foreach (locais local in list)
                                {
                                    if (local.StatusAuditoria == 1 || local.StatusAuditoria == 0)
                                    {
                                        allLocaisConcluidos = false;
                                    }
                                }
                                if (allLocaisConcluidos)
                                {
                                    cli.StatusAuditoria = 2;
                                    dados.AtualizarClientes(cli);
                                }

                            }
                            else
                            {
                                App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                                App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                                return loc.Nome + " : " + response.Mensagem;
                            }
                        }
                    }
                }
                OS OrdemServico = new OS
                {
                    IdUnidade = cli.IdUnidade,
                    Responsavel = cli.RespNome,
                    IdEvento = cli.IdEvento,
                    CEvento = cli.cevento,
                    Classificacao = cli.Classificacao,
                    IdAuditor = cli.idPessoa,
                    Assinatura = cli.Assinatura,
                    ResponsavelCargo = cli.RespCargo,
                    ResponsavelEmail = cli.RespEmail,
                    ResponsavelTelefone = cli.RespTelefone,
                    Versao = DependencyService.Get<IInfoService>().AppVersionName,
                    DeviceID = Build.Serial

                };

                 var retornoOS = await EnviarOS(OrdemServico);

                if (!retornoOS.TeveErro)
                {
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    return "Sucesso";
                }
                else
                {
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    return retornoOS.Mensagem;
                }
            }
            else
            {
                App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                return "Você não possui conexão com a internet.";
            }
        }
        #endregion
        //Funcionando
        #region CancelarAuditoria

        public async Task<string> CancelarAuditoria(vistoria vistoria)
        {
            string vistoriaString = JsonConvert.SerializeObject(vistoria);

            string url = "http://tailler.ddns.net/ws/Auditoria/retornoauditoria";
            var json = await PostAPI(url, vistoriaString);

            return json;
        }
        #endregion
        //Funcionando
        #region EnviarImagem

        public async Task<string> EnviarImagem(imagem imagem)
        {
            string imagemString = JsonConvert.SerializeObject(imagem);

            string url = "http://tailler.ddns.net/WS/Upload/Imagem";
            var json = await PostAPI(url, imagemString);

            return json;
        }

        #endregion
        //Funcionando
        #region GetApi
        private async Task<string> GetAPI(string endereco)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();

            client.DefaultRequestHeaders.Add("User-Agent", "Other");
            try
            {
                response = await client.GetAsync(string.Format(endereco));
                var content = await response.Content.ReadAsStringAsync();
                response.Dispose();
                client.Dispose();
                return content;
            }
            catch
            {
                return "HTTP error";
            }

        }
        #endregion
        //Funcionando
        #region PostApi
        //private async Task<string> PostAPI(string endereco, string json)
        public async Task<string> PostAPI(string url, string serializedDataString, bool isJson = true)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (isJson)
                    request.ContentType = "application/json";
                else
                    request.ContentType = "application/x-www-form-urlencoded";

                request.Method = "POST";
                var stream = await request.GetRequestStreamAsync();
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(serializedDataString);
                    writer.Flush();
                    writer.Dispose();
                }

                var response = await request.GetResponseAsync();
                var respStream = response.GetResponseStream();

                response.Dispose();
                respStream.Dispose();

                using (StreamReader sr = new StreamReader(respStream))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                return "False";
            }
        }
        #endregion
        //Funcionando
        #region GetTimestamp
        public String GetTimestamp(DateTime value, string modo)
        {
            if (modo == "dia")
            {
                return value.ToString("yyyy-MM-dd");
            }
            else if (modo == "hora")
            {
                return value.ToString("HH:mm:ss");
            }
            else if (modo == "diahora")
            {
                return value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return "";

        }
        #endregion
        //Sincronizando Auditoria
        #region AuditoriaSync
        public String AuditoriaSync(locais local, clientes cliente)
        {
            using (var dados = new DataAccess())
            {
                #region Destrincha objeto "local"
                int LocalStatus = 0;
                List<auditoria> auditoria = new List<auditoria>();
                foreach (areas area in local.auditoria)
                {
                    foreach (itens item in area.Itens)
                    {
                        foreach (subitens subitem2 in item.SubItens)
                        {
                            auditoria audit = new auditoria();
                            string confirmSource = "";
                            string unconfirmSource = "";
                            string NaSource = "";
                            if (subitem2.SubCheck == 0)
                            {
                                confirmSource = "confirmnull.png";
                                unconfirmSource = "unconfirmnull.png";
                                NaSource = "nanull.png";
                            }
                            if (subitem2.SubCheck == 1)
                            {
                                confirmSource = "confirmon.png";
                                unconfirmSource = "unconfirmnull.png";
                                NaSource = "nanull.png";
                            }
                            if (subitem2.SubCheck == 2)
                            {
                                confirmSource = "confirmnull.png";
                                unconfirmSource = "unconfirmon.png";
                                NaSource = "nanull.png";
                            }
                            if (subitem2.SubCheck == 3)
                            {
                                LocalStatus = 1;
                                confirmSource = "confirmnull.png";
                                unconfirmSource = "unconfirmnull.png";
                                NaSource = "naon.png";
                            }
                            string idAuditoria = "" + cliente.IdUnidade
                                + local.IdLocal
                                + area.AreaCodigo
                                + item.ItemCodigo
                                + subitem2.SubCodigo + "";

                            audit = new auditoria
                            {
                                IdAuditoria = idAuditoria,
                                IdUnidade = cliente.IdUnidade,
                                DescArea = area.AreaNome,
                                DescItem = item.ItemDescricao,
                                DescSubItem = subitem2.SubDescricao,
                                IdArea = area.AreaCodigo,
                                IdItem = item.ItemCodigo,
                                IdLocal = local.IdLocal,
                                IdSubItem = subitem2.SubCodigo,
                                SubCheck = subitem2.SubCheck,
                                SubCheckConfirmSource = confirmSource,
                                SubCheckNaSource = NaSource,
                                SubCheckUnconfirmSource = unconfirmSource,
                                SubDescId = subitem2.SubCodigoTexto,
                                SubPhotoUrl = subitem2.SubPhotoUrl,
                                SubMotivoBytes = subitem2.SubMotivoBytes
                            };
                            auditoria.Add(audit);
                        }
                    }
                }
                #endregion
                #region Checa informações
                string nomeResp;
                string telefoneResp;
                string emailResp;
                string cargoResp;
                if (string.IsNullOrEmpty(local.RespNome))
                {
                    nomeResp = "Não definido";
                }
                else
                {
                    nomeResp = local.RespNome;
                }
                if (string.IsNullOrEmpty(local.RespTelefone))
                {
                    telefoneResp = "Não definido";
                }
                else
                {
                    telefoneResp = local.RespTelefone;
                }
                if (string.IsNullOrEmpty(local.RespEmail))
                {
                    emailResp = "Não definido";
                }
                else
                {
                    emailResp = local.RespEmail;
                }
                if (string.IsNullOrEmpty(local.RespCargo))
                {
                    cargoResp = "Não definido";
                }
                else
                {
                    cargoResp = local.RespCargo;
                }
                #endregion
                #region Atualiza vistoria temp
                vistoria_temp temp = dados.GetVistoriaTemp(local.IdLocal);
                if (temp == null)
                {
                    vistoria_temp vistoriaTemp = new vistoria_temp
                    {
                        LocalCodigo = local.IdLocal,
                        UnidadeCodigo = cliente.IdUnidade,
                        Auditor = cliente.idPessoa,
                        RespNome = nomeResp,
                        RespTelefone = telefoneResp,
                        RespEmail = emailResp,
                        RespCargo = cargoResp,
                        Assinatura = local.Assinatura
                    };
                    dados.InserirAtualizarVistoriaTemp(vistoriaTemp);
                    dados.AtualizarLocais(local);
                }
                else if (cliente.IdEvento != local.IdEvento)
                {
                    cliente.IdEvento = local.IdEvento;
                    cliente.cevento = local.cevento;
                    vistoria_temp vistoriaTemp = new vistoria_temp
                    {
                        LocalCodigo = local.IdLocal,
                        UnidadeCodigo = cliente.IdUnidade,
                        Auditor = cliente.idPessoa,
                        RespNome = nomeResp,
                        RespTelefone = telefoneResp,
                        RespEmail = emailResp,
                        RespCargo = cargoResp,
                        Assinatura = local.Assinatura
                    };
                    dados.InserirAtualizarVistoriaTemp(vistoriaTemp);
                    dados.AtualizarClientes(cliente);
                    dados.AtualizarLocais(local);
                }
                #endregion
                #region Atualiza auditorias e local
                dados.InserirAtualizarAllAuditorias(auditoria);

                if (auditoria == null)
                {
                    return "HTTP error";
                }
                dados.AtualizarLocais(local);
                return "";
                #endregion
            }
        }
        #endregion
    }
}