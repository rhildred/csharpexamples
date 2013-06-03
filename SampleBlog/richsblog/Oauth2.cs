using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for Oauth2
/// </summary>
public class Oauth2
{
    WebClient conn = null;
    String sKey = null;
    String sSecretToken = null;
    String sRedirect = null;
    private HttpSessionStateBase Session;
    /// <summary>
    /// Name that has come back from Oauth2
    /// </summary>
    public String sName
    {
        get
        {
            return (String)this.Session["sName"];
        }
    }
    /// <summary>
    /// Google Id that has come back from oauth2 ... there won't be one unless logged in
    /// </summary>
    public String sGoogleId
    {
        get
        {
            return (String)this.Session["sGoogleId"];
        }
    }
    /// <summary>
    /// sets up a new Oauth2 object
    /// </summary>
    /// <param name="sClientId">Client ID from App Console</param>
    /// <param name="sSecret">Secret from App Console</param>
    /// <param name="sReturnUrl">where we return to after login</param>
    public Oauth2(String sClientId, String sSecret, String sReturnUrl, HttpSessionStateBase Session)
    {
        this.conn = new WebClient();
        this.sKey = sClientId;
        this.sSecretToken = sSecret;

        //get rid of any query params
        Regex rgxQuery = new Regex(@"\?.*$");
        this.sRedirect = rgxQuery.Replace(sReturnUrl, "");
        if (!this.sRedirect.Contains("localhost"))
        {
            Regex rgxPort = new Regex(@":\d\d*");
            this.sRedirect = rgxPort.Replace(this.sRedirect, "");
        }
        this.Session = Session;
    }


    /// <summary>
    /// does the initial redirect
    /// </summary>
    /// <param name="sReturnUrl">takes the url for google to return to</param>
    /// <param name="oResponse">takes the response object</param>
    public void Redirect(HttpResponseWrapper oResponse)
    {
        String sAuthUrl = "https://accounts.google.com/o/oauth2/auth?redirect_uri={0}&client_id={1}&scope=https://www.googleapis.com/auth/userinfo.profile&response_type=code&max_auth_age=0";
        String sRedirectToGoogle = String.Format(sAuthUrl, this.sRedirect, this.sKey);
        oResponse.Redirect(sRedirectToGoogle);
    }

    public void HandleCode(String sCode)
    {
        if (this.Session["sGoogleId"] == null)
        {
            //step 5
            // then google has redirected to us so build up query for 2nd phase of authentication
            NameValueCollection pairs = new NameValueCollection()
            {
                { "grant_type", "authorization_code" },
                { "client_id", this.sKey },
                { "client_secret", this.sSecretToken },
                { "code", sCode },
                { "redirect_uri", this.sRedirect }
            };
            //step 6
            byte[] response = conn.UploadValues("https://accounts.google.com/o/oauth2/token", pairs);

            // response is a byte array so need to convert it to a string
            String result = System.Text.Encoding.UTF8.GetString(response);
            var oResult = Json.Decode(result);
            String sAccessToken = oResult.access_token;

            // step 7
            // now we can get the user info
            String sUserInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={0}";
            String sInfo = conn.DownloadString(String.Format(sUserInfoUrl, sAccessToken));
            var oInfo = Json.Decode(sInfo);
            this.Session["sName"] = oInfo.name;
            this.Session["sGoogleId"] = oInfo.id;
        }
    }

    public void Close()
    {
        this.conn.Dispose();
    }
}
