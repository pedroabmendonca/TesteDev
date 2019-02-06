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
    public partial class Regiao : System.Web.UI.Page
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

        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //carrega a dropdownList
                    CarregarEstados(ddlEstados);
                    MontaEstruturaDataTable();
                    CarregarRegioes();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Action

        protected void btnInserir_click(object sender, EventArgs e)
        {
            try
            {
                string Mensagem = string.Empty;

                //caso os dois campos esteja preechido
                if (ValidaCampos())
                {
                    if (btnInserir.Text.Equals(App_GlobalResources.Resource.Botao_Inserir))
                    {
                        //inclui a regiao
                        Mensagem = Incluir();
                    }
                    else
                    {
                        Mensagem = Atualizar();
                    }

                    if (String.IsNullOrEmpty(Mensagem))
                    {
                        //limpa os campos
                        LimparCampos();

                        //carrega a lista de regiao
                        CarregarRegioes();

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alerta", "alert('" + App_GlobalResources.Resource.Salvo_Sucesso + "')", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alerta", "alert('" + Mensagem + "')", true);
                    }
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

        protected void gridRegiao_OnRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int Linha = Convert.ToInt32(e.CommandArgument);
                GridViewRow dr = gridRegiao.Rows[Linha];

                switch (e.CommandName)
                {
                    case "Editar":
                        {
                            //pega as informações do literais
                            Literal LitRegiao = (Literal)gridRegiao.Rows[Linha].FindControl("LitIdRegiao");

                            //seta nas variaveis
                            Int64 IdRegiao = Convert.ToInt64(LitRegiao.Text);

                            //preenche os campos para edição
                            PreencherCampoEdicao(IdRegiao);

                        } break;
                    case "AtivarInativar":
                        {
                            //pega as informações do literais
                            Literal LitRegiao = (Literal)gridRegiao.Rows[Linha].FindControl("LitIdRegiao");
                            Label LabSituacao = (Label)gridRegiao.Rows[Linha].FindControl("LabSituacao");

                            //seta nas variaveis
                            Int64 IdRegiao = Convert.ToInt64(LitRegiao.Text);
                            string Ativo = LabSituacao.Text;

                            //chama metodo de ativar ou desativar
                            AtivarDesativar(IdRegiao, Ativo);

                            //carrega a lista regiao
                            CarregarRegioes();

                            //mensagem de salvo 
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "AtivarDesativar", "alert('" + App_GlobalResources.Resource.Salvo_Sucesso + "')", true);
                        } break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void gridRegiao_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    #region Situacao

                    Label litAtivo = (Label)e.Row.FindControl("LabSituacao");
                    LinkButton lnkAtivarInativar = (LinkButton)e.Row.FindControl("LnkAtivarInativar");

                    if (litAtivo.Text.Equals("True"))
                    {
                        litAtivo.Text = App_GlobalResources.Resource.Situacao_Ativo;
                        litAtivo.ForeColor = System.Drawing.Color.Black;
                        lnkAtivarInativar.Text = App_GlobalResources.Resource.Link_Button_Desativar;
                        lnkAtivarInativar.Attributes.Add("onclick", "javascript:return Confirmar();");
                    }
                    else
                    {
                        litAtivo.Text = App_GlobalResources.Resource.Situacao_Inativo;
                        litAtivo.ForeColor = System.Drawing.Color.Red;
                        lnkAtivarInativar.Text = App_GlobalResources.Resource.Link_Button_Ativar;
                    }

                    #endregion
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void gridRegiao_OnPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gridRegiao.PageIndex = e.NewPageIndex;

                GridBind(gridRegiao, RegiaoDataTable);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #endregion

        #region Metodos

        public void CarregarEstados(DropDownList Dll)
        {
            try
            {
                EstadoBLL negocio = new EstadoBLL();
                List<EstadoDTO> lstEstado = negocio.Consultar(int.MinValue);

                //adiciona o valor padrao
                Dll.Items.Add(new ListItem("Selecione", "Selecione"));

                //adiciona os valores da lista
                foreach (var item in lstEstado)
                {
                    Dll.Items.Add(new ListItem(item.Descricao, item.Id.ToString()));
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
                int Uf = ddlEstados.SelectedItem.Equals("Selecione") ? int.MinValue : Convert.ToInt32(ddlEstados.SelectedValue);
                string Regiao = txtRegiao.Text.ToString();

                if (Uf.Equals(int.MinValue) || String.IsNullOrEmpty(Regiao))
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public string Incluir()
        {
            try
            {
                string Mensagem = string.Empty;
                int IdEstado = Convert.ToInt32(ddlEstados.SelectedValue);
                string Regiao = txtRegiao.Text.ToString();

                RegiaoBLL negocio = new RegiaoBLL();
                negocio.Incluir(IdEstado, Regiao, out Mensagem);

                return Mensagem;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string Atualizar()
        {
            try
            {
                string Mensagem = string.Empty;
                Int64 IdRegiao = (Int64)ViewState["IdRegiao"];
                int IdEstado = Convert.ToInt32(ddlEstados.SelectedValue);
                string Descricao = txtRegiao.Text;

                RegiaoBLL negocio = new RegiaoBLL();
                negocio.Editar(IdRegiao, IdEstado, Descricao, out Mensagem);

                return Mensagem;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void PreencherCampoEdicao(Int64 IdRegiao)
        {
            try
            {
                RegiaoBLL negocio = new RegiaoBLL();
                List<RegiaoDTO> lstRegiao = negocio.Consultar(IdRegiao);

                RegiaoDTO regiao = lstRegiao.FirstOrDefault();
                ddlEstados.SelectedValue = regiao.Estado.Id.ToString();
                txtRegiao.Text = regiao.Descricao;

                btnInserir.Text = "Editar";

                ViewState["IdRegiao"] = regiao.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void LimparCampos()
        {
            try
            {
                ddlEstados.SelectedIndex = 0;
                txtRegiao.Text = string.Empty;
                btnInserir.Text = "Inserir";

                ViewState["IdRegiao"] = null;
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
                DataColumn[] _col = new DataColumn[]
                {
                    new DataColumn("IdLinha", typeof(int)),
                    new DataColumn("IdRegiao", typeof(Int64)),
                    new DataColumn("DescricaoUF", typeof(string)),
                    new DataColumn("Regiao", typeof(string)),
                    new DataColumn("Ativo", typeof(bool))
                };

                RegiaoDataTable = new DataTable("ListaRegiao");
                RegiaoDataTable.Columns.AddRange(_col);
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
                GridBind(gridRegiao, RegiaoDataTable);
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
                            linha,
                            item.Id,
                            item.Estado.Descricao,
                            item.Descricao,
                            item.Ativo.ToString()
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

        public void AtivarDesativar(Int64 IdRegiao, string ativar)
        {
            try
            {
                if (!String.IsNullOrEmpty(ativar))
                {
                    bool AtivarDesativar = ativar.Equals("Ativo") ? true : false;

                    RegiaoBLL negocio = new RegiaoBLL();
                    negocio.AtivarDesativar(IdRegiao, AtivarDesativar);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }
}