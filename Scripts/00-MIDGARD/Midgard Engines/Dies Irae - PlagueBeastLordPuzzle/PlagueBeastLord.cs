using System;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    [CorpseName( "a plague beast lord corpse" )]
    public class PuzzlePlagueBeastLord : BaseCreature
    {
        #region fields
        private Item m_Central;
        private Item m_Squid; //To notify it when a healthy gland is placed.
        private bool m_IsFrosted;
        private bool m_IsCutOpen;
        private Timer m_FrostTimer;
        private bool[] m_Receptacles; //Correctly placed brains
        #endregion

        #region properties
        /// <summary>
        /// Monster life time, once it is opened.
        /// </summary>
        [CommandProperty( AccessLevel.Developer )]
        public virtual double MortalitySeconds
        {
            get { return IsInDebugMode ? 6000 : 120; }
        }

        /// <summary>
        /// If true this plague is in debug mode.
        /// </summary>
        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsInDebugMode { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsFrosted
        {
            get { return m_IsFrosted; }
            set
            {
                bool oldValue = m_IsFrosted;
                if( oldValue != value )
                {
                    m_IsFrosted = value;
                    OnIsFrostedChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsCutOpen
        {
            get { return m_IsCutOpen; }
            set
            {
                bool oldValue = m_IsCutOpen;
                if( oldValue != value )
                {
                    m_IsCutOpen = value;
                    OnIsCutOpenChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public Mobile CutOpenBy { get; set; }

        public override bool DeleteCorpseOnDeath { get { return m_IsFrosted; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        #endregion

        #region constructors
        [Constructable]
        public PuzzlePlagueBeastLord()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 1, 1 )
        {
            Name = "a plague beast lord";

            Body = 0x307;
            BaseSoundID = 0x2A7;

            SpeechHue = 0x26;
            EmoteHue = 0x26;

            SetStr( 500 );
            SetDex( 100 );
            SetInt( 30 );

            SetHits( 1800 );

            SetDamage( 20, 25 );

            SetDamageType( ResistanceType.Physical, 50 );
            SetDamageType( ResistanceType.Fire, 25 );
            SetDamageType( ResistanceType.Poison, 25 );

            SetResistance( ResistanceType.Physical, 45, 55 );
            SetResistance( ResistanceType.Fire, 40, 50 );
            SetResistance( ResistanceType.Cold, 25, 35 );
            SetResistance( ResistanceType.Energy, 25, 35 );
            SetResistance( ResistanceType.Poison, 75, 85 );

            SetSkill( SkillName.Tactics, 100 );
            SetSkill( SkillName.Wrestling, 100 );

            Fame = 2000;
            Karma = -2000;

            VirtualArmor = 50;

            AddItem( new PlagueBackpack() );

            m_IsFrosted = false;
            m_IsCutOpen = false;
            m_Receptacles = new bool[] { false, false, false, false };
        }
        #endregion

        #region events OnChanged
        /// <summary>
        /// Event invoked when Frost property is changed
        /// </summary>
        public virtual void OnIsFrostedChanged( bool oldValue )
        {
            if( IsInDebugMode )
                Say( !oldValue ? 1066139 : 1066140 ); // Hey guy, now I'm Frosted. / WoW, I'm no longer Frosted.

            if( !oldValue )
            {
                StartFrostTimer();
                DoFrost();
            }
            else
            {
                StopFrostTimer();
                DoUnfrost();

                m_IsCutOpen = false;
            }
        }

        /// <summary>
        /// Event invoked when CutOpen property is changed
        /// </summary>
        public virtual void OnIsCutOpenChanged( bool oldValue )
        {
            if( IsInDebugMode )
                Say( !oldValue ? 1066145 : 1066146 ); // Ahi! I don't like vivisection! / Oh thanks: nobody is my vivisectioner!

            if( !oldValue )
            {
                if( !IsFrosted )
                    IsFrosted = true;
                InitializePuzzle();
            }
            else
            {
                Backpack.Delete();
                AddItem( new PlagueBackpack() );
            }
        }
        #endregion

        #region members
        /// <summary>
        /// Stop FrostTimer.
        /// </summary>
        public void StopFrostTimer()
        {
            if( IsInDebugMode )
                Say( 1066141 ); // My frost timer stopped.

            if( m_FrostTimer != null && m_FrostTimer.Running )
                m_FrostTimer.Stop();
        }

        /// <summary>
        /// Start FrostTimer.
        /// </summary>
        public void StartFrostTimer()
        {
            if( IsInDebugMode )
                Say( 1066142 ); // My frost timer started.

            if( m_FrostTimer != null && m_FrostTimer.Running )
                m_FrostTimer.Stop();

            TimeSpan delay = TimeSpan.FromSeconds( MortalitySeconds / 2 );
            m_FrostTimer = Timer.DelayCall( delay, delay, new TimerCallback( OnFrozenTimerTick ) );
        }

        /// <summary>
        /// Open the creature. Creates the puzzle.
        /// </summary>
        public void InitializePuzzle()
        {
            m_IsCutOpen = true;

            //Place decorative stuff. Blood, red receptacles...
            for( int i = 0; i < 8; i++ )
            {
                CreateGuts( new DecoItem( 0x9DF, 1066101, 32 ), 80, 360, 100, 270 ); // receptacle      	
                CreateGuts( new DecoItem( 0x1D92 + ( i % 5 ), 1066125, 0 ), 60, 350, 90, 250 ); // blood         	
                CreateGuts( new DecoItem( 0x1D92 + ( i % 5 ), 1066125, 0 ), 80, 360, 110, 270 ); // blood        	          	
                CreateGuts( new DecoItem( 0x1D0A + i, 1066125, 0 ), 80, 360, 110, 270 ); // blood         	
            }

            //We place the organs and brains randomly
            int position = Utility.Random( 8 );
            int color = Utility.Random( 8 );
            int type = Utility.Random( 8 );

            //Place organs and other useful things
            for( int i = 0; i < m_OrgansTypes.Length; i++ )
            {
                OrganTypes organType = m_OrgansTypes[ ( type + i ) % m_OrgansTypes.Length ];
                BrainTypes brainType = m_BrainsType[ ( color + i ) % m_BrainsType.Length ];
                Point3D organLocation = m_OrgansLocations[ ( position + i ) % m_OrgansLocations.Length ];

                //Depending on what kind of organ we create one of the four possibles:
                switch( organType )
                {
                    case OrganTypes.Squid:
                        {
                            m_Squid = new SquidOrgan( brainType );
                            CreateOrgan( (BaseOrgan)m_Squid, organLocation );
                            break;
                        }
                    case OrganTypes.Maiden:
                        CreateOrgan( new MaidenOrgan( brainType ), organLocation );
                        break;
                    case OrganTypes.Polyp:
                        CreateOrgan( new PolypOrgan( brainType ), organLocation );
                        break;
                    case OrganTypes.Veins:
                        CreateOrgan( new BubblyOrgan( brainType ), organLocation );
                        break;
                }
            }

            //Now we create the receptacles near the core
            //First, the veins connecting them to the central organ
            CreateGuts( new DecoItem( 0x1B1B, 1066128, (int)BrainHues.Green ), GetNervLocFromBrainType( BrainTypes.Green ) ); // nerve
            CreateGuts( new DecoItem( 0x1B1C, 1066128, (int)BrainHues.Blue ), GetNervLocFromBrainType( BrainTypes.Blue ) ); // nerve
            CreateGuts( new DecoItem( 0x1B1C, 1066128, (int)BrainHues.Yellow ), GetNervLocFromBrainType( BrainTypes.Yellow ) ); // nerve
            CreateGuts( new DecoItem( 0x1B1B, 1066128, (int)BrainHues.Purple ), GetNervLocFromBrainType( BrainTypes.Purple ) ); // nerve

            //Then, the 4 receptacles.
            CreateGuts( new DecoItem( 0x9DF, 1066101, (int)BrainHues.Green ), GetReceptorLocFromBrainType( BrainTypes.Green ) ); // receptacle
            CreateGuts( new DecoItem( 0x9DF, 1066101, (int)BrainHues.Blue ), GetReceptorLocFromBrainType( BrainTypes.Blue ) ); // receptacle
            CreateGuts( new DecoItem( 0x9DF, 1066101, (int)BrainHues.Yellow ), GetReceptorLocFromBrainType( BrainTypes.Yellow ) ); // receptacle
            CreateGuts( new DecoItem( 0x9DF, 1066101, (int)BrainHues.Purple ), GetReceptorLocFromBrainType( BrainTypes.Purple ) ); // receptacle

            //Create the Central organ.
            m_Central = new CentralOrgan();
            CreateOrgan( (BaseOrgan)m_Central, new Point3D( 153, 176, 0 ) );
        }

        /// <summary>
        /// Adds an item to the creature, and place it on a give position p
        /// </summary>
        public void CreateGuts( Item t, Point3D p )
        {
            Backpack.AddItem( t );
            t.Location = p;
        }

        /// <summary>
        /// Adds an item to the creature, and place it on a give x, y location
        /// </summary>
        public void CreateGuts( Item t, int x, int y )
        {
            Point3D p = new Point3D( x, y, 0 );
            CreateGuts( t, p );
        }

        /// <summary>
        /// Adds an item to the creature, and place it on a given positon
        /// at random in X and Y limits
        /// </summary>
        public void CreateGuts( Item t, int minX, int maxX, int minY, int maxY )
        {
            Point3D p = new Point3D( Utility.RandomMinMax( minX, maxX ), Utility.RandomMinMax( minY, maxY ), 0 );
            CreateGuts( t, p );
        }

        /// <summary>
        /// Create one organ and place in location given by p
        /// </summary>
        public void CreateOrgan( BaseOrgan t, Point3D p )
        {
            CreateGuts( t, p );
            t.CreateTissue();
        }

        /// <summary>
        /// Check it the healthy gland is correctly placed on the squid organ
        /// If yes, call the method OnGlandDrop 
        /// </summary>
        public void HealtyGlandPlaceCheck( Point3D p, Item gland, Mobile from )
        {
            if( m_Squid.X - 10 <= p.X && p.X <= m_Squid.X + 30 && m_Squid.Y - 10 <= p.Y && p.Y <= m_Squid.Y + 30 )
            {
                SquidOrgan squid = m_Squid as SquidOrgan;
                if( squid != null )
                    squid.OnGlandDrop( from, gland );
            }
        }

        /// <summary>
        /// Freeze the creature
        /// </summary>
        public void DoFrost()
        {
            FightMode = FightMode.None;
            FocusMob = null;
            Combatant = null;

            Frozen = true;
            CantWalk = true;

            Hue = 0x480;

            Emote( 1066103 ); // * The plague beast's amorphous flesh hardens and becomes immobilized *
        }

        /// <summary>
        /// Unfreeze the creature.
        /// </summary>
        public void DoUnfrost()
        {
            FightMode = FightMode.Closest;

            Frozen = false;
            CantWalk = false;

            Hue = 0x0;

            Emote( 1066104 ); // * The Plague Beast Lord starts to move again! *
        }

        /// <summary>
        /// Used when a brain is moved. If all brains are correctly placed. The central organ is opened
        /// </summary>
        public void CoreCheck( int spot, bool correct )
        {
            m_Receptacles[ spot ] = correct;

            for( int i = 0; i < 4; i++ )
            {
                if( !m_Receptacles[ i ] )
                    return; //If any brain isn't correctly placed we exit
            }

            CentralOrgan co = m_Central as CentralOrgan;
            if( co != null )
                co.OpenOrganTo( this );
        }

        /// <summary>
        /// Override: Only owner or GM can open the pack.
        /// </summary>
        public override void OnDoubleClick( Mobile from )
        {
            if( m_IsCutOpen && CutOpenBy == from )
                Backpack.DisplayTo( from );
        }

        /// <summary>
        /// Override: Only brains and healthy glands can be drop inside it.
        /// </summary>
        public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
        {
            return item is Gland || item is HealtyGland || from.AccessLevel == AccessLevel.Developer;
        }

        /// <summary>
        /// We always can move things from its backpack
        /// </summary>
        public override bool CheckNonlocalLift( Mobile from, Item item )
        {
            return true;
        }

        /// <summary>
        /// If he is damaged, it unfreezes
        /// </summary>
        public override void OnDamage( int amount, Mobile from, bool willKill )
        {
            if( m_IsFrosted && !m_IsCutOpen )
                IsFrosted = false;

            base.OnDamage( amount, from, willKill );
        }

        /// <summary>
        /// On killed give first a simpathetic message
        /// </summary>
        public override void Kill()
        {
            Say( 1066105 ); // * The Plague Beast died *
            base.Kill();
            // Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), new TimerCallback( base.Kill ));
        }

        private void OnFrozenTimerTick()
        {
            if( !Deleted && Alive && Map != Map.Internal )
            {
                if( !m_IsFrosted )
                    StopFrostTimer();

                if( CantWalk )
                {
                    if( m_IsCutOpen )
                        Say( 1066143 ); // * The plague beast begins to dissolve *
                    else
                        Say( 1066144 ); // * The plague beast burbles incoherently *

                    CantWalk = false;
                }
                else
                {
                    StopFrostTimer();

                    if( m_IsCutOpen )
                        Kill();
                    else
                        m_IsFrosted = false;
                }
            }
            else
                StopFrostTimer();
        }
        #endregion

        #region lists
        /// <summary>
        /// These are the positions of the 8 organs of the beast
        /// </summary>
        private static Point3D[] m_OrgansLocations = new Point3D[]
		{
	   	    new Point3D( 96, 265, 0 ),
			new Point3D( 70, 180, 0 ),
			new Point3D( 293, 265, 0 ),
			new Point3D( 96, 95, 0 ),
			new Point3D( 293, 100, 0 ),
 			new Point3D( 188, 98, 0 ),
			new Point3D( 315, 180, 0 ),
 			new Point3D( 188, 263, 0 )
		};

        /// <summary>
        /// There will be 8 organs randomly placed
        /// </summary>
        private static OrganTypes[] m_OrgansTypes = new OrganTypes[]
		{
			OrganTypes.Maiden,
			OrganTypes.Polyp,
			OrganTypes.Squid,
			OrganTypes.Veins,
			OrganTypes.Maiden,
			OrganTypes.Polyp,
			OrganTypes.Veins,
			OrganTypes.Maiden
		};

        /// <summary>
        /// Brain type inside each organ
        /// </summary>
        private static BrainTypes[] m_BrainsType = new BrainTypes[]
		{
 			BrainTypes.Blue,
 			BrainTypes.Green,
 			BrainTypes.Purple,
 			BrainTypes.Yellow,
 			BrainTypes.None, //That means the organ doesn't have brain inside
 			BrainTypes.None,
 			BrainTypes.None,
 			BrainTypes.None
		};

        /// <summary>
        /// Return relative location of nerves in plague pack from brain type
        /// </summary>
        public static Point3D GetNervLocFromBrainType( BrainTypes type )
        {
            return m_NervLocs[ (int)type ];
        }

        public static Point3D[] m_NervLocs = new Point3D[]
		{
			new Point3D( 10, 10, 0 ),
			new Point3D( 170, 179, 0 ),
			new Point3D( 229, 179, 0 ),
			new Point3D( 170, 229, 0 ),
			new Point3D( 229, 229, 0 )
		};

        /// <summary>
        /// Return relative location of brain receptors in plague pack from brain type
        /// </summary>
        public static Point3D GetReceptorLocFromBrainType( BrainTypes type )
        {
            return m_ReceptorsLocs[ (int)type ];
        }

        public static Point3D[] m_ReceptorsLocs = new Point3D[]
		{
			new Point3D( 10, 10, 0 ),
			new Point3D( 150, 170, 0 ),
			new Point3D( 240, 170, 0 ),
			new Point3D( 150, 240, 0 ),
			new Point3D( 240, 240, 0 ),
		};
        #endregion

        #region serial-deserial
        public PuzzlePlagueBeastLord( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( m_IsFrosted );
            writer.Write( m_Central );
            writer.Write( m_Squid );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_IsFrosted = reader.ReadBool();
            m_Central = reader.ReadItem();
            m_Squid = reader.ReadItem();

            if( m_IsFrosted ) //Delete 'frozen' Plague Beast Lords.
                Delete();
        }
        #endregion
    }
}