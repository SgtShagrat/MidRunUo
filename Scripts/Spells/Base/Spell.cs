using System;

using Midgard.Engines.SpellSystem;

using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells.Second;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using System.Collections.Generic;
using Server.Spells.Spellweaving;
using Server.Spells.Bushido;

using EssenceOfWindSpell=Midgard.Engines.SpellSystem.EssenceOfWindSpell;
using ReaperFormSpell=Midgard.Engines.SpellSystem.ReaperFormSpell;
using ThunderstormSpell=Midgard.Engines.SpellSystem.ThunderstormSpell;

namespace Server.Spells
{
	public abstract class Spell : ISpell
	{
		private Mobile m_Caster;
		private Item m_Scroll;
		private SpellInfo m_Info;
		private SpellState m_State;
		private DateTime m_StartCastTime;

		public SpellState State{ get{ return m_State; } set{ m_State = value; } }
		public Mobile Caster{ get{ return m_Caster; } }
		public SpellInfo Info{ get{ return m_Info; } }
		public string Name{ get{ return m_Info.Name; } }
		public string Mantra{ get{ return m_Info.Mantra; } }
		public Type[] Reagents{ get{ return m_Info.Reagents; } }
		public Item Scroll{ get{ return m_Scroll; } }
		public DateTime StartCastTime { get { return m_StartCastTime; } }

        private static TimeSpan NextSpellDelay = TimeSpan.FromSeconds( Core.AOS ? 0.75 : 1.0 ); // mod by Dies Irae
        private static TimeSpan AnimateDelay = TimeSpan.FromSeconds( Core.AOS ? 1.5 : 1.1 ); // mod by Dies Irae

        public virtual double TimeoutDelay { get { return 30.0; } } // mod by Dies Irae

		public virtual SkillName CastSkill{ get{ return SkillName.Magery; } }
		public virtual SkillName DamageSkill{ get{ return SkillName.EvalInt; } }

		public virtual bool RevealOnCast{ get{ return true; } }
		public virtual bool DisruptOnCast{ get{ return true; } }
		public virtual bool ClearHandsOnCast{ get{ return true; } }
		public virtual bool ShowHandMovement{ get{ return true; } }

        public virtual bool DelayedDamage { get { return !Core.AOS; } } // mod by Dies Irae

        public virtual bool DelayedDamageStacking { get { return true; } }
        //In reality, it's ANY delayed Damage spell Post-AoS that can't stack, but, only 
        //Expo & Magic Arrow have enough delay and a short enough cast time to bring up 
        //the possibility of stacking 'em.  Note that a MA & an Explosion will stack, but
		//of course, two MA's won't.

		private static Dictionary<Type, DelayedDamageContextWrapper> m_ContextTable = new Dictionary<Type, DelayedDamageContextWrapper>();

		private class DelayedDamageContextWrapper
		{
			private Dictionary<Mobile, Timer> m_Contexts = new Dictionary<Mobile, Timer>();

			public void Add( Mobile m, Timer t )
			{
				Timer oldTimer;
				if( m_Contexts.TryGetValue( m, out oldTimer ) )
				{
					oldTimer.Stop();
					m_Contexts.Remove( m );
				}

				m_Contexts.Add( m, t );
			}

			public void Remove( Mobile m )
			{
				m_Contexts.Remove( m );
			}
		}

		public void StartDelayedDamageContext( Mobile m, Timer t )
		{
			if( DelayedDamageStacking )
				return; //Sanity

			DelayedDamageContextWrapper contexts;

			if( !m_ContextTable.TryGetValue( GetType(), out contexts ) )
			{
				contexts = new DelayedDamageContextWrapper();
				m_ContextTable.Add( GetType(), contexts );
			}

			contexts.Add( m, t );
		}

		public void RemoveDelayedDamageContext( Mobile m )
		{
			DelayedDamageContextWrapper contexts;

			if( !m_ContextTable.TryGetValue( GetType(), out contexts ) )
				return;

			contexts.Remove( m );
		}

		public Spell( Mobile caster, Item scroll, SpellInfo info )
		{
			m_Caster = caster;
			m_Scroll = scroll;
			m_Info = info;

            #region mod by Dies Irae [StrangeEffects]
            CheckHasStrangeEffect();
            #endregion
		}

		public virtual int GetNewAosDamage( int bonus, int dice, int sides, Mobile singleTarget )
		{
			if( singleTarget != null )
			{
				return GetNewAosDamage( bonus, dice, sides, (Caster.Player && singleTarget.Player), GetDamageScalar( singleTarget ) );
			}
			else
			{
				return GetNewAosDamage( bonus, dice, sides, false );
			}
		}

		public virtual int GetNewAosDamage( int bonus, int dice, int sides, bool playerVsPlayer )
		{
			return GetNewAosDamage( bonus, dice, sides, playerVsPlayer, 1.0 );
		}

