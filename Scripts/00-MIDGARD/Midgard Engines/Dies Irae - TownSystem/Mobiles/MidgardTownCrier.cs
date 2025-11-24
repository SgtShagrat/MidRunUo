/***************************************************************************
 *                                  CitizenNotary.cs
 *                            		--------------
 *  begin                	: Aprile, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    [CorpseName( "a crier corpse" )]
    public class MidgardTownCrier : TownCrier
    {
        private MidgardTowns m_Town;

        [CommandProperty( AccessLevel.GameMaster )]
        public MidgardTowns Town
        {
            get { return m_Town; }
            set
            {
                MidgardTowns oldValue = m_Town;

                if( oldValue != value )
                {
                    m_Town = value;
                    OnTownChanged( oldValue );
                }
            }
        }

        public TownSystem System
        {
            get { return TownSystem.Find( m_Town ); }
        }

        public void OnTownChanged( MidgardTowns oldValue )
        {
            InvalidateProperties();

            if( m_Town != MidgardTowns.None )
            {
                if( System != null )
                {
                    Say( true, string.Format( "Yessir! Now I'm the town crier of {0}", System.Definition.TownName ) );

                    System.RegisterTownCrier( this );
                }
            }
        }

        public override TownCrierEntry GetRandomEntry()
        {
            if( System != null && !System.IsEmptyNews )
                return System.GetRandomEntry();

            return base.GetRandomEntry();
        }

        public override void OnDelete()
        {
            if( System != null )
                System.UnRegisterTownCrier( this );

            base.OnDelete();
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.AccessLevel >= AccessLevel.GameMaster )
                from.SendMessage( "To edit its town news, use townstone instead." );
            else
                base.OnDoubleClick( from );
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( System != null )
                list.Add( "Town Crier of {0}", System.Definition.TownName );
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        [Constructable]
        public MidgardTownCrier()
            : this( MidgardTowns.None )
        {
        }

        [Constructable]
        public MidgardTownCrier( MidgardTowns town )
        {
            m_Town = town;

            InitStats( 100, 100, 25 );

            Hue = Utility.RandomSkinHue();

            Female = Utility.RandomBool();

            if( Female )
            {
                Body = 0x191;
                Name = NameList.RandomName( "female" );
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );
            }

            AddItem( new FancyShirt( Utility.RandomBlueHue() ) );

            Item skirt;

            switch( Utility.Random( 2 ) )
            {
                case 0:
                    skirt = new Skirt();
                    break;
                case 1:
                    skirt = new Kilt();
                    break;
                default:
                    skirt = new Kilt();
                    break;
            }

            skirt.Hue = Utility.RandomGreenHue();

            AddItem( skirt );

            AddItem( new FeatheredHat( Utility.RandomGreenHue() ) );

            Item boots;

            switch( Utility.Random( 2 ) )
            {
                case 0:
                    boots = new Boots();
                    break;
                default:
                case 1:
                    boots = new ThighBoots();
                    break;
            }

            AddItem( boots );

            Utility.AssignRandomHair( this );
        }

        public MidgardTownCrier( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( (int)m_Town );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Town = (MidgardTowns)reader.ReadInt();
        }
    }
}