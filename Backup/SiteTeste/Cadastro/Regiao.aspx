<%@ Page Title="Cadastro de Região" Language="C#" MasterPageFile="~/SiteTeste.Master" AutoEventWireup="true"
    CodeBehind="Regiao.aspx.cs" Inherits="SiteTeste.Cadastro.Regiao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container1">
        <div class="divBack">
            <asp:Label ID="Titulo" runat="server" Text="<%$ Resources:Resource, Label_Tela_Cadastro_Regiao %>" />
        </div>
        <div class="espacamento-vert">
            <asp:Label runat="server" ID="lblUf" Text="<%$ Resources:Resource, Label_Tela_Cadastro_UF %>" CssClass="UF"></asp:Label>
            <asp:Label ID="Label2" runat="server" Text="<%$ Resources:Resource, Campo_Obrigatorio %>" CssClass="obrigatorio"></asp:Label>
            <asp:DropDownList runat="server" ID="ddlEstados" CssClass="select">
            </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ID="UfRequired" ControlToValidate="ddlEstados"
                InitialValue="Selecione" ErrorMessage="Obrigatório" ForeColor="Red" ValidationGroup="FormularioRegiao"
                CssClass="obrigatorio">
            </asp:RequiredFieldValidator>
        </div>
        <div class="espacamento-vert">
            <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, Label_Tela_Cadastro_Regiao_Form %>"></asp:Label>
            <asp:Label ID="Label3" runat="server" Text="<%$ Resources:Resource, Campo_Obrigatorio %>" CssClass="obrigatorio"></asp:Label>
            <asp:TextBox ID="txtRegiao" runat="server" MaxLength="50" CssClass="input"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="txtRegiaoRequired" ControlToValidate="txtRegiao"
                ErrorMessage="Obrigatório" ForeColor="Red" ValidationGroup="FormularioRegiao"
                CssClass="obrigatorio">
            </asp:RequiredFieldValidator>
        </div>
        <div class="espacamento-vert">
            <asp:Button runat="server" ID="btnInserir" Text="<%$ Resources:Resource, Botao_Inserir %>"
                OnClick="btnInserir_click" ValidationGroup="FormularioRegiao" />
        </div>
        <div class="espacamento-vert">
            <asp:GridView runat="server" ID="gridRegiao" AllowPaging="true" AutoGenerateColumns="false"
                OnRowDataBound="gridRegiao_OnRowDataBound" OnPageIndexChanging="gridRegiao_OnPageIndexChanging"
                OnRowCommand="gridRegiao_OnRowCommand" EmptyDataText="<%$ Resources:Resource, Nenhum_registro_encontrado_GridView %>"
                DataKeyNames="IdRegiao, Ativo" CssClass="grid">
                <Columns>
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
                    <asp:TemplateField HeaderText="<%$ Resources:Resource, Label_Situacao %>">
                        <ItemTemplate>
                            <asp:Label ID="LabSituacao" runat="server" Text='<%# Eval("Ativo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$ Resources:Resource, Label_Acoes %>">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkEditar" Text="<%$ Resources:Resource, Botao_Editar %>"
                                CommandName="Editar" CommandArgument='<%# Eval("IdLinha") %>' />
                            <asp:LinkButton runat="server" ID="LnkAtivarInativar" CommandName="AtivarInativar"
                                CommandArgument='<%# Eval("IdLinha") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
