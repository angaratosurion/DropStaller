using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using DropNet;
using System.Diagnostics;
namespace DropStaller
{
    public partial class MainWindow : Form
    {
        public const string appkey = "wzwebe4tyxh7wlt";
        public const string appsecret = "e3off0z6jadipes";
        public const string dirname = "installer";
        DropNetClient client = new DropNetClient(appkey, appsecret, DropNetClient.AuthenticationMethod.OAuth1);
        public string authtoken = null;
        string filesinfo =Path.Combine(Application.StartupPath, "files.txt");
        string tempdir = Path.Combine(Application.StartupPath, "temp");
        public MainWindow()
        {
            InitializeComponent();
          

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            string url = null;
            client.GetToken();
            url = client.BuildAuthorizeUrl("http://www.google.com");
            this.webBrowser1.Navigate(url);
            if( !Directory.Exists(tempdir))
            {
                Directory.CreateDirectory(tempdir);


            }


        }
        void exited(object snd,EventArgs a)
        {
            string cont = null; ;
            foreach (string f in Directory.GetFiles(tempdir))
            {
                cont += "\n" + Path.GetFileName(f) ;

            }
            StreamWriter str = File.CreateText(filesinfo);
            str.Write(cont);
            str.Flush();
            str.Close();


        }
        bool appalreadyinstalled(string name)
        {
            bool ap = false;
            if (name != null)
            {
                if (File.Exists(filesinfo))
                {
                    FileStream strm = File.OpenRead(filesinfo);
                    StreamReader rdr = new StreamReader(strm);
                    string cont = rdr.ReadToEnd();
                    rdr.Close();
                    cont.Contains(name);

                }

            }

            return ap;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                WebBrowser wb = sender as WebBrowser;

                this.Text = wb.Url.ToString();
                if ((((wb.Url.ToString().Contains("google"))
                     &&(wb.DocumentTitle.Contains("Google"))) ) )
                    
                {
                    var accessToken = client.GetAccessToken();
                   
                       
                        //this.Hide();
                         if (client.Search(dirname, "/") != null)
                        {
                              if (client.GetMetaData("/" + dirname) != null)
                            {
                                DropNet.Models.MetaData metdata = client.GetMetaData(dirname);
                                foreach (DropNet.Models.MetaData t in metdata.Contents)
                                {

                                    if ((!t.Is_Dir) && (!appalreadyinstalled(t.Name)))
                                    {
                                        byte[] fil = client.GetFile(t.Path);
                                        File.WriteAllBytes(Path.Combine(tempdir, t.Name), fil);
                                        System.Diagnostics.ProcessStartInfo inf = new ProcessStartInfo();
                                        inf.Arguments = "/qn";
                                        inf.FileName = Path.Combine(tempdir, t.Name);
                                        Process p = new Process();
                                        p.StartInfo = inf;
                                        p.Exited += new EventHandler(exited);
                                        p.Start();



                                    }
                                }


                            }
                        



                    }

                }
            }
            catch (Exception ed)
            {

                throw (ed);
            }
        }
    }
}
