using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using S22.Imap;

class Gmail
{
    private string HostName
    {
        get; set;
    }

    private int Port
    {
        get; set;
    }

    private string MailAddress
    {
        get; set;
    }

    private string Password
    {
        get; set;
    }

    private ImapClient Client
    {
        get; set;
    }

    private MailMessage Message
    {
        get; set;
    }

    public Gmail(string _hostName, int _port, string _mailAddress, string _passWord)
    {
        this.HostName = _hostName;
        this.Port = _port;
        this.MailAddress = _mailAddress;
        this.Password = _passWord;

        Client = new ImapClient(this.HostName, this.Port, this.MailAddress, this.Password, AuthMethod.Login, true);
    }

    public string ReceiveTopUnseenMessage(string SenderAddress)
    {
        Client = new ImapClient(this.HostName, this.Port, this.MailAddress, this.Password, AuthMethod.Login, true);
        IEnumerable<uint> uids = Client.Search(SearchCondition.Unseen());
        if (uids.Count<uint>() > 0)
        {
            foreach (var uid in uids)
            {
                var message = Client.GetMessage(uid, true);
                if (message.From.Address == SenderAddress)
                {
                    return message.Body;
                }
            }
        }
        return "NotFound";
    }

    public void Receive()
    {
        if (!Client.Supports("IDLE"))
        {
            Console.WriteLine("Server does not support IMAP Idle");
            return;
        }
        //イベントハンドラIdleMessageEventArgsを用いて、リアルタイムに新着メッセージを受信
        Client.NewMessage += new EventHandler<IdleMessageEventArgs>(OnNewMessage);
    }

    private void OnNewMessage(object sender, IdleMessageEventArgs e)
    {
        Message = e.Client.GetMessage(e.MessageUID, FetchOptions.Normal);
        Console.WriteLine("NewMessage");
        Console.WriteLine($"From:{Message.From}");
        Console.WriteLine($"Subject:{Message.Subject}");
        Console.WriteLine($"Body:{Message.Body}");
        Console.WriteLine();
    }
}