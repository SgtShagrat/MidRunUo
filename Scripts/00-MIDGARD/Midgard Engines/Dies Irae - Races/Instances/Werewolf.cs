using System;

using Midgard.Engines.SpellSystem;

using Server;
using Server.Commands;
using Server.Engines.PartySystem;
using Server.Items;
using Server.Spells;

using PARTY = Server.Engines.PartySystem.Party;

namespace Midgard.Engines.Races
{
    internal class Werewolf : MidgardRace
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "Ulula", AccessLevel.Player, new CommandEventHandler( Grawl_OnCommand ) );
        }

        [Usage( "Ulula" )]
        [Description( "Permette di ululare durante la trasformazione del licantropo" )]
        public static void Grawl_OnCommand( CommandEventArgs e )
        {
            if( !Config.RaceMorphEnabled )
            {
                e.Mobile.SendMessage( "Questo potere e' stato disabilitato." );
                return;
            }

            Mobile from = e.Mobile;
            if( from == null || e.Length != 0 )
                return;

            if( from.Race == Core.Werewolf )
            {
                if( Morph.UnderTransformation( from ) )
                    from.PlaySound( 0xE6 );
            }
            else
                from.SendMessage( "You cannot do that now!" );
        }

        public Werewolf( int raceID, int raceIndex )
            : base( raceID, raceIndex, "Werewolf", "Werewolves", 400, 401, 402, 403 )
        {
        }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.Werewolf; } }

        public override bool ValidateHair( bool female, int itemID )
        {
            if( itemID == 0 )
                return true;

            if( ( female && itemID == 0x2048 ) || ( !female && itemID == 0x2046 ) )
                return false;	//Buns & Receeding Hair

            if( itemID >= 0x203B && itemID <= 0x203D )
                return true;

            if( itemID >= 0x2044 && itemID <= 0x204A )
                return true;

            return false;
        }

        public override int RandomHair( bool female )
        {
            switch( Utility.Random( 9 ) )
            {
                case 0: return 0x203B;	//Short
                case 1: return 0x203C;	//Long
                case 2: return 0x203D;	//Pony Tail
                case 3: return 0x2044;	//Mohawk
                case 4: return 0x2045;	//Pageboy
                case 5: return 0x2047;	//Afro
                case 6: return 0x2049;	//Pig tails
                case 7: return 0x204A;	//Krisna
                default: return ( female ? 0x2046 : 0x2048 );	//Buns or Receeding Hair
            }
        }

        public override bool ValidateFacialHair( bool female, int itemID )
        {
            if( itemID == 0 )
                return true;

            if( female )
                return false;

            if( itemID >= 0x203E && itemID <= 0x2041 )
                return true;

            if( itemID >= 0x204B && itemID <= 0x204D )
                return true;

            return false;
        }

        public override int RandomFacialHair( bool female )
        {
            if( female )
                return 0;

            int rand = Utility.Random( 7 );

            return ( ( rand < 4 ) ? 0x203E : 0x2047 ) + rand;
        }

        public override int ClipSkinHue( int hue )
        {
            if( hue < 1002 )
                return 1002;
            else if( hue > 1058 )
                return 1058;
            else
                return hue;
        }

        public override int RandomSkinHue()
        {
            return Utility.Random( 1002, 57 ) | 0x8000;
        }

        public override int ClipHairHue( int hue )
        {
            if( hue < 1102 )
                return 1102;
            else if( hue > 1149 )
                return 1149;
            else
                return hue;
        }

        public override int RandomHairHue()
        {
            return Utility.Random( 1102, 48 );
        }

        private static readonly MorphEntry[] m_MorphList = new MorphEntry[]
                                                               {
                                                                   new WolfMorphEntry()
        };

        internal class WolfMorphEntry : MorphEntry
        {
            public WolfMorphEntry()
                : base( 290, 0, "a werewolf", 5, 1, 0.0, false, true, false, true, TimeSpan.Zero, 230, 14, 19, false, 30, true, true, true, true )
            {
            }

            public override int GetArmorBonusByPack( Mobile from )
            {
                Party party = Party.Get( from );
                if( party == null )
                    return 0;

                int validPackMember = 0;

                foreach( PartyMemberInfo info in party.Members )
                {
                    if( info == null )
                        continue;

                    Mobile owner = info.Mobile;
                    if( owner.Race == Core.Werewolf && Morph.UnderTransformation( owner ) && owner.GetDistanceToSqrt( from ) < 16 )
                        validPackMember++;
                }

                if( from.PlayerDebug )
                    from.SendMessage( "Debug: party members: {0} - valid {1}", party.Members.Count, validPackMember );

                return validPackMember;
            }

            public override bool IsRestrictedItem( Item i )
            {
                return i is BasePotion || i is Bandage;
            }

            public override bool IsRestrictedSpell( Spell spell )
            {
                if( spell is MagerySpell )
                    return ( (MagerySpell)spell ).Circle >= SpellCircle.Sixth;
                if( spell is DruidSpell )
                    return ( (DruidSpell)spell ).Circle >= SpellCircle.Sixth;

                return false;
            }
        }

        public override MorphEntry[] GetMorphList()
        {
            return m_MorphList;
        }

        public static bool IsDay( Mobile from )
        {
            int hours, minutes;
            Clock.GetTime( from.Map, from.X, from.Y, out hours, out minutes );

            return hours >= 4 && hours < 22;
        }

        public static void EatRawFood( CookableFood food, Mobile from )
        {
            Food dummy = food.Cook();
            if( dummy == null )
                return;

            if( Food.FillHunger( from, dummy.FillFactor ) )
            {
                from.PlaySound( Utility.Random( 0x3A, 3 ) );

                if( from.Body.IsHuman && !from.Mounted )
                    from.Animate( 34, 5, 1, true, false, 0 );

                food.Consume();
                dummy.Delete();

                AddBlood( from );
            }
        }

        private static void AddBlood( IEntity target )
        {
            new Blood().MoveToWorld( target.Location, target.Map );

            int extraBlood = Utility.RandomMinMax( 0, 1 );

            for( int i = 0; i < extraBlood; i++ )
            {
                new Blood().MoveToWorld( new Point3D(
                    target.X + Utility.RandomMinMax( -1, 1 ),
                    target.Y + Utility.RandomMinMax( -1, 1 ),
                    target.Z ), target.Map );
            }
        }

        public override int InfravisionLevel { get { return 15; } }
        public override int InfravisionDuration { get { return 60; } }
    }
}