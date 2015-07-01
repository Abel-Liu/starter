using System;
using System.Windows;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Starter
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BugReport : Window
    {
        /// <summary>
        /// 初始化错误信息界面
        /// </summary>
        /// <param name="ea">异常实例</param>
        public BugReport(Exception ea)
        {
            InitializeComponent();
            label.Content = "很抱歉软件出现了异常.\r\r异常信息已记录,点击确定关闭程序.";
            runmessage.Text = ea.Message + "\r\r" + ea.StackTrace;
            this.ResizeMode = ResizeMode.NoResize;
            NoteMessage(runmessage.Text);
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if (MyWork.ConfigPath.CheckAutoBug())
                SendBug(runmessage.Text);
        }
        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="message">信息</param>
        private void NoteMessage(string message)
        {
            try
            {
                StreamWriter sw = new StreamWriter(MyWork.LogPath, true);
                sw.WriteLine(DateTime.Now.ToString() + "," + System.Environment.MachineName + "  By  " + System.Environment.UserName);
                sw.WriteLine(System.Environment.OSVersion.ToString());
                sw.WriteLine(message);
                sw.WriteLine();
                sw.Close();
            }
            catch { }
        }
        /// <summary>
        /// 发送错误报告
        /// </summary>
        /// <param name="message">主要信息</param>
        private void SendBug(string message)
        {
            try
            {
                MailMessage mail = SetMail(message);
                SmtpClient smtpClient = InitSmtp("smtp.163.com", 25);
                smtpClient.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
                smtpClient.SendAsync(mail, "000000000");
            }
            catch { }
        }

        void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 初始化邮件信息
        /// </summary>
        /// <param name="body">邮件正文</param>
        /// <param name="attachments">包含作为附件的文件路径</param>
        /// <param name="dispalyName">发件人显示名称</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="from">发送方邮件地址</param>
        /// <param name="to">接收方邮件地址</param>
        private MailMessage SetMail(string body,string[] attachments=null,string dispalyName=null, string subject = "错误报告", string from = "mymailforprogram@163.com", string to = "easystarter@163.com")
        {
            MailMessage mail = new MailMessage(from, to, subject, body);
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            if (attachments != null)
            {
                for (int i = 0; i < attachments.Length; i++)
                    mail.Attachments.Add(new Attachment(attachments[i], MediaTypeNames.Application.Octet));
            }
            return mail;
        }

        /// <summary>
        /// 初始化smtpClient
        /// </summary>
        /// <param name="ServerHost">服务器名称或IP地址</param>
        /// <param name="Port">端口</param>
        /// <param name="MailAddress">发件人邮件地址</param>
        /// <param name="MailPwd">发件人邮件密码</param>
        /// <returns></returns>
        private SmtpClient InitSmtp(string ServerHost, int Port, string MailAddress = "mymailforprogram@163.com", string MailPwd = "df(jk4@58.*2%mc9")
        {
            SmtpClient sc = new SmtpClient(ServerHost, Port);
            sc.Timeout = 0;
            sc.Credentials = new NetworkCredential(MailAddress, MailPwd);
            return sc;
        }
    }
}
