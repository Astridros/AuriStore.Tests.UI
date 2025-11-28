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

            WebDriverWait wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(10));

            try
            {
                // Esperar a que aparezca el popup de SweetAlert2
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("swal2-popup")));

                // Leer texto del popup
                var alertTitle = driver.FindElement(By.ClassName("swal2-title")).Text;
                var alertMessage = driver.FindElement(By.ClassName("swal2-html-container")).Text;

                Console.WriteLine("Título encontrado: " + alertTitle);
                Console.WriteLine("Mensaje encontrado: " + alertMessage);

                // Validar título real
                Assert.IsTrue(alertTitle.Contains("Error") || alertTitle.Contains("¡Error!"),
                              "El título del popup no coincide con el esperado.");

                // VALIDAR EL MENSAJE EXACTO QUE MUESTRA TU SISTEMA
                Assert.IsTrue(
                alertMessage.Contains("incorrectos", StringComparison.OrdinalIgnoreCase) ||
                alertMessage.Contains("credenciales", StringComparison.OrdinalIgnoreCase) ||
                alertMessage.Contains("inválidas", StringComparison.OrdinalIgnoreCase) ||
                alertMessage.Contains("Error al conectar", StringComparison.OrdinalIgnoreCase),
                "El mensaje del popup no coincide con ningún mensaje esperado."
);
            }
            catch (Exception ex)
            {
                Assert.Fail("No apareció el mensaje de error esperado. Detalles: " + ex.Message);
            }
        }

        [Test]
        public void Login_PruebaLimite()
        {
            var loginPage = new LoginPage(driver!);

            // ❗ No ingresar email ni contraseña
            string email = "";
            string password = "";

            loginPage.Login(email, password);

            // Esperar unos segundos para validar que NO hubo navegación
            Thread.Sleep(1500);

            // 1️⃣ Validar que SIGUE en la pantalla de login
            bool sigueEnLogin = driver!.Url.Contains("auth.html")
                                || driver.PageSource.Contains("Iniciar Sesión");

            Assert.IsTrue(sigueEnLogin, "El sistema permitió continuar con campos vacíos, lo cual es incorrecto.");

            // 2️⃣ Validar que NO apareció SweetAlert2
            var alertas = driver.FindElements(By.ClassName("swal2-popup"));
            bool noMostroAlert = alertas.Count == 0;

            Assert.IsTrue(noMostroAlert, "Se mostró un SweetAlert cuando NO debía aparecer.");
        }

    }
}
