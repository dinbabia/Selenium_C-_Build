using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using NUnit.Framework;
using wa = SeleniumTest.TestSuites._Tools.WebActions;
using resx = SeleniumTest.TestSuites._Resources.homepage;

namespace SeleniumTest.TestSuites.Homepage.Testactions
{
    class Actions
    {

        /// <summary>
        /// Visit Google Homepage
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="parentUrl">The Parent Url from Assembly Base</param>
        public static void go_to_homepage(IWebDriver driver, string parentUrl)
        {
            wa.GoToUrl(driver, parentUrl: parentUrl, locator: By.Name(resx.search_box));
            wa.WaitElementIsDisplayed(driver, locator: By.CssSelector(resx.google_image));
            wa.WaitElementIsDisplayed(driver, locator: By.Name(resx.search_box));
        }

        /// <summary>
        /// Type in the search input and click Google Search
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="text"></param>
        public static void search(IWebDriver driver, string text = "")
        {
            wa.SendKeys(driver, locator: By.Name(resx.search_box), text_val:text);
            wa.Click(driver, locator: By.Name(resx.search_button));
        }

        /// <summary>
        /// Check and assert search results title
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="expected"></param>
        public static void assert_title_search_results(IWebDriver driver, string expected = "")
        {
            string actual = wa.GetElementText(driver, locator: By.XPath(resx.result_title));
            Assert.AreEqual(expected, actual);
        }

        
    }
}
