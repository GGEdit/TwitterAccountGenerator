using System;
using System.IO;

namespace TwitterAccountGenerator
{
    class AccountStorage
    {
        public static void Save(TwitterUser _user, string _saveFileName)
        {
            try
            {
                if (_user != null && _saveFileName.Contains(".csv"))
                {
                    var content = $"{_user.ScreenName}, {_user.Email}, {_user.Password}, {_user.CreatedTime} {Environment.NewLine}";
                    File.AppendAllText(_saveFileName, content);
                    Console.WriteLine("Save..ok!");
                }
                else
                {
                    Console.WriteLine("保存形式が不正なため、保存されませんでした。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveError:{ex.Message}");
            }
        }
    }
}
