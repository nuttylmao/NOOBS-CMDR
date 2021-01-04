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
        public static List<string> GetProfiles(this OBSWebsocket obs)
        {
            if (!obs.IsConnected)
                return null;

            return obs.ListProfiles();
        }

        public static List<string> GetScenes(this OBSWebsocket obs)
        {
            if (!obs.IsConnected)
                return null;

            return obs.GetSceneList().Scenes.ConvertAll(x => x.Name);
        }

        public static List<string> GetSources(this OBSWebsocket obs, string sceneName = null, bool sourcesOnly = false)
        {
            // Don't return anything if you're not connected to OBS Websockets
            if (!obs.IsConnected)
                return null;

            // Get list of source types that have audio
            List<string> sources = new List<string>();

            if (sceneName != null)
            {
                foreach (OBSScene scene in obs.GetSceneList().Scenes)
                {
                    if (scene.Name == sceneName || string.IsNullOrWhiteSpace(scene.Name))
                    {
                        foreach (SceneItem source in scene.Items)
                        {
                            sources.Add(source.SourceName);
                        }
                    }
                }
            }
            else
            {
                if (!sourcesOnly)
                {
                    // Add scenes
                    foreach (OBSScene scene in obs.GetSceneList().Scenes)
                    {
                        sources.Add(scene.Name);
                    }
                }

                // Add sources
                foreach (OBSScene scene in obs.GetSceneList().Scenes)
                {
                    foreach (SceneItem source in scene.Items)
                    {
                        if (!sources.Contains(source.SourceName) && source.InternalType != "scene")
                            sources.Add(source.SourceName);
                    }
                }

                sources.Sort();

            }

            return sources;
        }

        public static List<string> GetAudioSources(this OBSWebsocket obs)
        {
            // Don't return anything if you're not connected to OBS Websockets
            if (!obs.IsConnected)
                return null;

            // Get list of source types that have audio
            List<string> sourceTypes = obs.GetSourceTypesList().Where(x => x.Capabilities.HasAudio).Select(x => x.TypeID).ToList();

            List<string> sources = new List<string>();

            // Get a list of all sources that have audio
            foreach (OBSScene scene in obs.GetSceneList().Scenes)
            {
                foreach (SceneItem source in scene.Items)
                {
                    if (sourceTypes.Contains(source.InternalType))
                    {
                        if (!sources.Contains(source.SourceName))
                            sources.Add(source.SourceName);
                    }
                }
            }

            // Get global audio sources
            foreach (var specialSource in obs.GetSpecialSources())
            {
                if(!string.IsNullOrWhiteSpace(specialSource.Value) && !sources.Contains(specialSource.Value))
                    sources.Add(specialSource.Value);
            }
            
            sources.Sort();

            return sources;
        }

        public static List<string> GetFilters(this OBSWebsocket obs, string sourceName)
        {
            if (!obs.IsConnected)
                return null;

            List<FilterSettings> filters = new List<FilterSettings>();

            try
            {
                filters = obs.GetSourceFilters(sourceName);
            }
            catch
            {
                return null;
            }

            return filters.Select(x => x.Name).ToList();
        }

        public static List<string> GetTransitions(this OBSWebsocket obs)
        {
            if (!obs.IsConnected)
                return null;

            return obs.GetTransitionList().Transitions.ConvertAll(x => x.Name);
        }


    }
}
