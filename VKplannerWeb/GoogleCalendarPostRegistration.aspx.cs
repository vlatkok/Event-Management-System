using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Script.Serialization;
using Google.GData.Client;
using Google.GData.Calendar;
using Google.GData.Extensions;
using System.Net;
using System.Text;
using System.IO;
using System.Xml;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Configuration;
public partial class GoogleCalendarRegistration : System.Web.UI.Page
{
   
    private static string ReturnUrl = @System.Configuration.ConfigurationManager.AppSettings["GoogleReturnPageAddress"].ToString();


    private static string OwnCalendarFeed = @"http://www.google.com/calendar/feeds/default/owncalendars/full";
    private static string AllCalendarFeed = @"http://www.google.com/calendar/feeds/default/allcalendars/full";
    private static string PrivateFeed = @"http://www.google.com/calendar/feeds/default/private/full";

    private static string ClientID = System.Configuration.ConfigurationManager.AppSettings["GoogleCalendarApplicationClientID"].ToString();

    private static string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["GoogleCalendarApplicationClientSecret"].ToString();

    private string AllCalendarFeedURL = @"https://www.google.com/calendar/feeds/default/allcalendars/full";
    protected void Page_Load(object sender, EventArgs e)
    {

        if (IsPostBack == false)
        {
            if (string.IsNullOrEmpty(Request.QueryString["code"]) == false)
            {
                GoogleTokenModel TokenData = new GoogleTokenModel();
                string Result = "";
                try
                {
                    TokenData = ExchangeCodeWithAccessAndRefreshToken();
                }
                catch
                {
                    Result = "?Error=Internet connectivity problem. Please try later";
                  // HyperLink1.Visible = false;
                  //  hyperLinkRegistration.Visible = true;
                }

                //If no proper response is given
                if (string.IsNullOrEmpty(TokenData.Access_Token) == true)
                {
                    Result = "?Error=Some problem with google registration. Please try later or use a different gmail account to register";
                  //  HyperLink1.Visible = false;
                   // hyperLinkRegistration.Visible = true;
                }

                //If the gmail account is already registered
                else if (string.IsNullOrEmpty(TokenData.Access_Token) == false && string.IsNullOrEmpty(TokenData.Refresh_Token) == true)
                {
                    Result = "?Error=This gmail account is already registered with this application. Please use a different gmail account to register";
                  //  HyperLink1.Visible = false;
                 //   hyperLinkRegistration.Visible = true;
                }
                //Check if the some error is thrown
                if (string.IsNullOrEmpty(Result) == false)
                {
                    LblMessage.Text = Result;
                    return;

                }

                //If proper token data is acquired
                else if (string.IsNullOrEmpty(TokenData.Access_Token) == false && string.IsNullOrEmpty(TokenData.Refresh_Token) == false)
                {
                  //  HyperLink1.Visible = true;
                 //   hyperLinkRegistration.Visible = false;
                    LblMessage.Text = "Registration Complete. You can view that your gmail account is linked with your calendar application here <a href='https://accounts.google.com/b/0/IssuedAuthSubTokens?hl=en' target='blank'>Authorized Access to your Google Account</a> ";

             

                    SqlConnection konekcija = new SqlConnection();
                    konekcija.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

                    SqlCommand komanda = new SqlCommand();
                    komanda.Connection = konekcija;
                    komanda.CommandText = "UPDATE Users SET AccessToken=@AccessToken,"+
                                         "RefreshToken=@RefreshToken "+
                                         "WHERE UserName=@UserName";
                   komanda.Parameters.Add("@AccessToken", TokenData.Access_Token.ToString());
                  komanda.Parameters.Add("@RefreshToken", TokenData.Refresh_Token.ToString());
                    komanda.Parameters.Add("@UserName", (string)Session["UserName"]);
                    try
                    {
                        konekcija.Open();
                        komanda.ExecuteNonQuery();
                    }
                    catch (Exception ex) { LblMessage.Text += ex.ToString(); }
                    finally { konekcija.Close(); };
                
                
                
                }
            }
            else
            {
                //if access denied 
                if (Request.QueryString["error"] != null)
                {
                    LblMessage.Text = "Access is Denied. Please grant access to continue to google calendar registration";
                 //   HyperLink1.Visible = false;
                  //  hyperLinkRegistration.Visible = true;
                }
                else
                {

                }
            }
        }






    }

