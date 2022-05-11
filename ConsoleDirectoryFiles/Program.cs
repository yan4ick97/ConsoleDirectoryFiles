using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDirectoryFiles
{
    [System.Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class Program : SystemException
    {

        private static DirectoryInfo _rootDirectory;
        private static string[] _specDirectories = new string[] { "Изображения", "Документы", "Прочее" };
        private static int _imageCount = 0, _documentCount = 0, _otherCount = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь к диску:");
            string directoryPath = Console.ReadLine();
            
            var driveInfo = new DriveInfo(directoryPath);
            Console.WriteLine($"Информация о диске:{driveInfo.VolumeLabel}, всего {driveInfo.TotalSize /1024/1024} Мб,"+
                $"свободно {driveInfo.AvailableFreeSpace /1024/1024} Мб.");
            _rootDirectory = driveInfo.RootDirectory;
            SearchDirectories(_rootDirectory);

            foreach (var directory in _rootDirectory.GetDirectories())
            {
                if (!_specDirectories.Contains(directory.Name))
                    directory.Delete(true);
            }

            var resultText = $"Всего обработано {_imageCount+_documentCount+_otherCount} файлов."+
                $"Из них {_imageCount} изображений, {_documentCount} документов, {_otherCount} прочих файлов";
            Console.WriteLine(resultText);
            File.WriteAllText(_rootDirectory + "\\Инфо.txt", resultText);

            Console.ReadLine();
        }
        
        private static void SearchDirectories(DirectoryInfo currentDirectory)
        {
            if (!_specDirectories.Contains(currentDirectory.Name))
            {
                FilterFiles(currentDirectory);
                foreach (var childDirectory in currentDirectory.GetDirectories())
                {
                    SearchDirectories(childDirectory);
                }
            }
        }
        private static void FilterFiles(DirectoryInfo currentDirectory)
        {
            var currentFiles = currentDirectory.GetFiles();

             
            foreach (var fileInfo in currentFiles)
            {
                if (new string[] { ".jpg", ".png", ".jpeg", ".bmp" }
                .Contains(fileInfo.Extension.ToLower()))
                {
                    var photoDirectory = new DirectoryInfo(_rootDirectory + $"{_specDirectories[0]}\\");
                    if (!photoDirectory.Exists)
                        photoDirectory.Create();
                    

                    
                    var yearDirectory = new DirectoryInfo(photoDirectory + $"{fileInfo.LastWriteTime.Date.Year}\\");
                    if (!yearDirectory.Exists)
                        yearDirectory.Create();

                   
                    MoveFile(fileInfo, yearDirectory);
                    _imageCount++;
                }
                 
            }
        }

        private static void MoveFile(FileInfo fileInfo, DirectoryInfo directoryInfo)
        {
            var newFileInfo = new FileInfo(directoryInfo + $"\\{fileInfo.Name}");
            while (!newFileInfo.Exists)
                newFileInfo = new FileInfo(directoryInfo + $"\\{Path.GetFileNameWithoutExtension(fileInfo.FullName)}(1)" +
                    $"{newFileInfo.Extension}");
            fileInfo.MoveTo(newFileInfo.FullName);
        }
        
    }
    
}
