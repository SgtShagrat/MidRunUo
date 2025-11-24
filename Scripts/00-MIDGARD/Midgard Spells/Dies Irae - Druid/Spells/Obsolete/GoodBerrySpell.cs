/***************************************************************************
 *                               GoodBerrySpell.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class GoodBerrySpell : DruidSpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Good Berry", "In Mani Oum",
            224,
            9011,
            Reagent.Ginseng,
            Reagent.SpringWater
            );

        private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( GoodBerrySpell ),
            "The druid creates a magical berry infused with natural energy.",
            "",
            2303
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.First; }
        }

        public GoodBerrySpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            if( CheckSequence() )
            {
                GoodBerry berry = new GoodBerry();

                Caster.AddToBackpack( berry );

                Caster.SendMessage( "Thou created a good berry." );

                Caster.FixedParticles( 0, 10, 5, 2003, EffectLayer.RightHand );
                Caster.PlaySound( 0x1E2 );
            }
            FinishSequence();
        }
    }
}