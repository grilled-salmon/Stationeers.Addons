// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    public sealed class UnityWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes();
            whitelist.WhitelistTypesNamespaces( // Each one of this types comes from different module, so we have to add all of them
                typeof(UnityEngine.Debug),
                typeof(UnityEngine.AssetBundle),
                typeof(UnityEngine.GUILayout),
                typeof(UnityEngine.UI.Button),
                typeof(UnityEngine.ParticleSystem),
                typeof(UnityEngine.Physics),
                typeof(UnityEngine.StreamingController),
                typeof(UnityEngine.TextAlignment),
                typeof(UnityEngine.TextCore.Glyph),
                typeof(UnityEngine.TextCore.LowLevel.FontEngine),
                typeof(UnityEngine.Input),
                typeof(UnityEngine.Rendering.AmbientMode),
                typeof(UnityEngine.Video.VideoClip),
                typeof(UnityEngine.JsonUtility),
                typeof(UnityEngine.Networking.Utility),
                typeof(UnityEngine.Assertions.Assert),
                typeof(UnityEngine.EventSystems.BaseInput)
            );
            whitelist.BlacklistTypes(
                //typeof(UnityEngine.Networking.UnityWebRequest) // DLL not included in compilation, we can omit this.
            );
        }
    }
}