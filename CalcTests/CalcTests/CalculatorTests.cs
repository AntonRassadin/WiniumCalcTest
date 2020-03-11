using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Winium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace CalcTests
{
    [TestFixture]
    class CalculatorTests
    {
        WiniumDriver driver;

        [SetUp]
        public void Initialize()
        {
            //var dc = new DesiredCapabilities();
            //dc.SetCapability("app", @"C:/windows/system32/calc.exe");
            //driver = new RemoteWebDriver(new Uri("http://localhost:9999"), dc);
            DesktopOptions options = new DesktopOptions();
            options.ApplicationPath = @"C:/windows/system32/calc.exe";
            options.LaunchDelay = 2;
            
            driver = new WiniumDriver(@"C:/temp/", options); //C:/temp/Winium.Desktop.Driver.exe 
            Thread.Sleep(1000);

        }
        [TearDown]
        public void Quit()
        {
            driver.Quit();
            Process[] procs = Process.GetProcessesByName("Calculator");
            foreach (Process p in procs) { p.Kill(); }
        }

        [Test]
        public void CalcTest()
        {
            Random random = new Random();
            List<int> digits = new List<int>();

            int firstDigit;
            int sectondDigit;

            int inputValue = random.Next(100);

            if (inputValue > 9)
            {
                sectondDigit = inputValue % 10;
                firstDigit = inputValue / 10;
                digits.Add(firstDigit);
                digits.Add(sectondDigit);
            }
            else
                digits.Add(inputValue);

            foreach (int item in digits)
            {
                ClickOnDigitButton(item); //Нажать рандомное число от 1 до 100
            }

            driver.FindElement(By.Id("squareRootButton")).Click(); //нажать кнопку для извлечения корня из числа

            string operation = (
                Regex.Match(
                    driver.FindElement(By.Id("CalculatorExpression")).GetAttribute("Name"),
                    @"√\(\d+\)").Value
                    );

            Console.WriteLine(operation);
            Console.WriteLine($"√({inputValue})");

            Assert.AreEqual(operation, $"√({inputValue})", $"Operaion: {operation} are not equal to √({inputValue})");  //Проверить, что в поле ввода верно отображена операция  например - √(89)

            Console.WriteLine(GetResult());
            Console.WriteLine(Math.Sqrt(inputValue));

            Assert.AreEqual(Math.Sqrt(inputValue), GetResult(), $"result is not equal to Math.Sqrt({inputValue})"); //Проверить совпадение результата операции с вычисленным программно значением

            driver.FindElement(By.Id("multiplyButton")).Click();
            driver.FindElement(By.Id("equalButton")).Click(); //Нажать кнопку умножения и вычисления результата, 

            Assert.AreEqual(inputValue, GetResult(), $"Result is not equal to {inputValue}"); //проверить что получилось число , из которого вычисляли корень.
            Console.WriteLine(GetResult());
            
        }

        private double GetResult()
        {
            string resultString = driver.FindElement(By.Id("CalculatorResults")).GetAttribute("Name");
            double result = (Double.Parse(Regex.Match(resultString, @"(?<=Display is\s).*\d").Value));
            return result;
        }

        private void ClickOnDigitButton(int digit)
        {
            string[] buttonsDigits = new string[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
            driver.FindElement(By.Name(buttonsDigits[digit])).Click();
        }
    }
}
