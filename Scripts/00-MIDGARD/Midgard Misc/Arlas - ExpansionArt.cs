using System;
using Server;
using Server.Network;

namespace Midgard.Misc
{
    public class ExpansionArt
    {
          public static void Initialize() {
          Server.Network.SupportedFeatures.Value = FeatureFlags.ExpansionSA;

        }
    }
}