		public virtual int GetNewAosDamage( int bonus, int dice, int sides, bool playerVsPlayer, double scalar )
		{
			int damage = Utility.Dice( dice, sides, bonus ) * 100;
			int damageBonus = 0;

			int inscribeSkill = GetInscribeFixed( m_Caster );
			int inscribeBonus = (inscribeSkill + (1000 * (inscribeSkill / 1000))) / 200;
			damageBonus += inscribeBonus;

			int intBonus = Caster.Int / 10;
			damageBonus += intBonus;

			int sdiBonus = AosAttributes.GetValue( m_Caster, AosAttribute.SpellDamage );
			
			#region Mondain's Legacy
			sdiBonus += ArcaneEmpowermentSpell.GetSpellBonus( m_Caster, playerVsPlayer );
			#endregion
			
            // mod by Dies Irae
            sdiBonus += DruidEmpowermentSpell.GetSpellBonus( m_Caster, playerVsPlayer );

			// PvP spell damage increase cap of 15% from an items magic property
			if ( playerVsPlayer && sdiBonus > 15 )
				sdiBonus = 15;

			damageBonus += sdiBonus;

			TransformContext context = TransformationSpellHelper.GetContext( Caster );

		    #region mod by Dies Irae
		    //if( context != null && context.Spell is ReaperFormSpell )
		    //    damageBonus += ((ReaperFormSpell)context.Spell).SpellDamageBonus;
		    if( context != null && context.Spell is ReaperFormSpell )
		        damageBonus += ( (ReaperFormSpell)context.Spell ).SpellDamageBonus;
		    #endregion

			damage = AOS.Scale( damage, 100 + damageBonus );

			int evalSkill = GetDamageFixed( m_Caster );
			int evalScale = 30 + ((9 * evalSkill) / 100);

			damage = AOS.Scale( damage, evalScale );

			damage = AOS.Scale( damage, (int)(scalar*100) );

			return damage / 100;
		}

		public virtual bool IsCasting{ get{ return m_State == SpellState.Casting; } }

        public virtual bool IsSequencing { get { return m_State == SpellState.Sequencing; } } // mod by Dies Irae

        public virtual void OnCasterDamaged( Mobile from, int damage )
        {
            OnCasterHurt();
        }

		public virtual void OnCasterHurt()
		{
			//Confirm: Monsters and pets cannot be disturbed.
			if ( !Caster.Player && Core.AOS ) // mod by Dies Irae
				return;

			if ( IsCasting )
			{
				object o = ProtectionSpell.Registry[m_Caster];
				bool disturb = true;

				if ( o != null && o is double )
				{
					if ( ((double)o) > Utility.RandomDouble()*100.0 )
						disturb = false;
				}

				if ( disturb || !Core.AOS )
					Disturb( DisturbType.Hurt, false, true );
			}
		}

		public virtual void OnCasterKilled()
		{
			Disturb( DisturbType.Kill );
		}

		public virtual void OnConnectionChanged()
		{
			FinishSequence();
		}

		public virtual bool OnCasterMoving( Direction d )
		{
			if ( m_Caster is BaseNecroFamiliar && ((BaseNecroFamiliar)m_Caster).Experience > 3 )
				return true;

			if ( IsCasting && BlocksMovement )
			{
				m_Caster.SendLocalizedMessage( 500111 ); // You are frozen and can not move.
				return false;
			}

			return true;
		}

		public virtual bool OnCasterEquiping( Item item )
		{
			if ( IsCasting || ( !Core.AOS && IsSequencing ) ) // mod by Dies Irae : pre-aos stuff
				Disturb( DisturbType.EquipRequest );

			return true;
		}

		public virtual bool OnCasterUsingObject( object o )
		{
			if ( m_State == SpellState.Sequencing )
				Disturb( DisturbType.UseRequest );

            if( Caster.PlayerDebug )
                Caster.SendMessage( "You are using {0}. Spell: {1} Spell state: {2}", o.GetType().Name, GetType().Name, m_State.ToString() );
			return true;
		}

		public virtual bool OnCastInTown( Region r )
		{
			return m_Info.AllowTown;
		}

