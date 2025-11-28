using OpenQA.Selenium;

namespace AuriStore.UI.Tests.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver driver;


        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }


        private IWebElement EmailInput => driver.FindElement(By.Id("loginEmail"));
        private IWebElement PasswordInput => driver.FindElement(By.Id("loginPassword"));
        private IWebElement LoginButton => driver.FindElement(By.Id("loginBtn"));


        public void Login(string email, string password)
        {
            EmailInput.Clear();
            EmailInput.SendKeys(email);

            PasswordInput.Clear();
            PasswordInput.SendKeys(password);

            LoginButton.Click();
        }
    }
}

