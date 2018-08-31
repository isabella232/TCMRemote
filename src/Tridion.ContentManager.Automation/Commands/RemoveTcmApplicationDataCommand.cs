using System.Management.Automation;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Deletes all application data from the system for the specified application identifier.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "TcmApplicationData", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveTcmAppDataCommand : TransactionalTcmCmdlet
    {
        /// <summary>
        /// Gets or sets the application identifiers for which must perform purging application data process
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Application identifiers for which purging application data process must be preformed", ValueFromPipeline = true)]
        public string[] TcmApplicationIds { get; set; }

        /// <summary>
        /// Gets the should process message.
        /// </summary>
        /// <value>The should process message.</value>
        protected override string ShouldProcessMessage
        {
            get
            {
                return string.Join(", ", TcmApplicationIds);
            }
        }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            foreach (string id in TcmApplicationIds)
            {
                CoreServiceClient.PurgeApplicationData(id);
            }
        }
    }
}
