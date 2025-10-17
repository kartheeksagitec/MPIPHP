using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using Sagitec.WebClient;

public partial class wfmChildChart : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if ((Request.Params["sfwIndex"] != null) &&
            (Request.Params["sfwObjectID"] != null) &&
            (Request.Params["sfwChildObjectID"] != null) &&
            (Request.Params["sfwChildXValue"] != null) &&
            (Request.Params["sfwChildYValue"] != null))
        {
            int lintIndex = Convert.ToInt32(Request.Params["sfwIndex"]);
            string lstrObjectID = Request.Params["sfwObjectID"];
            string lstrChildObjectID = Request.Params["sfwChildObjectID"];
            string lstrChildXValue = Request.Params["sfwChildXValue"];
            string lstrChildYValue = Request.Params["sfwChildYValue"];

            lstrObjectID = lstrObjectID.Substring(lstrObjectID.LastIndexOf('.') + 1);
            lstrChildObjectID = lstrChildObjectID.Substring(lstrChildObjectID.LastIndexOf('.') + 1);

            object lobjBase = Framework.SessionForWindow["CenterMiddle"];
            IList llstBase = (IList)DataBinder.Eval(lobjBase, lstrObjectID);
            IList llstChild = (IList)DataBinder.Eval(llstBase[lintIndex], lstrChildObjectID);

            object[] larrXValues = new object[llstChild.Count];
            object[] larrYValues = new object[llstChild.Count];

            int i = 0;
            foreach (object lobjChild in llstChild)
            {
                larrXValues[i] = DataBinder.Eval(lobjChild, lstrChildXValue);
                larrYValues[i] = DataBinder.Eval(lobjChild, lstrChildYValue);
                i++;
            }

            chrChildChart.Series[0].Points.DataBindXY(larrXValues, "XValue", larrYValues, "YValues");
            foreach (DataPoint dp in chrChildChart.Series[0].Points)
            {
                dp.Label = "#VAL";
                dp.LegendText = "#VALX : #VAL";
            }
        }

        if (Request.Params["sfwChildChartType"] != null)
        {
            SeriesChartType lenmChartType;
            string lstrChartType = Request.Params["sfwChildChartType"];
            if (Enum.TryParse<SeriesChartType>(lstrChartType, out lenmChartType))
            {
                chrChildChart.Series[0].ChartType = lenmChartType;
            }
        }

        chrChildChart.Legends.Add(new Legend());
        chrChildChart.Legends[0].Docking = Docking.Bottom;
        chrChildChart.ChartAreas[0].Area3DStyle.Enable3D = true;
    }
}
