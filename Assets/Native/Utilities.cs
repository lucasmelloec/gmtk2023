﻿using System;
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
    }
}
