using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using AuriStore.UI.Tests.Base;   
namespace AuriStore.UI.Tests
{
    [SetUpFixture]
    public class ReportGenerator
    {
        private static StringBuilder html = new StringBuilder();
        private static DateTime startTime;

        public static readonly string ResultsPath = Path.Combine(
            Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "Results"
        );

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            startTime = DateTime.Now;

            if (!Directory.Exists(ResultsPath))
                Directory.CreateDirectory(ResultsPath);

            html.AppendLine("<html><head><title>Reporte de Pruebas – AuriStore</title>");
            html.AppendLine("<style>");
            html.AppendLine("body{font-family:Arial;background:#f3f4f6;margin:20px;}");
            html.AppendLine(".section{background:white;padding:20px;border-radius:10px;margin-bottom:20px;}");
            html.AppendLine(".success{color:white;background:#16a34a;padding:6px;border-radius:6px;text-align:center;}");
            html.AppendLine(".fail{color:white;background:#dc2626;padding:6px;border-radius:6px;text-align:center;}");
            html.AppendLine("table{width:100%;border-collapse:collapse;margin-top:20px;}");
            html.AppendLine("td,th{border:1px solid #ccc;padding:10px;background:white;}");
            html.AppendLine("img{width:300px;border:1px solid #444;border-radius:8px;margin-top:10px;}");
            html.AppendLine("</style></head><body>");

            html.AppendLine("<div class='section'>");
            html.AppendLine("<h1>📘 Reporte de Pruebas Automatizadas – AuriStore</h1>");
            html.AppendLine($"<p><b>Fecha:</b> {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            html.AppendLine("<p><b>Proyecto:</b> AuriStore – Sistema de Gestión</p>");
            html.AppendLine("<p><b>Probado por:</b> Astrid Roselin Rondón Berroa</p>");
            html.AppendLine("<p><b>Asignatura:</b> Pruebas Automatizadas</p>");
            html.AppendLine("</div>");
        }

        [OneTimeTearDown]
        public void AfterAllTests()
        {
            var results = BaseTest.TestResults;   
            Console.WriteLine($"Resultados capturados: {results.Count}"); 

            TimeSpan duration = DateTime.Now - startTime;

            int total = results.Count;
            int passed = results.Count(r => r.status == "APROBADA");
            int failed = results.Count(r => r.status == "FALLIDA");
            double successRate = total > 0 ? (passed * 100.0 / total) : 0;

            html.AppendLine("<div class='section'>");
            html.AppendLine("<h2>📊 Resumen General</h2>");
            html.AppendLine($"<p><b>Total de pruebas ejecutadas:</b> {total}</p>");
            html.AppendLine($"<p><b>Pruebas aprobadas:</b> {passed}</p>");
            html.AppendLine($"<p><b>Pruebas fallidas:</b> {failed}</p>");
            html.AppendLine($"<p><b>Porcentaje de éxito:</b> {successRate:F1}%</p>");
            html.AppendLine($"<p><b>Duración total:</b> {duration.TotalSeconds:F1} segundos</p>");
            if (total > 0)
                html.AppendLine($"<p><b>Tiempo promedio por prueba:</b> {(duration.TotalSeconds / total):F2} segundos</p>");
            html.AppendLine("</div>");

            html.AppendLine("<div class='section'>");
            html.AppendLine("<h2>🖥️ Información del Entorno</h2>");
            html.AppendLine($"<p><b>Sistema operativo:</b> {Environment.OSVersion}</p>");
            html.AppendLine($"<p><b>.NET Version:</b> {Environment.Version}</p>");
            html.AppendLine($"<p><b>Navegador:</b> Google Chrome</p>");
            html.AppendLine($"<p><b>Driver:</b> ChromeDriver (Selenium)</p>");
            html.AppendLine($"<p><b>URL de pruebas:</b> http://127.0.0.1:5500/html/auth.html</p>");
            html.AppendLine("</div>");

            html.AppendLine("<div class='section'>");
            html.AppendLine("<h2>📸 Resultados por Prueba</h2>");
            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Prueba</th><th>Resultado</th><th>Captura</th></tr>");

            foreach (var r in results)
            {
                string screenshotPath = Path.Combine(ResultsPath, $"{r.name}.png").Replace("\\", "/");
                string statusClass = r.status == "APROBADA" ? "success" : "fail";

                html.AppendLine("<tr>");
                html.AppendLine($"<td>{r.name}</td>");
                html.AppendLine($"<td class='{statusClass}'>{r.status}</td>");

                if (File.Exists(Path.Combine(ResultsPath, $"{r.name}.png")))
                    html.AppendLine($"<td><img src='{screenshotPath}'></td>");
                else
                    html.AppendLine("<td>Sin captura</td>");

                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
            html.AppendLine("</div>");

            File.WriteAllText(Path.Combine(ResultsPath, "report.html"), html.ToString());
        }
    }
}

