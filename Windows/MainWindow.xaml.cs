using System.Windows.Controls;
using System.Windows.Media;
using Backup.Database;
using System.Windows;

namespace Backup.Windows
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 递归查找指定类型的子元素
        /// </summary>
        /// <typeparam name="T">要查找的元素类型</typeparam>
        /// <param name="parent">父元素</param>
        /// <returns>找到的子元素集合</returns>
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break; // 终止条件，如果 parent 为 null 则返回空集合
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                {
                    yield return t; // 找到元素，返回
                }

                // 递归查找子元素的子元素
                foreach (var descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant; // 找到子元素，返回
                }
            }
        }
        private readonly FilesDatabase db; // 数据库实例

        public MainWindow()
        {
            InitializeComponent();

            db = new FilesDatabase(); // 实例化数据库
            db.Initialize(); // 初始化数据库
        }

        // 加载要备份的文件项目
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        // 滚动条滚动时同步更新ScrollViewer的滚动条
        private void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scrollViewer.ScrollToVerticalOffset(VerticalScrollBar.Value); // 设置当前值
        }
        private void HorizontalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scrollViewer.ScrollToHorizontalOffset(HorizontalScrollBar.Value); // 设置当前值
        }

        // ScrollViewer滚动时同步更新滚动条
        private void ListViewScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (scrollViewer != null)
            {
                // 更新纵向滚动条
                VerticalScrollBar.Maximum = scrollViewer.ScrollableHeight; // 设置最大值
                VerticalScrollBar.ViewportSize = scrollViewer.ViewportHeight; // 设置视口大小
                VerticalScrollBar.Value = scrollViewer.VerticalOffset; // 设置当前值

                // 更新横向滚动条
                HorizontalScrollBar.Maximum = scrollViewer.ScrollableWidth; // 设置最大值
                HorizontalScrollBar.ViewportSize = scrollViewer.ViewportWidth; // 设置视口大小
                HorizontalScrollBar.Value = scrollViewer.HorizontalOffset; // 设置当前值
            }
        }

        // 点击选择要备份的类型
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = true; // 打开弹出框
        }

        // 备份文件
        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addwindow = new("File");
            addwindow.ShowDialog();
        }

        // 备份文件夹
        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addwindow = new("Folder");
            addwindow.ShowDialog();
        }

        // 点击备份按钮备份文件
        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            var checkboxes = FindVisualChildren<System.Windows.Controls.CheckBox>(scrollViewer); // 获取所有复选框
            foreach (var checkbox in checkboxes)
            {
                if (checkbox.IsChecked == true)
                {

                }
            }
        }

        // 关闭窗口清理资源
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e); // 先调用基类方法
            GC.Collect(); // 强制回收内存
        }
    }
}