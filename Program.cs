
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace irc_bot
{
    class Program
    {

        private const string server = "chat.freenode.net";
        private const string gecos = "Cerk";
        private const string nick = "Priyomwindowsapp";
        private const string ident = "Priyomwindowsapp";
        private const string channel = "#priyom";
        private static string priyom;
        public string loop;

        static void Main(string[] args)
        {
            using (var client = new TcpClient())
            {
                Console.WriteLine("Welcome to Numbers Station Finder. From here you can search for Number Station broadcast times and other shortwave / HAM radio oddities right from your Windows desktop. I will also present a link to navigate to in your web browser to listen in real time. For note, I use priyom.orgs IRC server. While on the IRC server you will have the nickname Priyomwindowsapp. To display the next signal to broadcast type !n. To search for a signal type !n space enigma ID (!n HM01 for example). Program C 2020 keifmeister. There may be text characters I don't recognize. You may also messsage others on the server. If you want to keep private what station you are looking up, type PRIVMSG IvoSchwarz !n. You may also private message other Priyom users doing this. Program C 2020 Keifmeister.");
                Console.WriteLine("");
                Console.WriteLine("Current bug, may take a very long time tom loop back to typing another message.");
                Console.WriteLine($"Connecting to {server}");
                client.Connect(server, 6667);
                Console.WriteLine($"Connected: {client.Connected}");
                String message1 = "a";
                while (message1 == "a")
                    using (var stream = client.GetStream())
                    using (var writer = new StreamWriter(stream))
                    using (var reader = new StreamReader(stream))
                    {
                        writer.WriteLine($"USER {ident} * 8 {gecos}");
                        writer.WriteLine($"NICK {nick}");
                        // identify with the server so your bot can be an op on the channel
                        writer.WriteLine($"PRIVMSG NickServ :IDENTIFY {nick}");
                        writer.Flush();

                        while (client.Connected)
                        {
                            var data = reader.ReadLine();

                            if (data != null)
                            {
                                var d = data.Split(' ');
                                Console.WriteLine($"Data: {data}");

                                if (d[0] == "PING")
                                {
                                    writer.WriteLine("PONG");
                                    writer.Flush();
                                }

                                if (d.Length > 1)
                                {

                                    switch (d[1])
                                    {
                                        case "376":
                                        case "422":
                                            {
                                                writer.WriteLine($"JOIN {channel}");

                                                // communicate with everyone on the channel as soon as the bot logs 

                                                Console.WriteLine("What do you want to do?");
                                                string message = Convert.ToString(Console.ReadLine());
                                                writer.WriteLine(message);

                                                writer.Flush();
                                                writer.WriteLine(string.Format("PRIVMSG {0} :{1}", channel, message));
                                                writer.Flush();
                                                break;

                                            }



                                        case "PRIVMSG":

                                            {
                                                if (d.Length > 2)
                                                {
                                                    if (d[2] == priyom)
                                                    {
                                                        // someone sent a private message to the bot
                                                        var sender = data.Split('!')[0].Substring(1);
                                                        var message = data.Split(':')[2];
                                                        Console.WriteLine($"Message: {message}");
                                                        // handle all your bot logic here
                                                        writer.WriteLine($@"PRIVMSG {sender} : {message}");
                                                        writer.Flush();

                                                    }
                                                }
                                            }
                                            break;

                                    }
                                }
                            }
                        }
                    }
            }
        }
    }
}

