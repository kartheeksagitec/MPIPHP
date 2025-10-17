using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sagitec.WebClient;

public partial class wfmMSSVideos : System.Web.UI.Page
{

    protected override void OnLoadComplete(EventArgs e)
    {
 
        if (Request.Browser.Type.Contains("Firefox"))
        {
            Media_Player_Control1.Visible = false;
            string strVideo = "<object classid='clsid:6BF52A52-394A-11D3-B153-00C04F79FAA6i' id='Player1'>";
            strVideo = strVideo + "<PARAM name='autoStart' value='False'>";
            strVideo = strVideo + "<PARAM name='URL' value='" + Framework.SessionForWindow["Movie URL"].ToString() + "'>";
            strVideo = strVideo + "<embed type='application/x-mplayer2' pluginspage='http://www.microsoft.com/Windows/Downloads/Contents/MediaPlayer/' width='500' height='500' src='" + Framework.SessionForWindow["Movie URL"].ToString() + "' filename='" + Framework.SessionForWindow["Movie URL"].ToString() + "' autostart='1' showcontrols='1' showstatusbar='1' showdisplay='1' >";
            strVideo = strVideo + "</object>";
            divVideo.InnerHtml = strVideo;
        }
        else
        {
            this.Media_Player_Control1.MovieURL = Framework.SessionForWindow["Movie URL"].ToString();
        }

        base.OnLoadComplete(e);

    }
}