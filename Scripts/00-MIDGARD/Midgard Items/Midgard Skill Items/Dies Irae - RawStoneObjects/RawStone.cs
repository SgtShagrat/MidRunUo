/***************************************************************************
 *                                  RawStone.cs
 *                            		-------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
	public class RawStone : Item, ICommodity
	{
		public override double DefaultWeight
		{
			get { return 1.0; }
		}

		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} raw stone" : "{0} raw stones", Amount );
			}
		}

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName
        {
            get
            {
                return "a raw stone";
            }
        }
		[Constructable]
		public RawStone() : this( 1 )
		{
		}

		[Constructable]
		public RawStone( int amount ) : base( 0x19B9 )
		{
			Stackable = true;
			Amount = amount;

			Hue = 0x38E;
		}

		public RawStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}