using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace InstagramBot
{
    public class Spider
    {
        protected static IWebDriver Driver;//Firefox driver
        public string Username;

        public Spider(String username, String password)
        {
            //Log into instgarm, get past notifications and save password popups
            FirefoxOptions options = new FirefoxOptions();
            Driver = new FirefoxDriver(Environment.CurrentDirectory, options);
            Driver.Url = "https://instagram.com";
            WaitFor("password", "Name");
            Driver.FindElement(By.Name("username")).SendKeys(username);
            Driver.FindElement(By.Name("password")).SendKeys(password);
            Driver.FindElement(By.Name("password")).Submit();
            try
            {
                ClickOn("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div[2]/section/main/div/div/div/div/button");
            }
            catch { }
            try
            {
                ClickOn("/html/body/div[1]/div/div/div/div[2]/div/div/div[1]/div/div[2]/div/div/div/div/div[2]/div/div/div[3]/button[2]");
            }
            catch { }
        }


        public static void ClickOn(string Element)//Makes writing out a click command easier
        {
            WaitFor(Element);
            Driver.FindElement(By.XPath(Element)).Click();
        }


        public void GoTo(string url)
        {
            Driver.Url = url;
        }


        public static void WaitFor(string Element, string type = "XPath")//Wait for element to load, throw error if element doesn't load in time
        {
            Thread.Sleep(200);
            for (int i = 0; i < 40; i++)
            {
                try
                {
                    if (type == "XPath")
                    {
                        Driver.FindElement(By.XPath(Element));
                    }
                    if (type == "Name")
                    {
                        Driver.FindElement(By.Name(Element));
                    }
                    return;
                }
                catch (Exception)
                {
                    Thread.Sleep(200);
                }
            }
            throw new Exception("Load timeout, Element not found:" + Element);
        }


        public static bool CheckFor(string Element)//Returns true if element can be found, false if not
        {
            try
            {
                Driver.FindElement(By.XPath(Element));
                return true;
            }
            catch
            {
                return false;
            }

        }


        public List<String> NewMessages()//Creates/Updates list Inbox
        {
            List<String> NewMessageInbox = new List<String>();
            int Unread;

            Driver.Url = "https://www.instagram.com/direct/inbox/";
            //Get number of unread from red notifaction bubble, if blank quit
            try
            {
                WaitFor("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[1]/div/div/div/div/div[2]/div[4]/div/a/div/div[1]/div/div[2]/div/span");
                Unread = Int32.Parse(Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[1]/div/div/div/div/div[2]/div[4]/div/a/div/div[1]/div/div[2]/div/span")).GetAttribute("innerHTML"));
            }
            catch
            {
                Unread = 0;
                return null;
            }
            //Go to inbox page, set up scroll function
            Actions Scroll = new Actions(Driver);
            Console.WriteLine(Unread);
            //Scan inbox only as many times as there are unread messages, ex. 2 unread = 2 scans
            int InboxIndex = 1;
            for (int ReadIndex = 0; ReadIndex != Unread; ReadIndex++)
            {
                WaitFor("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/" +
                    "div/div/div/div[1]/div[3]/div/div/div/div/div[" + InboxIndex + "]/div/a/div");
                if (CheckFor("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[1]/div[3]/div/div/div/div/div[" + InboxIndex + "]/div/a/div/div[3]"))
                {
                    Console.WriteLine(Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[1]/div[3]/div/div/div/div/div[" + InboxIndex + "]/div/a")).GetAttribute("href"));
                    NewMessageInbox.Add(Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[1]/div[3]/div/div/div/div/div[" + InboxIndex + "]/div/a")).GetAttribute("href"));
                    InboxIndex++;
                }
            }
            return NewMessageInbox;
        }


        public string GetMessage()
        {
            try
            {
                WaitFor("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[2]/div[2]/div/div[1]/div/div/div[last()]/div[2]/div/div/div/div/div/div/div/div/div");
                String message = Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[2]/div[2]/div/div[1]/div/div/div[last()]/div[2]/div/div/div/div/div/div/div/div/div")).Text;
                return message;
            }
            catch
            {
                Console.WriteLine("Failed to get message");
                return null;
            }
        }


        public List<String> AcceptRequests()
        {
            List<String> NewMessageInbox = new List<string> { };
            Driver.Url = "https://www.instagram.com/direct/requests/";
            WaitFor("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[1]/div[2]");
            if (!CheckFor("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[1]/div[3]/div/div/div/div/a"))
            {
                return NewMessageInbox;
            }
            while (CheckFor("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[1]/div[3]/div/div/div/div/a"))
            {
                ClickOn("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[1]/div[3]/div/div/div/div/a");
                NewMessageInbox.Add(Driver.Url);
                ClickOn("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[2]/div[2]/div/div[2]/div/div[2]/div[5]/button/div/div");
            }
            return NewMessageInbox;
        }


        public void NewestPost(String user)
        {
            Driver.Url = "https://www.instagram.com/anon_nrhs/";
            ClickOn("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[2]/div[2]/section/main/div/div[2]/article/div/div/div/div[1]");
        }


        public void Comment(String text)
        {
            WaitFor("/html/body/div[1]/div/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/article/div/div[2]/div/div/div[2]/section[4]/div/form/textarea");
            Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/article/div/div[2]/div/div/div[2]/section[4]/div/form/textarea")).SendKeys(text);
            Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/article/div/div[2]/div/div/div[2]/section[4]/div/form/textarea")).SendKeys(Keys.Enter);
        }


        public class Messanger
        {
            string Name { get; set; }
            string Href { get; set; }
            string LastMessage { get; set; }

            public Messanger(string url)
            {
                if (Driver.Url != url) { Driver.Url = url; }
                Href = url;
                
            }

            private void onMessage()
            {
                if (Driver.Url != Href)
                {
                    Driver.Url = Href;
                }
            }


            public void sendMessage(string message)
            {
                onMessage();
                WaitFor("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[2]/div[2]/div/div[2]/div/div/div[2]/textarea");
                Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[2]/div[2]/div/div[2]/div/div/div[2]/textarea")).SendKeys(message);
                Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[2]/div[2]/div/div[2]/div/div/div[2]/textarea")).SendKeys(Keys.Enter);
            }


            public string getMessage()
            {
                onMessage();
                try
                {
                    WaitFor("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[2]/div[2]/div/div[1]/div/div/div[last()]/div[2]/div/div/div/div/div/div/div/div/div");
                    return LastMessage = Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div[1]/div/div/div/div[1]/div[1]/div/div[2]/div/section/div/div/div/div/div[2]/div[2]/div/div[1]/div/div/div[last()]/div[2]/div/div/div/div/div/div/div/div/div")).Text;

                }
                catch
                {
                    Console.WriteLine("Failed to get message");
                    return null;
                }

            }
        }


        public class Posts
        {
            public Posts()
            {

            }
        }
    }
}
