using System;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Xml;
using System.Collections;
using System.Security.Principal;

namespace DAL
{
    /* Descricao:
     * Este arquivo contem as definicoes necessarias para a implementacao
     * da ConexaoBD, uma classe abstrata que provê a maioria dos métodos 
     * necessários para tratamento de conexão com BancoObject de Dados
     * 
     */
    /// <summary>
    /// O conjunto de providers ADO.NET  que são suportados por <see cref="ProviderFactory"/>.
    /// </summary>
    public enum ProviderType
    {
        /// <summary>
        /// The OLE DB (<see cref="System.Data.OleDb"/>) .NET data provider.
        /// </summary>
        OleDb = 0,
        /// <summary>
        /// The SQL Server (<see cref="System.Data.SqlClient"/>) .NET data provider.
        /// </summary>
        SqlClient,
        /// <summary>
        /// The ODBC (<see cref="System.Data.SqlClient"/>) .NET data provider.
        /// </summary>
        ODBC,
        /// <summary>
        /// MySql (<see cref="System.Data.SqlClient"/>) MySQL .NET data provider.
        /// </summary>
        MySqlClient,
        /// <summary>
        /// Oracle (<see cref="System.Data.SqlClient"/>) Oracle .NET data provider.
        /// </summary>
        OracleClient

    };

    /// <summary>
    /// Classe abstrata que provê a maioria dos metodos para tratamento 
    /// de conexão com BancoObject de Dados
    /// </summary>

    abstract public class ConexaoBD
    {

        private const string PROVIDERSTR_AS400 = "IBMDA400";
        private const string PROVIDERSTR_SQLOLEDB = "SQLOLEDB";
        private IDbTransaction mobjTransaction;
        private Hashtable mobjHashTableParameters; //Hashtable contendo  System.Data.IDataParameterCollection para cada proc


        #region public:
        protected string mstrLastError = "";
        protected ProviderType menuProvider;
        protected string mstrConnString;
        protected WindowsIdentity _identidadeLogin;//utilizado para armazenar a identidade do login para a conexaoBD, isto é utilizado na autenticação com o banco - Sidclei 29/10/2008
        WindowsImpersonationContext oUsuario; //utilizado para armazenar a identidade do login para a conexaoBD, isto é utilizado na autenticação com o banco - Sidclei 29/10/2008

        private ProviderFactory pf = null;

        protected IDbConnection mobjConexao;
        protected IDbConnection mobjConexaoParametros; //Conexao utilizada para o DeriveParameters
        protected bool m_blnTransacaoControlada = false; //indica se a transacao é controlada de fora
        /*
         * Nota: Esta conexao precisa ser utilizada porque não funciona o DeriveParameters quando 
         * uma conexão está com uma transação aberta. Parece que isto estará corrigido no Framework 2.0
         * */

        //public IDbCommand		cmd;
        protected string mstrOLEDBProvider;
        protected bool _impersonateDesligado; //informa se será efetuado o impersonate ou não - Sidclei 04/11/2008


        /// <summary>
        /// Indica se a transação é controlada de fora do objeto.
        /// </summary>
        /// <remarks>
        /// Nestes casos, os métodos BeginTrans, 
        /// Commit,Rollback e FecharConexao não têm efeito.
        /// </remarks>
        protected bool TransacaoControlada
        {
            get
            {
                return m_blnTransacaoControlada;
            }

            set
            {
                m_blnTransacaoControlada = value;
            }
        }

        protected bool EstaFechada
        {
            get
            {
                if (mobjConexao == null)
                    return true;
                else
                    return mobjConexao.State == ConnectionState.Closed;
            }
        }

        #endregion

        public string GetLastError() { return mstrLastError; }

        /// <summary>
        /// Retorna se a conexao atual tem transacao associada
        /// </summary>
        public bool TransacaoAberta
        {
            get
            {
                if (mobjTransaction == null)
                    return false;
                else
                {
                    if (mobjTransaction.Connection == null)
                        return false;
                    else
                        return true;

                }
            }
        }

        public ConexaoBD(ProviderType penuProvider)
        {
            menuProvider = penuProvider;

            mobjHashTableParameters = new Hashtable();

        }

