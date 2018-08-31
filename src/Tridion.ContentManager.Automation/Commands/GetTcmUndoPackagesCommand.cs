using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Gets a list of undo packages stored in the application data (to be available across scaled-out TCM instances).
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "TcmUndoPackages")]
    public class GetTcmUndoPackagesCommand : TcmUndoPackagesCmdlet
    {
        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected override void ProcessCoreServiceRecord()
        {
            var undoPackagesList = GetUndoPackages();
            WriteObject(undoPackagesList, true);
        }
    }
}