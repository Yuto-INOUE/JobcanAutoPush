using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace JobcanAutoPush
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
#if !DEBUG
			var today = DateTime.Today;
			if (today.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
				|| JapanHoliday.JapanHoliday.IsHoliday(today))
			{
				return;
			}
#endif

			await PushJobcan();

			Environment.Exit(0);
		}

		private static async Task PushJobcan()
		{
			IWebDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService());
			driver.Navigate().GoToUrl("https://id.jobcan.jp/users/sign_in?app_key=atd");
			await Task.Delay(1000);

			// ログイン
			driver.FindElement(By.Name("user[email]")).SendKeys(Credentials.MailAddr);
			driver.FindElement(By.Name("user[password]")).SendKeys(Credentials.Password);
			driver.FindElement(By.Name("commit")).Click();
			await Task.Delay(1000);

			// 打刻PUSH
#if !DEBUG
			driver.FindElement(By.Name("adit_item")).Click();
			await Task.Delay(1000);
#endif

			// 工数入力
			if (driver.FindElement(By.Id("working_status")).Text.Trim() == "退室中")
			{
				driver.Navigate().GoToUrl("https://ssl.jobcan.jp/employee/man-hour-manage");
				driver.FindElement(By.ClassName("today-record")).FindElement(By.ClassName("btn")).Click();

				await Task.Delay(1000); // 読み込み待ち

				var templateList = new SelectElement(driver.FindElement(By.Name("template")));
				templateList.SelectByIndex(1);

				driver.FindElement(By.Id("save")).Click();
				await Task.Delay(1000);
			}

			driver.Quit();
		}
	}
}
