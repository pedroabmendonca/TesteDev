<%@ Page Title="" Language="C#" MasterPageFile="~/SiteTeste.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="SiteTeste.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>
        <asp:Label runat="server" Text="<%$ Resources:Resource, Label_Bem_Vindo %>"></asp:Label>
    </h1>
</asp:Content>
