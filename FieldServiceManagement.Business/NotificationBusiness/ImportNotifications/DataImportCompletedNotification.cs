using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.EmailBusiness;
using FieldServiceManagement.Enum;
using System.Net;
using System.Text;

namespace FieldServiceManagement.Business.NotificationBusiness.ImportNotifications
{
    // One row of the summary table: one uploaded file
    public record ImportFileSummary(string Module, string FileName, int Records, int Added, int Updated, int Skipped);

    // The summary email sent when a data import finishes (or fails)
    public class DataImportCompletedNotification : EmailNotification
    {
        private readonly string _name;
        private readonly string _organisation;
        private readonly string _module;
        private readonly DateTime _initiatedAt;
        private readonly DataImportStatus _status;
        private readonly List<ImportFileSummary> _files;

        public DataImportCompletedNotification(string email, string name, string organisation, string module,
            DateTime initiatedAt, DataImportStatus status, List<ImportFileSummary> files)
            : base(status == DataImportStatus.Completed ? "OTT FSM | Data Import has been completed." : "OTT FSM | Data Import failed.", email)
        {
            _name = name;
            _organisation = organisation;
            _module = module;
            _initiatedAt = initiatedAt;
            _status = status;
            _files = files;
        }

        protected override string MergeEmailTemplate()
        {
            var template = GetTemplate();

            template = template.Replace("#Title#", _subject);
            template = template.Replace("#year#", DateTime.Now.Year.ToString());
            template = template.Replace("#Content#", EmailContent());

            return template;
        }

        protected override void PopulateSendNotification() { }

        private string EmailContent()
        {
            var historyUrl = $"{AppSettings.baseUrl.TrimEnd('/')}/ImportHistory?module={Uri.EscapeDataString(_module)}";
            var date = $"{_initiatedAt:MMM dd, yyyy hh:mm} {_initiatedAt.ToString("tt").ToUpper()}";

            var sb = new StringBuilder();
            sb.Append($"<p><strong>Dear {WebUtility.HtmlEncode(_name)},</strong></p>");

            if (!string.IsNullOrWhiteSpace(_organisation))
                sb.Append($"<p><strong>Organization :</strong> {WebUtility.HtmlEncode(_organisation)}</p>");

            // Failed: no table, because nothing reliable can be reported per file
            if (_status != DataImportStatus.Completed)
            {
                sb.Append($"<p>The data import initiated by you on {date} could not be completed. Please check your files and try again. If it keeps failing, contact support.</p>");
                sb.Append($"<p><a href='{historyUrl}' style=\"color: #2e7d32; text-decoration: none;\">View Import History</a></p>");
                return sb.ToString();
            }

            sb.Append($"<p>The data import initiated by you on {date} has been completed and below is the summary of the import.</p>");

            const string th = "border:1px solid #e0e0e0;padding:14px 12px;font-weight:bold;text-align:center;";
            const string td = "border:1px solid #e0e0e0;padding:14px 12px;text-align:center;color:#8a8a8a;";

            sb.Append("<table style=\"border-collapse:collapse;border:1px solid #e0e0e0;font-size:13px;margin:16px 0;\">");
            sb.Append("<tr>");
            sb.Append($"<th style=\"{th}\">Module</th>");
            sb.Append($"<th style=\"{th}\">Status</th>");
            sb.Append($"<th style=\"{th}\">Total records in file</th>");
            sb.Append($"<th style=\"{th}\">Added</th>");
            sb.Append($"<th style=\"{th}\">Updated</th>");
            sb.Append($"<th style=\"{th}\">Skipped</th>");
            sb.Append("</tr>");

            foreach (var f in _files)
            {
                sb.Append("<tr>");
                sb.Append($"<td style=\"{td}color:#000;\"><strong>{WebUtility.HtmlEncode(f.Module)}</strong><br/>" +
                          $"<span style=\"font-size:12px;word-break:break-all;\">{WebUtility.HtmlEncode(f.FileName)}</span></td>");
                sb.Append($"<td style=\"{td}\">{_status}</td>");
                sb.Append($"<td style=\"{td}\">{f.Records:N0}</td>");
                sb.Append($"<td style=\"{td}\">{f.Added:N0}</td>");
                sb.Append($"<td style=\"{td}\">{f.Updated:N0}</td>");
                sb.Append($"<td style=\"{td}\">{f.Skipped:N0}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");

            sb.Append("<p>To view the list of records skipped during the import, along with the reason for each, " +
                      $"go to <a href='{historyUrl}' style=\"color: #2e7d32; text-decoration: none;\">Import History</a> and download the error report.</p>");

            sb.Append("<p>Feel free to reach out to us if you have any queries.</p>");

            return sb.ToString();
        }
    }
}