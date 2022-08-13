using NUnit.Framework;
using OpenQA.Selenium;


[assembly: NonParallelizable]
namespace SeleniumTest.TestSuites
{
    // URL Here is a sample only
    [TestFixture(new object[] { "https://stage.google.com", "chrome", "local" }, Category = "Local-Stage-Chrome")]
    [TestFixture(new object[] { "https://www.google.com", "chrome", "local" }, Category = "Local-Prod-Chrome")]
    [TestFixture(new object[] { "https://stage.google.com", "firefox", "local" }, Category = "Local-Stage-Firefox")]
    [TestFixture(new object[] { "https://www.google.com", "firefox", "local" }, Category = "Local-Prod-Firefox")]
    [TestFixture(new object[] { "https://stage.google.com", "edge", "local" }, Category = "Local-Stage-Edge")]
    [TestFixture(new object[] { "https://www.google.com", "edge", "local" }, Category = "Local-Prod-Edge")]
    
    abstract class AssemblyBase
    {
        protected string parentUrl, browser, environment;
        protected IWebDriver _driver;

        [SetUp]
        public void Setup()
        {
            _driver = Driver.GenerateWebDriver(browser, environment);
        }

        [TearDown]
        public void Cleanup()
        {
            //_driver.Quit();
        }
    }
}
