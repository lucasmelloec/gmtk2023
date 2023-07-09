using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Native
{
    public static class Constants
    {
        public static float PlayableAreaInclineFactor = 0.5f;
        public static float PlayableAreaHeight = 40;

        public static float ChunkWidth = 50;
        public static float ChunkMaxY = 20;
        public static float ChunkMinY = -20;

        public static float PlatformWidth = 10;
        public static float PlatformNoOverlapWidth = 12;
        public static float PlatformNoOverlapHeight = 12;

        public static int CloudCountPerChunk = 60;
        public static float CloudPlayableAreaDistance = 10;
    }
}
