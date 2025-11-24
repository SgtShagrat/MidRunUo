/***************************************************************************
 *                               PaladinAttributes.cs
 *
 *   begin                : 10 ottobre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;

namespace Midgard.Engines.Classes
{
    [PropertyObject]
    public sealed class PaladinAttributes : BaseClassAttributes
    {
        private static readonly Type typeofSwordOfLight = typeof( SwordOfLightDefinition );
        private static readonly Type typeofBanishEvil = typeof( BanishEvilDefinition );
        private static readonly Type typeofBlessedDrops = typeof( BlessedDropsDefinition );
        private static readonly Type typeofHolyMount = typeof( HolyMountDefinition );
        private static readonly Type typeofHolyWill = typeof( HolyWillDefinition );
        private static readonly Type typeofInvulnerability = typeof( InvulnerabilityDefinition );
        private static readonly Type typeofOneEnemyOneShot = typeof( OneEnemyOneShotDefinition );
        private static readonly Type typeofPathToHeaven = typeof( PathToHeavenDefinition );
        private static readonly Type typeofSacredBeam = typeof( SacredBeamDefinition );
        private static readonly Type typeofSacredFeast = typeof( SacredFeastDefinition );
        private static readonly Type typeofShieldOfRighteousness = typeof( ShieldOfRighteousnessDefinition );
        private static readonly Type typeofLayOfHands = typeof( LayOfHandsDefinition );
        private static readonly Type typeofCurePoison = typeof( CurePoisonDefinition );
        private static readonly Type typeofChalmChaos = typeof( ChalmChaosDefinition );
        private static readonly Type typeofHolyCircle = typeof( HolyCircleDefinition );
        private static readonly Type typeofLegalThoughts = typeof( LegalThoughtsDefinition );

        public PaladinAttributes( ClassPlayerState state )
            : base( state )
        {
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int SwordOfLight
        {
            get { return GetLevel( typeofSwordOfLight ); }
            set { SetLevel( typeofSwordOfLight, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int BanishEvil
        {
            get { return GetLevel( typeofBanishEvil ); }
            set { SetLevel( typeofBanishEvil, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int BlessedDrops
        {
            get { return GetLevel( typeofBlessedDrops ); }
            set { SetLevel( typeofBlessedDrops, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int HolyMount
        {
            get { return GetLevel( typeofHolyMount ); }
            set { SetLevel( typeofHolyMount, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int HolyWill
        {
            get { return GetLevel( typeofHolyWill ); }
            set { SetLevel( typeofHolyWill, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Invulnerability
        {
            get { return GetLevel( typeofInvulnerability ); }
            set { SetLevel( typeofInvulnerability, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int OneEnemyOneShot
        {
            get { return GetLevel( typeofOneEnemyOneShot ); }
            set { SetLevel( typeofOneEnemyOneShot, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PathToHeaven
        {
            get { return GetLevel( typeofPathToHeaven ); }
            set { SetLevel( typeofPathToHeaven, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int SacredBeam
        {
            get { return GetLevel( typeofSacredBeam ); }
            set { SetLevel( typeofSacredBeam, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int SacredFeast
        {
            get { return GetLevel( typeofSacredFeast ); }
            set { SetLevel( typeofSacredFeast, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ShieldOfRighteousness
        {
            get { return GetLevel( typeofShieldOfRighteousness ); }
            set { SetLevel( typeofShieldOfRighteousness, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int LayOfHands
        {
            get { return GetLevel( typeofLayOfHands ); }
            set { SetLevel( typeofLayOfHands, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int CurePoison
        {
            get { return GetLevel( typeofCurePoison ); }
            set { SetLevel( typeofCurePoison, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ChalmChaos
        {
            get { return GetLevel( typeofChalmChaos ); }
            set { SetLevel( typeofChalmChaos, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int HolyCircle
        {
            get { return GetLevel( typeofHolyCircle ); }
            set { SetLevel( typeofHolyCircle, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int LegalThoughts
        {
            get { return GetLevel( typeofLegalThoughts ); }
            set { SetLevel( typeofLegalThoughts, value ); }
        }
    }
}