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
using Quobject.SocketIoClientDotNet.Client;
using System.ComponentModel;
using System.Web.Security;

namespace DanmakuChi {

    /// <summary>
    /// Interactive logic of MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public DanmakuCurtain dmkCurt;
        public Boolean isConnected = false;
        public BackgroundWorker bg = new BackgroundWorker();
        public MainWindow() {
            InitializeComponent();
            bg.WorkerSupportsCancellation = true;
            bg.DoWork += new DoWorkEventHandler(SocketDotIO);
        }

        private void btnShowDmkCurt_Click(object sender, RoutedEventArgs e) {
            dmkCurt = new DanmakuCurtain(chkShadow.IsChecked.Value);
            dmkCurt.Show();
        }

        private void btnShotDmk_Click(object sender, RoutedEventArgs e) {
            Random ran = new Random();
            var text = "2";
            for (var i = 0; i < ran.Next(1, 40); i += 1) {
                text += "3";
            }
            dmkCurt.Shoot(text);
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
            }));
        }
        private void ShootDanmaku(string text) {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                dmkCurt.Shoot(text);
                listLog.Items.Add("[" + DateTime.Now.ToString() + "] " + text);
            }));
        }
        private void button_Click(object sender, RoutedEventArgs e) {
            if (!isConnected) {
                btnConnect.Content = "Connecting...";
                btnConnect.IsEnabled = false;
                bg.RunWorkerAsync(new String[] { textServer.Text, textChannel.Text });
            } else {
                CancelDMK();
            }
        }
        private void CancelDMK() {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                bg.CancelAsync();
                btnConnect.Content = "Connect";
                isConnected = false;
                btnConnect.IsEnabled = true;
                if (dmkCurt != null) {
                    dmkCurt.Close();
                }
            }));
        }
        private void SocketDotIO(object sender, DoWorkEventArgs e) {
            var server = ((string[])e.Argument)[0].ToString();
            var socket = IO.Socket(server);
            var channel = ((string[])e.Argument)[1].ToString();
            var channelMd5 = FormsAuthentication.HashPasswordForStoringInConfigFile(channel, "MD5");
            socket.On(Socket.EVENT_CONNECT, () => {
                socket.Emit("channel", channelMd5);
            });
            socket.On("channel", (data) => {
                AppendLog("Successfully connected to " + server);
                if (data.ToString() != "no") {
                    AppendLog("Successfully joined " + channel);
                    InitDanmaku();
                } else {
                    AppendLog("Channel " + channel + " does not exist.");
                    CancelDMK();
                }
            });
            socket.On(channelMd5, (data) => {
                ShootDanmaku(data.ToString());
            });
            bg.RunWorkerCompleted += (s, ee) => {
                socket.Close();
            };
        }

        private void Window_Closed(object sender, EventArgs e) {
            Application.Current.Shutdown();
        }
    }
}
