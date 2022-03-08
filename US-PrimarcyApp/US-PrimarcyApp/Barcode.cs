using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using AForge.Video.DirectShow;
using AForge.Video;

namespace US_PrimarcyApp
{
    public partial class Barcode : Form
    {
        FilterInfoCollection FilterInfoCollection;
        VideoCaptureDevice VideoCaptureDevice;
        public Barcode()
        {
            InitializeComponent();
        }

        private void Barcode_Load(object sender, EventArgs e)
        {
            FilterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in FilterInfoCollection)
            {
                cmbCamera.Items.Add(device.Name);
                cmbCamera.SelectedIndex = 0;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                VideoCaptureDevice = new VideoCaptureDevice(FilterInfoCollection[cmbCamera.SelectedIndex].MonikerString);
                VideoCaptureDevice.NewFrame += VideoCaptureDevice_newframe;
                VideoCaptureDevice.Start(); 
            }
            catch (Exception)
            {
                if (VideoCaptureDevice != null)
                {
                    if (VideoCaptureDevice.IsRunning) 
                    {
                        VideoCaptureDevice.Stop();
                    }
                }

            }
        }

        private void VideoCaptureDevice_newframe(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            BarcodeReader reader = new BarcodeReader();
            var result = reader.Decode(bitmap);


            if (result != null)
            {
                txtBar.Invoke(new MethodInvoker( delegate ()
                {
                    txtBar.Text = result.ToString();    
                }
                ));
            }
            pictureBox1.Image = bitmap;
        }
    }
}
