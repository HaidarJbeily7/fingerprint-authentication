using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using libzkfpcsharp;
using Sample;
using System.Drawing.Imaging;
using System.Net.Http;

namespace J
{
    static class Program
    {
        static IntPtr mDevHandle = IntPtr.Zero;
        static IntPtr mDBHandle = IntPtr.Zero;
        static IntPtr FormHandle = IntPtr.Zero;
        static bool bIsTimeToDie = false;
        static bool IsRegister = false;
        static bool bIdentify = true;
        static byte[] FPBuffer;
        static int RegisterCount = 0;
        const int REGISTER_FINGER_COUNT = 3;

        static byte[][] RegTmps = new byte[3][];
        static byte[] RegTmp = new byte[2048];
        static byte[] CapTmp = new byte[2048];

        static int cbCapTmp = 2048;
        static int cbRegTmp = 0;
        static int iFid = 0;

        private static int mfpWidth = 0;
        private static  int mfpHeight = 0;
        private static int mfpDpi = 0;

        const int MESSAGE_CAPTURED_OK = 0x0400 + 6;

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            int ret = init_device();
            if (ret != -1)
            {
                ret = open_device(0);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                F_welcome form = new F_welcome();
                    Application.Run(form);
                
            }
        }
        static void Authenticate()
        {
            while(iFid != 1)
            {
            }
            MessageBox.Show("in auth");
            int matched = 0;
            MemoryStream ms = new MemoryStream();
            BitmapFormat.GetBitmap(FPBuffer, mfpWidth, mfpHeight, ref ms);
            Bitmap bmp = new Bitmap(ms);
            saveImage(bmp);
            /* process matching here */
            if (matched == 0)
            {
                F_signup frm = new F_signup();
                frm.Show();
            }
        }
        
        static async void saveImage(Bitmap image)
        {

            
            string fileName =  new Random().Next() +".bmp";
            if (fileName != "" && fileName != null && image != null)
            {
                Bitmap bmp = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(image, 0, 0, bmp.Width, bmp.Height);

                }
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int bytes = bmpData.Stride * bmpData.Height;
                byte[] rgbValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
                Rectangle rect2 = new Rectangle(0, 0, bmp.Width, bmp.Height);

                Bitmap bit = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                System.Drawing.Imaging.BitmapData bmpData2 = bit.LockBits(rect2, System.Drawing.Imaging.ImageLockMode.ReadWrite, bit.PixelFormat);
                IntPtr ptr2 = bmpData2.Scan0;
                int bytes2 = bmpData2.Stride * bmpData2.Height;
                byte[] rgbValues2 = new byte[bytes2];
                System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes2);
                double colorTemp = 0;
                for (int i = 0; i < bmpData.Height; i++)
                {
                    for (int j = 0; j < bmpData.Width * 3; j += 3)
                    {
                        colorTemp = rgbValues[i * bmpData.Stride + j + 2] * 0.299 + rgbValues[i * bmpData.Stride + j + 1] * 0.578 + rgbValues[i * bmpData.Stride + j] * 0.114;
                        rgbValues2[i * bmpData2.Stride + j / 3] = (byte)colorTemp;
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(rgbValues2, 0, ptr2, bytes2);
                bmp.UnlockBits(bmpData);
                ColorPalette tempPalette;
                {
                    using (Bitmap tempBmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
                    {
                        tempPalette = tempBmp.Palette;
                    }
                    for (int i = 0; i < 256; i++)
                    {
                        tempPalette.Entries[i] = Color.FromArgb(i, i, i);
                    }
                    bit.Palette = tempPalette;
                }
                bit.UnlockBits(bmpData2);

                bit.Save(fileName, image.RawFormat);
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String AsString = sr.ReadToEnd();
                    byte[] AsBytes = new byte[AsString.Length];
                    Buffer.BlockCopy(AsString.ToCharArray(), 0, AsBytes, 0, AsBytes.Length);
                    String AsBase64String = Convert.ToBase64String(AsBytes);

                    HttpClient httpClient = new HttpClient();
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    //byte[] file = BitmapFormat.StructToBytes(bmp, 2048);
                    form.(AsBase64String, "fp");
                    HttpResponseMessage response = await httpClient.PostAsync("http://127.0.0.1:5000/register", form);

                    response.EnsureSuccessStatusCode();
                    httpClient.Dispose();
                }
                bit.Dispose();
            }
            
        }
    
        static int init_device()
        {
            int ret = zkfperrdef.ZKFP_ERR_OK;
            if ((ret = zkfp2.Init()) == zkfperrdef.ZKFP_ERR_OK)
            {
                int nCount = zkfp2.GetDeviceCount();
                if (nCount < 1)
                {
                    zkfp2.Terminate();
                    MessageBox.Show("No device connected!");
                }
            }
            else
            {
                MessageBox.Show("Initialize fail, ret=" + ret + " !");
            }
            return ret;
        }


         static int open_device(int ind)
         {

            int ret = zkfp.ZKFP_ERR_OK;
            if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(ind)))
            {
                MessageBox.Show("OpenDevice fail");
                return -1;
            }
            else
            {
                MessageBox.Show("OpenDevice Successeded");
            }
            //Thread.Sleep(5000);
            //Console.WriteLine(zkfp2.DBInit());
            //DialogResult dialogResult = MessageBox.Show(zkfp2.DBInit());
            if (IntPtr.Zero == (mDBHandle = zkfp2.DBInit()))
            {
                MessageBox.Show("Init DB fail");
                zkfp2.CloseDevice(mDevHandle);
                mDevHandle = IntPtr.Zero;
                return -1;
            }
            else
            {
                MessageBox.Show("Init DB Successeded");
            }
            int RegisterCount = 0;
            cbRegTmp = 0;
            iFid = 1;
            for (int i = 0; i < 3; i++)
            {
                RegTmps[i] = new byte[2048];
            }
            byte[] paramValue = new byte[4];
            int size = 4;
            zkfp2.GetParameters(mDevHandle, 1, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpWidth);

            size = 4;
            zkfp2.GetParameters(mDevHandle, 2, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpHeight);

            FPBuffer = new byte[mfpWidth * mfpHeight];

            size = 4;
            zkfp2.GetParameters(mDevHandle, 3, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpDpi);

            //textRes.AppendText("reader parameter, image width:" + mfpWidth + ", height:" + mfpHeight + ", dpi:" + mfpDpi + "\n");

            Thread captureThread = new Thread(new ThreadStart(DoCapture));
            captureThread.SetApartmentState(ApartmentState.STA);
            captureThread.IsBackground = false;
            captureThread.Start();
            bIsTimeToDie = false;
           
            return ret;
        }

        static void DoCapture()
        {
            MessageBox.Show("place you finger!");
            while (!bIsTimeToDie)
            {
                cbCapTmp = 2048;
                int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (ret == zkfp.ZKFP_ERR_OK)
                {
                    SendMessage(FormHandle, MESSAGE_CAPTURED_OK, IntPtr.Zero, IntPtr.Zero);
                    MessageBox.Show("Fingerprint has been taken!, ret=" + ret);
                    iFid = 1;
                    Thread auth = new Thread(new ThreadStart(Authenticate));
                    auth.SetApartmentState(ApartmentState.STA);
                    auth.Start();
                    auth.Join();
                    return;
                }
                else
                {
                    //MessageBox.Show("Fingerprint hasn't been taken!, ret=" + ret);
                }
                Thread.Sleep(200);
            }
        }
    }
}
