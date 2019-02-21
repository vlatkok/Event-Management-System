using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Configuration;
using Google.GData.Client;
using Google.GData.Calendar;
using Google.GData.AccessControl;
using Google.GData.Extensions;
using System.Data.SqlClient;

/// <summary>
/// Summary description for GoogleAccessExpiration
/// </summary>
public class GoogleAccessExpiration
{
    public static bool CheckForNotExpiredAccess(string AccessTokenStatus, string RefreshTokenStatus)
    {
        GoogleTokenModel TokenObject = new GoogleTokenModel();
        TokenObject.Access_Token = AccessTokenStatus;
        TokenObject.Refresh_Token = RefreshTokenStatus;
        CalendarService CalService = GoogleCalendarManager.GetCalendarService(TokenObject);
        string AllCalendarFeed = @"http://www.google.com/calendar/feeds/default/allcalendars/full";
        Uri postUri = new Uri(AllCalendarFeed);

        CalendarQuery CalendarQuery = new CalendarQuery();

        CalendarQuery.Uri = postUri;

        bool inform = true;

        try
        {
            CalendarFeed calFeed = CalService.Query(CalendarQuery);
        }
        catch (Exception e) { inform = false; };



        return inform;

    }

    public static void RevokeExpiredAccess(string AccessTokenStatus, string RefreshTokenStatus,string UserName)
    {
        string Access_Token = AccessTokenStatus;
        string Refresh_Token = RefreshTokenStatus;

      


        //Attempt the revoke from google
        //if successful then do a db / 
        if (GoogleCalendarManager.RevokeAccessToken(Access_Token.ToString(), Refresh_Token.ToString()) == true)
        {
            SqlConnection konekcija1 = new SqlConnection();
            konekcija1.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

            SqlCommand komanda1 = new SqlCommand();
            komanda1.Connection = konekcija1;

            komanda1.CommandText = "UPDATE Users SET AccessToken=@AccessToken,RefreshToken=@RefreshToken WHERE UserName=@UserName";
            komanda1.Parameters.Add("@AccessToken", "");
            komanda1.Parameters.Add("@RefreshToken", "");
            komanda1.Parameters.Add("@UserName", UserName);
            try
            {
                konekcija1.Open();
                komanda1.ExecuteNonQuery();
            }
            catch (Exception ex) {  }
            finally { konekcija1.Close(); };

        }

    }
}