namespace SecureDelete.Models
{
    /// <summary>
    ///     Supported file system types.
    /// </summary>
    public enum FileSystemType
    {
        /// <summary>
        ///     New Technology File System.
        /// </summary>
        NTFS,

        /// <summary>
        ///     File Allocation Table.
        /// </summary>
        FAT32,
        /// <summary>
        ///     Used for unknown.
        /// </summary>
        Default,
        /// <summary>
        ///     Used with ExpanDrive.
        /// </summary>
        EXFS
    }
}