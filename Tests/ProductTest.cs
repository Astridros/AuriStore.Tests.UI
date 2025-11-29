using AuriStore.UI.Tests.Pages;
using AuriStore.UI.Tests.Base;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace AuriStore.UI.Tests.Tests
{
    public class ProductTests : BaseTest
    {
        [Test]
        public void CrearProducto_CaminoFeliz()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            productPage.GoToProductsSection();
            productPage.OpenNewProductModal();

            string nombre = "Headset Selenium Test";
            string descripcion = "Audífonos creados desde prueba automatizada";
            string precio = "199.99";
            string stock = "10";
            string categoriaValue = "1";
            string imagen = "https://via.placeholder.com/150";

            productPage.FillProductForm(nombre, descripcion, precio, stock, categoriaValue, imagen);
            productPage.SaveProductExpectSuccess();

            var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(15));
            wait.Until(d => productPage.ExistsProductInTable(nombre));

            Assert.IsTrue(productPage.ExistsProductInTable(nombre), "El producto NO apareció en la tabla después de crearlo.");
        }

        [Test]
        public void CrearProducto_PruebaNegativa()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            productPage.GoToProductsSection();
            productPage.OpenNewProductModal();

            productPage.FillProductForm("", "", "", "", "", "");
            productPage.SaveProductExpectError();

            var alertTitle = driver!.FindElement(By.ClassName("swal2-title")).Text;
            var alertMessage = driver!.FindElement(By.ClassName("swal2-html-container")).Text;

            Assert.That(alertMessage.Contains("Error", StringComparison.OrdinalIgnoreCase) ||
                        alertTitle.Contains("Error", StringComparison.OrdinalIgnoreCase),
                        $"Se esperaba mensaje de error pero apareció: {alertMessage}");
        }

        [Test]
        public void CrearProducto_PruebaLimite()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            productPage.GoToProductsSection();
            productPage.OpenNewProductModal();

            string nombre = "Producto Límite";
            string descripcion = "Prueba con valores inválidos";
            string precio = "abc";   
            string stock = "xyz";    
            string categoriaValue = "1";
            string imagen = "https://via.placeholder.com/150";

            productPage.FillProductForm(nombre, descripcion, precio, stock, categoriaValue, imagen);
            productPage.SaveProductExpectError();

            var alertMessage = driver!.FindElement(By.ClassName("swal2-html-container")).Text;
            Assert.IsTrue(alertMessage.Contains("error", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void EditarProducto_CaminoFeliz()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            productPage.GoToProductsSection();

            string nombreOriginal = "Sony WH-1000XM4";
            
            if (!productPage.ExistsProductInTable(nombreOriginal))
            {
                productPage.OpenNewProductModal();
                productPage.FillProductForm(
                    nombreOriginal,
                    "Over-ear premium con ANC.",
                    "299.99",
                    "15",
                    "1",
                    "https://via.placeholder.com/150"
                );
                productPage.SaveProductExpectSuccess();

                var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(15));
                wait.Until(d => productPage.ExistsProductInTable(nombreOriginal));
            }

            productPage.OpenEditProductModal(nombreOriginal);

            string nuevoNombre = "Sony WH-1000XM4 Editado";
            productPage.FillProductForm(
                nuevoNombre,
                "Over-ear premium con ANC, versión editada",
                "279.99",
                "20",
                "1",
                "https://via.placeholder.com/200"
            );

            productPage.SaveEditedProductExpectSuccess();

            Assert.IsTrue(productPage.ExistsProductInTable(nuevoNombre), "El producto editado NO apareció en la tabla.");
        }

        [Test]
        public void EditarProducto_PruebaNegativa()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            ((IJavaScriptExecutor)driver).ExecuteScript("localStorage.setItem('testMode', 'true');");
            productPage.GoToProductsSection();

            string nombreOriginal = "Bose QuietComfort 45";

            productPage.OpenEditProductModal(nombreOriginal);

            productPage.ClearProductCategory();

            productPage.SaveEditedProductExpectError();

            var alertTitle = driver.FindElement(By.ClassName("swal2-title")).Text;
            Assert.IsTrue(alertTitle.Contains("Error", StringComparison.OrdinalIgnoreCase));
        }


        [Test]
        public void EditarProducto_PruebaLimite()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            productPage.GoToProductsSection();

            string nombreOriginal = "Sony WH-1000XM4";
            if (!productPage.ExistsProductInTable(nombreOriginal))
            {
                productPage.OpenNewProductModal();
                productPage.FillProductForm(
                    nombreOriginal,
                    "Over-ear premium con ANC.",
                    "299.99",
                    "15",
                    "1",
                    "https://via.placeholder.com/150"
                );
                productPage.SaveProductExpectSuccess();

                var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(15));
                wait.Until(d => productPage.ExistsProductInTable(nombreOriginal));
            }

            productPage.OpenEditProductModal(nombreOriginal);

            productPage.FillProductForm(
                "Sony WH-1000XM4 Límite",
                "Descripción con precio inválido",
                "abc",   
                "10",
                "1",
                "https://via.placeholder.com/200"
            );

            productPage.SaveEditedProductExpectError();

            var alertMessage = driver!.FindElement(By.ClassName("swal2-html-container")).Text;
            Assert.That(alertMessage.Contains("Error", StringComparison.OrdinalIgnoreCase),
                        $"Se esperaba mensaje de error pero apareció: {alertMessage}");
        }

        [Test]
        public void EliminarProducto_CaminoFeliz()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            productPage.GoToProductsSection();

            string nombreProducto = "Sennheiser HD 450BT";

            Assert.IsTrue(
                productPage.ExistsProductInTable(nombreProducto),
                $"El producto '{nombreProducto}' debe existir previamente para esta prueba."
            );

            productPage.OpenDeleteProductModal(nombreProducto);

            productPage.ConfirmDeleteProductExpectSuccess();

            var waitDelete = new WebDriverWait(driver!, TimeSpan.FromSeconds(15));
            waitDelete.Until(d => !productPage.ExistsProductInTable(nombreProducto));

            Assert.IsFalse(
                productPage.ExistsProductInTable(nombreProducto),
                "El producto todavía aparece en la tabla después de eliminarlo."
            );
        }


        [Test]
        public void EliminarProducto_PruebaNegativa()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            productPage.GoToProductsSection();

            string nombreProducto = "Beats Studio3";

            Assert.IsTrue(
                productPage.ExistsProductInTable(nombreProducto),
                $"El producto '{nombreProducto}' debe existir previamente para esta prueba."
            );

            productPage.OpenDeleteProductModal(nombreProducto);

            productPage.CancelDeleteProduct();

            Assert.IsTrue(
                productPage.ExistsProductInTable(nombreProducto),
                "El producto NO debería haberse eliminado al cancelar."
            );
        }

        [Test]
        public void EliminarProducto_PruebaLimite()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            productPage.GoToProductsSection();

            string nombreProducto = "ProductoInexistenteXYZ";

            Assert.Throws<Exception>(() => productPage.OpenDeleteProductModal(nombreProducto),
                "Se esperaba excepción al intentar eliminar un producto inexistente.");
        }

        [Test]
        public void ListarProductos_CaminoFeliz()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            productPage.GoToProductsSection();

            var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(15));
            wait.Until(d =>
                driver!.FindElements(By.CssSelector("#productsTableBody tr")).Count > 0
            );

            int totalFilas = driver!.FindElements(By.CssSelector("#productsTableBody tr")).Count;

            Assert.Greater(totalFilas, 0, "La tabla debe contener productos cargados.");
        }

        [Test]
        public void ListarProductos_PruebaNegativa()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();
            productPage.GoToProductsSection();

            ((IJavaScriptExecutor)driver).ExecuteScript("localStorage.setItem('testModeListarError', 'true');");

            driver.FindElement(By.CssSelector("a.nav-link[data-section='products']")).Click();

            Thread.Sleep(2000);

            bool alertaVisible = false;
            try
            {
                driver.FindElement(By.ClassName("swal2-container"));
                alertaVisible = true;
            }
            catch { }

            string pageSource = driver.PageSource.ToLower();
            bool textoError = pageSource.Contains("error") || pageSource.Contains("hubo un error");

            Assert.IsTrue(alertaVisible || textoError,
                "El sistema debería mostrar algún tipo de error al fallar la carga de productos.");
        }

        [Test]
        public void ListarProductos_PruebaLimite()
        {
            var productPage = new ProductPage(driver!);

            productPage.LoginAsAdmin();

            ((IJavaScriptExecutor)driver).ExecuteScript("localStorage.setItem('testModeListarVacio','true');");

            productPage.GoToProductsSection();

            var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(10));
            wait.Until(d =>
                driver!.FindElements(By.CssSelector("#productsTableBody tr")).Count == 0
            );

            int filas = driver!.FindElements(By.CssSelector("#productsTableBody tr")).Count;

            Assert.AreEqual(0, filas, "La tabla debería estar vacía cuando no hay productos.");
        }
    }


}


