// FileCopy.cpp
#include "pch.h"
#define FILECOPY_API __declspec(dllexport)

#include <windows.h>
#include <string>
#include <filesystem>
#include <vector>

// 声明CopyDirectory函数
static void CopyDirectory(const std::string& sourceDir, const std::string& targetDir);

extern "C" FILECOPY_API void CopyFiles(const char* sourcePaths, const char* targetPath, const char* style, bool cleanTargetFolder)
{
    try
    {
        std::string sourcePathsStr = sourcePaths; // 源路径
        std::string targetPathStr = targetPath; // 目标路径
        std::string styleStr = style; // 复制方式

        // 如果需要删除目标文件夹，则先删除
        if (cleanTargetFolder)
        {
            std::filesystem::remove_all(targetPathStr);
            std::filesystem::create_directories(targetPathStr); // 重新创建空文件夹
        }

        if (styleStr == "File")
        {
            std::vector<std::string> files; // 文件名列表
            size_t pos = 0; // 字符串分割位置
            std::string delimiter = "\n"; // 文件名分隔符

            while ((pos = sourcePathsStr.find(delimiter)) != std::string::npos)
            {
                std::string file = sourcePathsStr.substr(0, pos); // 获取文件名
                files.push_back(file); // 加入文件名列表
                sourcePathsStr.erase(0, pos + delimiter.length()); // 去掉已复制的文件名
            }
            files.push_back(sourcePathsStr); // 最后一个文件

            for (const auto& file : files)
            {
                if (!file.empty() && std::filesystem::exists(file))
                {
                    std::string fileName = std::filesystem::path(file).filename().string(); // 获取文件名
                    std::string destFile = targetPathStr + "\\" + fileName; // 计算目标文件路径
                    CopyFileA(file.c_str(), destFile.c_str(), TRUE); // 复制文件
                }
            }
        }
        else if (styleStr == "Folder")
        {
            if (std::filesystem::exists(sourcePathsStr))
            {
                std::string sourceFolder = sourcePathsStr; // 获取源文件夹名
                std::string targetFolder = targetPathStr + "\\" + std::filesystem::path(sourceFolder).filename().string(); // 计算目标文件夹名
                CopyDirectory(sourceFolder, targetFolder); // 复制文件夹
            }
        }
    }
    catch (const std::exception& e)
    {
        MessageBoxA(NULL, e.what(), "错误", MB_ICONERROR); // 弹出错误提示
    }
}

// 定义CopyDirectory函数
static void CopyDirectory(const std::string& sourceDir, const std::string& targetDir)
{
    std::filesystem::create_directories(targetDir); // 先创建目标文件夹
    for (const auto& entry : std::filesystem::recursive_directory_iterator(sourceDir))
    {
        const std::filesystem::path& path = entry.path(); // 获取文件或文件夹路径
        std::string targetPath = targetDir + path.generic_string().substr(sourceDir.length()); // 计算目标路径

        if (std::filesystem::is_directory(path))
        {
            std::filesystem::create_directories(targetPath); // 递归创建子文件夹
        }
        else
        {
            CopyFileA(path.string().c_str(), targetPath.c_str(), TRUE); // 复制文件
        }
    }
}