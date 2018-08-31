using System;
using System.Management.Automation;
using Tridion.ContentManager.CoreService.Client;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Delete TCM ProcessHistory items for current user.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "TcmProcessHistories", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveTcmProcessHistoriesCommand : TransactionalTcmCmdlet
    {
        /// <summary>
        /// Gets or sets date before which must perform deleting TCM ProcessHistory items, if not specified - purge process will perform for all ProcessHistory items.
        /// </summary>
        [Parameter(HelpMessage = "Delete process items finished before this date, if not specified - purge process will perform for all ProcessHistories items.")]
        public DateTime? Before { get; set; }

        [Parameter(HelpMessage = "Delete process items from this publication. If not specified - purge process will perform for all ProcessHistories items.")]
        public string PublicationId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the should process message.
        /// </summary>
        /// <value>The should process message.</value>
        protected override string ShouldProcessMessage
        {
            get
            {
                string result = "TCM process histories";
                bool filtered = false;
                if (Before.HasValue)
                {
                    result += " before " + Before.Value;
                    filtered = true;
                }
                if (!string.IsNullOrEmpty(PublicationId))
                {
                    result += " for Publication " + PublicationId;
                    filtered = true;
                }

                return filtered ? result : "All TCM process histories";
            }
        }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            PurgeWorkflowHistoryInstructionData instruction = new PurgeWorkflowHistoryInstructionData();
            if (!string.IsNullOrEmpty(PublicationId))
            {
                if (PublicationId == TcmUri.UriNull)
                {
                    throw new ArgumentException("Operation is not supported on a new item or on a null URI. Parameter name 'PublicationId'.");
                }
                instruction.Publication = new LinkToPublicationData
                {
                    IdRef = CoreServiceClient.GetTcmUri(PublicationId, null, null)
                };
            }
            if (Before.HasValue) 
            {
                instruction.DeleteHistoryBefore = Before;
            }
            CoreServiceClient.PurgeWorkflowHistory(instruction);
            WriteObject("Done!");
        }
    }
}
