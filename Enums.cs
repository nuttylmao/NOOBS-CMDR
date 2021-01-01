using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR
{
    class Enums
    {
        public static List<string> AudioSourceTypes
        {
            get
            {
                return
                    new List<String>
                        {
                            "wasapi_output_capture",                // Audio Output
                            "wasapi_input_capture",                 // Audio Input
                            "ffmpeg_source",                        // Media Source
                            "browser_source",                       // Browser Source
                            "dshow_input",                          // Video Capture Device
                            "obs-stream-effects-source-mirror"      // Source Mirror
                        };
            }

        }

    }
}
