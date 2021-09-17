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
using System.Collections.Generic;

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

        [When(@"I log in to the (.*) portal")]
        public void DoctorsPortalLogIn(string portal)
        {
            if (portal == "doctors'")
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
            if (portal == "Dynamics")
            {
                var DynamicsPortalUri = configuration["DYNAMICS_BASE_URI"];
                var account = configuration["DYNAMICS_ACCOUNT"];
                var accountName = configuration["DYNAMICS_ACCOUNT_NAME"];
                var passcode = configuration["DYNAMICS_PASSWORD"];

                ngDriver.IgnoreSynchronization = true;
                ngDriver.WrappedDriver.Navigate().GoToUrl($"{DynamicsPortalUri}");

                // concatenate the account and account name
                var userName = account +  @"\" + accountName;

                // enter the combined account name
                var userNameInput = ngDriver.WrappedDriver.FindElement(By.Id("userNameInput"));
                userNameInput.SendKeys(userName);

                // enter Passcode
                var passcodeInput = ngDriver.WrappedDriver.FindElement(By.Id("passwordInput"));
                passcodeInput.SendKeys(passcode);

                // click on the Submit button
                var submitButton = ngDriver.WrappedDriver.FindElement(By.Id("submitButton"));
                submitButton.Click();
            }
        }


        [And(@"I log in to the (.*) portal")]
        public void DoctorsPortalLogIn2(string portal)
        {
            DoctorsPortalLogIn(portal);
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

                ngDriver.WrappedDriver.SwitchTo().Frame(0);

                Thread.Sleep(2000);

                // confirm value of driver's licence
                //var driversLicence = GetSeleniumValueField("data[textTargetDriverLicense]");
                //Assert.True(driversLicence.GetAttribute("value") == "0200700");

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

            if (contentType == "the provider")
            {
                WaitForFrame();

                ngDriver.WrappedDriver.SwitchTo().Frame(0);

                // confirm value of provider's given name
                var providerGivenName = GetSeleniumValueField("data[providerNameGiven]");
                Assert.True(providerGivenName.GetAttribute("value") == "providerNameGiven");
                
                // confirm value of provider's surname
                //var providerSurname = GetSeleniumValueField("[data[providerNameFamily]");
                //Assert.True(providerSurname.GetAttribute("value") == "providerNameFamily");                

                // confirm value of provider ID
                var providerID = GetSeleniumValueField("data[providerId]");
                Assert.True(providerID.GetAttribute("value") == "1234");
                
                // confirm value of provider ID type
                var providerIDType = GetSeleniumValueField("data[providerIdType]");
                Assert.True(providerIDType.GetAttribute("value") == "OPTID");
                
                // confirm value of provider role
                var providerRole = GetSeleniumValueField("data[providerRole]");
                Assert.True(providerRole.GetAttribute("value") == "Physician");
                
                // confirm value of provider specialty
                var providerSpecialty = GetSeleniumValueField("data[providerSpecialty]");
                Assert.True(providerSpecialty.GetAttribute("value") == "Cardiology");
                
                // confirm value of provider phone use
                var providerPhoneUse = GetSeleniumValueField("data[phoneUse][eg2m937]");
                //Assert.True(providerPhoneUse.GetAttribute("value") == "Work");               
               
                // confirm value of provider phone number
                var providerPhoneNumber = GetSeleniumValueField("data[providerPhoneNumber]");
                Assert.True(providerPhoneNumber.GetAttribute("value") == "123-123-1234");
                
                // confirm value of provider extension
                var providerPhoneExtension = GetSeleniumValueField("data[providerPhoneNumberExt]");
                Assert.True(providerPhoneExtension.GetAttribute("value") == "123");
                
                // confirm value of provider fax use
                var providerFaxUse = GetSeleniumValueField("data[faxUse][erx7c6f]");
                //Assert.True(providerFaxUse.GetAttribute("value") == "Work");
                
                // confirm value of provider fax number
                var providerFaxNumber = GetSeleniumValueField("data[providerFaxNumber]");
                Assert.True(providerFaxNumber.GetAttribute("value") == "123-123-1233");
              
                // confirm value of provider street address 1
                var providerStreetAddressLine1 = GetSeleniumValueField("data[providerStreetAddressLine1]");
                Assert.True(providerStreetAddressLine1.GetAttribute("value") == "providerStreetAddressLine1");

                // confirm value of provider street address 1
                var providerStreetAddressLine2 = GetSeleniumValueField("data[providerStreetAddressLine2]");
                Assert.True(providerStreetAddressLine2.GetAttribute("value") == "providerStreetAddressLine2");

                // confirm value of provider city
                var providerCity = GetSeleniumValueField("data[providerCityTown]");
                Assert.True(providerCity.GetAttribute("value") == "providerCityTown");
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

            if (element == "the Case ID for 333")
            {
                var caseID = ngDriver.FindElement(By.LinkText("333"));
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
                var nextButton = GetSeleniumElementByCss("button.btn-wizard-nav-next", 20);
                nextButton.Click();
            }

            if (element == "the form submit button")
            {
                // full class of the next button is "btn btn-primary btn-wizard-nav-submit"
                var formSubmitButton = GetSeleniumElementByCss("button.btn-wizard-nav-submit", 20);
                formSubmitButton.Click();
            }

            if (element == "the Syncope checkbox")
            {
                // to be completed
            }

            if (element == "the Cause Remains Unexplained radio button")
            {
                // to be completed
            }

            if (element == "the Single Syncopal Event radio button")
            {
                // to be completed
            }

            if (element == "No for Syncopal Event in the past 7 days")
            {
                // to be completed
            }

            if (element == "Yes for Syncopal Event in the past 7 days")
            {
                // to be completed
            }

            if (element == "the Commercial DMER option")
            {
                // to be completed
            }
        }


        [And(@"I wait for the (.*) field to have a value")]
        public void WaitForField(string field)
        {
            WaitForFrame();

            ngDriver.WrappedDriver.SwitchTo().Frame(0);

            Thread.Sleep(3000);

            Dictionary<string, string> fieldMap = new Dictionary<string, string>()
            {
                {"drivers licence","data[textTargetDriverLicense]"},
                {"notice","data[dropCommercialDMER]"}
            };
            var fieldObject = GetSeleniumValueField(fieldMap[field]);
        }


        [And(@"I wait for the (.*) content to be displayed")]
        public void WaitForContent(string field)
        {
            WaitForFrame();

            ngDriver.WrappedDriver.SwitchTo().Frame(0);

            if (field == "second page")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Commercial DMER')]"))
                        .Displayed);
            }

            if (field == "Dynamics homepage")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Sales Activity Social Dashboard')]"))
                        .Displayed);
            }
        }



        [And(@"the second page content is displayed")]
        public void SecondPageContent()
        {                
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Commercial DMER')]"))
                        .Displayed);
        }


        [And(@"I refresh the page")]
        public void PageRefresh()
        {
            ngDriver.Navigate().Refresh();
        }


        [And(@"I enter the (.*)")]
        public void UncorrectedBinocularVision(string option)
        {
            if (option == "Uncorrected Binocular Vision as 20")
            {
                var twenty = "20";

                var uncorrectedBinoVision = ngDriver.WrappedDriver.FindElement(By.Name("data[numVIS_2_1a3]"));
                uncorrectedBinoVision.SendKeys(twenty);
            }

            if (option == "medical opinion and confirmations")
            {
                // select 'Yes' for 'It is my opinion that this patient is fit to drive without additional accommodation or specialist input'
                // data[yornMISC_1_1]

                // select 'No' for 'It is my opinion that an Enhanced Road Assessment or Road Test should be performed for this driver'
                // data[yornMISC_2_1]

                // select 'No' for 'Additional specialist reports supporting this review are, or will be, provided for assessment'
                // data[yornMISC_3_1]
            }

            if (option == "single unexplained no repeat syncope details")
            {
                SyncopeSetup();

                // select 'Cause Remains Unexplained'
                var causeRemainsUnexplained = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-causeRemainsUnexplained"));
                causeRemainsUnexplained.Click();

                // select 'Single' for 'Syncopal Event'
                var singleSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-single"));
                singleSyncopalEvent.Click();

                // select 'No' for syncopal event in the past year
                var noSyncopalEventPastYear = ngDriver.WrappedDriver.FindElement(By.Id("e1waq6p-n"));
                noSyncopalEventPastYear.Click();
            }


            if (option == "recurrent unexplained past year syncope details")
            {
                SyncopeSetup();

                // select 'Cause Remains Unexplained'
                var causeRemainsUnexplained = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-causeRemainsUnexplained"));
                causeRemainsUnexplained.Click();

                // select 'Recurrent' for 'Syncopal Event'
                var singleSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-recurrent"));
                singleSyncopalEvent.Click();

                // select 'Yes' for syncopal event in the past year
                var noSyncopalEventPastYear = ngDriver.WrappedDriver.FindElement(By.Id("emtd13-y"));
                noSyncopalEventPastYear.Click();
            }

            // DMERSyncope.feature :: Syncope Currently Untreated Single
            if (option == "currently untreated no repeat syncope details")
            {
                SyncopeSetup();

                // select 'Currently Untreated'
                var currentlyUntreated = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-currentlyUntreated"));
                currentlyUntreated.Click();

                // select 'Single' for 'Syncopal Event'
                var singleSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-single"));
                singleSyncopalEvent.Click();
            }

            // DMERSyncope.feature :: Syncope Currently Untreated Recurrent
            if (option == "untreated currently recurrent syncope details")
            {
                SyncopeSetup();

                // select 'Currently Untreated'
                var currentlyUntreated = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-currentlyUntreated"));
                currentlyUntreated.Click();

                // select 'Recurrent' for 'Syncopal Event'
                var recurrentSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-recurrent"));
                recurrentSyncopalEvent.Click();
            }

            // DMERSyncope.feature :: Syncope Diagnosed, Treated Successfully, Single, Recent
            if (option == "diagnosed treated successfully single recent syncope details")
            {
                SyncopeSetup();

                // select 'Diagnosed, Treated Successfully'
                var diagnosedTreatedSuccessfully = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-diagnosedTreatedSuccessfully"));
                diagnosedTreatedSuccessfully.Click();

                // select 'Single' for 'Syncopal Event'
                var singleSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-single"));
                singleSyncopalEvent.Click();

                // select 'Yes' for 'Has the patient experienced a Syncopal Event in the past 1 Month?'
                var syncopalEventPastMonthYes = ngDriver.WrappedDriver.FindElement(By.Id("ev7hw28-y"));
                syncopalEventPastMonthYes.Click();
            }

            // DMERSyncope.feature :: Syncope Diagnosed, Treated Successfully, Recurrent, Not Recent
            if (option == "not recent diagnosed treated successfully recurrent syncope details")
            {
                SyncopeSetup();

                // select 'Diagnosed, Treated Successfully'
                var diagnosedTreatedSuccessfully = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-diagnosedTreatedSuccessfully"));
                diagnosedTreatedSuccessfully.Click();

                // select 'Recurrent' for 'Syncopal Event'
                var recurrentSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-recurrent"));
                recurrentSyncopalEvent.Click();

                // select 'No' for 'Has the patient experienced a Syncopal Event in the past 1 Month?'
                var syncopalEventPastMonthNo = ngDriver.WrappedDriver.FindElement(By.Id("ev7hw28-n"));
                syncopalEventPastMonthNo.Click();
            }

            // DMERSyncope.feature :: Syncope Reversible, Treated Successfully, Single
            if (option == "reversible, treated successfully single syncope details")
            {
                SyncopeSetup();

                // select 'Reversible, Treated Successfully'
                var reversibleTreatedSuccessfully = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-reversibleTreatedSuccessfully"));
                reversibleTreatedSuccessfully.Click();

                // select 'Single' for 'Syncopal Event'
                var singleSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-single"));
                singleSyncopalEvent.Click();
            }

            // DMERSyncope.feature :: Syncope Reversible, Treated Successfully, Recurrent
            if (option == "treated successfully recurrent syncope details")
            {
                SyncopeSetup();

                // select 'Reversible, Treated Successfully'
                var reversibleTreatedSuccessfully = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-reversibleTreatedSuccessfully"));
                reversibleTreatedSuccessfully.Click();

                // select 'Recurrent' for 'Syncopal Event'
                var recurrentSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-recurrent"));
                recurrentSyncopalEvent.Click();
            }

            // DMERSyncope.feature :: Syncope Situational, Avoidable Trigger, Single, Past 7 Days Yes
            if (option == "situational single past 7 days yes syncope details")
            {
                SyncopeSetup();

                // select 'Situational, Avoidable Trigger'
                var situationalAvoidableTrigger = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-situationalAvoidableTrigger"));
                situationalAvoidableTrigger.Click();

                // select 'Single' for 'Syncopal Event'
                var singleSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-single"));
                singleSyncopalEvent.Click();

                // select 'Yes' for 'Has the patient experienced a Syncopal Event in the past 7 days?'
                var SyncopalEventPastSevenDaysNo = ngDriver.WrappedDriver.FindElement(By.Id("eie7dlr-y"));
                SyncopalEventPastSevenDaysNo.Click();
            }

            // DMERSyncope.feature :: Syncope Situational, Avoidable Trigger, Single, Past 7 Days No
            if (option == "past 7 days no situational recurrent syncope details")
            {
                SyncopeSetup();

                // select 'Situational, Avoidable Trigger'
                var situationalAvoidableTrigger = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-situationalAvoidableTrigger"));
                situationalAvoidableTrigger.Click();

                // select 'Recurrent' for 'Syncopal Event'
                var recurrentSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-recurrent"));
                recurrentSyncopalEvent.Click();

                // select 'Yes' for 'Has the patient experienced a Syncopal Event in the past 7 days?'
                var SyncopalEventPastSevenDaysYes = ngDriver.WrappedDriver.FindElement(By.Id("eie7dlr-y"));
                SyncopalEventPastSevenDaysYes.Click();
            }

            // DMERSyncope.feature :: Syncope Vasovagal, Single, Typical
            if (option == "vasovagal single typical syncope details")
            {
                SyncopeSetup();

                // select 'Vasovagal Syncope'
                var vasovagalSyncope = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-vasovagalSyncope"));
                vasovagalSyncope.Click();

                // select 'Single' for 'Syncopal Event'
                var singleSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-single"));
                singleSyncopalEvent.Click();

                // select 'Typical Vasovagal' for 'Syncopal Expression is:'
                var typicalVasovagal = ngDriver.WrappedDriver.FindElement(By.Id("ejhgp9v-typicalVasovagal"));
                typicalVasovagal.Click();
            }

            // DMERSyncope.feature :: Syncope Vasovagal, Recurrent, Atypical
            if (option == "atypical vasovagal recurrent syncope details")
            {
                SyncopeSetup();

                // select 'Vasovagal Syncope'
                var vasovagalSyncope = ngDriver.WrappedDriver.FindElement(By.Id("e7vqgi-vasovagalSyncope"));
                vasovagalSyncope.Click();

                // select 'Recurrent' for 'Syncopal Event'
                var recurrentSyncopalEvent = ngDriver.WrappedDriver.FindElement(By.Id("ea4ioq5h-recurrent"));
                recurrentSyncopalEvent.Click();

                // select 'Atypical Vasovagal' for 'Syncopal Expression is:'
                var atypicalVasovagal = ngDriver.WrappedDriver.FindElement(By.Id("ejhgp9v-atypicalVasovagal"));
                atypicalVasovagal.Click();

                // select 'No' for 'Has the patient experienced a Syncopal Event in the past 7 days?'
                var SyncopalEventPastSevenDaysYes = ngDriver.WrappedDriver.FindElement(By.Id("e9pkjt-n"));
                SyncopalEventPastSevenDaysYes.Click();
            }

            if (option == "cardio steps")
            {
                // select 'Cardiovascular Diseases'
                var cardiovascularDiseases = ngDriver.WrappedDriver.FindElement(By.Name("data[checkFilterCH_3_1]"));
                cardiovascularDiseases.Click();

                // select 'Does the patient have any Cardiovascular Conditions?'
                var patientHasCardiovascularDiseases = ngDriver.WrappedDriver.FindElement(By.Name("data[yornCORO_1_x]"));
                patientHasCardiovascularDiseases.Click();

                /*
                 * Cardiovascular Condition Details
                 */

                // select 'No' for 'Has this patient experienced a related impaired level of consciousness in the last 6 months?'
                var impairedConsciousnessNo = ngDriver.WrappedDriver.FindElement(By.Id("er2n6wd-n"));
                impairedConsciousnessNo.Click();

                // select 'Yes' for 'Has this patient experienced a related impaired level of consciousness in the last 6 months?'
                var impairedConsciousnessYes = ngDriver.WrappedDriver.FindElement(By.Id("er2n6wd-y"));
                impairedConsciousnessYes.Click();

                // select 'No' for 'Does the patient experience Syncope symptoms which should be reported?'
                var syncopeSymptomsNo = ngDriver.WrappedDriver.FindElement(By.Id("efw8jmi-n"));
                syncopeSymptomsNo.Click();

                // select 'Yes' for 'Does the patient experience Syncope symptoms which should be reported?'
                var syncopeSymptomsYes = ngDriver.WrappedDriver.FindElement(By.Id("efw8jmi-y"));
                syncopeSymptomsYes.Click();

                // select 'Yes' for 'Have all of the patient's cardiovascular conditions been treated successfully?' 
                var successfulTreatmentYes = ngDriver.WrappedDriver.FindElement(By.Id("ez8gd78-y"));
                successfulTreatmentYes.Click();

                // select 'No' for 'Have all of the patient's cardiovascular conditions been treated successfully?' 
                var successfulTreatmentNo = ngDriver.WrappedDriver.FindElement(By.Id("ez8gd78-n"));
                successfulTreatmentNo.Click();

                /********
                    LVEF
                ********/

                Thread.Sleep(1000);

                // select 'Less Than 35%'
                var lessThan35Percent = ngDriver.WrappedDriver.FindElement(By.Id("elopoc8-less35"));
                lessThan35Percent.Click();

                // select 'Greater than or = 35%'
                var greaterThanOrEqual35Percent = ngDriver.WrappedDriver.FindElement(By.Id("elopoc8-more35"));
                greaterThanOrEqual35Percent.Click();

                // select 'Unavailable or not applicable'
                var unavailableOrNotApplicable = ngDriver.WrappedDriver.FindElement(By.Id("elopoc8-unavailable"));
                unavailableOrNotApplicable.Click();

                /********
                    NYHA
                ********/

                // select '1' for 'NYHA (Only required for specific conditions.)'
                var NYHA1 = ngDriver.WrappedDriver.FindElement(By.Id("evtoz-1"));
                NYHA1.Click();

                // select '2' for 'NYHA (Only required for specific conditions.)'
                var NYHA2 = ngDriver.WrappedDriver.FindElement(By.Id("evtoz-2"));
                NYHA2.Click();

                // select '3' for 'NYHA (Only required for specific conditions.)'
                var NYHA3 = ngDriver.WrappedDriver.FindElement(By.Id("evtoz-3"));
                NYHA3.Click();

                // select '4' for 'NYHA (Only required for specific conditions.)'
                var NYHA4 = ngDriver.WrappedDriver.FindElement(By.Id("evtoz-4"));
                NYHA4.Click();

                /*
                 * Simplified Cardio Survey
                 */

                /*
                 * Implanted Cardioverter Defibrillator (ICD)
                 */

                // select 'ICD Not Applicable' for 'Has the patient received an ICD?' 
                var hasPatientReceivedICDNotApplicable = ngDriver.WrappedDriver.FindElement(By.Id("ewzoei-icdNotApplicable"));
                hasPatientReceivedICDNotApplicable.Click();

                // select 'Recommended, patient declined' for 'Has the patient received an ICD?' 
                var hasPatientReceivedICDRecommendedPatientDeclined = ngDriver.WrappedDriver.FindElement(By.Id("ewzoei-recommendedPatientDeclined"));
                hasPatientReceivedICDRecommendedPatientDeclined.Click();

                // select 'No, scheduled to occur' for 'Has the patient received an ICD?' 
                var hasPatientReceivedICDNoScheduledToOccur = ngDriver.WrappedDriver.FindElement(By.Id("ewzoei-noScheduledToOccur"));
                hasPatientReceivedICDNoScheduledToOccur.Click();

                // select 'Yes' for 'Has the patient received an ICD?' 
                var hasPatientReceivedICDYes = ngDriver.WrappedDriver.FindElement(By.Id("ewzoei-yes"));
                hasPatientReceivedICDYes.Click();

                // select 'Secondary' for 'ICD Prophylaxis Condition is:'
                var ICDProphylaxisConditionIsSecondary = ngDriver.WrappedDriver.FindElement(By.Id("emogb8e-secondary"));
                ICDProphylaxisConditionIsSecondary.Click();

                // select 'Primary' for 'ICD Prophylaxis Condition is:'
                var ICDProphylaxisConditionIsPrimary = ngDriver.WrappedDriver.FindElement(By.Id("emogb8e-primary"));
                ICDProphylaxisConditionIsPrimary.Click();

                // select 'Within the past 6 Months' for 'ICD therapy (ATP/Shock) delivered'
                var ICDTherapyDeliveredWithinPast6Months = ngDriver.WrappedDriver.FindElement(By.Id("epqos1q-withinThePast6Months"));
                ICDTherapyDeliveredWithinPast6Months.Click();

                // select for 'No' for 'ICD therapy (ATP/Shock) delivered'
                var ICDTherapyDeliveredNo = ngDriver.WrappedDriver.FindElement(By.Id("epqos1q-no"));
                ICDTherapyDeliveredNo.Click();

                /*
                 * Coronary Artery Disease
                 */

                /*
                 * Treatments
                 */

                // select 'Coronary Artery Disease'
                var CoronaryArteryDisease = ngDriver.WrappedDriver.FindElement(By.Name("data[yornCORO_1_a]"));
                CoronaryArteryDisease.Click();

                // select 'No' for 'CAD has been treated with CABG?'
                var CADTreatedWithCABGNo = ngDriver.WrappedDriver.FindElement(By.Id("etjjl3h-no"));
                CADTreatedWithCABGNo.Click();

                // select 'Yes' for 'CAD has been treated with CABG?'
                var CADTreatedWithCABGYes = ngDriver.WrappedDriver.FindElement(By.Id("etjjl3h-yes"));
                CADTreatedWithCABGYes.Click();

                Thread.Sleep(1000);

                /**************************** 
                    Arrhythmias and Pacemaker
                *****************************/

                // select 'Arrhythmias and Pacemaker'
                var arrhythmiasAndPacemaker = ngDriver.WrappedDriver.FindElement(By.Name("data[yornCORO_1_d]"));
                arrhythmiasAndPacemaker.Click();

                // select 'Pacemaker Not Applicable' for 'Has the patient received a Pacemaker?'
                var patientReceivedAPacemakerPacemakerNotApplicable = ngDriver.WrappedDriver.FindElement(By.Id("ezlh3pi-pacemakerNotApplicable"));
                patientReceivedAPacemakerPacemakerNotApplicable.Click();

                // select 'Recommended, patient declined' for 'Has the patient received a Pacemaker?'
                var patientReceivedAPacemakerRecommendedPatientDeclined = ngDriver.WrappedDriver.FindElement(By.Id("ezlh3pi-recommendedPatientDeclined"));
                patientReceivedAPacemakerRecommendedPatientDeclined.Click();

                // select 'No, scheduled to occur' for 'Has the patient received a Pacemaker?'
                var patientReceivedAPacemakerNoScheduledToOccur = ngDriver.WrappedDriver.FindElement(By.Id("ezlh3pi-noScheduledToOccur"));
                patientReceivedAPacemakerNoScheduledToOccur.Click();

                // select 'Yes' for 'Has the patient received a Pacemaker?'
                var patientReceivedAPacemakerYes = ngDriver.WrappedDriver.FindElement(By.Id("ezlh3pi-yes"));
                patientReceivedAPacemakerYes.Click();

                // select 'No' for 'Was the pacemaker installed within the past 7 days'
                var pacemakerInstalledPast30DaysNo = ngDriver.WrappedDriver.FindElement(By.Id("e0fvxwc-no"));
                pacemakerInstalledPast30DaysNo.Click();

                // select 'Yes' for 'Was the pacemaker installed within the past 7 days'
                var pacemakerInstalledPast30DaysYes = ngDriver.WrappedDriver.FindElement(By.Id("e0fvxwc-yes"));
                pacemakerInstalledPast30DaysYes.Click();

                // select 'No' for 'Does the pacemaker show normal sensing and capture on a post-implant ECG? '
                var pacemakerShowsNormalSensingOnPostImplantECGNo = ngDriver.WrappedDriver.FindElement(By.Id("eeaqy9b-no"));
                pacemakerShowsNormalSensingOnPostImplantECGNo.Click();

                // select 'Yes' for 'Does the pacemaker show normal sensing and capture on a post-implant ECG? '
                var pacemakerShowsNormalSensingOnPostImplantECGYes = ngDriver.WrappedDriver.FindElement(By.Id("eeaqy9b-yes"));
                pacemakerShowsNormalSensingOnPostImplantECGYes.Click();

                // select 'Scheduled to occur' for 'Cardiac Ablation'
                var cardiacAblationScheduledToOccur = ngDriver.WrappedDriver.FindElement(By.Id("ea57m2b-scheduledToOccur"));
                cardiacAblationScheduledToOccur.Click();

                // select 'Within the past 3 Months' for 'Cardiac Ablation'
                var cardiacAblationWithinPast3Months = ngDriver.WrappedDriver.FindElement(By.Id("ea57m2b-withinThePast3Months"));
                cardiacAblationWithinPast3Months.Click();

                // select 'No' for 'Cardiac Ablation'
                var cardiacAblationNo = ngDriver.WrappedDriver.FindElement(By.Id("ea57m2b-no"));
                cardiacAblationNo.Click();

                // select 'Sustained VT'
                var sustainedVTNo = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a12]"));
                sustainedVTNo.Click();

                Thread.Sleep(2000);

                // select 'VF with no reversible cause'
                var VFNoReversibleCause = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a16]"));
                VFNoReversibleCause.Click();

                // select 'No' for 'Experienced within the past 6 months?'
                var sustainedVIExperiencedWithinPast6MonthsNo = ngDriver.WrappedDriver.FindElement(By.Id("e1r8j2-no"));
                sustainedVIExperiencedWithinPast6MonthsNo.Click();

                // select 'Yes' for 'Experienced within the past 6 months?'
                var sustainedVIExperiencedWithinPast6MonthsYes = ngDriver.WrappedDriver.FindElement(By.Id("e1r8j2-yes"));
                sustainedVIExperiencedWithinPast6MonthsYes.Click();

                /******************************
                    Hemodynamically Unstable VT
                ******************************/

                // select 'Hemodynamically Unstable VT'
                var hemodynamicallyUnstableVT = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a14]"));
                hemodynamicallyUnstableVT.Click();

                // select 'No' for 'Experienced within the past 6 months?'
                var hemodynamicallyUnstableVTNo = ngDriver.WrappedDriver.FindElement(By.Id("eo36dpm-n"));
                hemodynamicallyUnstableVTNo.Click();

                // select 'Yes' for 'Experienced within the past 6 months?'
                var hemodynamicallyUnstableVTYes = ngDriver.WrappedDriver.FindElement(By.Id("eo36dpm-y"));
                hemodynamicallyUnstableVTYes.Click();

                /************ 
                    AV Blocks
                ************/

                // select 'AV Blocks'
                var AVBlocks = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a11]"));
                AVBlocks.Click();

                // select 'None of the above' for 'AV Block Expression'
                var noneOfTheAboveAVBlockExpression = ngDriver.WrappedDriver.FindElement(By.Id("emo22xd-noneOfTheAbove"));
                noneOfTheAboveAVBlockExpression.Click();

                // select 'Acquired 3rd degree AV block' for 'AV Block Expression'
                var acquired3rdDegreeAVBlockAVBlockExpression = ngDriver.WrappedDriver.FindElement(By.Id("emo22xd-acquired3rdDegreeAvBlock"));
                acquired3rdDegreeAVBlockAVBlockExpression.Click();

                // select 'Alternating LBBB and RBBB' for 'AV Block Expression'
                var alternatingLBBBAndRBBBAVBlockExpression = ngDriver.WrappedDriver.FindElement(By.Id("emo22xd-alternatingLbbbAndRbbb"));
                alternatingLBBBAndRBBBAVBlockExpression.Click();

                // select '2nd degree AV block /Mobitz II' for 'AV Block Expression'
                var secondDegreeAVBlockMobitzIIAVBlockExpression = ngDriver.WrappedDriver.FindElement(By.Id("emo22xd-2ndDegreeAvBlockMobitzIi"));
                secondDegreeAVBlockMobitzIIAVBlockExpression.Click();

                /************* 
                    PAC or PVC
                *************/

                // select 'PAC or PVC'
                var PACorPVC = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a15]"));
                PACorPVC.Click();

                // select 'No' for 'With impaired Level of Consciousness caused by cerebral ischemia?'
                var impairedLevelOfConsciousnessCausedByCerebralIschemiaNo = ngDriver.WrappedDriver.FindElement(By.Id("edpjl4x-no"));
                impairedLevelOfConsciousnessCausedByCerebralIschemiaNo.Click();

                // select 'Yes' for 'With impaired Level of Consciousness caused by cerebral ischemia?'
                var impairedLevelOfConsciousnessCausedByCerebralIschemiaYes = ngDriver.WrappedDriver.FindElement(By.Id("edpjl4x-yes"));
                impairedLevelOfConsciousnessCausedByCerebralIschemiaYes.Click();

                /************************************ 
                    Sinus Node Dysfunction
                ************************************/

                // select 'Sinus Node Dysfunction'
                var sinusNodeDysfunction = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a17]"));
                sinusNodeDysfunction.Click();

                /***************************** 
                    Paroxysmal SVT, AF, or AFL
                 ****************************/

                // select 'Paroxysmal SVT, AF, or AFL'
                var ParoxysmalSVT_AF_Or_AFL = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a10]"));
                ParoxysmalSVT_AF_Or_AFL.Click();

                // select 'No' for 'Treated with therapy, no recurrence within past 3 months'
                var treatedWithTherapyNoRecurrenceWithinPast3MonthsNo = ngDriver.WrappedDriver.FindElement(By.Id("egr4ltw-n"));
                treatedWithTherapyNoRecurrenceWithinPast3MonthsNo.Click();

                // select 'Yes' for 'Treated with therapy, no recurrence within past 3 months'
                var treatedWithTherapyNoRecurrenceWithinPast3MonthsYes = ngDriver.WrappedDriver.FindElement(By.Id("egr4ltw-y"));
                treatedWithTherapyNoRecurrenceWithinPast3MonthsYes.Click();

                Thread.Sleep(1000);

                // select 'No additional detail available' for 'Condition has been treated with:'
                var conditionTreatedWithNoAdditionalDetailAvailable = ngDriver.WrappedDriver.FindElement(By.Id("edzke8e-noAdditionalDetailAvailable"));
                conditionTreatedWithNoAdditionalDetailAvailable.Click();

                // select 'AFL, they have had a successful isthmus ablation with proven establishment of bidirectional isthmus block' for 'Condition has been treated with:'
                var conditionTreatedWithSuccessfulIsthmusAblationWithProvenEstablishmentOfBidirectionalIsthmusBlock = ngDriver.WrappedDriver.FindElement(By.Id("edzke8e-aflTheyHaveHadASuccessfulIsthmusAblationWithProvenEstablishmentOfBidirectionalIsthmusBlock"));
                conditionTreatedWithSuccessfulIsthmusAblationWithProvenEstablishmentOfBidirectionalIsthmusBlock.Click();
                
                Thread.Sleep(1000);

                // select 'AF, they have had AV node ablation and pacemaker implantation and meet the standard for pacemaker treatment' for 'Condition has been treated with:'
                var conditionTreatedWithAFAVNodeAblationAndPacemakerImplantation = ngDriver.WrappedDriver.FindElement(By.Id("edzke8e-afTheyHaveHadAvNodeAblationAndPacemakerImplantationAndMeetTheStandardForPacemakerTreatment"));
                conditionTreatedWithAFAVNodeAblationAndPacemakerImplantation.Click();

                // select 'SVT, it has been successfully treated with radiofrequency ablation' for 'Condition has been treated with:'
                var conditionTreatedWithSVTRadiofrequencyAblation = ngDriver.WrappedDriver.FindElement(By.Id("edzke8e-svtItHasBeenSuccessfullyTreatedWithRadiofrequencyAblation"));
                conditionTreatedWithSVTRadiofrequencyAblation.Click();
                
                /***************************** 
                    Heart Failure
                ****************************/

                // select 'Heart Failure'
                var heartFailure = ngDriver.WrappedDriver.FindElement(By.Name("data[yornCORO_1_b]"));
                heartFailure.Click();

                // select 'Congestive Heart Failure'
                var congestiveHeartFailure = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a1]"));
                congestiveHeartFailure.Click();

                // select 'Left Ventricular Dysfunction / Cardiomyopathy'
                var leftVentricularDysfunctionCardiomyopathy = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a3]"));
                leftVentricularDysfunctionCardiomyopathy.Click();

                // select 'No' for 'Patient has Continuous Flow Left Ventricular Assist Device (LVAD)'
                var patientHasLVADNo = ngDriver.WrappedDriver.FindElement(By.Id("evtl3tx-no"));
                patientHasLVADNo.Click();

                // select 'Yes' for 'Patient has Continuous Flow Left Ventricular Assist Device (LVAD)'
                var patientHasLVADYes = ngDriver.WrappedDriver.FindElement(By.Id("evtl3tx-yes"));
                patientHasLVADYes.Click();

                // select 'No' for 'LVAD has been stable for 2 or more months post implantation?'
                var stableTwoOrMonthsPostImplantationNo = ngDriver.WrappedDriver.FindElement(By.Id("eyd1qf6-n"));
                stableTwoOrMonthsPostImplantationNo.Click();

                // select 'Yes' for 'LVAD has been stable for 2 or more months post implantation?'
                var stableTwoOrMonthsPostImplantationYes = ngDriver.WrappedDriver.FindElement(By.Id("eyd1qf6-y"));
                stableTwoOrMonthsPostImplantationYes.Click();

                // select 'No' for 'Is the patient receiving Intermittent Inotropes?'
                var receivingIntermittentInotropesNo = ngDriver.WrappedDriver.FindElement(By.Id("eca20bv-n"));
                receivingIntermittentInotropesNo.Click();

                // select 'Yes' for 'Is the patient receiving Intermittent Inotropes?'
                var receivingIntermittentInotropesYes = ngDriver.WrappedDriver.FindElement(By.Id("eca20bv-y"));
                receivingIntermittentInotropesYes.Click();

                /***************************** 
                    Hypertrophic Cardiomyopathy
                ****************************/

                // select 'Hypertrophic Cardiomyopathy'
                var hypertrophicCardiomyopathy = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a6]"));
                hypertrophicCardiomyopathy.Click();

                /*
                // select 'Left ventricle wall thickness of < 30 mm'
                var leftVentricleWallThicknessOfLessThan30mm = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a19]"));
                leftVentricleWallThicknessOfLessThan30mm.Click();

                // select 'Family history of sudden death at a young age'
                var familyHistorySuddenDeathYoungAge = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a8]"));
                familyHistorySuddenDeathYoungAge.Click();

                // select 'Increase in blood pressure with exercise'
                var increaseBloodPressureWithExercise = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a18]"));
                increaseBloodPressureWithExercise.Click();

                // select 'Non-sustained VT'
                var nonSustainedVT = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a13]"));
                nonSustainedVT.Click();
                */

                // select 'Structural Heart Disease'
                var structuralHeartDisease = ngDriver.WrappedDriver.FindElement(By.Name("data[yornCORO_1_c]"));
                structuralHeartDisease.Click();

                Thread.Sleep(1000);

                // select 'Surgically Treated Valvular Heart Disease'
                var surgicallyTreatedValvularHeartDisease = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a9]"));
                surgicallyTreatedValvularHeartDisease.Click();

                // select 'No' for 'Has it been at least 6 weeks since discharge following treatment?'
                var atLeast3MonthsSinceDischargeNo = ngDriver.WrappedDriver.FindElement(By.Id("e7kuha-n"));
                atLeast3MonthsSinceDischargeNo.Click();

                // select 'Yes' for 'Has it been at least 6 weeks since discharge following treatment?'
                var atLeast3MonthsSinceDischargeYes = ngDriver.WrappedDriver.FindElement(By.Id("e7kuha-y"));
                atLeast3MonthsSinceDischargeYes.Click();

                /*
                // select 'No' for 'Is the patient subject to one of the following prosthesis/therapies?'
                var prosthesisTherapiesNo = ngDriver.WrappedDriver.FindElement(By.Id("e465rgk-no"));
                prosthesisTherapiesNo.Click();

                // select 'Yes' for 'Is the patient subject to one of the following prosthesis/therapies?'
                var prosthesisTherapiesYes = ngDriver.WrappedDriver.FindElement(By.Id("e465rgk-yes"));
                prosthesisTherapiesYes.Click();

                // select 'No' for 'Is the patient on anti-coagulant therapy due to one of the above prosthesis/therapies?'
                var anticoagulantTherapyNo = ngDriver.WrappedDriver.FindElement(By.Id("eecbl7s-no"));
                anticoagulantTherapyNo.Click();

                // select 'Yes' for 'Is the patient on anti-coagulant therapy due to one of the above prosthesis/therapies?'
                var anticoagulantTherapyYes = ngDriver.WrappedDriver.FindElement(By.Id("eecbl7s-yes"));
                anticoagulantTherapyYes.Click();
                */

                Thread.Sleep(1000);

                // select 'Mitral Valve Prolapse'
                var mitralValveProlapse = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a22]"));
                mitralValveProlapse.Click();

                // select 'Yes, but they have not been assessed for Arrhythmias or do not meet standards' for 'Is the patient symptomatic for this condition?'
                var symptomaticYesNotAssessed = ngDriver.WrappedDriver.FindElement(By.Id("ei8sz1-yesButTheyHaveNotBeenAssessedForArrhythmiasOrDoNotMeetStandards"));
                symptomaticYesNotAssessed.Click();

                // select 'Yes, but they have been assessed for Arrhythmias and meet applicable standards' for 'Is the patient symptomatic for this condition?'
                var symptomaticYesAssessed = ngDriver.WrappedDriver.FindElement(By.Id("ei8sz1-yesButTheyHaveBeenAssessedForArrhythmiasAndMeetApplicableStandards"));
                symptomaticYesAssessed.Click();

                // select 'No' for 'Is the patient symptomatic for this condition?'
                var symptomaticNo = ngDriver.WrappedDriver.FindElement(By.Id("ei8sz1-no"));
                symptomaticNo.Click();

                // select 'Medically Treated Valvular Heart Disease'
                var medicallyTreatedValvularHeartDisease = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a4]"));
                medicallyTreatedValvularHeartDisease.Click();

                /*
                // select 'Medically Treated Aortic Stenosis or Aortic Sclerosis'
                var medicallyTreatedAorticStenosisOrAorticSclerosis = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a21]"));
                medicallyTreatedAorticStenosisOrAorticSclerosis.Click();

                // select 'Not Available' for 'Patient has an aortic valve area (AVA) ) > 1.0 cm2'
                var aorticValveAreaGreaterThanNotAvailable = ngDriver.WrappedDriver.FindElement(By.Id("eoqaij2-notAvailable"));
                aorticValveAreaGreaterThanNotAvailable.Click();

                // select 'No' for 'Patient has an aortic valve area (AVA) ) > 1.0 cm2'
                var aorticValveAreaGreaterThanNo = ngDriver.WrappedDriver.FindElement(By.Id("eoqaij2-no"));
                aorticValveAreaGreaterThanNo.Click();

                // select 'Yes' for 'Patient has an aortic valve area (AVA) ) > 1.0 cm2'
                var aorticValveAreaGreaterThanYes = ngDriver.WrappedDriver.FindElement(By.Id("eoqaij2-yes"));
                aorticValveAreaGreaterThanYes.Click();

                // select 'No' for 'Has a current Cardiologist Report confirmed these results, including risk of Syncope?'
                var cardiologistReportNo = ngDriver.WrappedDriver.FindElement(By.Id("ee0u4tl-n"));
                cardiologistReportNo.Click();

                // select 'Yes' for 'Has a current Cardiologist Report confirmed these results, including risk of Syncope?'
                var cardiologistReportYes = ngDriver.WrappedDriver.FindElement(By.Id("ee0u4tl-y"));
                cardiologistReportYes.Click();

                // select 'Medically Treated Aortic or Mitral Regurgitation or Mitral Stenosis'
                var medicallyTreatedAorticMitralRegurgitationMitralStenosis = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a20]"));
                medicallyTreatedAorticMitralRegurgitationMitralStenosis.Click();

                // select 'Congenital'
                Thread.Sleep(1000);

                var congenital = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a7]"));
                congenital.Click();

                // select 'No' for 'Has the congenital heart defect been repaired, with the patient condition being considered stable with no concerns?'
                var congenitalDefectRepairedNoConcernsNo = ngDriver.WrappedDriver.FindElement(By.Id("e6kng3n-n"));
                congenitalDefectRepairedNoConcernsNo.Click();

                // select 'Yes' for 'Has the congenital heart defect been repaired, with the patient condition being considered stable with no concerns?'
                var congenitalDefectRepairedNoConcernsYes = ngDriver.WrappedDriver.FindElement(By.Id("e6kng3n-y"));
                congenitalDefectRepairedNoConcernsYes.Click();

                // select 'Inherited'
                var inherited = ngDriver.WrappedDriver.FindElement(By.Name("data[checkCORO_HF_a23]"));
                inherited.Click();

                // select 'No' for 'Has the patient condition has been investigated and treated by a cardiologist?'
                var investigatedTreatedByCardiologistNo = ngDriver.WrappedDriver.FindElement(By.Id("e2ofetf-n"));
                investigatedTreatedByCardiologistNo.Click();

                // select 'Yes' for 'Has the patient condition has been investigated and treated by a cardiologist?'
                var investigatedTreatedByCardiologistYes = ngDriver.WrappedDriver.FindElement(By.Id("e2ofetf-y"));
                investigatedTreatedByCardiologistYes.Click();
                */

                // select 'No' for 'Has the patient received a Heart Transplant?'
                var heartTransplantNo = ngDriver.WrappedDriver.FindElement(By.Id("ech9bcn-no"));
                heartTransplantNo.Click();

                // select 'Yes' for 'Has the patient received a Heart Transplant?'
                var heartTransplantYes = ngDriver.WrappedDriver.FindElement(By.Id("ech9bcn-yes"));
                heartTransplantYes.Click();

                // select 'No' for 'Does the patient have active ischemia?'
                var activeIschemiaNo = ngDriver.WrappedDriver.FindElement(By.Id("eup7vds-n"));
                activeIschemiaNo.Click();

                // select 'Yes' for 'Does the patient have active ischemia?'
                var activeIschemiaYes = ngDriver.WrappedDriver.FindElement(By.Id("eup7vds-y"));
                activeIschemiaYes.Click();

                // select 'No' for 'Is the patient considered stable on immunotherapy?'
                var patientStableOnImmunotherapyNo = ngDriver.WrappedDriver.FindElement(By.Id("ebxm2x-n"));
                patientStableOnImmunotherapyNo.Click();

                // select 'Yes' for 'Is the patient considered stable on immunotherapy?'
                var patientStableOnImmunotherapyYes = ngDriver.WrappedDriver.FindElement(By.Id("ebxm2x-y"));
                patientStableOnImmunotherapyYes.Click();
                
                // select 'No' for 'Transplanted within the past 6 months?'
                var transplantedWithinPast6MonthsNo = ngDriver.WrappedDriver.FindElement(By.Id("em2e8pq-n"));
                transplantedWithinPast6MonthsNo.Click();

                // select 'Yes' for 'Transplanted within the past 6 months?'
                var transplantedWithinPast6MonthsYes = ngDriver.WrappedDriver.FindElement(By.Id("em2e8pq-y"));
                transplantedWithinPast6MonthsYes.Click();
            }
        }

        public void SyncopeSetup()
        {
            // select 'Syncope'
            var syncopeCheckbox = ngDriver.WrappedDriver.FindElement(By.Name("data[checkFilterCH_19_1]"));
            syncopeCheckbox.Click();

            // select 'Has the patient experienced any Syncopal Episodes?'
            var syncopalEpisodesCheckbox = ngDriver.WrappedDriver.FindElement(By.Name("data[checkPreQ_SYNC_1_x]"));
            syncopalEpisodesCheckbox.Click();
        }


        //temporary method - to be removed
        [And(@"I do not select the Commercial DMER option")]
        public void DoNotSelectCommercialDMER()
        {
        }


        [Then(@"I log out of the portal")]
        public void PortalLogOut()
        {
        }

        [And(@"I log out of the portal")]
        public void PortalLogOut2()
        {
            PortalLogOut();
        }


        protected IWebElement GetSeleniumValueField(string fieldName )
        {
            Thread.Sleep(10000);

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
            var wait = new WebDriverWait(ngDriver.WrappedDriver, TimeSpan.FromSeconds(90));

            wait.Until(iWebDriver => (bool)(((IJavaScriptExecutor)iWebDriver).ExecuteScript("return window.frames != undefined && window.frames[0] != undefined")) );
        }

    }
}