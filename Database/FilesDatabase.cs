using System.Data.SQLite;
using System.IO;

namespace Backup.Database
{
    class FilesDatabase
    {
        private readonly string dbPath = "Data Source=Backup.db;";

        // 初始化数据库
        public void Initialize()
        {
            if (File.Exists("Backup.db")) return; // 如果数据库已存在，则不再初始化
            SQLiteConnection.CreateFile("Backup.db"); // 创建数据库文件

            using var connection = new SQLiteConnection(dbPath); // 创建数据库连接
            connection.Open(); // 打开数据库连接

            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS [FilesData]
            (
                FileID INTEGER PRIMARY KEY AUTOINCREMENT,
                FileName TEXT,
                SourcePath TEXT,
                TargetPath TEXT,
                Style TEXT,
                CleanTargetFloder BOOLEAN
            );";
            using var command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
        }

        // 添加文件数据
        public void AddFileData(string sourcePath, string targetPath, string fileName, string style, bool cleanTargetFloder)
        {
            using var connection = new SQLiteConnection(dbPath);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                string insertQuery = @"
                INSERT INTO [FilesData] (SourcePath, TargetPath, FileName, Style, CleanTargetFloder)
                VALUES (@SourcePath, @TargetPath, @FileName, @Style, @CleanTargetFloder);";
                using var command = new SQLiteCommand(insertQuery, connection);
                command.Transaction = transaction;
                command.Parameters.AddWithValue("@SourcePath", sourcePath);
                command.Parameters.AddWithValue("@TargetPath", targetPath);
                command.Parameters.AddWithValue("@FileName", fileName);
                command.Parameters.AddWithValue("@Style", style);
                command.Parameters.AddWithValue("@CleanTargetFloder", cleanTargetFloder);
                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        // 获取所有文件数据
        public List<FileData> GetAllFileData()
        {
            using var connection = new SQLiteConnection(dbPath);
            connection.Open();

            string selectQuery = @"
            SELECT * FROM [FilesData]";
            using var command = new SQLiteCommand(selectQuery, connection); // 创建命令对象
            using var reader = command.ExecuteReader(); // 执行查询并返回结果集

            var fileDataList = new List<FileData>(); // 定义文件数据列表
            while (reader.Read())
            {
                var fileData = new FileData
                {
                    FileID = reader.GetInt32(0),
                    FileName = reader.GetString(1),
                    SourcePath = reader.GetString(2),
                    TargetPath = reader.GetString(3),
                    Style = reader.GetString(4),
                    CleanTargetFloder = reader.GetBoolean(5)
                };
                fileDataList.Add(fileData); // 添加文件数据
            }

            return fileDataList; // 返回所有文件数据
        }

        // 通过ID获取备份信息
        public FileData GetFileData(int fileID)
        {
            using var connection = new SQLiteConnection(dbPath);
            connection.Open();

            string selectQuery = @"
            SELECT * FROM [FilesData]
            WHERE FileID = @FileID";
            using var command = new SQLiteCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@FileID", fileID);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var fileData = new FileData
                {
                    FileID = reader.GetInt32(0),
                    FileName = reader.GetString(1),
                    SourcePath = reader.GetString(2),
                    TargetPath = reader.GetString(3),
                    Style = reader.GetString(4),
                    CleanTargetFloder = reader.GetBoolean(5)
                };
                return fileData;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更新文件数据
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="fileName"></param>
        /// <param name="style"></param>
        public void UpdateFileData(int fileID, string sourcePath, string targetPath, string fileName, string style, bool cleanTargetFloder)
        {
            using var connection = new SQLiteConnection(dbPath);
            connection.Open();

            string updateQuery = @"
            UPDATE [FilesData]
            SET SourcePath = @SourcePath, TargetPath = @TargetPath, FileName = @FileName, Style = @Style, cleanTargetFloder = @CleanTargetFloder
            WHERE FileID = @FileID";
            using var command = new SQLiteCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@FileID", fileID);
            command.Parameters.AddWithValue("@SourcePath", sourcePath);
            command.Parameters.AddWithValue("@TargetPath", targetPath);
            command.Parameters.AddWithValue("@FileName", fileName);
            command.Parameters.AddWithValue("@Style", style);
            command.Parameters.AddWithValue("@CleanTargetFloder", cleanTargetFloder);
            command.ExecuteNonQuery();
        }

        // 删除文件数据
        public void DeleteFileData(int fileID)
        {
            using var connection = new SQLiteConnection(dbPath);
            connection.Open();

            string deleteQuery = @"
            DELETE FROM [FilesData]
            WHERE FileID = @FileID";
            using var command = new SQLiteCommand(deleteQuery, connection);
            command.Parameters.AddWithValue("@FileID", fileID);
            command.ExecuteNonQuery();
        }

        public class FileData
        {
            public int FileID { get; set; } // 主键
            public required string FileName { get; set; } // 文件名
            public required string SourcePath { get; set; } // 源路径
            public required string TargetPath { get; set; } // 目标路径
            public required string Style { get; set; } // 备份方式
            public bool CleanTargetFloder { get; set; } // 是否清空目标文件夹
        }
    }
}