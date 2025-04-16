using System.Windows;

namespace Backup.Windows
{
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();
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
            var folderDialog = new FolderBrowserDialog
            {
                Description = "选择要备份的文件夹",
                ShowNewFolderButton = true
            };

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath; // 获取选择的文件夹路径
                SourceLocationTextBox.Text = selectedPath; // 显示选择的文件夹路径
            }
        }

        private void TargetLocationSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog
            {
                Description = "选择备份目标文件夹",
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