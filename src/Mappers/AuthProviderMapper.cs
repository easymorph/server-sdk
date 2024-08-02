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

        public static IdentityProviderBase MapItem(AuthProviderDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            return Map(dto.DisplayName, dto.IdPId, dto.IdPType, dto.CanKeepLongSession);            
        }

        private static IdentityProviderBase Map(string dispayName, string idPId, string idPType, bool canKeepLongSession)
        {

            if(IdPTypeMapper.TryParse(idPType, out var parsedIdPType))
            {
                switch (parsedIdPType)
                {
                    case IdPType.Anonymous:
                        return new AnonymousIdP(dispayName, idPId);
                    case IdPType.SpacePwd:
                        return new SpacePwdIdP(dispayName, idPId);
                    case IdPType.InternalIdP:
                        return new InternalIdP(dispayName, idPId, canKeepLongSession);
                    case IdPType.AdSeamlessIdP:
                        return new AdSeamlessIdP(dispayName, idPId, canKeepLongSession);
                    case IdPType.RescueLogin:
                        return new RescueLoginIdP(dispayName, idPId);
                    case IdPType.OpenId:
                        return new OpenIdP(dispayName, idPId, canKeepLongSession);
                    default:
                        return new UnknownIdP(dispayName, idPId, idPType);

                }
            }
            else
            {
                return new UnknownIdP("<UNKNONW>", idPId, idPType);
            }
          
        }



    }
}