		public virtual bool ConsumeReagents()
		{
			if ( m_Scroll != null || !m_Caster.Player )
				return true;

			if ( AosAttributes.GetValue( m_Caster, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
				return true;

			Container pack = m_Caster.Backpack;

			if ( pack == null )
				return false;

			if ( pack.ConsumeTotal( m_Info.Reagents, m_Info.Amounts ) == -1 )
				return true;

            #region mod by Dies Irae
            Container waistPack = m_Caster.Waistpack;

            if( waistPack == null )
                return false;

            if( waistPack.ConsumeTotal( m_Info.Reagents, m_Info.Amounts ) == -1 )
                return true;
            #endregion

			return false;
		}

		public virtual double GetInscribeSkill( Mobile m )
		{
			// There is no chance to gain
			// m.CheckSkill( SkillName.Inscribe, 0.0, 120.0 );

			return m.Skills[SkillName.Inscribe].Value;
		}

		public virtual int GetInscribeFixed( Mobile m )
		{
			// There is no chance to gain
			// m.CheckSkill( SkillName.Inscribe, 0.0, 120.0 );

			return m.Skills[SkillName.Inscribe].Fixed;
		}

		public virtual int GetDamageFixed( Mobile m )
		{
			//m.CheckSkill( DamageSkill, 0.0, m.Skills[DamageSkill].Cap );

			return m.Skills[DamageSkill].Fixed;
		}

		public virtual double GetDamageSkill( Mobile m )
		{
			//m.CheckSkill( DamageSkill, 0.0, m.Skills[DamageSkill].Cap );

			return m.Skills[DamageSkill].Value;
		}

		public virtual double GetResistSkill( Mobile m )
		{
			return m.Skills[SkillName.MagicResist].Value;
		}

		#region mod by Dies Irae
        public virtual double GetResistScalar( Mobile target )
        {
            /*
	        var resvalue:=cdbl((mag+evi-res)/2);
	        var perc:=cdbl(100.0 - resvalue); 
	        var bonusres := CustomSpellResist( defender, type);
	        perc := perc - bonusres;         
            var absorb:=cint(cdbl(dmg*perc/100.0)+0.5);
            */

            double res = target.Skills[ SkillName.MagicResist ].Value;
            double mag = Caster.Skills[ CastSkill ].Value;
            double evi = Caster.Skills[ DamageSkill ].Value;

            // resValue: from -50 to 100
            double resValue = 0.5 * ( mag + evi - res );

            // perc: from 150 to 0
            double perc = 100.0 - resValue;
            
            perc = perc / 100.0;

            // sanity
            if( perc > 1.0 )
                perc = 1.0;
            else if( perc < 0.0 )
                perc = 0.0;

            // TODO implementare la resistenza elementale.
            double bonusRes = 0.0;

            perc = perc - bonusRes;

            if( Caster.PlayerDebug )
                Caster.SendMessage( "Debug GetResistScalar: res {0:F2} - mag {1:F2} - evi {2:F2} - final scalar {3:F2}", res, mag, evi, perc );

            return perc;
        }
		#endregion

		public virtual double GetDamageScalar( Mobile target )
		{
			double scalar = 1.0;
			// mod by Dies Irae
			bool pvp = ( Caster.Player && target.Player );

			if( !Core.AOS )	//EvalInt stuff for AoS is handled elsewhere
			{
				double casterEI = m_Caster.Skills[DamageSkill].Value;
				double targetRS = target.Skills[SkillName.MagicResist].Value;

				/*
				if( Core.AOS )
					targetRS = 0;
				*/

				//m_Caster.CheckSkill( DamageSkill, 0.0, 120.0 );

				if( casterEI > targetRS )
					scalar = (1.0 + ((casterEI - targetRS) / 500.0));
				else
					scalar = (1.0 + ((casterEI - targetRS) / 200.0));

				// magery damage bonus, -25% at 0 skill, +0% at 100 skill, +5% at 100 skill
				// scalar += (m_Caster.Skills[CastSkill].Value - 100.0) / 400.0;
				double mageryScalar = ( 0.003 * m_Caster.Skills[ CastSkill ].Value ) - 0.25;

				#region mod by Dies Irae
				// inscription damage bonus, 0% at 0 skill, +10% at 100 skill
				int inscribeSkillFixed = GetInscribeFixed( m_Caster );
				int inscribeBonusFixed = ( inscribeSkillFixed + ( 1000 * ( inscribeSkillFixed / 1000 ) ) ) / 200;
				double inscriptionScalar = 0.01 * inscribeBonusFixed;

				// intelligence damage bonus, 0% at 0 skill, +10% at 100 skill
				// double intScalar = Caster.Int * 0.001;

				double total = scalar + mageryScalar + inscriptionScalar /* + intScalar */;

				if( Caster.PlayerDebug )
				{
					// Caster.SendMessage( "Debug spell damage: Bas: {0:F3} - Mag {1:F3} - Ins {2:F3} - Int {3:F3} - Tot {4:F3}", scalar, mageryScalar, inscriptionScalar, intScalar, total );
					Caster.SendMessage( "Debug spell damage: Bas: {0:F3} - Mag {1:F3} - Tot {2:F3}", scalar, mageryScalar, total );
				}

				scalar = total;
				#endregion
				if( !m_Caster.Player )//no bonus per mostri
					scalar = 1.0;

				if( !target.Player && !target.Body.IsHuman && !Core.AOS )
					scalar *= /* 2.0 */ 1.5; // Double magery damage to monsters/animals if not AOS
			}

			if ( target is BaseCreature )
				((BaseCreature)target).AlterDamageScalarFrom( m_Caster, ref scalar );

			if ( m_Caster is BaseCreature )
				((BaseCreature)m_Caster).AlterDamageScalarTo( target, ref scalar );

            #region mod by Dies Irae
            if( m_Caster is BaseCreature && ( (BaseCreature)m_Caster ).Summoned &&
                ( (BaseCreature)m_Caster ).SummonMaster != null && ( (BaseCreature)m_Caster ).SummonMaster.Player && target is PlayerMobile )
                scalar = 0.1;
            #endregion

			if( Core.SE )
				scalar *= GetSlayerDamageScalar( target );

			target.Region.SpellDamageScalar( m_Caster, target, ref scalar );

            /*
			if( Evasion.CheckSpellEvasion( target ) )	//Only single target spells an be evaded
				scalar = 0;
            */

            #region mod by Dies Irae
            TransformContext context = TransformationSpellHelper.GetContext( m_Caster );

            if( context != null && context.Spell is ReaperFormSpell )
            {
                double reaperScalar = ( (ReaperFormSpell)context.Spell ).SpellDamageBonus;
                scalar +=  reaperScalar / 100.0;
            }

            if( EssenceOfWindSpell.IsDebuffed( m_Caster ) )
            {
                double windScalar = EssenceOfWindSpell.GetSSIMalus( m_Caster );
                scalar -= windScalar / 100.0;
            }

            double druidEmpScalar = DruidEmpowermentSpell.GetSpellBonus( m_Caster, pvp );
            scalar += druidEmpScalar / 100.0;
            #endregion

            return scalar;
		}

		public virtual double GetSlayerDamageScalar( Mobile defender )
		{
			Spellbook atkBook = Spellbook.FindEquippedSpellbook( m_Caster );

			double scalar = 1.0;
			if( atkBook != null )
			{
				SlayerEntry atkSlayer = SlayerGroup.GetEntryByName( atkBook.Slayer );
				SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName( atkBook.Slayer2 );

				if( atkSlayer != null && atkSlayer.Slays( defender ) || atkSlayer2 != null && atkSlayer2.Slays( defender ) )
				{
					defender.FixedEffect( 0x37B9, 10, 5 );	//TODO: Confirm this displays on OSIs
					scalar = 2.0;
				}


				TransformContext context = TransformationSpellHelper.GetContext( defender );

				if( (atkBook.Slayer == SlayerName.Silver || atkBook.Slayer2 == SlayerName.Silver) && context != null && context.Type != typeof( HorrificBeastSpell ) )
					scalar +=.25; // Every necromancer transformation other than horrific beast take an additional 25% damage

				if( scalar != 1.0 )
					return scalar;
			}

			ISlayer defISlayer = Spellbook.FindEquippedSpellbook( defender );

			if( defISlayer == null )
				defISlayer = defender.Weapon as ISlayer;

			if( defISlayer != null )
			{
				SlayerEntry defSlayer = SlayerGroup.GetEntryByName( defISlayer.Slayer );
				SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName( defISlayer.Slayer2 );

				if( defSlayer != null && defSlayer.Group.OppositionSuperSlays( m_Caster ) || defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays( m_Caster ) )
					scalar = 2.0;
			}

			return scalar;
		}

		public virtual void DoFizzle()
		{
			m_Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502632 ); // The spell fizzles.

			if ( m_Caster.Player )
			{
				if ( Core.AOS )
					m_Caster.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );
				else
					m_Caster.FixedEffect( 0x3735, 6, 30 );

				m_Caster.PlaySound( 0x5C );

				#region mod by Dies Irae [Strange Effects]
                if( StrangeEffect == StrangeEffects.Explode )
                    Explode();

                if( m_Scroll != null && !m_Scroll.Deleted && m_Scroll is SpellScroll )
                    m_Scroll.Consume();
				#endregion
			}
			else if ( m_Caster is BaseCreature && ((BaseCreature)m_Caster).Controlled)
			{
				m_Caster.FixedEffect( 0x3735, 6, 30 );
				m_Caster.PlaySound( 0x5C );
			}
		}