        public ConexaoBD()
        {
            mobjHashTableParameters = new Hashtable();
        }

        //Late initialization
        public void Initialize(ProviderType penuProvider)
        {
            menuProvider = penuProvider;

        }

        ~ConexaoBD()
        {
        }

        public void BeginTrans()
        {
            //garantir que não há dirty reads
            if (!m_blnTransacaoControlada)
                mobjTransaction = mobjConexao.BeginTransaction(IsolationLevel.ReadCommitted);

        }

        public void BeginTransUncommitted()
        {
            //garantir que não há dirty reads
            if (!m_blnTransacaoControlada)
                mobjTransaction = mobjConexao.BeginTransaction(IsolationLevel.ReadUncommitted);

        }

        public void Commit()
        {
            if (!m_blnTransacaoControlada)
                mobjTransaction.Commit();
        }

        public void Rollback()
        {
            if (!m_blnTransacaoControlada)
                mobjTransaction.Rollback();
        }

        public void Abrir()
        {
            if (!_impersonateDesligado && _identidadeLogin != null)
            {
                oUsuario = _identidadeLogin.Impersonate(); //inicia um usuario para ser utilizado como autenticacao - Sidclei 29/10/2008
            }

            if (mstrConnString == null)
                throw new SystemException("String de conexão não foi especificada.");


            if (mobjConexao != null)
            {
                if (mobjConexao.State != ConnectionState.Closed)
                    throw new SystemException("A conexão já foi aberta.");
            }

            try
            {
                pf = new ProviderFactory(menuProvider);
                mobjConexao = pf.CreateConnection(mstrConnString);
                mobjConexaoParametros = pf.CreateConnection(mstrConnString);

                mobjConexao.Open();
                mobjConexaoParametros.Open();
            }
            catch
            {
                throw;
            }
        }

        public void Fechar()
        {
            if (!m_blnTransacaoControlada)
            {
                try
                {
                    mobjConexao.Close();
                    mobjConexaoParametros.Close();

                    if (!_impersonateDesligado && oUsuario != null)
                    {
                        oUsuario.Undo(); //encerra o objeto do usuario AD utilizado para conexão - Sidclei 29/10/2008
                    }
                }
                catch (SystemException e)
                {
                    throw e;
                }
            }
        }

        static public bool ProviderAS400(string pstrProvider)
        {
            if (pstrProvider != null)
                return pstrProvider.Substring(0, PROVIDERSTR_AS400.Length) == PROVIDERSTR_AS400;
            else
                return false;
        }

        static public bool ProviderSQLOLEDB(string pstrProvider)
        {
            if (pstrProvider != null)
                return pstrProvider.Substring(0, PROVIDERSTR_SQLOLEDB.Length) == PROVIDERSTR_SQLOLEDB;
            else
                return false;
        }

        public virtual bool LerProcedure(string pstrCommand, object[] parrParam, out IDataReader pobjDataReader)
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
        /// Inclusão de método que executa uma procedure recebendo como parametro um DataType do SQL Server.
        /// </summary>
        /// <param name="pstrCommand"></param>
        /// <param name="parrParam"></param>
        /// <param name="parametro"></param>
        /// <param name="tipoSqlServer"></param>
        /// <param name="blnRetorno"></param>
        /// <returns></returns>

