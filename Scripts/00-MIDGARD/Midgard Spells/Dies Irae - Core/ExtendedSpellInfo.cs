using System;
using Server;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class ExtendedSpellInfo
    {
        /// <summary>
        /// Type of our owner spell
        /// </summary>
        public Type SpellTypeName { get; set; }

        /// <summary>
        /// Name of Spell
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Spell description
        /// </summary>
        public TextDefinition Description { get; private set; }

        /// <summary>
        /// Spell description in italian
        /// </summary>
        public TextDefinition DescriptionIta { get; private set; }

        /// <summary>
        /// Spell description in italian for the staff
        /// </summary>
        public TextDefinition DescriptionStaff { get; private set; }

        /// <summary>
        /// Icon GumpID of spell Icon
        /// </summary>
        public int SpellIcon { get; private set; }

        /// <summary>
        /// Mana required
        /// </summary>
        public int Mana { get; private set; }

        /// <summary>
        /// Skill required
        /// </summary>
        public double Skill { get; private set; }

        /// <summary>
        /// String of abreviations of Reagents required, seperated by commas(From following list) (5 max)
        /// </summary>
        public string Reagents { get; private set; }

        /// <summary>
        /// School of our spell
        /// </summary>
        public SchoolFlag School { get; private set; }

        private static readonly object[] m_Params = new object[ 2 ];

        public ExtendedSpellInfo( Type t, TextDefinition desc, TextDefinition descIta, int icon )
            : this( t, null, desc, descIta, icon )
        {
        }

        public ExtendedSpellInfo( Type t, string name, TextDefinition desc, TextDefinition descIta, int icon )
            : this( t, name, desc, descIta, descIta, icon )
        {
        }

        public ExtendedSpellInfo( Type t, string name, TextDefinition desc, TextDefinition descIta, TextDefinition descStaff, int icon )
        {
            SpellTypeName = t;

            Spell spell = null;
            m_Params[ 0 ] = null;
            m_Params[ 1 ] = null;

            try
            {
                spell = (Spell)Activator.CreateInstance( t, m_Params );
            }
            catch( Exception ex )
            {
                Console.WriteLine( "ExtendedSpellInfo exception:" );
                Console.WriteLine( ex.ToString() );
            }

            if( spell != null )
            {
                Name = string.IsNullOrEmpty( name ) ? spell.Name : name;

                Description = desc;
                DescriptionIta = descIta;
                DescriptionStaff = descStaff;

                SpellIcon = icon;
                Mana = spell.GetMana();

                if( spell is ICustomSpell )
                    Skill = ( (ICustomSpell)spell ).RequiredSkill;
                else
                {
                    double min, max;
                    spell.GetCastSkills( out min, out max );

                    Skill = min;
                }

                if( spell is RPGPaladinSpell )
                    Reagents = string.Format( "T.{0}", ( (RPGPaladinSpell)spell ).GetTithes() );
                else if( spell.Info.Reagents != null )
                    Reagents = Reagent.PackRegs( spell.Info.Reagents );
                else
                    Reagents = String.Empty;

                if( spell is ICustomSpell )
                    School = ( (ICustomSpell)spell ).SpellSchool;
            }
            else
                Console.WriteLine( "Warning: spell {0} has an invalid extended info.", t.Name );
        }
    }
}