		#region mod by Dies Irae [Strange Effects]
        public enum StrangeEffects
        {
            None = 0,
            Explode,
            RedirectToCaster,
            RedirectToRandom
        }

        private void CheckHasStrangeEffect()
        {
            if( m_Scroll is SpellScroll )
            {
                int per = GetScrollStrangeEffectChance();
                if( per <= 0 )
                    per = 1;
                else if( per >= 99 )
                    per = 99;

                if( PerCheck( per ) )
                {
                    StrangeEffect = GetRandomStrangeEffect();
                }
            }
        }

        public StrangeEffects GetRandomStrangeEffect()
        {
            return (StrangeEffects)Utility.RandomMinMax( 1, 3 );
        }

        public StrangeEffects StrangeEffect { get; set; }

        public static bool PerCheck( int chance )
        {
            if( chance >= 100 )
                chance = 99;

            if( chance <= 0 )
                chance = 0;

            return Utility.Random( 100 ) + 1 <= chance;
        }

	    public virtual int GetScrollStrangeEffectChance()
        {
            return 0;
        }

	    private void Explode()
        {
            Caster.SendMessage( "Whaa what's strange effect???" );

            int damage = Midgard.DiceRoll.Roll( "4d9+10" );

            Caster.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
            Caster.PlaySound( 0x307 );

            SpellHelper.Damage( this, Caster, damage, 0, 100, 0, 0, 0 );

            Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "* the scroll explode when you attepting to speak power words *" );
        }

        public bool HandleChangeTarget( Action<Mobile> target )
        {
            if( StrangeEffect == StrangeEffects.None )
                return false;

            Mobile newTarget = null;

            if( StrangeEffect == StrangeEffects.RedirectToRandom )
                newTarget = GetRandomNearbyPlayer();
            else if( StrangeEffect == StrangeEffects.RedirectToCaster )
                newTarget = Caster;

            if( newTarget == null || Caster == null )
                return false;

            string message = string.Format( "*the spell diverting on {0}*", newTarget == Caster ? "you" : newTarget.Name );       
            Caster.PrivateOverheadMessage( MessageType.Spell, 0x3B2, true, message, Caster.NetState );
            target.Invoke( Caster );

            return true;
        }

	    private Mobile GetRandomNearbyPlayer()
        {
            IPooledEnumerable eable = Caster.Map.GetMobilesInRange( Caster.Location );

            List<Mobile> validTargets = new List<Mobile>();

            foreach( Mobile m in eable )
            {
                if( m.Alive && Caster.CanSee( m ) && Caster.CanBeHarmful( m ) )
                    validTargets.Add( m );
            }

            eable.Free();

            return validTargets.Count > 0 ? validTargets[ Utility.Random( validTargets.Count ) ] : Caster;
        }
		#endregion

