/***************************************************************************
 *                                  SerpentsHoldMine.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Xml;

using Midgard.Engines.SpellSystem;

using Server.Items;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;

namespace Server.Regions
{
    public class PaladineHeaven : BaseRegion
    {
        public PaladineHeaven( XmlElement xml, Map map, Region parent )
            : base( xml, map, parent )
        {
        }

        public override void OnEnter( Mobile m )
        {
            try
            {
                m.Poison = null;
                m.Paralyzed = false;

                EvilOmenSpell.CheckEffect( m );
                StrangleSpell.RemoveCurse( m );
                CorpseSkinSpell.RemoveCurse( m );
                CurseSpell.RemoveEffect( m );
                MortalStrike.EndWound( m );
                BloodOathSpell.RemoveCurse( m );
                MindRotSpell.ClearMindRotScalar( m );

                //DarkOmenSpell.CheckEffect( m );
                ChokingSpell.RemoveCurse( m );
                BloodConjunctionSpell.RemoveCurse( m );
                LobotomySpell.ClearMindRotScalar( m );

                BuffInfo.RemoveBuff( m, BuffIcon.Clumsy );
                BuffInfo.RemoveBuff( m, BuffIcon.FeebleMind );
                BuffInfo.RemoveBuff( m, BuffIcon.Weaken );
                BuffInfo.RemoveBuff( m, BuffIcon.MassCurse );

                m.Stam = m.StamMax;
                m.Hits = m.HitsMax;
                m.Mana = m.ManaMax;
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        public override bool AllowHarmful( Mobile from, Mobile target )
        {
            if( from.AccessLevel == AccessLevel.Player )
                from.SendMessage( "You may not do that here." );

            return ( from.AccessLevel > AccessLevel.Player );
        }

        public override bool AllowHousing( Mobile from, Point3D p )
        {
            return false;
        }

        public override bool OnCombatantChange( Mobile from, Mobile Old, Mobile New )
        {
            return ( from.AccessLevel > AccessLevel.Player );
        }
    }
}