using Morph.Server.Sdk.Dto;
using Morph.Server.Sdk.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Morph.Server.Sdk.Dto.Commands;
using System.Collections.Generic;
using Morph.Server.Sdk.Dto.SharedMemory;
using Morph.Server.Sdk.Model.InternalModels;
using Morph.Server.Sdk.Events;
using Morph.Server.Sdk.Dto.SpaceFilesSearch;
using Morph.Server.Sdk.Model.SharedMemory;
using Morph.Server.Sdk.Dto.Auth;

namespace Morph.Server.Sdk.Client
{
    internal interface ILowLevelApiClient: IDisposable
    {
        IRestClient RestClient { get; }

        // TASKS
        
        Task<ApiResult<TasksListDto>> GetTasksListAsync(ApiSession apiSession, string spaceName, CancellationToken cancellationToken);
        Task<ApiResult<TaskFullDto>> GetTaskAsync(ApiSession apiSession,string spaceName,  Guid taskId, CancellationToken cancellationToken);
        Task<ApiResult<TaskFullDto>> TaskChangeModeAsync(ApiSession apiSession, string spaceName, Guid taskId, SpaceTaskChangeModeRequestDto requestDto, CancellationToken cancellationToken);

        // start/cancel/info running workflow
        
        Task<ApiResult<ComputationDetailedItemDto>> StartTaskAsync(ApiSession apiSession, string spaceName, TaskStartRequestDto taskStartRequestDto, CancellationToken cancellationToken);
        
        Task<ApiResult<ComputationDetailedItemDto>> GetComputationDetailsAsync(ApiSession apiSession, string spaceName, string computationId , CancellationToken cancellationToken);
        Task CancelComputationAsync(ApiSession apiSession, string spaceName, string computationId , CancellationToken cancellationToken);

        Task<ApiResult<WorkflowResultDetailsDto>> GetWorkflowResultDetailsAsync(ApiSession apiSession, string spaceName, string resultToken,
            CancellationToken cancellationToken);
        
        Task AcknowledgeWorkflowResultAsync(ApiSession apiSession, string spaceName, string resultToken, CancellationToken cancellationToken);
        

        // Tasks validation
        Task<ApiResult<ValidateTasksResponseDto>> ValidateTasksAsync(ApiSession apiSession, ValidateTasksRequestDto validateTasksRequestDto, CancellationToken cancellationToken);


        // Auth and sessions
        Task<ApiResult<AuthProvidersDto>> AuthGetProvidersListAsync(CancellationToken cancellationToken);
        Task<ApiResult<NoContentResult>> AuthLogoutAsync(ApiSession apiSession, CancellationToken cancellationToken);
        Task<ApiResult<LoginResponseDto>> AuthLoginPasswordAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken);
        Task<ApiResult<GenerateNonceResponseDto>> AuthGenerateNonce(CancellationToken cancellationToken);
        Task<ApiResult<AuthenticatedUserDto>> GetCurrentAuthenticatedUser(ApiSession apiSession, CancellationToken cancellationToken);


        // Server interaction
        Task<ApiResult<ServerStatusDto>> ServerGetStatusAsync(CancellationToken cancellationToken);


        // spaces

        Task<ApiResult<SpacesEnumerationDto>> SpacesGetListAsync(CancellationToken cancellationToken);
        Task<ApiResult<SpacesLookupResponseDto>> SpacesLookupAsync(SpacesLookupRequestDto requestDto, CancellationToken cancellationToken);
        Task<ApiResult<SpaceStatusDto>> SpacesGetSpaceStatusAsync(ApiSession apiSession, string spaceName, CancellationToken cancellationToken);

