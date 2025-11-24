
using System;
using System.Collections.Generic;

using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class PetPowerScroll : SpecialScroll
    {
        public override string DefaultName
        {
            get
            {
                return "a pet power scroll";
            }
        }

        public override int Message { get { return 1049469; } } /* Using a scroll increases the maximum amount of a specific skill or your maximum statistics.
																* When used, the effect is not immediately seen without a gain of points with that skill or statistics.
																* You can view your maximum skill values in your skills window.
																* You can view your maximum statistic value in your statistics window. */
        public override int Title
        {
            get
            {
                double level = ( Value - 105.0 ) / 5.0;

                if( level >= 0.0 && level <= 3.0 && Value % 5.0 == 0.0 )
                    return 1049635 + (int)level;	/* Wonderous Scroll (105 Skill): OR
													* Exalted Scroll (110 Skill): OR
													* Mythical Scroll (115 Skill): OR
													* Legendary Scroll (120 Skill): */

                return 0;
            }
        }

        public override string DefaultTitle { get { return String.Format( "<basefont color=#FFFFFF>Pet Power Scroll ({0} Skill):</basefont>", Value ); } }

        private static SkillName[] m_Skills = new SkillName[]
			{
				SkillName.Magery,
				SkillName.EvalInt,
				SkillName.Meditation,
				SkillName.MagicResist,
				SkillName.Tactics,
				SkillName.Wrestling,
				SkillName.Anatomy
			};

        private static SkillName[] m_AOSSkills = new SkillName[]
			{
				SkillName.Chivalry,
				SkillName.Focus,
				SkillName.Necromancy,
				SkillName.Stealing,
				SkillName.Stealth,
				SkillName.SpiritSpeak
			};

        private static SkillName[] m_SESkills = new SkillName[]
			{
				SkillName.Ninjitsu,
				SkillName.Bushido
			};

        private static List<SkillName> _Skills = new List<SkillName>();

        public static List<SkillName> Skills
        {
            get
            {
                if( _Skills.Count == 0 )
                {
                    switch( Core.Expansion )
                    {
                        case Expansion.ML: _Skills.Add( SkillName.Spellweaving ); goto case Expansion.SE;
                        case Expansion.SE: _Skills.AddRange( m_SESkills ); goto case Expansion.AOS;
                        case Expansion.AOS: _Skills.AddRange( m_AOSSkills ); goto default;
                        default: _Skills.AddRange( m_Skills ); break;
                    }
                }

                return _Skills;
            }
        }

        public static PowerScroll CreateRandom( int min, int max )
        {
            #region mod by Dies Irae
            if( !Midgard.Misc.Midgard2Persistance.PowerScrollsEnabled )
                return null;
            #endregion

            min /= 5;
            max /= 5;

            return new PowerScroll( Skills[ Utility.Random( Skills.Count ) ], 100 + ( Utility.RandomMinMax( min, max ) * 5 ) );
        }

        public PetPowerScroll()
            : this( SkillName.Alchemy, 0.0 )
        {
        }

        [Constructable]
        public PetPowerScroll( SkillName skill, double value )
            : base( skill, value )
        {
            Hue = 0x481;

            if( Value == 105.0 )
                LootType = LootType.Regular;
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            double level = ( Value - 105.0 ) / 5.0;

            if( level >= 0.0 && level <= 3.0 && Value % 5.0 == 0.0 )
                list.Add( 1049639 + (int)level, GetNameLocalized() );	/* a wonderous scroll of ~1_type~ (105 Skill) OR
																		* an exalted scroll of ~1_type~ (110 Skill) OR
																		* a mythical scroll of ~1_type~ (115 Skill) OR
																		* a legendary scroll of ~1_type~ (120 Skill) */
            else
                list.Add( "a pet power scroll of {0} ({1} Skill)", GetName(), Value );
        }

        public PetPowerScroll( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
            else if( from.InRange( GetWorldLocation(), 1 ) )
            {

                from.SendMessage( "What pet would you like to use this on?" );
                from.Target = new PSTarget( this, Skill, Value );
            }
            else
            {
                from.SendLocalizedMessage( 500446 ); // That is too far away. 
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        private class PSTarget : Target
        {
            private PetPowerScroll m_Scroll;
            private SkillName m_Skill;
            private double m_Value;

            public PSTarget( PetPowerScroll charge, SkillName skill, double value )
                : base( 10, false, TargetFlags.None )
            {
                m_Scroll = charge;
                m_Skill = skill;
                m_Value = value;
            }

            protected override void OnTarget( Mobile from, object target )
            {
                if( target == from )
                    from.SendMessage( "You cant do that." );

                else if( target is BaseCreature )
                {
                    BaseCreature c = (BaseCreature)target;

                    if( c.Controlled == false )
                    {
                        from.SendMessage( "That Creature is not tamed." );
                    }
                    else if( c.ControlMaster != from )
                    {
                        from.SendMessage( "This is not your pet." );
                    }
                    else if( c.Controlled && c.ControlMaster == from )
                    {
                        Skill skill = c.Skills[ m_Skill ];

                        if( skill != null )
                        {
                            if( skill.Cap >= m_Value )
                            {
                                from.SendMessage( "Your pets {0} is to high for this powerscroll.", m_Skill );
                            }
                            else
                            {
                                from.SendMessage( "Your pets {0} has been caped at {1}.", m_Skill, m_Value );

                                skill.Cap = m_Value;

                                Effects.SendLocationParticles( EffectItem.Create( c.Location, c.Map, EffectItem.DefaultDuration ), 0, 0, 0, 0, 0, 5060, 0 );
                                Effects.PlaySound( from.Location, from.Map, 0x243 );

                                Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( c.X - 6, from.Y - 6, c.Z + 15 ), c.Map ), c, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );
                                Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( c.X - 4, from.Y - 6, c.Z + 15 ), c.Map ), c, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );
                                Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( c.X - 6, from.Y - 4, c.Z + 15 ), c.Map ), c, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );

                                Effects.SendTargetParticles( c, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100 );
                                m_Scroll.Delete();
                            }
                        }
                    }
                }
            }
        }
    }
}