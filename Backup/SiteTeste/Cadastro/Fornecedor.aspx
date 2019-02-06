<%@ Page Title="Cadastro de Região do Fornecedor" Language="C#" AutoEventWireup="true"
    CodeBehind="Fornecedor.aspx.cs" Inherits="SiteTeste.Cadastro.Fornecedor" MasterPageFile="~/SiteTeste.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container1">
        <div class="divBack">
            <asp:Label ID="Titulo" runat="server" Text="<%$ Resources:Resource, Label_Cadastro_Regiao_Fornecedor %>" />
        </div>
        <div class="espacamento-vert">
            <asp:Label runat="server" ID="lblFornecedor" Text="<%$ Resources:Resource, Label_Fornecedor %>"></asp:Label>
            <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, Campo_Obrigatorio %>"
                CssClass="obrigatorio"></asp:Label>
            <asp:DropDownList runat="server" ID="ddlFornecedor" CssClass="select" ValidationGroup="Buscar">
            </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ID="ddlFornecedorRequired" ControlToValidate="ddlFornecedor"
                InitialValue="Selecione" ErrorMessage="Obrigatório" ForeColor="Red" CssClass="obrigatorio"
                ValidationGroup="Buscar">
            </asp:RequiredFieldValidator>
        </div>
        <div class="espacamento-vert">
            <asp:Button runat="server" ID="btnBuscar" Text="<%$ Resources:Resource, Botao_Buscar %>"
                OnClick="btnBuscar_click" ValidationGroup="Buscar" />
        </div>
        <div class="espacamento-vert">
            <asp:GridView runat="server" ID="gridFornecedor" AllowPaging="true" AutoGenerateColumns="false"
                OnRowDataBound="gridFornecedor_OnRowDataBound" OnPageIndexChanging="gridFornecedor_OnPageIndexChanging"
                EmptyDataText="<%$ Resources:Resource, Nenhum_registro_encontrado_GridView %>"
                DataKeyNames="IdRegiao, Ativo" CssClass="grid">
                <Columns>
                    <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="LabSituacao" runat="server" Text='<%# Eval("Ativo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:Literal runat="server" ID="LitIdLinha" Text='<%# Eval("IdLinha") %>'></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:Literal runat="server" ID="LitIdRegiao" Text='<%# Eval("IdRegiao") %>'></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="checkGrid" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$ Resources:Resource, Label_Tela_Cadastro_UF %>">
                        <ItemTemplate>
                            <asp:Literal runat="server" ID="LitUF" Text='<%# Eval("DescricaoUF") %>'></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$ Resources:Resource, Label_Tela_Cadastro_Regiao_Form %>">
                        <ItemTemplate>
                            <asp:Literal ID="LitRegiao" runat="server" Text='<%# Eval("Regiao") %>'></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <div class="espacamento-vert">
            <asp:Button runat="server" ID="btnGravar" Text="<%$ Resources:Resource, Botao_Gravar %>"
                OnClick="btnGravar_click" />
        </div>
    </div>
</asp:Content>
