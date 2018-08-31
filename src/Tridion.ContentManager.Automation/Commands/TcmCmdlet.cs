using System;
using System.Management.Automation;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using Tridion.ContentManager.CoreService.Client;

namespace Tridion.ContentManager.Automation.Commands
{
    /// <summary>
    /// Abstract base class for TCM cmdlets.
    /// </summary>
    public abstract class TcmCmdlet : Cmdlet
    {
        private readonly NetTcpBinding _coreServiceNetTcpBinding = new NetTcpBinding
        {
            TransactionProtocol = TransactionProtocol.OleTransactions,
            TransactionFlow = true,
            MaxReceivedMessageSize = Int32.MaxValue,
            ReaderQuotas =
                new XmlDictionaryReaderQuotas { MaxStringContentLength = Int32.MaxValue, MaxArrayLength = Int32.MaxValue }
        };

        private readonly NetTcpBinding _streamDownloadNetTcpBinding = new NetTcpBinding
        {
            TransferMode = TransferMode.StreamedResponse,
            MaxReceivedMessageSize = Int32.MaxValue,
            SendTimeout = new TimeSpan(0, 10, 0) // 10 minutes
        };

        private SessionAwareCoreServiceClient _coreServiceClient;
        private StreamDownloadClient _streamDownloadClient;
        private TimeSpan _timeout = new TimeSpan(0, 10, 0);

        #region Parameters

        /// <summary>
        /// Gets or sets the CM server host name
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the port of the Core Service net.tcp endpoints
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the send and receive timeout (in minutes) of the Core Service net.tcp endpoints.
        /// </summary>
        [Parameter(Mandatory = false)]
        public int TimeOut
        {
            get { return _timeout.Minutes; }
            set { _timeout = new TimeSpan(0, value, 0); }
        }

        #endregion

        #region Overridables
        /// <summary>
        /// Gets the should process message.
        /// </summary>
        /// <value>The should process message.</value>
        protected virtual string ShouldProcessMessage
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Processes the core service record.
        /// </summary>
        /// <remarks>Used for proper error handling of core service fault exception.</remarks>
        protected abstract void ProcessCoreServiceRecord();
        #endregion

        #region Internals

        /// <summary>
        /// Gets the Core Service base URL (excluding the endpoint name).
        /// </summary>
        /// <returns>
        /// The core service base url.
        /// </returns>
        private string GetCoreServiceBaseUrl()
        {
            string coreServiceHost = Server ?? "localhost";
            int coreServicePort = (Port == 0) ? 2660 : Port;
#pragma warning disable 436 // Type conflicts with imported type
            return string.Format("net.tcp://{0}:{1}/CoreService/201701", coreServiceHost, coreServicePort);
#pragma warning restore 436 // Type conflicts with imported type
        }

        /// <summary>
        /// Modifies the endpoint to add the DataContractSerializerOperationBehavior.
        /// </summary>
        private static void ModifyDataContractSerializerBehavior(ServiceEndpoint endpoint)
        {
            foreach (OperationDescription operation in endpoint.Contract.Operations)
            {
                // Find the serializer behavior.
                DataContractSerializerOperationBehavior behavior =
                    operation.Behaviors.Find<DataContractSerializerOperationBehavior>();

                // If the serializer is not found, create one and add it.
                if (behavior == null)
                {
                    behavior = new DataContractSerializerOperationBehavior(operation);
                    operation.Behaviors.Add(behavior);
                }

                // Change the settings of the behavior.
                behavior.MaxItemsInObjectGraph = 2147483647;
            }
        }

        /// <summary>
        /// Gets the Core Service client interface.
        /// </summary>
        protected ISessionAwareCoreService CoreServiceClient
        {
            get
            {
                if (_coreServiceClient == null)
                {
                    Uri coreServiceUrl = new Uri(GetCoreServiceBaseUrl() + "/netTcp");
                    WriteVerbose("Using Core Service URL: " + coreServiceUrl);

                    SetNetTcpBindingTimeoutValue(_coreServiceNetTcpBinding);
                    

                    _coreServiceClient = new SessionAwareCoreServiceClient(
                        _coreServiceNetTcpBinding, new EndpointAddress(coreServiceUrl));

                    ModifyDataContractSerializerBehavior(_coreServiceClient.Endpoint);
                }

                return _coreServiceClient;
            }
        }

        private void SetNetTcpBindingTimeoutValue(NetTcpBinding netTcpBinding)
        {
            netTcpBinding.ReceiveTimeout = _timeout;
            netTcpBinding.SendTimeout = _timeout;
        }

        /// <summary>
        /// Gets the Stream Download client interface
        /// </summary>
        protected IStreamDownload StreamDownloadClient
        {
            get
            {
                if (_streamDownloadClient == null)
                {
                    Uri streamDownloadUrl = new Uri(GetCoreServiceBaseUrl() + "/streamDownload_netTcp");
                    WriteVerbose("Using Stream Download URL: " + streamDownloadUrl);
                    SetNetTcpBindingTimeoutValue(_streamDownloadNetTcpBinding);
                    _streamDownloadClient = new StreamDownloadClient(
                        _streamDownloadNetTcpBinding, new EndpointAddress(streamDownloadUrl));

                    ModifyDataContractSerializerBehavior(_streamDownloadClient.Endpoint);
                }
                return _streamDownloadClient;
            }
        }

        /// <summary>
        /// Gets the TCM API version as reported by the Core Service.
        /// </summary>
        protected string TcmApiVersion { get; private set; }

        #endregion

        #region Overrides

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        /// <remarks>
        /// We open the Core Service proxy here.
        /// </remarks>
        protected override void BeginProcessing()
        {
            WriteVerbose("TcmCmdlet.BeginProcessing...");
            TcmApiVersion = CoreServiceClient.GetApiVersion();
            WriteVerbose("TCM API version = " + TcmApiVersion);
        }

        /// <summary>
        /// Provides a one-time, postprocessing functionality for the cmdlet.
        /// </summary>
        /// <remarks>
        /// We close the Core Service proxies here.
        /// </remarks>
        protected override void EndProcessing()
        {
            WriteVerbose("TcmCmdlet.EndProcessing");

            if (_coreServiceClient != null)
            {
                _coreServiceClient.Dispose();
            }

            if (_streamDownloadClient != null)
            {
                _streamDownloadClient.Dispose();
            }
        }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            string message = ShouldProcessMessage;
            if (!string.IsNullOrEmpty(message))
            {
                if (!ShouldProcess(message))
                {
                    return;
                }
                WriteVerbose(message);
            }

            try
            {
                ProcessCoreServiceRecord();
            }
            catch (FaultException<CoreServiceFault> e)
            {
                ErrorRecord errorRecord = new ErrorRecord(e, e.Detail.ErrorCode, ErrorCategory.NotSpecified, null);
                WriteError(errorRecord);
            }
        }
        #endregion
    }
}
