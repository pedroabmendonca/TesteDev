using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DTO;
using System.Configuration;
using Interfaces;

namespace DAL
{
    public class EstadoDAL : IDisposable, IEstado
    {
        #region variaveis

        private bool Transaction = false;
        private ConexaoDAL conexao;
        private int TimeOut;

        #endregion

        #region Construtor

        public EstadoDAL(bool transacation)
        {
            Transaction = transacation;
            conexao = new ConexaoDAL(Transaction);
            TimeOut = Convert.ToInt32(ConfigurationSettings.AppSettings["TimeOut"]);
        }

        #endregion

        #region Metodos

        public List<EstadoDTO> Consultar(int Id)
        {
            try
            {
                List<EstadoDTO> ListaEstado = new List<EstadoDTO>();
                int count = 0;
                IDataReader rd = null;

                var parametros = conexao.GeraParametros(
                        Id
                    );

                bool sucesso = conexao.LerProcedure("NEO_CONS_ESTADO", parametros, out rd, TimeOut);

                if (sucesso)
                {
                    while (rd.Read())
                    {
                        count = 0;

                        //inicializa o objeto
                        EstadoDTO estado = new EstadoDTO();

                        //iniciliza as propriedades
                        estado.Id = rd.IsDBNull(count) ? 0 : rd.GetInt32(count); count++;
                        estado.Descricao = rd.IsDBNull(count) ? string.Empty : rd.GetString(count); count++;

                        //adiciona na lista 
                        ListaEstado.Add(estado);
                    }
                }

                //retorna a string
                return ListaEstado;
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
