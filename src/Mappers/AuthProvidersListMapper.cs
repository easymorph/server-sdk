using Morph.Server.Sdk.Dto.Auth;
using Morph.Server.Sdk.Model;
using System.Linq;

namespace Morph.Server.Sdk.Mappers
{
    internal static class AuthProvidersListMapper
    {
        public static AuthProvidersList MapFromDto(AuthProvidersDto dto)
        {
            return new AuthProvidersList()
            {
                Items = dto.Values?.Select(AuthProviderMapper.MapItem)?.ToList()
            };
        }
    }
}
