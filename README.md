#securedelete-net [![NuGet Version](https://img.shields.io/nuget/v/securedelete-net.svg?style=flat-square)](https://www.nuget.org/packages/securedelete-net/) [![License](http://img.shields.io/badge/license-MIT-green.svg?style=flat-square)](https://github.com/bitbeans/securedelete-net/blob/master/LICENSE.md)


Class to securely delete files on a HDD (only windows based systems).

**Current Features:**

- Ability to detect the underlying hardware (to prevent overwrites on SSD's)
- Overwrite files with simple random data
- Resets the file times
- Obfuscate file and directory names

## Status

Pull requests and/or optimization proposals are always welcome!

## Further reading 

[Data Evaporation from a SSD](https://samsclass.info/121/proj/ssd-evaporation.htm)

[DEF CON 21 - Sam Bowne - Data Evaporation from SSDs](https://www.youtube.com/watch?v=zG0orMGf_Go)

## Installation

There is a [NuGet package](https://www.nuget.org/packages/securedelete-net/) available.

## Example

```csharp

using SecureDelete;


Delete.DeleteDirectory(new DirectoryInfo(@"F:\MyFolderOnHdd"), true);

Delete.DeleteDirectoryWithoutDriveDetection(new DirectoryInfo(@"F:\MyFolderOnHdd"), true);

Delete.DeleteFile(@"F:\aFile.jpg");

Delete.DeleteFileWithoutDriveDetection(@"F:\aFile.jpg");


```

## License
[MIT](https://en.wikipedia.org/wiki/MIT_License)