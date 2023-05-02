using Morph.Server.Sdk.Dto;
using Morph.Server.Sdk.Dto.SpaceFilesSearch;
using Morph.Server.Sdk.Model;
using System;
using System.Linq;

namespace Morph.Server.Sdk.Mappers
{
    internal static class FilesQuickSearchResponseMapper
    {
        public static FilesQuickSearchResponse MapFromDto(FilesQuickSearchResponseDto dto)
        {
            return new FilesQuickSearchResponse
            {
                HasMore = dto.HasMore,
                Values = dto.Values.Select(Map).ToArray()
            };
        }

        private static FoundSpaceFolderItem Map(FoundSpaceFolderItemDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            return new FoundSpaceFolderItem
            {
                Files = dto.Files.Select(MapFile).ToArray(),
                Name = dto.Name,
                LastModified = dto.LastModified,
                Path = dto.Path
            };
        }

        private static FoundSpaceFileItem MapFile(FoundSpaceFileItemDto dto)
        {
            return new FoundSpaceFileItem
            {
                Extension = dto.Extension,
                FileSizeBytes = dto.FileSizeBytes,
                Highlights = dto.Highlights,
                LastModified = dto.LastModified,
                Name = dto.Name
            };
        }
    }
}
