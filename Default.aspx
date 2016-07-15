<%@ Page Language="VB" MasterPageFile="~/Samples.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Sample" Runat="Server">

This ASP.NET 2.0 website is intended to provide a growing list of samples implementing the MapScript C# API.<br />
<br />
    <asp:TreeView ID="TreeView1" runat="server" DataSourceID="SiteMapDataSource1">
    </asp:TreeView>
    <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
</asp:Content>

