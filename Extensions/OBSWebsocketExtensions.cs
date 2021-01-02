using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Extensions
{
    public static class OBSWebsocketExtensions
    {

        public static List<string> GetScenes(this OBSWebsocket obs)
        {
            if (!obs.IsConnected)
            {
                return null;
            }

            return obs.GetSceneList().Scenes.ConvertAll(x => x.Name);
        }

        public static List<string> GetAudioSources(this OBSWebsocket obs)
        {
            // Don't return anything if you're not connected to OBS Websockets
            if (!obs.IsConnected)
            {
                return null;
            }

            // Get list of source types that have audio
            List<string> sourceTypes = obs.GetSourceTypesList().Where(x => x.Capabilities.HasAudio).Select(x => x.TypeID).ToList();

            List<string> sources = new List<string>();

            // Get a list of all sources that have audio
            foreach (OBSScene scene in obs.GetSceneList().Scenes)
            {
                foreach (SceneItem source in scene.Items)
                {
                    Console.WriteLine(source.InternalType);
                    if (sourceTypes.Contains(source.InternalType))
                    {
                        if (!sources.Contains(source.SourceName))
                            sources.Add(source.SourceName);
                    }
                }
            }

            sources.Sort();

            return sources;
        }

    }
}