		private CastTimer m_CastTimer;
		private AnimTimer m_AnimTimer;

		public void Disturb( DisturbType type )
		{
			Disturb( type, true, false );
		}

		public virtual bool CheckDisturb( DisturbType type, bool firstCircle, bool resistable )
		{
			if ( resistable && m_Scroll is BaseWand )
				return false;

			return true;
		}

		public void Disturb( DisturbType type, bool firstCircle, bool resistable )
		{
			if ( !CheckDisturb( type, firstCircle, resistable ) )
				return;

			if ( m_State == SpellState.Casting )
			{
				if( !firstCircle && !Core.AOS && this is MagerySpell &&  ((MagerySpell)this).Circle == SpellCircle.First )
					return;

				m_State = SpellState.None;
				m_Caster.Spell = null;

				OnDisturb( type, true );

				if ( m_CastTimer != null )
					m_CastTimer.Stop();

				if ( m_AnimTimer != null )
					m_AnimTimer.Stop();

				if ( /* Core.AOS && */ ( m_Caster.Player || (m_Caster is BaseCreature && ((BaseCreature)m_Caster).Controlled )) && type == DisturbType.Hurt ) // mod by Dies Irae
					DoHurtFizzle();

				m_Caster.NextSpellTime = DateTime.Now + GetDisturbRecovery();
			}
			else if ( m_State == SpellState.Sequencing )
			{
				if( !firstCircle && !Core.AOS && this is MagerySpell &&  ((MagerySpell)this).Circle == SpellCircle.First )
					return;

				m_State = SpellState.None;
				m_Caster.Spell = null;

				OnDisturb( type, false );

				Targeting.Target.Cancel( m_Caster );

				if ( /* Core.AOS && */ m_Caster.Player && type == DisturbType.Hurt ) // mod by Dies Irae
					DoHurtFizzle();
			}
		}

		public virtual void DoHurtFizzle()
		{
			m_Caster.FixedEffect( 0x3735, 6, 30 );
			m_Caster.PlaySound( 0x5C );
		}

		public virtual void OnDisturb( DisturbType type, bool message )
		{
			if ( message )
				m_Caster.SendLocalizedMessage( 500641 ); // Your concentration is disturbed, thus ruining thy spell.
		}

		public virtual bool CheckCast()
		{
			return true;
		}

		public virtual void SayMantra()
		{
			if ( m_Scroll is BaseWand )
				return;

			if ( !string.IsNullOrEmpty( m_Info.Mantra ) && ( ( m_Caster is BaseCreature && ((BaseCreature)m_Caster).CanSpeakMantra ) || m_Caster.Player ) ) // mod by Dies Irae
				m_Caster.PublicOverheadMessage( MessageType.Spell, m_Caster.SpeechHue, true, m_Info.Mantra, false );
		}

	    #region mod by Dies Irae
        public virtual void OnAnimationStarted()
	    {
	    }
	    #endregion

		public virtual bool BlockedByHorrificBeast{ get{ return true; } }
		public virtual bool BlockedByAnimalForm{ get{ return true; } }
		public virtual bool BlocksMovement{ get{ return true; } }

		public virtual bool CheckNextSpellTime{ get{ return !(m_Scroll is BaseWand); } }

        #region mod by Dies Irae: pre-aos stuff
        public virtual bool DoEquipBlocksSpellCircle( bool message )
        {
            return false; 
        }

        public virtual int GetFatigueBySpellDamage( Mobile target, int damage )
        {
            return 0;
        }

	    public bool AbsorbReftectedSpell( Mobile reflector )
        {
            if( Caster != reflector && Caster.MagicDamageAbsorb > 0 )
            {
                if( Caster.Player )
                    Caster.SendMessage( "Your magic reflect spell saved you!" );

                Caster.MagicDamageAbsorb = 0;
                DefensiveSpell.Nullify( Caster );

                FinishSequence();

                return true;
            }

	        return false;
        }

	    public virtual bool ForceEndRessImmune
	    {
            get { return false; }
	    }
	    #endregion

