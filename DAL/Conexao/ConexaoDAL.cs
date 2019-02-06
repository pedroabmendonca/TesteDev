using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data;
using System.Configuration;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Reflection;
using System.Collections;
using DTO;

namespace DAL
{
    public class ConexaoDAL
    {
        #region variaveis

        private Hashtable mobjHashTableParameters; //Hashtable contendo  System.Data.IDataParameterCollection para cada proc
        protected string mstrLastError = ""; //mensagem de erro
        private ProviderFactory pf = null;
        private ProviderType menuProvider;

        private ConexaoDTO _SqlConn;
        public ConexaoDTO SqlConn
        {
            get
            {
                if (_SqlConn == null)
                    _SqlConn = new ConexaoDTO();

                return _SqlConn;
            }
            set { _SqlConn = value; }
        }

        #endregion

        #region construtor

        public ConexaoDAL(bool OpenTransaction)
        {
            try
            {
                //inicializa a hashtable para os parametros
                mobjHashTableParameters = new Hashtable();

                //providerBD
                menuProvider = (ProviderType)Convert.ToInt32(ConfigurationSettings.AppSettings["ProviderBD"]);

                //string conexao
                string StrConexao = ConfigurationSettings.AppSettings["Conexao"];

                //seta os valores principais
                SqlConn.StrConexao = ConfigurationSettings.AppSettings["Conexao"];
                SqlConn.Servidor = ConfigurationSettings.AppSettings["DataSource"];
                SqlConn.UserName = ConfigurationSettings.AppSettings["UserName"];
                SqlConn.Password = ConfigurationSettings.AppSettings["PassWord"];
                SqlConn.DataBase = ConfigurationSettings.AppSettings["DataBase"];

                if (!String.IsNullOrEmpty(StrConexao))
                {
                    //inicializa o provider
                    pf = new ProviderFactory(menuProvider);

                    //iniciliza a conexao
                    SqlConn.Conexao = pf.CreateConnection(StrConexao);
                    SqlConn.Conexao.Open();

                    //inicializa a transação
                    if (OpenTransaction)
                    {
                        if (SqlConn.Trans == null)
                            SqlConn.Trans = SqlConn.Conexao.BeginTransaction(IsolationLevel.ReadCommitted);
                    }
                }
                else
                {
                    throw new Exception("String de Conexão está vazio ou inválido");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ConexaoDAL Error: " + ex.Message);
            }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Retorna dados para a leitura das informações do banco de dados
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="param"></param>
        /// <param name="TypeCommando"></param>
        /// <param name="TimeOut"></param>
        /// <returns></returns>        
        //public virtual bool LerProcedure(string pstrCommand, object[] parrParam, CommandType TypeCommando, out SqlDataReader pobjDataReader, int TimeOut)
        //{
        //    int intIndexParams = 0;
        //    int intErrorParamIndex = -1;
        //    bool blnRet = true;
        //    SqlParameter objParamErroRetorno;
        //    string strValorParamErro;

        //    try
        //    {

        //        SqlCommand objCmd = new SqlCommand();

        //        objCmd.CommandText = pstrCommand;
        //        objCmd.CommandType = TypeCommando;
        //        if (TimeOut != 0)
        //            objCmd.CommandTimeout = TimeOut;

        //        if (SqlConn.Trans != null)
        //            objCmd.Transaction = SqlConn.Trans;

        //        DeriveParameters(objCmd);

        //        if (parrParam != null)
        //        {
        //            for (intIndexParams = 0; intIndexParams < parrParam.Length; intIndexParams++)
        //            {
        //                SqlParameter iDataParam = new SqlParameter();

        //                iDataParam = (SqlParameter)objCmd.Parameters[intIndexParams + 1];
        //                if (parrParam[intIndexParams] != null)
        //                    iDataParam.Value = parrParam[intIndexParams];
        //                else
        //                    iDataParam.Value = DBNull.Value;
        //            }
        //        }

        //        objCmd.Prepare();
        //        pobjDataReader = objCmd.ExecuteReader();

        //        //testar retorno erro
        //        if (intErrorParamIndex > -1)
        //        {
        //            objParamErroRetorno = (SqlParameter)objCmd.Parameters[intErrorParamIndex];
        //            strValorParamErro = objParamErroRetorno.Value.ToString();
        //            blnRet = (strValorParamErro == "0");
        //        }

        //        return blnRet;
        //    }
        //    catch (SystemException e)
        //    {
        //        throw e;
        //    }

        //}
        public virtual bool LerProcedure(string pstrCommand, object[] parrParam, out IDataReader pobjDataReader, int timeout)
        {
            int intIndexParams = 0;

            // Para o provider nativo SQL, verificamos que o primeiro parâmetro (indice 0)
            //é o código de retorno da procedure
            //Para o provider OleDB não existe esse parametro 0. Testamos apenas com IBMDA400
            int intParamOleDB;

            int intErrorParamIndex = -1;
            bool blnRet = true;
            IDataParameter objParamErroRetorno;
            string strValorParamErro;

            try
            {

                IDbCommand objCmd = pf.CreateCommand();

                objCmd.CommandText = pstrCommand;
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.CommandTimeout = timeout;
                objCmd.Connection = SqlConn.Conexao;

                if (SqlConn.Trans != null)
                    objCmd.Transaction = SqlConn.Trans;

                DeriveParameters(ref objCmd);


                // Como não existe uma interface para o CommandBuilder, 
                // precisamos determinar que tipo utilizaremos.
                switch (menuProvider)
                {
                    case ProviderType.SqlClient:
                        intParamOleDB = 1;
                        break;
                    case ProviderType.OracleClient:
                    case ProviderType.ODBC:
                    case ProviderType.OleDb:
                    case ProviderType.MySqlClient:
                        intParamOleDB = 0;
                        break;
                    default:
                        intParamOleDB = 0;
                        break;
                }

                if (parrParam != null)
                {
                    for (intIndexParams = 0; intIndexParams < parrParam.Length; intIndexParams++)
                    {
                        IDataParameter iDataParam = pf.CreateDataParameter();

                        iDataParam = (IDataParameter)objCmd.Parameters[intIndexParams + intParamOleDB];
                        if (parrParam[intIndexParams] != null)
                            iDataParam.Value = parrParam[intIndexParams];
                        else
                            iDataParam.Value = DBNull.Value;
                    }
                }

                objCmd.Prepare();

                if (menuProvider.Equals(ProviderType.MySqlClient))
                    pobjDataReader = objCmd.ExecuteReader(CommandBehavior.SequentialAccess);
                else
                    pobjDataReader = objCmd.ExecuteReader();

                //testar retorno erro
                if (intErrorParamIndex > -1)
                {
                    objParamErroRetorno = (IDataParameter)objCmd.Parameters[intErrorParamIndex];
                    strValorParamErro = objParamErroRetorno.Value.ToString();
                    blnRet = (strValorParamErro == "0");
                }

                return blnRet;
            }
            catch (SystemException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Executa o comando e retorna a quantidade de linha afetadas
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="param"></param>
        /// <param name="TypeCommando"></param>
        /// <param name="TimeOut"></param>
        //public virtual bool EscreverProcedure(string pstrCommand, object[] parrParamIn, CommandType TypeCommando, out object[] parrParamOut, int Timeout)
        //{
        //    int intIndexParams;
        //    int intErrorParamIndex = -1;
        //    bool blnRet = true;
        //    SqlParameter objParamErroRetorno;
        //    string strValorParamErro;

        //    try
        //    {

        //        SqlCommand objCmd = new SqlCommand();
        //        objCmd.CommandText = pstrCommand;
        //        objCmd.CommandType = TypeCommando;
        //        if (Timeout > 0)
        //            objCmd.CommandTimeout = Timeout;

        //        DeriveParameters(objCmd);

        //        if (parrParamIn != null)
        //        {
        //            for (intIndexParams = 0; intIndexParams < parrParamIn.Length; intIndexParams++)
        //            {
        //                SqlParameter iDataParam = new SqlParameter();

        //                iDataParam = (SqlParameter)objCmd.Parameters[intIndexParams + 1];
        //                if (parrParamIn[intIndexParams] != null && parrParamIn[intIndexParams].ToString() != "")
        //                    iDataParam.Value = parrParamIn[intIndexParams];
        //                else
        //                    iDataParam.Value = DBNull.Value;
        //            }

        //            // A procedure deve ter como último param
        //            // o param de retorno de erro
        //            SqlParameter objParamErro = new SqlParameter();
        //        }

        //        objCmd.ExecuteNonQuery();

        //        //Copiar os parametros de retorno
        //        CopiarParamsRetorno(objCmd.Parameters, out parrParamOut);

        //        //testar retorno erro
        //        if (intErrorParamIndex > -1)
        //        {
        //            objParamErroRetorno = (SqlParameter)objCmd.Parameters[intErrorParamIndex];
        //            strValorParamErro = objParamErroRetorno.Value.ToString();
        //            blnRet = (strValorParamErro == "0" || strValorParamErro == "");
        //            mstrLastError = strValorParamErro;
        //        }
        //    }
        //    catch (SystemException e)
        //    {
        //        throw e;
        //    }
        //    return blnRet;
        //}
        public virtual bool EscreverProcedure(string pstrCommand, object[] parrParamIn, out object[] parrParamOut, int pintTimeout)
        {
            int intIndexParams;
            int intErrorParamIndex = -1;
            bool blnRet = true;
            IDataParameter objParamErroRetorno;
            //IDataParameter	objParamRetorno; 
            string strValorParamErro;


            //Para o provider OleDB não existe esse parametro 0. Testamos apenas com IBMDA400
            int intParamOleDB;

            try
            {

                IDbCommand objCmd = pf.CreateCommand();

                objCmd.CommandText = pstrCommand;
                if (pintTimeout > 0)
                    objCmd.CommandTimeout = pintTimeout;
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Connection = SqlConn.Conexao;

                if (SqlConn.Trans != null)
                    objCmd.Transaction = SqlConn.Trans;

                DeriveParameters(ref objCmd);

                // Como não existe uma interface para o CommandBuilder, 
                // precisamos determinar que tipo utilizaremos.
                switch (menuProvider)
                {
                    case ProviderType.SqlClient:
                    case ProviderType.MySqlClient:
                        intParamOleDB = 1;
                        break;
                    case ProviderType.OracleClient:
                    case ProviderType.ODBC:
                    case ProviderType.OleDb:
                        intParamOleDB = 0;
                        break;
                    default:
                        intParamOleDB = 0;
                        break;
                }

                if (parrParamIn != null)
                {
                    for (intIndexParams = 0; intIndexParams < parrParamIn.Length; intIndexParams++)
                    {
                        IDataParameter iDataParam = pf.CreateDataParameter();

                        iDataParam = (IDataParameter)objCmd.Parameters[intIndexParams + intParamOleDB];
                        if (parrParamIn[intIndexParams] != null && parrParamIn[intIndexParams].ToString() != "")
                            iDataParam.Value = parrParamIn[intIndexParams];
                        else
                            iDataParam.Value = DBNull.Value;
                    }

                    // A procedure deve ter como último param
                    // o param de retorno de erro

                    IDataParameter objParamErro = pf.CreateDataParameter();
                }


                if (menuProvider == ProviderType.ODBC)
                    MontaCommandTextODBC(ref objCmd);

                objCmd.ExecuteNonQuery();

                //Copiar os parametros de retorno
                CopiarParamsRetorno(objCmd.Parameters, out parrParamOut);

                //testar retorno erro
                if (intErrorParamIndex > -1)
                {
                    objParamErroRetorno = (IDataParameter)objCmd.Parameters[intErrorParamIndex];
                    strValorParamErro = objParamErroRetorno.Value.ToString();
                    blnRet = (strValorParamErro == "0" || strValorParamErro == "");
                    mstrLastError = strValorParamErro;
                }


            }
            catch (SystemException e)
            {
                throw e;
            }
            return blnRet;
        }

        /// <summary>
        /// finaliza a conexao e a transaction
        /// </summary>
        public void FinalizaConexaoTransaction(bool Commit)
        {
            try
            {
                //verifica se a transação está em aberto
                if (SqlConn.Trans != null)
                {
                    if (Commit)
                    {
                        SqlConn.Trans.Commit();
                        SqlConn.Trans.Dispose();
                    }
                    else
                    {
                        SqlConn.Trans.Rollback();
                        SqlConn.Trans.Dispose();
                    }
                }

                //verifica se a conexao está aberta
                if (SqlConn.Conexao.State == ConnectionState.Open)
                {
                    SqlConn.Conexao.Close();
                    SqlConn.Conexao.Dispose();
                    GC.SuppressFinalize(SqlConn.Conexao);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("FinalizaConexaoTransaction Error: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// responsavel por buscar os parametros no banco da procedure
        /// </summary>
        /// <param name="Command"></param>
        //private void DeriveParameters(SqlCommand Command)
        //{
        //    SqlParameter[] iStoredParameters;
        //    SqlParameter[] iCommandParameters;


        //    /*
        //     * Verifica se já existe a collection parameters criada
        //     */
        //    iStoredParameters = (SqlParameter[])mobjHashTableParameters[Command.CommandText];

        //    if (iStoredParameters != null)
        //    {
        //        iCommandParameters = CloneParameters(iStoredParameters);
        //        foreach (IDataParameter p in iCommandParameters)
        //            Command.Parameters.Add(p);

        //        Command.Connection = SqlConn.Conexao;

        //        ////atribui a transacao se for o caso
        //        if (SqlConn.Trans != null)
        //            Command.Transaction = SqlConn.Trans;
        //    }
        //    else
        //    {
        //        //Se não existir, consulta
        //        //associar a conexão para listar os parâmetros da proc
        //        Command.Connection = SqlConn.Conexao;
        //        SqlCommandBuilder.DeriveParameters((SqlCommand)Command);

        //        //Adiciona na collection
        //        iStoredParameters = new SqlParameter[Command.Parameters.Count];

        //        Command.Parameters.CopyTo(iStoredParameters, 0);
        //        mobjHashTableParameters.Add(Command.CommandText, iStoredParameters);

        //        foreach (SqlParameter parameter in Command.Parameters)
        //        {
        //            if (parameter.SqlDbType != SqlDbType.Structured)
        //            {
        //                continue;
        //            }
        //            string name = parameter.TypeName;
        //            int index = name.IndexOf(".");
        //            if (index == -1)
        //            {
        //                continue;
        //            }
        //            name = name.Substring(index + 1);
        //            if (name.Contains("."))
        //            {
        //                parameter.TypeName = name;
        //            }
        //        }

        //        //devolve o cammand para a conexão original
        //        Command.Connection = SqlConn.Conexao;

        //        //atribui a transacao se for o caso
        //        if (SqlConn.Trans != null)
        //            Command.Transaction = SqlConn.Trans;
        //    }
        //}
        private void DeriveParameters(ref IDbCommand pobjCommand)
        {
            string strCommandText;
            IDataParameter[] iStoredParameters;
            IDataParameter[] iCommandParameters;


            /*
             * Verifica se já existe a collection parameters criada
             */
            iStoredParameters = (IDataParameter[])mobjHashTableParameters[pobjCommand.CommandText];

            if (iStoredParameters != null)
            {
                iCommandParameters = CloneParameters(iStoredParameters);
                foreach (IDataParameter p in iCommandParameters)
                    pobjCommand.Parameters.Add(p);

                pobjCommand.Connection = SqlConn.Conexao;

                //atribui a transacao se for o caso
                if (SqlConn.Trans != null)
                    pobjCommand.Transaction = SqlConn.Trans;
            }
            else
            {
                //Se não existir, consulta

                //associar a conexão para listar os parâmetros da proc
                pobjCommand.Connection = SqlConn.Conexao;

                //atribui a transacao se for o caso
                if (SqlConn.Trans != null)
                    pobjCommand.Transaction = SqlConn.Trans;

                switch (menuProvider)
                {
                    case ProviderType.SqlClient:
                        SqlCommandBuilder.DeriveParameters((SqlCommand)pobjCommand);
                        break;

                    case ProviderType.OleDb:
                        OleDbCommandBuilder.DeriveParameters((OleDbCommand)pobjCommand);
                        break;

                    case ProviderType.ODBC:
                        strCommandText = pobjCommand.CommandText;
                        pobjCommand.CommandText = strCommandText.Substring(strCommandText.IndexOf(".") + 1);

                        OdbcCommandBuilder.DeriveParameters((OdbcCommand)pobjCommand);
                        pobjCommand.CommandText = strCommandText;
                        break;

                    case ProviderType.OracleClient:
                        //strCommandText = pobjCommand.CommandText;
                        //pobjCommand.CommandText = strCommandText;
                        //pobjCommand.CommandText = strCommandText.Substring(strCommandText.IndexOf(".") + 1);

                        //OracleCommandBuilder.DeriveParameters((OracleCommand)pobjCommand);
                        //pobjCommand.CommandText = strCommandText;
                        break;

                    case ProviderType.MySqlClient:
                        //MySqlCommandBuilder.DeriveParameters((MySqlCommand)pobjCommand);
                        break;
                }

                //Adiciona na collection
                iStoredParameters = new IDataParameter[pobjCommand.Parameters.Count];

                pobjCommand.Parameters.CopyTo(iStoredParameters, 0);
                mobjHashTableParameters.Add(pobjCommand.CommandText, iStoredParameters);

                if (menuProvider == ProviderType.SqlClient)
                {
                    /* TASK #7296 - Necessidade de passar o DATATABLE direto */
                    //Para parametros DataTable funcionarem, é necessario realizar a verificação do TypeName gerado através do SqlCommandBuilder.DeriveParameters
                    //pois ele é gereado concatenado com o nome do banco de dados e é necessário remover ele para que funcione.
                    foreach (SqlParameter parameter in pobjCommand.Parameters)
                    {
                        if (parameter.SqlDbType != SqlDbType.Structured)
                        {
                            continue;
                        }
                        string name = parameter.TypeName;
                        int index = name.IndexOf(".");
                        if (index == -1)
                        {
                            continue;
                        }
                        name = name.Substring(index + 1);
                        if (name.Contains("."))
                        {
                            parameter.TypeName = name;
                        }
                    }
                }

                //devolve o cammand para a conexão original
                pobjCommand.Connection = SqlConn.Conexao;

                //atribui a transacao se for o caso
                if (SqlConn.Trans != null)
                    pobjCommand.Transaction = SqlConn.Trans;
            }
        }

        /// <summary>
        /// responsavel por clonar os parametros da procedure
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        //private SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        //{
        //    SqlParameter[] clonedParameters;

        //    clonedParameters = (SqlParameter[])new SqlParameter[originalParameters.Length];

        //    for (int i = 0, j = originalParameters.Length; i < j; i++)
        //    {
        //        clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
        //    }

        //    return (SqlParameter[])clonedParameters;
        //}
        private IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
        {
            IDataParameter[] clonedParameters = null;


            switch (menuProvider)
            {
                case ProviderType.SqlClient:
                    clonedParameters = (IDataParameter[])new SqlParameter[originalParameters.Length];

                    for (int i = 0, j = originalParameters.Length; i < j; i++)
                    {
                        clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
                    }
                    break;

                case ProviderType.OleDb:
                    clonedParameters = (IDataParameter[])new OleDbParameter[originalParameters.Length];

                    for (int i = 0, j = originalParameters.Length; i < j; i++)
                    {
                        clonedParameters[i] = (OleDbParameter)((ICloneable)originalParameters[i]).Clone();
                    }
                    break;

                case ProviderType.ODBC:
                    clonedParameters = (IDataParameter[])new OdbcParameter[originalParameters.Length];

                    for (int i = 0, j = originalParameters.Length; i < j; i++)
                    {
                        clonedParameters[i] = (OdbcParameter)((ICloneable)originalParameters[i]).Clone();
                    }
                    break;
                case ProviderType.OracleClient:
                    //clonedParameters = (IDataParameter[])new OracleParameter[originalParameters.Length];

                    //for (int i = 0, j = originalParameters.Length; i < j; i++)
                    //{
                    //    clonedParameters[i] = (OracleParameter)((ICloneable)originalParameters[i]).Clone();
                    //}
                    break;
                case ProviderType.MySqlClient:
                    //clonedParameters = (IDataParameter[])new MySqlParameter[originalParameters.Length];

                    //for (int i = 0, j = originalParameters.Length; i < j; i++)
                    //{
                    //    clonedParameters[i] = (MySqlParameter)((ICloneable)originalParameters[i]).Clone();
                    //}
                    break;
                default:
                    throw (new SystemException("ProviderType não informado!"));

            }

            return (IDataParameter[])clonedParameters;
        }

        /// <summary>
        /// responsavel por gerar os parametros
        /// </summary>
        /// <param name="p_arrParams"></param>
        /// <returns></returns>
        internal object[] GeraParametros(params object[] p_arrParams)
        {
            int intLen;
            object[] arrRet;

            if (p_arrParams == null)
            {
                return null;
            }
            else
                intLen = p_arrParams.Length;

            arrRet = new object[intLen];

            p_arrParams.CopyTo(arrRet, 0);


            for (int i = 0; i < arrRet.Length; i++)
            {
                arrRet[i] = VerificarValorParametroProcedure(arrRet[i]);
            }

            return arrRet;

        }

        ///// <summary>
        ///// O objetivo é checar parâmetros não inicializados que possam ser 
        ///// rejeitados pelos valores correspondentes em banco de dados
        ///// </summary>
        ///// <param name="p_objParametro"></param>
        ///// <returns>NULL se o parametro estiver não inicializado</returns>
        ///// <remarks>
        ///// Ex: no .NET um datetime não inicializado tem valor 1/1/1.
        ///// Em SQL, o valor minimo do datetime é 1/1/1753
        ///// </remarks>
        public object VerificarValorParametroProcedure(object p_objParametro)
        {
            object objRet = p_objParametro;
            if (p_objParametro is string)
            {
                if (((string)p_objParametro) == string.Empty || ((string)p_objParametro) == null)
                    objRet = null;
            }
            if (p_objParametro is DateTime)
            {
                if (((DateTime)p_objParametro) == DateTime.MinValue)
                    objRet = null;
            }
            if (p_objParametro is TimeSpan)
            {
                if (((TimeSpan)p_objParametro) == TimeSpan.MinValue)
                    objRet = new TimeSpan(0, 0, 0);
            }
            if (p_objParametro is Byte[])
            {
                if (((Byte[])p_objParametro) == null)
                    objRet = null;
            }
            if (p_objParametro is int)
            {
                if (((int)p_objParametro) == int.MinValue)
                    objRet = null;
            }
            if (p_objParametro is Int64)
            {
                if (((Int64)p_objParametro) == Int64.MinValue)
                    objRet = null;
            }
            if (p_objParametro is decimal)
            {
                if (((decimal)p_objParametro) == decimal.MinValue)
                    objRet = null;
            }
            return objRet;
        }

        /// <summary>
        /// Copia o retorno do parametro
        /// </summary>
        /// <param name="pDataColParams"></param>
        /// <param name="parrParams"></param>
        //private void CopiarParamsRetorno(SqlParameterCollection pDataColParams, out object[] parrParams)
        //{
        //    int intNumOutput = 0;


        //    //Conta os parametros de output
        //    for (int i = 0; i < pDataColParams.Count; i++)
        //    {
        //        IDataParameter objParam = (IDataParameter)pDataColParams[i];

        //        if (objParam.Direction == ParameterDirection.Output || objParam.Direction == ParameterDirection.InputOutput)
        //            intNumOutput++;
        //    }

        //    parrParams = new object[intNumOutput];
        //    intNumOutput = 0;

        //    for (int i = 0; i < pDataColParams.Count; i++)
        //    {
        //        IDataParameter objParam = (IDataParameter)pDataColParams[i];

        //        if (objParam.Direction == ParameterDirection.Output || objParam.Direction == ParameterDirection.InputOutput)
        //        {
        //            parrParams[intNumOutput] = objParam.Value;
        //            intNumOutput++;
        //        }
        //    }

        //}

        private void CopiarParamsRetorno(IDataParameterCollection pDataColParams, out object[] parrParams)
        {
            int intNumOutput = 0;


            //Conta os parametros de output
            for (int i = 0; i < pDataColParams.Count; i++)
            {
                IDataParameter objParam = (IDataParameter)pDataColParams[i];

                if (objParam.Direction == ParameterDirection.Output || objParam.Direction == ParameterDirection.InputOutput)
                    intNumOutput++;
            }

            parrParams = new object[intNumOutput];
            intNumOutput = 0;

            for (int i = 0; i < pDataColParams.Count; i++)
            {
                IDataParameter objParam = (IDataParameter)pDataColParams[i];

                if (objParam.Direction == ParameterDirection.Output || objParam.Direction == ParameterDirection.InputOutput)
                {
                    parrParams[intNumOutput] = objParam.Value;
                    intNumOutput++;
                }
            }

        }

        /// <summary>
        /// NÃO CHAMAR ESTA FUNÇÃO SE NÃO FOR PROCEDURE
        /// Monta a chamada do command para funcionar corretamente com o ODBC
        /// Esta função foi escrita porque no ODBC, chamando DB2/400, dá erro no command 
        /// ao executar procedure a menos que se especifique no CommandText o nome da lib 
        /// e a string para a chamada (com CALL e parâmetros)
        /// 
        /// </summary>
        /// <param name="pobjCommand">o command já preeenchido com parâmetros e o nome da proc com library name.</param>
        private void MontaCommandTextODBC(ref IDbCommand pobjCommand)
        {
            string strCmdText = "{CALL ";
            strCmdText += pobjCommand.CommandText;
            strCmdText += "(";


            for (int i = 0; i < pobjCommand.Parameters.Count; i++)
                strCmdText += "?,";

            //remover a última vírgula
            strCmdText = strCmdText.Remove(strCmdText.Length - 1, 1);

            strCmdText += ")}";

            pobjCommand.CommandText = strCmdText;

        }

        #endregion
    }
}
