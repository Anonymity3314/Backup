using System.Windows;

namespace Backup.Windows
{
    public partial class AddWindow : Window
    {
        new string Style { get; set; }
        public AddWindow(string style)
        {
            InitializeComponent();
            Style = style; // 设置样式
        }

        // 选择样式
        private void AddWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(Style == "File")
            {
                this.Title = "添加要备份的文件"; // 设置标题
                DescriptionLabel.Content = "源文件地址"; // 设置描述
            }
            else if(Style == "Folder")
            {
                this.Title = "添加要备份的文件夹"; // 设置标题
                DescriptionLabel.Content = "源文件夹地址"; // 设置描述
            }
        }

        // 关闭添加窗口
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // 关闭添加窗口
        }

        // 保存添加信息
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SourceLocationButton_Click(object sender, RoutedEventArgs e)
        {
            if (Style == "File")
            {
                var fileDialog = new OpenFileDialog
                {
                    Multiselect = true
                };

                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string[] selectedPaths = fileDialog.FileNames; // 获取选择的文件路径
                    foreach (string path in selectedPaths)
                    {
                        SourceLocationTextBox.Text += path + "\n"; // 显示选择的文件路径
                    }
                }
            }
            else if (Style == "Folder")
            {
                var folderDialog = new FolderBrowserDialog
                {
                    ShowNewFolderButton = true
                };

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath; // 获取选择的文件夹路径
                    SourceLocationTextBox.Text = selectedPath; // 显示选择的文件夹路径
                }
            }
        }

        private void TargetLocationSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath; // 获取选择的文件夹路径
                TargetLocationTextBox.Text = selectedPath; // 显示选择的文件夹路径
            }
        }
    }
}