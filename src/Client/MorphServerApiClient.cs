using Morph.Server.Sdk.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Morph.Server.Sdk.Events;
using Morph.Server.Sdk.Dto.Commands;
using Morph.Server.Sdk.Mappers;
using Morph.Server.Sdk.Model.Commands;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Morph.Server.Sdk.Model.InternalModels;
using Morph.Server.Sdk.Dto;
using System.Collections.Concurrent;
using Morph.Server.Sdk.Exceptions;
using Morph.Server.Sdk.Model.SharedMemory;

namespace Morph.Server.Sdk.Client
{
    public partial class MorphServerApiClient : IMorphServerApiClient, IDisposable, ICanCloseSession
    {

        public event EventHandler<FileTransferProgressEventArgs> OnDataDownloadProgress;
        public event EventHandler<FileTransferProgressEventArgs> OnDataUploadProgress;

        protected readonly string _userAgent = "MorphServerApiClient/next";
        protected readonly string _api_v1 = "api/v1/";

        private readonly ILowLevelApiClient _lowLevelApiClient;
        protected readonly IRestClient RestClient;
        private ClientConfiguration clientConfiguration = new ClientConfiguration();

        private bool _disposed = false;
        private object _lock = new object();


        public IClientConfiguration Config => clientConfiguration;

        internal ILowLevelApiClient BuildApiClient(ClientConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // handler will be disposed automatically
            HttpClientHandler aHandler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Automatic,
                ServerCertificateCustomValidationCallback = configuration.ServerCertificateCustomValidationCallback
            };

            var httpClient = BuildHttpClient(configuration, aHandler);
            var restClient = ConstructRestApiClient(httpClient, BuildBaseAddress(configuration), clientConfiguration);
            return new LowLevelApiClient(restClient);
        }



        /// <summary>
        /// Construct Api client
        /// </summary>
        /// <param name="apiHost">Server url</param>
        public MorphServerApiClient(Uri apiHost)
        {
            if (apiHost == null)
            {
                throw new ArgumentNullException(nameof(apiHost));
            }

            var defaultConfig = new ClientConfiguration
            {
                ApiUri = apiHost
            };
            clientConfiguration = defaultConfig;
            _lowLevelApiClient = BuildApiClient(clientConfiguration);
            RestClient = _lowLevelApiClient.RestClient;
        }

        public MorphServerApiClient(ClientConfiguration clientConfiguration)
        {
            this.clientConfiguration = clientConfiguration ?? throw new ArgumentNullException(nameof(clientConfiguration));
            _lowLevelApiClient = BuildApiClient(clientConfiguration);
            RestClient = _lowLevelApiClient.RestClient;
        }


