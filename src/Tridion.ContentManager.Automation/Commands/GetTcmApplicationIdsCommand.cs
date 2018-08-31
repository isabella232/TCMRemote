using System.Management.Automation;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Gets a list of all TCM application IDs for all application data stored in the system.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "TcmApplicationIds")]
    public class GetTcmApplicationIdsCommand : TcmCmdlet
    {
        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            WriteObject(CoreServiceClient.GetApplicationIds(), true);
        }
    }
}
