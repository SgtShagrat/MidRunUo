/***************************************************************************
 *                               StoneDefinition
 *                            ---------------------
 *   begin                : 27 gennaio, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Engines.StoneEnchantSystem
{
    [PropertyObject]
    public abstract class StoneDefinition
    {
        public static Type[] ArmorAndWeapons = new Type[] { typeof( BaseArmor ), typeof( BaseWeapon ) };

        [CommandProperty( AccessLevel.GameMaster, true )]
        public abstract string Name { get; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public abstract string Prefix { get; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public abstract string Suffix { get; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public abstract string NameIta { get; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public abstract string PrefixIta { get; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public abstract string SuffixIta { get; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public abstract int Hue { get; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public abstract StoneTypes StoneType { get; }

        public abstract Type[] TypesSupported { get; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public virtual int AttackLevelDamage { get { return 0; } }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public virtual int DefenceLevelDamage { get { return 0; } }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public virtual int ManaLevelForUse { get { return 1; } }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public virtual CustomResType ResistStoneType { get { return CustomResType.General; } }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public virtual int ResistLevel { get { return 0; } }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public abstract StoneAttributeFlag StoneFlag { get; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public virtual SkillName EnchantSkill { get { return SkillName.Magery; } }

        #region Instances
        public static readonly StoneDefinition Phoenix = new Phoenix();
        public static readonly StoneDefinition Fire = new Fire();
        public static readonly StoneDefinition Electric = new Electric();
        public static readonly StoneDefinition Cloud = new Cloud();
        public static readonly StoneDefinition Explosion = new Explosion();
        public static readonly StoneDefinition Lockmind = new Lockmind();
        public static readonly StoneDefinition Spectre = new Spectre();
        public static readonly StoneDefinition Serpent = new Serpent();
        public static readonly StoneDefinition Perforation = new Perforation();
        public static readonly StoneDefinition AntiMagical = new AntiMagical();
        public static readonly StoneDefinition Vampire = new Vampire();
        public static readonly StoneDefinition Mammoth = new Mammoth();
        public static readonly StoneDefinition Sonic = new Sonic();

        public static readonly StoneDefinition[] StoneDefinitions = new StoneDefinition[]
            {
                Phoenix,
                Fire,
                Electric,
                Cloud,
                Explosion,
                Lockmind,
                Spectre,
                Serpent,
                Perforation,
                AntiMagical,
                Vampire,
                Mammoth,
                Sonic,
            };
        #endregion

        public static StoneDefinition Parse( string name )
        {
            if( String.IsNullOrEmpty( name ) )
                return null;

            foreach( StoneDefinition t in StoneDefinitions )
            {
                if( Insensitive.Compare( name, t.Name ) == 0 )
                    return t;
            }

            return null;
        }

        public static StoneDefinition GetFromIndex( int index )
        {
            if( index < StoneDefinitions.Length && index > -1 )
                return StoneDefinitions[ index ];

            return null;
        }

        public virtual void OnHit( Mobile attacker, Mobile defender, Item hitFrom, Item hitTo, int baseDamage )
        {
            Mobile possessor, victim;
            Item possessorItem, victimItem;
            int magicDamage;

            if( !( hitFrom is IStoneEnchantItem ) )
                return;

            StoneEnchantItem state = StoneEnchantItem.Find( hitFrom );
            if( state == null )
                return;

            StoneDefinition definition = GetFromIndex( (int)state.StoneType - 1 );
            if( definition == null )
                return;

            if( hitFrom is BaseWeapon )
            {
                possessor = attacker;
                victim = defender;
                possessorItem = hitFrom;
                victimItem = hitTo;
                magicDamage = definition.AttackLevelDamage;
            }
            else
            {
                possessor = defender;
                victim = attacker;
                possessorItem = hitFrom;
                victimItem = hitTo;
                magicDamage = definition.DefenceLevelDamage;
            }

            int level = possessor.GetStonePower( (int)state.Definition.StoneFlag );

            int itemID = (int)possessor.Skills[ SkillName.ItemID ].Value;

            if( ( itemID / 2 ) + 25 < Utility.Random( 100 ) )
            {
                EndEffect( possessor, victim, possessorItem, victimItem );
                return;
            }

            int manaRequired = ( definition.ManaLevelForUse * level ) / 2;
            if( manaRequired < 1 )
                manaRequired = 1;

            if( possessor.Mana < manaRequired )
            {
                EndEffect( possessor, victim, possessorItem, victimItem );
                return;
            }

            possessor.Mana -= manaRequired;

            int resist = possessor.GetCustomResistance( definition.ResistStoneType );
            if( resist > 25 )
                resist = 25;

            int powerValue = Utility.Dice( level, magicDamage, 0 );
            int endValue = powerValue - (int)( powerValue * ( resist / 100.0 ) );

            if( resist > 0 )
            {
                victim.SendMessage( "You absorb some {0} damage [{1}%].", definition.Prefix, resist );
                possessor.SendMessage( "Your victim absorbs some {0} damage [{1}%].", definition.Prefix, resist );
            }

            if( possessor.PlayerDebug )
                possessor.SendMessage( "Debug: base: {0}, power {1}, resist {2}, final {3}", level, powerValue, resist, endValue );

            DoSpecialEffect( possessor, victim, possessorItem, victimItem, endValue );
        }

        public virtual void OnMiss( BaseRanged bow, Mobile attacker, Mobile defender )
        {
        }

        public virtual bool OnFired( BaseRanged bow, Mobile attacker, Mobile defender )
        {
            attacker.MovingEffect( defender, bow.EffectID, 18, 1, false, false, Hue, 0 );
            return false;
        }

        public virtual void EndEffect( Mobile attacker, Mobile defender, Item hitFrom, Item hitTo )
        {
            if( hitFrom is Arrow || hitFrom is Bolt )
                hitFrom.Delete();
        }

        public virtual void DoSpecialEffect( Mobile possessor, Mobile victim, Item possessorItem, Item victimItem, int damage )
        {
        }
    }
}