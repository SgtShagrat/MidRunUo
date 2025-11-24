/***************************************************************************
 *                               DruidReagents.cs
 *
 *   begin                : 10 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class DestroyingAngel : BaseReagent, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( "{0} destroying angel", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        public override string DefaultName
        {
            get { return "destroying angel"; }
        }

        [Constructable]
        public DestroyingAngel()
            : this( 1 )
        {
        }

        [Constructable]
        public DestroyingAngel( int amount )
            : base( 0xE1F, amount )
        {
            Hue = 0x290;
        }

        public DestroyingAngel( Serial serial )
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
    }

    public class PetrifiedWood : BaseReagent, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( "{0} petrified wood", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        public override string DefaultName
        {
            get { return "petrified wood"; }
        }

        [Constructable]
        public PetrifiedWood()
            : this( 1 )
        {
        }

        [Constructable]
        public PetrifiedWood( int amount )
            : base( 0x97A, amount )
        {
            Hue = 0x46C;
        }

        public PetrifiedWood( Serial serial )
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
    }

    public class SpringWater : BaseReagent, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( "{0} spring water", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        public override string DefaultName
        {
            get { return "spring water"; }
        }

        [Constructable]
        public SpringWater()
            : this( 1 )
        {
        }

        [Constructable]
        public SpringWater( int amount )
            : base( 0xE24, amount )
        {
            Hue = 0x47F;
        }

        public SpringWater( Serial serial )
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
    }
}