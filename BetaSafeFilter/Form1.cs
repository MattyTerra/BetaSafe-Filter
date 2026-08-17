using NsfwSharp;
using OpenCvSharp;
using SkiaSharp;
using System.Diagnostics;

namespace BetaSafeFilter
{
    public partial class Form1 : Form
    {
        private string SourceFilePath;
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
                    SourceFilePath = OFD.FileName; //Save file location of the new image


                    CensorImg.Image?.Dispose(); //Clear any previous image ?. means if its null nothing happens
                    CensorImg.Image = null; //set Value to Null to avoid any issues

                    FileNameLabel.Text = "Image Loaded: " + System.IO.Path.GetFileName(SourceFilePath);
                }
            }

        }

        private void CensorButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(SourceFilePath))
            {
                MessageBox.Show("No File Uploaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Bitmap ImageFile = new Bitmap(SourceFilePath);
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
                    SourceFilePath = OFD.FileName; //Save file location of the new image


                    CensorImg.Image?.Dispose(); //Clear any previous image ?. means if its null nothing happens
                    CensorImg.Image = null; //set Value to Null to avoid any issues

                    FileNameLabel.Text = "Video Loaded: " + System.IO.Path.GetFileName(SourceFilePath);
                }
            }
        }

        private void CensorVid_Click(object sender, EventArgs e)
        {
            //start with error handling If no image uploaded, dont even start
            if (String.IsNullOrEmpty(SourceFilePath))
            {
                MessageBox.Show("Error: No Video Uploaded");
                return;
            }

            CensorVid.Enabled = false;
            CensorImg.Image?.Dispose();
            string SelectedPath;
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Optional: Customize the dialog text
                saveFileDialog.Title = "Select the Location where you want to save Video:";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog.Filter = "Video Files (*.mp4)|*.mp4";
                saveFileDialog.FileName = $"{System.IO.Path.GetFileName(SourceFilePath)} CENSORED";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                // Optional: Set a default starting directory
                //folderBrowser.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Show the dialog and check if the user clicked "OK"
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Grab the selected folder path
                    SelectedPath = saveFileDialog.FileName;

                    // Display it in your TextBox
                    //txtSaveLocation.Text = selectedPath;

                    // You can now use 'selectedPath' to save your files
                }
                else
                {
                    MessageBox.Show("Error: No File Location Saved");
                    CensorVid.Enabled = true;
                    return;
                }
            }


            string VideoPath = SelectedPath;
            //String VideoPath = Path.Combine(_ProjectRoot, @"Frameholder\Video.mp4");

            Task.Run(() =>
            {
                try
                {
                    if (_CensorsList.Contains("Non-Nude"))
                    {
                        _SFWcensorService.CensorVideoFast(
                            SourceFilePath, VideoPath, _Option,
                            censorType: _CensorType,
                            progressCallback: AppendProgress);
                    }

                    if (_CensorsList.Contains("Nudes"))
                    {
                        _NSFWcensorService.CensorVideoFast(
                            SourceFilePath, VideoPath, _Option,
                            censorType: _CensorType,
                            progressCallback: AppendProgress);
                    }

                    // Completion message must also be marshaled
                    this.BeginInvoke(() =>
                    {
                        MessageBox.Show("Complete!");
                        CensorVid.Enabled = true;
                    });
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(() =>
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        CensorVid.Enabled = true;
                    });
                }
            });


            //if (_CensorsList.Contains("Non-Nude"))
            //    _SFWcensorService.CensorVideoFast(ImageFilePath, VideoPath, _Option, censorType: _CensorType, progressCallback: AppendProgress);

            //if (_CensorsList.Contains("Nudes"))
            //    _NSFWcensorService.CensorVideoFast(ImageFilePath, VideoPath, _Option, censorType: _CensorType, progressCallback: AppendProgress);

            //MessageBox.Show("Complete!");

            CensorVid.Enabled = true;
        }

        private void AppendProgress(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            if (VideoProgress.InvokeRequired)
            {
                VideoProgress.BeginInvoke(new Action<string>(AppendProgress), line);
                return;
            }

            VideoProgress.Text=line;
            //.SelectionStart = VideoProgress.Text.Length;
            //VideoProgress.ScrollToCaret();
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
            _Option.ChangePixelateFactor(PixelateFactor: PixelationDensity.Value);
            PixelLabel.Text = PixelationDensity.Value.ToString();
        }

        private void BoxColorButton_Click(object sender, EventArgs e)
        {
            CensorBoxColor.AllowFullOpen = true;
            CensorBoxColor.Color = Color.Black;
            if (CensorBoxColor.ShowDialog() == DialogResult.OK)
            {
                _Option.ChangeCensorColor(CensorColor: CensorBoxColor.Color);
            }
        }

        private void GaussianBlurrSlider_Scroll(object sender, EventArgs e)
        {
            _Option.ChangeGaussianBlurFactor(GaussianBlurFactor: GaussianBlurrSlider.Value);
            GaussianBlurLabel.Text = GaussianBlurrSlider.Value.ToString();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(SourceFilePath))
            {
                MessageBox.Show("No File Uploaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
    }
}
