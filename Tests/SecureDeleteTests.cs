using System.IO;
using SecureDelete;
using Xunit;

namespace Tests
{
    /// <summary>
    ///     Class to test the SecureDelete.
    /// </summary>
    public class SecureDeleteTests
    {
		[Fact]
		public void DeleteDirectory()
        {
            Delete.DeleteDirectory(new DirectoryInfo(@"F:\MyFolderOnHdd"), true);
        }

		[Fact]
		public void DeleteDirectoryWithoutDriveDetection()
        {
            Delete.DeleteDirectoryWithoutDriveDetection(new DirectoryInfo(@"F:\MyFolderOnHdd"), true);
        }

		[Fact]
		public void DeleteFile()
        {
            Delete.DeleteFile(@"F:\aFile.jpg");
        }

		[Fact]
		public void DeleteFileWithoutDriveDetection()
        {
            Delete.DeleteFileWithoutDriveDetection(@"F:\aFile.jpg");
        }

		[Fact]
		public void DeleteFileOnNetworkDrive()
        {
            //Test note: mapped with ExpanDrive and hubiC (EXFS filesystem)
            Delete.DeleteFile(@"Z:\image-201504171044060011.jpg");
        }
    }
}
