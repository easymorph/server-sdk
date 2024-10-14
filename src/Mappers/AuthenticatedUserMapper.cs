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
            if (authenticatedUserDto.RealUser != null)
            {
                return MapRealUser(authenticatedUserDto.RealUser);
            }
            else if (authenticatedUserDto.LegacyUser != null)
            {
                return MapLeagacyUser(authenticatedUserDto.LegacyUser);
            }
            else if (authenticatedUserDto.Anonymous != null)
            {
                return MapAnonymous(authenticatedUserDto.Anonymous);
            }
            else throw new NotSupportedException("AuthenticatedUser identity not supported");

            
        }

        private static AuthenticatedUser MapAnonymous(AnonymousAuthenticatedUserDto anonymous)
        {
            return new AnonumousAuthenticatedUser
            {
                AnonymousDueWrongSession = anonymous.AnonymousDueWrongSession
            };
        }

        private static AuthenticatedUser MapLeagacyUser(LegacyAuthenticatedUserDto legacyUser)
        {
            if (legacyUser is null)
            {
                throw new ArgumentNullException(nameof(legacyUser));
            }

            return new LegacyAuthenticatedUser
            {
                SpaceName = legacyUser.SpaceName
            };
        }

        private static AuthenticatedUser MapRealUser(RealAuthenticatedUserDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            return new RealAuthenticatedUser
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                ExternalIdentity = dto.ExternalIdentity,
                FullName = dto.FullName,
                UserId = dto.UserId,
                UserName = dto.UserName

            };
        }
    }
}
