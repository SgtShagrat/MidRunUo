using System;
using System.Collections;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Second
{
	public class ProtectionSpell : MagerySpell
	{
		private static Hashtable m_Registry = new Hashtable();
		public static Hashtable Registry { get { return m_Registry; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Protection", "Uus Sanct",
				236,
				9011,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public ProtectionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( Core.AOS )
				return true;

            #region mod by Dies Irae
            if( Targetable )
                return true;
            #endregion

			if ( m_Registry.ContainsKey( Caster ) )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				return false;
			}
			else if ( !Caster.CanBeginAction( typeof( DefensiveSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
				return false;
			}

			return true;
		}

		private static Hashtable m_Table = new Hashtable();

		public static void Toggle( Mobile caster, Mobile target )
		{
			/* Players under the protection spell effect can no longer have their spells "disrupted" when hit.
			 * Players under the protection spell have decreased physical resistance stat value (-15 + (Inscription/20),
			 * a decreased "resisting spells" skill value by -35 + (Inscription/20),
			 * and a slower casting speed modifier (technically, a negative "faster cast speed") of 2 points.
			 * The protection spell has an indefinite duration, becoming active when cast, and deactivated when re-cast.
			 * Reactive Armor, Protection, and Magic Reflection will stay on—even after logging out,
			 * even after dying—until you “turn them off” by casting them again.
			 */

			object[] mods = (object[])m_Table[target];

			if ( mods == null )
			{
				target.PlaySound( 0x1E9 );
				target.FixedParticles( 0x375A, 9, 20, 5016, EffectLayer.Waist );

				mods = new object[2]
					{
						new ResistanceMod( ResistanceType.Physical, -15 + Math.Min( (int)(caster.Skills[SkillName.Inscribe].Value / 20), 15 ) ),
						new DefaultSkillMod( SkillName.MagicResist, true, -35 + Math.Min( (int)(caster.Skills[SkillName.Inscribe].Value / 20), 35 ) )
					};

				m_Table[target] = mods;
				Registry[target] = 100.0;

				target.AddResistanceMod( (ResistanceMod)mods[0] );
				target.AddSkillMod( (SkillMod)mods[1] );

				int physloss = -15 + (int) (caster.Skills[SkillName.Inscribe].Value / 20);
				int resistloss = -35 + (int) (caster.Skills[SkillName.Inscribe].Value / 20);
				string args = String.Format("{0}\t{1}", physloss, resistloss);
				BuffInfo.AddBuff(target, new BuffInfo(BuffIcon.Protection, 1075814, 1075815, args.ToString()));
			}
			else
			{
				target.PlaySound( 0x1ED );
				target.FixedParticles( 0x375A, 9, 20, 5016, EffectLayer.Waist );

				m_Table.Remove( target );
				Registry.Remove( target );

				target.RemoveResistanceMod( (ResistanceMod)mods[0] );
				target.RemoveSkillMod( (SkillMod)mods[1] );

				BuffInfo.RemoveBuff(target, BuffIcon.Protection);
			}
		}

		public override void OnCast()
		{
            #region mod by Dies Irae
            if( Targetable )
            {
                Caster.Target = new InternalTarget( this );
                return;
            }
            #endregion

			if ( Core.AOS )
			{
				if ( CheckSequence() )
					Toggle( Caster, Caster );

				FinishSequence();
			}
			else
			{
				if ( m_Registry.ContainsKey( Caster ) )
				{
					Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				}
				else if ( !Caster.CanBeginAction( typeof( DefensiveSpell ) ) )
				{
					Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
				}
				else if ( CheckSequence() )
				{
					if ( Caster.BeginAction( typeof( DefensiveSpell ) ) )
					{
						double value = (int)(Caster.Skills[SkillName.EvalInt].Value + Caster.Skills[SkillName.Meditation].Value + Caster.Skills[SkillName.Inscribe].Value);
						value /= 4;

						if ( value < 0 )
							value = 0;
						else if ( value > 75 )
							value = 75.0;

						Registry.Add( Caster, value );
						new InternalTimer( Caster ).Start();

						Caster.FixedParticles( 0x375A, 9, 20, 5016, EffectLayer.Waist );
						Caster.PlaySound( 0x1ED );
					}
					else
					{
						Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
					}
				}

				FinishSequence();
			}
		}

	    public class InternalTimer : Timer
		{
			private Mobile m_Caster;

			public InternalTimer( Mobile caster ) : base( TimeSpan.FromSeconds( 0 ) )
			{
				double val = caster.Skills[SkillName.Magery].Value * 2.0;
				if ( val < 15 )
					val = 15;
				else if ( val > 240 )
					val = 240;

				m_Caster = caster;
				Delay = TimeSpan.FromSeconds( val );
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				ProtectionSpell.Registry.Remove( m_Caster );
				DefensiveSpell.Nullify( m_Caster );
			}
		}

		#region Mondain's Legacy
		public static void EndProtection( Mobile m )
		{
			if ( m_Table.Contains( m ) )
			{
				object[] mods = (object[]) m_Table[ m ];

				m_Table.Remove( m );
				Registry.Remove( m );

				m.RemoveResistanceMod( (ResistanceMod) mods[ 0 ] );
				m.RemoveSkillMod( (SkillMod) mods[ 1 ] );

				BuffInfo.RemoveBuff( m, BuffIcon.Protection );
			}
		}
		#endregion

	    #region mod by Dies Irae
        private static bool Targetable = true;

        public void Target( Mobile m )
        {
            if( !Caster.CanSee( m ) )
            {
                Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
            }
            else if( CheckBSequence( m ) )
            {
                SpellHelper.Turn( Caster, m );

                // mod by Dies Irae
                // http://forum.uosecondage.com/viewtopic.php?f=7&t=5980
                // AR bonus amounts (now 1-10, was 1-11)
                // int val = Math.Min( (int)( Caster.Skills[ SkillName.Magery ].Value / 10.0 + 1 ), 10 );

                int protBase = (int)( Caster.Skills[ SkillName.Magery ].Value / 6.0 ) + Utility.Dice( 2, 5, 0 );
                double scalar = 0.01 * ( 100 - ( m.ArmorRating * 2 ) );
                int val = (int)( protBase * scalar );

                if( SpellHelper.AddStatBonus( Caster, m, StatType.AR, val, SpellHelper.GetDuration( Caster, m ) ) )
                {
                    // doMobAnimation(usedon, 0x375A, 0x09, 0x14, 0x00, 0x00);
                    m.FixedParticles( 0x375A, 0x09, 0x14, 5027, EffectLayer.Waist );

                    m.PlaySound( 0x1F7 );
                }
                else
                {
                    if( Caster != m )
                        Caster.SendMessage( "The spell will not adhere to him at this time." );
                    else
                        Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
                }
            }

            FinishSequence();
        }

        public static void RemoveEffect( Mobile target, bool message )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.AR );

            StatMod mod = target.GetStatMod( name );

            if( mod != null )
            {
                target.RemoveStatMod( name );
                if( message )
                    target.SendMessage( "Your magical protection has been nullified." );
            }
        }

        protected class InternalTarget : Target
        {
            ProtectionSpell m_Owner;

            public InternalTarget( ProtectionSpell owner )
                : base( 10, false, TargetFlags.Beneficial )
            {
                m_Owner = owner;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is Mobile )
                {
                    m_Owner.Target( (Mobile)o );
                }
            }

            protected override void OnTargetFinish( Mobile from )
            {
                m_Owner.FinishSequence();
            }
        }
	    #endregion
	}
}
