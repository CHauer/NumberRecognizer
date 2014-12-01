using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognizer.Lib.Network
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum HiddenLayerType
    {
        Lining = 1,
        Boxing = 2,
        OverlayedLining = 4,
        OverlayedBoxing = 8,
    }
}
