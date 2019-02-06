using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DTO;
using System.Data;

namespace SiteTeste.Cadastro
{
    public partial class Fornecedor : System.Web.UI.Page
    {
        #region Propriedades

        private DataTable _RegiaoDataTable;
        public DataTable RegiaoDataTable
        {
            get
            {
                if (_RegiaoDataTable == null)
                {
                    _RegiaoDataTable = new DataTable();
                    MontaEstruturaDataTable();
                }

                return _RegiaoDataTable;
            }
            set
            {
                _RegiaoDataTable = value;
            }
        }

        private DataTable _FornecedorRegiao;
        public DataTable FornecedorRegiaoObj
        {
            get
            {
                if (_FornecedorRegiao == null)
                {
                    _FornecedorRegiao = new DataTable();
                    MontaEstruturaDataTable();
                }

                return _FornecedorRegiao;
            }
            set
            {
                _FornecedorRegiao = value;
            }
        }

        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    MontaEstruturaDataTable();
                    CarregarRegioes();
                    CarregarFornecedores(ddlFornecedor);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Action

        protected void btnBuscar_click(object sender, EventArgs e)
        {
            try
            {
                if (!ddlFornecedor.SelectedValue.Equals("Selecione"))
                {
                    //carrega novamente as informações
                    int IdFornecedor = Convert.ToInt32(ddlFornecedor.SelectedValue);
                    FornecedorDTO forn = FornecedorRegiao(IdFornecedor);
                    CheckRegiaoGrid(forn);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "btnBuscar", "alert('" + App_GlobalResources.Resource.Validacao_DropDownList_Fornecedor + "')", true);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void btnGravar_click(object sender, EventArgs e)
        {
            try
            {
                string Mensagem = string.Empty;

                //caso os dois campos esteja preechido
                if (ValidaCampos())
                {
                    Int64 fornecedor = ddlFornecedor.SelectedItem.Equals("Selecione") ? Int64.MinValue : Convert.ToInt64(ddlFornecedor.SelectedValue);

                    //salvar as informações
                    Salvar(fornecedor, FornecedorRegiaoObj);

                    //limpa os campos
                    LimparCampos();

                    //carrega a lista de regiao
                    CarregarRegioes();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alerta", "alert('" + App_GlobalResources.Resource.Salvo_Sucesso + "')", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alerta", "alert('" + String.Format(App_GlobalResources.Resource.Erro, App_GlobalResources.Resource.Erro_Formulario) + "')", true);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region GridView

        protected void gridFornecedor_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    #region Situacao

                    int IdLinha;

                    Literal Linha = (Literal)e.Row.FindControl("LitIdLinha");
                    Label litAtivo = (Label)e.Row.FindControl("LabSituacao");
                    CheckBox lnkAtivarInativar = (CheckBox)e.Row.FindControl("checkGrid");

                    IdLinha = Convert.ToInt32(Linha.Text);

                    if (!litAtivo.Text.Equals("True"))
                    {
                        e.Row.BackColor = System.Drawing.Color.LightSalmon;
                    }

                    #endregion
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void gridFornecedor_OnPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gridFornecedor.PageIndex = e.NewPageIndex;

                GridBind(gridFornecedor, RegiaoDataTable);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #endregion

        #region Metodos

        public void CarregarFornecedores(DropDownList Dll)
        {
            try
            {
                FornecedorBLL negocio = new FornecedorBLL();
                List<FornecedorDTO> lstFornecedor = negocio.Consultar(Int64.MinValue);

                //adiciona o valor padrao
                Dll.Items.Add(new ListItem("Selecione", "Selecione"));

                //adiciona os valores da lista
                foreach (var item in lstFornecedor)
                {
                    Dll.Items.Add(new ListItem(item.Nome, item.Id.ToString()));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public FornecedorDTO FornecedorRegiao(int IdFornecedor)
        {
            try
            {
                FornecedorBLL negocio = new FornecedorBLL();
                return negocio.ConsultarFornecedorRegiao(IdFornecedor);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void CheckRegiaoGrid(FornecedorDTO forn)
        {
            try
            {
                foreach (GridViewRow item in gridFornecedor.Rows)
                {
                    Literal LitRegiao = (Literal)item.FindControl("LitIdRegiao");
                    int regiao = Convert.ToInt32(LitRegiao.Text);

                    //pega o checkbox da linha
                    CheckBox checkContem = (CheckBox)item.FindControl("checkGrid");

                    //verifica se contem a regiao e marca o checkbox
                    RegiaoDTO reg = forn.LstRegiao.Where(x => x.Id == regiao).FirstOrDefault();

                    if (reg != null)
                    {
                        checkContem.Checked = true;
                    }
                    else
                    {
                        checkContem.Checked = false;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool ValidaCampos()
        {
            try
            {
                //limpa a lista
                FornecedorRegiaoObj.Clear();

                int count = 0;
                Int64 fornecedor = ddlFornecedor.SelectedItem.Equals("Selecione") ? Int64.MinValue : Convert.ToInt64(ddlFornecedor.SelectedValue);
                bool[] arrayChecks = new bool[gridFornecedor.Rows.Count];

                if (fornecedor.Equals(int.MinValue))
                    return false;


                //verifica se preencheu algum checkbox
                foreach (GridViewRow item in gridFornecedor.Rows)
                {
                    Literal Regiao = (Literal)item.FindControl("LitIdRegiao");
                    Int64 IdRegiao = Convert.ToInt64(Regiao.Text);

                    //insere a propriedade check no array
                    CheckBox Check = (CheckBox)item.FindControl("checkGrid");
                    arrayChecks[count] = Check.Checked;

                    //adiciona somente que está checado
                    if (Check.Checked)
                    {
                        //adiciona na datatable
                        FornecedorRegiaoObj.Rows.Add(IdRegiao, fornecedor);
                    }

                    //adiciona mais um
                    count += 1;
                }

                //verifica se ao menos uma está preechida
                if (arrayChecks.Contains(true))
                    return true;
                else
                    return false;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Salvar(Int64 idFornecedor, DataTable FornecedorRegioes)
        {
            try
            {
                FornecedorBLL negocio = new FornecedorBLL();
                negocio.Editar(idFornecedor, FornecedorRegioes);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public void PreencherCampoEdicao(Int64 IdRegiao)
        //{
        //    try
        //    {
        //        //RegiaoBLL negocio = new RegiaoBLL();
        //        //List<RegiaoDTO> lstRegiao = negocio.Consultar(IdRegiao);

        //        //RegiaoDTO regiao = lstRegiao.FirstOrDefault();
        //        //ddlEstados.SelectedValue = regiao.Estado.Id.ToString();
        //        //txtRegiao.Text = regiao.Descricao;

        //        //btnInserir.Text = "Editar";

        //        //ViewState["IdRegiao"] = regiao.Id;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public void LimparCampos()
        {
            try
            {
                ddlFornecedor.SelectedIndex = 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void MontaEstruturaDataTable()
        {
            try
            {
                #region Regiao

                DataColumn[] _col = new DataColumn[]
                {
                    new DataColumn("Ativo", typeof(bool)),
                    new DataColumn("IdLinha", typeof(int)),
                    new DataColumn("IdRegiao", typeof(Int64)),
                    new DataColumn("DescricaoUF", typeof(string)),
                    new DataColumn("Regiao", typeof(string))                    
                };

                RegiaoDataTable = new DataTable("ListaRegiao");
                RegiaoDataTable.Columns.AddRange(_col);

                #endregion

                #region FornecedorRegiao

                DataColumn[] _col_2 = new DataColumn[]
                {                    
                    new DataColumn("IdRegiao", typeof(Int64)),
                    new DataColumn("IdFornecedor", typeof(Int64))                                      
                };

                FornecedorRegiaoObj = new DataTable("ListaFornecedorRegiao");
                FornecedorRegiaoObj.Columns.AddRange(_col_2);

                #endregion

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void CarregarRegioes()
        {
            try
            {
                RegiaoBLL negocio = new RegiaoBLL();
                List<RegiaoDTO> lstRegiao = negocio.Consultar(Int64.MinValue);

                PreencheListaDataTable(lstRegiao);
                GridBind(gridFornecedor, RegiaoDataTable);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void PreencheListaDataTable(List<RegiaoDTO> listaRegioes)
        {
            try
            {
                int linha = 0;
                //limpa a lista
                RegiaoDataTable.Rows.Clear();

                foreach (var item in listaRegioes)
                {
                    //adiciona a linha 
                    RegiaoDataTable.Rows.Add(
                            item.Ativo,
                            linha,
                            item.Id,
                            item.Estado.Descricao,
                            item.Descricao
                        );

                    linha += 1;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void GridBind(GridView Grid, object lst)
        {
            try
            {
                Grid.DataSource = lst;
                Grid.DataBind();

                //deixa a grid visivel
                Grid.Visible = true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }
}