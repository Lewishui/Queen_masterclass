using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Gecko;
using ISR_System;
using QM.DB;

namespace QM.Buiness
{
    public enum ProcessStatus
    {
        初始化,
        登录界面,
        确认YES,
        第一页面,
        第二页面,
        Filter下拉,
        结束页面

    }
    public enum EapprovalProcessStatus
    {
        初始化,
        Current_Task,
        Task_Queue,
        Search,
        Process
    }


    public class clsAllnew
    {
        private readonly string xulrunnerPath = Application.StartupPath + "\\xulrunner";
        private const string testUrl = "http://webapp.hj8828.com/login.html";
        private ProcessStatus isrun = ProcessStatus.初始化;
        private EapprovalProcessStatus isrun1 = EapprovalProcessStatus.初始化;
        private bool isOneFinished = false;
        private Form viewForm;
        public ToolStripProgressBar pbStatus { get; set; }
        public ToolStripStatusLabel tsStatusLabel1 { get; set; }
        private WbBlockNewUrl MyWebBrower;
        WbBlockNewUrl myDoc = null;
        GeckoWebBrowser GmyDoc = null;
        private GeckoWebBrowser Browser;
        #region  Webbroswer
        public List<clsWEBINFO> ReadWEN()
        {

            try
            {
                InitialWebbroswer();

                HtmlElement Scope = null;
                HtmlElement View = null;
                int io = 0;

                while (!isOneFinished)
                {
                    System.Windows.Forms.Application.DoEvents();


                }
                System.Windows.Forms.Application.DoEvents();
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);

                throw;
            }
        }
        public void InitialWebbroswer()
        {
            try
            {

                MyWebBrower = new WbBlockNewUrl();
                //不显示弹出错误继续运行框（HP方可）
                MyWebBrower.ScriptErrorsSuppressed = true;
                #region new add

                MyWebBrower.AllowWebBrowserDrop = false;
                MyWebBrower.IsWebBrowserContextMenuEnabled = false;
                MyWebBrower.WebBrowserShortcutsEnabled = false;
                MyWebBrower.ObjectForScripting = this;
                //Uncomment the following line when you are finished debugging.
                //webBrowser1.ScriptErrorsSuppressed = true;

                MyWebBrower.DocumentText =
                  "<html><head><script>" +
                  "function test(message) { alert(message); }" +
                  "</script></head><body><button " +
                  "onclick=\"window.external.Test('called from script code')\">" +
                  "call client code from script code</button>" +
                  "</body></html>";

                #endregion
                MyWebBrower.BeforeNewWindow += new EventHandler<WebBrowserExtendedNavigatingEventArgs>(MyWebBrower_BeforeNewWindow);
                MyWebBrower.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(AnalysisWebInfo);
                MyWebBrower.Dock = DockStyle.Fill;

                //显示用的窗体
                viewForm = new Form();
                //viewForm.Icon=
                viewForm.ClientSize = new System.Drawing.Size(550, 600);
                viewForm.StartPosition = FormStartPosition.CenterScreen;
                viewForm.Controls.Clear();
                viewForm.Controls.Add(MyWebBrower);
                viewForm.FormClosing += new FormClosingEventHandler(viewForm_FormClosing);
                //显示窗体

                viewForm.Show();

                MyWebBrower.Url = new Uri("http://webapp.hj8828.com/login.html");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void viewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tsStatusLabel1.Text != " Search Finished  !")
            {
                if (MessageBox.Show("正在进行，是否中止?", "CCW", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (MyWebBrower != null)
                    {
                        if (MyWebBrower.IsBusy)
                        {
                            MyWebBrower.Stop();
                        }
                        MyWebBrower.Dispose();
                        MyWebBrower = null;
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        void MyWebBrower_BeforeNewWindow(object sender, WebBrowserExtendedNavigatingEventArgs e)
        {
            #region 在原有窗口导航出新页
            e.Cancel = true;//http://pro.wwpack-crest.hp.com/wwpak.online/regResults.aspx
            MyWebBrower.Navigate(e.Url);
            #endregion
        }
        protected void AnalysisWebInfo(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //  WbBlockNewUrl myDoc = sender as WbBlockNewUrl;
            myDoc = sender as WbBlockNewUrl;

            if (myDoc.Url.ToString().IndexOf("http://webapp.hj8828.com/login.html") >= 0)
            {
            }
        }
        #endregion

        #region GeckoWebBrowser

        public List<clsWEBINFO> ReadGeckoWEN()
        {
            try
            {
                Xpcom.Initialize(xulrunnerPath);
                isrun = ProcessStatus.初始化;
                InitAllCityData();
                while (!isOneFinished)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                return null;
                throw;
            }
        }
        private void InitAllCityData()
        {
            Browser = new GeckoWebBrowser();


            Browser.DocumentCompleted += new EventHandler<Gecko.Events.GeckoDocumentCompletedEventArgs>(Browser_DocumentCompleted_Init);

            GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;
            Browser.Dock = DockStyle.Fill;

            //显示用的窗体
            viewForm = new Form();
            //viewForm.Icon=
            viewForm.ClientSize = new System.Drawing.Size(550, 600);
            viewForm.StartPosition = FormStartPosition.CenterScreen;
            viewForm.Controls.Clear();
            viewForm.Controls.Add(Browser);
            viewForm.FormClosing += new FormClosingEventHandler(viewForm_FormClosing);
            viewForm.Show();
            Browser.Navigate(testUrl);
        }
        void Browser_DocumentCompleted_Init(object sender, EventArgs e)
        {
            GeckoWebBrowser br = sender as GeckoWebBrowser;
            if (br.Url.ToString() == "about:blank") { return; }


            GmyDoc = sender as GeckoWebBrowser;

            if (GmyDoc.Url.ToString().IndexOf("http://webapp.hj8828.com/login.html") >= 0 && isrun == ProcessStatus.初始化)
            {
                GeckoElement userName = null;
                GeckoHtmlElement submit = null;
                GeckoElementCollection userNames = GmyDoc.Document.GetElementsByTagName("input");
                foreach (GeckoHtmlElement item in userNames)
                {
                    if (item.GetAttribute("value") == "default")
                        submit = item;

                }
                //myBrowser.Document.GetHtmlElementById("vcodeA").Click();
                submit.Click();

                isrun = ProcessStatus.结束页面;

            }
            if (isrun == ProcessStatus.结束页面)
            {


                GeckoHtmlElement namevalue = null;
                GeckoHtmlElement password = null;
                GeckoHtmlElement submit = null;
                GeckoElementCollection userNames = GmyDoc.Document.GetElementsByTagName("input");
                GeckoElementCollection userNames1 = GmyDoc.Document.GetElementsByTagName("a");

                foreach (GeckoHtmlElement item in userNames)
                {
                    if (item.GetAttribute("id") == "username")
                        namevalue = item;
                    if (item.GetAttribute("id") == "password")
                        password = item;
                }
                foreach (GeckoHtmlElement item in userNames1)
                {
                    if (item.GetAttribute("id") == "ensure")
                        submit = item;

                }
                namevalue.SetAttribute("Value", "w7830");
                password.SetAttribute("Value", "123456");

                submit.Click();


                isrun = ProcessStatus.登录界面;
            }
            else if (isrun == ProcessStatus.登录界面)
            {


            }

        }


        #endregion
    }
}
