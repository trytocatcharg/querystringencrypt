<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Page1.aspx.cs" Inherits="Page1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1
        {
            width: 55%;
            height: 96px;
        }
        .auto-style2
        {
            height: 33px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    
                <table class="auto-style1">
                    <tr>
                        <td>
        <asp:Label ID="Label1" runat="server" Text="Primer Parametro"></asp:Label>
                        </td>
                        <td><asp:TextBox ID="txtValor1" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
        <asp:Label ID="Label2" runat="server" Text="Primer Parametro"></asp:Label>
                        </td>
                        <td><asp:TextBox ID="txtValor2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style2" colspan="2">  
                             <div style="text-align:center">
                            <asp:Button ID="Button1" runat="server" Text="Ir a Página 2" OnClick="Button1_Click" /> 
                                 </div>

                        </td>
                    </tr>
                </table>
    </form>
</body>
</html>
