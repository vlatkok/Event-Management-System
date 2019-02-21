using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Configuration;
using Google.GData.Client;
using Google.GData.Calendar;
using Google.GData.Extensions;
// proverka  i generiranje na tokeni za registracija na gmail adresa

public partial class GoogleCalendarRegistration : System.Web.UI.Page
{
    protected string RefreshTokenStatus;
    protected string AccessTokenStatus;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessTokenStatus = ""; RefreshTokenStatus = "";


            string rightsrevoked = (string)Request.QueryString["message"];

            

                if (CheckUserGoogleAccessInDatabase((string)Session["UserName"]) == false)
                {
                    lblRecommendation.Text = "Please click the register button to register with google calendar ";
                    if (rightsrevoked == "Revoke")
                    { lblMessage.Text = "Rights revoked successfully.You can <a href='https://accounts.google.com/b/0/IssuedAuthSubTokens?hl=en' target='blank'>view</a> that you gmail account is not linked with your calendar application anymore"; }
                }
                else if (rightsrevoked == "Revoke")
                { lblMessage.Text = "Rights revoked successfully.You can <a href='https://accounts.google.com/b/0/IssuedAuthSubTokens?hl=en' target='blank'>view</a> that you gmail account is not linked with your calendar application anymore"; }

                else
                {// proverka dali se isteceni revoke ...

                    if (GoogleAccessExpiration.CheckForNotExpiredAccess(AccessTokenStatus, RefreshTokenStatus))
                    { Response.Redirect("Events.aspx"); }
                    else { 
                    //revoke and redirect
                        GoogleAccessExpiration.RevokeExpiredAccess(AccessTokenStatus, RefreshTokenStatus, (string)Session["UserName"]);

                        lblRecommendation.Text = "Your registration Access Token Expired !!! Please click the register button to register with google calendar ";
                    
                    }


                }
        }

        if (Session["UserName"] == null)
        {
            Response.Redirect("Default.aspx");

        }
        else
        {
            lblUser.Text = "You are logged in as: " + "<b>" + (string)Session["UserName"] + "</b>";

        }





    } ///registracija
    protected void btnRegister_Click(object sender, EventArgs e)
    {
        string GoogleReturnPageAddress = System.Configuration.ConfigurationManager.AppSettings["GoogleReturnPageAddress"];

        Response.Redirect(GoogleCalendarManager.GenerateGoogleOAuthURL());
    }
    // proverka dali korisnikot e vo bazata na podatoci
    protected bool CheckUserGoogleAccessInDatabase(string User)
    {
        bool answer = true;
        SqlConnection konekcija = new SqlConnection();
        konekcija.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

        SqlCommand komanda = new SqlCommand();
        komanda.Connection = konekcija;
        komanda.CommandText = "SELECT * FROM Users WHERE UserName=@UserName";
        komanda.Parameters.Add("@UserName", User);

        try
        {
            konekcija.Open();
            SqlDataReader citac = komanda.ExecuteReader();
            while (citac.Read())
            {


                if (citac["UserName"].ToString().ToLower() == User.ToLower())
                {
                    AccessTokenStatus = citac["AccessToken"].ToString();
                    RefreshTokenStatus = citac["RefreshToken"].ToString();

                    if ((citac["AccessToken"].ToString().ToLower() == "") && (citac["RefreshToken"].ToString().ToLower() == ""))
                    {

                        answer = false;
                    }
                    else if ((citac["AccessToken"].ToString().ToLower() != "") && ((citac["RefreshToken"].ToString().ToLower() == "")))
                    {
                        answer = true;

                    }
                    break;
                }

            }
            citac.Close();
        }
        catch (Exception ex) { lblRecommendation.Text = ex.ToString(); }
        finally { konekcija.Close(); }
        return answer;
    }
    // odjavuvanje od web stranata
    protected void btnLogOut_Click(object sender, EventArgs e)
    {
        Session["UserName"] = null;
        Response.Redirect("Default.aspx");

    }
  


}