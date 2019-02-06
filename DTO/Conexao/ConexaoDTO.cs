using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DTO
{
    public class ConexaoDTO
    {
        #region privado

        private string _Servidor;
        private string _UserName;
        private string _Password;
        private string _DataBase;
        private string _StrConexao;
        private IDbConnection _Conexao;
        private IDbTransaction _Trans;

        #endregion

        #region publico

        public string Servidor
        {
            get { return _Servidor; }
            set { _Servidor = value; }
        }
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        public string DataBase
        {
            get { return _DataBase; }
            set { _DataBase = value; }
        }

        public string StrConexao
        {
            get { return _StrConexao; }
            set { _StrConexao = value; }
        }

        public IDbConnection Conexao
        {
            get
            {
                return _Conexao;
            }
            set { _Conexao = value; }
        }

        public IDbTransaction Trans
        {
            get
            {
                return _Trans;
            }
            set { _Trans = value; }
        }

        #endregion
    }
}
