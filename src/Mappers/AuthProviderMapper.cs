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

    internal static class IdPTypeMapper
    {
        public static bool TryParse(string input, out IdPType idPType)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var type = input.ToLowerInvariant();
            switch (type)
            {
                case "anonymous":
                    idPType = IdPType.Anonymous;
                    return true;
                case "spacepwd":
                    idPType = IdPType.SpacePwd;
                    return true;
                case "internalidp":
                    idPType = IdPType.InternalIdP;
                    return true;
                case "adseamlessidp":
                    idPType = IdPType.AdSeamlessIdP;
                    return true;
                case "rescuelogin":
                    idPType = IdPType.RescueLogin;
                    return true;
                case "openid":
                    idPType = IdPType.OpenId;
                    return true;
                default:
                    idPType = IdPType.Unknown;
                    return false;
             }
            
        }
    }


    internal static class AuthProviderMapper
    {   

        public static AuthProvider MapItem(AuthProviderDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            return new AuthProvider
            {
               CanKeepLongSession = dto.CanKeepLongSession,
               DisplayName = dto.DisplayName,               
               IdentityProvider = Map(dto.IdPId, dto.IdPType)
            };
        }

        private static IdentityProviderBase Map(string idPId, string idPType)
        {

            if(IdPTypeMapper.TryParse(idPType, out var parsedIdPType))
            {
                switch (parsedIdPType)
                {
                    case IdPType.Anonymous:
                        return new AnonymousIdP(idPId);
                    case IdPType.SpacePwd:
                        return new SpacePwdIdP(idPId);
                    case IdPType.InternalIdP:
                        return new InternalIdP(idPId);
                    case IdPType.AdSeamlessIdP:
                        return new AdSeamlessIdP(idPId);
                    case IdPType.RescueLogin:
                        return new RescueLoginIdP(idPId);
                    case IdPType.OpenId:
                        return new OpenIdP(idPId);
                    default:
                        return new UnknownIdP(idPId, idPType);

                }
            }
            else
            {
                return new UnknownIdP(idPId, idPType);
            }
          
        }



    }
}
