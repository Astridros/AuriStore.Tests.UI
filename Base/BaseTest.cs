using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace AuriStore.UI.Tests.Base
{
    public class BaseTest
    {
        public static List<(string name, string status)> TestResults = new();

        protected IWebDriver driver;

        public static readonly string ResultsPath = Path.Combine(
            Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "Results"
        );

        protected string baseUrl = "http://127.0.0.1:5500/html/auth.html";

        [SetUp]
        public void SetUp()
        {
            if (!Directory.Exists(ResultsPath))
                Directory.CreateDirectory(ResultsPath);

            var options = new ChromeOptions();

            options.AcceptInsecureCertificates = true;
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--disable-site-isolation-trials");

            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);

            driver.Navigate().GoToUrl(baseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            string testName = TestContext.CurrentContext.Test.Name;
            bool failed = TestContext.CurrentContext.Result.FailCount > 0;

            TakeScreenshot(testName);

            TestResults.Add((testName, failed ? "FALLIDA" : "APROBADA"));

            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null;
            }
        }

        private void TakeScreenshot(string testName)
        {
            try
            {
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                string filePath = Path.Combine(ResultsPath, $"{testName}.png");
                ss.SaveAsFile(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠ Error al tomar captura: " + ex.Message);
            }
        }
    }


}
