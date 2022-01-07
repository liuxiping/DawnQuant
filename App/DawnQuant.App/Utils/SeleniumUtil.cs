using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Utils
{
    public class SeleniumUtil
    {

        public static WebDriver GetWebDriver()
        {
            ChromeOptions options = new ChromeOptions();

            //无头模式
           // options.AddArgument("--headless");


            var cds = ChromeDriverService.CreateDefaultService();
            //是否应隐藏服务的命令提示符窗口
            cds.HideCommandPromptWindow = true;
            cds.Start();

            //防止被检测   反反爬
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);

            var driver = new ChromeDriver(cds,options);

            var dic = new Dictionary<string, object>();
            dic.Add("source", "Object.defineProperty(navigator, 'webdriver', { get: () => undefined})");
            driver.ExecuteCdpCommand("Page.addScriptToEvaluateOnNewDocument", dic);
           
            //driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);

            //设置隐式等待超时随机等待10-20秒
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(new Random().Next(10, 15));

            return driver;
        }
    }
}
