<%@ Page  Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>


 



<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" >
    
    <div>
 
    
    </div>
    <table align=center><tr>     

   <!-- <td align="center">
        <asp:Image ID="Image1" runat="server" Height="332px" ImageUrl="~/MyPlanner.jpg" 
            Width="577px" />
    
    </td> -->
    
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
     </tr> 
     </table>
     <table align=center><tr><td>
    <asp:Panel ID="panelLogin" runat="server" Height="111px" >
        &nbsp;Login:<br /> UserName:&nbsp;&nbsp;
        <asp:TextBox ID="txt_UserName" runat="server"></asp:TextBox>
        &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
            ControlToValidate="txt_UserName" ErrorMessage="RequiredField" 
            ForeColor="#CC0000" validationgroup="LoginValGr"></asp:RequiredFieldValidator>
        <asp:Label ID="lblMsgLogin" runat="server"></asp:Label>
        <br />
        Password:&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
            ControlToValidate="txtPassword" ErrorMessage="RequiredField" 
            ForeColor="#CC0000" validationgroup="LoginValGr"></asp:RequiredFieldValidator>
        <br />
        <asp:Button ID="btnLogin" runat="server" onclick="btnLogin_Click" 
            Text="Login" validationgroup="LoginValGr" />
    </asp:Panel>
    <p>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblMessageStatus" 
            runat="server"></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    </p>
    </td>
    <td>
    <asp:Panel ID="panelRegistration" runat="server">
        <p>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
    Registration to MyPlanner:</p>
        <p>
            &nbsp;Your UserName:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:TextBox 
        ID="regUserName" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="regUserName" 
        ErrorMessage="RequiredField" ForeColor="#CC0000" validationgroup="RegisterValGr"></asp:RequiredFieldValidator>
            <asp:Label ID="lblregMsg" runat="server"></asp:Label>
        </p>
          <p>
            &nbsp;Name:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox 
        ID="txtName" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtName" 
        ErrorMessage="RequiredField" ForeColor="#CC0000" validationgroup="RegisterValGr"></asp:RequiredFieldValidator>
            <asp:Label ID="labelname" runat="server"></asp:Label>
        </p>
           <p>
            &nbsp;SurName:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox 
        ID="txtsurname" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtsurname" 
        ErrorMessage="RequiredField" ForeColor="#CC0000" validationgroup="RegisterValGr"></asp:RequiredFieldValidator>
            <asp:Label ID="labelsurname" runat="server"></asp:Label>
        </p>
        <p>
            Password:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="regPass1" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                ControlToValidate="regPass1" ErrorMessage="RequiredField" 
        ForeColor="#CC0000" validationgroup="RegisterValGr"></asp:RequiredFieldValidator>
        </p>
        <p>
            Re-enter Password:&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="regPass2" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                ControlToValidate="regPass2" ErrorMessage="RequiredField" 
        ForeColor="#CC0000" validationgroup="RegisterValGr"></asp:RequiredFieldValidator>
            &nbsp;
            <asp:CompareValidator ID="CompareValidator1" runat="server" 
                ControlToCompare="regPass1" ControlToValidate="regPass2" 
        ErrorMessage="The both paswords must be same" ForeColor="#CC0000" validationgroup="RegisterValGr"></asp:CompareValidator>
        </p>
        <p>
            <asp:Button ID="btnRegister" runat="server" 
        onclick="btnRegister_Click" Text="Register" validationgroup="RegisterValGr" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </p>
    </asp:Panel></td>
   </tr> </table>
 
    
   </asp:Content>
