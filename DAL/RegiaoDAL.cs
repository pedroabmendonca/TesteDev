using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using System.Data;
using System.Configuration;
using Interfaces;

namespace DAL
{
    public class RegiaoDAL : IDisposable, IRegiao
    {
        #region variaveis

        private bool Transaction = false;
        private ConexaoDAL conexao;
        private int TimeOut;

        #endregion

        #region Construtor

        public RegiaoDAL(bool transacation)
        {
            Transaction = transacation;
            conexao = new ConexaoDAL(Transaction);
            TimeOut = Convert.ToInt32(ConfigurationSettings.AppSettings["TimeOut"]);
        }

        #endregion

        #region Metodos

        public List<RegiaoDTO> Consultar(Int64 Id)
        {
            try
            {
                List<RegiaoDTO> ListaRegiao = new List<RegiaoDTO>();
                int count = 0;
                IDataReader rd = null;

                var parametros = conexao.GeraParametros(
                        Id
                    );

                bool sucesso = conexao.LerProcedure("NEO_CONS_REGIAO", parametros, out rd, TimeOut);

                if (sucesso)
                {
                    while (rd.Read())
                    {
                        count = 0;

                        //iniciliza as propriedades
                        RegiaoDTO regiao = new RegiaoDTO();

                        regiao.Id = rd.IsDBNull(count) ? 0 : rd.GetInt64(count); count++;
                        regiao.Descricao = rd.IsDBNull(count) ? string.Empty : rd.GetString(count); count++;
                        regiao.Ativo = rd.IsDBNull(count) ? false : !rd.GetBoolean(count); count++;
                        regiao.Estado.Id = rd.IsDBNull(count) ? 0 : rd.GetInt32(count); count++;
                        regiao.Estado.Descricao = rd.IsDBNull(count) ? string.Empty : rd.GetString(count); count++;

                        //adiciona na lista 
                        ListaRegiao.Add(regiao);
                    }
                }

                //retorna a string
                return ListaRegiao;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Incluir(Int64 IdEstado, string Regiao, out string Mensagem)
        {

            bool IsCommit = true;
            IDataReader rd = null;
            Mensagem = string.Empty;

            try
            {
                var parametros = conexao.GeraParametros(
                        IdEstado,
                        Regiao
                    );

                bool sucesso = conexao.LerProcedure("NEO_INS_REGIAO", parametros, out rd, TimeOut);

                if (sucesso)
                {
                    while (rd.Read())
                    {
                        Mensagem = rd.IsDBNull(0) ? string.Empty : rd.GetString(0);
                    }
                }
            }
            catch (Exception ex)
            {
                IsCommit = false;
                throw ex;
            }
            finally
            {
                if (Transaction)
                {
                    if (conexao.SqlConn.Trans != null)
                        rd.Close();

                    //commit
                    conexao.FinalizaConexaoTransaction(IsCommit);
                }

            }
        }

        public void AtivarDesativar(Int64 IdRegiao, bool Ativo)
        {
            bool IsCommit = true;
            IDataReader rd = null;

            try
            {
                var parametros = conexao.GeraParametros(
                        IdRegiao,
                        Ativo
                    );

                conexao.LerProcedure("NEO_ATUALIZA_ATIVA_INATIVA_REGIAO", parametros, out rd, TimeOut);
            }
            catch (Exception ex)
            {
                IsCommit = false;
                throw ex;
            }
            finally
            {
                if (Transaction)
                {
                    if (conexao.SqlConn.Trans != null)
                        rd.Close();

                    //commit
                    conexao.FinalizaConexaoTransaction(IsCommit);
                }

            }
        }

        public void Editar(Int64 IdRegiao, int IdEstado, string DescricaoRegiao, out string Mensagem)
        {
            try
            {
                bool IsCommit = true;
                IDataReader rd = null;
                Mensagem = string.Empty;

                try
                {
                    var parametros = conexao.GeraParametros(
                            IdRegiao,
                            IdEstado,
                            DescricaoRegiao
                        );

                    bool sucesso = conexao.LerProcedure("NEO_ATUALIZA_REGIAO", parametros, out rd, TimeOut);

                    if (sucesso)
                    {
                        while (rd.Read())
                        {
                            Mensagem = rd.IsDBNull(0) ? string.Empty : rd.GetString(0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    IsCommit = false;
                    throw ex;
                }
                finally
                {
                    if (Transaction)
                    {
                        if (conexao.SqlConn.Trans != null)
                            rd.Close();

                        //commit
                        conexao.FinalizaConexaoTransaction(IsCommit);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            if (conexao.SqlConn.Trans == null)
            {
                if (conexao.SqlConn.Conexao.State == ConnectionState.Open)
                {
                    conexao.SqlConn.Conexao.Close();
                    conexao.SqlConn.Conexao.Dispose();
                    GC.SuppressFinalize(conexao.SqlConn.Conexao);
                }
            }
        }

        #endregion
    }
}
