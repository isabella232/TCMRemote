using System.Linq;
using System.Management.Automation;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Synchronizes (part of) the Tridion Content Manager full-text index.
    /// </summary>
    [Cmdlet(VerbsData.Sync, "TcmSearchIndex", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class SyncTcmSearchIndexCommand : TransactionalTcmCmdlet
    {
        /// <summary>
        /// Gets or sets the repository identifiers for which full-text index synchronization must be performed.
        /// </summary>
        /// <remarks>
        /// If <c>null</c> or empty list is specified, then all repositories will be re-indexed.
        /// </remarks>
        [Parameter(Position = 0, HelpMessage = "Repository identifiers for which full-text index synchronization must be performed. If no identifier is specified then full syncronization will be performed.", ValueFromPipeline = true)]
        public string[] TcmRepositoryIds { get; set; }

        /// <summary>
        /// Gets the should process message.
        /// </summary>
        /// <value>The should process message.</value>
        protected override string ShouldProcessMessage
        {
            get
            {
                return TcmRepositoryIds == null || !TcmRepositoryIds.Any() ? "All" : string.Join(", ", TcmRepositoryIds);
            }
        }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            if (TcmRepositoryIds != null && TcmRepositoryIds.Any())
            {
                foreach (string id in TcmRepositoryIds)
                {
                    CoreServiceClient.ReIndex(id);
                }
            }
            else
            {
                CoreServiceClient.ReIndex(null);
            }
        }
    }
}
