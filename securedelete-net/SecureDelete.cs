using System;
using System.IO;
using System.Security;
using DiskDetector;
using DiskDetector.Exceptions;
using DiskDetector.Models;
using Helper;
using SecureDelete.Models;

namespace SecureDelete
{
    /// <summary>
    ///     Simple class for secure file deletion on HDD`s.
    /// </summary>
    public static class Delete
    {
        /// <summary>
        ///     The maximal buffer size for the file overwrite.
        /// </summary>
        private const int MaxBufferSize = 67108864; // 64MB

        /// <summary>
        ///     Obfuscation rounds.
        /// </summary>
        private const int ObfuscationRounds = 8;

        /// <summary>
        ///     Try to delete a file in a secure way.
        /// </summary>
        /// <param name="directoryInfo">The directory to delete.</param>
        /// <param name="recursive">Delete the directory recursive.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DetectionFailedException"></exception>
        public static void DeleteDirectory(DirectoryInfo directoryInfo, bool recursive)
        {
            if (!directoryInfo.Exists)
            {
                return;
            }

            if ((directoryInfo.Attributes & FileAttributes.ReparsePoint) == 0)
            {
                if (!recursive && directoryInfo.GetFileSystemInfos().Length != 0)
                {
                    throw new InvalidOperationException("The directory is not empty.");
                }
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    DeleteDirectory(directory, true);
                }
                foreach (var file in directoryInfo.GetFiles())
                {
                    DeleteFile(file);
                }
            }
            ObfuscateDirectory(directoryInfo);
            directoryInfo.Delete();
        }

