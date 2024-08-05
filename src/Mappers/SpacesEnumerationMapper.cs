using Morph.Server.Sdk.Dto;
using Morph.Server.Sdk.Dto.Errors;
using Morph.Server.Sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Morph.Server.Sdk.Mappers
{
    internal static class SpacesEnumerationMapper
    {
        public static SpacesEnumerationList MapFromDto(SpacesEnumerationDto dto)
        {
            return new SpacesEnumerationList()
            {
                Items = dto.Values?.Select(MapItemFromDto)?.ToList()                
            };
        }

       
        public static SpaceEnumerationItem MapItemFromDto(SpaceEnumerationItemDto dto)
        {
            var spaceAccessRestriction = ParseSpaceAccessRestriction(dto.SpaceAccessRestriction);
            return new SpaceEnumerationItem
            {
                IsPublic = dto.IsPublic,
                SpaceName = dto.SpaceName,
                SpaceAccessRestriction = spaceAccessRestriction,
                SpaceAuthenticationProviderTypes = FillProviders(spaceAccessRestriction, dto.AuthenticationProviders).ToArray()
            };
        }


        private static IEnumerable<IdPType> FillProviders(SpaceAccessRestriction spaceAccessRestriction,
                                                            string[] authenticationProviders)
        {

            // authentication providers can be null for servers before `feature\users` (5.8.0)
            if (authenticationProviders == null)
            {
                switch (spaceAccessRestriction)
                {
                    case SpaceAccessRestriction.None:
                        yield return IdPType.Anonymous;
                        break;
                    case SpaceAccessRestriction.BasicPassword:
                        yield return IdPType.SpacePwd;
                        break;
                    //case SpaceAccessRestriction.WindowsAuthentication:
                    //    yield return IdPType.AdSeamlessIdP;
                    //    break;
                    default:
                        break;

                }
            }
            else 
            {
                foreach (var provider in authenticationProviders)
                {
                    if (IdPTypeMapper.TryParse(provider, out IdPType idPType))
                    {
                        yield return idPType;
                    }
                }
            }
        }

        internal static SpaceAccessRestriction ParseSpaceAccessRestriction(string value)
        {
            SpaceAccessRestriction parsed;
            if (value != null && Enum.TryParse(value, true, out parsed))
            {
                return parsed;
            }
            else
            {
                return SpaceAccessRestriction.NotSupported;
            }
        }
    }

}
