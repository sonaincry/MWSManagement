using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.UI;

namespace Report.Reports
{
    public class ReportConnectionHelper
    {
        public static void ApplyConnection(XtraReport report, string connectionString)
        {
            foreach (var item in report.ComponentStorage)
            {
                if (item is SqlDataSource ds)
                {
                    ds.ConnectionParameters =
                        new CustomStringConnectionParameters(connectionString);
                }
            }
        }
    }
}
