/***************************************************************************
 *                               BaseHuman.cs
 *
 *   begin                : 21 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    public abstract class BaseHuman : BaseCreature
    {
        public BaseHuman( bool isFemale, bool mounted )
            : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4 )
        {
            GenerateBody( isFemale, true );

            SpeechHue = Utility.RandomDyedHue();

            SetStr( 90, 100 );
            SetDex( 90, 100 );
            SetInt( 15, 25 );

            SetHits( 480, 530 );
            SetMana( 15, 35 );
            SetStam( 150, 165 );

            SetDamage( 55, 65 );
            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 0 );
            SetResistance( ResistanceType.Fire, 0 );
            SetResistance( ResistanceType.Cold, 0 );
            SetResistance( ResistanceType.Poison, 0 );
            SetResistance( ResistanceType.Energy, 0 );

            Fame = 0;
            Karma = 0;

            if( mounted )
                AddItem( Immovable( Rehued( new VirtualMountItem( this ), 0 ) ) );

            if( Backpack == null )
                AddItem( Immovable( new Backpack() ) );

            DeleteOnDeath = false;
            HumanBardImmune = true;
            HumanInnocent = true;
            HumanMurderer = false;
        }

        public BaseHuman( Serial serial )
            : base( serial )
        {
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool DeleteOnDeath { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HumanMurderer { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public override bool AlwaysMurderer
        {
            get { return HumanMurderer; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HumanInnocent { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public override bool InitialInnocent
        {
            get { return HumanInnocent; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HumanBardImmune { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public override bool BardImmune
        {
            get { return HumanBardImmune; }
        }

        public virtual void GenerateBody( bool isFemale, bool randomHair )
        {
            Hue = Utility.RandomSkinHue();

            if( isFemale )
            {
                Female = true;
                Body = 401;
                Name = NameList.RandomName( "female" );
            }
            else
            {
                Female = false;
                Body = 400;
                Name = NameList.RandomName( "male" );
            }

            if( randomHair )
                GenerateRandomHair();
        }

        public virtual void GenerateRandomHair()
        {
            Utility.AssignRandomHair( this );
        }

        #region Item equipping
        public Item Immovable( Item item )
        {
            item.Movable = false;
            return item;
        }

        public Item Newbied( Item item )
        {
            item.LootType = LootType.Newbied;
            return item;
        }

        public Item Rehued( Item item, int hue )
        {
            item.Hue = hue;
            return item;
        }

        public Item Layered( Item item, Layer layer )
        {
            item.Layer = layer;
            return item;
        }

        public Item Resourced( BaseWeapon weapon, CraftResource resource )
        {
            weapon.Resource = resource;
            return weapon;
        }

        public Item Resourced( BaseArmor armor, CraftResource resource )
        {
            armor.Resource = resource;
            return armor;
        }
        #endregion

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if( DeleteOnDeath )
                c.Delete();
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version

            writer.Write( DeleteOnDeath );
            writer.Write( HumanBardImmune );
            writer.Write( HumanInnocent );
            writer.Write( HumanMurderer );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            DeleteOnDeath = reader.ReadBool();
            HumanBardImmune = reader.ReadBool();
            HumanInnocent = reader.ReadBool();
            HumanMurderer = reader.ReadBool();
        }
        #endregion

        public class VirtualMount : IMount
        {
            private VirtualMountItem m_Item;

            public VirtualMount( VirtualMountItem item )
            {
                m_Item = item;
            }

            #region IMount Members
            public Mobile Rider
            {
                get { return m_Item.Rider; }
                set { }
            }

            public virtual void OnRiderDamaged( int amount, Mobile from, bool willKill )
            {
            }
            #endregion
        }

        public class VirtualMountItem : Item, IMountItem
        {
            private VirtualMount m_Mount;
            private Mobile m_Rider;

            public VirtualMountItem( Mobile mob )
                : base( 0x3EA0 )
            {
                Layer = Layer.Mount;

                m_Rider = mob;
                m_Mount = new VirtualMount( this );
            }

            public VirtualMountItem( Serial serial )
                : base( serial )
            {
                m_Mount = new VirtualMount( this );
            }

            public Mobile Rider
            {
                get { return m_Rider; }
            }

            #region IMountItem Members
            public IMount Mount
            {
                get { return m_Mount; }
            }
            #endregion

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( 0 ); // version

                writer.Write( m_Rider );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();

                m_Rider = reader.ReadMobile();

                if( m_Rider == null )
                    Delete();
            }
        }
    }
}