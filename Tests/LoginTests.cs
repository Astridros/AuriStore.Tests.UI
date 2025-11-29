using AuriStore.UI.Tests.Base;
using AuriStore.UI.Tests.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
namespace AuriStore.UI.Tests.Tests
{
    public class LoginTests : BaseTest
    {
        [Test]
        public void Login_CaminoFeliz()
        {
            var loginPage = new LoginPage(driver!);

            string email = "roselin@gmail.com";
            string password = "1234567";

            loginPage.Login(email, password);

            WebDriverWait wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(10));

            bool loginCorrecto = false;

            try
            {
                loginCorrecto = wait.Until(driver =>
                    driver.Url.Contains("admin.html") ||
                    driver.PageSource.Contains("Panel Administrativo") ||
                    driver.PageSource.Contains("Dashboard") ||
                    driver.PageSource.Contains("Gestión de Usuarios") ||
                    driver.PageSource.Contains("AuriStore")
                );
            }
            catch (Exception)
            {
                loginCorrecto = false;
            }

            Console.WriteLine("URL actual: " + driver!.Url);

            Assert.IsTrue(loginCorrecto, "Selenium NO detectó la navegación hacia el panel administrador.");
        }

        [Test]
        public void Login_PruebaNegativa()
        {
            var loginPage = new LoginPage(driver!);

            string email = "falso@gmail.com";
            string password = "falsa";

            loginPage.Login(email, password);

            var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(10));

            bool loginFallido = wait.Until(d =>
            {
                var alerts = d.FindElements(By.ClassName("swal2-popup"));
                if (alerts.Any(a => a.Displayed))
                    return true;

                return !d.Url.Contains("admin.html");
            });

            Assert.IsTrue(loginFallido, "❌ No apareció mensaje de error y/o el sistema permitió el acceso.");
        }
        

        [Test]
        public void Login_PruebaLimite()
        {
            var loginPage = new LoginPage(driver!);

            string email = "";
            string password = "";

            loginPage.Login(email, password);

            Thread.Sleep(1500);

            bool sigueEnLogin = driver!.Url.Contains("auth.html")
                                || driver.PageSource.Contains("Iniciar Sesión");

            Assert.IsTrue(sigueEnLogin, "El sistema permitió continuar con campos vacíos, lo cual es incorrecto.");

            
            var alertas = driver.FindElements(By.ClassName("swal2-popup"));
            bool noMostroAlert = alertas.Count == 0;

            Assert.IsTrue(noMostroAlert, "Se mostró un SweetAlert cuando NO debía aparecer.");
        }

    }
}
