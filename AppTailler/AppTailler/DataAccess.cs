using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using Xamarin.Forms;
using System.IO;
using AppTailler.Models;
using SQLite.Net.Interop;

namespace AppTailler
{
    class DataAccess : IDisposable
    {
        private SQLiteConnection connection;

        public DataAccess()
        {
            var config = DependencyService.Get<IConfig>();
            config.Plataforma.SQLiteApi.Config(ConfigOption.Serialized);
            connection = new SQLiteConnection(config.Plataforma, Path.Combine(config.DiretorioDB, "Tailler.db3"));
            connection.CreateTable<settings>();
            connection.CreateTable<usuarios>();
            connection.CreateTable<clientes>();
            connection.CreateTable<VinculoPessoa>();
            connection.CreateTable<locais>();
            connection.CreateTable<auditoria>();
            connection.CreateTable<vistoria_temp>();
            connection.CreateTable<motivos>();
            connection.CreateTable<filaAuditoria>();
        }
        public filaAuditoria GetfilaAuditoria(int id)
        {
            return connection.Table<filaAuditoria>().FirstOrDefault(c => c.IdPessoa == id);
        }
        public void DeletefilaAuditoria(filaAuditoria fila)
        {
            connection.Delete(fila);
        }
        public void InserirFilaAuditoria(filaAuditoria filaAuditoria)
        {
            connection.Insert(filaAuditoria);
        }
        public void AtualizarFilaAuditoria(filaAuditoria filaAuditoria)
        {
            connection.Update(filaAuditoria);
        }

        public void InserirSettings(settings setting)
        {
            connection.Insert(setting);
        }

        public void AtualizarSettings(settings setting)
        {
            connection.Update(setting);
        }

        public settings GetSettings(int id)
        {
            return connection.Table<settings>().FirstOrDefault(c => c.ID == id);
        }

        public void InserirUsuarios(usuarios usuario)
        {
            connection.Insert(usuario);
        }

        public void AtualizarUsuarios(usuarios usuario)
        {
            connection.Update(usuario);
        }

        public usuarios GetUsuarios(int id)
        {
            return connection.Table<usuarios>().FirstOrDefault(c => c.IdPessoa == id);
        }

        public usuarios GetUsuariosByLogin(string login)
        {
            return connection.FindWithQuery<usuarios>("select * from usuarios where usuUsuario='" + login + "';");
        }

        public void InserirAtualizarAllClientes(List<clientes> cliente)
        {
            connection.InsertOrReplaceAll(cliente);
        }

        public void InserirAtualizarAllVinculos(List<VinculoPessoa> vinculo)
        {
            connection.InsertOrReplaceAll(vinculo);
        }

        public void AtualizarClientes(clientes cliente)
        {
            connection.Update(cliente);
        }

        public void AtualizarStatusAuditoriaClientes(clientes cliente)
        {
            string query = "update clientes set " +
                           "StatusAuditoria= " + cliente.StatusAuditoria + " where IdUnidade= " + cliente.IdUnidade + ";";
            connection.Execute(query);
        }

        public clientes GetClientes(int id)
        {
            return connection.FindWithQuery<clientes>("select * from clientes where IdUnidade=" + id);
        }

        public List<clientes> GetAllClientes(int id)
        {
            //return connection.Query<clientes>("select * from clientes where idPessoa=" + id).ToList();
            return connection.Query<clientes>("select * from clientes where IdUnidade in (select IdUnidade from VinculoPessoa where idPessoa = " + id+");").ToList();
        }

        public List<clientes> GetFilteredClientes(int id, string text)
        {
            string query = "select * from clientes where idPessoa=" + id + " and Unidade LIKE '%" + text + "%'";
            return connection.Query<clientes>(query).ToList();
        }

        public void InserirAtualizarAllLocais(List<locais> local)
        {
            connection.InsertOrIgnoreAll(local);
        }

        public void AtualizarLocais(locais local)
        {
            connection.Update(local);
        }

