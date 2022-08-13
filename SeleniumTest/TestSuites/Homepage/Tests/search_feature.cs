using System;
using action = SeleniumTest.TestSuites.Homepage.Testactions.Actions;
using NUnit.Framework;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Threading;
using System.Collections.Generic;




namespace SeleniumTest.TestSuites.Homepage.Tests
{
    class search_feature : AssemblyBase
    {
        public search_feature(string parentUrl, string browser, string environment) 
        {
            this.parentUrl = parentUrl;
            this.browser = browser;
            this.environment = environment;
        }

       
        [Test]
        [Author("Din Laurence Babia")]
        [Description("This is to check the homepage contents.")]
        [Category("Page Test")]
        public void Check_Homepage_Contents()
        {
            action.go_to_homepage(_driver, parentUrl);
        }

        [Test]
        [Author("Din Laurence Babia")]
        [Description("This is to check the homepage contents.")]
        [Category("Process Test")]
        public void Search_Something(
            [Values("Selenium" , "Python" , "NUnit")] string value)
        {
            action.go_to_homepage(_driver, parentUrl);
            action.search(_driver, text: value);
            action.assert_title_search_results(_driver, expected: value);
        }

    }
}