        public bool Cast()
		{
			m_StartCastTime = DateTime.Now;

			if ( /* Core.AOS && */ m_Caster.Spell is Spell && ((Spell)m_Caster.Spell).State == SpellState.Sequencing )
				((Spell)m_Caster.Spell).Disturb( DisturbType.NewCast );

			if ( !m_Caster.CheckAlive() )
			{
				return false;
            }
            #region mod by Dies Irae: pre-aos stuff
            else if( m_Caster is Midgard2PlayerMobile && DoEquipBlocksSpellCircle( true ) )
            {
                return false;
            }
            #endregion
            #region Mondain's Legacy
            else if ( m_Caster is PlayerMobile  && ((PlayerMobile) m_Caster).Peaced )
			{
				m_Caster.SendLocalizedMessage( 1072060 ); // You cannot cast a spell while calmed.
			}
			#endregion
			else if ( m_Caster.Spell != null && m_Caster.Spell.IsCasting )
			{
				m_Caster.SendLocalizedMessage( 502642 ); // You are already casting a spell.
			}
			else if ( BlockedByHorrificBeast && TransformationSpellHelper.UnderTransformation( m_Caster, typeof( HorrificBeastSpell ) ) || ( BlockedByAnimalForm && AnimalForm.UnderTransformation( m_Caster ) ))
			{
				m_Caster.SendLocalizedMessage( 1061091 ); // You cannot cast that spell in this form.
            }
            #region mod by Dies Irae
            else if( BlockedByHorrificBeast && TransformationSpellHelper.UnderTransformation( m_Caster, typeof( EvilAvatarSpell ) ) )
            {
                m_Caster.SendLocalizedMessage( 1061091 ); // You cannot cast that spell in this form.
            }
            #endregion
            else if ( !(m_Scroll is BaseWand) && (m_Caster.Paralyzed || m_Caster.Frozen) )
			{
				m_Caster.SendLocalizedMessage( 502643 ); // You can not cast a spell while frozen.
			}
            else if( Core.AOS && CheckNextSpellTime && DateTime.Now < m_Caster.NextSpellTime )  // mod by Dies Irae
			{
				m_Caster.SendLocalizedMessage( 502644 ); // You have not yet recovered from casting a spell.
			}
			else if ( m_Caster is PlayerMobile && ( (PlayerMobile) m_Caster ).PeacedUntil > DateTime.Now )
			{
				m_Caster.SendLocalizedMessage( 1072060 ); // You cannot cast a spell while calmed.
			}
			else if ( m_Caster.Mana >= ScaleMana( GetMana() ) )
			{
				if ( m_Caster.Spell == null && m_Caster.CheckSpellCast( this ) && CheckCast() && m_Caster.Region.OnBeginSpellCast( m_Caster, this ) )
				{
					m_State = SpellState.Casting;
					m_Caster.Spell = this;

					if ( RevealOnCast )
						m_Caster.RevealingAction( DisruptOnCast );

				    try
				    {
				        SayMantra();
				    }
				    catch
				    {
				    }

					TimeSpan castDelay = this.GetCastDelay();

					if ( ShowHandMovement && m_Caster.Body.IsHuman )
					{
						int count = (int)Math.Ceiling( castDelay.TotalSeconds / AnimateDelay.TotalSeconds );

						if ( count != 0 )
						{
							m_AnimTimer = new AnimTimer( this, count );
							m_AnimTimer.Start();
						}

						if ( m_Info.LeftHandEffect > 0 )
							Caster.FixedParticles( 0, 10, 5, m_Info.LeftHandEffect, EffectLayer.LeftHand );

						if ( m_Info.RightHandEffect > 0 )
							Caster.FixedParticles( 0, 10, 5, m_Info.RightHandEffect, EffectLayer.RightHand );
					}

					if ( ClearHandsOnCast )
						m_Caster.ClearHands();

					if ( Core.ML )
						WeaponAbility.ClearCurrentAbility( m_Caster );

					m_CastTimer = new CastTimer( this, castDelay );
					m_CastTimer.Start();

					OnBeginCast();

					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				m_Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 502625 ); // Insufficient mana
			}

			return false;
		}

		public abstract void OnCast();

		public virtual void OnBeginCast()
		{
		}

		public virtual void GetCastSkills( out double min, out double max )
		{
			min = max = 0;	//Intended but not required for overriding.
		}

		public virtual bool CheckFizzle()
		{
			if( m_Scroll is BaseWand )
				return true;

			double minSkill, maxSkill;

			GetCastSkills( out minSkill, out maxSkill );

			if( DamageSkill != CastSkill )
				Caster.CheckSkill( DamageSkill, minSkill, maxSkill );// edit By Arlas, old:, 0.0, Caster.Skills[ DamageSkill ].Cap );

			bool success = Caster.CheckSkill( CastSkill, minSkill, maxSkill );

			#region mod by Dies Irae
			if( Caster != null && Caster.PlayerDebug )
				Caster.SendMessage( "Skills: m {0:F2} - M {1:F2} -- skill {2:F2} -- checkskill success {3}", minSkill, maxSkill, Caster.Skills[ CastSkill ].Value, success );
			#endregion

			return success;
		}

		public abstract int GetMana();

		public virtual int ScaleMana( int mana )
		{
			double scalar = 1.0;

			if ( !Necromancy.MindRotSpell.GetMindRotScalar( Caster, ref scalar ) )
				scalar = 1.0;

            #region mod by Dies Irae
            if( !LobotomySpell.GetMindRotScalar( Caster, ref scalar ) )
                scalar = 1.0;
            #endregion
            
			// Lower Mana Cost = 40%
			int lmc = AosAttributes.GetValue( m_Caster, AosAttribute.LowerManaCost );
			if ( lmc > 40 )
				lmc = 40;

			scalar -= (double)lmc / 100;

			return (int)(mana * scalar);
		}

		public virtual TimeSpan GetDisturbRecovery()
		{
			if ( Core.AOS )
				return TimeSpan.Zero;

			double delay = 1.0 - Math.Sqrt( (DateTime.Now - m_StartCastTime).TotalSeconds / GetCastDelay().TotalSeconds );

			if ( delay < 0.2 )
				delay = 0.2;

			return TimeSpan.FromSeconds( delay );
		}

		public virtual int CastRecoveryBase{ get{ return 6; } }
		public virtual int CastRecoveryFastScalar{ get{ return 1; } }
		public virtual int CastRecoveryPerSecond{ get{ return 4; } }
		public virtual int CastRecoveryMinimum{ get{ return 0; } }

