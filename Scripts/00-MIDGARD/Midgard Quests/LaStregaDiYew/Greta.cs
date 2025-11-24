//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 15/02/2009 23.16.20
//=================================================
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class Greta : MondainQuester
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();

        [Constructable]
        public Greta()
            : base( "Greta", "La Strega" )
        {
            InitBody();
        }

        public Greta( Serial serial )
            : base( serial )
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[]{
                                 typeof ( LaStregadiYew )
                                 };
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        public override void InitBody()
        {
            InitStats( 100, 100, 100 );
            Female = true;
            Race = Race.Human;
            base.InitBody();
        }

        public override void InitOutfit()
        {
            TallStrawHat head = new TallStrawHat();
            head.Hue = 1709;
            AddItem( head );
            Robe chest = new Robe();
            chest.Hue = 1709;
            AddItem( chest );
            AddItem( new GnarledStaff() );
            Sandals feet = new Sandals();
            feet.Hue = 1709;
            AddItem( feet );
        }
    }
}