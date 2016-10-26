using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Google.GData.Client;
using Google.GData.Spreadsheets;

using Newtonsoft.Json;



namespace WebApplication1
{
    public partial class _Default : System.Web.UI.Page
    {
        //spreadsheet things;
        string SCOPE = "https://spreadsheets.google.com/feeds";
        string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                aToken.Enabled = false;
                Button2.Visible = false;
                clientID.Text = "757756100-aub4ekgnendipaqlns316qon9655boqt.apps.googleusercontent.com";
                clientSecret.Text = "t-hwOxoLUIaA602vW0wdBQTd";
                sID.Text = "Rapid";
                sToken.Text = "https://hooks.slack.com/services/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";
                cID.Text = "rapid_lions";
            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //dealing with spreadsheets;
            OAuth2Parameters parameters = new OAuth2Parameters();
            parameters.ClientId = clientID.Text.Trim();
            parameters.ClientSecret = clientSecret.Text.Trim();
            parameters.RedirectUri = REDIRECT_URI;
            parameters.Scope = SCOPE;

            //Disable button 1 and enable button 2;
            Button1.Visible = false;
            Button2.Visible = true;

            //disable all text boxes except for the access token.
            uStory.Enabled = false;
            owner.Enabled = false;
            clientID.Enabled = false;
            clientSecret.Enabled = false;
            sID.Enabled = false;
            sToken.Enabled = false;
            cID.Enabled = false;
            aToken.Enabled = true;
            try
            {
                //authorization using OAuth2;
                string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);

                msg.Text = @"Please allow the App to get required details and paste the access token from the new tab 
                                            to process further.Also please make sure that you do not refresh the page";

                //opean a new tab with the hyperlink for the access token;
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('" + authorizationUrl + "');", true);
            }
            catch (Exception)
            {
                msg.Text = "Error occured while fetching access token";
            }


        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            OAuth2Parameters parameters = new OAuth2Parameters();
            parameters.ClientId = clientID.Text.Trim();
            parameters.ClientSecret = clientSecret.Text.Trim();
            parameters.RedirectUri = REDIRECT_URI;
            parameters.Scope = SCOPE;
            parameters.AccessCode = aToken.Text.Trim();
            try
            {
                //dealing with slack;
                string urlWithAccessToken = sToken.Text.Trim();
                SlackClient client = new SlackClient(urlWithAccessToken);
                client.PostMessage(username: owner.Text.Trim(), text: uStory.Text.Trim(), channel: "#" + cID.Text.Trim());

                //Now Deal with spreadsheet.
                OAuthUtil.GetAccessToken(parameters);
                string accessToken = parameters.AccessToken;
            
                GOAuth2RequestFactory requestFactory =new GOAuth2RequestFactory(null, "MySpreadsheetIntegration-v1", parameters);
                SpreadsheetsService service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
                service.RequestFactory = requestFactory;

                SpreadsheetQuery query = new SpreadsheetQuery();
                query.Title = sID.Text.Trim();

                // Make a request to the API and get the spreadsheet;
                SpreadsheetFeed feed = service.Query(query);
                if (feed.Entries.Count != 1)
                {
                    msg.Text = "The Mentioned Spreadsheet does not exist";
                    return;
                }
            
                SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
                
                WorksheetFeed wsFeed = spreadsheet.Worksheets;
                WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries[0];

                AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
                ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
                ListFeed listFeed = service.Query(listQuery);

                // Create a local representation of the new row.
                ListEntry row = new ListEntry();
                row.Elements.Add(new ListEntry.Custom() { LocalName = "userstory", Value = uStory.Text.Trim() });
                row.Elements.Add(new ListEntry.Custom() { LocalName = "owner", Value = owner.Text.Trim() });
                try
                {
                    // Send the new row to the API for insertion.
                    service.Insert(listFeed, row);
                    //Disable button 1 and enable button 2.
                    Button1.Visible = true;
                    Button2.Visible = false;

                    //disable all text boxes except for the access token.
                    uStory.Enabled = true;
                    owner.Enabled = true;
                    clientID.Enabled = true;
                    clientSecret.Enabled = true;
                    sID.Enabled = true;
                    sToken.Enabled = true;
                    cID.Enabled = true;
                    aToken.Enabled = true;

                    //clear all textboxes
                    uStory.Text = owner.Text = sToken.Text = "";

                    msg.Text = "Submitted Successfully";
                }
                catch (Exception)
                {
                    msg.Text = "Error while sending data to spreadsheet.";
                }
            }
            catch (Exception)
            {
                msg.Text = "Some error occured.";
            }
        }
    }

    public class SlackClient
    {
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();

        public SlackClient(string urlWithAccessToken)
        {
            _uri = new Uri(urlWithAccessToken);
        }

        //Post a message using simple strings;
        public void PostMessage(string text, string username = null, string channel = null)
        {
            Payload payload = new Payload()
            {
                Channel = channel,
                Username = username,
                Text = text
            };

            PostMessage(payload);
        }

        //Post a message using a Payload object
        public void PostMessage(Payload payload)
        {
            string payloadJson = JsonConvert.SerializeObject(payload);

            using (WebClient client = new WebClient())
            {
                NameValueCollection data = new NameValueCollection();
                data["payload"] = payloadJson;

                var response = client.UploadValues(_uri, "POST", data);

                //The response text is usually "ok"
                string responseText = _encoding.GetString(response);
            }
        }
    }

    //This class serializes into the Json payload required by Slack Incoming WebHooks;
    public class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
