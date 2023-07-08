using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Native
{
    public static class Utilities
    {
        public static float GetYCenterAt(float x)
        {
            return x * Constants.PlayableAreaInclineFactor;
        }

        public static float Clamp(float val, float minVal, float maxVal)
        {
            if (val < minVal)
            {
                return minVal;
            }
            if (val > maxVal)
            {
                return maxVal;
            }
            else
            {
                return val;
            }
        }
    }
}
