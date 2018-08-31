using System.Management.Automation;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Gets a list of QueueData objects.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "TcmQueueInfo")]
    public class GetTcmQueueInfoCommand : TcmCmdlet
    {
        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            WriteObject(CoreServiceClient.GetListQueues(), true);
        }
    }
}
