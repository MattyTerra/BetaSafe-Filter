using NsfwSharp;
using OpenCvSharp;
using SkiaSharp;
using System.Diagnostics;

namespace BetaSafeFilter
{
    public partial class Form1 : Form
    {
        private string ImageFilePath;
        private readonly NSFWCensorService _NSFWcensorService;
        private readonly NSFWCensorService _SFWcensorService;
        private readonly string _ProjectRoot = Environment.CurrentDirectory;

        //settings
        private List<string> _CensorsList = new List<string>();
        private Options _Option = new Options();
        private CensorType _CensorType = CensorType.Pixelate;

        public Form1()
        {


            InitializeComponent();

            for (int i = 0; i < CensorsChecklist.Items.Count; i++)
            {
                CensorsChecklist.SetItemChecked(i, true);
                _CensorsList.Add(CensorsChecklist.Items[i].ToString());
            }

            foreach (object Selection in CensorsChecklist.CheckedItems)
            {
                _CensorsList.Add(Selection.ToString());
            }
            PixelLabel.Text = PixelationDensity.Value.ToString();

            //_censorService = new NSFWCensorService(@"erax_nsfw_yolo11m.onnx", 0.20);
            _NSFWcensorService = new NSFWCensorService(@"erax_nsfw_yolo11m.onnx", .20);  //Not safe for work variant
            _SFWcensorService = new NSFWCensorService(@"YOLO26SFW.onnx", .20);
            _ProjectRoot = Environment.CurrentDirectory;
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

            if (_CensorsList.Contains("Non-Nude"))
                ImageFile = _SFWcensorService.CensorImage(ImageFile, _Option, censorType: _CensorType);
            if (_CensorsList.Contains("Nudes"))
                ImageFile = _NSFWcensorService.CensorImage(ImageFile, _Option, censorType: _CensorType);

            CensorImg.Image = ImageFile;
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

            if (_CensorsList.Contains("Non-Nude"))
                _SFWcensorService.CensorVideoFast(ImageFilePath, VideoPath, _Option, censorType: _CensorType);

            if (_CensorsList.Contains("Nudes"))
                _NSFWcensorService.CensorVideoFast(ImageFilePath, VideoPath, _Option, censorType: _CensorType);

            MessageBox.Show("Complete!");

            CensorVid.Enabled = true;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void ImageTab_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CensorsChecklist_SelectedIndexChanged(object sender, EventArgs e)
        {
            _CensorsList.Clear();
            foreach (object Selection in CensorsChecklist.CheckedItems)
            {
                _CensorsList.Add(Selection.ToString());
            }
            Debug.WriteLine(_CensorsList.ToString());
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void PixelateButton_CheckedChanged(object sender, EventArgs e)
        {
            _CensorType = CensorType.Pixelate;
        }

        private void CensorBoxButton_CheckedChanged(object sender, EventArgs e)
        {
            _CensorType = CensorType.SolidColor;
        }

        private void BlurButton_CheckedChanged(object sender, EventArgs e)
        {
            _CensorType = CensorType.GaussianBlur;
        }

        private void PixelationDensity_Scroll(object sender, EventArgs e)
        {
            _Option = new Options(PixelateFactor: PixelationDensity.Value);
            PixelLabel.Text = PixelationDensity.Value.ToString();
        }

        private void BoxColorButton_Click(object sender, EventArgs e)
        {
            CensorBoxColor.AllowFullOpen = true;
            CensorBoxColor.Color = Color.Black;
            if (CensorBoxColor.ShowDialog() == DialogResult.OK)
            {
                _Option= new Options(CensorColor: CensorBoxColor.Color);
            }
        }
    }
}
