using System.Linq;
using System.Management.Automation;
using Tridion.ContentManager.CoreService.Client;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Gets the repositories from the system.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "TcmRepository")]
    public class GetTcmRepositoryCommand : TcmCmdlet
    {
        /// <summary>
        /// Gets or sets the identifiers for repositories to get.
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Specifies one or more repository by id.", ValueFromPipeline = true)]
        public string[] Ids { get; set; }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            if (Ids == null || !Ids.Any())
            {
                WriteObject(CoreServiceClient.GetSystemWideList(new RepositoriesFilterData()), true);
                return;
            }
            Ids.ToList().ForEach(id => WriteObject((RepositoryData)CoreServiceClient.Read(id, null)));
        }
    }
}
