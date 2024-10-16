﻿namespace Morph.Server.Sdk.Model
{
    public class SpaceEnumerationItem
    {
        public string SpaceName { get; internal set; }
        public bool IsPublic { get; internal set; }
        public IdPType[] SpaceAuthenticationProviderTypes { get; internal set; }
    }

}
