using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium;
using System;
using OpenQA.Selenium.Remote;
using System.IO;
using System.Reflection;

namespace SeleniumTest.TestSuites
{
    sealed class Driver
    {
        public static IWebDriver GenerateWebDriver(string browser, string environment)
        {
            if (environment.Equals("local"))
            {
                switch (browser)
                {
                    case "chrome":
                        return GenerateChromeDriverLocal();
                    case "firefox":
                        return GenerateFirefoxDriverLocal();
                    case "edge":
                        return GenerateEdgeDriverLocal();
                    case "safari":
                        return GenerateSafariDriverLocal();
                    default:
                        throw new SystemException("No driver class provided.");
                }
            } 
            else if (environment.Equals("remote"))
            {
                // Implement code here for connecting to RemoteWebDriver
                switch (browser)
                {
                    case "safari":
                        return GenerateSafariDriverRemote();
                    default:
                        throw new SystemException("Invalid setup.");
                }
            }  
            else
            {
                throw new SystemException("Unidentified environment.");
            }
        }

        private static IWebDriver GenerateChromeDriverLocal()
        {
            IWebDriver driver;
            ChromeOptions options = new ChromeOptions();
            //options.AddArguments(new string[] { "--headless", "--window-size=1920,1080" });

            // This is required for running in headless mode
            var test_path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            
            options.AddUserProfilePreference("download.default_directory", @test_path);
            options.AddUserProfilePreference("download.prompt_for_download", false);

            driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromSeconds(180));
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(30));
            return driver;
        }

        private static IWebDriver GenerateFirefoxDriverLocal()
        {
            IWebDriver driver;
            Environment.SetEnvironmentVariable("MOZ_HEADLESS_WIDTH", "1920");
            Environment.SetEnvironmentVariable("MOZ_HEADLESS_HEIGHT", "1080");
            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--headless");

            //       NEVER ASK WHEN DOWNLOADING PDF 
            var test_path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            options.SetPreference("browser.download.folderList", 2);
            options.SetPreference("browser.download.dir", @test_path);
            options.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/pdf");
            options.SetPreference("pdfjs.disabled", true);
            

            driver = new FirefoxDriver(FirefoxDriverService.CreateDefaultService(), options, TimeSpan.FromSeconds(180));
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(30));
            return driver;
        }

        private static IWebDriver GenerateEdgeDriverLocal()
        {
            IWebDriver driver;
            EdgeOptions options = new EdgeOptions();
            options.UseChromium = true;
            options.AddArguments(new string[] { "--headless", "--window-size=1920,1080" });

            // This is required for running in headless mode
            var test_path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            
            options.AddUserProfilePreference("download.default_directory", @test_path);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            
            driver = new EdgeDriver(EdgeDriverService.CreateChromiumService(), options, TimeSpan.FromSeconds(180));

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(30));
            return driver;
        }

        private static IWebDriver GenerateSafariDriverLocal()
        {
            IWebDriver driver;
            SafariOptions options = new SafariOptions();
            //[11-16-20 10:00:00] Do not uncomment. Headless does not work on Safari as of the time of writing.
            //options.AddArguments(new string[] { "--headless", "--window-size=1920,1080" }); 
            driver = new SafariDriver(SafariDriverService.CreateDefaultService(), options, TimeSpan.FromSeconds(180));
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(30));
            return driver;
        }

        private static IWebDriver GenerateSafariDriverRemote()
        {
            IWebDriver driver;
            SafariOptions options = new SafariOptions();
            //[11-16-20 10:00:00] Do not uncomment. Headless does not work on Safari as of the time of writing.
            //options.AddArguments(new string[] { "--headless", "--window-size=1920,1080" }); 
            driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub/"), options.ToCapabilities(), TimeSpan.FromSeconds(180));
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(30));
            return driver;
        }
    }
}
