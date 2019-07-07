using System;

namespace TwitterAccountGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //Twitter
            var email = "";
            var name = "TestApplication";
            var password = "123xxxxxx";
            //Gmail
            var gmailAddress = "";
            var gmailPassword = "";
            //アカウントデータ保存先
            var saveFilePath = "1.csv";

            Gmail gmail = new Gmail("imap.gmail.com", 993, gmailAddress, gmailPassword);
            Console.WriteLine("Logging to Gamil..ok!");
            Console.WriteLine("Enterキーで生成開始");
            Console.ReadLine();
            if (TwitterAPI.CheckEmailAvailable(email))
            {
                Twitter tw = new Twitter(email, password, name, gmail);
                tw.EmailVerification();
                string pinCode = tw.GetPinCode();
                Console.WriteLine($"PIN-Code:{pinCode}");
                var result = tw.EmailTask(pinCode);
                if (result)
                {
                    var user = tw.VerifyCredentials();
                    Console.WriteLine("ScreenName:" + user.ScreenName);
                    Console.WriteLine("Name:" + user.Name);
                    Console.WriteLine("UserId:" + user.UserId);
                    Console.WriteLine("Email:" + user.Email);
                    Console.WriteLine("Password:" + user.Password);
                    Console.WriteLine("CreatedTime:" + user.CreatedTime);
                    Console.WriteLine("Suspended:" + user.Suspended);
                    AccountStorage.Save(user, saveFilePath);
                }
            }
            else
            {
                Console.WriteLine("このメールアドレスは既に使われています。");
            }
            Console.ReadLine();
        }
    }
}