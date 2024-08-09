using Morph.Server.Sdk.Dto;
using Morph.Server.Sdk.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Morph.Server.Sdk.Dto.Commands;
using Morph.Server.Sdk.Mappers;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Morph.Server.Sdk.Exceptions;
using Morph.Server.Sdk.Model.InternalModels;
using Morph.Server.Sdk.Helper;
using Morph.Server.Sdk.Events;
using System.Net.Http;
using Morph.Server.Sdk.Dto.SpaceFilesSearch;
using System.Collections.Specialized;
using Morph.Server.Sdk.Dto.SharedMemory;
using Morph.Server.Sdk.Model.SharedMemory;
using Morph.Server.Sdk.Dto.Auth;

namespace Morph.Server.Sdk.Client
{
    internal partial class LowLevelApiClient : ILowLevelApiClient
    {
        private readonly IRestClient apiClient;

        public IRestClient RestClient => apiClient;

        public LowLevelApiClient(IRestClient apiClient)
        {
            this.apiClient = apiClient;
        }


        public Task<ApiResult<AuthProvidersDto>> AuthGetProvidersListAsync(CancellationToken cancellationToken)
        {
            var url = "auth/providers";
            return apiClient.GetAsync<AuthProvidersDto>(url, null, new HeadersCollection(), cancellationToken);

        }

        public Task<ApiResult<NoContentResult>> AuthLogoutAsync(ApiSession apiSession, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }
            var url = "auth/logout";
            return apiClient.PostAsync<NoContentRequest, NoContentResult>(url, null, null, apiSession.ToHeadersCollection(), cancellationToken);
        }


        public Task<ApiResult<AuthenticatedUserDto>> GetCurrentAuthenticatedUser(ApiSession apiSession, CancellationToken cancellationToken)
        {
            var url = "user/authenticated";
            return apiClient.GetAsync<AuthenticatedUserDto>(url, null, apiSession.ToHeadersCollection(), cancellationToken);

        }


        public Task<ApiResult<SpacesEnumerationDto>> SpacesGetListAsync(CancellationToken cancellationToken)
        {
            var url = "spaces/list";
            return apiClient.GetAsync<SpacesEnumerationDto>(url, null, new HeadersCollection(), cancellationToken);

        }

        public Task<ApiResult<SpacesEnumerationDto>> SpacesGetAccessibleListAsync(ApiSession apiSession, CancellationToken cancellationToken)
        {
            var url = "spaces/list/accessible";
            return apiClient.GetAsync<SpacesEnumerationDto>(url, null, apiSession.ToHeadersCollection(), cancellationToken);

        }

        public Task<ApiResult<SpacesLookupResponseDto>> SpacesLookupAsync(SpacesLookupRequestDto requestDto, CancellationToken cancellationToken)
        {
            var url = "spaces/list/lookup";
            return apiClient.PostAsync<SpacesLookupRequestDto,SpacesLookupResponseDto>(url, requestDto,null, new HeadersCollection(), cancellationToken);
        }

