using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Protractor;
using Xunit.Gherkin.Quick;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        protected string applicationID;
        protected string baseUri;
        protected IConfigurationRoot configuration;
        protected string endorsementID;

        protected string licenceID;

        // Protractor driver
        protected NgWebDriver ngDriver;
        protected string returnUser;

        protected TestBase()
        {
            var path = Directory.GetCurrentDirectory();

            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets("151C3067-C181-4482-A8C8-0628D14E5FF8")
                .Build();

            //bool runlocal = true;
            var options = new ChromeOptions();
            // run headless when in CI
            if (!string.IsNullOrEmpty(configuration["OPENSHIFT_BUILD_COMMIT"]) ||
                !string.IsNullOrEmpty(configuration["Build.BuildNumber"]))
            {
                Console.Out.WriteLine("Enabling Headless Mode");
                // could try --shm-size=1gb "disable-dev-shm-usage"
                options.AddArguments("headless", "no-sandbox", "no-zygote", "disable-gpu",
                    "disable-dev-shm-usage", "disable-infobars", "start-maximized", "hide-scrollbars",
                    "window-size=1920,1080");
                if (!string.IsNullOrEmpty(configuration["CHROME_BINARY_LOCATION"]))
                    options.BinaryLocation = configuration["CHROME_BINARY_LOCATION"];
            }
            else
            {
                options.AddArguments("start-maximized");
            }
            
            options.AddArgument("ignore-certificate-errors");
           
            // setup ChromeDriver with a command timeout of 2 minutes.
            var driver = new ChromeDriver(path, options, TimeSpan.FromMinutes(2));

            

            // var timeout = 45.0;
            // temp change to explore pipeline impact
            var timeout = 90.0;

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            //driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout * 2);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);

            ngDriver = new NgWebDriver(driver);

            ngDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);
            ngDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            //ngDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout * 2);

            baseUri = configuration["BASE_URI"] ?? "https://dev.justice.gov.bc.ca/lcrb";
        }

        protected bool IsIdPresent(string id)
        {
            var result = true;
            try
            {
                var x = ngDriver.FindElements(By.Id(id));
                if (x.Count == 0) result = false;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        protected void ScrollToElement(NgWebElement element)
        {
            var executor = (IJavaScriptExecutor) ngDriver.WrappedDriver;
            executor.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }


        /// <summary>
        ///     Find a given css selector, with a retry.  Useful for cases where a given control may not have been loaded at the
        ///     time the selector is used.
        /// </summary>
        /// <param name="cssSelector">The Css Selector</param>
        /// <returns>The found element, or null if not found.</returns>
        protected NgWebElement FindFirstElementByCssWithRetry(string cssSelector)
        {
            NgWebElement result = null;
            var retry = 5;
            while (retry > 0)
            {
                var elements = ngDriver.FindElements(By.CssSelector(cssSelector));
                if (elements != null && elements.Count > 0)
                {
                    result = elements[0];
                    retry = 0;
                }
                else
                {
                    retry--;
                }
            }

            if (result == null)
                // attempt to save a screenshot.
                try
                {
                    ((ITakesScreenshot) ngDriver.WrappedDriver).GetScreenshot().SaveAsFile("error.png");
                }
                catch (Exception)
                {
                    // ignore any errors that occur when saving the screenshot.
                }

            return result;
        }

        
        public void Dispose()
        {
            ngDriver.Quit();

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        [When(@"I log in to the doctors' portal")]
        public void DoctorsPortalLogIn()
        {
            var DoctorsPortalUri = configuration["BASE_URI"];
            var cardSerialNumber = configuration["BC_SERVICE_CARD_VIRTUAL_TOKEN"];
            var passcode = configuration["BC_SERVICE_CARD_PASSWORD"];

            ngDriver.IgnoreSynchronization = true;
            ngDriver.WrappedDriver.Navigate().GoToUrl($"{DoctorsPortalUri}");

            // click on Virtual card testing button
            var virtualCardTestingButton = ngDriver.WrappedDriver.FindElement(By.Id("tile_virtual_device_div_id"));
            virtualCardTestingButton.Click();

            Thread.Sleep(3000);

            // enter Card Serial Number
            var cardSerialNumberInput = ngDriver.WrappedDriver.FindElement(By.Id("csn"));
            cardSerialNumberInput.SendKeys(cardSerialNumber);

            // click on the Continue button
            var continueButton = ngDriver.WrappedDriver.FindElement(By.Id("continue"));
            continueButton.Click();

            // enter Passcode
            var passcodeInput = ngDriver.WrappedDriver.FindElement(By.Id("passcode"));
            passcodeInput.SendKeys(passcode);

            // click on the second Continue button
            var secondContinueButton = ngDriver.WrappedDriver.FindElement(By.Id("btnSubmit"));
            secondContinueButton.Click();
        }

        /* Temp workaround for S3DMFT-24 - to be removed
         */
        [And(@"I accept the cert request")]
        public void CertRequest()
        {
            var advancedButton = ngDriver.WrappedDriver.FindElement(By.Id("details-button"));
            advancedButton.Click();

            var proceedLink = ngDriver.WrappedDriver.FindElement(By.Id("proceed-link"));
            proceedLink.Click();
        }


        [And(@"the content is displayed for (.*)")]
        public void ContentDisplayed(string contentType)
        {
            if (contentType == "the doctors portal")
            {
                ngDriver.WrappedDriver.PageSource.Contains("Welcome to the doctor's portal!");
            }

            if (contentType == "the DMER dashboard")
            {
                ngDriver.WrappedDriver.PageSource.Contains("Dashboard");

                ngDriver.WrappedDriver.PageSource.Contains("Search DMER Case");

                ngDriver.WrappedDriver.PageSource.Contains("Submitted DMER Forms");
            }

            if (contentType == "the DMER clean pass")
            {
                ngDriver.WrappedDriver.PageSource.Contains("PASS! No Clean Pass responses failed.");
            }

            if (contentType == "the ICBC tombstone data")
            {
                WaitForFrame();
                //Thread.Sleep(3000);

                ngDriver.WrappedDriver.SwitchTo().Frame(0);

                var driversLicence = GetSeleniumValueField("data[textTargetDriverLicense]");
                    
                Assert.True(driversLicence.GetAttribute("value") == "0200700");

                // confirm value of driver's surname
                var driverSurname = ngDriver.WrappedDriver.FindElement(By.Name("data[textTargetDriverName]"));
                Assert.True(driverSurname.GetAttribute("value") == "PAKKER");

                // confirm value of driver's given name
                var driverGivenName = ngDriver.WrappedDriver.FindElement(By.Name("data[textTargetDriverFirstname]"));
                Assert.True(driverGivenName.GetAttribute("value") == "PETER");

                // confirm value of driver's date of birth
                var driverDateOfBirth = ngDriver.WrappedDriver.FindElement(By.Name("data[tDateTargetDriverBirthdate]"));
                Assert.True(driverDateOfBirth.GetAttribute("value") == "1987-03-26");

                // confirm value of driver's gender
                var driverGender = ngDriver.WrappedDriver.FindElement(By.Name("data[radioTargetDriverGender][eoz6u4e]"));
                Assert.True(driverGender.GetAttribute("checked") == "true");

                // confirm value of driver's country
                var driverCountry = ngDriver.WrappedDriver.FindElement(By.Name("data[selTargetDriverCountry]"));
                Assert.True(driverCountry.GetAttribute("value") == "Canada");

                // confirm value of driver's province
                var driverProvince = ngDriver.WrappedDriver.FindElement(By.Name("data[textTargetDriverProvince]"));
                Assert.True(driverProvince.GetAttribute("value") == "British Columbia");

                // confirm value of driver's city
                var driverCity = ngDriver.WrappedDriver.FindElement(By.Name("data[textTargetDriverCity]"));
                Assert.True(driverCity.GetAttribute("value") == "VICTORIA");

                // confirm value of driver's street address 1
                var driverStreetAddress1 = ngDriver.WrappedDriver.FindElement(By.Name("data[textTargetDriverAddr1]"));
                Assert.True(driverStreetAddress1.GetAttribute("value") == "129 DEAN RD");

                // confirm value of driver's street address 2
                var driverStreetAddress2 = ngDriver.WrappedDriver.FindElement(By.Name("data[textTargetDriverAddr2]"));
                Assert.True(driverStreetAddress2.GetAttribute("value") == "");

                // confirm value of driver's postal code
                var driverPostalCode = ngDriver.WrappedDriver.FindElement(By.Name("data[textTargetDriverPostal]"));
                Assert.True(driverPostalCode.GetAttribute("value") == "V8K 2K4");
            }
        }


        [And(@"I enter the login credentials")]
        public void LoginCredentials()
        {
        }


        [And(@"I click on (.*)")]
        public void ClickOnElement(string element)
        {
            if (element == "the Submit button")
            {
                NgWebElement submitButton = null;
                for (var i = 0; i < 20; i++)
                    try
                    {
                        // Submit button
                        var names = ngDriver.FindElements(By.CssSelector("button.mat-primary"));
                        if (names.Count > 0)
                        {
                            submitButton = names[0];
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                submitButton.Click();
            }

            if (element == "the DMER Forms tab")
            {
                var DMERFormsTab = ngDriver.FindElement(By.LinkText("DMER Forms"));
                DMERFormsTab.Click();
            }

            if (element == "the Case ID for 111")
            {
                var caseID = ngDriver.FindElement(By.LinkText("111"));
                caseID.Click();
            }

            if (element == "the Known Medical Conditions and Histories tab")
            {
                var knownMedicalConditionsAndHistories = ngDriver.FindElement(By.LinkText("Known Medical Conditions and Histories"));
                knownMedicalConditionsAndHistories.Click();
            }

            if (element == "the doctors' portal")
            {
                var DoctorsPortalUri = configuration["BASE_URI"];

                ngDriver.IgnoreSynchronization = true;
                ngDriver.WrappedDriver.Navigate().GoToUrl($"{DoctorsPortalUri}");
            }

            if (element == "the Preliminary Visual Assessment tab")
            {
                var preliminaryVisualAssessment= ngDriver.WrappedDriver.FindElement(By.PartialLinkText("Preliminary Visual Assessment"));
                preliminaryVisualAssessment.Click();
            }

            if (element == "the Next button")
            {
                // full class of the next button is "btn btn-primary btn-wizard-nav-next"
                var nextButton = GetSeleniumElementByCss("button.btn-wizard-nav-next");
                nextButton.Click();
            }
        }


        [And(@"I refresh the page")]
        public void PageRefresh()
        {
            ngDriver.Navigate().Refresh();
        }


        [And(@"I enter the Uncorrected Binocular Vision as 20")]
        public void UncorrectedBinocularVision()
        {
            var twenty = "20";

            var uncorrectedBinoVision = ngDriver.WrappedDriver.FindElement(By.Name("data[numVIS_2_1a3]"));
            uncorrectedBinoVision.SendKeys(twenty);
        }


        [Then(@"I log out of the portal")]
        public void PortalLogOut()
        {

        }


        protected IWebElement GetSeleniumValueField(string fieldName )
        {
            IWebElement result = null;
            for (var i = 0; i < 60; i++)
            {
                var names = ngDriver.WrappedDriver.FindElements(By.Name(fieldName));
                if (names.Count > 0 && !string.IsNullOrEmpty(names[0].GetAttribute("value")))
                {
                    result = names[0];
                    break;
                }

                Thread.Sleep(500);
            }
            
            return result;
        }

        /// <summary>
        /// Routine to get an element by CSS selector.  By default tries 3 times to get the element.
        /// </summary>
        /// <param name="css"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        protected IWebElement GetSeleniumElementByCss(string css, int attempts = 3)
        {
            IWebElement result = null;
            for (var i = 0; i < attempts; i++) 
            {
                var names = ngDriver.WrappedDriver.FindElements(By.CssSelector(css));
                if (names.Count > 0)
                {
                    result = names[0];
                    break;
                }

                Thread.Sleep(500);
            }

            return result;
        }


        // helper function for React.
        protected void WaitForFrame()
        {
            var wait = new WebDriverWait(ngDriver.WrappedDriver, TimeSpan.FromSeconds(30));

            wait.Until(iWebDriver => (bool)(((IJavaScriptExecutor)iWebDriver).ExecuteScript("return window.frames != undefined && window.frames[0] != undefined")) );
        }

    }
}