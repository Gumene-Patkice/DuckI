using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace DuckI.Tests
{
    [TestFixture]
    class TestSuite
    {
        // Radimo usera
        private IWebDriver _driver;
        private string _testEmail = $"testuser_{Guid.NewGuid()}@exampletest.com";
        private string _testPassword = "Test@1234";

        [SetUp]
        public void SetUp()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            _driver = new ChromeDriver();
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
        
        // Updateana verzija TestOpeningPageDisplaysWelcomeMessage sluzi da provjeri ako se pocetni site uspjesno otvara

        [Test, Order(1)]
        public void TestOpeningPageDisplaysWelcomeMessageNew()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/");

            var welcomeElement = _driver.FindElement(By.ClassName("text-primary"));
            Assert.That(welcomeElement.Text, Is.EqualTo("Welcome to DuckI!"));
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
            
            Assert.That(_driver.Url, Is.EqualTo("https://localhost:44385/"));
        }

        // Test registracije za neispravan format emaila

        [Test, Order(4)]
        public void TestRegisterUserIncorectEmail()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Register");

            var usernameField = _driver.FindElement(By.Id("Input_Email"));
            var passwordField = _driver.FindElement(By.Id("Input_Password"));
            var confirmPasswordField = _driver.FindElement(By.Id("Input_ConfirmPassword"));
            var registerButton = _driver.FindElement(By.CssSelector("button[type='submit']"));

            usernameField.SendKeys("invalidemail");
            passwordField.SendKeys(_testPassword);
            confirmPasswordField.SendKeys(_testPassword);
            registerButton.Click();

            var errorElement = _driver.FindElement(By.Id("Input_Email-error"));

            Assert.That(errorElement.Text, Is.EqualTo("The Email field is not a valid e-mail address."));
        }

        // Test za ispitivanje public materials navigacijskog gumba
        [Test, Order(5)]
        public void TestPublicMaterialsButton()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Login");

            var usernameField = _driver.FindElement(By.Id("Input_Email"));
            var passwordField = _driver.FindElement(By.Id("Input_Password"));
            var loginButton = _driver.FindElement(By.Id("login-submit"));

            usernameField.SendKeys(_testEmail);
            passwordField.SendKeys(_testPassword);
            loginButton.Click();

            var publicMaterialsButton = _driver.FindElement(By.XPath("//a[@href='/Pdf/ViewPublicMaterial']"));

            publicMaterialsButton.Click();

            Assert.That(_driver.Url, Is.EqualTo("https://localhost:44385/Pdf/ViewPublicMaterial"));
        }

        // Test za potpun proces od logina do uploadanja pdf-a
        [Test, Order(6)]
        public void TestLoginAndUploadPdf()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Login");

            var usernameField = _driver.FindElement(By.Id("Input_Email"));
            var passwordField = _driver.FindElement(By.Id("Input_Password"));
            var loginButton = _driver.FindElement(By.Id("login-submit"));

            usernameField.SendKeys(_testEmail);
            passwordField.SendKeys(_testPassword);
            loginButton.Click();

            var userButton = _driver.FindElement(By.XPath("//a[@href='/Identity/Account/Manage']"));
            userButton.Click();


            var roleButton = _driver.FindElement(By.XPath("//a[@href='/Home/Roles']"));
            roleButton.Click();

            var studentRoleButton = _driver.FindElement(By.XPath("//button[text()='Get Student Role']"));
            studentRoleButton.Click();

            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Manage");
            var logoutButton = _driver.FindElement(By.XPath("//button[text()='Sign out']"));
            logoutButton.Click();

            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Login");

            var usernameFieldAggain = _driver.FindElement(By.Id("Input_Email"));
            var passwordFieldAggain = _driver.FindElement(By.Id("Input_Password"));
            var loginButtonAggain = _driver.FindElement(By.Id("login-submit"));

            usernameFieldAggain.SendKeys(_testEmail);
            passwordFieldAggain.SendKeys(_testPassword);
            loginButtonAggain.Click();

            var uploadPdfButton = _driver.FindElement(By.XPath("//a[@href='/Pdf/UploadPdf']"));
            uploadPdfButton.Click();

            var fileInput = _driver.FindElement(By.Id("file"));
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var projectPath = Directory.GetParent(basePath).Parent.Parent.Parent.FullName;
            var filePath = Path.Combine(projectPath, "Tests", "TestPDF.pdf");

            fileInput.SendKeys(filePath);

            var uploadButton = _driver.FindElement(By.XPath("//button[text()='Upload Private PDF']"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", uploadButton);
            Thread.Sleep(1000);
            var confirmation = _driver.FindElement(By.XPath("//*[text()='File uploaded successfully.']"));
            var confirmationText = ((IJavaScriptExecutor)_driver)
                .ExecuteScript("return arguments[0].innerText;", confirmation).ToString();

            Assert.That(confirmationText, Is.EqualTo("File uploaded successfully."));
        }

        // Test za upload kalendara
        [Test, Order(7)]
        public void TestFileUpload()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Login");

            var usernameField = _driver.FindElement(By.Id("Input_Email"));
            var passwordField = _driver.FindElement(By.Id("Input_Password"));
            var loginButton = _driver.FindElement(By.Id("login-submit"));
            
            usernameField.SendKeys(_testEmail);
            passwordField.SendKeys(_testPassword);
            loginButton.Click();

            var calendarButton = _driver.FindElement(By.XPath("//a[@href='/Home/Calendar']"));
            calendarButton.Click();
            
            var uploadCalendarButton = _driver.FindElement(By.XPath("//button[text()='Upload Calendar']"));
            uploadCalendarButton.Click();
            
            var fileInput = _driver.FindElement(By.Id("file"));
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var projectPath = Directory.GetParent(basePath).Parent.Parent.Parent.FullName;
            var filePath = Path.Combine(projectPath, "Tests", "TestingCalendar.csv");
            var confirmCalendarButton = _driver.FindElement(By.XPath("//button[text()='Upload']"));
            
            fileInput.SendKeys(filePath);
            confirmCalendarButton.Click();

            Assert.That(_driver.Url, Is.EqualTo("https://localhost:44385/"));
        }
        
        // Testing uploading something that isn't a pdf, calendar used as exsample
        [Test, Order(8)]
        public void TestLoginAndUploadNonPdf()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Login");

            var usernameField = _driver.FindElement(By.Id("Input_Email"));
            var passwordField = _driver.FindElement(By.Id("Input_Password"));
            var loginButton = _driver.FindElement(By.Id("login-submit"));

            usernameField.SendKeys(_testEmail);
            passwordField.SendKeys(_testPassword);
            loginButton.Click();

            var uploadPdfButton = _driver.FindElement(By.XPath("//a[@href='/Pdf/UploadPdf']"));
            uploadPdfButton.Click();

            var fileInput = _driver.FindElement(By.Id("file"));
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var projectPath = Directory.GetParent(basePath).Parent.Parent.Parent.FullName;
            var filePath = Path.Combine(projectPath, "Tests", "TestingCalendar.csv");

            fileInput.SendKeys(filePath);
            var uploadButton = _driver.FindElement(By.XPath("//button[text()='Upload Private PDF']"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", uploadButton);
            Thread.Sleep(1000);
            var confirmation =
                _driver.FindElement(
                    By.XPath("//*[text()='Error: Internal server error: Only PDF files are allowed.']"));
            var confirmationText = ((IJavaScriptExecutor)_driver)
                .ExecuteScript("return arguments[0].innerText;", confirmation).ToString();

            Assert.That(confirmationText, Is.EqualTo("Error: Internal server error: Only PDF files are allowed."));
        }

        // Dodavanje eventa u kalendar putem pritiska na relevantni gumb
        [Test, Order(9)]
        public void TestAddEvent()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Login");

            var usernameField = _driver.FindElement(By.Id("Input_Email"));
            var passwordField = _driver.FindElement(By.Id("Input_Password"));
            var loginButton = _driver.FindElement(By.Id("login-submit"));
            
            usernameField.SendKeys(_testEmail);
            passwordField.SendKeys(_testPassword);
            loginButton.Click();

            var calendarButton = _driver.FindElement(By.XPath("//a[@href='/Home/Calendar']"));
            calendarButton.Click();
            
            var uploadCalendarButton = _driver.FindElement(By.XPath("//button[text()='Upload Calendar']"));
            uploadCalendarButton.Click();
            
            var fileInput = _driver.FindElement(By.Id("file"));
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var projectPath = Directory.GetParent(basePath).Parent.Parent.Parent.FullName;
            var filePath = Path.Combine(projectPath, "Tests", "TestingCalendar.csv");
            var confirmCalendarButton = _driver.FindElement(By.XPath("//button[text()='Upload']"));
            
            fileInput.SendKeys(filePath);
            confirmCalendarButton.Click();
            
            var calendarButtonTwo = _driver.FindElement(By.XPath("//a[@href='/Home/Calendar']"));
            calendarButtonTwo.Click();
            
            var element = _driver.FindElement(By.XPath("//div[@data-day='1']"));
            Console.WriteLine(element);
            element.Click();
            

            var addInputDescription = _driver.FindElement(By.Id("eventDescription"));
            var confirmButton = _driver.FindElement(By.CssSelector("button.btn.btn-success.text-light"));

            addInputDescription.SendKeys("Test event");
            confirmButton.Click();
            Thread.Sleep(1000);
            var firstEventTextDiv = _driver.FindElement(By.CssSelector("div.event-text"));
            Assert.That(firstEventTextDiv.Text, Is.EqualTo("Test event"));
        }

        // Test deletanja novostvorenog usera
        [Test, Order(10)]
        public void TestDeleteUser()
        {
            _driver.Navigate().GoToUrl("https://localhost:44385/Identity/Account/Login");

            var usernameField = _driver.FindElement(By.Id("Input_Email"));
            var passwordField = _driver.FindElement(By.Id("Input_Password"));
            var loginButton = _driver.FindElement(By.Id("login-submit"));

            usernameField.SendKeys("Adminko@Adminovic1.admin");
            passwordField.SendKeys("Adminko@Adminovic1.admin");
            loginButton.Click();
            
            var controlPannel = _driver.FindElement(By.XPath("//a[@href='/Admin/ControlPanel']"));
            controlPannel.Click();
            
            var listAllUsersButton = _driver.FindElement(By.XPath("//a[@href='/Admin/GetAllUsers']"));
            listAllUsersButton.Click();
            
            var lastButton = _driver.FindElement(By.XPath("(//button[contains(@class, 'btn btn-danger')])[last()]"));
            lastButton.Click();
            
            bool isEmailPresent = _driver.PageSource.Contains(_testEmail);
            
            Assert.That(isEmailPresent, Is.False, $"The email {_testEmail} was found on the screen after deletion.");
        }
    }
}