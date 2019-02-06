using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contract;
using System.Data;
using System.Configuration;
using DTO;

namespace DAL
{
    public class FornecedorDAL : IDisposable, IFornecedor
    {
        #region variaveis

        private bool Transaction = false;
        private ConexaoDAL conexao;
        private int TimeOut;

        #endregion

        #region Construtor

        public FornecedorDAL(bool transacation)
        {
            Transaction = transacation;
            conexao = new ConexaoDAL(Transaction);
            TimeOut = Convert.ToInt32(ConfigurationSettings.AppSettings["TimeOut"]);
        }

        #endregion

        #region Metodos

        public List<FornecedorDTO> Consultar(long Id)
        {
            try
            {
                List<FornecedorDTO> ListaFornecedor = new List<FornecedorDTO>();
                int count = 0;
                IDataReader rd = null;

                var parametros = conexao.GeraParametros(
                        Id
                    );

                bool sucesso = conexao.LerProcedure("NEO_CONS_FORNECEDOR", parametros, out rd, TimeOut);

                if (sucesso)
                {
                    while (rd.Read())
                    {
                        count = 0;

                        //inicializa o objeto
                        FornecedorDTO forn = new FornecedorDTO();

                        //iniciliza as propriedades
                        forn.Id = rd.IsDBNull(count) ? 0 : rd.GetInt64(count); count++;
                        forn.Cnpj = rd.IsDBNull(count) ? string.Empty : rd.GetString(count); count++;
                        forn.Nome = rd.IsDBNull(count) ? string.Empty : rd.GetString(count); count++;

                        //adiciona na lista 
                        ListaFornecedor.Add(forn);
                    }
                }

                //retorna a string
                return ListaFornecedor;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public FornecedorDTO ConsultarFornecedorRegiao(long IdFornecedor)
        {
            try
            {
                FornecedorDTO forn = new FornecedorDTO();

                int count = 0;
                IDataReader rd = null;

                var parametros = conexao.GeraParametros(
                        IdFornecedor
                    );

                bool sucesso = conexao.LerProcedure("NEO_CONS_FORNECEDOR_REGIAO", parametros, out rd, TimeOut);

                if (sucesso)
                {
                    while (rd.Read())
                    {
                        count = 0;

                        //inicializa o objeto
                        RegiaoDTO regiao = new RegiaoDTO();

                        regiao.Id = rd.IsDBNull(count) ? 0 : rd.GetInt64(count); count++;
                        regiao.Descricao = rd.IsDBNull(count) ? string.Empty : rd.GetString(count); count++;
                        regiao.Ativo = rd.IsDBNull(count) ? false : rd.GetBoolean(count); count++;
                        regiao.Estado.Id = rd.IsDBNull(count) ? 0 : rd.GetInt32(count); count++;
                        regiao.Estado.Descricao = rd.IsDBNull(count) ? string.Empty : rd.GetString(count); count++;

                        //adiciona na lista 
                        forn.LstRegiao.Add(regiao);
                    }
                }

                //retorna a string
                return forn;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Editar(Int64 IdFornecedor, DataTable FornecedorRegiao)
        {
            try
            {
                bool IsCommit = true;
                IDataReader rd = null;

                try
                {
                    var parametros = conexao.GeraParametros(
                            IdFornecedor,
                            FornecedorRegiao
                        );

                    conexao.LerProcedure("NEO_ATUALIZA_FORNECEDOR_REGIAO", parametros, out rd, TimeOut);
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
