using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Google.AccessControl;
using Google.GData;
using Google.GData.Calendar;
using Google.GData.AccessControl;
using Google.GData.Client;
using System.Data.SqlClient;
using System.Data.Sql;
public partial class _Default : System.Web.UI.Page
{
    XmlDocument XmlDoc = new XmlDocument();

    static string UserID = "UserID";
    static string UserName = "UserName";
    static string Password = "Password";
  // static string AccessToken = "AccessToken";
  // static string RefreshToken = "RefreshToken";
    static string labelMessage;

    // glavna funkcija 
    protected void Page_Load(object sender, EventArgs e)
    { // proverka za najaven i registriran korisnik (sesija)
        if (!IsPostBack)
        {
           
            if (Session["UserName"] != null)
            {

                Response.Redirect("GoogleCalendarRegistration.aspx");

            }

            else
            {


                if (Session["Registration"] != null)
                {
                    lblMessageStatus.Text = (String)Session["Registration"];
                    panelRegistration.Visible = false;
                    Session["Registration"] = null;
                }


            }

        }

        
            

    }

    // funkcija za registracija na korisnikot
    protected void btnRegister_Click(object sender, EventArgs e)
    {
        lblregMsg.Text = "";
          bool cont = true;
        SqlConnection konekcija = new SqlConnection();
        konekcija.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;
      
        SqlCommand komanda = new SqlCommand();
        komanda.Connection = konekcija;
        komanda.CommandText = "SELECT UserName FROM Users";

        try
        {
            konekcija.Open();
            SqlDataReader citac = komanda.ExecuteReader();
            while (citac.Read())
            {


                if (citac["UserName"].ToString().ToLower() == regUserName.Text.ToString().ToLower())
                {


                    cont = false;
                    lblregMsg.Text = "This UserName is not available. Please choose another UserName";
                    regPass1.Text = "";
                    regPass2.Text = "";
                    break;
                }
                        
            }
            citac.Close();
        }
        catch (Exception ex) { lblregMsg.Text = ex.ToString(); }
        finally { konekcija.Close(); }
      
    
     

        if (cont)
        {
            //zapisi vo baza   
            komanda.CommandText = "INSERT INTO [Users] (UserName, Password,AccessToken,RefreshToken,Name,Surname) VALUES(@username ,@password,@access,@refresh,@name,@surname)";
            komanda.Parameters.Add("@username", regUserName.Text.ToString().Trim());
            komanda.Parameters.Add("@password", regPass1.Text.ToString().Trim());
            komanda.Parameters.Add("@access", "");
            komanda.Parameters.Add("@refresh", "");
            komanda.Parameters.Add("@name", txtName.Text.ToString().Trim());
            komanda.Parameters.Add("@surname", txtsurname.Text.ToString().Trim());
            try
            {
                konekcija.Open();
                komanda.ExecuteNonQuery();
            }
            catch (Exception ex) { lblregMsg.Text = ex.ToString(); }
            finally { konekcija.Close();
              ViewState["Registration"]=
                labelMessage="Successful registration. Please Login !!!";
              Session["Registration"] = labelMessage;
                Session["UserName"] = regUserName.Text.ToString().Trim(); }

            Response.Redirect("Default.aspx");
        }
    }

    // funkcija za najavuvanje na korisnikot
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        lblMessageStatus.Text = "";
        SqlConnection konekcija = new SqlConnection();
        konekcija.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;
       
        SqlCommand komanda = new SqlCommand();
        komanda.Connection = konekcija;
        komanda.CommandText = "SELECT * FROM Users";

        try
        {
            konekcija.Open();
            SqlDataReader citac = komanda.ExecuteReader();
            bool cont2 = true;
            while (citac.Read())
            {
               // lblMsgLogin.Text += citac["UserName"].ToString().ToLower();

                if (citac["UserName"].ToString().ToLower() == txt_UserName.Text.ToString().ToLower() && citac["Password"].ToString().ToLower() == txtPassword.Text.ToString().ToLower())
                {
                    cont2 = false;
                    Session["UserName"] = txt_UserName.Text.ToString().Trim();
                    lblMsgLogin.Text = "Successful Login !";
                    Response.Redirect("GoogleCalendarRegistration.aspx");
                }

            }
            if (cont2) { lblMsgLogin.Text = "Wrong UserName or Password !!!"; }
            citac.Close();
        }
        catch (Exception ex) { lblMsgLogin.Text = ex.ToString(); }
        finally { konekcija.Close();  }

    }
}