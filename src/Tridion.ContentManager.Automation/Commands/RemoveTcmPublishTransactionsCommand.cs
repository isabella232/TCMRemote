using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Tridion.ContentManager.CoreService.Client;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Deletes TCM Publish Transactions, If no parameter specified then purges all finished transactions.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "TcmPublishTransactions", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class RemoveTcmPublishTransactionsCommand : TransactionalTcmCmdlet
    {
        #region Parameters

        /// <summary>
        /// Gets or sets a value indicating whether successful transaction will be deleted or not.
        /// </summary>
        [Parameter(HelpMessage = "If is specified, the deleting will occur for successful transactions, otherwise not>.")]
        public SwitchParameter Successful { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether failed transaction will be deleted or not.
        /// </summary>
        [Parameter(HelpMessage = "If is specified, the deleting will occur for failed transactions, otherwise not>.")]
        public SwitchParameter Failed { get; set; }

        /// <summary>
        /// Gets or sets date and delete publish transaction completed before this date.
        /// </summary>
        [Parameter(HelpMessage = "Delete publish transactions which are completed before this date.")]
        public DateTime? Before { get; set; }

        #endregion

        /// <summary>
        /// Gets the should process message.
        /// </summary>
        /// <value>The should process message.</value>
        protected override string ShouldProcessMessage
        {
            get
            {
                string message = Before.HasValue
                                    ? "TCM publish transactions items completed before" + Before
                                    : "All TCM publish transactions";
                if (Successful)
                {
                    message += ", are successful";
                }
                if (Failed)
                {
                    message += ", are failed";
                }
                return message;
            }
        }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            PublishTransactionsFilterData filter = new PublishTransactionsFilterData()
                                                   {
                                                       EndDate = Before
                                                   };
            IEnumerable<PublishTransactionData> list = CoreServiceClient.GetSystemWideList(filter).Cast<PublishTransactionData>();
            foreach (var publishTransactionData in list)
            {
                if ((!Successful && !Failed)
                    || (Successful && publishTransactionData.State == PublishTransactionState.Success)
                        || (Failed && publishTransactionData.State == PublishTransactionState.Failed))
                {
                    CoreServiceClient.Delete(publishTransactionData.Id);
                    WriteObject(publishTransactionData);
                }
            }
        }
    }
}
