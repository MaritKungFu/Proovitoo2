using NUnit.Framework;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace MaritTests
{
	class Tests
	{
		IWebDriver driver;

		WebDriverWait wait;
		// Get and validate logo text.
		public void ValidateLogo()
		{
			string logo = wait.Until(ExpectedConditions.ElementExists(By.XPath("//div/p[@class='logo']"))).Text;
			Assert.AreEqual("okidoki", logo, "Lehe pealkiri ei ole okidoki.");
		}

		[OneTimeSetUp]
		public void Start_Browser()
		{
			driver = new ChromeDriver
			{
				// Base url.
				Url = "https://www.okidoki.ee/"
			};

			wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
		}

		[Test, Order(1)]
		[TestCase("Testitestimine", "ckHD4d4a4TibTvN")]
		public void LogIn(string username, string password)
		{
			ValidateLogo();

			// Find and click on "Logi sisse" button.
			Console.WriteLine("Kliki avalehel logi sisse nupule.");
			driver.FindElement(By.XPath("//a[@href='/login/']")).Click();

			ValidateLogo();

			// Find h1 and validate value.
			string header = wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1"))).Text;
			Assert.AreEqual("Sisene okidoki keskkonda", header, "H1 ei ole korrektne.");

			// Find "Kasutajanimi" field and insert username.
			driver.FindElement(By.Id("login-input")).SendKeys(username);

			// Find "Parool" field and insert password.
			driver.FindElement(By.Id("password-input")).SendKeys(password);

			// Find "logi sisse" button and click
			driver.FindElement(By.XPath("//*[@value='Logi sisse']")).Click();

			ValidateLogo();

			// Validate that user is loged in and log off button becomes visible.
			string logi_valja = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='homepage']/div[1]/div/div[1]/p/a[2]"))).Text;
			Assert.AreEqual(logi_valja, "Logi välja", "Logi välja nupp ei ole nähtav.");
			string loggedInUser = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='homepage']/div[1]/div/div[1]/p/a[1]"))).Text;
			Assert.AreEqual(username, loggedInUser, "Sisse logitud kasutajanimi on vale.");

			// Log off and validate that log in button becomes visible.
			Console.WriteLine("Kliki logi välja nupule.");
			wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='homepage']/div[1]/div/div[1]/p/a[2]"))).Click();
			ValidateLogo();
			string logi_sisse = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//a[@href='/login/']"))).Text;
			Assert.AreEqual(logi_sisse, "Logi sisse", "Logi sisse nupp ei ole nähtav.");
		}

		[Test, Order (2)]
		// TODO: Tee message paremaks!
		[TestCase("", "", "Sisesta oma ")]
		[TestCase("Testitestimine", "", "Sisesta oma parool")]
		[TestCase("", "ckHD4d4a4TibTvN", "Sisesta oma kasutajanimi")]
		[TestCase("Marit", "ckHD4d4a4TibTvN", "Sisestasid vale kasutajanime või parooli")]
		[TestCase("Testitestimine", "pauh", "Sisestasid vale kasutajanime või parooli")]
		public void FailToLogIn(string username, string password, string message)
		{
			driver.Url = "https://www.okidoki.ee/login/";

			ValidateLogo();

			// Find h1 and validate value.
			string header= wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1"))).Text;
			Assert.AreEqual("Sisene okidoki keskkonda", header, "H1 ei ole korrektne.");

			// Find "Kasutajanimi" field and insert username.
			driver.FindElement(By.Id("login-input")).SendKeys(username);

			// Find "Parool" field and insert password.
			driver.FindElement(By.Id("password-input")).SendKeys(password);

			// Find "logi sisse" button and click
			driver.FindElement(By.XPath("//*[@value='Logi sisse']")).Click();

			// Validate that error appears and user is not loged in.
			string error = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='login']/div/h1/following-sibling::*[1]"))).Text;
			Assert.IsTrue(error.Contains(message), "Error ei ole nähtav.");
		}

		[OneTimeTearDown]
		public void Close_Browser()
		{
			driver.Dispose();
		}
	}
}