        public virtual bool LerProcedure(string pstrCommand, object[] parrParam, string parametro, string tipoSqlServer, out DataSet objDataSet)
        {
            int intIndexParams = 0;

            // Para o provider nativo SQL, verificamos que o primeiro parâmetro (indice 0)
            //é o código de retorno da procedure
            //Para o provider OleDB não existe esse parametro 0. Testamos apenas com IBMDA400
            int intParamOleDB;

            bool blnRet = true;
            objDataSet = new DataSet();

            try
            {

                IDbCommand objCmd = pf.CreateCommand();

                objCmd.CommandText = pstrCommand;
                objCmd.CommandType = CommandType.StoredProcedure;

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
                        intParamOleDB = 0;
                        break;
                    default:
                        intParamOleDB = 0;
                        break;
                }

                if (parrParam != null)
                {
                    //for (intIndexParams = 0; intIndexParams < parrParam.Length; intIndexParams++)
                    //{
                    //    if (intIndexParams == 0)
                    //    {
                    //        var dbNewType = new SqlDbType();
                    //        dbNewType = SqlDbType.Structured;
                    //        IDataParameter iDataParam = pf.CreateDataParameter(parametro, tipoSqlServer, dbNewType);

                    //        iDataParam = (IDataParameter)objCmd.Parameters[intIndexParams + intParamOleDB];
                    //        if (parrParam[intIndexParams] != null)
                    //            iDataParam.Value = parrParam[intIndexParams];
                    //        else
                    //            iDataParam.Value = DBNull.Value;
                    //    }
                    //    else
                    //    {
                    //        IDataParameter iDataParam = pf.CreateDataParameter();

                    //        iDataParam = (IDataParameter)objCmd.Parameters[intIndexParams + intParamOleDB];
                    //        if (parrParam[intIndexParams] != null)
                    //            iDataParam.Value = parrParam[intIndexParams];
                    //        else
                    //            iDataParam.Value = DBNull.Value;
                    //    }
                    //}
                    for (intIndexParams = 0; intIndexParams < parrParam.Length; intIndexParams++)
                    {
                        IDataParameter iDataParam = pf.CreateDataParameter();

                        iDataParam = (IDataParameter)objCmd.Parameters[intIndexParams + intParamOleDB];
                        iDataParam.Value = parrParam[intIndexParams].ToString();
                    }
                }

                objCmd.Prepare();

                // Faço esse foreach, pois ao setar os parametros para o objCmd, o parametro de um tipo definido no SqlServer (DataType) fica com
                //o TypeName concatenado com DatabaseName.SchemaDoBanco.Tipo. Com isso é gerada uma exception conforme mensagem abaixo:
                //The incoming tabular data stream (TDS) remote procedure call (RPC) protocol stream is incorrect. 
                //Table-valued parameter 30 ("@DesignationList"), row 0, column 0: Data type 0xF3 (user-defined table type) has a non-zero 
                //length database name specified. Database name is not allowed with a table-valued parameter, only schema name and type name are valid.
                //Para resolver essa exception preciso que o TypeName do Parametro SqlParameter seja igual ao passado no CreateDataParameter.

                //foreach (SqlParameter i in objCmd.Parameters)
                //{
                //    if (i.ParameterName == parametro)
                //        i.TypeName = tipoSqlServer;
                //}

                IDataAdapter adapter = pf.CreateDataAdapter(objCmd);
                adapter.Fill(objDataSet);

                blnRet = true;
            }
            catch (SystemException e)
            {
                blnRet = false;
                throw e;
            }
            return blnRet;
        }

        public virtual bool LerProcedure(string pstrCommand, object[] parrParam, out XmlReader pobjDataReader)
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

                // A procedure deve ter como último param
                // o param de retorno de erro
                IDataParameter objParamErro = pf.CreateDataParameter();
                intErrorParamIndex = intIndexParams + intParamOleDB;
                objParamErro = (IDataParameter)objCmd.Parameters[intErrorParamIndex];
                objParamErro.Value = 0;
                if (menuProvider == ProviderType.ODBC)
                    MontaCommandTextODBC(ref objCmd);

                objCmd.Prepare();

                switch (menuProvider)
                {
                    //case ProviderType.OracleClient:
                    //    pobjDataReader = ((OracleCommand)objCmd).ExecuteXmlReader();
                    //    break;
                    case ProviderType.SqlClient:
                        pobjDataReader = ((SqlCommand)objCmd).ExecuteXmlReader();
                        break;
                    case ProviderType.ODBC:
                    case ProviderType.OleDb:
                    default:
                        throw new ApplicationException("Operação XML Reader não suportado pelo provider!");
                }

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

        public virtual bool LerProcedure(string pstrCommand, object[] parrParam, out DataSet pobjDataSet)
        {
            return this.LerProcedure(pstrCommand, parrParam, out pobjDataSet, false);
        }

        public virtual bool LerProcedure(string pstrCommand, object[] parrParam, out DataSet pobjDataSet, bool pblnXML)
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

            pobjDataSet = new DataSet();

