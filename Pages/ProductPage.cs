using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace AuriStore.UI.Tests.Pages
{
    public class ProductPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public ProductPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        public void LoginAsAdmin()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5500/html/auth.html");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("loginEmail")));

            driver.FindElement(By.Id("loginEmail")).SendKeys("roselin@gmail.com");
            driver.FindElement(By.Id("loginPassword")).SendKeys("1234567");
            driver.FindElement(By.Id("loginBtn")).Click();

            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("swal2-container")));
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("⚠ No se detectó SweetAlert.");
            }

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".sidebar")));
        }

        public void GoToProductsSection()
        {
            try
            {
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(
                    By.ClassName("swal2-container")
                ));
            }
            catch { }

            var productsLink = wait.Until(
                ExpectedConditions.ElementToBeClickable(
                    By.CssSelector("a.nav-link[data-section='products']")
                )
            );

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", productsLink);

            try
            {
                productsLink.Click();
            }
            catch (ElementClickInterceptedException)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", productsLink);
            }

            wait.Until(ExpectedConditions.ElementExists(By.Id("productsTableBody")));

            Console.WriteLine("✅ Se cargó la sección de productos.");
        }

        private IWebElement GetProductsTable()
        {
            return wait.Until(ExpectedConditions.ElementExists(
                By.CssSelector("tbody[data-product-table='true']")));
        }

        private IWebElement GetRowByProductName(string productName)
        {
            var table = GetProductsTable();
            var rows = table.FindElements(By.TagName("tr"));

            foreach (var row in rows)
            {
                if (row.Text.Contains(productName, StringComparison.OrdinalIgnoreCase))
                    return row;
            }

            throw new Exception($"❌ No se encontró producto '{productName}' en la tabla.");
        }

        public void OpenNewProductModal()
        {
            var btn = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.CssSelector("button[data-bs-target='#productModal']")));

            btn.Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("productModal")));
        }

        public void OpenEditProductModal(string productName)
        {
            var row = GetRowByProductName(productName);

            var editButton = row.FindElement(By.CssSelector("button.btn-info"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", editButton);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", editButton);

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("productModal")));
        }

        public void OpenDeleteProductModal(string productName)
        {
            var row = GetRowByProductName(productName);

            var deleteBtn = row.FindElement(By.CssSelector("button.btn-danger"));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", deleteBtn);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteBtn);

            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("swal2-container")));
        }

        public void ClearProductCategory()
        {
            var selectElem = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("productCategory")));

            wait.Until(d => selectElem.FindElements(By.TagName("option")).Count > 0);

            var select = new SelectElement(selectElem);

            try { select.SelectByValue(""); }
            catch { select.SelectByIndex(0); }
        }

        public void FillProductForm_IgnoreSelect(string nombre, string descripcion, string precio, string stock, string imagen)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("productForm")));

            var name = driver.FindElement(By.Id("productName"));
            var desc = driver.FindElement(By.Id("productDescription"));
            var price = driver.FindElement(By.Id("productPrice"));
            var st = driver.FindElement(By.Id("productStock"));
            var img = driver.FindElement(By.Id("productImage"));

            name.Clear(); desc.Clear(); price.Clear(); st.Clear(); img.Clear();

            if (!string.IsNullOrEmpty(nombre)) name.SendKeys(nombre);
            if (!string.IsNullOrEmpty(descripcion)) desc.SendKeys(descripcion);
            if (!string.IsNullOrEmpty(precio)) price.SendKeys(precio);
            if (!string.IsNullOrEmpty(stock)) st.SendKeys(stock);
            if (!string.IsNullOrEmpty(imagen)) img.SendKeys(imagen);
        }

        public void FillProductForm(string nombre, string descripcion, string precio, string stock, string categoriaValue, string imagen)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("productForm")));

            void Set(string id, string txt)
            {
                if (txt == null) return;
                var el = driver.FindElement(By.Id(id));
                el.Clear();
                el.SendKeys(txt);
            }

            Set("productName", nombre);
            Set("productDescription", descripcion);
            Set("productPrice", precio);
            Set("productStock", stock);
            Set("productImage", imagen);

            if (categoriaValue != null)
            {
                var select = new SelectElement(driver.FindElement(By.Id("productCategory")));

                if (categoriaValue == "")
                    select.SelectByIndex(0);
                else
                    select.SelectByValue(categoriaValue);
            }
        }

        public void SaveProductExpectSuccess()
        {
            var wait = new WebDriverWait(driver!, TimeSpan.FromSeconds(15));

            wait.Until(d =>
            {
                var modal = d.FindElement(By.Id("productModal"));
                return modal.Displayed && modal.GetAttribute("class").Contains("show");
            });

            IWebElement saveBtn = null;

            try
            {
                saveBtn = driver.FindElement(By.CssSelector("button[onclick='saveProduct()']"));
            }
            catch { }

            if (saveBtn == null)
            {
                try
                {
                    saveBtn = driver.FindElement(By.CssSelector("#productModal .btn-primary-custom"));
                }
                catch { }
            }

            if (saveBtn == null)
            {
                try
                {
                    saveBtn = driver.FindElements(By.TagName("button"))
                                    .FirstOrDefault(b => b.Text.Trim().ToLower() == "guardar");
                }
                catch { }
            }

            if (saveBtn == null)
                throw new Exception("❌ No se pudo encontrar el botón Guardar dentro del modal.");

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", saveBtn);

            saveBtn.Click();

            try
            {
                
                wait.Until(d => d.FindElement(By.ClassName("swal2-container")).Displayed);

                
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                    .InvisibilityOfElementLocated(By.ClassName("swal2-container")));
            }
            catch
            {
                wait.Until(d =>
                {
                    var modal = d.FindElement(By.Id("productModal"));
                    return !modal.GetAttribute("class").Contains("show");
                });
            }
        }

        public void SaveProductExpectError()
        {
            var saveBtn = wait.Until(
                ExpectedConditions.ElementToBeClickable(
                    By.CssSelector(".modal-footer .btn.btn-primary-custom")
                )
            );

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", saveBtn);
            saveBtn.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("swal2-container")));
        }

        public void SaveEditedProductExpectSuccess()
        {
            var saveBtn = wait.Until(
                ExpectedConditions.ElementToBeClickable(
                    By.CssSelector(".modal-footer .btn.btn-primary-custom")
                )
            );

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", saveBtn);
            saveBtn.Click();

            // SweetAlert aparece y luego desaparece
            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("swal2-container")));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("swal2-container")));
        }

        public void ForceClickGuardar()
        {
            var saveBtn = wait.Until(
                ExpectedConditions.ElementExists(
                    By.CssSelector(".modal-footer .btn.btn-primary-custom")
                )
            );

            // Forzar clic por JS, evitando overlays
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", saveBtn);
        }

        // ⇩ 3. Guardar producto EDITADO esperando error
        public void SaveEditedProductExpectError()
        {
            // 🔥 Forzar clic en guardar (evita el bloqueo del overlay)
            ForceClickGuardar();

            // Esperar la alerta (sí aparece cuando se da clic real)
            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("swal2-container")));
        }

        /* ===========================================================
         *                  ELIMINAR
         * =========================================================== */
        public void ConfirmDeleteProductExpectSuccess()
        {
            var btn = wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("swal2-confirm")));
            btn.Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("swal2-container")));
        }

        public void CancelDeleteProduct()
        {
            var btn = wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("swal2-cancel")));
            btn.Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("swal2-container")));
        }

        /* ===========================================================
         *              VERIFICAR EXISTENCIA
         * =========================================================== */
        public bool ExistsProductInTable(string productName)
        {
            try
            {
                var table = GetProductsTable();
                return table.Text.Contains(productName, StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }
    }
}