<%@ Page Language="VB" MasterPageFile="~/Samples.master" AutoEventWireup="false" CodeFile="BasicMap.aspx.vb" Inherits="BasicMap" title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Sample" Runat="Server">
<table width="100%">
    <tr>
        <td align="center">
            <asp:Label ID="Label1" runat="server" Text="Map Mode:"></asp:Label>&nbsp; &nbsp;
            <asp:DropDownList AutoPostBack="true"
                ID="MapAction" runat="server">
                <asp:ListItem>ZoomIn</asp:ListItem>
                <asp:ListItem>ZoomOut</asp:ListItem>
                <asp:ListItem>Pan</asp:ListItem>
                <asp:ListItem>ZoomBox</asp:ListItem>
                <asp:ListItem>ZoomFullExtent</asp:ListItem>
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
</table>
    

</asp:Content>