            //XML só retorna com SQL server
            if (pblnXML && menuProvider != ProviderType.SqlClient)
                return false;

            try
            {

                IDbCommand objCmd = pf.CreateCommand();

                objCmd.CommandText = pstrCommand;
                objCmd.CommandType = CommandType.StoredProcedure;

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
                        iDataParam.Value = parrParam[intIndexParams].ToString();
                    }
                }


                // A procedure deve ter como último param
                // o param de retorno de erro
                IDataParameter objParamErro = pf.CreateDataParameter();
                intErrorParamIndex = intIndexParams + intParamOleDB;
                objParamErro = (IDataParameter)objCmd.Parameters[intErrorParamIndex];
                objParamErro.Value = 0;
                if (menuProvider == ProviderType.ODBC)
                    MontaCommandTextODBC(ref objCmd);

                objCmd.Prepare();

                if (pblnXML)
                {
                    //só funciona com o provider SQL
                    XmlReader objXmlReader;

                    switch (menuProvider)
                    {
                        //case ProviderType.OracleClient:
                        //    objXmlReader = ((OracleCommand)objCmd).ExecuteXmlReader();
                        //    break;
                        case ProviderType.SqlClient:
                            objXmlReader = ((SqlCommand)objCmd).ExecuteXmlReader();
                            break;
                        case ProviderType.ODBC:
                        case ProviderType.OleDb:
                        default:
                            throw new ApplicationException("Operação XML Reader não suportado pelo provider!");
                    }

                    pobjDataSet.ReadXml(objXmlReader, XmlReadMode.InferSchema);
                    objXmlReader.Close();

                }
                else
                {
                    IDataAdapter adapter = pf.CreateDataAdapter(objCmd);
                    adapter.Fill(pobjDataSet);
                }


                //testar retorno erro
                if (intErrorParamIndex > -1)
                {
                    objParamErroRetorno = (IDataParameter)objCmd.Parameters[intErrorParamIndex];
                    strValorParamErro = objParamErroRetorno.Value.ToString();
                    blnRet = (strValorParamErro == "0");
                }

                return blnRet;
            }
            catch
            {
                throw;
            }


        }

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

        public virtual bool EscreverProcedure(string pstrCommand, object[] parrParam)
        {
            object[] foo;
            return this.EscreverProcedure(pstrCommand, parrParam, out foo, 0);
        }

        public virtual bool EscreverProcedure(string pstrCommand, object[] parrParamIn, out object[] parrParamOut)
        {
            return this.EscreverProcedure(pstrCommand, parrParamIn, out parrParamOut, 0);
        }

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



        /// <summary>
        /// Utilizada internamente para carregar os parametros das procs chamadas.
        /// 
        /// Nas chamadas ODBC removemos o nome das libs. Só testamos ODBC para db2/400.
        /// Para o Deriveparameters não pode ter o nome da lib. Mas na hora de executar tem que ter.
        /// Por isso removemos e depois colocamos devolta o nome da lib.
        /// </summary>
        /// <param name="pobjCommand"></param>


        private IDataParameterCollection RetornaParametros(string pstrChave)
        {
            return (IDataParameterCollection)mobjHashTableParameters[pstrChave];
        }




        private IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
        {
            IDataParameter[] clonedParameters;


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
                //case ProviderType.OracleClient:
                //    clonedParameters = (IDataParameter[])new OracleParameter[originalParameters.Length];

                //    for (int i = 0, j = originalParameters.Length; i < j; i++)
                //    {
                //        clonedParameters[i] = (OracleParameter)((ICloneable)originalParameters[i]).Clone();
                //    }
                //    break;
                default:
                    throw (new SystemException("ProviderType não informado!"));

            }

            return (IDataParameter[])clonedParameters;
        }



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

                pobjCommand.Connection = mobjConexao;

                //atribui a transacao se for o caso
                if (mobjTransaction != null)
                    pobjCommand.Transaction = mobjTransaction;
            }
            else
            {
                //Se não existir, consulta

                //associar a conexão para listar os parâmetros da proc
                pobjCommand.Connection = mobjConexaoParametros;

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

                    //case ProviderType.OracleClient:
                    //    strCommandText = pobjCommand.CommandText;
                    //    pobjCommand.CommandText = strCommandText;
                    //    pobjCommand.CommandText = strCommandText.Substring(strCommandText.IndexOf(".") + 1);

                    //    OracleCommandBuilder.DeriveParameters((OracleCommand)pobjCommand);
                    //    pobjCommand.CommandText = strCommandText;
                    //    break;

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
                //else if (menuProvider == ProviderType.OracleClient)
                //{
                //    foreach (OracleParameter parameter in pobjCommand.Parameters)
                //    {
                //        string nomeParametro = parameter.ParameterName;
                //        parameter.ParameterName = string.Format(":{0}", nomeParametro);
                //    }
                //}

                //devolve o cammand para a conexão original
                pobjCommand.Connection = mobjConexao;

                //atribui a transacao se for o caso
                if (mobjTransaction != null)
                    pobjCommand.Transaction = mobjTransaction;
            }
        }

        public virtual bool ExecutaComando(string pstrCommand, out IDataReader pobjDataReader)
        {
            bool blnRet = true;

            try
            {

                IDbCommand objCmd = pf.CreateCommand();

                objCmd.CommandText = pstrCommand;
                objCmd.CommandType = CommandType.Text;
                objCmd.Connection = mobjConexao;

                pobjDataReader = objCmd.ExecuteReader();

                return blnRet;
            }
            catch (SystemException e)
            {
                throw e;
            }

        }

    }

    #region Classe Factory utilizada para prover abstração do Data Provider
    /// <summary>
    /// A classe <b>ProviderFactory</b> apresenta uma interface para os providers ADO.NET através de Factory Methods que retornam
    /// os tipos <see cref="System.Data"/> adequados.
    /// </summary>
    /// <remarks>
    /// Este código foi inspirado por "Design an Effective Data-Access Architecture" de Dan Fox (.netmagazine, vol. 2, no. 7) e no artigo
    /// "A Generic Data Access Component using Factory Pattern por Michael Bouck (C# Corner 07/24/2002)"
    /// 
    /// Adaptado em 23.03.2007 por Sidclei, para servir tambem Oracle
    /// </remarks>
    public class ProviderFactory
    {
        #region private variables
        private static Type[] _connectionTypes = new Type[] { typeof(OleDbConnection), typeof(SqlConnection), typeof(OdbcConnection) };
        private static Type[] _commandTypes = new Type[] { typeof(OleDbCommand), typeof(SqlCommand), typeof(OdbcCommand) };
        private static Type[] _dataAdapterTypes = new Type[] { typeof(OleDbDataAdapter), typeof(SqlDataAdapter), typeof(OdbcDataAdapter) };
        private static Type[] _dataParameterTypes = new Type[] { typeof(OleDbParameter), typeof(SqlParameter), typeof(OdbcParameter) };
        private static Type[] _dataReaderTypes = new Type[] { typeof(OleDbDataReader), typeof(SqlDataReader), typeof(OdbcDataReader) };
        private ProviderType _provider;
        #endregion

        #region ctors
        internal ProviderFactory() { } // force user to specify provider
        internal ProviderFactory(ProviderType provider)
        {
            _provider = provider;
        }
        #endregion

        #region Provider property
        internal ProviderType Provider
        {
            get
            {
                return _provider;
            }
            set
            {
                _provider = value;
            }
        }
        #endregion

        #region IDbConnection methods
        public IDbConnection CreateConnection()
        {
            IDbConnection conn = null;

            try
            {
                conn = (IDbConnection)Activator.CreateInstance(_connectionTypes[(int)_provider]);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return conn;
        }
        public IDbConnection CreateConnection(string connectionString)
        {
            IDbConnection conn = null;
            object[] args = { connectionString };

            try
            {
                conn = (IDbConnection)Activator.CreateInstance(_connectionTypes[(int)_provider], args);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return conn;
        }
        #endregion

        #region IDbCommand methods
        public IDbCommand CreateCommand()
        {
            IDbCommand cmd = null;

            try
            {
                cmd = (IDbCommand)Activator.CreateInstance(_commandTypes[(int)_provider]);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return cmd;
        }
        public IDbCommand CreateCommand(string cmdText)
        {
            IDbCommand cmd = null;
            object[] args = { cmdText };

            try
            {
                cmd = (IDbCommand)Activator.CreateInstance(_commandTypes[(int)_provider], args);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return cmd;
        }
        public IDbCommand CreateCommand(string cmdText, IDbConnection connection)
        {
            IDbCommand cmd = null;
            object[] args = { cmdText, connection };

            try
            {
                cmd = (IDbCommand)Activator.CreateInstance(_commandTypes[(int)_provider], args);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return cmd;
        }
        public IDbCommand CreateCommand(string cmdText, IDbConnection connection, IDbTransaction transaction)
        {
            IDbCommand cmd = null;
            object[] args = { cmdText, connection, transaction };

            try
            {
                cmd = (IDbCommand)Activator.CreateInstance(_commandTypes[(int)_provider], args);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return cmd;
        }
        #endregion

        #region IDbDataAdapter methods
        public IDbDataAdapter CreateDataAdapter()
        {
            IDbDataAdapter da = null;

            try
            {
                da = (IDbDataAdapter)Activator.CreateInstance(_dataAdapterTypes[(int)_provider]);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return da;
        }
        public IDbDataAdapter CreateDataAdapter(IDbCommand selectCommand)
        {
            IDbDataAdapter da = null;
            object[] args = { selectCommand };

            try
            {
                da = (IDbDataAdapter)Activator.CreateInstance(_dataAdapterTypes[(int)_provider], args);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return da;
        }
        public IDbDataAdapter CreateDataAdapter(string selectCommandText, IDbConnection selectConnection)
        {
            IDbDataAdapter da = null;
            object[] args = { selectCommandText, selectConnection };

            try
            {
                da = (IDbDataAdapter)Activator.CreateInstance(_dataAdapterTypes[(int)_provider], args);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return da;
        }
        public IDbDataAdapter CreateDataAdapter(string selectCommandText, string selectConnectionString)
        {
            IDbDataAdapter da = null;
            object[] args = { selectCommandText, selectConnectionString };

            try
            {
                da = (IDbDataAdapter)Activator.CreateInstance(_dataAdapterTypes[(int)_provider], args);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return da;
        }
        #endregion

        #region IDbDataParameter methods
        public IDbDataParameter CreateDataParameter()
        {
            IDbDataParameter param = null;

            try
            {
                param = (IDbDataParameter)Activator.CreateInstance(_dataParameterTypes[(int)_provider]);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return param;
        }
        public IDbDataParameter CreateDataParameter(string parameterName, object value)
        {
            IDbDataParameter param = null;
            object[] args = { parameterName, value };

            try
            {
                param = (IDbDataParameter)Activator.CreateInstance(_dataParameterTypes[(int)_provider], args);
            }
            catch (TargetInvocationException e)
            {
                throw new SystemException(e.InnerException.Message, e.InnerException);
            }

            return param;
        }
        public IDbDataParameter CreateDataParameter(string parameterName, DbType dataType)
        {
            IDbDataParameter param = CreateDataParameter();

            if (param != null)
            {
                param.ParameterName = parameterName;
                param.DbType = dataType;
            }

            return param;
        }
        public IDbDataParameter CreateDataParameter(string parameterName, DbType dataType, int size)
        {
            IDbDataParameter param = CreateDataParameter();

            if (param != null)
            {
                param.ParameterName = parameterName;
                param.DbType = dataType;
                param.Size = size;
            }

            return param;
        }
        public IDbDataParameter CreateDataParameter(string parameterName, string strTypeName, SqlDbType dataType)
        {
            var param = new SqlParameter();

            if (param != null)
            {
                param.ParameterName = parameterName;
                param.SqlDbType = dataType;
                param.TypeName = strTypeName;
                param.UdtTypeName = strTypeName;
            }

            return param;
        }
        public IDbDataParameter CreateDataParameter(string parameterName, DbType dataType, int size, string sourceColumn)
        {
            IDbDataParameter param = CreateDataParameter();

            if (param != null)
            {
                param.ParameterName = parameterName;
                param.DbType = dataType;
                param.Size = size;
                param.SourceColumn = sourceColumn;
            }

            return param;
        }
        #endregion
    }

    #endregion
}

