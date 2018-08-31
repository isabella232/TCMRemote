using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Gets a list of undo packages stored in the application data (to be available across scaled-out TCM instances).
    /// </summary>
    public abstract class TcmUndoPackagesCmdlet : TcmCmdlet
    {
        protected List<UndoPackageInfo> GetUndoPackages()
        {
            var undoAppData = CoreServiceClient.ReadAllApplicationData(null).Where(appData => appData.ApplicationId.StartsWith("UndoPackage")).ToList();
            var packages = undoAppData.Where(appData => appData.ApplicationId.StartsWith("UndoPackage_")).ToList();
            var packagesMeta = undoAppData.Where(appData => appData.ApplicationId.StartsWith("UndoPackageMetadata_")).ToList();

            List<UndoPackageInfo> undoPackagesList = new List<UndoPackageInfo>();
            foreach (var packageMeta in packagesMeta)
            {
                string metaString = Encoding.UTF8.GetString(packageMeta.Data);
                var meta = new UndoPackageInfo(packageMeta.ApplicationId, metaString);
                if (packages.Any(packageAppData => packageAppData.ApplicationId == meta.PackageId))
                {
                    undoPackagesList.Add(meta);
                }
            }
            return undoPackagesList;
        }
    }
}