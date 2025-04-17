using Backup.Database;
using System.Windows;

namespace Backup.Windows
{
    public partial class AddWindow : Window
    {
        private readonly FilesDatabase db; // 数据库
        new string Style { get; set; }

        public AddWindow(string style)
        {
            InitializeComponent();
            db = new FilesDatabase(); // 创建数据库
            db.Initialize(); // 初始化数据库
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

        // 如果内容为空，不启用保存按钮
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SourceLocationTextBox.Text) &&
                !string.IsNullOrEmpty(TargetLocationTextBox.Text) &&
                !string.IsNullOrEmpty(TitleTextBox.Text)) // 检查输入是否为空
            {
                SaveButton.IsEnabled = true; // 启用保存按钮
            }
            else
            {
                SaveButton.IsEnabled = false; // 禁用保存按钮
            }
        }

        // 保存添加信息
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            db.AddFileData(SourceLocationTextBox.Text, TargetLocationTextBox.Text, TitleTextBox.Text); // 添加文件数据
            this.Close(); // 关闭添加窗口
        }

        // 选择源文件或文件夹
        private void SourceLocationButton_Click(object sender, RoutedEventArgs e)
        {
            if (Style == "File") // 选择文件
            {
                var fileDialog = new OpenFileDialog
                {
                    Multiselect = true // 允许多选
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
            else if (Style == "Folder") // 选择文件夹
            {
                var folderDialog = new FolderBrowserDialog
                {
                    ShowNewFolderButton = true // 显示新建文件夹按钮
                };

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath; // 获取选择的文件夹路径
                    SourceLocationTextBox.Text = selectedPath; // 显示选择的文件夹路径
                }
            }
        }

        // 选择目标文件夹
        private void TargetLocationSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true // 显示新建文件夹按钮
            };

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath; // 获取选择的文件夹路径
                TargetLocationTextBox.Text = selectedPath; // 显示选择的文件夹路径
            }
        }
    }
}