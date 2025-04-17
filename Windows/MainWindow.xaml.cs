using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media;
using Backup.Database;
using System.Windows;
using System.IO;

namespace Backup.Windows
{
    public partial class MainWindow : Window
    {
        private readonly List<FilesDatabase.FileData> selectedFiles = [];
        private readonly FilesDatabase db;

        [DllImport("FileCopy.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CopyFiles(string sourcePaths, string targetPath, string style, bool cleanTargetFolder);

        public MainWindow()
        {
            InitializeComponent();
            db = new FilesDatabase();
            db.Initialize();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {
            MainStackPanel.Children.Clear();
            var files = db.GetAllFileData();
            foreach (var file in files)
            {
                CreateFileItem(file);
            }
        }

        private void CreateFileItem(FilesDatabase.FileData file)
        {
            Grid grid = new Grid
            {
                Height = 24,
                Margin = new Thickness(0, 5, 0, 0)
            };
            MainStackPanel.Children.Add(grid);

            System.Windows.Controls.CheckBox checkbox = new System.Windows.Controls.CheckBox
            {
                Tag = file.FileID,
                Content = file.FileName,
                Margin = new Thickness(10, 0, 158, 0)
            };
            grid.Children.Add(checkbox);

            System.Windows.Controls.Button deleteButton = new System.Windows.Controls.Button
            {
                Width = 64,
                Content = "删除",
                Tag = file.FileID,
                Margin = new Thickness(497, 0, 0, 0),
                Style = (Style)FindResource("WhiteButton"),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left
            };
            deleteButton.Click += DeleteButton_Click;
            grid.Children.Add(deleteButton);

            System.Windows.Controls.Button editButton = new System.Windows.Controls.Button
            {
                Width = 64,
                Content = "编辑",
                Tag = file.FileID,
                Margin = new Thickness(428, 0, 0, 0),
                Style = (Style)FindResource("WhiteButton"),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left
            };
            editButton.Click += EditButton_Click;
            grid.Children.Add(editButton);
        }

        private async void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            var checkboxes = FindVisualChildren<System.Windows.Controls.CheckBox>(scrollViewer);
            selectedFiles.Clear();

            foreach (var checkbox in checkboxes)
            {
                if (checkbox.IsChecked == true)
                {
                    int fileID;
                    if (int.TryParse(checkbox.Tag.ToString(), out fileID))
                    {
                        var fileData = db.GetFileData(fileID);
                        if (fileData != null)
                        {
                            selectedFiles.Add(fileData);
                        }
                    }
                }
            }

            if (selectedFiles.Count == 0)
            {
                System.Windows.MessageBox.Show("没有选中任何文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            BackupButton.IsEnabled = false;

            try
            {
                await BackupFilesAsync();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"备份失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BackupButton.IsEnabled = true;
                if (ExitAfterBackupCheckBox.IsChecked == true)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }

        private async Task BackupFilesAsync()
        {
            foreach (var file in selectedFiles)
            {
                try
                {
                    Directory.CreateDirectory(file.TargetPath);
                    await Task.Run(() => CopyFiles(file.SourcePath, file.TargetPath, file.Style, file.CleanTargetFloder));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        System.Windows.MessageBox.Show($"备份文件 {file.FileName} 失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            }

            Dispatcher.Invoke(() =>
            {
                System.Windows.MessageBox.Show("备份完成！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (System.Windows.Controls.Button)sender;
            int fileID = (int)button.Tag;
            db.DeleteFileData(fileID);
            MainStackPanel.Children.Remove(button.Parent as Grid);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (System.Windows.Controls.Button)sender;
            int fileID = (int)button.Tag;
            AddWindow addwindow = new AddWindow(fileID.ToString());
            addwindow.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            GC.Collect();
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                {
                    yield return t;
                }
                foreach (var descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = true;
        }

        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addwindow = new AddWindow("File");
            addwindow.ShowDialog();
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addwindow = new AddWindow("Folder");
            addwindow.ShowDialog();
        }

        private void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scrollViewer.ScrollToVerticalOffset(VerticalScrollBar.Value);
        }

        private void HorizontalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scrollViewer.ScrollToHorizontalOffset(HorizontalScrollBar.Value);
        }

        private void ListViewScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (scrollViewer != null)
            {
                VerticalScrollBar.Maximum = scrollViewer.ScrollableHeight;
                VerticalScrollBar.ViewportSize = scrollViewer.ViewportHeight;
                VerticalScrollBar.Value = scrollViewer.VerticalOffset;

                HorizontalScrollBar.Maximum = scrollViewer.ScrollableWidth;
                HorizontalScrollBar.ViewportSize = scrollViewer.ViewportWidth;
                HorizontalScrollBar.Value = scrollViewer.HorizontalOffset;
            }
        }
    }
}