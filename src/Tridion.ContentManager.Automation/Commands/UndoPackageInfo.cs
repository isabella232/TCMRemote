using System;
using System.Globalization;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Undo package metadata stored in the application data.
    /// </summary>
    public class UndoPackageInfo
    {
        public string PackageId { get; }

        public DateTime CreationTime { get; }

        public string ImportUserId { get; }

        public int Actions { get; }

        public string ImportDescription { get; }

        internal string PackageMetadataId { get; }

        public UndoPackageInfo(string packageMetadataId, string packageMetadata)
        {
            string[] metadata = packageMetadata.Split('|');

            CreationTime = DateTime.ParseExact(metadata[0], "dd-MM-yyyy-HH-mm-ss", CultureInfo.InvariantCulture);
            ImportUserId = new TcmUri(metadata[1]);
            PackageId = metadata[2];
            Actions = int.Parse(metadata[3]);
            ImportDescription = metadata[4];
            PackageMetadataId = packageMetadataId;
        }
    }
}