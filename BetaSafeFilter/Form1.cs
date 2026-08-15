using NsfwSharp;
using OpenCvSharp;
using SkiaSharp;

namespace BetaSafeFilter
{
    public partial class Form1 : Form
    {
        private string ImageFilePath;
        private readonly NSFWCensorService _censorService1;
        private readonly NSFWCensorService _censorService2;
        private readonly string _ProjectRoot = Environment.CurrentDirectory;
        public Form1()
        {
            InitializeComponent();

            //_censorService = new NSFWCensorService(@"erax_nsfw_yolo11m.onnx", 0.20);
            _censorService1 = new NSFWCensorService(@"erax_nsfw_yolo11m.onnx", .20);
            _censorService2 = new NSFWCensorService(@"YOLO26SFW.onnx",.20);
            _ProjectRoot= Environment.CurrentDirectory;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog OFD = new OpenFileDialog()) //Standard Open File Dialog Box, Using means that memory is freed up after uploading
            {
                OFD.Title = "Select an Image";
                OFD.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp; *.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                OFD.Multiselect = false;

                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    ImageFilePath = OFD.FileName; //Save file location of the new image


                    CensorImg.Image?.Dispose(); //Clear any previous image ?. means if its null nothing happens
                    CensorImg.Image = null; //set Value to Null to avoid any issues

                    FileNameLabel.Text = "Image Loaded: " + System.IO.Path.GetFileName(ImageFilePath);
                }
            }

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CensorButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ImageFilePath))
            {
                MessageBox.Show("No File Uploaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Bitmap ImageFile = new Bitmap(ImageFilePath);
            CensorButton.Enabled = false;
            CensorImg.Image?.Dispose();
            Bitmap CensoredImage = _censorService1.CensorImage(ImageFile,censorType:CensorType.SolidColor,color:Scalar.HotPink);
            CensoredImage = _censorService2.CensorImage(CensoredImage, censorType: CensorType.Pixelate, color:Scalar.MintCream);

            CensorImg.Image = CensoredImage;
            CensorButton.Enabled = true;
        }

        private void VidButton_Click(object sende, EventArgs e)
        {
            using (OpenFileDialog OFD = new OpenFileDialog()) //Standard Open File Dialog Box, Using means that memory is freed up after uploading
            {
                OFD.Title = "Select an Video";
                OFD.Filter = "Video Files (*.mp4)|*.mp4";
                OFD.Multiselect = false;

                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    ImageFilePath = OFD.FileName; //Save file location of the new image


                    CensorImg.Image?.Dispose(); //Clear any previous image ?. means if its null nothing happens
                    CensorImg.Image = null; //set Value to Null to avoid any issues

                    FileNameLabel.Text = "Video Loaded: " + System.IO.Path.GetFileName(ImageFilePath);
                }
            }
        }

        private void CensorVid_Click(object sender, EventArgs e)
        {
            //start with error handling If no image uploaded, dont even start
            if (String.IsNullOrEmpty(ImageFilePath))
            {
                MessageBox.Show("Error: No Video Uploaded");
                return;
            }
            
            CensorVid.Enabled = false;
            CensorImg.Image?.Dispose();


            String VideoPath = Path.Combine(_ProjectRoot, @"Frameholder\Video.mp4");

            _censorService1.CensorVideoFast(ImageFilePath, VideoPath, censorType: CensorType.SolidColor, color: Scalar.White);
            _censorService2.CensorVideoFast(ImageFilePath, VideoPath, censorType: CensorType.SolidColor, color: Scalar.LightGoldenrodYellow);
            MessageBox.Show("Complete!");

            CensorVid.Enabled = true;
        }
    }
}
