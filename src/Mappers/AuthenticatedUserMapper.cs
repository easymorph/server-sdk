using Morph.Server.Sdk.Dto;
using Morph.Server.Sdk.Dto.Auth;
using Morph.Server.Sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Mappers
{

    internal static class AuthenticatedUserMaper
    {
        public static AuthenticatedUser Map(AuthenticatedUserDto authenticatedUserDto)
        {
            if (authenticatedUserDto is null)
            {
                throw new ArgumentNullException(nameof(authenticatedUserDto));
            }
            return new AuthenticatedUser
            {
                DisplayName = authenticatedUserDto.DisplayName,
                Email = authenticatedUserDto.Email,
                ExternalIdentity = authenticatedUserDto.ExternalIdentity,
                FullName = authenticatedUserDto.FullName,
                UserId = authenticatedUserDto.UserId,
                UserName = authenticatedUserDto.UserName

            };
        }
    }
}
