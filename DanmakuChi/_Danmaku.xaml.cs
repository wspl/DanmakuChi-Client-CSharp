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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DanmakuChi {
    /// <summary>
    /// Danmaku.xaml 的交互逻辑
    /// </summary>
    public class Danmaku : TextBlock {
        public int FontSize = 26;
        public Color Foreground = Colors.White;
        public string Text = "Hello DanmakuChi!";

        public Danmaku() {
            InitializeComponent();
        }
    }

    public class DanmakuManager {
        private Grid container;

        private int lineHeight = 40;
        private int paddingTop = 20;

        private Boolean[] isOccupy;
        private int lines;
        
        public int usableLine() {
            for (int line = 0; line < lines; line += 1) {
                if (!this.isOccupy[line]) {
                    this.isOccupy[line] = true;
                    return line;
                }
            }
            return -1;
        }

        public int lineLocationY(int line) {
            return (line * this.lineHeight) + this.paddingTop;
        }

        public DanmakuManager(Grid container) {
            this.container = container;

            this.lines = (int)(container.RenderSize.Height / lineHeight);
            this.isOccupy = new Boolean[this.lines];
        }

        public void Shoot(string text) {
            var line = usableLine();

            if (line == -1) {
                return;
            }

            // The top margin of danmaku
            var locationY = lineLocationY(line);

            // Danmaku initilization and display
            var danmaku = new Danmaku();
            Random random = new Random();
            danmaku.Text = "2";
            var testLength = random.Next(1, 30);
            for (var i = 0; i < testLength; i += 1) {
                danmaku.Text += "3";
            }
            danmaku.Margin = new Thickness(0, locationY, 0, 0);
            this.container.Children.Add(danmaku);

            // Some args. for flying control
            var LengthNum = Encoding.Default.GetByteCount(danmaku.Text);
            var danmakuWidth = LengthNum * 16;
            var speedScale = LengthNum < 20 ? (0.015 * LengthNum) + 0.9 : 0.2 + Math.Pow(0.9999, LengthNum - 20);
            var animDuration = 4000 * speedScale;

            // Initilizing animation
            var anim = new DoubleAnimation();
            anim.From = this.container.RenderSize.Width;
            anim.To = -danmakuWidth;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(animDuration));
            TranslateTransform trans = new TranslateTransform();
            danmaku.RenderTransform = trans;

            // Handling the end of danmaku
            anim.Completed += new EventHandler(delegate (Object o, EventArgs a) {
                this.container.Children.Remove(danmaku);
            });

            // Managing the danmaku lines
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(delegate (Object o, EventArgs a) {
                Point relativePoint = danmaku.TransformToAncestor(container)
                          .Transform(new Point(0, 0));
                if (relativePoint.X < container.ActualWidth - danmakuWidth) {
                    timer.Stop();
                    this.isOccupy[line] = false;
                }
            });
            timer.Start();

            // Play animation
            trans.BeginAnimation(TranslateTransform.XProperty, anim);
        }
    }
}
