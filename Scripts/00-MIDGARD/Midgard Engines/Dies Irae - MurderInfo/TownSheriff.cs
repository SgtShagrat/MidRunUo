/***************************************************************************
 *                                  TownSheriff.cs
 *                            		--------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System.Collections.Generic;
using Midgard.Items;
using Server;
using Server.Items;
using Server.Mobiles;
using Midgard.Engines.MidgardTownSystem;

namespace Midgard.Engines.MurderInfo
{
    [CorpseName( "a sheriff corpse" )]
    public class TownSheriff : BaseCreature
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
                    InvalidateProperties();
                }
            }
        }

        public TownSystem System
        {
            get { return TownSystem.Find( m_Town ); }
        }

        private static readonly string MurderString = "Sheriff, I have a murderer to report!";
        private static readonly string HandcuffsString = "Sheriff, give me my handcuffs!";
        // private static readonly string ShowMurderString = "Sheriff, show me my murders!";
        private static readonly int AccessCost = 3000;

        #region constructors
        [Constructable]
        public TownSheriff()
            : this( MidgardTowns.None )
        {
        }

        [Constructable]
        public TownSheriff( MidgardTowns town )
            : base( AIType.AI_Melee, FightMode.Aggressor, 22, 1, 0.2, 1.0 )
        {
            m_Town = town;

            // Stats
            SetStr( 3900, 4000 );
            SetDex( 3900, 4000 );
            SetInt( 3150, 3250 );

            SetHits( 66, 125 );
            SetDamage( 30, 50 );

            CantWalk = true;
            CanHearGhosts = false;
            Blessed = true;

            // Resistances
            SetResistance( ResistanceType.Physical, 40, 50 );
            SetResistance( ResistanceType.Fire, 40, 50 );
            SetResistance( ResistanceType.Cold, 40, 50 );
            SetResistance( ResistanceType.Poison, 40, 50 );
            SetResistance( ResistanceType.Energy, 40, 50 );

            // Skills
            SetSkill( SkillName.Macing, 85.0, 105.5 );
            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Swords, 85.0, 105.5 );
            SetSkill( SkillName.Tactics, 115.0, 119.5 );
            SetSkill( SkillName.Wrestling, 190, 200 );
            SetSkill( SkillName.Parry, 1050.0, 1100.0 );

            // Fama e Karma
            Fame = 5000;
            Karma = 5000;

            // Hair
            HairItemID = 0x203B;

            // Speech & Skin hues
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            VirtualArmor = 70;

            // Nome, Sesso, Titolo  e BodyValue
            Name = NameList.RandomName( "male" );
            Female = false;
            Body = 400;
        }
        #endregion

        #region members
        public virtual void OnTownChanged( MidgardTowns oldValue )
        {
            if( m_Town != MidgardTowns.None )
            {
                if( System != null )
                {
                    if( oldValue == MidgardTowns.None )
                    {
                        EquipItem( TownSystem.Immovable( new PlateChest() ) );
                        EquipItem( TownSystem.Immovable( new PlateLegs() ) );
                        EquipItem( TownSystem.Immovable( new PlateHelm() ) );
                        EquipItem( TownSystem.Immovable( new PlateGorget() ) );
                        EquipItem( TownSystem.Immovable( new PlateArms() ) );
                        EquipItem( TownSystem.Immovable( new PlateGloves() ) );
                        EquipItem( TownSystem.Newbied( new Halberd() ) );
                        EquipItem( TownSystem.Immovable( new Backpack() ) );
                    }

                    System.DressTownVendor( this );

                    Say( true, string.Format( "Yessir! Now I'm the sheriff of {0}", System.Definition.TownName ) );
                }
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( System != null )
                list.Add( "Sheriff of {0}", System.Definition.TownName );
        }

        public override bool HandlesOnSpeech( Mobile from )
        {
            if( InRange( Location, 4 ) )
                return true;

            return base.HandlesOnSpeech( from );
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( !e.Handled && e.Mobile.InRange( Location, 4 ) )
            {
                Midgard2PlayerMobile m2pm = e.Mobile as Midgard2PlayerMobile;

                if( m2pm == null )
                    return;

                if( Insensitive.Contains( e.Speech, MurderString ) )
                {
                    if( Town == m2pm.Town )
                    {
                        if( Banker.GetBalance( m2pm ) >= AccessCost )
                        {
                            Say( true, "Yessir I am at your service." );
                            Banker.Withdraw( m2pm, AccessCost );

                            DoGuardCheck( m2pm );
                        }
                        else
                            Say( true, "My services need money, Sir!" );
                    }
                    else
                        Say( true, "I'm not a citizen of yours" );

                    e.Handled = true;
                }
                else if( Insensitive.Contains( e.Speech, HandcuffsString ) )
                {
                    if( Town == m2pm.Town )
                    {
                        if( Banker.GetBalance( m2pm ) >= 500 )
                        {
                            Say( true, "Yessir I am at your service." );
                            Banker.Withdraw( m2pm, AccessCost );

                            m2pm.AddToBackpack( new Handcuffs() );
                        }
                        else
                            Say( true, "These chains need 500 golds, Sir!" );
                    }
                    else
                        Say( true, "I'm not a citizen of yours" );

                    e.Handled = true;
                }
                else if( Insensitive.Contains( e.Speech, HandcuffsString ) )
                {
                    if( Town == m2pm.Town )
                    {
                        m2pm.SendGump( new MurdersInfoGump( m2pm ) );
                    }
                    else
                        Say( true, "I'm not a citizen of yours" );

                    e.Handled = true;
                }
            }

            base.OnSpeech( e );
        }

        private static void DoGuardCheck( Mobile victim )
        {
            List<MurderInfo> list = MurderInfoPersistance.GetMurderInfoForVictim( victim );
            if( list != null && list.Count > 0 )
            {
                foreach( MurderInfo info in list )
                {
                    if( info != null && info.Killer != null && info.Victim != null && info.Victim == victim )
                        MurderInfoHelper.HandleKiller( info.Killer, false );
                }
            }
        }
        #endregion

        #region serial-deserial
        public TownSheriff( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version

            writer.Write( (int)m_Town );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            m_Town = (MidgardTowns)reader.ReadInt();
        }
        #endregion
    }
}