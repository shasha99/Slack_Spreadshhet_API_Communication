<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebApplication1._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<div align="center">
   <table border="solid black" style="width:500px;height:360px" class="main_table">
        <tr style="height:120px">
            <td style="width:120px">
                <label>Daily Story</label>
            </td>
            <td>
                <asp:TextBox ID="uStory" runat="server" Height="108px" Width="363px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label>Story Owner</label>
            </td>
            <td>
                <asp:TextBox ID="owner" runat="server" Height="22px" Width="363px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label>Client ID</label>
            </td>
            <td>
                <asp:TextBox ID="clientID" runat="server" Height="22px" Width="363px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label>Client Secret</label>
            </td>
            <td>
                <asp:TextBox ID="clientSecret" runat="server" Height="22px" Width="363px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label>Spreadsheet Title</label>
            </td>
            <td>
                <asp:TextBox ID="sID" runat="server" Height="22px" Width="363px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label>Webhook URL</label>
            </td>
            <td>
                <asp:TextBox ID="sToken" runat="server" Height="22px" Width="363px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label>Slack Channel</label>
            </td>
            <td>
                <asp:TextBox ID="cID" runat="server" Height="22px" Width="363px"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>
                <label>Access Token</label>
            </td>
            <td>
                <asp:TextBox ID="aToken" runat="server" Height="22px" Width="363px"></asp:TextBox>
            </td>
        </tr>

   </table> <br/><br/>
   <div align="center">
        <asp:Button ID="Button2" runat="server" onclick="Button2_Click" Text="Submit" />
        <asp:Button ID="Button1" runat="server" Text="Submit" onclick="Button1_Click" />   
   </div>
   <br/><asp:Label ID="msg" runat="server" Text=""></asp:Label><br/>
</div>

</asp:Content>
