using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace CustomChangeWallpaper
{
    public partial class Form1 : Form
    {
        private string CurrentPath;
        private string WallpaperFolder = @"\Wallpaper\";
        private Bitmap BM = new Bitmap(Screen.PrimaryScreen.Bounds.Width / 10, Screen.PrimaryScreen.Bounds.Height / 10);
        List<Thread> al = new List<Thread>();
        private int globalCountScreen = 0;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private bool Flag
        {
            get
            {
                return flag;
            }
            set
            {
                flag = value;
            }
        }
        private bool flag = true;

        private Color[] colors;
        private Color[] colorsBlackWhite;
        private int previosSecond;
        private int currentCountBmp = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void ChangeWallpaper()
        {
            while (true)
            {
                if (Flag)
                {                 
                    previosSecond = DateTime.Now.Second;
                    picThumbnail.ImageLocation = CurrentPath + WallpaperFolder + globalCountScreen + ".bmp";
                    SetWallpaper(CurrentPath + WallpaperFolder + globalCountScreen + ".bmp", 2, 0);
                    globalCountScreen = globalCountScreen + 1;
                  
                    if (globalCountScreen >= currentCountBmp)
                    {
                        globalCountScreen = 0;
                    }

                }
            }
        }
        private void SetWallpaper(string WallpaperLocation, int WallpaperStyle, int TileWallpaper)
        {
            SystemParametersInfo(20, 0, WallpaperLocation, 0x01 | 0x02);
            RegistryKey rkWallPaper = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            rkWallPaper.SetValue("WallpaperStyle", WallpaperStyle);
            rkWallPaper.SetValue("TileWallpaper", TileWallpaper);
            rkWallPaper.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            previosSecond = DateTime.Now.Second;
            colors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Aqua, Color.Magenta, Color.Orange, Color.ForestGreen, Color.Coral };
            colorsBlackWhite = new Color[] { Color.Black, Color.White };
            picThumbnail.SizeMode = PictureBoxSizeMode.Zoom;
            picThumbnail.ImageLocation = GetCurrentWallpaper();
            CurrentPath = Application.StartupPath;
            if (!Directory.Exists(CurrentPath + WallpaperFolder))
                Directory.CreateDirectory(CurrentPath + WallpaperFolder);
            DirectoryInfo dirInfo = new DirectoryInfo(CurrentPath + WallpaperFolder);          
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                currentCountBmp = currentCountBmp + 1;
            }
            Thread mainThread = new Thread(ChangeWallpaper);
            al.Add(mainThread);
            mainThread.Start();
        }

        private string GetCurrentWallpaper()
        {
            RegistryKey rkWallPaper = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            string WallpaperPath = rkWallPaper.GetValue("WallPaper").ToString();
            rkWallPaper.Close();
            return WallpaperPath;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Thread t in al)
            {
                t.Abort();
            }
        }

        private void CreateImage_Click(object sender, EventArgs e)
        {
            Flag = false;
            if (!Flag)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(CurrentPath + WallpaperFolder);
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    file.Delete();
                }
                currentCountBmp = 0;
                globalCountScreen = 0;
                for (int i = 0; i < Convert.ToInt16(textBox1.Text); i++)
                {
                    Random rand = new Random();

                    for (int y = 0; y < Screen.PrimaryScreen.Bounds.Height / 10; y++)
                    {
                        for (int x = 0; x < Screen.PrimaryScreen.Bounds.Width / 10; x++)
                        {
                            Color tempColor = Color.Olive;
                            if (Convert.ToInt16(textBox2.Text) == 1)
                                tempColor = colors[rand.Next(0, 9)];
                            else if (Convert.ToInt16(textBox2.Text) == 2)
                                tempColor = colorsBlackWhite[rand.Next(0, 2)];
                            BM.SetPixel(x, y, tempColor);
                        }
                    }
                    BM.Save(CurrentPath + WallpaperFolder + currentCountBmp + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                    currentCountBmp = currentCountBmp + 1;
                    SetWallpaper(CurrentPath + WallpaperFolder + currentCountBmp + ".bmp", 2, 0);
                    picThumbnail.ImageLocation = CurrentPath + WallpaperFolder + currentCountBmp + ".bmp";
                }
            }
            Flag = true;
        }
    }
}
