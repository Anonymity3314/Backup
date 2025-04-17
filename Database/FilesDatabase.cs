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
            if (File.Exists("Backup.db")) return;
            SQLiteConnection.CreateFile("Backup.db");

            using var connection = new SQLiteConnection(dbPath);
            connection.Open();

            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS [FilesData]
            (
                FileID INTEGER PRIMARY KEY AUTOINCREMENT,
                FileName TEXT,
                SourcePath TEXT,
                TargetPath TEXT
            );";
            using var command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
        }

        // 添加文件数据
        public void AddFileData(string sourcePath, string targetPath, string fileName)
        {
            using var connection = new SQLiteConnection(dbPath);
            connection.Open();

            string insertQuery = @"
            INSERT INTO [FilesData] (SourcePath, TargetPath, FileName)
            VALUES (@SourcePath, @TargetPath, @FileName);";
            using var command = new SQLiteCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@SourcePath", sourcePath);
            command.Parameters.AddWithValue("@TargetPath", targetPath);
            command.Parameters.AddWithValue("@FileName", fileName);
            command.ExecuteNonQuery();
        }

        // 获取文件数据
        public FileData GetFileData(string fileID)
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
                return new FileData
                {
                    FileID = reader.GetInt32(reader.GetOrdinal("FileID")),
                    FileName = reader["FileName"].ToString(),
                    SourcePath = reader["SourcePath"].ToString(),
                    TargetPath = reader["TargetPath"].ToString()
                };
            }
            return null;
        }

        // 更新文件数据
        public void UpdateFileData(string fileID, string sourcePath, string targetPath, string fileName)
        {
            using var connection = new SQLiteConnection(dbPath);
            connection.Open();

            string updateQuery = @"
            UPDATE [FilesData]
            SET SourcePath = @SourcePath, TargetPath = @TargetPath, FileName = @FileName
            WHERE FileID = @FileID";
            using var command = new SQLiteCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@FileID", fileID);
            command.Parameters.AddWithValue("@SourcePath", sourcePath);
            command.Parameters.AddWithValue("@TargetPath", targetPath);
            command.Parameters.AddWithValue("@FileName", fileName);
            command.ExecuteNonQuery();
        }

        // 删除文件数据
        public void DeleteFileData(string fileID)
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
            public int FileID { get; set; }
            public string FileName { get; set; }
            public string SourcePath { get; set; }
            public string TargetPath { get; set; }
        }
    }
}