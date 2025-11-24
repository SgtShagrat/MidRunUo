/***************************************************************************
 *                                  CitizenNotary.cs
 *                            		--------------
 *  begin                	: Aprile, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;
using System.Text;

namespace Midgard.Engines.MidgardTownSystem
{
    [CorpseName( "a notary corpse" )]
    public class CitizenNotary : BaseCreature
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

        private static readonly string ResignString = "I resign from my town";

        private const int AccessCostInf30 = 10000;
        private const int AccessCostSup360 = 2000;
        private const int DefaultAccessCost = 5000;

        public static readonly TimeSpan DeltaTimeLower = TimeSpan.FromDays( 30.0 );
        public static readonly TimeSpan DeltaTimeUpper = TimeSpan.FromDays( 180.0 );

        #region constructors
        [Constructable]
        public CitizenNotary()
            : this( MidgardTowns.None )
        {
        }

        [Constructable]
        public CitizenNotary( MidgardTowns town )
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

        public CitizenNotary( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region members
        public virtual void OnTownChanged( MidgardTowns oldValue )
        {
            InvalidateProperties();

            if( m_Town != MidgardTowns.None )
            {
                if( System != null )
                {
                    System.DressTownVendor( this );

                    Say( true, string.Format( "Yessir! Now I'm the notary of {0}", System.Definition.TownName ) );
                }
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( System != null )
                list.Add( "Notary of {0}", System.Definition.TownName );
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public override bool HandlesOnSpeech( Mobile from )
        {
            if( InRange( Location, 4 ) )
                return true;

            return base.HandlesOnSpeech( from );
        }

        private static int GetCost( Mobile m )
        {
            Midgard2PlayerMobile m2pm = m as Midgard2PlayerMobile;

            if( m2pm == null )
                return 0;

            DateTime lastLeaving = m2pm.LastTownLeaving;

            if( lastLeaving + DeltaTimeLower > DateTime.Now )
                return AccessCostInf30;
            else if( lastLeaving + DeltaTimeUpper < DateTime.Now  )
                return AccessCostSup360;

            return DefaultAccessCost;
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( !e.Handled && e.Mobile.InRange( Location, 4 ) )
            {
                Midgard2PlayerMobile m2pm = e.Mobile as Midgard2PlayerMobile;

                if( m2pm == null )
                    return;

                if( e.Speech.ToLower().IndexOf( ResignString.ToLower() ) >= 0 )
                {
                    if( Town == m2pm.Town )
                    {
                        int cost = GetCost( m2pm );

                        if( Banker.GetBalance( m2pm ) >= cost )
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat(
                                "{0} tou are going to leave <em><basefont color=red>{1}</basefont></em>.<br>" +
                                "Because of your last town leaving, this notary act will cost you " +
                                "<em><basefont color=red>{2}</basefont></em> gold pieces.<br>" +
                                "Are you really sure you want to proceede? This action is irreversible.",
                                m2pm.Name, System.Definition.TownName, cost );

                            m2pm.SendGump( new WarningGump( 1060635, 30720, sb.ToString(), 0xFFC000, 420, 280,
                                new WarningGumpCallback( ConfirmResetCallBack ),
                                new object[] { this, cost } ) );
                        }
                        else
                            Say( true, "My services need money, Sir!" );
                    }
                    else
                        Say( true, "I'm not a citizen of yours" );

                    e.Handled = true;
                }
            }

            base.OnSpeech( e );
        }

        private static void ConfirmResetCallBack( Mobile from, bool okay, object state )
        {
            object[] states = (object[])state;

            CitizenNotary notary = (CitizenNotary)states[0];
            int cost = (int)states[1];

            if( notary == null || !from.InRange( notary.Location, 3 ) )
            {
                from.SendMessage( "You are to far away from the notary to proceed." );
                return;
            }

            TownSystem t = notary.System;
            if( t == null )
                return;

            if( okay )
            {
                Account a = from.Account as Account;

                if( a != null )
                {
                    notary.Say( true, "Yessir I am at your service. You are now free from this city" );

                    Banker.Withdraw( from, cost );
                    t.RegisterTransaction( cost );

                    TownHelper.DoAccountReset( (Account)from.Account );

                    TownLog.Log( LogType.Treasure, String.Format(
                                    "{0} (account {1}) has used {2} notary for the cost of {3} in dateTime {4}.",
                                    from.Name, a.Username, t.Definition.TownName, cost, DateTime.Now.ToString() ) );

                    if( from is Midgard2PlayerMobile )
                        ( (Midgard2PlayerMobile)from ).LastTownLeaving = DateTime.Now;
                }
            }
            else
            {
                notary.Say( true, "Yessir. Take your time to evaluate your decision." );
            }
        }
        #endregion

        #region serial-deserial
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
        #endregion
    }
}