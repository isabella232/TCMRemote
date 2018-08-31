using System.Linq;
using System.Management.Automation;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Decommission the specified Publication Target.
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, "TcmPublicationTarget", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class DecommissionPublicationTargetCommand : TransactionalTcmCmdlet
    {
        /// <summary>
        /// List of TcmUri of the Publication Targets to decommission.
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Sets TcmUri(s) of the one or more Publication Targets to decommission.", ValueFromPipeline = true)]
        public string[] PublicationTargetIds { get; set; }

        /// <summary>
        /// Gets the should process message.
        /// </summary>
        /// <value>The should process message.</value>
        protected override string ShouldProcessMessage
        {
            get
            {
                return string.Join(", ", PublicationTargetIds);
            }
        }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>
        /// Used for proper error handling of core service fault exception.
        /// </remarks>
        protected override void ProcessCoreServiceRecord()
        {
            if (PublicationTargetIds != null && PublicationTargetIds.Any())
            {
                foreach (string publicationTargetId in PublicationTargetIds)
                {
                    CoreServiceClient.DecommissionPublicationTarget(publicationTargetId);    
                }
            }
        }
    }
}
