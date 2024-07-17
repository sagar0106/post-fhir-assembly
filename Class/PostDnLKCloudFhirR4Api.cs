using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using PropertyGridExt;
using HL7UtilV2;
using LKEventLog;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Linq;
using Ellkay.Events;
using System.Xml.Linq;
using FhirR4API;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Model;
using static Hl7.Fhir.Model.Bundle;
using Newtonsoft.Json;

namespace PostDnLKCloudFhirR4Api.Class
{
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class PostDnLKCloudFhirR4Api : IDisposable
    {
        private readonly LKEventLogHelper<PostDnLKCloudFhirR4Api> _LKEventLogHelper;

        private bool _disposedValue = false; //To detect redundant calls
        private string _configFileName = "";
        private string _fileContent = "";
        private TrxFile _trxFile;
        private HL7Util _hl7Util;

        public PostDnLKCloudFhirR4Api()
        {
            _hl7Util = new HL7Util();
            _LKEventLogHelper = new LKEventLogHelper<PostDnLKCloudFhirR4Api>();
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) below.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    // TODO: free unmanaged resources when explicitly called
                    _hl7Util = null;
                }

                // TODO: free shared unmanaged resources
            }
            this._disposedValue = true;
        }

        [CategoryAttribute("Folder"),
         DisplayName("Config File Name"),
         DescriptionAttribute("Specify the file name of Configuration."),
         DefaultValueAttribute(""),
         BrowsableAttribute(false)]
        public string ConfigFileName
        {
            get { return _configFileName; }
            set
            {
                _configFileName = value;
                if (_configFileName.Length > 0)
                    FillProperties();
            }
        }

        [PropertyOrder(1)]
        [CategoryAttribute("Authorization"),
        DisplayName("Authorization URL"),
        DescriptionAttribute("Specify Authorization Url to fetch token"),
        DefaultValueAttribute(""),
        BrowsableAttribute(true)]
        public string AuthorizationUrl { get; set; }

        [PropertyOrder(2)]
        [CategoryAttribute("Authorization"),
        DisplayName("Client Id"),
        DescriptionAttribute("Specify Client Id"),
        DefaultValueAttribute(""),
        BrowsableAttribute(true)]
        public string ClientId { get; set; }

        [PropertyOrder(3)]
        [CategoryAttribute("Authorization"),
        DisplayName("Client Secret"),
        DescriptionAttribute("Specify Client Secret"),
        DefaultValueAttribute(""),
        BrowsableAttribute(true)]
        public string ClientSecret { get; set; }

        [PropertyOrder(1)]
        [CategoryAttribute("Request"),
        DisplayName("FHIR Base URL"),
        DescriptionAttribute("Specify the FHIR Base URL"),
        DefaultValueAttribute(""),
        BrowsableAttribute(true)]
        public string FHIRBaseUrl { get; set; }

        [PropertyOrder(2)]
        [CategoryAttribute("Request"),
        DisplayName("Site Service Key"),
        DescriptionAttribute("Specify the Site Service Key"),
        DefaultValueAttribute(""),
        BrowsableAttribute(true)]
        public string SiteServiceKey { get; set; }

        [PropertyOrder(3)]
        [CategoryAttribute("Request"),
         DisplayName("Response Files Folder"),
         DescriptionAttribute("Specify the folder to drop the FHIR responses."),
         DefaultValueAttribute(""),
         BrowsableAttribute(true)]
        public string ResponseFilesFolder { get; set; }

        /// <summary>
        /// Restore the original values from Config file
        /// </summary>
        /// <remarks></remarks>
        public void Restore()
        {
            FillProperties();
        }

        /// <summary>
        /// Executes the post-download assembly and returns a ResultDetails object with details on post-download processing.
        /// </summary>
        /// <param name="oConfigRow"></param>
        /// <param name="oDataRow"></param>
        /// <returns>A ResultDetails response object containing an LKTransfer Status, Error Message, Debug Log and original File Bytes</returns>
        public ResultDetails Execute(DataRow oConfigRow, DataRow oDataRow)
        {
            var sw = new Stopwatch();
            sw.Start();

            var resultDetails = new ResultDetails() { Status = ResultDetails.LKTransferResponseStatus.Error };
            var accountNumber = oConfigRow["UserID"].ToString();
            var interfaceType = oConfigRow["InterfaceType"].ToString();
            var fileName = oDataRow["FileName"].ToString();
            IFileSpecificEventFactory fileSpecificEvents = null;

            try
            {
                fileSpecificEvents = _LKEventLogHelper.CreateFileSpecificEvents(accountNumber, interfaceType, fileName);
                fileSpecificEvents.EnterAssembly();
                fileSpecificEvents.EnterSpecificAssembly<LKEventLog.IFileSpecificEventFactory>();
                fileSpecificEvents.EnterSpecificAssembly<HL7UtilV2.HL7Util>();
                fileSpecificEvents.EnterSpecificAssembly<Ellkay.Events.EventFactory>();
                fileSpecificEvents.EnterSpecificAssembly<Newtonsoft.Json.JsonConverter>();
                fileSpecificEvents.EnterSpecificAssembly<log4net.AssemblyInfo>();
                fileSpecificEvents.EnterSpecificAssembly<PropertiesDeluxeTypeConverter>();
                fileSpecificEvents.EnterSpecificAssembly<FhirR4APIHelper>();

                // Populate TrxFileRecord
                _trxFile = new TrxFile(oDataRow);
                _trxFile.FileFolder = oConfigRow["DeliveryFolderLocation"].ToString();

                if (_trxFile.Content == null || _trxFile.Content.Length == 0)
                {
                    fileSpecificEvents.Debug("Nothing to process exiting");
                    resultDetails.ErrorMessage = "File content is blank";
                    return resultDetails;
                }
                else
                {
                    resultDetails.Body = _trxFile.Content;
                    _fileContent = _hl7Util.ByteArrayToStr(_trxFile.Content);
                }

                //Ensure all assembly properties are provided 
                ValidateProperties(fileSpecificEvents);

                resultDetails = ProcessData(resultDetails, fileSpecificEvents).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                if (fileSpecificEvents != null)
                {
                    fileSpecificEvents.Exception("Unexpected exception occurred in Execute().", ex);
                    fileSpecificEvents.Debug("Marking file as Error.");
                }
                else
                {
                    resultDetails.DebugLog = ex.ToString();
                }
                resultDetails = _hl7Util.ErrorResultDetails(resultDetails, ex.Message, ResultDetails.LKTransferResponseStatus.Error);
            }
            finally
            {
                sw.Stop();

                if (fileSpecificEvents != null)
                {
                    fileSpecificEvents.Debug($"File Processed in {sw.ElapsedMilliseconds} milliseconds.");
                    fileSpecificEvents.ExitAssembly();
                    resultDetails.DebugLog += $"\n{fileSpecificEvents.GetLogs()}";
                }
            }

            return resultDetails;
        }

        /// <summary>
        /// Saves Configuration record
        /// </summary>
        /// <returns>True on success</returns>
        /// <remarks></remarks>
        public bool Save()
        {
            try
            {
                //Validate and then save
                if (Validate() == false)
                    return false;

                // Read Data from XML file Create DataSet and Get Table
                DataTable oTable = GetConfigDataSet().Tables[0];

                //Update Changes                               
                oTable.Rows[0]["AuthorizationUrl"] = AuthorizationUrl;
                oTable.Rows[0]["ClientId"] = ClientId;
                oTable.Rows[0]["ClientSecret"] = ClientSecret;
                oTable.Rows[0]["FHIRBaseUrl"] = FHIRBaseUrl;
                oTable.Rows[0]["SiteServiceKey"] = SiteServiceKey;
                oTable.Rows[0]["ResponseFilesFolder"] = ResponseFilesFolder;

                oTable.AcceptChanges();

                // Write to Disk
                oTable.WriteXml(ConfigFileName, XmlWriteMode.WriteSchema);
                return true;
            }
            catch (Exception ex)
            {
                _LKEventLogHelper.CreateTopLevelEvents().Exception("Unexpected exception occurred in Save().", ex);
                return false;
            }
        }

        private void ValidateProperties(IFileSpecificEventFactory events)
        {
            bool isValid = true;

            void CheckProperty(string value, string propertyName)
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    events.Debug($"Please specify {propertyName}.");
                    isValid = false;
                }
            }

            CheckProperty(ResponseFilesFolder, nameof(ResponseFilesFolder));

            if (!isValid)
            {
                throw new Exception("Required assembly properties missing! See logs for details.");
            }
        }

        private async Task<ResultDetails> ProcessData(ResultDetails resultDetails, IFileSpecificEventFactory events)
        {

            var fhirSerializer = new FhirJsonSerializer();
            var fhirParser = new FhirJsonParser();
            Auth auth = new Auth { AuthorizationUrl = AuthorizationUrl, ClientID = ClientId, ClientSecret = ClientSecret };
            FhirR4APIHelper fhirR4API = new FhirR4APIHelper(auth, events);
            try
            {
                // logic
                Bundle bundle = fhirParser.Parse<Bundle>(_fileContent);

                bool hasPatientResource = false;
                if (bundle.Entry.Find(x => x.Resource.TypeName.ToUpper() == "PATIENT") != null)
                {
                    hasPatientResource = true;
                }

                if (!hasPatientResource)
                {
                    throw new Exception($"Patient resource not found in FHIR Bundle.");
                }

                bundle.Type = BundleType.BatchResponse;

                string xRequestId = string.Empty;
                Patient pResource = bundle.GetResources().Where(p => p.TypeName.ToUpper() == "PATIENT").FirstOrDefault() as Patient;
                xRequestId = pResource.Extension.Any() ? pResource.Extension?.Find(x => x.ElementId == "intermountain-XRequestID")?.Value?.ToString() : "";

                if (String.IsNullOrWhiteSpace(xRequestId))
                {
                    Encounter eResource = bundle.GetResources().Where(p => p.TypeName.ToUpper() == "ENCOUNTER").FirstOrDefault() as Encounter;
                    xRequestId = eResource.Extension.Any() ? eResource.Extension?.Find(x => x.ElementId == "intermountain-XRequestID")?.Value?.ToString() : "";
                }


                foreach (EntryComponent entry in bundle.Entry)
                {
                    if (entry.Request != null)
                    {
                        if (entry.Request.Method != null)
                        {
                            if (entry.Request.Method == HTTPVerb.POST)
                            {
                                string serialized = fhirSerializer.SerializeToString(entry.Resource);
                                if (entry.Resource.TypeName == "Coverage")
                                {
                                    Encounter encounterResource = bundle.GetResources().Where(p => p.TypeName.ToUpper() == "ENCOUNTER").FirstOrDefault() as Encounter;
                                    string finNumber = encounterResource.Extension.Any() ? encounterResource.Extension[0].Extension?.Find(x => x.ElementId == "custom-intermountain-fin-fin")?.Value?.ToString() : "";
                                    if (!String.IsNullOrWhiteSpace(finNumber))
                                    {
                                        serialized = serialized.Replace("<FIN_NUMBER>", finNumber);
                                    }
                                }
                                dynamic response = await fhirR4API.CreateRecord(serialized, entry.Resource.TypeName, FHIRBaseUrl, SiteServiceKey, xRequestId).ConfigureAwait(false);
                                if (response.responseString.Contains("OperationOutcome"))
                                {
                                    OperationOutcome outcome = fhirParser.Parse<OperationOutcome>(response);
                                    string errorMessages = String.Join("; ", outcome.Issue.Select(x => x.Diagnostics));
                                    if (errorMessages.Contains("Authentication Error") || errorMessages.Contains("Authentication failed") || errorMessages.Contains("Credentials are not valid") || response.httpStatus == "401-Unauthorized" || response.httpStatus == "403-Forbidden")
                                    {
                                        await fhirR4API.ExpireToken();
                                        throw new TransientException(errorMessages);
                                    }
                                    else
                                    {
                                        throw new Exception(errorMessages);
                                    }
                                }
                                else
                                {
                                    entry.Request = null;
                                    if (entry.Resource.TypeName == "Patient")
                                    {
                                        Patient patient = fhirParser.Parse<Patient>(response.responseString);
                                        if (!String.IsNullOrWhiteSpace(patient?.Id))
                                        {
                                            response = await fhirR4API.GetRecord($"{FHIRBaseUrl}/{entry.Resource.TypeName}/{patient.Id}", SiteServiceKey).ConfigureAwait(false);
                                            entry.Resource = fhirParser.Parse<Patient>(response.responseString);
                                        }
                                    }
                                    else if (entry.Resource.TypeName == "Encounter")
                                        entry.Resource = fhirParser.Parse<Encounter>(response.responseString);

                                    entry.Response = new ResponseComponent();
                                    entry.Response.Status = response.httpStatus;
                                }
                            }
                            else
                            {
                                throw new Exception($"Request.Method {entry.Request.Method} is not supported.");
                            }
                        }
                        else
                        {
                            throw new Exception($"Request.Method is missing for entry {entry.Request.Method}.");
                        }
                    }
                }
                File.WriteAllText(Path.Combine(ResponseFilesFolder, _hl7Util.GetNextFileName(ResponseFilesFolder, Path.GetFileName(_trxFile.FileName))), fhirSerializer.SerializeToString(bundle));
                resultDetails.Status = ResultDetails.LKTransferResponseStatus.Success;
                return resultDetails;
            }
            catch (AggregateException aggEx)
            {
                events.Exception("Unexpected exception occurred while processing the file.", aggEx);

                int errorStatus = ResultDetails.LKTransferResponseStatus.Error;
                string errorMessages = String.Join("; ", aggEx.InnerExceptions.Select(x => x.Message));

                //unwrap the AggregateException and loop through each one to try and identify transient errors, otherwise mark file as error 
                foreach (Exception innerException in aggEx.InnerExceptions)
                {
                    errorStatus = EvaluateExceptionForTransientError(innerException, events, fhirR4API);

                    if (errorStatus == ResultDetails.LKTransferResponseStatus.TransientError)
                    {
                        break;
                    }
                }

                return _hl7Util.ErrorResultDetails(resultDetails, $"Error: {errorMessages}", errorStatus);
            }
            catch (Exception ex)
            {
                events.Exception("Unexpected exception occurred while processing the file.", ex);
                events.Debug("Marking file as Error.");
                return _hl7Util.ErrorResultDetails(resultDetails, $"Error: {ex.Message}", EvaluateExceptionForTransientError(ex, events, fhirR4API));
            }
        }

        private int EvaluateExceptionForTransientError(Exception ex, IEventFactory events, FhirR4APIHelper fhirR4API)
        {
            try
            {
                switch (ex)
                {
                    case CustomAPIException customResponseEx:
                        return GetErrorStatusBasedOnHttpStatusCode(customResponseEx.HttpResponse.StatusCode, events, fhirR4API);
                    case TransientException _:
                    case HttpRequestException _:
                    case WebException _:
                    case SocketException _:
                    case TaskCanceledException _:
                    case TimeoutException _:
                        events.Debug("Marking file as TransientError. An unknown exception was thrown.");
                        return ResultDetails.LKTransferResponseStatus.TransientError;
                    default:
                        events.Debug("Marking file as Error. An unknown exception was thrown.");
                        return ResultDetails.LKTransferResponseStatus.Error;
                }
            }
            catch (Exception unexpectedException)
            {
                events.Exception("Unexpected exception occurred in EvaluateExceptionForTransientError().", unexpectedException);
                events.Debug("Marking file as Error. An unexpected Exception was thrown.");
                return ResultDetails.LKTransferResponseStatus.Error;
            }
        }

        private int GetErrorStatusBasedOnHttpStatusCode(HttpStatusCode statusCode, IEventFactory events, FhirR4APIHelper fhirR4API)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized: //401 - requires human intervention to fix the credentials, mark as transient-error so once credentials are fixed queue continues, If token expire unauthorized
                    events.Debug("Marking file as TransientError. Credentials are invalid or Token is invalid.");
                    fhirR4API.ExpireToken().GetAwaiter().GetResult();
                    return ResultDetails.LKTransferResponseStatus.TransientError;
                case HttpStatusCode.ServiceUnavailable: //503 - api is down, hold message queue & keep retrying 
                    events.Debug("Marking file as TransientError. API is currently unavailable.");
                    return ResultDetails.LKTransferResponseStatus.TransientError;
                case HttpStatusCode.Forbidden: //403 - token expired 
                    events.Debug("Marking file as TransientError - token expired. Token will refresh on next attempt.");
                    fhirR4API.ExpireToken().GetAwaiter().GetResult();
                    return ResultDetails.LKTransferResponseStatus.TransientError;
                default:
                    events.Debug("Marking file as Error.");
                    return ResultDetails.LKTransferResponseStatus.Error;
            }
        }

        /// <summary>
        /// Validate Configuration record
        /// </summary>
        /// <returns>True on success</returns>
        /// <remarks></remarks>
        private bool Validate()
        {
            // Validate Porperties for this object
            return true;
        }

        /// <summary>
        /// Get Configuration DataSet
        /// </summary>
        /// <returns>DataSet</returns>
        /// <remarks></remarks>
        private DataSet GetConfigDataSet()
        {
            try
            {
                var ds = new DataSet();

                if (File.Exists(_configFileName) == false)
                {
                    _hl7Util.CheckConfigFolder(ConfigFileName);
                    ValidateDataSet(ref ds, _configFileName);
                }

                ds.ReadXml(_configFileName, XmlReadMode.Auto);

                //Check if the dataset is validated
                ValidateDataSet(ref ds, _configFileName);
                return ds;
            }
            catch (Exception ex)
            {
                _LKEventLogHelper.CreateTopLevelEvents().Exception("Unexpected exception occurred in GetConfigDataSet().", ex);
                return null;
            }
        }

        /// <summary>
        /// Fill Properties
        /// </summary>
        /// <returns></returns>
        /// <remarks>Fills properties from configuration record</remarks>
        private bool FillProperties()
        {
            try
            {
                DataTable oTable = GetConfigDataSet().Tables[0];
                if (oTable.Rows.Count == 0)
                    return true;

                AuthorizationUrl = Convert.ToString(oTable.Rows[0]["AuthorizationUrl"]);
                ClientId = Convert.ToString(oTable.Rows[0]["ClientId"]);
                ClientSecret = Convert.ToString(oTable.Rows[0]["ClientSecret"]);
                FHIRBaseUrl = Convert.ToString(oTable.Rows[0]["FHIRBaseUrl"]);
                SiteServiceKey = Convert.ToString(oTable.Rows[0]["SiteServiceKey"]);
                ResponseFilesFolder = Convert.ToString(oTable.Rows[0]["ResponseFilesFolder"]);
                return true;
            }
            catch (Exception ex)
            {
                _LKEventLogHelper.CreateTopLevelEvents().Exception("Unexpected exception occurred in FillProperties().", ex);
                return false;
            }
        }

        /// <summary>
        /// Validate DataSet, Verify all fields exists or create it and write an XML file
        /// </summary>
        /// <param name="toDataSet">DataSet</param>
        /// <param name="XmlFileName">XML file name</param>
        /// <remarks></remarks>
        private void ValidateDataSet(ref DataSet toDataSet, string XmlFileName)
        {
            //If General.DataSetValidated = True Then
            //    Return
            //End If

            if (toDataSet.Tables.Count == 0)
            {
                toDataSet.Tables.Add("TableProcess");
            }

            // Check the dataset for new fields
            bool isFieldUpdated = false;

            _hl7Util.CheckField(ref toDataSet, "ConfigFileName", typeof(string), "", ref isFieldUpdated);
            _hl7Util.CheckField(ref toDataSet, "AuthorizationUrl", typeof(string), "", ref isFieldUpdated);
            _hl7Util.CheckField(ref toDataSet, "ClientId", typeof(string), "", ref isFieldUpdated);
            _hl7Util.CheckField(ref toDataSet, "ClientSecret", typeof(string), "", ref isFieldUpdated);
            _hl7Util.CheckField(ref toDataSet, "FHIRBaseUrl", typeof(string), "", ref isFieldUpdated);
            _hl7Util.CheckField(ref toDataSet, "SiteServiceKey", typeof(string), "", ref isFieldUpdated);
            _hl7Util.CheckField(ref toDataSet, "ResponseFilesFolder", typeof(string), "", ref isFieldUpdated);

            //If any fields have been updated, save the dataset first to the file
            if (isFieldUpdated == true)
            {
                if (toDataSet.Tables[0].Rows.Count == 0)
                {
                    toDataSet.Tables[0].Rows.Add();
                }

                toDataSet.Tables[0].AcceptChanges();
                toDataSet.Tables[0].WriteXml(XmlFileName, XmlWriteMode.WriteSchema);
            }

            //Set the flag that the dataset is now validated
            //DataSetValidated = true;
        }
    }
}
