using System.Management.Automation;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Abstract base class for transactional TCM cmdlets.
    /// </summary>
    public abstract class TransactionalTcmCmdlet : TcmCmdlet
    {
        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            PSTransactionContext transaction = null;
            try
            {
                if (TransactionAvailable())
                {
                    // Currently only isolation level Serializable is supported by power shell Start-Transaction cmdlet and it won't work with CM due to isolation level Read Committed. 
                    // Microsoft ticket was created for this issue (Support Request Number - 112061334810313) and was closed. ER was created at connect.microsoft.com: https://connect.microsoft.com/PowerShell/feedback/details/753399/possibility-to-change-isolationlevel-on-start-transaction-cmdlet
                    // transaction = CurrentPSTransaction;
                }
                base.ProcessRecord();
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
            }
        }
    }
}
