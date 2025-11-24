/***************************************************************************
 *                               SkillSubCap.cs
 *
 *   begin                : 15 settembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.IO;

using Server;

namespace Midgard.Engines.SkillSystem
{
    public class SkillSubCap
    {
        public SkillName[] Skills { get; private set; }
        public int MaxValue { get; private set; }
        public string Name { get; private set; }

        public static SkillName[] m_SkillsCapped = new SkillName[] { SkillName.Magery, SkillName.Archery, SkillName.Swords, 
                                                                     SkillName.Macing, SkillName.Fencing, SkillName.MagicResist, SkillName.SpiritSpeak };

        private static readonly SkillSubCap[] m_SubCaps = new SkillSubCap[]
                                                          {
                                                              new SkillSubCap( "Mag-Arch-Swo", new SkillName[]{SkillName.Magery, SkillName.Archery, SkillName.Swords}, 150),
                                                              new SkillSubCap( "Mag-Arch-Mac", new SkillName[]{SkillName.Magery, SkillName.Archery, SkillName.Macing}, 150),
                                                              new SkillSubCap( "Mag-Arch-Fen", new SkillName[]{SkillName.Magery, SkillName.Archery, SkillName.Fencing}, 150),
                                                              new SkillSubCap( "Res-Spi", new SkillName[]{SkillName.MagicResist, SkillName.SpiritSpeak}, 150)
                                                          };

        public SkillSubCap( string name, SkillName[] skills, int maxValue )
        {
            Name = name;
            Skills = skills;
            MaxValue = maxValue;
        }

        public static bool IsUnderAnySubCap( SkillName skill )
        {
            return Array.LastIndexOf( m_SkillsCapped, skill ) > -1;
        }

        private bool IsUnderSubCap( SkillName skill )
        {
            return Array.LastIndexOf( Skills, skill ) > -1;
        }

        public static bool CanGainUnderSubCaps( Mobile m, SkillName skillName )
        {
            foreach( SkillSubCap subcap in m_SubCaps )
            {
                if( subcap.IsUnderSubCap( skillName ) && subcap.IsAtSubCap( m ) )
                {
                    if( m.PlayerDebug )
                        m.SendMessage( "Debug: you cannon gain any further in {0} because of a subcap ({1}, max {2}).", skillName, subcap, subcap.MaxValue );

                    return false;
                }
            }

            return true;
        }

        private bool IsAtSubCap( Mobile m )
        {
            return GetValueForThisCap( m ) >= MaxValue;
        }

        private bool IsViolatingSubCap( Mobile m )
        {
            return GetValueForThisCap( m ) > MaxValue;
        }

        private double GetValueForThisCap( Mobile m )
        {
            double sum = 0;
            foreach( SkillName skill in Skills )
                sum += m.Skills[ skill ].Base;

            return sum;
        }

        public override string ToString()
        {
            return Name;
        }

        public static void CheckLogin( Mobile m )
        {
            foreach( SkillSubCap subcap in m_SubCaps )
            {
                if( !subcap.IsViolatingSubCap( m ) )
                    continue;

                double diff = subcap.GetValueForThisCap( m ) - subcap.MaxValue;

                using( TextWriter tw = File.AppendText( Path.Combine( "Logs", "subcapViolations.log" ) ) )
                    tw.WriteLine( "{0} - Mobile {1} ( Serial {2} ) is violating {3} subcap. Difference: {4:F1}", DateTime.Now, m.Name, m.Serial.Value, subcap.Name, diff );
            }
        }
    }
}