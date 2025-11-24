using Server;
using Server.Items;
using System;

using Midgard.Engines.Classes;

namespace Midgard.Items
{
	public abstract class BasePoisonAmmonition : Item, ISpecialAmmo
	{
		private Poison m_Poison;

		[CommandProperty( AccessLevel.GameMaster )]
		public /*abstract*/ Poison Poison
		{
			get{ return m_Poison; }
			set
			{
				m_Poison = value;

				if ( m_Poison.Level >= 39 )//Lentezza
					Hue = 0x3F;
				else if ( m_Poison.Level >= 34 )//Blocco
					Hue = 0x429;
				else if ( m_Poison.Level >= 29 )//Paralisi
					Hue = 0x35;
				else if ( m_Poison.Level >= 24 )//Stanchezza
					Hue = 0x21;
				else if ( m_Poison.Level >= 19 )//Magia
					Hue = 0x3;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double PoisonerSkill { get; set; }

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		//edit by arlas
		public override void OnAfterDuped( Item newItem )
		{
			if (PoisonerSkill == 0)
				PoisonerSkill = ((BasePoisonAmmonition)newItem).PoisonerSkill;
			else if ( ((BasePoisonAmmonition)newItem).PoisonerSkill == 0 )
				((BasePoisonAmmonition)newItem).PoisonerSkill = PoisonerSkill;
		}

		public BasePoisonAmmonition( int itemID ) : base( itemID )
		{
			Stackable = true;
			PoisonerSkill = 0.0;
			Hue = 1920;
		}

		public virtual bool CanPoison( Mobile from )
		{
			return Poison == null || from.Skills[ SkillName.Poisoning ].Value >= GetMinSkillToPoison( Poison );
		}

		public static double GetMinSkillToPoison( Poison poison )
		{
			if( poison == null )
				return 0.0;

			switch( poison.RealLevel )
			{
				case 0: return 30;  // Lesser
				case 1: return 45;  // Regular
				case 2: return 63;  // Great
				case 3: return 80;  // Deadly
				case 4: return 100; // Lethal
				default: return 100;
			}
		}

		#region ISpecialAmmo members
		public virtual void OnHit( BaseRanged baseRanged, Mobile attacker, Mobile defender, double damageBonus )
		{
			if ( !ClassSystem.IsScout( attacker ) && Poison != null && Poison.Level >= 19 && Poison.Level <= 43 )
			{
				attacker.SendMessage( (attacker.Language == "ITA" ? "Questa freccia non sembra efficace nelle tue mani.": "This arrow isn't so effective in your hands.") );
				return;
			}

			int scoutbonus = ClassSystem.IsScout( attacker ) ? 30 : 0;

			if( Poison != null && Utility.RandomDouble() <= ( PoisonerSkill + attacker.Skills[ SkillName.Poisoning ].Value + scoutbonus ) / ( 300.0 + defender.Skills[ SkillName.TasteID ].Value ) && CanPoison( attacker ) )
			{
				defender.ApplyPoison( attacker, Poison );
			}

			double frecciatrovata = ClassSystem.IsScout( attacker ) ? 0.8 : 0.4;

			if( !defender.Player && ( defender.Body.IsAnimal || defender.Body.IsMonster ) && frecciatrovata >= Utility.RandomDouble() )
			{
				Item item = (Item)Activator.CreateInstance( this.GetType() );
				BasePoisonAmmonition freccia = (BasePoisonAmmonition)item;
				freccia.Amount = 1;
				freccia.PoisonerSkill = PoisonerSkill;

				defender.AddToBackpack( freccia );
			}

			Consume();
		}

		public virtual void OnMiss( BaseRanged baseRanged, Mobile attacker, Mobile defender )
		{
			double tiro = Utility.RandomDouble();
			double frecciatrovata = ClassSystem.IsScout( attacker ) ? 0.8 : 0.4;
			double frecciafallita = ClassSystem.IsScout( attacker ) ? 0.05 : 0.1;

			if( tiro < frecciafallita )
			{
				//fallimento critico
				attacker.SendMessage( (attacker.Language == "ITA" ? "Che colpo orribile!": "What an awful shot!") );
				int range = tiro < 0.02 ? (tiro < 0.01 ? 4 : 3 ) : 2; 
				foreach( Mobile mobile in defender.GetMobilesInRange( range ) )
				{
					if( !attacker.CanBeHarmful( mobile ) || !attacker.InLOS( mobile ) )
						continue;

					if (mobile != defender && mobile != attacker )
					{
						//baseRanged.OnHit( attacker, mobile, this );
						if (mobile.Hits > 10)
							mobile.Hits -= 10;
						attacker.SendMessage( (attacker.Language == "ITA" ? "Hai colpito {0}!": "You hit {0}!"), mobile.Name );

						if ( mobile.Player )
						{
							mobile.SendMessage( (mobile.Language == "ITA" ? "Sei stato colpito per errore!": "You've been mistaken hit!") );
							if (attacker.CanAreaHarmful( mobile ))
								attacker.DoHarmful( mobile, true );

							//if ( mobile.Combatant == null )
							//	mobile.Combatant = attacker;
						}
						break;
					}
				}
			}
			else if( tiro < frecciatrovata )
			{
				Item item = (Item)Activator.CreateInstance( this.GetType() );
				BasePoisonAmmonition freccia = (BasePoisonAmmonition)item;
				freccia.Amount = 1;
				freccia.PoisonerSkill = PoisonerSkill;
				double t = Utility.RandomDouble()*2+0.5;
				Point3D target = new Point3D( attacker.X + (int)((defender.X + Utility.RandomMinMax( -1, 1 ) - attacker.X)*t), attacker.Y + (int)((defender.Y + Utility.RandomMinMax( -1, 1 ) -attacker.Y)*t), defender.Z);
				if (attacker.InLOS(target))
					freccia.MoveToWorld( target, defender.Map );
				else
					freccia.MoveToWorld( new Point3D( defender.X , defender.Y, defender.Z ), defender.Map );
			}
			Consume();
		}

		public virtual bool OnFired( BaseRanged baseRanged, Mobile attacker, Mobile defender )
		{
			attacker.MovingEffect( defender, baseRanged.EffectID, 18, 1, false, false, 1920, 0 );
			return true;
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if( Poison != null )
			{
				string veleno = (from.Language == "ITA" ? "veleno" : "poison");

				if ( m_Poison.Level >= 39 )//Lentezza
					veleno = (from.Language == "ITA" ? "veleno lentezza" : "slow poison");
				else if ( m_Poison.Level >= 34 )//Blocco
					veleno = (from.Language == "ITA" ? "veleno blocco" : "block poison");
				else if ( m_Poison.Level >= 29 )//Paralisi
					veleno = (from.Language == "ITA" ? "veleno paralisi" : "paralysis poison");
				else if ( m_Poison.Level >= 24 )//Stanchezza
					veleno = (from.Language == "ITA" ? "veleno stanchezza" : "fatigue poison");
				else if ( m_Poison.Level >= 19 )//Magia
					veleno = (from.Language == "ITA" ? "veleno magia" : "magic poison");
				else
					veleno = (from.Language == "ITA" ? "veleno" : "poison");

				switch( m_Poison.RealLevel )
				{
					case 0: veleno = (from.Language == "ITA" ? "["+veleno+" minore]" : "[lesser "+veleno+"]"); break;// Lesser
					case 1: veleno = (from.Language == "ITA" ? "["+veleno+"]" : "["+veleno+"]"); break;// Regular
					case 2: veleno = (from.Language == "ITA" ? "["+veleno+" maggiore]" : "[great "+veleno+"]"); break;// Great
					case 3: veleno = (from.Language == "ITA" ? "["+veleno+" mortale]" : "[deadly "+veleno+"]"); break;// Deadly
					case 4: veleno = (from.Language == "ITA" ? "["+veleno+" letale]" : "[lethal "+veleno+"]"); break;// Lethal
					default: veleno = "["+veleno+"]"; break;
				}
				LabelTo( from, veleno );//string.Format( from.Language == "ITA" ? "[avvelenato]" : "[Poisoned]" ) );
			}
		}

		#region serialization
		public BasePoisonAmmonition( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 ); // version

			Poison.Serialize( m_Poison, writer );

			writer.Write( PoisonerSkill );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version >= 1 )
				m_Poison = Poison.Deserialize( reader );

			PoisonerSkill = reader.ReadDouble();
		}
		#endregion
	}
}