        /// <summary>
        ///     Try to delete a directory in a secure way, but without drive detection.
        /// </summary>
        /// <param name="directoryInfo">The directory to delete.</param>
        /// <param name="recursive">Delete the directory recursive.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void DeleteDirectoryWithoutDriveDetection(DirectoryInfo directoryInfo, bool recursive)
        {
            if (!directoryInfo.Exists)
            {
                return;
            }

            if ((directoryInfo.Attributes & FileAttributes.ReparsePoint) == 0)
            {
                if (!recursive && directoryInfo.GetFileSystemInfos().Length != 0)
                {
                    throw new InvalidOperationException("The directory is not empty.");
                }
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    DeleteDirectoryWithoutDriveDetection(directory, true);
                }
                foreach (var file in directoryInfo.GetFiles())
                {
                    DeleteFileWithoutDriveDetection(file);
                }
            }
            ObfuscateDirectory(directoryInfo);
            directoryInfo.Delete();
        }

        /// <summary>
        ///     Try to delete a file in a secure way.
        /// </summary>
        /// <param name="directory">The directory to delete.</param>
        /// <param name="recursive">Delete the directory recursive.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DetectionFailedException"></exception>
        public static void DeleteDirectory(string directory, bool recursive)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException("directory", "directory can not be empty");
            }
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException("directory could not be found.");
            }
            DeleteDirectory(new DirectoryInfo(directory), recursive);
        }

        /// <summary>
        ///     Try to delete a directory in a secure way, but without drive detection.
        /// </summary>
        /// <param name="directory">The directory to delete.</param>
        /// <param name="recursive">Delete the directory recursive.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void DeleteDirectoryWithoutDriveDetection(string directory, bool recursive)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException("directory", "directory can not be empty");
            }
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException("directory could not be found.");
            }
            DeleteDirectoryWithoutDriveDetection(new DirectoryInfo(directory), recursive);
        }

        /// <summary>
        ///     Try to delete a file in a secure way, but without drive detection.
        /// </summary>
        /// <remarks>This method will always overwrite and obfuscate the file, even on SSD.</remarks>
        /// <param name="fileInfo">The file to delete.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public static void DeleteFileWithoutDriveDetection(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return;
            }
            // check if the file is read only protected
            if (fileInfo.IsReadOnly)
            {
                fileInfo.IsReadOnly = false;
            }
            OverwriteFileWithRandomData(fileInfo);
            ObfuscateFile(fileInfo, FileSystemType.Default);
            fileInfo.Delete();
        }

        /// <summary>
        ///     Try to delete a file in a secure way, but without drive detection.
        /// </summary>
        /// <remarks>This method will always overwrite and obfuscate the file, even on SSD.</remarks>
        /// <param name="file">The file to delete.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public static void DeleteFileWithoutDriveDetection(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException("file", "file can not be empty");
            }
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("file", "file could not be found.");
            }
            DeleteFileWithoutDriveDetection(new FileInfo(file));
        }

        /// <summary>
        ///     Try to delete a file in a secure way.
        /// </summary>
        /// <param name="fileInfo">The file to delete.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="DetectionFailedException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public static void DeleteFile(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return;
            }

            var parent = Directory.GetParent(fileInfo.FullName);
            if (parent != null)
            {
                // try to get the drive letter
                var driveLetter = parent.Root.Name.Substring(0, 1);
                // recieve some informations about the drive
                var driveInfo = Detector.DetectDrive(driveLetter);
                if (driveInfo != null)
                {
                    // check if the file is read only protected
                    if (fileInfo.IsReadOnly)
                    {
                        fileInfo.IsReadOnly = false;
                    }
                    // check if the file is a hard link or a symbolic link.
                    //if ((fileInfo.Attributes & FileAttributes.ReparsePoint) == 0) { }
                    FileSystemType fileSystemType;
                    try
                    {
                        fileSystemType = ParseEnum<FileSystemType>(driveInfo.DriveFormat);
                    }
                    catch (Exception)
                    {
                        fileSystemType = FileSystemType.Default;
                    }
                    switch (driveInfo.HardwareType)
                    {
                        case HardwareType.Hdd:
                            OverwriteFileWithRandomData(fileInfo);
                            ObfuscateFile(fileInfo, fileSystemType);
                            fileInfo.Delete();
                            break;
                        case HardwareType.Ssd:
                            // on SSD drives we only perform a simple delete
                            fileInfo.Delete();
                            break;
                        default:
                            if (driveInfo.DriveType == DriveType.Network)
                            {
                                fileInfo.Delete();
                            }
                            else
                            {
                                OverwriteFileWithRandomData(fileInfo);
                                ObfuscateFile(fileInfo, fileSystemType);
                                fileInfo.Delete();
                            }
                            break;
                    }
                }
                else
                {
                    throw new DetectionFailedException(
                        "Could not detect drive informations. Maybe it`s not a fixed, removable or network drive?");
                }
            }
        }

        /// <summary>
        ///     Try to delete a file in a secure way.
        /// </summary>
        /// <param name="file">The file to delete.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="DetectionFailedException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public static void DeleteFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException("file", "file could not be empty");
            }
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("file", "file could not be found.");
            }
            DeleteFile(new FileInfo(file));
        }

        /// <summary>
        ///     Obfuscates the file times of a file.
        /// </summary>
        /// <param name="fileSystemInfo">The FileSystemInfo of the file.</param>
        /// <param name="fileSystemType">A supported FileSystemType.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        internal static void ObfuscateFileTimes(FileSystemInfo fileSystemInfo, FileSystemType fileSystemType)
        {
            DateTime newFileTime;
            switch (fileSystemType)
            {
                case FileSystemType.NTFS:
                    newFileTime = new DateTime(1601, 1, 1, 0, 0, 0, 1, DateTimeKind.Utc);
                    break;
                case FileSystemType.FAT32:
                    newFileTime = new DateTime(1980, 1, 1, 0, 0, 0);
                    break;
                default:
                    newFileTime = new DateTime(1980, 1, 1, 0, 0, 0);
                    break;
            }

            fileSystemInfo.LastAccessTime = newFileTime;
            fileSystemInfo.LastWriteTime = newFileTime;
            fileSystemInfo.CreationTime = newFileTime;
        }

        /// <summary>
        ///     Move a file to a new location.
        /// </summary>
        /// <param name="fileSystemInfo">the file system info of the file.</param>
        /// <param name="path">The new location to move the file.</param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        internal static void MoveTo(this FileSystemInfo fileSystemInfo, string path)
        {
            var fileInfo = fileSystemInfo as FileInfo;
            var directoryInfo = fileSystemInfo as DirectoryInfo;

            if (fileInfo != null)
            {
                fileInfo.MoveTo(path);
            }
            else if (directoryInfo != null)
            {
                directoryInfo.MoveTo(path);
            }
            else
            {
                throw new ArgumentException("Unknown FileSystemInfo type.");
            }
        }

        /// <summary>
        ///     Obfuscates a directory.
        /// </summary>
        /// <param name="fileSystemInfo">The directory to obfuscate.</param>
        /// <param name="fileSystemType">The file system type.</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        internal static void ObfuscateDirectory(FileSystemInfo fileSystemInfo,
            FileSystemType fileSystemType = FileSystemType.Default)
        {
            if (!fileSystemInfo.Exists)
            {
                return;
            }

            try
            {
                // prevent a file lock by indexing service
                fileSystemInfo.Attributes = FileAttributes.NotContentIndexed;
            }
            catch (ArgumentException e)
            {
                throw new UnauthorizedAccessException(e.Message, e);
            }
            // generate a new random directory name and set a new path
            var newPath = Path.Combine(Path.GetDirectoryName(fileSystemInfo.FullName), Path.GetRandomFileName());
            // reset the file times
            ObfuscateFileTimes(fileSystemInfo, fileSystemType);
            //rename the directory once to erase the entry from the file system table.
            fileSystemInfo.MoveTo(newPath);
        }

        /// <summary>
        ///     Obfuscates a file.
        /// </summary>
        /// <param name="fileSystemInfo">The file to obfuscate.</param>
        /// <param name="fileSystemType">The file system type.</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        internal static void ObfuscateFile(FileSystemInfo fileSystemInfo,
            FileSystemType fileSystemType = FileSystemType.Default)
        {
            if (!fileSystemInfo.Exists)
            {
                return;
            }

            try
            {
                // prevent a file lock by indexing service
                fileSystemInfo.Attributes = FileAttributes.NotContentIndexed;
            }
            catch (ArgumentException e)
            {
                throw new UnauthorizedAccessException(e.Message, e);
            }

            //rename the file a few times to erase the entry from the file system table.
            for (var obfuscateRound = 0; obfuscateRound < ObfuscationRounds; obfuscateRound++)
            {
                // generate a new random file name and set a new path
                var newPath = Path.Combine(Path.GetDirectoryName(fileSystemInfo.FullName), Path.GetRandomFileName());
                // reset the file times
                ObfuscateFileTimes(fileSystemInfo, fileSystemType);
                fileSystemInfo.MoveTo(newPath);
            }
        }

        /// <summary>
        ///     Overwrite a file with random data and set a new file length.
        /// </summary>
        /// <param name="fileInfo">The file info of the file, to overwrite.</param>
        /// <param name="setRandomFileSize">Use a random value for the new file size, otherwise set it to 0.</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        internal static void OverwriteFileWithRandomData(FileInfo fileInfo, bool setRandomFileSize = false)
        {
            if (!fileInfo.Exists)
            {
                return;
            }
            const int minRandomSize = 0;
            const int maxRandomSize = 1024;
            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                for (var size = fileStream.Length; size > 0; size -= MaxBufferSize)
                {
                    var bufferSize = (size < MaxBufferSize) ? size : MaxBufferSize;
                    var buffer = new byte[bufferSize];
                    for (var bufferIndex = 0; bufferIndex < bufferSize; ++bufferIndex)
                    {
                        // overwrite every byte with a random byte.
                        // Note: there is no need for a secure random provider
                        buffer[bufferIndex] = (byte) (RandomProvider.Next()%256);
                    }
                    fileStream.Write(buffer, 0, buffer.Length);
                    fileStream.Flush(true);
                }
                fileStream.SetLength(setRandomFileSize ? RandomProvider.Next(minRandomSize, maxRandomSize) : 0);
            }
        }

        /// <summary>
        ///     Parse a string to an enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        internal static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof (T), value, true);
        }
    }
}