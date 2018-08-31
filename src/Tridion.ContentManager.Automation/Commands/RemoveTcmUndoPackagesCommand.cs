using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Gets a list of undo packages stored in the application data (to be available across scaled-out TCM instances).
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "TcmUndoPackages", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveTcmUndoPackagesCommand : TcmUndoPackagesCmdlet
    {
        private readonly List<UndoPackageInfo> _packagesToDelete = new List<UndoPackageInfo>();

        /// <summary>
        /// Gets or sets date and keeps undo packages after this date. Older packages will be deleted.
        /// </summary>
        [Parameter(HelpMessage = "Keep undo packages after this date.")]
        public DateTime? KeepAfter { get; set; }

        /// <summary>
        /// Gets or sets ID of the undo package that should be deleted.
        /// </summary>
        [Parameter(HelpMessage = "Undo package ID to be deleted.")]
        public string PackageId { get; set; }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            foreach (var packageInfo in _packagesToDelete)
            {
                CoreServiceClient.DeleteApplicationData(null, packageInfo.PackageId);
                CoreServiceClient.DeleteApplicationData(null, packageInfo.PackageMetadataId);
            }
        }

        protected override string ShouldProcessMessage
        {
            get
            {
                return string.Join(", ", _packagesToDelete.Select(package => package.PackageId));
            }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            var undoPackagesList = GetUndoPackages();

            if (!string.IsNullOrEmpty(PackageId))
            {
                var undoPackageInfo = undoPackagesList.FirstOrDefault(info => info.PackageId == PackageId);
                if (undoPackageInfo != null)
                {
                    _packagesToDelete.Add(undoPackageInfo);
                }
            }
            else if (KeepAfter != null)
            {
                foreach (var undoPackageInfo in undoPackagesList)
                {
                    if (undoPackageInfo.CreationTime < KeepAfter)
                    {
                        _packagesToDelete.Add(undoPackageInfo);
                    }
                }
            }
            else
            {
                WriteWarning("Either -Id or -KeepAfter parameter must be specified for this cmdlet.");
            }
        }
    }
}