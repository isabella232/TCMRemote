using System;
using System.Management.Automation;
using Tridion.ContentManager.CoreService.Client;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Gets the list of the publish transactions.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "TcmPublishTransaction")]
    public class GetTcmPublishTransactionCommand : TcmCmdlet
    {
        private const string GetByIdParameterSetName = "GetById";
        private const string GetListParameterSetName = "GetList";

        /// <summary>
        /// Gets or sets the identifier of the publish transaction to get.
        /// </summary>
        [Parameter(ParameterSetName = GetByIdParameterSetName, Position = 0, HelpMessage = "Publish transaction identifier.", ValueFromPipeline = true, Mandatory = true)]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the server with Publisher service that handles publish transactions.
        /// </summary>
        [Parameter(ParameterSetName = GetListParameterSetName, HelpMessage = "The name of the server with Publisher service that handles publish transactions.", ValueFromPipeline = true, Mandatory = false)]
        public string PublisherHost
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter condition, whether publish transactions must be completed.
        /// </summary>
        [Parameter(ParameterSetName = GetListParameterSetName, HelpMessage = "Handling of the publish transaction must be completed or not?", ValueFromPipeline = true, Mandatory = false)]
        public bool? IsCompleted 
        {
            get;
            set;
        }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception. </remarks>
        protected override void ProcessCoreServiceRecord()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                IdentifiableObjectData result = CoreServiceClient.Read(Id, null);
                if (!(result is PublishTransactionData))
                {
                    throw new ArgumentException(string.Format("Expected item type: {0}.", ItemType.PublishTransaction), "Id");
                }
                WriteObject(result);
            }
            else
            {
                PublishTransactionsFilterData filter = new PublishTransactionsFilterData
                {
                    PublisherHost = PublisherHost,
                    IsCompleted = IsCompleted
                };
                WriteObject(CoreServiceClient.GetSystemWideList(filter), true);
            }
        }
    }
}
