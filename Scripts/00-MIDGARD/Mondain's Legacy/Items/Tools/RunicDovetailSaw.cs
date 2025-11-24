using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class RunicDovetailSaw : BaseRunicTool
	{
		public override CraftSystem CraftSystem{ get{ return DefCarpentry.CraftSystem; } }

		#region mod by Dies Irae
		/*
		public override int LabelNumber
		{
			get
			{
				int index = CraftResources.GetIndex( Resource );

				if ( index >= 1 && index <= 6 )
					return 1072633 + index;
					
				return 1024137; // dovetail saw
			}
		}
		*/

		public override void AddNameProperty( ObjectPropertyList list )
		{
			string v = "";

			if ( !CraftResources.IsStandard( Resource ) )
				v = CraftResources.GetName( Resource );

			list.Add( String.Format( "{0} Runic Dovetail Saw", v ) );
		}
		#endregion
		
		[Constructable]
		public RunicDovetailSaw( CraftResource resource ) : base( resource, 0x1028 )
		{
			Weight = 2.0;
			Hue = CraftResources.GetHue( resource );
		}

		[Constructable]
		public RunicDovetailSaw( CraftResource resource, int uses ) : base( resource, uses, 0x1028 )
		{
			Weight = 2.0;
			Hue = CraftResources.GetHue( resource );
		}

		public RunicDovetailSaw( Serial serial ) : base( serial )
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
		}
	}
}