		public virtual TimeSpan GetCastRecovery()
		{
            #region mod by Dies Irae
            if( !Core.AOS )
                return NextSpellDelay;
            #endregion

		    int fcr = AosAttributes.GetValue( m_Caster, AosAttribute.CastRecovery );

		    #region mod by Dies Irae
		    //fcr -= ThunderstormSpell.GetCastRecoveryMalus( m_Caster );
		    #endregion

			int fcrDelay = -(CastRecoveryFastScalar * fcr);

			int delay = CastRecoveryBase + fcrDelay;

			if ( delay < CastRecoveryMinimum )
				delay = CastRecoveryMinimum;

			return TimeSpan.FromSeconds( (double)delay / CastRecoveryPerSecond );
		}



		public abstract TimeSpan CastDelayBase { get; }

		public virtual double CastDelayFastScalar { get { return 1; } }
		public virtual double CastDelaySecondsPerTick { get { return 0.25; } }
		public virtual TimeSpan CastDelayMinimum { get { return TimeSpan.FromSeconds( 0.25 ); } }

		//public virtual int CastDelayBase{ get{ return 3; } }
		//public virtual int CastDelayFastScalar{ get{ return 1; } }
		//public virtual int CastDelayPerSecond{ get{ return 4; } }
		//public virtual int CastDelayMinimum{ get{ return 1; } }

		public virtual TimeSpan GetCastDelay()
		{
			if ( m_Scroll is BaseWand )
				return TimeSpan.Zero;

			// Faster casting cap of 2 (if not using the protection spell) 
			// Faster casting cap of 0 (if using the protection spell) 
			// Paladin spells are subject to a faster casting cap of 4 
			// Paladins with magery of 70.0 or above are subject to a faster casting cap of 2 
			int fcMax = 4;

			if ( CastSkill == SkillName.Magery || CastSkill == SkillName.Necromancy || ( CastSkill == SkillName.Chivalry && m_Caster.Skills[SkillName.Magery].Value >= 70.0 ) )
				fcMax = 2;

			int fc = AosAttributes.GetValue( m_Caster, AosAttribute.CastSpeed );

			if ( fc > fcMax )
				fc = fcMax;

			if ( ProtectionSpell.Registry.Contains( m_Caster ) )
				fc -= 2;

		    #region mod by Dies Irae
		    //if( EssenceOfWindSpell.IsDebuffed( m_Caster ) )
		    //    fc -= EssenceOfWindSpell.GetFCMalus( m_Caster );
		    #endregion

			TimeSpan baseDelay = CastDelayBase;

			TimeSpan fcDelay = TimeSpan.FromSeconds( -(CastDelayFastScalar * fc * CastDelaySecondsPerTick) );

			//int delay = CastDelayBase + circleDelay + fcDelay;
			TimeSpan delay = baseDelay + fcDelay;

			if ( delay < CastDelayMinimum )
				delay = CastDelayMinimum;
				
			#region Mondain's Legacy
			if ( DreadHorn.IsUnderInfluence( m_Caster ) )
				delay.Add( delay );
			#endregion

			//return TimeSpan.FromSeconds( (double)delay / CastDelayPerSecond );
			return delay;
		}

        #region mod by Dies Irae
        public virtual double GetOldCastDelayBonus()
        {
            double delay = 0.0;

            if( EssenceOfWindSpell.IsDebuffed( Caster ) )
                delay += EssenceOfWindSpell.GetFCMalus( Caster );

            return delay;
        }

        public virtual bool HandleSelfMagicalAbsorption( Mobile target )
        {
            if( Caster == null || target == null )
                return false;

            if( target == Caster && Caster.MagicDamageAbsorb > 0 )
            {
                Caster.MagicDamageAbsorb = 0;
                Caster.SendMessage( "Your magic reflect spell saved you!" );
                return true;
            }

            return false;
        }
        #endregion

		public virtual void FinishSequence()
		{
			m_State = SpellState.None;

			if ( m_Caster.Spell == this )
				m_Caster.Spell = null;
		}

		public virtual int ComputeKarmaAward()
		{
			return 0;
		}

