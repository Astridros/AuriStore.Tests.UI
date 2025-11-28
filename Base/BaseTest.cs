using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace AuriStore.UI.Tests.Base
{
    public class BaseTest
    {
        protected IWebDriver driver;

        // URL base del login
        protected string baseUrl = "http://127.0.0.1:5500/html/auth.html";

        // Carpeta donde se guardarán capturas y reportes
        public static string ResultsPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "results"
        );

        [SetUp]
        public void SetUp()
        {
            // Crear carpeta results si no existe
            if (!Directory.Exists(ResultsPath))
                Directory.CreateDirectory(ResultsPath);

            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(baseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            // Nombre del test actual
            var testName = TestContext.CurrentContext.Test.Name;

            // Tomar captura automática
            TakeScreenshot(testName);

            driver.Quit();
        }

        // Método que toma la captura
        public void TakeScreenshot(string testName)
        {
            try
            {
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

                string filePath = Path.Combine(ResultsPath, $"{testName}.png");

                ss.SaveAsFile(filePath);

                Console.WriteLine($"📸 Captura guardada: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error tomando captura: " + ex.Message);
            }
        }
    }
}

