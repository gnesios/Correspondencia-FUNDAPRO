<%@ Assembly Name="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Assembly Name="FPC_ECBImprimirHojaDRuta, Version=1.0.0.0, Culture=neutral, PublicKeyToken=fa52ae7fed3d803b" %>
<%@ Page Language="C#" MasterPageFile="~/_layouts/application.master" CodeBehind="ImprimirHojaDRuta.aspx.cs" Inherits="FPC_ECBImprimirHojaDRuta.ImprimirHojaDRuta" %>

<asp:Content ID="Main" contentplaceholderid="PlaceHolderMain" runat="server">
  <asp:Literal ID="ltlResultados" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="PageTitle" runat="server" contentplaceholderid="PlaceHolderPageTitle" >
  Imprimir Hoja de Ruta
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" runat="server" contentplaceholderid="PlaceHolderPageTitleInTitleArea" >
  Acci&oacute;n: Imprimir Hoja de Ruta
</asp:Content>