        protected virtual IRestClient ConstructRestApiClient(HttpClient httpClient, Uri baseAddress, ClientConfiguration clientConfiguration)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            return new MorphServerRestClient(httpClient, baseAddress,
                clientConfiguration.LegacySessionRefresher,
                clientConfiguration.HttpSecurityState);
        }


        protected virtual Uri BuildBaseAddress(ClientConfiguration config)
        {
           var baseAddress = new Uri(config.ApiUri, new Uri(_api_v1, UriKind.Relative));
           return baseAddress;

        }


        protected virtual HttpClient BuildHttpClient(ClientConfiguration config, HttpClientHandler httpClientHandler)
        {
            if (httpClientHandler == null)
            {
                throw new ArgumentNullException(nameof(httpClientHandler));
            }

            var client = new HttpClient(httpClientHandler, true);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                {
                    CharSet = "utf-8"
                });
            client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            client.DefaultRequestHeaders.Add("X-Client-Type", config.ClientType);
            client.DefaultRequestHeaders.Add("X-Client-Id", config.ClientId);
            client.DefaultRequestHeaders.Add("X-Client-Sdk", config.SDKVersionString);

            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=120");

            
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };



            client.Timeout = config.HttpClientTimeout;

            return client;
        }


        /// <summary>
        /// Start Task like "fire and forget"
        /// </summary>
        public Task<ComputationDetailedItem> StartTaskAsync(ApiSession apiSession, string spaceName, StartTaskRequest startTaskRequest, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (startTaskRequest == null)
            {
                throw new ArgumentNullException(nameof(startTaskRequest));
            }

            return WrappedWithSession(async (token) =>
            {
                var requestDto = new TaskStartRequestDto()
                {
                    TaskId = startTaskRequest.TaskId,
                    TaskParameters = startTaskRequest.TaskParameters?.Select(TaskParameterMapper.ToDto)?.ToList()
                };

                var apiResult = await _lowLevelApiClient.StartTaskAsync(apiSession, spaceName, requestDto, token);
                return MapOrFail(apiResult, ComputationDetailedItemMapper.FromDto);

            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }

        public Task<ComputationDetailedItem> GetComputationDetailsAsync(ApiSession apiSession, string spaceName, string computationId, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }


            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.GetComputationDetailsAsync(apiSession, spaceName, computationId, token);
                return MapOrFail(apiResult, ComputationDetailedItemMapper.FromDto);

            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }

        public Task CancelComputationAsync(ApiSession apiSession, string spaceName, string computationId, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }


            return WrappedWithSession(async (token) =>
            {
                await _lowLevelApiClient.CancelComputationAsync(apiSession, spaceName, computationId, token);
                return Task.FromResult(0);

            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }

        public Task<WorkflowResultDetails> GetWorkflowResultDetailsAsync(ApiSession apiSession, string spaceName, string resultToken, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }


            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.GetWorkflowResultDetailsAsync(apiSession, spaceName, resultToken, token);
                return MapOrFail(apiResult, WorkflowResultDetailsMapper.FromDto);

            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }

        public Task AcknowledgeWorkflowResultAsync(ApiSession apiSession, string spaceName, string resultToken, CancellationToken cancellationToken)
        {
            return WrappedWithSession(async (token) =>
            {
                await _lowLevelApiClient.AcknowledgeWorkflowResultAsync(apiSession, spaceName, resultToken, token);
                return Task.FromResult(0);

            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }

        protected virtual async Task<TResult> WrappedWithSession<TResult>(Func<CancellationToken, Task<TResult>> fun, CancellationToken orginalCancellationToken, OperationType operationType, ApiSession apiSession)
        {
            if (apiSession is null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MorphServerApiClient));
            }


            TimeSpan maxExecutionTime;
            switch (operationType)
            {
                case OperationType.FileTransfer:
                    maxExecutionTime = clientConfiguration.FileTransferTimeout; break;
                case OperationType.ShortOperation:
                    maxExecutionTime = clientConfiguration.OperationTimeout; break;
                case OperationType.SessionOpenAndRelated:
                    maxExecutionTime = clientConfiguration.SessionOpenTimeout; break;
                default: throw new NotImplementedException();
            }


            CancellationTokenSource derTokenSource = null;
            try
            {
                derTokenSource = CancellationTokenSource.CreateLinkedTokenSource(orginalCancellationToken);
                {
                    derTokenSource.CancelAfter(maxExecutionTime);
                    try
                    {
                        return await fun(derTokenSource.Token);
                    }

                    catch (OperationCanceledException) when (!orginalCancellationToken.IsCancellationRequested && derTokenSource.IsCancellationRequested)
                    {
                        if (operationType == OperationType.SessionOpenAndRelated)
                        {
                            throw new Exception($"Can't connect to host {clientConfiguration.ApiUri}.  Operation timeout ({maxExecutionTime})");
                        }
                        else
                        {
                            throw new Exception($"Operation timeout ({maxExecutionTime}) when processing command to host {clientConfiguration.ApiUri}");
                        }
                    }
                    catch(MorphApiUnauthorizedException morphApiUnauthorizedException)
                    {
                        var data = new ApiSessionUnauthenticatedEventData(this, apiSession);
                        await AppWideApiSessionEventNotifier.Instance.InvokeOnApiSessionUnauthenticated(data).ConfigureAwait(false);
                        throw;
                    }
                }
            }
            finally
            {
                if (derTokenSource != null)
                {
                    if (operationType == OperationType.FileTransfer)
                    {
                        RegisterForDisposing(derTokenSource);
                    }
                    else
                    {
                        derTokenSource.Dispose();
                    }
                }
            }

        }

        protected virtual async Task<TResult> WrappedNoSession<TResult>(Func<CancellationToken, Task<TResult>> fun, CancellationToken orginalCancellationToken, OperationType operationType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MorphServerApiClient));
            }


            TimeSpan maxExecutionTime;
            switch (operationType)
            {
                case OperationType.FileTransfer:
                    maxExecutionTime = clientConfiguration.FileTransferTimeout; break;
                case OperationType.ShortOperation:
                    maxExecutionTime = clientConfiguration.OperationTimeout; break;
                case OperationType.SessionOpenAndRelated:
                    maxExecutionTime = clientConfiguration.SessionOpenTimeout; break;
                default: throw new NotImplementedException();
            }


            CancellationTokenSource derTokenSource = null;
            try
            {
                derTokenSource = CancellationTokenSource.CreateLinkedTokenSource(orginalCancellationToken);
                {
                    derTokenSource.CancelAfter(maxExecutionTime);
                    try
                    {
                        return await fun(derTokenSource.Token);
                    }

                    catch (OperationCanceledException) when (!orginalCancellationToken.IsCancellationRequested && derTokenSource.IsCancellationRequested)
                    {
                        if (operationType == OperationType.SessionOpenAndRelated)
                        {
                            throw new Exception($"Can't connect to host {clientConfiguration.ApiUri}.  Operation timeout ({maxExecutionTime})");
                        }
                        else
                        {
                            throw new Exception($"Operation timeout ({maxExecutionTime}) when processing command to host {clientConfiguration.ApiUri}");
                        }
                    }
                }
            }
            finally
            {
                if (derTokenSource != null)
                {
                    if (operationType == OperationType.FileTransfer)
                    {
                        RegisterForDisposing(derTokenSource);
                    }
                    else
                    {
                        derTokenSource.Dispose();
                    }
                }
            }

        }

        private ConcurrentBag<CancellationTokenSource> _ctsForDisposing = new ConcurrentBag<CancellationTokenSource>();

        private void RegisterForDisposing(CancellationTokenSource derTokenSource)
        {
            if (derTokenSource == null)
            {
                throw new ArgumentNullException(nameof(derTokenSource));
            }

            _ctsForDisposing.Add(derTokenSource);
        }

        protected virtual void FailIfError<TDto>(ApiResult<TDto> apiResult)
        {
            if (!apiResult.IsSucceed)
            {
                throw apiResult.Error;
            }
        }



        protected virtual TDataModel MapOrFail<TDto, TDataModel>(ApiResult<TDto> apiResult, Func<TDto, TDataModel> maper)
        {
            if (apiResult.IsSucceed)
            {
                return maper(apiResult.Data);
            }
            else
            {
                throw apiResult.Error;
            }
        }


        /// <summary>
        /// Close opened session
        /// </summary>
        /// <param name="apiSession">api session</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ICanCloseSession.CloseSessionAsync(ApiSession apiSession, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }
            
            if (apiSession.IsAnonymous)
                return Task.FromResult(0);

            return WrappedNoSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.AuthLogoutAsync(apiSession, token);
                // if task fail - do nothing. server will close this session after inactivity period
                return Task.FromResult(0);

            }, cancellationToken, OperationType.ShortOperation);

        }


      


      
        /// <summary>
        /// Change task mode
        /// </summary>
        /// <param name="apiSession">api session</param>
        /// <param name="taskId">task guid</param>
        /// <param name="taskChangeModeRequest"></param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Returns task status</returns>
        public Task<SpaceTask> TaskChangeModeAsync(ApiSession apiSession, string spaceName, Guid taskId, TaskChangeModeRequest taskChangeModeRequest, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (taskChangeModeRequest is null)
            {
                throw new ArgumentNullException(nameof(taskChangeModeRequest));
            }

            return WrappedWithSession(async (token) =>
            {
                var request = new SpaceTaskChangeModeRequestDto
                {
                    TaskEnabled = taskChangeModeRequest.TaskEnabled
                };
                var apiResult = await _lowLevelApiClient.TaskChangeModeAsync(apiSession, spaceName, taskId, request, token);
                return MapOrFail(apiResult, (dto) => SpaceTaskMapper.MapFull(dto));

            }, cancellationToken, OperationType.ShortOperation, apiSession);

        }


        /// <summary>
        /// Retrieves space status
        /// </summary>
        /// <param name="apiSession">api session</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<SpaceStatus> GetSpaceStatusAsync(ApiSession apiSession, string spaceName, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.SpacesGetSpaceStatusAsync(apiSession,spaceName , token);
                return MapOrFail(apiResult, (dto) => SpaceStatusMapper.MapFromDto(dto));

            }, cancellationToken, OperationType.ShortOperation, apiSession);

        }


        public HttpSecurityState HttpSecurityState => RestClient.HttpSecurityState;

        /// <summary>
        /// Returns server status. May raise exception if server is unreachable
        /// </summary>
        /// <returns></returns>
        public Task<ServerStatus> GetServerStatusAsync(CancellationToken cancellationToken)
        {
            return WrappedNoSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.ServerGetStatusAsync(token);
                return MapOrFail(apiResult, (dto) => ServerStatusMapper.MapFromDto(dto));

            }, cancellationToken, OperationType.SessionOpenAndRelated);
        }

        public async Task<SpacesEnumerationList> GetSpacesAccessibleListAsync(ApiSession apiSession, CancellationToken cancellationToken)
        {
            return await WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.SpacesGetAccessibleListAsync(apiSession, token);
                return MapOrFail(apiResult, (dto) => SpacesEnumerationMapper.MapFromDto(dto));

            }, cancellationToken, OperationType.SessionOpenAndRelated, apiSession);
        }

        public async Task<SpacesEnumerationList> GetSpacesListAsync(CancellationToken cancellationToken)
        {
            return await WrappedNoSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.SpacesGetListAsync(token);
                return MapOrFail(apiResult, (dto) => SpacesEnumerationMapper.MapFromDto(dto));

            }, cancellationToken, OperationType.SessionOpenAndRelated);
        }

        public async Task<SpacesLookupResponse> SpacesLookupAsync(SpacesLookupRequest request, CancellationToken cancellationToken)
        {
            return await WrappedNoSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.SpacesLookupAsync(SpacesLookupMapper.ToDto(request), token);
                return MapOrFail(apiResult, (dto) => SpacesLookupMapper.MapFromDto(dto));

            }, cancellationToken, OperationType.SessionOpenAndRelated);
        }
       

        protected void TriggerOnDataDownloadProgress(FileTransferProgressEventArgs e)
        {
            OnDataDownloadProgress?.Invoke(this, e);
        }

        protected void TriggerOnDataUploadProgress(FileTransferProgressEventArgs e)
        {
            OnDataUploadProgress?.Invoke(this, e);
        }




        /// <summary>
        /// Prerforms browsing the Space
        /// </summary>
        /// <param name="apiSession">api session</param>
        /// <param name="folderPath">folder path like /path/to/folder</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<SpaceBrowsingInfo> SpaceBrowseAsync(ApiSession apiSession, string spaceName, string folderPath, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.WebFilesBrowseSpaceAsync(apiSession, spaceName, folderPath, token);
                return MapOrFail(apiResult, (dto) => SpaceBrowsingMapper.MapFromDto(dto));

            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }


        /// <summary>
        /// Checks if file exists
        /// </summary>
        /// <param name="apiSession">api session</param>
        /// <param name="serverFolder">server folder like /path/to/folder</param>
        /// <param name="fileName">file name </param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns true if file exists.</returns>
        public Task<bool> SpaceFileExistsAsync(ApiSession apiSession, string spaceName, string serverFilePath, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(serverFilePath))
            {
                throw new ArgumentException(nameof(serverFilePath));
            }

            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.WebFileExistsAsync(apiSession, spaceName, serverFilePath, token);
                return MapOrFail(apiResult, (dto) => dto);
            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }


        /// <summary>
        /// Performs file deletion
        /// </summary>
        /// <param name="apiSession">api session</param>
        /// <param name="serverFolder">Path to server folder like /path/to/folder</param>
        /// <param name="fileName">file name</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SpaceDeleteFileAsync(ApiSession apiSession, string spaceName, string serverFilePath, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.WebFilesDeleteFileAsync(apiSession, spaceName, serverFilePath, token);
                FailIfError(apiResult);
                return Task.FromResult(0);

            }, cancellationToken, OperationType.ShortOperation, apiSession);

        }



        /// <summary>
        ///  Performs file renaming
        /// </summary>
        /// <param name="apiSession">API session</param>
        /// <param name="parentFolderPath"> parent folder path like /path/to/folder</param>
        /// <param name="oldFileName"> old file name</param>
        /// <param name="newFileName"> new file name</param>
        /// <param name="cancellationToken"> cancellation token</param>
        /// <exception cref="ArgumentNullException"> if apiSession is null</exception>
        public Task SpaceRenameFileAsync(ApiSession apiSession, string spaceName, string parentFolderPath, string oldFileName, string newFileName,
            CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.WebFilesRenameFileAsync(apiSession, spaceName, parentFolderPath, oldFileName, newFileName, token);
                FailIfError(apiResult);
                return Task.FromResult(0);

            }, cancellationToken, OperationType.ShortOperation, apiSession);

        }

        /// <summary>
        ///     Deletes folder
        /// </summary>
        /// <param name="apiSession">api session</param>
        /// <param name="serverFolderPath">Path to server folder like /path/to/folder</param>
        /// <param name="failIfNotExists">Fails with error if folder does not exist</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task SpaceDeleteFolderAsync(ApiSession apiSession, string spaceName, string serverFolderPath, bool failIfNotExists, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.WebFilesDeleteFolderAsync(apiSession, spaceName, serverFolderPath, failIfNotExists, token);
                FailIfError(apiResult);
                return Task.FromResult(0);
            }, cancellationToken, OperationType.ShortOperation,apiSession);
        }

        /// <summary>
        ///     Creates a folder
        /// </summary>
        /// <param name="apiSession">api session</param>
        /// <param name="parentFolderPath">Path to server folder like /path/to/folder</param>
        /// <param name="folderName"></param>
        /// <param name="failIfExists">Fails with error if target folder exists already</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task SpaceCreateFolderAsync(ApiSession apiSession,
            string spaceName, 
            string parentFolderPath, string folderName,
            bool failIfExists, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.WebFilesCreateFolderAsync(apiSession, spaceName, parentFolderPath, folderName, failIfExists, token);
                FailIfError(apiResult);
                return Task.FromResult(0);
            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }

        /// <summary>
        ///     Renames a folder
        /// </summary>
        /// <param name="apiSession">api session</param>
        /// <param name="parentFolderPath">Path to containing server folder like /path/to/folder</param>
        /// <param name="oldFolderName">Old folder name</param>
        /// <param name="newFolderName">New folder name</param>
        /// <param name="failIfExists">Fails with error if target folder exists already</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task SpaceRenameFolderAsync(ApiSession apiSession, string spaceName, string parentFolderPath, string oldFolderName, string newFolderName,
            bool failIfExists, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.WebFilesRenameFolderAsync(apiSession, spaceName, parentFolderPath, oldFolderName, newFolderName,
                    failIfExists, token);
                FailIfError(apiResult);
                return Task.FromResult(0);
            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }

        /// <summary>
        /// Validate tasks. Checks that there are no missing parameters in the tasks. 
        /// </summary>
        /// <param name="apiSession">api session</param>
        /// <param name="projectPath">project path like /path/to/project.morph </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ValidateTasksResult> ValidateTasksAsync(ApiSession apiSession, string spaceName, string projectPath, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(projectPath))
            {
                throw new ArgumentException("projectPath is empty", nameof(projectPath));
            }

            return WrappedWithSession(async (token) =>
            {
                var request = new ValidateTasksRequestDto
                {
                    SpaceName = spaceName,
                    ProjectPath = projectPath
                };
                var apiResult = await _lowLevelApiClient.ValidateTasksAsync(apiSession, request, token);
                return MapOrFail(apiResult, (dto) => ValidateTasksResponseMapper.MapFromDto(dto));

            }, cancellationToken, OperationType.ShortOperation, apiSession);

        }


        public async Task<AuthenticatedUser> GetCurrentAuthenticatedUserAsync(ApiSession apiSession, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            return await WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.GetCurrentAuthenticatedUser(apiSession, token);
                return MapOrFail(apiResult, (dto) => AuthenticatedUserMaper.Map(dto));

            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }

        public Task<ApiSession> OpenSessionAsync(AnonymousIdP anonymousIdP, CancellationToken  cancellationToken)
        {
            if (anonymousIdP is null)
            {
                throw new ArgumentNullException(nameof(anonymousIdP));
            }
            cancellationToken.ThrowIfCancellationRequested();

            ApiSession session = ApiSessionFactory.CreateAnonymousSession();
            return Task.FromResult(session);

        }



         


        /// <summary>
        /// Opens session based on required authentication mechanism
        /// </summary>
        /// <param name="openSessionRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiSession> OpenLegacySessionAsync(OpenLegacySessionRequest openSessionRequest, CancellationToken ct)
        {
            if (openSessionRequest == null)
            {
                throw new ArgumentNullException(nameof(openSessionRequest));
            }
            if (string.IsNullOrWhiteSpace(openSessionRequest.SpaceName))
            {
                throw new ArgumentException("Space name is not set.", nameof(openSessionRequest.SpaceName));
            }

            using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {

                var timeout = clientConfiguration.SessionOpenTimeout;
                linkedTokenSource.CancelAfter(timeout);
                var cancellationToken = linkedTokenSource.Token;
                try
                {
                    // tring to resolve space and space auth method.
                    // Method 1. using get all spaces method
                    ApiResult<SpacesEnumerationDto> spacesListApiResult = await _lowLevelApiClient.SpacesGetListAsync(cancellationToken);
                    SpaceEnumerationItem desiredSpace = null;
                    if (!spacesListApiResult.IsSucceed && spacesListApiResult.Error is MorphApiForbiddenException)
                    {
                        // space listing disabled has been disabled be server admin. 
                        // Method 2. Using spaces lookup (new endpoint since next version of EM Server 4.3)
                        var lookupApiResult = await _lowLevelApiClient.SpacesLookupAsync(new SpacesLookupRequestDto() { SpaceNames = { openSessionRequest.SpaceName } }, cancellationToken);
                        desiredSpace = MapOrFail(lookupApiResult,
                            (dto) =>
                            {
                                // response have at least 1 element with requested space.
                                var lookup = dto.Values.First();
                                if (lookup.Error != null)
                                {
                                    // seems that space not found.
                                    throw new Exception($"Unable to open session. {lookup.Error.message}");
                                }
                                else
                                {
                                    // otherwise return space
                                    return SpacesEnumerationMapper.MapItemFromDto(lookup.Data);
                                }
                            }
                            );
                    }
                    else
                    {
                        var spacesListResult = MapOrFail(spacesListApiResult, (dto) => SpacesEnumerationMapper.MapFromDto(dto));
                        desiredSpace = spacesListResult.Items.FirstOrDefault(x => x.SpaceName.Equals(openSessionRequest.SpaceName, StringComparison.OrdinalIgnoreCase));
                    }

                    if (desiredSpace == null)
                    {
                        throw new Exception($"Unable to open session. Server has no space '{openSessionRequest.SpaceName}'");
                    }

                    var authenticator = CreateAuthenticator(openSessionRequest, desiredSpace);

                    return await authenticator(cancellationToken);
                }
                catch (OperationCanceledException) when (!ct.IsCancellationRequested && linkedTokenSource.IsCancellationRequested)
                {
                    throw new Exception($"Can't connect to host {clientConfiguration.ApiUri}.  Operation timeout ({timeout})");
                }
            }

        }


        private OpenSessionAuthenticatorContext CreateContext()
        {
            return new OpenSessionAuthenticatorContext(_lowLevelApiClient,
                        this,
                        (handler) =>
                            ConstructRestApiClient(
                                BuildHttpClient(clientConfiguration, handler),
                                BuildBaseAddress(clientConfiguration), clientConfiguration));
        }

        private Authenticator CreateAuthenticator(OpenLegacySessionRequest openSessionRequest, SpaceEnumerationItem desiredSpace)
        {
            var requestClone = openSessionRequest.Clone();

            return async ctoken =>
            {
                var session = await MorphServerLegacyAuthenticator.OpenLegacySessionMultiplexedAsync(desiredSpace,
                    CreateContext(),
                    requestClone,
                    ctoken);

                if (session != null && session is LegacyApiSession legacyApiSession)
                {
                    Config.LegacySessionRefresher.AssociateAuthenticator(legacyApiSession, CreateAuthenticator(requestClone, desiredSpace));
                }

                return session;
            };
        }


        public Task<AuthProvidersList> GetAuthProvidersList(CancellationToken cancellationToken)
        {
            
            return WrappedNoSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.AuthGetProvidersListAsync( token);
                return MapOrFail(apiResult, (dto) => AuthProvidersListMapper.MapFromDto(dto));

            }, cancellationToken, OperationType.ShortOperation);
        }

        public Task<SpaceTasksList> GetTasksListAsync(ApiSession apiSession, string spaceName, CancellationToken cancellationToken)
        {
            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.GetTasksListAsync(apiSession, spaceName, token);
                return MapOrFail(apiResult, (dto) => TasksListDtoMapper.MapFromDto(dto));

            }, cancellationToken, OperationType.ShortOperation, apiSession);

        }

        public Task<SpaceTask> GetTaskAsync(ApiSession apiSession, string spaceName, Guid taskId, CancellationToken cancellationToken)
        {
            return WrappedWithSession(async (token) =>
            {
                var apiResult = await _lowLevelApiClient.GetTaskAsync(apiSession, spaceName, taskId, token);
                return MapOrFail(apiResult, (dto) => SpaceTaskMapper.MapFull(dto));

            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }


        public Task<SharedMemoryValue> SharedMemoryRemember(ApiSession apiSession, string spaceName, string key, SharedMemoryValue value,
            OverwriteBehavior overwriteBehavior,
            CancellationToken token)
        {
            return WrappedWithSession(async t =>
            {
                var valueDto = SharedMemoryValueMapper.MapToDto(value);

                var apiResult =
                    await _lowLevelApiClient.SharedMemoryRemember(apiSession, spaceName, key, valueDto, overwriteBehavior, t);
                return MapOrFail(apiResult, SharedMemoryValueMapper.MapFromDto);
            }, token, OperationType.ShortOperation,apiSession);
        }

        public async Task<SharedMemoryValue> SharedMemoryRecall(ApiSession apiSession, string spaceName, string key,
            CancellationToken token)
        {
            return await WrappedWithSession(async t =>
            {
                var apiResult = await _lowLevelApiClient.SharedMemoryRecall(apiSession, spaceName, key, t);
                return MapOrFail(apiResult, SharedMemoryValueMapper.MapFromDto);
            }, token, OperationType.ShortOperation, apiSession);
        }

        public async Task<SharedMemoryListResponse> SharedMemoryList(ApiSession apiSession, string spaceName, string startsWith,
            int offset, int limit, CancellationToken token)
        {
            return await WrappedWithSession(async t =>
            {
                var apiResult = await _lowLevelApiClient.SharedMemoryList(apiSession,  spaceName, startsWith, offset, limit, t);
                return MapOrFail(apiResult, SharedMemoryValueMapper.MapFromDto);
            }, token, OperationType.ShortOperation, apiSession);
        }

        public async Task<int> SharedMemoryForget(ApiSession apiSession, string spaceName, string key, CancellationToken token)
        {
            return await WrappedWithSession(async t =>
            {
                var apiResult = await _lowLevelApiClient.SharedMemoryForget(apiSession, spaceName, key, t);
                return MapOrFail(apiResult, dto => dto.DeletedCount);
            }, token, OperationType.ShortOperation, apiSession);
        }


        public Task<ServerStreamingData> SpaceOpenStreamingDataAsync(ApiSession apiSession, string spaceName, string remoteFilePath, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            return WrappedWithSession(async (token) =>
            {
                Action<FileTransferProgressEventArgs> onReceiveProgress = TriggerOnDataDownloadProgress;
                var apiResult = await _lowLevelApiClient.WebFilesDownloadFileAsync(apiSession,spaceName, remoteFilePath, onReceiveProgress, token);
                return MapOrFail(apiResult, (data) => new ServerStreamingData(data.Stream, data.FileName, data.FileSize)
                );

            }, cancellationToken, OperationType.FileTransfer, apiSession);
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed)
                    return;
                if (_lowLevelApiClient != null)
                    _lowLevelApiClient.Dispose();

                Array.ForEach(_ctsForDisposing.ToArray(), z => z.Dispose());
                _disposed = true;
            }
        }

        public Task<Stream> SpaceOpenDataStreamAsync(ApiSession apiSession, string spaceName, string remoteFilePath, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            return WrappedWithSession(async (token) =>
            {
                Action<FileTransferProgressEventArgs> onReceiveProgress = TriggerOnDataDownloadProgress;
                var apiResult = await _lowLevelApiClient.WebFilesDownloadFileAsync(apiSession, spaceName, remoteFilePath, onReceiveProgress, token);
                return MapOrFail(apiResult, (data) => data.Stream);

            }, cancellationToken, OperationType.FileTransfer, apiSession);
        }

        public Task SpaceUploadDataStreamAsync(ApiSession apiSession, string spaceName, SpaceUploadDataStreamRequest spaceUploadFileRequest, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (spaceUploadFileRequest == null)
            {
                throw new ArgumentNullException(nameof(spaceUploadFileRequest));
            }

            return WrappedWithSession(async (token) =>
            {
                Action<FileTransferProgressEventArgs> onSendProgress = TriggerOnDataUploadProgress;
                var sendStreamData = new SendFileStreamData(
                    spaceUploadFileRequest.DataStream,
                    spaceUploadFileRequest.FileName,
                    spaceUploadFileRequest.FileSize);
                var apiResult =
                    spaceUploadFileRequest.OverwriteExistingFile ?
                    await _lowLevelApiClient.WebFilesPutFileStreamAsync(apiSession, spaceName, spaceUploadFileRequest.ServerFolder, sendStreamData, onSendProgress, token) :
                    await _lowLevelApiClient.WebFilesPostFileStreamAsync(apiSession, spaceName, spaceUploadFileRequest.ServerFolder, sendStreamData, onSendProgress, token);
                FailIfError(apiResult);
                return Task.FromResult(0);

            }, cancellationToken, OperationType.FileTransfer, apiSession);
        }

        public Task<SpaceFilesQuickSearchResponse> SpaceFilesQuickSearchAsync(ApiSession apiSession, string spaceName, SpaceFilesQuickSearchRequest request, CancellationToken cancellationToken, int? offset = null, int? limit = null)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return WrappedWithSession(async (token) =>
            {
                var requestDto = SpaceFilesQuickSearchRequestMapper.ToDto(request);
                var apiResult = await _lowLevelApiClient.WebFilesQuickSearchSpaceAsync(apiSession, spaceName, requestDto, offset, limit,  token);
                return MapOrFail(apiResult, (dto) => SpaceFilesQuickSearchResponseMapper.MapFromDto(dto));

            }, cancellationToken, OperationType.ShortOperation, apiSession);
        }
        
        /// <summary>
        /// Performs file upload using push callback
        /// </summary>
        /// <param name="apiSession">API session</param>
        /// <param name="continuousStreamRequest">What to upload</param>
        /// <param name="pushStreamCallback">Callback that has to provide data to upload</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task SpaceUploadPushDataStreamAsync(ApiSession apiSession,
            string spaceName,
            SpaceUploadContiniousStreamRequest continuousStreamRequest,
            PushStreamCallback pushStreamCallback, 
            CancellationToken cancellationToken)
        {
            if (apiSession == null)
                throw new ArgumentNullException(nameof(apiSession));
            if (continuousStreamRequest == null)
                throw new ArgumentNullException(nameof(continuousStreamRequest));
            if(continuousStreamRequest.FileName == null)
                throw new ArgumentNullException(nameof(continuousStreamRequest.FileName));
            if (continuousStreamRequest.ServerFolder == null)
                throw new ArgumentNullException(nameof(continuousStreamRequest.ServerFolder));
            if (pushStreamCallback == null)
                throw new ArgumentNullException(nameof(pushStreamCallback));

            return WrappedWithSession(async token =>
            {
                var request = new PushFileStreamData
                {
                    FileName = continuousStreamRequest.FileName,
                    IfMatch = continuousStreamRequest.IfMatch,
                    PushCallback = pushStreamCallback,
                };
                
                var result =
                    continuousStreamRequest.OverwriteExistingFile
                        ? await _lowLevelApiClient.WebFilesPushPutFileStreamAsync(apiSession, spaceName,
                            continuousStreamRequest.ServerFolder, request, token)
                        : await _lowLevelApiClient.WebFilesPushPostFileStreamAsync(apiSession, spaceName,
                            continuousStreamRequest.ServerFolder, request, token);

                FailIfError(result);

                return Task.CompletedTask;
            }, cancellationToken, OperationType.FileTransfer, apiSession);
        }

        public async Task<ApiSession> OpenSessionAsync(SpacePwdIdP provider, string spaceName, string password, CancellationToken ct)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));

            }

            return await OpenSessionWrapper(async (cancellationToken) =>
            {
                var session =
                    await SpacePwdAuthenticator.OpenSessionViaSpacePasswordAsync(CreateContext(), spaceName, password, cancellationToken);
                return session;

            }, ct);
        }

        private async Task<ApiSession> OpenSessionWrapper(Func<CancellationToken, Task<ApiSession>> apiSessionFactory, CancellationToken ct)
        {
            using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {

                var timeout = clientConfiguration.SessionOpenTimeout;
                linkedTokenSource.CancelAfter(timeout);
                var cancellationToken = linkedTokenSource.Token;
                try
                {
                    var apiSession = await apiSessionFactory(cancellationToken);

                    return apiSession;
                }
                catch (OperationCanceledException) when (!ct.IsCancellationRequested && linkedTokenSource.IsCancellationRequested)
                {
                    throw new Exception($"Can't connect to host {clientConfiguration.ApiUri}.  Operation timeout ({timeout})");
                }
            }
        }

        public async Task<ApiSession> OpenSessionAsync(InternalIdP provider, string userName, string password, bool keepSignedIn, CancellationToken ct)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException($"'{nameof(userName)}' cannot be null or empty.", nameof(userName));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            return await OpenSessionWrapper(async (cancellationToken) =>
            {
                var session =
                    await InternalIdPAuthenticator.OpenSessionUserPasswordAsync(CreateContext(), userName, password, keepSignedIn,  cancellationToken);
                return session;

            }, ct);
        }

        public async Task<ApiSession> OpenSessionAsync(AdSeamlessIdP provider, bool keepSignedId, CancellationToken ct)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            return await OpenSessionWrapper(async (cancellationToken) =>
            {
                var session =
                    await AdSeamlessIdPAuthenticator.OpenSession(CreateContext(), provider.IdPId, keepSignedId, cancellationToken);
                return session;

            }, ct);
        }

        public Task<ApiSession> OpenAnonymousSessionAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ApiSession session = ApiSessionFactory.CreateAnonymousSession();
            return Task.FromResult(session);
        }

        
    }

}