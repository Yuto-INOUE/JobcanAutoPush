using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace JobcanAutoPush
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			if (JapanHoliday.JapanHoliday.IsHoliday(DateTime.Today))
			{
				return;
			}

			await PushJobcan();

			Environment.Exit(0);
		}

		private static async Task PushJobcan()
		{
			IWebDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService());
			driver.Navigate().GoToUrl("https://id.jobcan.jp/users/sign_in?app_key=atd");

			// ログイン
			driver.FindElement(By.Name("user[email]")).SendKeys(Credentials.MailAddr);
			driver.FindElement(By.Name("user[password]")).SendKeys(Credentials.Password);
			driver.FindElement(By.Name("commit")).Click();

			// 打刻PUSH
			driver.FindElement(By.Name("adit_item")).Click();

			// 工数入力
			if (driver.FindElement(By.Id("working_status")).Text.Trim() == "退室中")
			{
				driver.Navigate().GoToUrl("https://ssl.jobcan.jp/employee/man-hour-manage");
				driver.FindElement(By.ClassName("today-record")).FindElement(By.ClassName("btn")).Click();

				await Task.Delay(1000); // 読み込み待ち

				var templateList = new SelectElement(driver.FindElement(By.Name("template")));
				templateList.SelectByIndex(1);

				driver.FindElement(By.Id("save")).Click();
			}

			driver.Quit();
		}
	}
}