        // WEB FILES
        Task<ApiResult<SpaceFilesQuickSearchResponseDto>> WebFilesQuickSearchSpaceAsync(ApiSession apiSession,
            string spaceName,
            SpaceFilesQuickSearchRequestDto request, 
            int? offset, int? limit, CancellationToken cancellationToken);
        Task<ApiResult<SpaceBrowsingResponseDto>> WebFilesBrowseSpaceAsync(ApiSession apiSession, string spaceName, string folderPath, CancellationToken cancellationToken);
        Task<ApiResult<bool>> WebFileExistsAsync(ApiSession apiSession, string spaceName, string serverFilePath, CancellationToken cancellationToken);
        Task<ApiResult<NoContentResult>> WebFilesDeleteFileAsync(ApiSession apiSession, string spaceName, string serverFilePath, CancellationToken cancellationToken);
        Task<ApiResult<NoContentResult>> WebFilesRenameFileAsync(ApiSession apiSession, string spaceName, string parentFolderPath, string oldFileName, string newFileName,
            CancellationToken cancellationToken);

        Task<ApiResult<NoContentResult>> WebFilesDeleteFolderAsync(ApiSession apiSession, string spaceName, string serverFilePath,
            bool failIfNotExists,
            CancellationToken cancellationToken);

        Task<ApiResult<NoContentResult>> WebFilesCreateFolderAsync(ApiSession apiSession, string spaceName, string parentFolderPath,
            string folderName,
            bool failIfExists, CancellationToken cancellationToken);

        Task<ApiResult<NoContentResult>> WebFilesRenameFolderAsync(ApiSession apiSession, string spaceName, string parentFolderPath,
            string oldFolderName, string newFolderName,
            bool failIfExists, CancellationToken cancellationToken);

        Task<ApiResult<FetchFileStreamData>> WebFilesDownloadFileAsync(ApiSession apiSession, string spaceName, string serverFilePath, Action<FileTransferProgressEventArgs> onReceiveProgress, CancellationToken cancellationToken);
        Task<ApiResult<NoContentResult>> WebFilesPutFileStreamAsync(ApiSession apiSession, string spaceName, string serverFolder, SendFileStreamData sendFileStreamData, Action<FileTransferProgressEventArgs> onSendProgress, CancellationToken cancellationToken);
        Task<ApiResult<NoContentResult>> WebFilesPostFileStreamAsync(ApiSession apiSession, string spaceName, string serverFolder, SendFileStreamData sendFileStreamData, Action<FileTransferProgressEventArgs> onSendProgress, CancellationToken cancellationToken);

        Task<ApiResult<NoContentResult>> WebFilesPushPutFileStreamAsync(ApiSession apiSession,
            string spaceName,
            string serverFolder,
            PushFileStreamData pushFileStreamData, CancellationToken cancellationToken);
        Task<ApiResult<NoContentResult>> WebFilesPushPostFileStreamAsync(ApiSession apiSession,
            string spaceName,
            string serverFolder,
            PushFileStreamData pushFileStreamData, CancellationToken cancellationToken);

        [Obsolete("Obsolete due to flaw in response checking. Use WebFilesPushPostFileStreamAsync instead. Will be removed in next major version.")]
        Task<ApiResult<ServerPushStreaming>> WebFilesOpenContiniousPostStreamAsync(ApiSession apiSession, string spaceName, string serverFolder, string fileName, CancellationToken cancellationToken);
        
        [Obsolete("Obsolete due to flaw in response checking. Use WebFilesPushPutFileStreamAsync instead. Will be removed in next major version.")]
        Task<ApiResult<ServerPushStreaming>> WebFilesOpenContiniousPutStreamAsync(ApiSession apiSession, string spaceName, string serverFolder, string fileName, CancellationToken cancellationToken);

        Task<ApiResult<SharedMemoryValueDto>> SharedMemoryRemember(ApiSession apiSession, string spaceName, string key,
            SharedMemoryValueDto value, OverwriteBehavior overwriteBehavior, CancellationToken cancellationToken);
        Task<ApiResult<SharedMemoryValueDto>> SharedMemoryRecall(ApiSession apiSession, string spaceName, string key, CancellationToken cancellationToken);
        Task<ApiResult<SharedMemoryListResponseDto>> SharedMemoryList(ApiSession apiSession, string spaceName, string startsWith, int offset, int limit, CancellationToken cancellationToken);
        Task<ApiResult<DeleteSharedMemoryResponseDto>> SharedMemoryForget(ApiSession apiSession, string spaceName, string key,
            CancellationToken cancellationToken);
    }
}