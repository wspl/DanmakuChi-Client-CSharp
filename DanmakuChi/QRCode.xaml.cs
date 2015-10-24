using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ThoughtWorks.QRCode.Codec;

namespace DanmakuChi {
    /// <summary>
    /// QRCode.xaml 的交互逻辑
    /// </summary>
    public partial class QRCode : Window {
        public QRCode(string data, string title) {
            InitializeComponent();
            textBox.Text = data;
            Title = title;
            var qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeScale = 16;
            var image = qrCodeEncoder.Encode(data, Encoding.UTF8);
            var imageSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
               image.GetHbitmap(),
               IntPtr.Zero,
               Int32Rect.Empty,
               BitmapSizeOptions.FromWidthAndHeight(image.Width, image.Height));
            imgBox.Source = imageSrc;
        }
    }
}
