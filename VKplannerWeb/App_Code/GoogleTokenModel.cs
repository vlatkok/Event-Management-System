using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GoogleTokenModel
/// </summary>
/// //
public class GoogleTokenModel
{
    public string Access_Token { get; set; }
    public string Refresh_Token { get; set; }
    public string Expires_In { get; set; }
    public string Token_Type { get; set; }

    //public string AccessToken { get; set; }
    //public string RefreshToken { get; set; }
    //public DateTime LastAccessDateTime { get; set; }
}