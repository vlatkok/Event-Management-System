<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GoogleCalendarPostRegistration.aspx.cs" Inherits="GoogleCalendarRegistration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <style type="text/css">
        .style1
        {
           color:White;
            background-color: #1E90FF;
        }
      
    </style>
</head>
<body class=style1>
    <form id="form1" runat="server">
    <div>
    
        <br />
        <asp:Label ID="LblMessage" runat="server"></asp:Label>
        <br />
        <br />
        <br />
        <br />
    </div>
        <table align=center runat=server style="height: 25px; width: 541px">       
       <tr><td align=justify>
        <asp:Button ID="btnGotoEvents" runat="server" onclick="btnGotoEvents_Click" 
            Text="Go to Events" Width="200px" />
            </td>
           <td align=justify>            
        <asp:Button ID="btnRevoke" runat="server" onclick="btnRevoke_Click" 
            Text="Try to Revoke  Google  Rights" Width="200px" />    
            </td>    
          </tr>
           
    </table>
    </form>
</body>
</html>
