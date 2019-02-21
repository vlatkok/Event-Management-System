using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using Google.GData.Client;
using Google.GData.Calendar;
using Google.GData.AccessControl;
using Google.GData.Extensions;
using System.Data.Odbc;
using System.Data;

// klasa za operacii so nastanite, dodavanje brisenje ,azuriranje
public partial class Events : System.Web.UI.Page
{
    string Access_Token ;
    string Refresh_Token ;
   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            pnlPreviewEvents.Visible = true;
            btnPreviewEvents.Visible = false;
            pnlAddEvent.Visible = false;
            pnlMapPreview.Visible = false;
            pnlMapEventLocation.Visible = false;
            pnlEditGreedView.Visible = false;

           
         }

        if (Session["UserName"] == null)
        {
            Response.Redirect("Default.aspx");

        }
        else {
            lblMessage.Text = "You are logged in as: " + "<b>" + (string)Session["UserName"] + "</b>";
        
        }

       


    }
    // odjava na access token i refresh token
    protected void btnGoogleRevoke_Click(object sender, EventArgs e)
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
        catch (Exception ex) { lblMessage.Text = ex.ToString(); }
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
            catch (Exception ex) { lblMessage.Text = ex.ToString(); }
            finally { konekcija1.Close(); };                                    

        }
       Response.Redirect("GoogleCalendarRegistration.aspx?message=Revoke");
    }
    protected void btnLogOut_Click(object sender, EventArgs e)
    {
        Session["UserName"] = null;
        Response.Redirect("Default.aspx");
    }
    protected void btnNewEvent_Click(object sender, EventArgs e)
    {
        btnSaveUpdate.Enabled = true;
        btnDeleteEvent.Enabled = false;
        CleanFieldsInNewEvents();
        pnlEditGreedView.Visible = false;
        pnlMapPreview.Visible = false;
        pnlMapEventLocation.Visible = false;
        pnlPreviewEvents.Visible = false;
        pnlAddEvent.Visible = true;
        btnNewEvent.Visible = false;
        chkBoxTransport.Visible = false;
        ddlTransport.Visible = false;
        chkBoxWeather.Visible = false;
        btnPreviewEvents.Visible = true;
        btnEditEvents.Visible = true;
        ViewState["access"] = "insert";
    }
    protected void chkBoxLocation_CheckedChanged(object sender, EventArgs e)
    {
        if (chkBoxLocation.Checked)
        {
            pnlMapPreview.Visible = true;
            pnlMapEventLocation.Visible = true;
            chkBoxTransport.Visible = true;
            chkBoxWeather.Visible = true;
            
        }
        else {

            pnlMapPreview.Visible = false;
            pnlMapEventLocation.Visible = false;
            chkBoxTransport.Visible = false;
            chkBoxWeather.Visible = false;
            ddlTransport.Visible = false;
           
        }
    }

    protected void chkBoxTransport_CheckedChanged(object sender, EventArgs e)
    {

        if (chkBoxTransport.Checked)
        {
            ddlTransport.Visible = true;
            
        }
        else { ddlTransport.Visible = false; }
        
    }


    // zacuvuvanje i azuriranje na nastan
    protected void btnSaveUpdate_Click(object sender, EventArgs e)
    {  ///// mm/dd//yyyy
      
        lblMessage.Text="";
        string[] separator1 = { "/" };
       string []startdate=txtStartDate.Text.ToString().Split(separator1,StringSplitOptions.None);
       string[] enddate = txtEndDate.Text.ToString().Split(separator1,StringSplitOptions.None);
        int starthour=Convert.ToInt16(lstboxStartHour.SelectedValue.ToString());
        int startminutes=Convert.ToInt16(lstboxStartMinutes.SelectedValue.ToString());
        int endhour=Convert.ToInt16(lstboxEndHours.SelectedValue.ToString());
        int endminute=Convert.ToInt16(lstboxEndMinutes0.SelectedValue.ToString());
       DateTime StartDateTime = new DateTime(Convert.ToInt16(startdate[2]), Convert.ToInt16(startdate[0]), Convert.ToInt16(startdate[1]),starthour ,startminutes , 0);
       DateTime EndDateTime = new DateTime(Convert.ToInt16(enddate[2]), Convert.ToInt16(enddate[0]), Convert.ToInt16(enddate[1]),endhour ,endminute , 0);
       //ako startDatetime e postaro od momentot na save
        if (DateTime.Compare(DateTime.Now,StartDateTime)>0)

        {lblDateMessage.Text="Start Date-Time Should be Later from Now !!!";}
        else{
            int rez=DateTime.Compare(StartDateTime,EndDateTime);
            if(rez==0)
            {
            
            lblErorEntryMessages.Text="Star Date-time and End Date-time are equal !!!";


            }
            else if (rez > 0) { lblErorEntryMessages.Text = "Start Date-time is Later than End Date-time"; }

            else
            {

                // prodolzi da azuriras


                List<GoogleCalendarAppointmentModel> GoogleCalendarAppointmentModelList = new List<GoogleCalendarAppointmentModel>();
                List<GoogleTokenModel> GoogleTokenModelList = new List<GoogleTokenModel>();

                GoogleCalendarAppointmentModel GoogleCalendarAppointmentModelObj = new GoogleCalendarAppointmentModel();
                GoogleTokenModel GoogleTokenModelObj = new GoogleTokenModel();



                string eventnumber = "";

                if (ViewState["access"].ToString() == "update")
                {
                    eventnumber = ViewState["eventID"].ToString();

                    gridViewEvent.SelectedIndex = -1;

                }
                else if (ViewState["access"].ToString() == "insert")
                {

                    eventnumber = MaxEventID().ToString();
                    ViewState["eventID"] = eventnumber;

                }
                else { lblSuccessfullUpdateSave.Text = "Error in decision=ViewState insert or update "; }


                //  ViewState["eventID"] = eventnumber.ToString();



                #region populate GoogleAppointment values


                GoogleCalendarAppointmentModelObj.EventID = eventnumber;
                GoogleCalendarAppointmentModelObj.EventTitle = txtTitleEvent.Text.Trim();
                GoogleCalendarAppointmentModelObj.EventStartTime = StartDateTime;
                GoogleCalendarAppointmentModelObj.EventEndTime = EndDateTime;
                if (txtRemainder1.Text != "")
                {
                    GoogleCalendarAppointmentModelObj.Remainder1 = ddlRemainder1.SelectedValue.Trim() + " " + txtRemainder1.Text.Trim() + " " + ddltimeRemainder1.SelectedValue.Trim();
                }
                else
                {
                    GoogleCalendarAppointmentModelObj.Remainder1 = "";
                }
                if (txtRemainder2.Text != "")
                {
                    GoogleCalendarAppointmentModelObj.Remainder2 = ddlRemainder2.SelectedValue.Trim() + " " + txtRemainder2.Text.Trim() + " " + ddltimeRemainder2.SelectedValue.Trim();
                }
                else
                {
                    GoogleCalendarAppointmentModelObj.Remainder2 = "";
                }

                //Giving the proper location so you can view on the map in google calendar
                if (chkBoxLocation.Checked)
                {
                    GoogleCalendarAppointmentModelObj.EventLocation = hfCoordinates.Value;
                }
                else
                {
                    GoogleCalendarAppointmentModelObj.EventLocation = "";
                };



                if (chkBoxTransport.Checked && (ddlTransport.SelectedIndex != -1))
                {

                    GoogleCalendarAppointmentModelObj.EventTransport = ddlTransport.SelectedValue;

                }
                else { GoogleCalendarAppointmentModelObj.EventTransport = ""; }

                if (chkBoxWeather.Checked)
                {
                    GoogleCalendarAppointmentModelObj.Weather = "yes";
                }
                else
                {
                    GoogleCalendarAppointmentModelObj.Weather = "no";
                };


                GoogleCalendarAppointmentModelObj.EventDetails = txtEventDescription.Text.Trim();
                GoogleCalendarAppointmentModelList.Add(GoogleCalendarAppointmentModelObj);
                #endregion

                #region populate GoogleToken values

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




                        GoogleTokenModelObj.Access_Token = (string)citac["AccessToken"];
                        GoogleTokenModelObj.Refresh_Token = (string)citac["RefreshToken"];


                    }
                    citac.Close();
                }
                catch (Exception ex) { lblMessage.Text = ex.ToString(); }
                finally { konekcija.Close(); }



                GoogleTokenModelList.Add(GoogleTokenModelObj);

                #endregion

                #region Add event to google calendar

                if (GoogleCalendarManager.AddUpdateDeleteEvent(GoogleTokenModelList, GoogleCalendarAppointmentModelList, 0) == true)
                {
                    SqlConnection konekcija2 = new SqlConnection();
                    konekcija2.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

                    SqlCommand komanda2 = new SqlCommand();
                    komanda2.Connection = konekcija2;
                    string successstring = "????";
                    if (ViewState["access"].ToString() == "insert")
                    {
                        komanda2.CommandText = "INSERT INTO [Events] (UserName, EventTitle,EventStartTime,EventEndTime,EventLocation,EventDetails,EventRemainder1,EventRemainder2,EventTransport,Weather) VALUES(@username,@EventTitle,@EventStartTime,@EventEndTime,@EventLocation,@EventDetails,@EventRemainder1,@EventRemainder2,@EventTransport,@Weather)";
                        komanda2.Parameters.Add("@username", (string)Session["UserName"]);
                        komanda2.Parameters.Add("@EventTitle", GoogleCalendarAppointmentModelObj.EventTitle);
                        komanda2.Parameters.Add("@EventStartTime", GoogleCalendarAppointmentModelObj.EventStartTime);
                        komanda2.Parameters.Add("@EventEndTime", GoogleCalendarAppointmentModelObj.EventEndTime);
                        komanda2.Parameters.Add("@EventLocation", GoogleCalendarAppointmentModelObj.EventLocation);
                        komanda2.Parameters.Add("@EventDetails", GoogleCalendarAppointmentModelObj.EventDetails);
                        komanda2.Parameters.Add("@EventRemainder1", GoogleCalendarAppointmentModelObj.Remainder1);
                        komanda2.Parameters.Add("@EventRemainder2", GoogleCalendarAppointmentModelObj.Remainder2);
                        komanda2.Parameters.Add("@EventTransport", GoogleCalendarAppointmentModelObj.EventTransport);
                        komanda2.Parameters.Add("@Weather", GoogleCalendarAppointmentModelObj.Weather);
                    }
                    else if (ViewState["access"].ToString() == "update")
                    {
                        komanda2.CommandText = "UPDATE Events SET  EventTitle=@EventTitle,EventStartTime=@EventStartTime,EventEndTime=@EventEndTime,EventLocation=@EventLocation,EventDetails=@EventDetails,EventRemainder1=@EventRemainder1,EventRemainder2=@EventRemainder2,EventTransport=@EventTransport,Weather=@Weather WHERE UserName=@UserName AND EventID=@EventID";
                        komanda2.Parameters.Add("@EventTitle", GoogleCalendarAppointmentModelObj.EventTitle);
                        komanda2.Parameters.Add("@EventStartTime", GoogleCalendarAppointmentModelObj.EventStartTime);
                        komanda2.Parameters.Add("@EventEndTime", GoogleCalendarAppointmentModelObj.EventEndTime);
                        komanda2.Parameters.Add("@EventLocation", GoogleCalendarAppointmentModelObj.EventLocation);
                        komanda2.Parameters.Add("@EventDetails", GoogleCalendarAppointmentModelObj.EventDetails);
                        komanda2.Parameters.Add("@EventRemainder1", GoogleCalendarAppointmentModelObj.Remainder1);
                        komanda2.Parameters.Add("@EventRemainder2", GoogleCalendarAppointmentModelObj.Remainder2);
                        komanda2.Parameters.Add("@EventTransport", GoogleCalendarAppointmentModelObj.EventTransport);
                        komanda2.Parameters.Add("@Weather", GoogleCalendarAppointmentModelObj.Weather);
                        komanda2.Parameters.Add("@UserName", (string)Session["UserName"]);
                        komanda2.Parameters.Add("@EventID", GoogleCalendarAppointmentModelObj.EventID);

                    }

                    try
                    {
                        konekcija2.Open();
                        komanda2.ExecuteNonQuery();
                    }
                    catch (Exception ex) { lblMessage.Text = ex.ToString(); }
                    finally
                    {
                        konekcija2.Close();
                        if (ViewState["access"].ToString() == "update") { successstring = "Updated"; CleanFieldsInNewEvents(); btnSaveUpdate.Enabled = false; btnDeleteEvent.Enabled = false; }
                        else if (ViewState["access"].ToString() == "insert") { successstring = "Created"; btnDeleteEvent.Enabled = true; };
                        lblSuccessfullUpdateSave.Text = "Event " + successstring + " successfully. Go to <a href='https://www.google.com/calendar/' target='blank'>Google Calendar</a> to view your event ";

                    }

                    btnDeleteEvent.Enabled = true;
                    if (ViewState["access"].ToString() == "update")
                    { FillGridView(); }

                }
                #endregion




            }
        }
    }
    // dodavanje na sledno id
    protected int MaxEventID()
    {

        SqlConnection konekcija = new SqlConnection();
        konekcija.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

        SqlCommand komanda = new SqlCommand();
        komanda.Connection = konekcija;
        komanda.CommandText = "SELECT EventID FROM Events WHERE UserName=@UserName";
        komanda.Parameters.Add("@UserName", (string)Session["UserName"]);
        int max = 0;
        try
        {
            konekcija.Open();
            SqlDataReader citac = komanda.ExecuteReader();
            while (citac.Read())
            {


                if (Convert.ToInt16(citac["EventID"].ToString())>max)
                {
                    max=Convert.ToInt16(citac["EventID"].ToString());
                }

            }
            citac.Close();
        }
        catch (Exception ex) { lblMessage.Text = ex.ToString(); }
        finally { konekcija.Close(); }

        return max+1;
    }
    // povikuvanje na id na defaultniot calendar
    public string GetCalendarIdentifier()
    {
        GoogleTokenModel GoogleTokenModelObject = new GoogleTokenModel();


        SqlConnection konekcija3 = new SqlConnection();
        konekcija3.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

        SqlCommand komanda = new SqlCommand();
        komanda.Connection = konekcija3;
        komanda.CommandText = "SELECT * FROM Users WHERE UserName=@UserName";
        komanda.Parameters.Add("@UserName", (string)Session["UserName"]);

        try
        {
            konekcija3.Open();
            SqlDataReader citac = komanda.ExecuteReader();
            while (citac.Read())
            {
                GoogleTokenModelObject.Access_Token = (string)citac["AccessToken"];
                GoogleTokenModelObject.Refresh_Token = (string)citac["RefreshToken"];
                }
            citac.Close();
        }
        catch (Exception ex) { lblMessage.Text = ex.ToString(); }
        finally { konekcija3.Close(); }

        // zapocnuva get calendar id
        CalendarService CalService = GoogleCalendarManager.GetCalendarService(GoogleTokenModelObject);
        string AllCalendarFeed = @"http://www.google.com/calendar/feeds/default/allcalendars/full";
         Uri postUri = new Uri(AllCalendarFeed);

        CalendarQuery CalendarQuery = new CalendarQuery();

        CalendarQuery.Uri = postUri;
        string CalendarID = "";
        try
        {
            CalendarFeed calFeed = CalService.Query(CalendarQuery);
        

       

        if (calFeed != null && calFeed.Entries.Count > 0)
        {
            foreach (CalendarEntry CalEntry in calFeed.Entries)
            {
                //Commented to post the new appointments on the main calendar instead of cleverfox calendar
                //if (CalEntry.Title.Text.Contains("Cleverfox") == true)
                //{
                //CalendarID = CalEntry.Title.Text;
                CalendarID = CalEntry.EditUri.ToString().Substring(CalEntry.EditUri.ToString().LastIndexOf("/") + 1);
                break;
                //}
            }
        }

        }
        catch (Exception e) { lblMessage.Text = e.ToString(); }
        return CalendarID;
        
    }





    protected void btnPreviewEvents_Click(object sender, EventArgs e)
    {
        pnlPreviewEvents.Visible = true;
        pnlMapPreview.Visible = false;
        pnlMapEventLocation.Visible = false;
        pnlAddEvent.Visible = false;
        btnNewEvent.Visible = false;
        pnlEditGreedView.Visible = false;
        btnPreviewEvents.Visible = false;
        btnEditEvents.Visible = true;
        btnNewEvent.Visible = true;
    }
    
    protected void btnEditEvents_Click(object sender, EventArgs e)
    {
        btnPreviewEvents.Visible = true;
        pnlPreviewEvents.Visible = false;
        pnlMapPreview.Visible = false;                                              
        pnlMapEventLocation.Visible = false;
        btnNewEvent.Visible = true;
        pnlEditGreedView.Visible = true;
        CleanFieldsInNewEvents();
        pnlAddEvent.Visible = false;
        FillGridView();
        btnEditEvents.Visible = false;

        ViewState["access"] = "update";
    }

  /**  protected void FillColorChooserDDl()
    {
        string[] colorslist = { "DarkBlue", "Turquoise", "DarkGreen", "Yellow", "Orange", "Red", "Pink", "Gainsboro" };
        string[] colorlistkey = { "#00008B", "#40E0D0", "#006400", "#FFFF00", "#FFA500", "#FF0000", "#FFC0CB", "#DCDCDC" };
     
        for (int i = 0; i < colorslist.Length; i++)
        {
            ListItem items = new ListItem();
            string colorname = "background-color:" + colorslist[i].ToString().Trim().ToLower();
            items.Attributes.Add("style", colorname);
            items.Value = colorlistkey[i].ToString();
            ddListColor.Items.Add(items);
          }

           
                

    }***/


    protected void btnDeleteEvent_Click(object sender, EventArgs e)
    {

        DeleteEventMethod();
                                     

            lblSuccessfullUpdateSave.Text = "Event deleted successfully. Go to <a href='https://www.google.com/calendar/' target='blank'>Google Calendar</a> to view that your event is deleted ! ";

            btnDeleteEvent.Enabled = false;
            CleanFieldsInNewEvents();
            if (ViewState["access"].ToString() == "update")
            {


                FillGridView();
                gridViewEvent.SelectedIndex = -1;
            
            }


    }
    // brisenje na nastan
    protected void DeleteEventMethod()
    { 
    
    List<GoogleCalendarAppointmentModel> GoogleCalendarAppointmentModelList = new List<GoogleCalendarAppointmentModel>();
        List<GoogleTokenModel> GoogleTokenModelList = new List<GoogleTokenModel>();

        GoogleCalendarAppointmentModel GoogleCalendarAppointmentModelObj = new GoogleCalendarAppointmentModel();
        GoogleTokenModel GoogleTokenModelObj = new GoogleTokenModel();

        #region populate GoogleAppointment values
        GoogleCalendarAppointmentModelObj.EventID = ViewState["eventID"].ToString();
        GoogleCalendarAppointmentModelObj.DeleteAppointment = true;
       
       GoogleCalendarAppointmentModelList.Add(GoogleCalendarAppointmentModelObj);
        #endregion

        #region populate GoogleToken values

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
                
                GoogleTokenModelObj.Access_Token = (string)citac["AccessToken"];
                GoogleTokenModelObj.Refresh_Token = (string)citac["RefreshToken"];
                
            }
            citac.Close();
        }
        catch (Exception ex) { lblMessage.Text = ex.ToString(); }
        finally { konekcija.Close(); }

       
        GoogleTokenModelList.Add(GoogleTokenModelObj);

        #endregion

        if (GoogleCalendarManager.AddUpdateDeleteEvent(GoogleTokenModelList, GoogleCalendarAppointmentModelList, 0) == true)
        {


            //save data in DB
            SqlConnection konekcija2 = new SqlConnection();
            konekcija2.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

            SqlCommand komanda2 = new SqlCommand();
            komanda2.Connection = konekcija2;
            komanda2.CommandText = "DELETE FROM Events WHERE UserName=@username AND EventID=@EventID";
            komanda2.Parameters.Add("@username", (string)Session["UserName"]);
            komanda2.Parameters.Add("@EventID", (string)ViewState["eventID"]);

            try
            {
                konekcija2.Open();
                komanda2.ExecuteNonQuery();
            }
            catch (Exception ex) { lblMessage.Text = ex.ToString(); }
            finally
            {
                konekcija2.Close();
                FillGridView();

            }

        }
    
    
    }




    protected void CleanFieldsInNewEvents()
    {
        txtEventDescription.Text = "";
        txtTitleEvent.Text = "";
        txtStartDate.Text = "";
        txtEndDate.Text = "";
        txtRemainder1.Text = "";
        txtRemainder2.Text = "";
        chkBoxLocation.Checked = false;
        chkBoxWeather.Checked = false;
        chkBoxTransport.Checked = false;
        lstboxStartHour.SelectedIndex = 0;
        lstboxStartMinutes.SelectedIndex = 0;
        lstboxEndHours.SelectedIndex = 0;
        lstboxEndMinutes0.SelectedIndex = 0;
        ddlRemainder1.SelectedIndex = 0;
        ddlRemainder2.SelectedIndex = 0;
        ddltimeRemainder1.SelectedIndex = 0;
        ddltimeRemainder2.SelectedIndex = 0;
        ddlTransport.SelectedIndex = 0;
        lblSuccessfullUpdateSave.Text = "";
        lblErorEntryMessages.Text = "";


    }
    // ispolnuvanje na gridview
    protected void FillGridView()
    {

        SqlConnection konekcija = new SqlConnection();
        konekcija.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

        SqlCommand komanda = new SqlCommand();
        komanda.Connection = konekcija;
        komanda.CommandText = "SELECT * FROM Events WHERE UserName=@UserName";
        komanda.Parameters.Add("@UserName", (string)Session["UserName"]);

        SqlDataAdapter adapter = new SqlDataAdapter(komanda);
        DataSet ds = new DataSet();

        try
        {

            konekcija.Open();
            adapter.Fill(ds, "Events");
            gridViewEvent.DataSource = ds;
            gridViewEvent.DataBind();
            ViewState["dataset"] = ds;


        }
        catch (Exception err)
        {
            lblMessage.Text = err.Message;

        }
        finally { konekcija.Close(); }


    }



    //f-ja za selekcija na red i ispolnuvanje na panel za karakteristiki na nastan
    protected void gridViewEvent_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["access"] = "update";
        string selectedID=gridViewEvent.SelectedRow.Cells[1].Text;
        pnlAddEvent.Visible=true;
        ViewState["eventID"] = selectedID;
        FillEventProperties(selectedID);

        btnSaveUpdate.Enabled = true;
        btnDeleteEvent.Enabled = true;

      }
    // funkcija za promena na stranicata
    protected void gridview_PageIndexChange(object sender, GridViewPageEventArgs e)
    {

        gridViewEvent.PageIndex = e.NewPageIndex;
        gridViewEvent.SelectedIndex = -1;
        DataSet ds = (DataSet)ViewState["dataset"];
        gridViewEvent.DataSource = ds;
        gridViewEvent.DataBind();
    
    
    }
    /*
    protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
    {

      
        DataSet ds = (DataSet)ViewState["dataset"];
        DataView dv = ds.Tables[0].DefaultView;
        if (ViewState["nasoka"] == null)
            ViewState["nasoka"] = "ASC";
        if ((string)ViewState["nasoka"] == "DESC")
        {
            dv.Sort = e.SortExpression + " DESC";
            ViewState["nasoka"] = "ASC";

        }
        else {

            dv.Sort = e.SortExpression + " " + " ASC";
            ViewState["nasoka"] = "DESC";
        
        
        }


        gridViewEvent.DataSource = dv;
        gridViewEvent.DataBind();


    }

  **/

    protected void FillEventProperties(string EventID)
    {
        SqlConnection konekcija = new SqlConnection();
        konekcija.ConnectionString = ConfigurationManager.ConnectionStrings["mojaKonekcija"].ConnectionString;

        SqlCommand komanda = new SqlCommand();
        komanda.Connection = konekcija;
        komanda.CommandText = "SELECT * FROM Events WHERE UserName=@UserName AND EventID=@EventID";
        komanda.Parameters.Add("@UserName", (string)Session["UserName"]);
        komanda.Parameters.Add("@EventID", EventID);

        try
        {
            konekcija.Open();
            SqlDataReader citac = komanda.ExecuteReader();
            while (citac.Read())
            {
                ViewState["eventID"] = EventID;
                txtTitleEvent.Text = citac["EventTitle"].ToString().Trim();
                DateTime startDate = Convert.ToDateTime(citac["EventStartTime"].ToString());
                txtStartDate.Text = startDate.ToString("MM/dd/yyyy");
                lstboxStartHour.SelectedIndex = Convert.ToInt16(startDate.Hour);
                lstboxStartMinutes.SelectedIndex = Convert.ToInt16(startDate.Minute);

                DateTime endDate = Convert.ToDateTime(citac["EventEndTime"].ToString());
                txtEndDate.Text = endDate.ToString("MM/dd/yyyy");
                lstboxEndHours.SelectedIndex = Convert.ToInt16(endDate.Hour);
                lstboxEndMinutes0.SelectedIndex = Convert.ToInt16(endDate.Minute);

                if ((string)citac["EventLocation"] == "")
                {
                    chkBoxLocation.Checked = false;
                    chkBoxTransport.Checked = false;
                    chkBoxWeather.Checked = false;

                                    

                }
                else {

                    chkBoxLocation.Checked = true;
                    lblLocation.Text = (string)citac["EventLocation"];

                    if ((citac["EventTransport"] !=null)&& (citac["EventTransport"].ToString() != ""))
                    {

                        chkBoxTransport.Checked = true;
                        ListItem lt = new ListItem();
                        lt.Text = citac["EventTransport"].ToString();
                        lt.Value = citac["EventTransport"].ToString();
                        //sluzi pri brisenje
                        ViewState["transport"] = citac["EventTransport"].ToString();
                        ddlTransport.Visible = true;
                        ddlTransport.SelectedIndex = ddlTransport.Items.IndexOf(lt);

                    }
                    else if (string.IsNullOrEmpty(citac["EventTransport"].ToString())) { chkBoxTransport.Checked = false; ddlTransport.Visible = false; }

                    if ((citac["Weather"]!=null)&&(citac["Weather"].ToString() == "yes") )
                    {
                        chkBoxWeather.Checked = true;
                       

                    }
                    else { chkBoxWeather.Checked = false; }
                    //sluzi pri brisenje
                    ViewState["weather"] = citac["Weather"].ToString();
                }

                if (citac["EventDetails"].ToString() != "")
                {

                    txtEventDescription.Text = citac["EventDetails"].ToString();
                
                
                }
                if (citac["EventRemainder1"].ToString() != "")
                {

                    string[] remainder1 = citac["EventRemainder1"].ToString().Trim().Split(' ');
                    ListItem it=new ListItem();
                    it.Text=remainder1[0];
                    it.Value=remainder1[0];
                    ddlRemainder1.SelectedIndex = ddlRemainder1.Items.IndexOf(it);
                    txtRemainder1.Text = remainder1[1];
                    ListItem itt=new ListItem();
                    itt.Text=remainder1[2];
                    itt.Value=remainder1[2];
                    ddltimeRemainder1.SelectedIndex = ddltimeRemainder1.Items.IndexOf(itt);                
                }

                if (citac["EventRemainder2"].ToString() != "")
                {

                    string[] remainder2 = citac["EventRemainder2"].ToString().Trim().Split(' ');
                     ListItem it=new ListItem();
                     it.Text = remainder2[0];
                     it.Value = remainder2[0];
                    ddlRemainder2.SelectedIndex = ddlRemainder2.Items.IndexOf(it);
                    txtRemainder2.Text = remainder2[1];
                    ListItem itt = new ListItem();
                    itt.Text = remainder2[2];
                    itt.Text = remainder2[2];
                    ddltimeRemainder2.SelectedIndex = ddltimeRemainder2.Items.IndexOf(itt);
                }

                

            }
            citac.Close();
        }
        catch (Exception ex) { lblMessage.Text = ex.ToString(); }
        finally { konekcija.Close(); }
    
    



    }







    protected void btnClearAll_Click(object sender, EventArgs e)
    {
        CleanFieldsInNewEvents();
        btnDeleteEvent.Enabled = false;
    }
   
}