        public Task<ApiResult<TaskFullDto>> GetTaskAsync(ApiSession apiSession, string spaceName, Guid taskId, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            var url = UrlHelper.JoinUrl("space", spaceName, "tasks", taskId.ToString("D"));
            // client supports extended parameters data
            var urlParameters = new NameValueCollection
            {
                { "mode", "extended" },
            };
            return apiClient.GetAsync<TaskFullDto>(url, urlParameters, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public Task<ApiResult<TasksListDto>> GetTasksListAsync(ApiSession apiSession, string spaceName, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            var url = UrlHelper.JoinUrl("space", spaceName, "tasks");

            return apiClient.GetAsync<TasksListDto>(url, null, apiSession.ToHeadersCollection(), cancellationToken);
        }


     

        public Task<ApiResult<TaskFullDto>> TaskChangeModeAsync(ApiSession apiSession, string spaceName,  Guid taskId, SpaceTaskChangeModeRequestDto requestDto, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            var url = UrlHelper.JoinUrl("space", spaceName, "tasks", taskId.ToString("D"), "changeMode");
            // client supports extended parameters data
            var urlParameters = new NameValueCollection
            {
                { "mode", "extended" },
            };

            return apiClient.PostAsync<SpaceTaskChangeModeRequestDto, TaskFullDto>(url, requestDto, urlParameters, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public Task<ApiResult<ServerStatusDto>> ServerGetStatusAsync(CancellationToken cancellationToken)
        {
            var url = "server/status";
            return apiClient.GetAsync<ServerStatusDto>(url, null, new HeadersCollection(), cancellationToken);
        }

        public Task<ApiResult<ComputationDetailedItemDto>> StartTaskAsync(ApiSession apiSession, string spaceName, TaskStartRequestDto taskStartRequestDto, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            var url = UrlHelper.JoinUrl("space", spaceName, "computations", "start", "task");
            
            return apiClient.PostAsync<TaskStartRequestDto, ComputationDetailedItemDto>(url, taskStartRequestDto, null, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public Task<ApiResult<ComputationDetailedItemDto>> GetComputationDetailsAsync(ApiSession apiSession, string spaceName, string computationId, CancellationToken cancellationToken)
        {
            if (apiSession == null) throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            if (computationId == null) throw new ArgumentNullException(nameof(computationId));

            
            var url = UrlHelper.JoinUrl("space", spaceName, "computations", computationId);
            return apiClient.GetAsync<ComputationDetailedItemDto>(url, null, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public Task CancelComputationAsync(ApiSession apiSession, string spaceName, string computationId, CancellationToken cancellationToken)
        {
            if (apiSession == null) throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            if (computationId == null) throw new ArgumentNullException(nameof(computationId));

            
            var url = UrlHelper.JoinUrl("space", spaceName, "computations", computationId);
            return apiClient.DeleteAsync<NoContentRequest>(url, null, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public Task<ApiResult<WorkflowResultDetailsDto>> GetWorkflowResultDetailsAsync(ApiSession apiSession, string spaceName, string resultToken, CancellationToken cancellationToken)
        {
            if (apiSession == null) throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            if (resultToken == null) throw new ArgumentNullException(nameof(resultToken));

            
            var url = UrlHelper.JoinUrl("space", spaceName, "workflows-result", resultToken, "details");
            return apiClient.GetAsync<WorkflowResultDetailsDto>(url, null, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public Task AcknowledgeWorkflowResultAsync(ApiSession apiSession, string spaceName, string resultToken, CancellationToken cancellationToken)
        {
            if (apiSession == null) throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            if (resultToken == null) throw new ArgumentNullException(nameof(resultToken));

            
            var url = UrlHelper.JoinUrl("space", spaceName, "workflows-result", resultToken, "ack");
            return apiClient.PostAsync<NoContentRequest, NoContentResult>(url, null, null, apiSession.ToHeadersCollection(), cancellationToken);
        }


        public Task<ApiResult<ValidateTasksResponseDto>> ValidateTasksAsync(ApiSession apiSession, ValidateTasksRequestDto validateTasksRequestDto, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            
            var url = "commands/validatetasks";

            return apiClient.PostAsync<ValidateTasksRequestDto, ValidateTasksResponseDto>(url, validateTasksRequestDto, null, apiSession.ToHeadersCollection(), cancellationToken);

        }

        public Task<ApiResult<SpaceStatusDto>> SpacesGetSpaceStatusAsync(ApiSession apiSession, string spaceName, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            spaceName = spaceName.Trim();
            var url = UrlHelper.JoinUrl("spaces", spaceName, "status");

            return apiClient.GetAsync<SpaceStatusDto>(url, null, apiSession.ToHeadersCollection(), cancellationToken);

        }

        public Task<ApiResult<SpaceBrowsingResponseDto>> WebFilesBrowseSpaceAsync(ApiSession apiSession, string spaceName, string folderPath, CancellationToken cancellationToken)
        {
            if (apiSession is null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            var url = UrlHelper.JoinUrl("space", spaceName, "browse", folderPath);
            return apiClient.GetAsync<SpaceBrowsingResponseDto>(url, null, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public Task<ApiResult<NoContentResult>> WebFilesDeleteFileAsync(ApiSession apiSession, string spaceName, string serverFilePath, CancellationToken cancellationToken)
        {
            if (apiSession is null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            
            var url = UrlHelper.JoinUrl("space", spaceName, "files", serverFilePath);

            return apiClient.DeleteAsync<NoContentResult>(url, null, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public Task<ApiResult<NoContentResult>> WebFilesRenameFileAsync(ApiSession apiSession,
            string spaceName, 
            string parentFolderPath, string oldFileName,
            string newFileName, CancellationToken cancellationToken)
        {
            if (apiSession == null) throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            if (parentFolderPath == null) throw new ArgumentNullException(nameof(parentFolderPath));
            if (oldFileName == null) throw new ArgumentNullException(nameof(oldFileName));
            if (newFileName == null) throw new ArgumentNullException(nameof(newFileName));

            

            return apiClient.PostAsync<FileRenameRequestDto, NoContentResult>(
                url: UrlHelper.JoinUrl("space", spaceName, "filesops", "rename"),
                model: new FileRenameRequestDto
                {
                    FolderPath = parentFolderPath,
                    OldName = oldFileName,
                    NewName = newFileName,
                },
                urlParameters: null,
                apiSession.ToHeadersCollection(),
                cancellationToken);
        }

        public Task<ApiResult<NoContentResult>> WebFilesDeleteFolderAsync(ApiSession apiSession,
            string spaceName,
            string serverFilePath,
            bool failIfNotExists, CancellationToken cancellationToken)
        {
            if (apiSession is null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            return apiClient.PostAsync<FolderDeleteRequestDto, NoContentResult>(
                url: UrlHelper.JoinUrl("space", spaceName, "foldersops", "delete"),
                model: new FolderDeleteRequestDto
                {
                    FolderPath = serverFilePath,
                    FailIfNotFound = failIfNotExists
                },
                urlParameters: null,
                apiSession.ToHeadersCollection(),
                cancellationToken);
        }

        public Task<ApiResult<NoContentResult>> WebFilesCreateFolderAsync(ApiSession apiSession,
            string spaceName, 
            string parentFolderPath,
            string folderName,
            bool failIfExists, CancellationToken cancellationToken)
        {
            if (apiSession is null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            return apiClient.PostAsync<FolderCreateRequestDto, NoContentResult>(
                url: UrlHelper.JoinUrl("space", spaceName, "foldersops", "create"),
                model: new FolderCreateRequestDto
                {
                    FolderPath = parentFolderPath,
                    FolderName = folderName,
                    FailIfExists = failIfExists,
                },
                urlParameters: null,
                apiSession.ToHeadersCollection(),
                cancellationToken);
        }

        public Task<ApiResult<NoContentResult>> WebFilesRenameFolderAsync(ApiSession apiSession,
            string spaceName,
            string parentFolderPath,
            string oldFolderName, string newFolderName,
            bool failIfExists, CancellationToken cancellationToken)
        {
            if (apiSession is null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            

            return apiClient.PostAsync<FolderRenameRequestDto, NoContentResult>(
                url: UrlHelper.JoinUrl("space", spaceName, "foldersops", "rename"),
                model: new FolderRenameRequestDto
                {
                    FolderPath = parentFolderPath,
                    Name = oldFolderName,
                    NewName = newFolderName,
                    FailIfExists = failIfExists,
                },
                urlParameters: null,
                apiSession.ToHeadersCollection(),
                cancellationToken);
        }

        public Task<ApiResult<LoginResponseDto>> AuthLoginPasswordAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            var url = "auth/login";
            return apiClient.PostAsync<LoginRequestDto, LoginResponseDto>(url, loginRequestDto, null, new HeadersCollection(), cancellationToken);
        }

        public Task<ApiResult<GenerateNonceResponseDto>> AuthGenerateNonce(CancellationToken cancellationToken)
        {
            var url = "auth/nonce";
            return apiClient.PostAsync<GenerateNonceRequestDto, GenerateNonceResponseDto>(url, new GenerateNonceRequestDto(), null, new HeadersCollection(), cancellationToken);
        }

        public void Dispose()
        {
            apiClient.Dispose();
        }

        public Task<ApiResult<FetchFileStreamData>> WebFilesDownloadFileAsync(ApiSession apiSession,
            string spaceName,
            string serverFilePath, Action<FileTransferProgressEventArgs> onReceiveProgress, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            
            var url = UrlHelper.JoinUrl("space", spaceName, "files", serverFilePath);
            return apiClient.RetrieveFileGetAsync(url, null, apiSession.ToHeadersCollection(), onReceiveProgress, cancellationToken);
        }

        public async Task<ApiResult<bool>> WebFileExistsAsync(ApiSession apiSession, string spaceName, string serverFilePath, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            var url = UrlHelper.JoinUrl("space", spaceName, "files", serverFilePath);
            var apiResult = await apiClient.HeadAsync<NoContentResult>(url, null, apiSession.ToHeadersCollection(), cancellationToken);
            //  http ok or http no content means that file exists
            if (apiResult.IsSucceed)
            {
                return ApiResult<bool>.Ok(true, apiResult.ResponseHeaders);
            }
            else
            {
                // if not found, return Ok with false result
                if(apiResult.Error is MorphApiNotFoundException)
                {
                    return ApiResult<bool>.Ok(false, apiResult.ResponseHeaders);
                }
                else
                {
                    // some error occured - return internal error from api result
                    return ApiResult<bool>.Fail(apiResult.Error, apiResult.ResponseHeaders);

                }
            }
        }

        public Task<ApiResult<NoContentResult>> WebFilesPutFileStreamAsync(ApiSession apiSession,
            string spaceName,
            string serverFolder, SendFileStreamData sendFileStreamData, Action<FileTransferProgressEventArgs> onSendProgress, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            if (sendFileStreamData == null)
            {
                throw new ArgumentNullException(nameof(sendFileStreamData));
            }

            
            var url = UrlHelper.JoinUrl("space", spaceName, "files", serverFolder);
            
            return apiClient.PutFileStreamAsync<NoContentResult>(url,sendFileStreamData,  null, apiSession.ToHeadersCollection(), onSendProgress, cancellationToken);

        }

        public Task<ApiResult<NoContentResult>> WebFilesPostFileStreamAsync(ApiSession apiSession, string spaceName, string serverFolder, SendFileStreamData sendFileStreamData, Action<FileTransferProgressEventArgs> onSendProgress, CancellationToken cancellationToken)
        {
            if (apiSession == null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            if (sendFileStreamData == null)
            {
                throw new ArgumentNullException(nameof(sendFileStreamData));
            }

         
            var url = UrlHelper.JoinUrl("space", spaceName, "files", serverFolder);

            return apiClient.PostFileStreamAsync<NoContentResult>(url, sendFileStreamData, null, apiSession.ToHeadersCollection(), onSendProgress, cancellationToken);
        }
        
        public Task<ApiResult<SpaceFilesQuickSearchResponseDto>> WebFilesQuickSearchSpaceAsync(ApiSession apiSession,
            string spaceName,
            SpaceFilesQuickSearchRequestDto request, int? offset, int? limit, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            var urlParameters = new NameValueCollection();
            if (offset.HasValue)
                urlParameters.Add("offset", offset.Value.ToString());
            if (limit.HasValue)
                urlParameters.Add("limit", limit.Value.ToString());

            var url = UrlHelper.JoinUrl("space", spaceName, "files-search", "quick");
            return apiClient.PostAsync<SpaceFilesQuickSearchRequestDto, SpaceFilesQuickSearchResponseDto>(
                url:url, 
                model: request ,
                urlParameters :urlParameters,
                headersCollection: apiSession.ToHeadersCollection(),
                cancellationToken: cancellationToken);
        }
        
        public async Task<ApiResult<NoContentResult>> WebFilesPushPutFileStreamAsync(ApiSession apiSession,
            string spaceName,
            string serverFolder,
            PushFileStreamData pushFileStreamData, CancellationToken cancellationToken)
        {
            if (apiSession == null)  throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            var url = UrlHelper.JoinUrl("space", spaceName, "files", serverFolder);

            return await apiClient.PushStreamAsync<NoContentResult>(HttpMethod.Put, url, pushFileStreamData, urlParameters: null, apiSession.ToHeadersCollection(), 
                cancellationToken);
        }

        public async Task<ApiResult<NoContentResult>> WebFilesPushPostFileStreamAsync(ApiSession apiSession,
            string spaceName,
            string serverFolder,
            PushFileStreamData pushFileStreamData, CancellationToken cancellationToken)
        {
            if (apiSession == null)  throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            var url = UrlHelper.JoinUrl("space", spaceName, "files", serverFolder);

            return await apiClient.PushStreamAsync<NoContentResult>(HttpMethod.Post, url, pushFileStreamData, urlParameters: null, apiSession.ToHeadersCollection(), 
                cancellationToken);
        }

        #region Shared memory

        public async Task<ApiResult<SharedMemoryValueDto>> SharedMemoryRemember(ApiSession apiSession,
            string spaceName,
            string key,
            SharedMemoryValueDto value, OverwriteBehavior overwriteBehavior, CancellationToken cancellationToken)
        {
            if (apiSession == null) throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            
            var url = UrlHelper.JoinUrl("space", spaceName, "sharedmemory", "item");
            
            var setSharedMemoryValueDto = new SetSharedMemoryValueDto
            {
                Key = key,
                Value = value,
                OverwriteBehavior = SharedMemoryValueMapper.MapOverwriteBehavior(overwriteBehavior)
            };
            
            return await apiClient.PutAsync<SetSharedMemoryValueDto, SharedMemoryValueDto>(url, 
                setSharedMemoryValueDto, urlParameters: null, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public async Task<ApiResult<SharedMemoryValueDto>> SharedMemoryRecall(ApiSession apiSession,
            string spaceName,
            string key,
            CancellationToken cancellationToken)
        {
            if (apiSession == null) throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            
            var url = UrlHelper.JoinUrl("space", spaceName, "sharedmemory", "item");
            
            var urlParameters = new NameValueCollection
            {
                { "key", key },
            };
            
            return await apiClient.GetAsync<SharedMemoryValueDto>(url, urlParameters, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public async Task<ApiResult<SharedMemoryListResponseDto>> SharedMemoryList(ApiSession apiSession,
            string spaceName,
            string startsWith, int offset,
            int limit, CancellationToken cancellationToken)
        {
            if (apiSession == null) throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            
            var url = UrlHelper.JoinUrl("space", spaceName, "sharedmemory", "list");
            
            var urlParameters = new NameValueCollection
            {
                { "startsWith", startsWith },
                { "offset", offset.ToString() },
                { "limit", limit.ToString() }
            };

            return await apiClient.GetAsync<SharedMemoryListResponseDto>(url, urlParameters, apiSession.ToHeadersCollection(), cancellationToken);
        }

        public async Task<ApiResult<DeleteSharedMemoryResponseDto>> SharedMemoryForget(ApiSession apiSession,
            string spaceName,
            string key,
            CancellationToken cancellationToken)
        {
            if (apiSession == null) throw new ArgumentNullException(nameof(apiSession));
            if (string.IsNullOrWhiteSpace(spaceName))
            {
                throw new ArgumentException($"'{nameof(spaceName)}' cannot be null or whitespace.", nameof(spaceName));
            }

            
            var url = UrlHelper.JoinUrl("space", spaceName, "sharedmemory", "item");
            
            var urlParameters = new NameValueCollection
            {
                { "key", key },
            };
            
            return await apiClient.DeleteAsync<DeleteSharedMemoryResponseDto>(url, urlParameters, 
                apiSession.ToHeadersCollection(), cancellationToken);
        }

        #endregion
    }
}