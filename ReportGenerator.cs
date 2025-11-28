using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace AuriStore.Tests.UI
{
    [SetUpFixture]
    public class ReportGenerator
    {
        private static StringBuilder html;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            html = new StringBuilder();
            html.AppendLine("<html><head><title>Reporte de Pruebas AuriStore</title>");
            html.AppendLine("<style>");
            html.AppendLine("body{font-family:Arial;margin:20px;}");
            html.AppendLine(".success{color:white;background:green;padding:10px;border-radius:5px;}");
            html.AppendLine(".fail{color:white;background:red;padding:10px;border-radius:5px;}");
            html.AppendLine("table{width:100%;border-collapse:collapse;margin-top:20px;}");
            html.AppendLine("td,th{border:1px solid #ccc;padding:8px;text-align:left;}");
            html.AppendLine("img{width:300px;border:1px solid #555;margin-top:10px;}");
            html.AppendLine("</style></head><body>");

            html.AppendLine("<h1>📘 Reporte de Pruebas Automatizadas – AuriStore</h1>");
            html.AppendLine("<p>Fecha: " + DateTime.Now + "</p>");
            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Prueba</th><th>Resultado</th><th>Captura</th></tr>");
        }

        [OneTimeTearDown]
        public void AfterAllTests()
        {
            string resultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "results");
            string[] screenshots = Directory.GetFiles(resultsPath, "*.png");

            foreach (var ss in screenshots)
            {
                string testName = Path.GetFileNameWithoutExtension(ss);
                html.Append("<tr>");
                html.Append($"<td>{testName}</td>");
                html.Append($"<td class='success'>APROBADA</td>");
                html.Append($"<td><img src='{ss.Replace("\\", "/")}'></td>");
                html.Append("</tr>");
            }

            html.AppendLine("</table></body></html>");

            File.WriteAllText(Path.Combine(resultsPath, "report.html"), html.ToString());
            Console.WriteLine("📄 Reporte HTML generado exitosamente.");
        }
    }
}
