/***************************************************************************
 *                              CustomReagents.cs
 *                            ---------------------
 *   begin                : 30 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

namespace Server.Items
{
    public class VolcaninAsh : BaseReagent, ICommodity
    {
        string ICommodity.Description { get { return String.Format( "{0} Volcanin Ash", Amount ); } }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Volcanin Ash"; } }

        [Constructable]
        public VolcaninAsh()
            : this( 1 )
        {
        }

        [Constructable]
        public VolcaninAsh( int amount )
            : base( 0xF7F, amount )
        {
        }

        public VolcaninAsh( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class SerpentScale : BaseReagent, ICommodity
    {
        string ICommodity.Description { get { return String.Format( "{0} Serpent Scale", Amount ); } }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Serpent Scale"; } }

        [Constructable]
        public SerpentScale()
            : this( 1 )
        {
        }

        [Constructable]
        public SerpentScale( int amount )
            : base( 0x26B6, amount )
        {
        }

        public SerpentScale( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class BlackMoor : BaseReagent, ICommodity
    {
        string ICommodity.Description { get { return String.Format( "{0} Black Moor", Amount ); } }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Black Moor"; } }

        [Constructable]
        public BlackMoor()
            : this( 1 )
        {
        }

        [Constructable]
        public BlackMoor( int amount )
            : base( 0xF88, amount )
        {
        }

        public BlackMoor( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class DragonBlood : BaseReagent, ICommodity
    {
        string ICommodity.Description { get { return String.Format( "{0} Dragon Blood", Amount ); } }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Dragon Blood"; } }

        [Constructable]
        public DragonBlood()
            : this( 1 )
        {
        }

        [Constructable]
        public DragonBlood( int amount )
            : base( 0xF82, amount )
        {
        }

        public DragonBlood( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class WyrmHeart : BaseReagent, ICommodity
    {
        string ICommodity.Description { get { return String.Format( "{0} Wyrm Heart", Amount ); } }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Wyrm Heart"; } }

        [Constructable]
        public WyrmHeart()
            : this( 1 )
        {
        }

        [Constructable]
        public WyrmHeart( int amount )
            : base( 0xF91, amount )
        {
        }

        public WyrmHeart( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class Pumices : BaseReagent, ICommodity
    {
        string ICommodity.Description { get { return String.Format( "{0} Pumices", Amount ); } }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Pumices"; } }

        [Constructable]
        public Pumices()
            : this( 1 )
        {
        }

        [Constructable]
        public Pumices( int amount )
            : base( 0xF8B, amount )
        {
        }

        public Pumices( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class Obsidian : BaseReagent, ICommodity
    {
        string ICommodity.Description { get { return String.Format( "{0} Obsidian", Amount ); } }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Obsidian"; } }

        [Constructable]
        public Obsidian()
            : this( 1 )
        {
        }

        [Constructable]
        public Obsidian( int amount )
            : base( 0xF89, amount )
        {
        }

        public Obsidian( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class ExecutionerCap : BaseReagent, ICommodity
    {
        string ICommodity.Description { get { return String.Format( "{0} Executioner Cap", Amount ); } }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Executioner Cap"; } }

        [Constructable]
        public ExecutionerCap()
            : this( 1 )
        {
        }

        [Constructable]
        public ExecutionerCap( int amount )
            : base( 0xF83, amount )
        {
        }

        public ExecutionerCap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class EyeOfNewt : BaseReagent, ICommodity
    {
        string ICommodity.Description { get { return String.Format( "{0} Eyes Of Newt", Amount ); } }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Eye Of Newt"; } }

        [Constructable]
        public EyeOfNewt()
            : this( 1 )
        {
        }

        [Constructable]
        public EyeOfNewt( int amount )
            : base( 0xF87, amount )
        {
        }

        public EyeOfNewt( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class BloodSpawn : BaseReagent, ICommodity
    {
        string ICommodity.Description { get { return String.Format( "{0} Blood Spawns", Amount ); } }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Blood Spawn"; } }

        [Constructable]
        public BloodSpawn()
            : this( 1 )
        {
        }

        [Constructable]
        public BloodSpawn( int amount )
            : base( 0xF7C, amount )
        {
        }

        public BloodSpawn( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}