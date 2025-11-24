using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{	
	public class Regina : MondainQuester
	{
		public override Type[] Quests{ get{ return null; } }
		
		[Constructable]
		public Regina() : base( "Regina", "the noble" )
		{			
		}
		
		public Regina( Serial serial ) : base( serial )
		{
		}
		
		public override void InitBody()
		{
			InitStats( 100, 100, 25 );
			
			Female = true;
			Race = Race.Human;
			
			Hue = 0x83EE;
			HairItemID = 0x2049;
			HairHue = 0x599;
		}

	    #region mod by Dies Irae
        /*
	    public override void InitOutfit()
	    {
	        AddItem( new Backpack() );
	        AddItem( new Boots() );
	        AddItem( new GildedDress() );
	    }
        */

        public override void InitOutfit()
        {
            if( Female )
                AddItem( new FancyDress() );
            else
                AddItem( new FancyShirt( GetRandomHue() ) );

            int lowHue = GetRandomHue();

            AddItem( new ShortPants( lowHue ) );

            if( Female )
                AddItem( new ThighBoots( lowHue ) );
            else
                AddItem( new Boots( lowHue ) );

            if( !Female )
                AddItem( new BodySash( lowHue ) );

            AddItem( new Cloak( GetRandomHue() ) );

            if( !Female )
                AddItem( new Longsword() );

            Utility.AssignRandomHair( this );

            PackGold( 200, 250 );
        }
        #endregion
		
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