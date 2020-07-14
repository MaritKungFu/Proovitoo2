using NUnit.Framework;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace KunglaProovitoo
{
	class Tests
	{
		IWebDriver driver;

		WebDriverWait wait;
		// Get and validate logo text.
		public void ValidateLogo()
		{
			string logo = wait.Until(ExpectedConditions.ElementExists(By.XPath("//div/p[@class='logo']"))).Text;
			Assert.AreEqual("okidoki", logo, "Logo text is not okidoki.");
		}

		[OneTimeSetUp]
		public void Start_Browser()
		{
			driver = new ChromeDriver(".")
			{
				// Base url.
				Url = "https://www.okidoki.ee/"
			};

			wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
		}

		[Test, Order(1)]
		// Add valid password.
		[TestCase("Testitestimine", "", Description = "It is possible to log in with correct username and password")]
		public void LogIn(string username, string password)
		{
			ValidateLogo();

			// Find and click on "Logi sisse" button.
			Console.WriteLine("Click log in button on homepage.");
			driver.FindElement(By.XPath("//a[@href='/login/']")).Click();

			ValidateLogo();

			// Find h1 and validate value.
			string header = wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1"))).Text;
			Assert.AreEqual("Sisene okidoki keskkonda", header, "H1 is not correct.");

			// Find "Kasutajanimi" field and insert username.
			driver.FindElement(By.Id("login-input")).SendKeys(username);

			// Find "Parool" field and insert password.
			driver.FindElement(By.Id("password-input")).SendKeys(password);

			// Find "logi sisse" button and click
			driver.FindElement(By.XPath("//*[@value='Logi sisse']")).Click();

			Assert.IsEmpty((driver.FindElements(By.XPath("//*[@id='login']/div/h1/following-sibling::*[1]"))), "Error visible");

			ValidateLogo();

			// Validate that user is loged in and log off button becomes visible.
			string logi_valja = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='homepage']/div[1]/div/div[1]/p/a[2]"))).Text;
			Assert.AreEqual(logi_valja, "Logi välja", "Log out button is not visible.");
			string loggedInUser = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='homepage']/div[1]/div/div[1]/p/a[1]"))).Text;
			Assert.AreEqual(username, loggedInUser, "Logged in username is not correct.");

			// Log off and validate that log in button becomes visible.
			Console.WriteLine("Click log off button.");
			wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='homepage']/div[1]/div/div[1]/p/a[2]"))).Click();
			ValidateLogo();
			string logi_sisse = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//a[@href='/login/']"))).Text;
			Assert.AreEqual(logi_sisse, "Logi sisse", "Log in button is not visible.");
		}

		[Test, Order (2)]
		// Empty fields.
		[TestCase("", "", "Sisesta oma kasutajanimi\r\nSisesta oma parool", Description = "It is not possible to log in with empty fields.")]
		[TestCase("Testitestimine", "", "Sisesta oma parool", Description = "It is not possible to log in with one empty password field.")]
		// Add correct password leave username empty.
		[TestCase("", "", "Sisesta oma kasutajanimi", Description = "It is not possible to log in with empty username field.")]
		// Add correct password.
		[TestCase("Marit", "", "Sisestasid vale kasutajanime või parooli", Description = "It is not possible to log in with wrong username.")]
		[TestCase("Testitestimine", "pauh", "Sisestasid vale kasutajanime või parooli", Description = "It is not possible to log in with wrong password.")]
		public void FailToLogIn(string username, string password, string message)
		{
			driver.Url = "https://www.okidoki.ee/login/";

			ValidateLogo();

			// Find h1 and validate value.
			string header= wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1"))).Text;
			Assert.AreEqual("Sisene okidoki keskkonda", header, "H1 is not correct.");

			// Find "Kasutajanimi" field and insert username.
			driver.FindElement(By.Id("login-input")).SendKeys(username);

			// Find "Parool" field and insert password.
			driver.FindElement(By.Id("password-input")).SendKeys(password);

			// Find "logi sisse" button and click
			driver.FindElement(By.XPath("//*[@value='Logi sisse']")).Click();

			// Validate that error appears and user is not loged in.
			string error = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='login']/div/h1/following-sibling::*[1]"))).Text;
			Assert.That(error.Contains(message), "Error is not correct.");
		}

		[OneTimeTearDown]
		public void Close_Browser()
		{
			driver.Dispose();
		}
	}
}