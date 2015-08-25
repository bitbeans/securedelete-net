using System.IO;
using NUnit.Framework;
using SecureDelete;

namespace Tests
{
    /// <summary>
    ///     Class to test the SecureDelete.
    /// </summary>
    [TestFixture]
    public class SecureDeleteTests
    {
        [Test]
        public void DeleteDirectory()
        {
            Delete.DeleteDirectory(new DirectoryInfo(@"F:\MyFolderOnHdd"), true);
        }

        [Test]
        public void DeleteDirectoryWithoutDriveDetection()
        {
            Delete.DeleteDirectoryWithoutDriveDetection(new DirectoryInfo(@"F:\MyFolderOnHdd"), true);
        }

        [Test]
        public void DeleteFile()
        {
            Delete.DeleteFile(@"F:\aFile.jpg");
        }

        [Test]
        public void DeleteFileWithoutDriveDetection()
        {
            Delete.DeleteFileWithoutDriveDetection(@"F:\aFile.jpg");
        }
    }
}
