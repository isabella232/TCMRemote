using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Tridion.ContentManager.CoreService.Client;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Purges specified queues (Search, Publish, Workflow Agent, Deploy, Batch) messages.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "TcmQueueMessages", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveTcmQueueMessagesCommand : TransactionalTcmCmdlet
    {
        /// <summary>
        /// Gets or sets List of PredefinedQueue names for which must perform purging process.
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Specifies the list of a Tridion.ContentManager.CoreService.Client.PredefinedQueue enum for which will be performed purging process, if not specified - the purging process will be performed for all queues.")]
        public string[] Queues { get; set; }

        /// <summary>
        /// Gets the should process message.
        /// </summary>
        /// <value>The should process message.</value>
        protected override string ShouldProcessMessage
        {
            get
            {
                return string.Join(", ", GetQueues());
            }
        }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            foreach (var queue in GetQueues())
            {
                CoreServiceClient.PurgeQueue((int)queue);
            }
        }

        private IEnumerable<PredefinedQueue> GetQueues()
        {
            if (Queues != null && Queues.Any())
            {
                return Queues.Select(item => Enum.Parse(typeof(PredefinedQueue), item)).Cast<PredefinedQueue>().ToList();
            }
            IList<PredefinedQueue> queues = Enum.GetValues(typeof(PredefinedQueue)).Cast<PredefinedQueue>().ToList();
            queues.Remove(PredefinedQueue.UnknownByClient);
            return queues;
        }
    }
}
