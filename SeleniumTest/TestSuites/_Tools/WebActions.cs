using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTest.TestSuites._Tools
{
    class WebActions
    {
        public static int time_span = 20;

        /// <summary>
        /// Used to click on any element, such as an anchor tag, a link, etc.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void Click(IWebDriver driver, By locator, bool hidden = false)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            try
            {
                var element = wait.Until(e => e.FindElement(locator));
                if (!hidden && element.Displayed && element.Enabled)
                {
                    element.Click();
                }
                else
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click()", element);
                }
            }
            catch
            {
                throw new Exception($"\nElement not found:\n{locator}");
            }
        }

        /// <summary>
        /// Gets the visible, inner text of the web-element.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static string GetElementText(IWebDriver driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            var element = wait.Until(e => e.FindElement(locator)).Text;
            return element;
        }

        /// <summary>
        /// Get a list of text of the web element
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static List<string> GetElementsText(IWebDriver driver, By locator)
        {
            var elements_list = new List<string>();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            var elements = wait.Until(e => e.FindElements(locator));
            //Console.WriteLine(elements.Count()); // Checking
            elements.ToList().ForEach(x => elements_list.Add(x.Text));
            return elements_list;
        }

        /// <summary>
        /// Get a particular tag with specific attribute value
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static string GetElementAttribute(IWebDriver driver, By locator, string attr)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            var element = wait.Until(e => e.FindElement(locator)).GetAttribute(attr);
            
            return element;
        }

        /// <summary>
        /// Get a particular tag with specific attribute value
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static List<string> GetElementsAttribute(IWebDriver driver, By locator, string attr)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            var elements_attr = new List<string>();
            var elements = wait.Until(e => e.FindElements(locator));
            foreach (var element in elements)
            {
                elements_attr.Add(element.GetAttribute(attr));
            }

            return elements_attr;
        }

        /// <summary>
        /// This is needed when "checked" is not displayed in DOM. Find elements and look for an element that is selected/checked. 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static string GetElementSelected(IWebDriver driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            var elements = wait.Until(e => e.FindElements(locator));
            foreach (var element in elements)
            {
                if (element.Selected) return element.GetAttribute("value");
            }
            return "No element selected";
        }

        /// <summary>
        /// Navigate to a link
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="parentUrl"></param>
        /// <param name="locator"></param>
        /// <param name="endpoint"></param>
        public static void GoToUrl(IWebDriver driver, string parentUrl, By locator, string endpoint = "")
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            driver.Navigate().GoToUrl(parentUrl + endpoint);
            wait.Until(e => e.FindElement(locator));
        }

        /// <summary>
        /// Mouse hover to an element
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void HoverToElement(IWebDriver driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            var element = wait.Until(e => e.FindElement(locator));
            Actions actions = new Actions(driver);
            actions.MoveToElement(element);
            actions.Perform();
        }

        /// <summary>
        /// Mouse hover to an element and get text
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static List<string> HoverAndGetTextElements(IWebDriver driver, By locator_main_elements, By locator_sub_element)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            var elements = wait.Until(e => e.FindElements(locator_main_elements));
            var texts_list = new List<string> {};
      
            foreach (var element in elements)
            {
                Actions actions = new Actions(driver);
                actions.MoveToElement(element).Perform();
                var texts = element.FindElements(locator_sub_element);
              
                foreach ( var text in texts)
                {
                    if (text.Text == "") continue;
                    texts_list.Add(text.Text.Trim());
                }
            }
            return texts_list;
        }

        /// <summary>
        /// Mouse hover to an element
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void ScrollToViewElement(IWebDriver driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            var element = wait.Until(e => e.FindElement(locator));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", element);
        }

        /// <summary>
        /// Enter editable content in the text and password fields during test execution
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void SendKeys(IWebDriver driver, By locator, string text_val)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            try
            {
                var element = wait.Until(e => e.FindElement(locator));
                element.Clear();
                element.SendKeys(text_val);
            }
            catch
            {
                throw new Exception($"\nElement not found:\n{locator}");
            }
        }

        /// <summary>
        /// Wait for element to be present
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void WaitElement(IWebDriver driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            var element = wait.Until(e => e.FindElement(locator));
        }


        /// <summary>
        /// Wait for element to be displayed
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void WaitElementIsDisplayed(IWebDriver driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            try
            {
                wait.Until(e => e.FindElement(locator).Displayed);
            }
            catch
            {
                throw new Exception($"\nElement not found:\n{locator}");
            }
        }

        /// <summary>
        /// Wait for element to be displayed
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void WaitElementIsEnabled(IWebDriver driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            try
            {
                var element = wait.Until(e => e.FindElement(locator).Enabled);
            }
            catch
            {
                throw new Exception($"\nElement not found:\n{locator}");
            }
        }

        /// <summary>
        /// Wait for element to be displayed
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public static void WaitElementIsSelected(IWebDriver driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time_span));
            try
            {
                var element = wait.Until(e => e.FindElement(locator).Selected);
            }
            catch
            {
                throw new Exception($"\nElement not found:\n{locator}");
            }
        }
    }
}
