using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Tridion.ContentManager.CoreService.Client;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Deletes TCM batches 
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "TcmBatches", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveTcmBatchesCommand : TransactionalTcmCmdlet
    {
        /// <summary>
        /// Gets or sets a value indicating whether all batches or only completed will be deleted.
        /// </summary>
        [Parameter(HelpMessage = "If specified we delete all batches, otherwise only completed.")]
        public SwitchParameter All { get; set; }

        /// <summary>
        /// Gets the should process message.
        /// </summary>
        /// <value>The should process message.</value>
        protected override string ShouldProcessMessage
        {
            get
            {
                return (All ? "All" : "Only completed") + " TCM batches";
            }
        }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            BatchesFilterData filter = new BatchesFilterData()
                                       {
                                           BaseColumns = CoreService.Client.ListBaseColumns.Default
                                       };
            IEnumerable<BatchData> batchDatas = CoreServiceClient.GetSystemWideList(filter).Cast<BatchData>().ToList();
            foreach (var batchData in batchDatas)
            {
                if (All || batchData.TotalNumberOfOperations == batchData.NumberOfDoneOperations)
                {
                    CoreServiceClient.Delete(batchData.Id);
                    WriteObject(batchData);
                }
            }
        }
    }
}
