

using AForge.Imaging.Filters;
using DlibDotNet;
using DlibDotNet.Extensions;
using PaddleOCRSharp;
using Spire.OCR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;


namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        #region 公共变量
        private FrontalFaceDetector faceDetector;
        //private string strFileAddress;
        Timer tFile = new Timer();
        Timer tWord = new Timer();
        Timer tImage = new Timer();
        PaddleOCREngine engine;
        string strSavePath = "";
        DataTable dtIdCard = new DataTable();
        #endregion
        #region 加载事件
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            tWord.Interval = 2000;
            tWord.Tick += TWord_Tick;
            tImage.Tick += TImage_Tick;
            // 初始化人脸检测器
            faceDetector = Dlib.GetFrontalFaceDetector();
            OCRModelConfig config = null;
            OCRParameter oCRParameter = new OCRParameter();
            //建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。
            engine = new PaddleOCREngine(config, oCRParameter);
            //把数据放在表格里
            dtIdCard.Columns.Add("Id");
            dtIdCard.Columns.Add("OldFileName");
            dtIdCard.Columns.Add("DirectoryName");
            dtIdCard.Columns.Add("NewFileName");
            dtIdCard.Columns.Add("World");

            dtIdCard.Columns.Add("FaceCount");
            dtIdCard.Columns.Add("IsFace");
            dtIdCard.Columns.Add("LastDate");

        }
        // 图像预处理：调整亮度和对比度
        private static Bitmap PreprocessImage(Bitmap image)
        {
            // 调整亮度和对比度
            float brightness = 0.2f; // 调整亮度的值，范围为-1到1
            float contrast = 1.2f; // 调整对比度的值，大于1增加对比度，小于1减小对比度

            Bitmap processedImage = new Bitmap(image.Width, image.Height);

            // 遍历图像的每个像素
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    // 调整亮度和对比度
                    int newR = (int)(pixel.R * contrast + brightness * 255);
                    int newG = (int)(pixel.G * contrast + brightness * 255);
                    int newB = (int)(pixel.B * contrast + brightness * 255);

                    // 将像素限制在0-255之间
                    newR = Math.Max(0, Math.Min(255, newR));
                    newG = Math.Max(0, Math.Min(255, newG));
                    newB = Math.Max(0, Math.Min(255, newB));

                    Color newPixel = Color.FromArgb(pixel.A, newR, newG, newB);
                    processedImage.SetPixel(x, y, newPixel);
                }
            }

            // 返回预处理后的图像
            return processedImage;
        }
        private void TImage_Tick(object sender, EventArgs e)
        {

            // 加载图像
            var imagePath = lstBoxDirectory.Items[0].ToString();
            var image = Dlib.LoadImage<RgbPixel>(imagePath);
            // 创建人脸检测器
            using (var faceDetector = Dlib.GetFrontalFaceDetector())
            {
                // 进行人脸检测
                var faces = faceDetector.Operator(image);

                // 显示结果
                //MessageBox.Show($"图像中发现了 {faces.Length} 张人脸。");
            }

        }

        private void TWord_Tick(object sender, EventArgs e)
        {
            //OpenFileDialog ofd = new OpenFileDialog();

            //ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";

            //if (ofd.ShowDialog() != DialogResult.OK) return;
            tWord.Stop();
            string folderPath = lstBoxDirectory.Items[0].ToString();
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
            bool isExpire = false;
            string strFileName = "不确定库";
            for (int i = 0; i < files.Length; i++)
            {


                //string filePath = @"C:\path\to\image.jpg";

                // 获取文件扩展名
                string extension = Path.GetExtension(files[i]);
                string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };


                // 检查扩展名是否为常见的图片格式
                bool isImage = imageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);

                if (isImage)
                {
                    Console.WriteLine("文件是图片");
                    DataRow drNew = dtIdCard.NewRow();
                    drNew["OldFileName"] = files[i];
                    drNew["DirectoryName"] = Path.GetDirectoryName(files[i]);
                    dtIdCard.Rows.Add(drNew);
                    try
                    {
                        var imagebyte = File.ReadAllBytes(files[i]);
                        Bitmap bitmap = new Bitmap(new MemoryStream(imagebyte));
                        // 将图像转换为灰度图像
                        Bitmap grayscaleImage = Grayscale.CommonAlgorithms.BT709.Apply(bitmap);
                        // 将图像转换为二值图像
                        Bitmap thresholdedImage = new Threshold(100).Apply(grayscaleImage);
                        // 对图像进行去噪处理
                        Bitmap denoisedImage = new Median().Apply(thresholdedImage);
                        // 对图像进行对比度增强处理

                        Bitmap contrastEnhancedImage = new ContrastStretch().Apply(denoisedImage);
                            
                        

                        //OCRResult ocrResult = new OCRResult();
                        //ocrResult = engine.DetectText(contrastEnhancedImage);

                        //创建一个OcrScanner类的实例

                        OcrScanner scanner = new OcrScanner();

                        //调用OcrScanner.Scan(string fileName)方法扫描图片上的文字

                        scanner.Scan(files[i]);
                        string strText = scanner.Text.ToString();

                        if (strText != "")
                        {

                            lblIdCard.Text = strText;
                            var query = from row in dtIdCard.AsEnumerable()
                                        where row.Field<string>("OldFileName") == files[i]
                                        select row;

                            foreach (var row in query)
                            {
                                row.SetField("world", lblIdCard.Text);
                            }
                            if (lblIdCard.Text.IndexOf("公安") > 0)
                            {
                                // 使用正则表达式匹配数字
                                //MatchCollection matches = Regex.Matches(input, @"\d+");

                                // 提取匹配到的数字

                                Regex regex = new Regex(@"\d+");
                                string[] strIdCardS = lblIdCard.Text.Split('-');
                                MatchCollection matches = regex.Matches(strIdCardS[1]);
                                string strDate = "";
                                foreach (Match match in matches)
                                {
                                    strDate += match.Value;
                                }
                                if (strDate.Length == 7)
                                {
                                    strDate += "1";
                                }
                                if (strDate.Length > 8)
                                {
                                    strDate = strDate.Substring(0, 8);
                                }

                                DateTime maxDate = DateTime.ParseExact(strDate, "yyyyMMdd", null);

                                foreach (var row in query)
                                {
                                    row.SetField("LastDate", maxDate.ToString());
                                }
                                if (matches.Count > 0)
                                {
                                    isExpire = true;


                                    DateTime currentDate = dateTimePicker1.Value;

                                    if (maxDate > currentDate)
                                    {
                                        //Copy(lstBoxDirectory.Items[0].ToString(), strSavePath + "\\未过期库");
                                        strFileName = "未过期库";
                                        Console.WriteLine("最大日期大于当前日期");

                                    }
                                    else if (maxDate <= currentDate)
                                    {
                                        //Copy(lstBoxDirectory.Items[0].ToString(), strSavePath + "\\已过期库");
                                        strFileName = "已过期库";
                                        Console.WriteLine("最大日期小于当前日期");

                                    }
                                    else
                                    {
                                        Console.WriteLine("最大日期等于当前日期");
                                    }
                                }
                                //MessageBox.Show(ocrResult.Text, "识别结果");

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        strFileName = "出问题库";
                    }
                }

            }
            Copy(lstBoxDirectory.Items[0].ToString(), strSavePath + "\\" + strFileName);
            lstBoxDirectory.Items.Remove(lstBoxDirectory.Items[0]);
            if (lstBoxDirectory.Items.Count > 0)
            {
                if (progressBar1.Value < progressBar1.Maximum)
                {
                    progressBar1.Value = progressBar1.Value + 1;
                }
                tWord.Start();
                //tImage.Start();

            }
            else
            {
                lblIdCard.Text = "全部检测完成！";
            }
        }
        private void Copy(string sourceDirectory, string targetDirectory)
        {
            // 如果目标文件夹不存在，则创建
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            string strName = new DirectoryInfo(sourceDirectory).Name;
            targetDirectory = targetDirectory + "\\" + strName;
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // 获取源文件夹中的所有文件和子文件夹
            string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
            string targetFilePath = "";
            foreach (string filePath in files)
            {
                // 获取文件相对于源文件夹的相对路径
                string relativePath = filePath.Substring(sourceDirectory.Length + 1);

                // 构建目标文件夹中的文件路径
                targetFilePath = Path.Combine(targetDirectory, relativePath);

                // 确保目标文件夹的父文件夹存在
                Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));



                // 加载图像
                using (Bitmap bitmap = new Bitmap(filePath))
                {
                    // 将图像转换为 24 位 RGB 格式
                    using (Bitmap convertedBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                    {
                        using (Graphics g = Graphics.FromImage(convertedBitmap))
                        {
                            g.DrawImage(bitmap, 0, 0);
                        }

                        // 将图像转换为 DlibDotNet 的图像类型
                        Array2D<RgbPixel> image = convertedBitmap.ToArray2D<RgbPixel>();

                        // 创建人脸检测器

                        using (var faceDetector = Dlib.GetFrontalFaceDetector())
                        {
                            // 进行人脸检测
                            var faces = faceDetector.Operator(image);

                            string strWorld = "";
                            var query = from row in dtIdCard.AsEnumerable()
                                        where row.Field<string>("OldFileName") == filePath
                                        select row;

                            foreach (var row in query)
                            {

                                strWorld = row["world"].ToString();
                                if (strWorld.IndexOf("姓") >= 0)
                                {
                                    IEnumerable<DataRow> queryFileName = from rowFileName in dtIdCard.AsEnumerable()
                                                                         where rowFileName.Field<string>("DirectoryName") == sourceDirectory
                                                                         && rowFileName.Field<string>("isface") == "是"
                                                                         && rowFileName.Field<string>("NewFileName") == "1"
                                                                         select rowFileName;
                                    if (queryFileName.Count<DataRow>() == 0)
                                    {
                                        row.SetField("NewFileName", "1");
                                        row.SetField("faceCount", 1);
                                        row.SetField("isface", "是");
                                    }
                                    else
                                    {
                                        row.SetField("NewFileName", "3");
                                        row.SetField("faceCount", 2);
                                        row.SetField("isface", "是");
                                    }
                                }
                                else if (strWorld.IndexOf("公安") >= 0)
                                {

                                    row.SetField("NewFileName", "2");
                                    row.SetField("faceCount", 0);
                                    row.SetField("isface", "是");

                                }
                                else
                                {
                                    row.SetField("faceCount", faces.Length);
                                    row.SetField("isface", "是");
                                }


                            }

                        }


                    }
                }
            }

            IEnumerable<DataRow> queryDirectory = from row in dtIdCard.AsEnumerable()
                                                  where row.Field<string>("DirectoryName") == sourceDirectory
                                                  && row.Field<string>("isface") == "是"
                                                  select row;

            foreach (DataRow row in queryDirectory)
            {
                //targetFilePath = row["OldFileName"].ToString();
                // 获取文件名和后缀
                string fileName = Path.GetFileNameWithoutExtension(targetFilePath);
                string fileExtension = Path.GetExtension(targetFilePath);
                string strFileName = "";
                if (row["NewFileName"].ToString() != "")
                {
                    strFileName = row["NewFileName"].ToString();
                }
                else
                {
                    if (row["faceCount"].ToString() == "0")
                    {
                        IEnumerable<DataRow> queryFileName = from rowFileName in dtIdCard.AsEnumerable()
                                                             where rowFileName.Field<string>("DirectoryName") == sourceDirectory
                                                             && rowFileName.Field<string>("isface") == "是"
                                                             && rowFileName.Field<string>("NewFileName") == "2"
                                                             select rowFileName;
                        if (queryFileName.Count<DataRow>() == 0)
                        {
                            strFileName = "2";
                            row.SetField("NewFileName", "2");
                        }
                    }
                    else if (row["faceCount"].ToString() == "1")
                    {
                        IEnumerable<DataRow> queryFileName = from rowFileName in dtIdCard.AsEnumerable()
                                                             where rowFileName.Field<string>("DirectoryName") == sourceDirectory
                                                             && rowFileName.Field<string>("isface") == "是"
                                                             && rowFileName.Field<string>("NewFileName") == "1"
                                                             select rowFileName;
                        if (queryFileName.Count<DataRow>() == 0)
                        {
                            strFileName = "1";
                            row.SetField("NewFileName", "1");
                        }

                    }
                    else if (row["faceCount"].ToString() == "2")
                    {
                        IEnumerable<DataRow> queryFileName = from rowFileName in dtIdCard.AsEnumerable()
                                                             where rowFileName.Field<string>("DirectoryName") == sourceDirectory
                                                             && rowFileName.Field<string>("isface") == "是"
                                                             && rowFileName.Field<string>("NewFileName") == "3"
                                                             select rowFileName;
                        if (queryFileName.Count<DataRow>() == 0)
                        {
                            strFileName = "3";
                            row.SetField("NewFileName", "3");
                        }
                    }
                }
                if (strFileName == "")
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        IEnumerable<DataRow> queryFileName = from rowFileName in dtIdCard.AsEnumerable()
                                                             where rowFileName.Field<string>("DirectoryName") == sourceDirectory
                                                             && rowFileName.Field<string>("isface") == "是"
                                                             && rowFileName.Field<string>("NewFileName") == i.ToString()
                                                             select rowFileName;
                        if (queryFileName.Count<DataRow>() == 0)
                        {
                            strFileName = i.ToString();
                            row.SetField("NewFileName", i.ToString());
                            break;
                        }
                    }
                }
                // 新的文件名
                string newFileName = strFileName + fileExtension;

                // 获取目录路径
                string directoryPath = Path.GetDirectoryName(targetFilePath);


                // 拼接新的文件路径
                string destinationFilePath = Path.Combine(directoryPath, newFileName);
                if (File.Exists(destinationFilePath))
                {
                    destinationFilePath = Path.Combine(directoryPath, fileName + fileExtension);
                }
                // 文件重命名
                // 复制文件
                string filePath = row["OldFileName"].ToString();
                File.Copy(filePath, destinationFilePath, true);
            }


            Console.WriteLine("文件夹复制完成。");
        }
        #endregion
        #region 提交事件

        private void btnImageUrl_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    string folderPath = folderDialog.SelectedPath;

                    // 检查文件夹是否存在
                    if (Directory.Exists(folderPath))
                    {
                        // 获取文件夹中的所有子文件夹和文件
                        string[] subdirectories = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories);
                        //string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

                        // 清空之前的列表
                        lstBoxDirectory.Items.Clear();
                        dtIdCard.Clear();
                        // 显示所有子文件夹
                        foreach (string subdirectory in subdirectories)
                        {
                            lstBoxDirectory.Items.Add(subdirectory);

                        }

                        // 显示所有文件
                        //foreach (string file in files)
                        //{
                        //    folderContentsListBox.Items.Add($"文件：{file}");
                        //}
                        progressBar1.Maximum = lstBoxDirectory.Items.Count;
                        progressBar1.Value = 0;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("文件夹不存在！");
                    }
                }
            }
        }
        private void btnStart_Click(object sender, EventArgs e)
        {

            if (btnStart.Text == "开始识别")
            {
                if (lstBoxDirectory.Items.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("请选择一个文件夹");
                    return;
                }
                if (strSavePath == "")
                {
                    System.Windows.Forms.MessageBox.Show("请选择一个文件夹来保存文件");
                    return;
                }
                btnStart.Text = "停止识别";
                tWord.Start();
            }
            else
            {
                btnStart.Text = "开始识别";
                tWord.Stop();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    strSavePath = folderDialog.SelectedPath;
                }
            }
        }




        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg, *.png)|*.jpg;*.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 加载图像
                    var imagePath = openFileDialog.FileName;
                    // 加载图像
                    Bitmap bitmap = new Bitmap(imagePath);

                    // 将图像转换为 24 位 RGB 格式
                    Bitmap convertedBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(convertedBitmap))
                    {
                        g.DrawImage(bitmap, 0, 0);
                    }
                    //string imagePath = @"F:\360MoveData\Users\Administrator\Desktop\涓嶇‘瀹氬簱\鑻宠秴\CIMG5624.JPG";

                    Array2D<RgbPixel> image = convertedBitmap.ToArray2D<RgbPixel>();
                    // 将图像转换为 DlibDotNet 的图像类型
                    //Array2D<RgbPixel> image = Dlib.LoadImage<RgbPixel>(imagePath);
                    //var image = Dlib.LoadImage<RgbPixel>(imagePath);
                    // 创建人脸检测器
                    using (var faceDetector = Dlib.GetFrontalFaceDetector())
                    {
                        // 进行人脸检测
                        var faces = faceDetector.Operator(image);

                        // 显示结果
                        //MessageBox.Show($"图像中发现了 {faces.Length} 张人脸。");
                    }
                }
            }
        }
    }
}
