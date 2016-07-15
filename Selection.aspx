<%@ Page Language="VB" MasterPageFile="~/Samples.master" AutoEventWireup="false" CodeFile="Selection.aspx.vb" Inherits="Selection" title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Sample" Runat="Server">
<table width="100%">
    <tr>
        <td align="center">
            <asp:Label ID="Label1" runat="server" Text="Map Mode:"></asp:Label>&nbsp; &nbsp;
            <asp:DropDownList AutoPostBack="true"
                ID="MapAction" runat="server">
                <asp:ListItem>Identify</asp:ListItem>
                <asp:ListItem>SelectExtent</asp:ListItem>
                <asp:ListItem>Query County by Name</asp:ListItem>
                <asp:ListItem>Query County by Multiple Name</asp:ListItem>
                <asp:ListItem>Buffer</asp:ListItem>
            </asp:DropDownList><br />
            <asp:Label ID="lblToolTip" runat="server" Text="Click the map to Zoom In."></asp:Label>
            <br />
            <asp:ImageButton Width="500" Height="400"
                ID="imgMap" runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" />
            <br />
            <br />
            <asp:Label ID="lblTip" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:Label ID="lblResult" runat="server"></asp:Label>
        </td>
    </tr>
</table>
</asp:Content>


