/***************************************************************************
 *                               TestVirtueQuests.cs
 *
 *   begin                : 18 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.Quests;
using Server.Items;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class TestVirtueQuester : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof( CompassionQuestOne ) }; } }

        [Constructable]
        public TestVirtueQuester()
            : base( "Virtue Quester", ", the tester" )
        {
        }

        public override void InitBody()
        {
            InitStats( 100, 100, 25 );

            Hue = 0x8412;
            Female = false;

            HairItemID = 0x203C;
            HairHue = 0x47A;
            FacialHairItemID = 0x204D;
            FacialHairHue = 0x47A;
        }

        public override void InitOutfit()
        {
            AddItem( new Sandals( 0x75E ) );
            AddItem( new Shirt() );
            AddItem( new ShortPants( 0x66C ) );
            AddItem( new SkullCap( 0x649 ) );
            AddItem( new Pitchfork() );
        }

        #region serialization
        public TestVirtueQuester( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}