    // avtentikacija na accountot
    public static string GenerateGoogleOAuthURL()
    {
        string Url = "https://accounts.google.com/o/oauth2/auth?scope={0}&redirect_uri={1}&response_type={2}&client_id={3}&state={4}&access_type={5}&approval_prompt={6}";
        //string scope = UrlEncodeForGoogle("http://www.google.com/calendar/feeds/default/private/full").Replace("%20", "+");
        string scope = UrlEncodeForGoogle(PrivateFeed).Replace("%20", "+");
        string redirect_uri_encode = UrlEncodeForGoogle(ReturnUrl);
        string response_type = "code";
        string state = "";
        string access_type = "offline";
        string approval_prompt = "auto";
        return string.Format(Url, scope, redirect_uri_encode, response_type, ClientID, state, access_type, approval_prompt);
    }
    //adresa do accountot
    private static string UrlEncodeForGoogle(string url)
    {

        string UnReservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        var result = new StringBuilder();

        foreach (char symbol in url)
        {
            if (UnReservedChars.IndexOf(symbol) != -1)
            {
                result.Append(symbol);
            }
            else
            {
                result.Append('%' + String.Format("{0:X2}", (int)symbol));
            }
        }
        return result.ToString();
    }// razmena na access token i refresh token
    private GoogleTokenModel ExchangeCodeWithAccessAndRefreshToken()
    {
        string Url = "https://accounts.google.com/o/oauth2/token";
        string grant_type = "authorization_code";
        string redirect_uri_encode = UrlEncodeForGoogle(ReturnUrl);
        string data = "code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type={4}";
        string Code = Request.QueryString["Code"];

        HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
        string result = null;
        request.Method = "POST";
        request.KeepAlive = true;
        request.ContentType = "application/x-www-form-urlencoded";
        string param = string.Format(data, Code, ClientID, ClientSecret, redirect_uri_encode, grant_type);
        var bs = Encoding.UTF8.GetBytes(param);
        using (Stream reqStream = request.GetRequestStream())
        {
            reqStream.Write(bs, 0, bs.Length);
        }

        using (WebResponse response = request.GetResponse())
        {
            var sr = new StreamReader(response.GetResponseStream());
            result = sr.ReadToEnd();
            sr.Close();
        }

        var jsonSerializer = new JavaScriptSerializer();
        var tokenData = jsonSerializer.Deserialize<GoogleTokenModel>(result);

        return tokenData;

    }
    /// <summary>
    /// Zemanje na nov access token
    /// </summary>
    /// <param name="GoogleTokenModelObj"></param>
    /// <returns></returns>
    private bool GetNewAccessToken(GoogleTokenModel GoogleTokenModelObj)
    {
        try
        {
            string Url = "https://accounts.google.com/o/oauth2/token";
            string grant_type = "refresh_token";
            string redirect_uri_encode = UrlEncodeForGoogle(ReturnUrl);
            string data = "client_id={0}&client_secret={1}&refresh_token={2}&grant_type={3}";

            HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
            string result = null;
            request.Method = "POST";
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";
            string param = string.Format(data, ClientID, ClientSecret, GoogleTokenModelObj.Refresh_Token, grant_type);
            var bs = Encoding.UTF8.GetBytes(param);
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }

            using (WebResponse response = request.GetResponse())
            {
                var sr = new StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();
            }

            var jsonSerializer = new JavaScriptSerializer();
            var TokenData = jsonSerializer.Deserialize<GoogleTokenModel>(result);

            GoogleTokenModelObj.Access_Token = TokenData.Access_Token;
            if (TokenData.Refresh_Token != null)
                GoogleTokenModelObj.Refresh_Token = TokenData.Refresh_Token;
            //GoogleTokenModelObj.LastAccessDateTime = DateTime.Now;

            //Update the refresh and access token in DB for next login usage
            //GoogleCalendarManager.UpdateRefreshTokenInGoogleAppointmentOAuth(GoogleTokenModelObj);

            // saving to database ne tokens !!!!

      
            SqlConnection konekcija = new SqlConnection();
            konekcija.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

            SqlCommand komanda = new SqlCommand();
            komanda.Connection = konekcija;
            komanda.CommandText = "UPDATE Users SET AccesToken=@AccessToken,RefreshToken=@RefreshToken WHERE UserName=@UserName";
            komanda.Parameters.Add("@AccessToken", TokenData.Access_Token.ToString());
            komanda.Parameters.Add("@RefreshToken", TokenData.Refresh_Token.ToString());
            komanda.Parameters.Add("@UserName", (string)Session["UserName"]);
            try
            {
                konekcija.Open();
                komanda.ExecuteNonQuery();
            }
            catch (Exception ex) { LblMessage.Text = ex.ToString(); }
            finally { konekcija.Close(); };




            return true;

        }
        catch
        {
            return false;
        }
    }



    // odjava od google profil

    protected void btnRevoke_Click(object sender, EventArgs e)
    {
        string Access_Token = "";
        string Refresh_Token = "";

        SqlConnection konekcija = new SqlConnection();
        konekcija.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

        SqlCommand komanda = new SqlCommand();
        komanda.Connection = konekcija;
        komanda.CommandText = "SELECT * FROM Users WHERE UserName=@UserName";
        komanda.Parameters.Add("@UserName", (string)Session["UserName"]);

        try
        {
            konekcija.Open();
            SqlDataReader citac = komanda.ExecuteReader();
            while (citac.Read())
            {


                if (citac["UserName"].ToString().ToLower() == ((string)Session["UserName"]).ToLower())
                {

                    Access_Token = (string)citac["AccessToken"];
                    Refresh_Token = (string)citac["RefreshToken"];
                }

            }
            citac.Close();
        }
        catch (Exception ex) { LblMessage.Text = ex.ToString(); }
        finally { konekcija.Close(); }



        //Attempt the revoke from google
        //if successful then do a db / xml delete as well
        if (GoogleCalendarManager.RevokeAccessToken(Access_Token.ToString(), Refresh_Token.ToString()) == true)
        {
            SqlConnection konekcija1 = new SqlConnection();
            konekcija1.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

            SqlCommand komanda1 = new SqlCommand();
            komanda1.Connection = konekcija1;

            komanda1.CommandText = "UPDATE Users SET AccessToken=@AccessToken,RefreshToken=@RefreshToken WHERE UserName=@UserName";
            komanda1.Parameters.Add("@AccessToken", "");
            komanda1.Parameters.Add("@RefreshToken", "");
            komanda1.Parameters.Add("@UserName", (string)Session["UserName"]);
            try
            {
                konekcija1.Open();
                komanda1.ExecuteNonQuery();
            }
            catch (Exception ex) { LblMessage.Text = ex.ToString(); }
            finally { konekcija1.Close(); };
            Response.Redirect("GoogleCalendarRegistration.aspx?message=Revoke");
        }
       
    }
    protected void btnGotoEvents_Click(object sender, EventArgs e)
    {
        Response.Redirect("Events.aspx");
    }
}