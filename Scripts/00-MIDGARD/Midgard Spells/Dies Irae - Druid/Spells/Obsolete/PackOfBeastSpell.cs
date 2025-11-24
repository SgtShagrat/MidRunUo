/***************************************************************************
 *                               PackOfBeastSpell.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class PackOfBeastSpell : DruidSpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Pack Of Beast", "En Sec Ohm Ess Sepa",
            266,
            9040,
            false,
            Reagent.BlackPearl,
            Reagent.Bloodmoss,
            Reagent.PetrifiedWood
            );

        private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( PackOfBeastSpell ),
            "Summons a pack of beasts to fight for the Druid. Spell length increases with skill.",
            "",
            2303
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Third; }
        }

        public PackOfBeastSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        private static Type[] m_Types = new Type[]
        {
            typeof( BrownBear ),
            typeof( TimberWolf ),
            typeof( Panther ),
            typeof( GreatHart ),
            typeof( Hind ),
            typeof( Alligator ),
            typeof( Boar ),
            typeof( GiantRat )
        };

        public override void OnCast()
        {
            if( CheckSequence() )
            {
                try
                {
                    Type beasttype = ( m_Types[ Utility.Random( m_Types.Length ) ] );

                    BaseCreature creaturea = (BaseCreature) Activator.CreateInstance( beasttype );
                    BaseCreature creatureb = (BaseCreature) Activator.CreateInstance( beasttype );
                    BaseCreature creaturec = (BaseCreature) Activator.CreateInstance( beasttype );
                    BaseCreature creatured = (BaseCreature) Activator.CreateInstance( beasttype );

                    SpellHelper.Summon( creaturea, Caster, 0x215, TimeSpan.FromSeconds( 4.0 * Caster.Skills[ CastSkill ].Value ), false, false );
                    SpellHelper.Summon( creatureb, Caster, 0x215, TimeSpan.FromSeconds( 4.0 * Caster.Skills[ CastSkill ].Value ), false, false );

                    Double morebeast;

                    morebeast = Utility.Random( 10 ) + ( Caster.Skills[ CastSkill ].Value * 0.1 );

                    if( morebeast > 11 )
                        SpellHelper.Summon( creaturec, Caster, 0x215, TimeSpan.FromSeconds( 4.0 * Caster.Skills[ CastSkill ].Value ), false, false );

                    if( morebeast > 18 )
                        SpellHelper.Summon( creatured, Caster, 0x215, TimeSpan.FromSeconds( 4.0 * Caster.Skills[ CastSkill ].Value ), false, false );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }

            FinishSequence();
        }

        public override TimeSpan GetCastDelay()
        {
            return TimeSpan.FromSeconds( 7.5 );
        }
    }
}