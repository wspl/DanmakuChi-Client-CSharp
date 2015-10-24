using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ComponentModel;
using System.Web.Security;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;
using WebSocketSharp;

namespace DanmakuChi {

    /// <summary>
    /// Interactive logic of MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public DanmakuCurtain dmkCurt;
        public Boolean isConnected = false;
        public WebSocket ws;
        public MainWindow() {
            try {
                InitializeComponent();

                AppendLog("Welcome to DanmakuChi CSharp Client!");

                // Load Configuration
                var content = File.ReadAllText("./config.yaml");
                var input = new StringReader(content);
                var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
                var config = deserializer.Deserialize<Config>(input);

                textServer.Text = config.Session.server;
                textChannel.Text = config.Session.channel;
                textWechat.Text = config.Wechat.url;
                chkShadow.IsChecked = config.Advanced.enableShadow;

            } catch (Exception e) {
                AppendLog(e.Message);
            }
        }

        public class Config {
            public Session Session { get; set; }
            public Wechat Wechat { get; set; }
            public Advanced Advanced { get; set; }
        }
        public class Session {
            public string server { get; set; }
            public string channel { get; set; }
        }
        public class Wechat {
            public string url { get; set; }
        }
        public class Advanced {
            public bool enableShadow { get; set; }
        }

        private void btnShowDmkCurt_Click(object sender, RoutedEventArgs e) {
            dmkCurt = new DanmakuCurtain(chkShadow.IsChecked.Value);
            dmkCurt.Show();
        }

        private void btnShotDmk_Click(object sender, RoutedEventArgs e) {
            if (dmkCurt != null) {
                Random ran = new Random();
                var text = "2";
                for (var i = 0; i < ran.Next(1, 40); i += 1) {
                    text += "3";
                }
                dmkCurt.Shoot(text);
            } else {
                MessageBox.Show("Cannot find any curtains.");
            }
        }
        private void InitDanmaku() {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                dmkCurt = new DanmakuCurtain(chkShadow.IsChecked.Value);
                dmkCurt.Show();
                isConnected = true;
                btnConnect.IsEnabled = true;
                btnConnect.Content = "Disconnect";
            }));
        }
        private void AppendLog(string text) {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                listLog.Items.Add("[" + DateTime.Now.ToString() + "] " + text);
                listLog.SelectedIndex = listLog.Items.Count - 1;
                listLog.ScrollIntoView(listLog.SelectedItem);
            }));
        }
        private void ShootDanmaku(string text) {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                dmkCurt.Shoot(text);
                AppendLog(text);
            }));
        }
        private void button_Click(object sender, RoutedEventArgs e) {
            if (!isConnected) {
                btnConnect.Content = "Connecting...";
                btnConnect.IsEnabled = false;

                var server = textServer.Text;
                var channel = textChannel.Text;
                
                ws = new WebSocket(server + "/ws?channel=" + channel);

                ws.OnMessage += (s, ee) => {
                    int dividerPos = ee.Data.IndexOf(':');
                    string type = ee.Data.Substring(0, dividerPos);
                    string body = ee.Data.Substring(dividerPos + 1);
                    switch (type) {
                        case "INFO":
                            if (body == "OK") {
                                AppendLog("Successfully joined " + channel);
                                InitDanmaku();
                            } else {
                                AppendLog("Channel " + channel + " does not exist.");
                                CancelDMK();
                            }
                            break;
                        case "DANMAKU":
                            ShootDanmaku(body);
                            break;
                    }
                };
                ws.OnClose += (s, ee) => {
                    AppendLog("Disconnected!");
                };
                ws.Connect();
            } else {
                CancelDMK();
            }
        }
        private void CancelDMK() {
            ws.Close();
            btnConnect.Content = "Connect";
            isConnected = false;
            btnConnect.IsEnabled = true;
            if (dmkCurt != null) {
                dmkCurt.Close();
            }
        }
        private void SocketDotIO(object sender, DoWorkEventArgs e) {
            var server = ((string[])e.Argument)[0].ToString();
            var channel = ((string[])e.Argument)[1].ToString();
            var channelMd5 = FormsAuthentication.HashPasswordForStoringInConfigFile(channel, "MD5");

            var ws = new WebSocket(server + "/ws?channel=" + channel);
            ws.OnMessage += (s, ee) => {
                int dividerPos = ee.Data.IndexOf(':');
                string type = ee.Data.Substring(0, dividerPos);
                string body = ee.Data.Substring(dividerPos + 1);
                switch (type) {
                    case "INFO":
                        if (body == "OK") {
                            AppendLog("Successfully joined " + channel);
                            InitDanmaku();
                        } else {
                            AppendLog("Channel " + channel + " does not exist.");
                            CancelDMK();
                        }
                        break;
                    case "DANMAKU":
                        ShootDanmaku(body);
                        break;
                }
            };
            ws.OnClose += (s, ee) => {
                AppendLog("DEAD");
            };
            ws.Connect();
        }

        private void Window_Closed(object sender, EventArgs e) {
            Application.Current.Shutdown();
        }

        private void btnQRCode_Click(object sender, RoutedEventArgs e) {
            QRCode qrcode = new QRCode(textWechat.Text + "?dmk_channel=" + textChannel.Text, "Channel QRCode");
            qrcode.Show();
        }
    }
}