		public virtual bool CheckSequence()
		{
			int mana = ScaleMana( GetMana() );

			if ( m_Caster.Deleted || !m_Caster.Alive || m_Caster.Spell != this || m_State != SpellState.Sequencing )
			{
				DoFizzle();
			}
			else if ( m_Scroll != null && !(m_Scroll is Runebook) && (m_Scroll.Amount <= 0 || m_Scroll.Deleted || m_Scroll.RootParent != m_Caster || (m_Scroll is BaseWand && (((BaseWand)m_Scroll).Charges <= 0 || m_Scroll.Parent != m_Caster))) )
			{
				DoFizzle();
			}
			else if ( !ConsumeReagents() )
			{
				m_Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 502630 ); // More reagents are needed for this spell.
			}
			else if ( m_Caster.Mana < mana )
			{
				m_Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 502625 ); // Insufficient mana for this spell.
			}
			else if ( Core.AOS && (m_Caster.Frozen || m_Caster.Paralyzed) )
			{
				m_Caster.SendLocalizedMessage( 502646 ); // You cannot cast a spell while frozen.
				DoFizzle();
			}
			else if ( m_Caster is PlayerMobile && ((PlayerMobile) m_Caster).PeacedUntil > DateTime.Now )
			{
				m_Caster.SendLocalizedMessage( 1072060 ); // You cannot cast a spell while calmed.
				DoFizzle();
			}
			#region Mondain'e Legacy
			else if ( m_Caster is PlayerMobile && ( (PlayerMobile) m_Caster ).PeacedUntil > DateTime.Now )
			{
				DoFizzle();
			}
			#endregion
			else if ( CheckFizzle() )
			{
				m_Caster.Mana -= mana;

				if ( m_Scroll is SpellScroll )
					m_Scroll.Consume();
				else if ( m_Scroll is BaseWand )
					((BaseWand)m_Scroll).ConsumeCharge( m_Caster );

				if ( m_Scroll is BaseWand )
				{
					bool m = m_Scroll.Movable;

					m_Scroll.Movable = false;

					if ( ClearHandsOnCast )
						m_Caster.ClearHands();

					m_Scroll.Movable = m;
				}
                else if( Core.AOS )  // mod by Dies Irae
                {
                    if( ClearHandsOnCast )
                        m_Caster.ClearHands();
                }

				int karma = ComputeKarmaAward();

				if ( karma != 0 )
					Misc.Titles.AwardKarma( Caster, karma, true );

                /*
                if( TransformationSpellHelper.UnderTransformation( m_Caster, typeof( VampiricEmbraceSpell ) ) || Caster.Race == Midgard.Engines.Races.Core.Vampire ) // mod by Dies Irae
				{
					bool garlic = false;

					for ( int i = 0; !garlic && i < m_Info.Reagents.Length; ++i )
						garlic = ( m_Info.Reagents[i] == Reagent.Garlic );

					if ( garlic )
					{
						m_Caster.SendLocalizedMessage( 1061651 ); // The garlic burns you!
						AOS.Damage( m_Caster, Utility.RandomMinMax( 17, 23 ), 100, 0, 0, 0, 0 );
					}
				}
                */

				return true;
			}
			else
			{
				DoFizzle();
			}

			return false;
		}

		public bool CheckBSequence( Mobile target )
		{
			return CheckBSequence( target, false );
		}

		public bool CheckBSequence( Mobile target, bool allowDead )
		{
			if ( !target.Alive && !allowDead )
			{
				m_Caster.SendLocalizedMessage( 501857 ); // This spell won't work on that!
				return false;
			}
			else if ( Caster.CanBeBeneficial( target, true, allowDead ) && CheckSequence() )
			{
				Caster.DoBeneficial( target );
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool CheckHSequence( Mobile target )
		{
			if ( !target.Alive )
			{
				m_Caster.SendLocalizedMessage( 501857 ); // This spell won't work on that!
				return false;
			}
			else if ( Caster.CanBeHarmful( target ) && CheckSequence() )
			{
				Caster.DoHarmful( target );
				return true;
			}
			else
			{
				return false;
			}
		}

		private class AnimTimer : Timer
		{
			private Spell m_Spell;

			public AnimTimer( Spell spell, int count ) : base( TimeSpan.Zero, AnimateDelay, count )
			{
				m_Spell = spell;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if ( m_Spell.State != SpellState.Casting || m_Spell.m_Caster.Spell != m_Spell )
				{
					Stop();
					return;
				}

				if ( !m_Spell.Caster.Mounted && m_Spell.Caster.Body.IsHuman && m_Spell.m_Info.Action >= 0 )
					m_Spell.Caster.Animate( m_Spell.m_Info.Action, 7, 1, true, false, 0 );
				else if (m_Spell.Caster.Body.IsHuman && m_Spell.Caster.Mounted)
					m_Spell.Caster.Animate( 27 + Utility.Random(2) , 7, 1, true, false, 0 ); // mod by Magius(CHE): perform crossbow action
				else if (!m_Spell.Caster.Body.IsHuman)
					m_Spell.Caster.Animate( 4 + Utility.Random(3) , 4, 1, true, false, 0 ); // mod by Magius(CHE): perform attack action

                m_Spell.OnAnimationStarted(); // mod by Dies Irae

				if ( !Running )
					m_Spell.m_AnimTimer = null;
			}
		}

        private class CastTimer : Timer
		{
			private Spell m_Spell;

			public CastTimer( Spell spell, TimeSpan castDelay ) : base( castDelay )
			{
				m_Spell = spell;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if ( m_Spell.m_State == SpellState.Casting && m_Spell.m_Caster.Spell == m_Spell )
				{
					m_Spell.m_State = SpellState.Sequencing;
					m_Spell.m_CastTimer = null;
					m_Spell.m_Caster.OnSpellCast( m_Spell );
					m_Spell.m_Caster.Region.OnSpellCast( m_Spell.m_Caster, m_Spell );
					m_Spell.m_Caster.NextSpellTime = DateTime.Now + m_Spell.GetCastRecovery();// Spell.NextSpellDelay;

					Target originalTarget = m_Spell.m_Caster.Target;

					m_Spell.OnCast();

					if ( m_Spell.m_Caster.Player && m_Spell.m_Caster.Target != originalTarget && m_Spell.Caster.Target != null )
						m_Spell.m_Caster.Target.BeginTimeout( m_Spell.m_Caster, TimeSpan.FromSeconds( Core.AOS ? 30.0 : m_Spell.TimeoutDelay ) );

					m_Spell.m_CastTimer = null;
				}
			}
		}
	}
}
