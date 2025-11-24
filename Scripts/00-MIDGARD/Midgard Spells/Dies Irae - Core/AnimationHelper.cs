using System;
using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.Classes
{
    public enum Animations
    {
        None = -1,

        Walkunarmed,
        Walkarmed,
        Rununarmed,
        Runarmed,
        Stand,
        Lookaround,
        Fidget,
        OneHwarmode,
        TwoHwarmode,
        OneHgenericmeleeswing,
        OneHFencingJab,
        OneHOverheadMace,
        TwoHMaceJab,
        TwoHGenericMeleeSwing,
        TwoHSpearJab,
        WalkWarmode,
        DirectionalSpellcast,
        AreaEffectSpellcast,
        BowAttack,
        CrossbowAttack,
        TakeAHit,
        DieOntoBack,
        DieOntoFace,
        RideSlow,
        RideFast,
        SitOnHorse,
        MountedAttack,
        MountedBowShot,
        MountedCrossbowShot,
        SlapHorse,
        Dodge,
        Punch,
        Bow,
        Salute,
        Eat
    }

    public class GenerateAnimationsDemo
    {
        public static void Initialize()
        {
            CommandSystem.Register( "GenerateAnimationsDemo", AccessLevel.Developer, new CommandEventHandler( GenerateAnimationsDemo_OnCommand ) );
        }

        private static readonly Point3D StartDemoLocation = new Point3D( 5519, 1176, 0 );
        private static readonly Map StartDemoMap = Map.Felucca;
        private static int m_Count;

        [Usage( "GenerateAnimationsDemo" )]
        [Description( "Generate animation demo items." )]
        private static void GenerateAnimationsDemo_OnCommand( CommandEventArgs e )
        {
            int x = StartDemoLocation.X;
            int y = StartDemoLocation.Y;
            int z = StartDemoLocation.Z;

            foreach( ParticleData i in ParticleData.Table )
            {
                if( FindEffectController( x, y, z ) )
                    return;

                EffectController controller = new EffectController();

                controller.Name = i.Name;
                controller.EffectType = ECEffectType.Target;
                controller.TriggerType = EffectTriggerType.DoubleClick;
                controller.EffectItemID = i.ItemID;
                controller.Speed = 10;
                controller.Duration = 50;
                controller.EffectHue = 0;
                controller.RenderMode = 2;      // trasparency
                controller.ParticleEffect = 0;  // not required in 2d clients
                controller.Unknown = 0;         // not required in 2d clients

                controller.MoveToWorld( new Point3D( x, y, z ), StartDemoMap );

                x += 1;

                m_Count++;
            }

            e.Mobile.SendMessage( "A total of {0} effect controller has been generated.", m_Count );
            e.Mobile.MoveToWorld( new Point3D( StartDemoLocation.X, StartDemoLocation.Y + 3, StartDemoLocation.Z ), StartDemoMap );
        }

        public static bool FindEffectController( int x, int y, int z )
        {
            IPooledEnumerable eable = Map.Felucca.GetItemsInRange( new Point3D( x, y, z ), 0 );

            foreach( Item item in eable )
            {
                if( item is EffectController && item.Z == z )
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }
    }

    public class ParticleData
    {
        public int ItemID { get; private set; }
        public int FrameLength { get; set; }
        public int SpeedOffset { get; set; }
        public string Name { get; private set; }

        public static readonly ParticleData[] Table = new ParticleData[] 
            {
                new ParticleData("Explosion", 0x36B0, 0),           // 14000 explosion 1
                new ParticleData("Explosion", 0x36BD, 0),           // 14013 explosion 2
                new ParticleData("Explosion",0x36CB, 0),            // 14027 explosion 3
                new ParticleData("Large Fireball",0x36D4, 0),       // 14036 large fireball
                new ParticleData("Small Fireball",0x36E4, 0),       // 14052 small fireball
                new ParticleData("Fire Snake",0x36F4, 0),           // 14068 fire snake
                new ParticleData("Explosion Ball",0x36FE, 0),       // 14078explosion ball
                new ParticleData("Fire Column",0x3709, 0),          // 14089 fire column
                                                                    // 14106 - display only the ending of fire column - is this actually used?
                new ParticleData("Smoke",0x3728, 0),                // 14120 smoke
                new ParticleData("Fizzle",0x3735, 0),               // 14133 fizzle
                new ParticleData("Sparkle Blue",0x373A, 0),         // 14138 sparkle blue
                new ParticleData("Sparkle Red",0x374A, 0),          // 14154 sparkle red
                new ParticleData("Sparkle Yellow",0x375A, 0),       // 14170 sparkle yellow blue
                new ParticleData("Sparkle Surround",0x376A, 0),     // 14186 sparkle surround
                new ParticleData("Sparkle Planar",0x3779, 0),       // 14201 sparkle planar
                new ParticleData("Death Vortex",0x3789, 0),         // 14217 death vortex (whirlpool on ground?)
                new ParticleData("Magic Arrow",0x379E, 0),          // glowing arrow
                new ParticleData("Small Bolt",0x379F, 0),           // small bolt
                new ParticleData("Field of Blades (Summon)",0x37A0, 0),     // field of blades (summon?)
                new ParticleData("Glow",0x37B9, 0),                 // glow
                new ParticleData("Death Vortex",0x37CC, 0),         // death vortex
                new ParticleData("Field of Blades (Folding)",0x37EB, 0),    // field of blades (folding up)
                new ParticleData("Field of Blades (Unfolding)",0x37F7, 0),  // field of blades (unfolding)
                new ParticleData("Energy",0x3818, 0),               // energy
                new ParticleData("Poison Wall (SW)",0x3914, 0),     // field of poison (facing SW)
                new ParticleData("Poison Wall (SE)",0x3920, 0),     // field of poison (facing SE)
                new ParticleData("Energy Wall (SW)",0x3946, 0),     // field of energy (facing SW)
                new ParticleData("Energy Wall (SE)",0x3956, 0),     // field of energy (facing SE)
                new ParticleData("Paralysis Wall (SW)",0x3967, 0),  // field of paralysis (facing SW, open and close?)
                new ParticleData("Paralysis Wall (SE)",0x3979, 0),  // field of paralysis (Facing SE, open and close?)
                new ParticleData("Fire Wall (SW)",0x398C, 0),       // field of fire (facing SW)
                new ParticleData("Fire Wall (SE)",0x3996, 0),       // field of fire (facing SE)
                new ParticleData("<null>",0x39A0, 0)                // Used to determine the frame length of the preceding effect.
            };

        static ParticleData()
        {
            DetermineParticleLengths();

            if( DefaultEffect == null )
                DefaultEffect = Table[ 0 ];
        }

        private static void DetermineParticleLengths()
        {
            for( int i = 0; i < Table.Length - 1; i++ )
            {
                Table[ i ].FrameLength = Table[ i + 1 ].ItemID - Table[ i ].ItemID;
            }
        }

        public static ParticleData DefaultEffect { get; set; }

        private static readonly int[] RandomExplosionList = new int[] { 0x36B0, 0x36BD, 0x36CB, 0x36B0 };

        public static ParticleData RandomExplosion
        {
            get { return GetData( Utility.RandomList( RandomExplosionList ) ); }
        }

        private static readonly int[] RandomSparkleList = new int[] { 0x373A, 0x374A, 0x375A, 0x3779 };

        public static ParticleData RandomSparkle
        {
            get { return GetData( Utility.RandomList( RandomSparkleList ) ); }
        }

        public static ParticleData GetData( int itemID )
        {
            if( itemID < Table[ 0 ].ItemID )
                return null;
            if( itemID >= Table[ Table.Length - 1 ].ItemID )
                return null;

            for( int i = 1; i < Table.Length; i++ )
            {
                if( itemID >= Table[ i ].ItemID )
                    continue;

                ParticleData pData = Table[ i - 1 ];
                if( itemID != pData.ItemID )
                    Console.WriteLine( "Mismatch Requested particle {0}, returning particle {1}.", itemID, pData.ItemID );

                return Table[ i - 1 ];
            }

            Console.WriteLine( "Unknown particle effect with ItemID={0}", itemID );

            return null;
        }

        public ParticleData( string name, int itemID, int speed )
        {
            Name = name;
            ItemID = itemID;
            SpeedOffset = speed;
        }
    }

    /// <summary>
    /// Command A T ACF B1 B2 AS
    /// A: Defines which animation to play (see the list below).
    /// T: Time to perform animation.
    /// ACF: Animation Cycle Flow
    ///     0 - Causes the mobile to cycle through all of its animations (see the list below). 
    ///     1 - Causes the mobile to cycle through the defined animation once.
    ///     2 - Causes the mobile to cycle through the defined animation (0(see the list)),then next 
    ///         animation (1),and then the defined animation again (0).
    ///     3 - Causes the mobile to cycle through the defined animation (0),then next animation 
    ///         (1),and then the animation after that (2), then backward (0-1-2-1-0).
    /// B1: Animation Cycle Direction
    ///     true - Forward then backward (as in: 0-1-2-1-0).
    ///     false - Backward then forward (as in: 2-1-0-1-2).
    /// B2: Animation Cycle
    ///     true - Animation is performed as in 'ACF'.
    ///     false - Animation is performed forward only (as in: 0-1-2).
    /// AS: Animation Speed
    ///     0 - The animation runs normally.
    ///     1 - The animation runs slower.
    ///     2 - The animation runs even slower.
    /// </summary>
    public class AnimateCommand : BaseCommand
    {
        public static void Initialize()
        {
            TargetCommands.Register( new AnimateCommand() );
        }

        public AnimateCommand()
        {
            AccessLevel = AccessLevel.Administrator;
            Supports = CommandSupport.AllMobiles;
            Commands = new string[] { "Animate" };
            ObjectTypes = ObjectTypes.Mobiles;
            Usage = "Animate <action>, <frameCount>, <repeatCount>, <forward>, <repeat>, <delay>";
            Description = "Animate targeted mobile.";
        }

        public override void Execute( CommandEventArgs e, object obj )
        {
            Mobile mob = (Mobile)obj;
            Mobile from = e.Mobile;

            string arg1 = e.GetString( 0 );

            int anim;
            try
            {
                if( arg1 != string.Empty && Enum.IsDefined( typeof( Animations ), arg1 ) )
                    anim = (int)Enum.Parse( typeof( Animations ), arg1 );
                else
                    anim = int.Parse( arg1 );
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception );
                LogFailure( "Could not parse first command argument." );
                return;
            }

            try
            {
                if( e.Length == 6 )
                {
                    CommandLogging.WriteLine( from, "{0} {1} animating {2}: \"{3}\"",
                        from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( mob ), anim );
                    mob.Animate( anim, e.GetInt32( 1 ), e.GetInt32( 2 ), e.GetBoolean( 3 ), e.GetBoolean( 4 ), e.GetInt32( 5 ) );
                }
                else if( e.Length == 2 )
                {
                    CommandLogging.WriteLine( from, "{0} {1} animating {2}: \"{3}\"",
                        from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( mob ), anim );
                    from.Animate( anim, e.GetInt32( 1 ), 1, true, false, 0 );
                }
                else if( e.Length == 1 )
                {
                    CommandLogging.WriteLine( from, "{0} {1} animating {2}: \"{3}\"",
                        from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( mob ), anim );
                    from.Animate( anim, 7, 1, true, false, 0 );
                }
                else
                    LogFailure( "Usage: Animate <action>, <frameCount>, <repeatCount>, <forward>, <repeat>, <delay>" );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
                LogFailure( "Usage: Animate <action>, <frameCount>, <repeatCount>, <forward>, <repeat>, <delay>" );
            }
        }
    }

    public abstract class BaseAnimTimer : Timer
    {
        public Mobile From { get; private set; }

        protected static readonly int[] DefaultAnimIds = new int[] { 245, 266 };
        protected static double DefaultAnimateDelay = 1.5;

        public virtual double AnimateDuration { get { return 6.0; } }

        public virtual void SayMessage( Mobile from )
        {
        }

        public virtual void Animate( Mobile from )
        {
            if( !From.Mounted && From.Body.IsHuman )
                From.Animate( GetAnimId(), 7, 1, true, false, 0 );
        }

        public virtual void DoSpecial( Mobile from )
        {
        }

        public virtual int GetAnimId()
        {
            return Utility.RandomList( DefaultAnimIds );
        }

        public BaseAnimTimer( Mobile from, double delay, int count )
            : base( TimeSpan.Zero, TimeSpan.FromSeconds( delay ), count )
        {
            From = from;

            Priority = TimerPriority.FiftyMS;
        }

        protected override void OnTick()
        {
            Animate( From );

            SayMessage( From );

            DoSpecial( From );
        }
    }

    public class SimpleAnimationTimer : BaseAnimTimer
    {
        private readonly int[] m_Anims;

        public SimpleAnimationTimer( Mobile from, int count )
            : this( from, DefaultAnimIds, count )
        {
        }

        public SimpleAnimationTimer( Mobile from, int[] anims, int count )
            : base( from, DefaultAnimateDelay, count )
        {
            m_Anims = anims;
        }

        public override int GetAnimId()
        {
            return Utility.RandomList( m_Anims );
        }

        protected override void OnTick()
        {
            Animate( From );
        }
    }

    public class SayAndMoveAnimationTimer : SimpleAnimationTimer
    {
        public string Message { get; set; }

        public SayAndMoveAnimationTimer( Mobile from, int count, string message )
            : this( from, DefaultAnimIds, count, message )
        {
        }

        public SayAndMoveAnimationTimer( Mobile from, int[] anims, int count, string message )
            : base( from, anims, count )
        {
            Message = message;
        }

        protected override void OnTick()
        {
            Animate( From );
            SayMessage( From );
        }

        public override void SayMessage( Mobile from )
        {
            if( from != null )
                from.PublicOverheadMessage( MessageType.Regular, 17, true, Message, true );
        }
    }
}