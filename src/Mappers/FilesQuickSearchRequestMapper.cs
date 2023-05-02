using Morph.Server.Sdk.Dto.SpaceFilesSearch;
using Morph.Server.Sdk.Model;
using System;

namespace Morph.Server.Sdk.Mappers
{
    internal static class FilesQuickSearchRequestMapper
    {
        internal static FilesQuickSearchRequestDto ToDto(FilesQuickSearchRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new FilesQuickSearchRequestDto
            {
                FileExtensions = request.FileExtensions?.ToArray() ?? Array.Empty<string>(),
                FolderPath = request.FolderPath,
                LookupString = request.LookupString
            };
        }

    }
}
