/***************************************************************************
 *                                  MidgardWarHorse.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Cavallo bardabile con un HorseBardingDeed.
 * 			Conserva anche il colore, l'itemid e il bodyvalue iniziali 
 * 			quando la bardatura viene rimossa.
 * 
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Mobiles;
using Midgard.Items;
using Server.Network;

namespace Midgard.Mobiles
{
    public class MidgardWarHorse : BaseWarHorse
    {
        public static readonly int BardedHorseItemID = 0x3E92;
        public static readonly int BardedHorseBodyValue = 0x11C;

        private static int[] m_IDs = new int[]
        {
            0xC8, 0x3E9F,
            0xE2, 0x3EA0,
            0xE4, 0x3EA1,
            0xCC, 0x3EA2
        };

        private bool m_HasBarding;
        private CraftResource m_BardingResource;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public Mobile BardingCrafter { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool BardingExceptional { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int BardingHP { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int BardingMaxHP { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool HasBarding
        {
            get { return m_HasBarding; }
            set
            {
                m_HasBarding = value;

                if( m_HasBarding )
                {
                    OriginalItemID = ItemID;
                    OriginalBodyValue = BodyValue;

                    Hue = CraftResources.GetHue( m_BardingResource );

                    BodyValue = BardedHorseBodyValue;
                    ItemID = BardedHorseItemID;
                }
                else
                {
                    Hue = OriginalHue;

                    ItemID = OriginalItemID;
                    BodyValue = OriginalBodyValue;
                }

                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public CraftResource BardingResource
        {
            get { return m_BardingResource; }
            set
            {
                m_BardingResource = value;

                if( m_HasBarding )
                    Hue = CraftResources.GetHue( value );
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int OriginalItemID { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int OriginalBodyValue { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int OriginalHue { get; set; }

        [Constructable]
        public MidgardWarHorse()
            : this( "a war horse" )
        {
        }

        [Constructable]
        public MidgardWarHorse( string name )
            : base( 0xE2, 0x3EA0, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            Name = name;

            int random = Utility.Random( 4 );

            Body = m_IDs[ random * 2 ];
            ItemID = m_IDs[ random * 2 + 1 ];

            OriginalItemID = ItemID;
            OriginalBodyValue = BodyValue;
            OriginalHue = Hue;
        }

        public MidgardWarHorse( Serial serial )
            : base( serial )
        {
        }

        public override bool CheckValidVirtue( Mobile from )
        {
            if( !HorseBardingDeed.IsOrderOrChaosMobile( from ) )
            {
                from.SendMessage( "Thou cannot ride this proud mount." );
                return false;
            }

            return base.CheckValidVirtue( from );
        }

        public override double GetControlChance( Mobile m, bool useBaseSkill )
        {
            return 1.0;
        }

        public override bool ReacquireOnMovement
        {
            get { return true; }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( m_HasBarding && BardingExceptional && BardingCrafter != null )
                list.Add( 1060853, BardingCrafter.Name ); // armor exceptionally crafted by ~1_val~
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            if( m_HasBarding && BardingExceptional && BardingCrafter != null )
                PrivateOverheadMessage( MessageType.Regular, 0x3B2, true, String.Format( "armor exceptionally crafted by {0}", BardingCrafter.Name ), from.NetState );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version

            writer.Write( BardingMaxHP );
            writer.Write( BardingExceptional );
            writer.Write( BardingCrafter );
            writer.Write( m_HasBarding );
            writer.Write( BardingHP );
            writer.Write( (int)m_BardingResource );

            writer.Write( OriginalItemID );
            writer.Write( OriginalBodyValue );
            writer.Write( OriginalHue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        BardingMaxHP = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        BardingExceptional = reader.ReadBool();
                        BardingCrafter = reader.ReadMobile();
                        m_HasBarding = reader.ReadBool();
                        BardingHP = reader.ReadInt();
                        m_BardingResource = (CraftResource)reader.ReadInt();

                        OriginalItemID = reader.ReadInt();
                        OriginalBodyValue = reader.ReadInt();
                        OriginalHue = reader.ReadInt();

                        break;
                    }
            }
        }
    }
}