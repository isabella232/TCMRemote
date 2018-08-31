using System;
using System.Linq;
using System.Management.Automation;
using Tridion.ContentManager.CoreService.Client;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Purges old versions of TCM items
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "TcmItemsOldVersions", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveTcmItemsOldVersionsCommand : TransactionalTcmCmdlet
    {
        #region Parameters

        /// <summary>
        /// Gets or sets the identifiers (TCM URI or WebDAV URL) of the TCM publications and organizational items in which we must perform purging process
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "List of publication or/and organizational item identifiers (TCM URI or WebDAV URL) in which a purging process will be performed.", ValueFromPipeline = true)]
        public string[] ContainerItemIds
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of versions which we must keep.
        /// </summary>
        [Parameter(HelpMessage = "The versions count to keep.")]
        public uint VersionsToKeep
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets date and keep versions which are going after this date.
        /// </summary>
        [Parameter(HelpMessage = "Keep versions which are going after this date.")]
        public DateTime? KeepAfter { get; set; }

        /// <summary>
        ///  Gets or sets days within last modification to keep versions.
        /// </summary>
        [Parameter(HelpMessage = "Versions created during the specified number of days from now (backward) are kept.")]
        public uint KeepWithinDaysBefore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets mode of search in organizational items. If this switch is specified, the search will occur in nested organizational items.
        /// </summary>
        [Parameter(HelpMessage = "If is specified, the search will occur in nested organizational items, otherwise not.")]
        public SwitchParameter Recursive { get; set; }

        /// <summary>
        ///  Gets or sets the max number of items which we can select for purging during one iteration.
        /// </summary>
        [Parameter(HelpMessage = "The max number of items which we can select for purging during one iteration.")]
        public uint MaxResolvedItemsCount { get; set; }

        #endregion

        /// <summary>
        /// Gets the should process message.
        /// </summary>
        /// <value>The should process message.</value>
        protected override string ShouldProcessMessage
        {
            get
            {
                return string.Join(", ", ContainerItemIds);
            }
        }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            var listLink = ContainerItemIds.Select(
                 item =>
                 {
                     string uri = item.Trim();
                     var x = new LinkToIdentifiableObjectData();

                     if (uri.ToLowerInvariant().StartsWith("tcm:"))
                     {
                         x.IdRef = uri;
                     }
                     else
                     {
                         x.WebDavUrl = uri;
                     }

                     return x;
                 }).ToArray();

            PurgeOldVersionsInstructionData instruction =
                new PurgeOldVersionsInstructionData
                {
                    Recursive = Recursive.ToBool(),
                    KeepVersionsModifiedAfter = KeepAfter,
                    KeepVersionsWithinDaysBeforeLastCheckIn =
                        KeepWithinDaysBefore,
                    VersionsToKeep = VersionsToKeep,
                    MaxResolvedVersionedItemsCount =
                        MaxResolvedItemsCount,
                    Containers = listLink
                };

            int result = CoreServiceClient.PurgeOldVersions(instruction);
            WriteObject(result);
        }
    }
}