        public void AtualizarRespAssinaturaLocais(locais local)
        {
            string query = "update locais set "
                + "RespCargo= '" + local.RespCargo
                + "' ,RespNome= '" + local.RespNome
                + "' ,RespNome= '" + local.RespNome
                + "' ,Assinatura= '" + local.Assinatura
                + "' ,AssinaturaArray= '" + local.AssinaturaArray
                + "' where IdLocal= " + local.IdLocal + ";";
            connection.Execute(query);
        }

        public void AtualizarStatusAuditoriaLocais(locais local)
        {
            string query = "update locais set " + "IdAuditoria= " + local.IdAuditoria +
                           ",StatusAuditoria= " + local.StatusAuditoria + " where IdLocal= " + local.IdLocal + ";";
            connection.Execute(query);
        }

        public locais GetLocais(int id)
        {
            return connection.FindWithQuery<locais>("select * from locais where IdLocal=" + id);
        }

        public List<locais> GetAllLocais(int id)
        {
            return connection.Query<locais>("select * from locais where IdUnidade=" + id).ToList();
        }

        public List<locais> GetFilteredLocais(int id, string text)
        {
            string query = "select * from locais where IdUnidade=" + id + " and Nome LIKE '%" + text + "%'";
            return connection.Query<locais>(query).ToList();
        }

        public void AtualizarAuditoria(auditoria auditoria)
        {
            connection.Update(auditoria);
        }

        public void CheckAtualizarAuditoria(auditoria auditoria)
        {
            string query = "update auditoria set " +
                           "SubCheck=" + auditoria.SubCheck +
                           " , SubCheckUnconfirmSource= '" + auditoria.SubCheckUnconfirmSource +
                           "' , SubCheckConfirmSource= '" + auditoria.SubCheckConfirmSource +
                           "' , SubCheckNaSource= '" + auditoria.SubCheckNaSource +
                           "' , SubChecked= " + auditoria.SubChecked +
                           " where IdAuditoria= " + auditoria.IdAuditoria +
                           ";";
            connection.Execute(query);
        }
        public auditoria GetAuditoria(string id)
        {
            return connection.FindWithQuery<auditoria>("select * from auditoria where IdAuditoria=" + id);
        }
        public auditoria GetIdAuditoria(int id)
        {
            return connection.Table<auditoria>().FirstOrDefault(c => c.IdLocal == id);
        }
        public List<auditoria> GetAllAuditorias(int id)
        {
            return connection.Query<auditoria>("select * from auditoria where IdLocal=" + id).ToList();
        }
        public List<auditoria> GetAllAreasAuditorias(int id)
        {
            return connection.Query<auditoria>("select distinct IdArea, DescArea, StatusArea from auditoria where IdLocal=" + id).ToList();
        }
        public List<auditoria> GetAllItensAuditorias(int id)
        {
            return connection.Query<auditoria>("select distinct IdItem, DescItem, StatusItem from auditoria where IdLocal=" + id).ToList();
        }
        public List<auditoria> GetAllSubItensAuditorias(int id)
        {
            return connection.Query<auditoria>("select * from auditoria where IdLocal=" + id).ToList();
        }

        public void DeleteAllAuditorias(int id)
        {
            connection.Query<auditoria>("delete from auditoria where IdAuditoria=" + id);
        }
        public void InserirAtualizarVistoriaTemp(vistoria_temp vistoria)
        {
            connection.InsertOrReplace(vistoria);
        }
        public vistoria_temp GetVistoriaTemp(int id)
        {
            return connection.FindWithQuery<vistoria_temp>("select * from vistoria_temp where LocalCodigo=" + id);
        }

        public void InserirAtualizarAllAuditorias(List<auditoria> auditoria)
        {
            connection.InsertOrReplaceAll(auditoria);
        }

        public void InserirAtualizarAllMotivos(List<motivos> motivos)
        {
            connection.InsertOrIgnoreAll(motivos);
        }

        public motivos GetMotivos(int id)
        {
            return connection.Table<motivos>().FirstOrDefault(c => c.IdTextoVistoria == id);
        }

        public List<motivos> GetAllMotivos(int id)
        {
            return connection.Query<motivos>("select * from motivos where idSubItem=" + id).ToList();
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
