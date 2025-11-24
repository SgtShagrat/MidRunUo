// #define verify

using System;
using Midgard;
using Server.Engines.Craft;

namespace Server.Items
{
	public class RunicFletcherKit : BaseRunicTool
	{
		public override CraftSystem CraftSystem{ get{ return DefBowFletching.CraftSystem; } }

		public override void AddNameProperty( ObjectPropertyList list )
		{
			string v = "";

			if ( !CraftResources.IsStandard( Resource ) )
				v = CraftResources.GetName( Resource );

			list.Add( String.Format( "{0} Runic Fletcher's Tools", v ) );
		}

		[Constructable]
		public RunicFletcherKit() : base( CraftHelper.GetRandomRunicWood(), 0x1022 )
		{
            UsesRemaining = CraftHelper.GetRandomRunicWoodCharges( Resource, 1 );
			Weight = 2.0;
			Hue = CraftResources.GetHue( Resource );
		}

		[Constructable]
		public RunicFletcherKit( CraftResource resource ) : base( resource, 0x1022 )
		{
			Weight = 2.0;
			Hue = CraftResources.GetHue( resource );
		}

		[Constructable]
		public RunicFletcherKit( CraftResource resource, int uses ) : base( resource, uses, 0x1022 )
		{
			Weight = 2.0;
			Hue = CraftResources.GetHue( resource );
		}

		public RunicFletcherKit( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
#if verify
			Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Midgard.CraftHelper.VerifyFletcherTools_Callback ), this );
#endif
		}
	}
}
