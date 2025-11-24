using System;
using Server.Network;

namespace Server
{
	public class CurrentExpansion
	{
		private static Expansion Expansion = Midgard.Misc.Midgard2Persistance.Expansion; // mod by Dies Irae

		public static void Configure()
		{
			Core.Expansion = Expansion;

			bool Enabled = Core.AOS;

			Mobile.InsuranceEnabled = Enabled;
			ObjectPropertyList.Enabled = Enabled;
			Mobile.VisibleDamageType = Core.Debug ? VisibleDamageType.Everyone : VisibleDamageType.None;
			Mobile.GuildClickMessage = !Enabled;
			Mobile.AsciiClickMessage = !Enabled;

		    Mobile.DisableHiddenSelfClick = false; // mod by Dies Irae

			if ( Enabled )
			{
				AOS.DisableStatInfluences();

				if ( ObjectPropertyList.Enabled )
					PacketHandlers.SingleClickProps = true; // single click for everything is overriden to check object property list
			}
		}
	}
}
