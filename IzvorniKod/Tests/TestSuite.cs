using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace DuckI.Tests
{
    [TestFixture]
    public class TestSuite
    {
        // Radimo usera, nema delete metode dobre pa se generatea random username, be warned runnanje testova n puta
        // kreira n usera
        private IWebDriver _driver;
        private string _testEmail = $"testuser_{Guid.NewGuid()}@exampletest.com";
        private string _testPassword = "Test@1234";
        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            _driver = new ChromeDriver();
            _httpClient = new HttpClient();
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        // lista testova, samo na dodavati testove koje zelite, order odreduje kojim se redoslijedom izvrsavaju
        // TestOpeningPageDisplaysWelcomeMessage sluzi da provjeri ako se pocetni site uspjesno otvara
        [Test, Order(1)]
        public void TestOpeningPageDisplaysWelcomeMessage()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/");

            var welcomeElement = _driver.FindElement(By.ClassName("display-4"));
            Assert.That(welcomeElement.Text, Is.EqualTo("Welcome To DuckI"));
        }

        // TestRegisterUser sluzi za provjeru registracije
        [Test, Order(2)]
        public void TestRegisterUser()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Register");

            var usernameField = _driver.FindElement(By.Id("Input_Email"));
            var passwordField = _driver.FindElement(By.Id("Input_Password"));
            var confirmPasswordField = _driver.FindElement(By.Id("Input_ConfirmPassword"));
            var registerButton = _driver.FindElement(By.CssSelector("button[type='submit']"));

            usernameField.SendKeys(_testEmail);
            passwordField.SendKeys(_testPassword);
            confirmPasswordField.SendKeys(_testPassword);
            registerButton.Click();

            Assert.That(_driver.Url, Is.EqualTo("https://localhost:44385/"));
        }

        // TestLogin provjerava login korisnika stvorenog sa TestRegisterUser
        [Test, Order(3)]
        public void TestLogin()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Login");

            var usernameField = _driver.FindElement(By.Id("Input_Email"));
            var passwordField = _driver.FindElement(By.Id("Input_Password"));
            var loginButton = _driver.FindElement(By.Id("login-submit"));

            usernameField.SendKeys(_testEmail);
            passwordField.SendKeys(_testPassword);
            loginButton.Click();

            TestContext.WriteLine($"Logging in with Email: {_testEmail}, Password: {_testPassword}");

            Assert.That(_driver.Url, Is.EqualTo("https://localhost:44385/"));
        }
        
        // ovdje se mogu pisati daljnji testovi vezani uz usera
    }
}