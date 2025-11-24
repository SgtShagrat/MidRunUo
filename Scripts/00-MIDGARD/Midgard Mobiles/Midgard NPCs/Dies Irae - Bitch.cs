/***************************************************************************
 *                                    Bitch.cs
 *                            		-----------
 *  begin                	: Luglio 2007
 *  version					: 2.1 **VERSIONE PER RUNUO 2.0**
 *  original concept		: Smeagol
 * 	rebuild					: Dies Irae
 *
 ***************************************************************************/

/***************************************************************************
* 
* 	Info:
* 			NPC che agisce come una prostituta.
*	Versionamento:
* 
***************************************************************************/

using System;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    public class Bitch : BaseCreature
    {
        #region proprietà
        public override bool CanTeach { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public Bitch()
            : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            // Stats
            SetStr( 304, 400 );
            SetDex( 102, 150 );
            SetInt( 204, 300 );

            SetHits( 66, 125 );
            SetDamage( 30, 50 );

            Blessed = true;
            CantWalk = true;

            // Resistances
            SetResistance( ResistanceType.Physical, 40, 50 );
            SetResistance( ResistanceType.Fire, 40, 50 );
            SetResistance( ResistanceType.Cold, 40, 50 );
            SetResistance( ResistanceType.Poison, 40, 50 );
            SetResistance( ResistanceType.Energy, 40, 50 );

            // Skills
            SetSkill( SkillName.Anatomy, 120.0, 120.0 );
            SetSkill( SkillName.Inscribe, 90.0, 100.0 );
            SetSkill( SkillName.Magery, 90.0, 100.0 );
            SetSkill( SkillName.Meditation, 90.0, 100.0 );
            SetSkill( SkillName.MagicResist, 90.0, 100.0 );
            SetSkill( SkillName.Wrestling, 90.0, 100.0 );

            // Fama e Karma
            Fame = Utility.RandomMinMax( 100, 1000 );
            Karma = -( Utility.RandomMinMax( 100, 1000 ) );

            // Hair
            HairItemID = 0x203B;
            HairHue = Utility.RandomHairHue();

            // Speech & Skin hues
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            // Title
            Title = "the gambler";

            Female = true;
            Name = NameList.RandomName( "female" );
            Title = ", the pretty girl";
            Body = 0x191;

            AddItem( new LongHair( Utility.RandomHairHue() ) );

            FemaleLeatherChest chest = new FemaleLeatherChest();
            chest.LootType = LootType.Blessed;
            chest.Hue = Utility.RandomSlimeHue();
            AddItem( chest );

            LeatherSkirt skirt = new LeatherSkirt();
            skirt.LootType = LootType.Blessed;
            skirt.Hue = Utility.RandomSlimeHue();
            AddItem( skirt );

            ThighBoots boots = new ThighBoots();
            boots.LootType = LootType.Blessed;
            boots.Hue = Utility.RandomSlimeHue();
            AddItem( boots );
        }

        public Bitch( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region SpeechLists
        private string[] m_SpeechList = new string[]
		{
			"sex"
		};

        private string[] m_BitchSpeechList = new string[] 
		{ 
			"Ah..You are a Mustang!!", 
			"Oh My God!! You are a beast!", 
			"Oh...Ahhh....AHhhh..", 
			"You are the best! ahhh...",
			"thanks for the gold baby!" 
		};
        #endregion

        #region metodi
        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich, 2 );
        }

        public override bool CheckTeach( SkillName skill, Mobile from )
        {
            if( !base.CheckTeach( skill, from ) )
                return false;

            return skill == SkillName.Anatomy;
        }

        public static readonly int BitchCost = 1000;

        public override bool HandlesOnSpeech( Mobile from )
        {
            return true;
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            PlayerMobile User = e.Mobile as PlayerMobile;

            if( User == null )
                return;

            if( e.Handled || !User.Alive || User.GetDistanceToSqrt( this ) > 4 )
                return;

            if( !e.Handled && User.InRange( this, 4 ) && CanSee( User ) )
            {
                foreach( string s in m_SpeechList )
                {
                    if( Insensitive.Equals( s, e.Speech ) )
                    {
                        e.Handled = true;

                        if( User.Mounted )
                        {
                            Say( "Ah, baby..? This Horse??" );
                            return;
                        }

                        if( Banker.GetBalance( User ) < BitchCost )
                        {
                            Say( "Ah, baby..? Thou hast not so much gold!" );
                            return;
                        }
                        else
                        {
                            Banker.Withdraw( User, BitchCost );
                            int oldLight = User.LightLevel;
                            User.LightLevel = -40000;
                            User.Frozen = true;
                            SpellHelper.Turn( this, User );
                            SpeechHelper.SayALot( m_BitchSpeechList, this, oldLight, User );
                        }
                    }
                    else
                    {
                        base.OnSpeech( e );
                    }
                }
            }
        }
        #endregion

        #region serialize-deserialize
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
        #endregion

        internal class SpeechHelper
        {
            public static void SayRandom( string[] say, Mobile mob )
            {
                mob.Say( say[ Utility.Random( say.Length ) ] );
            }

            public static void SayRandom( int[] say, Mobile mob )
            {
                mob.Say( say[ Utility.Random( say.Length ) ] );
            }

            public static void SayALot( string[] say, Mobile mob, TimeSpan delay, int oldlight, Mobile e )
            {
                SayALotTimer timer = new SayALotTimer( say, mob, delay, oldlight, e );
                timer.Start();
            }

            public static void SayALot( string[] say, Mobile mob, int oldlight, Mobile e )
            {
                SayALotTimer timer = new SayALotTimer( say, mob, TimeSpan.FromSeconds( 3.0 ), oldlight, e );
                timer.Start();
            }

            private class SayALotTimer : Timer
            {
                private int m_Mode;
                private Mobile m_Owner;
                private string[] m_Say;
                private int m_oldlight;
                private Mobile m_e;

                public SayALotTimer( string[] s, Mobile m, TimeSpan delay, int oldlight, Mobile e )
                    : base( delay, delay )
                {
                    m_Mode = 0;
                    m_Owner = m;
                    m_Say = s;
                    m_oldlight = oldlight;
                    m_e = e;

                    Priority = TimerPriority.TwoFiftyMS;
                    m_Owner.Say( m_Say[ m_Mode++ ] );
                }

                protected override void OnTick()
                {
                    m_Owner.Say( m_Say[ m_Mode++ ] );
                    Effects.PlaySound( m_Owner.Location, m_Owner.Map, 0x30A );
                    m_Owner.Animate( 32, 5, 1, true, false, 0 );
                    m_e.Animate( 32, 5, 1, true, false, 0 );

                    if( m_Mode >= m_Say.Length )
                    {
                        Stop();
                        m_e.LightLevel = m_oldlight;
                        m_e.Frozen = false;
                        Effects.PlaySound( m_Owner.Location, m_Owner.Map, 0x37 );
                    }
                }
            }
        }
    }
}
