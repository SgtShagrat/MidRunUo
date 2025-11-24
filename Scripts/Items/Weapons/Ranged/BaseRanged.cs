using System;
using System.Collections.Generic;

using Midgard;
using Midgard.Engines.Classes;
using Midgard.Engines.StoneEnchantSystem;
using Midgard.Items;

using Server.Menus.ItemLists;
using Server.Network;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items
{
	public enum ElementalEffects
	{
		None = 0,
		Water = 0x3D6D,
		Air = 0x3D71,
		Light = 0x3D75,
		Earth = 0x3D79,
		Lightning = 0x3D7D,
		Fire = 0x3D82
	}

	public abstract class BaseRanged : BaseMeleeWeapon
	{
		public abstract int EffectID{ get; }
		public abstract Type AmmoType{ get; }

		#region mod by Dies Irae
		public virtual Item Ammo
		{
			get { return Loot.Construct( CustomAmmoType == AmmoType ? AmmoType : CustomAmmoType ); }
		}

		public override void AlterDamageByDistance( Mobile attacker, Mobile defender, ref int percentageBonus )
		{
			if( attacker.Player && attacker.GetDistanceToSqrt( defender ) < 2 )
				percentageBonus -= 20;
		}

		public override bool CanEquip( Mobile from )
		{
			foreach( Item obj in from.Items )
			{
				if( (obj is BaseArmor) && 
				  (( (BaseArmor)obj ).MaterialType == ArmorMaterialType.Plate || ( (BaseArmor)obj ).MaterialType == ArmorMaterialType.Dragon ))
				{
					from.SendMessage( (from.Language == "ITA" ? "Non puoi indossare quest'arma per via dell'armatura pesante." : "You cannot equip this weapon because of your heavy armor.") );
					return false;
				}
			}

			if( from.Skills[ SkillName.Magery ].Base >= 50.0 )
			{
				from.SendMessage( (from.Language == "ITA" ? "Non puoi indossare quest'arma per via della tua forza magica." : "You cannot equip this weapon because of your magical power.") );
				return false;
			}

			return base.CanEquip( from );
		}
		#endregion

		public override int DefHitSound{ get{ return 0x234; } }
		public override int DefMissSound{ get{ return 0x238; } }

		public override SkillName DefSkill{ get{ return SkillName.Archery; } }
		public override WeaponType DefType{ get{ return WeaponType.Ranged; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootXBow; } }

		public override SkillName AccuracySkill{ get{ return SkillName.Archery; } }

		private Timer m_RecoveryTimer; // so we don't start too many timers
		private bool m_Balanced;
		private int m_Velocity;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Balanced
		{
			get{ return m_Balanced; }
			set{ m_Balanced = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Velocity
		{
			get{ return m_Velocity; }
			set{ m_Velocity = value; InvalidateProperties(); }
		}

		#region mod by Dies Irae : pre-aos stuff
		private AmnoEntry m_LastAmnoEntry;

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual Type CustomAmmoType
		{
			get
			{
				Mobile parent = Parent as Mobile;
				if( parent == null || m_LastAmnoEntry == null || !CanFireArrowByTypeHue( m_LastAmnoEntry.Type, parent, m_LastAmnoEntry.Hue ) )
					return AmmoType;

				return m_LastAmnoEntry.Type;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( from != null && from.Alive && from.Weapon == this )
				DisplayAmmoGump( from );
		}

		private void ChangeAmno( Mobile m, Type t )
		{
			AmnoEntry entry = GetEntryByType( m, t );
			if( entry != null )
				m_LastAmnoEntry = entry;

			if( m != null && entry != null )
				m.SendMessage( ( m.Language == "ITA" ? "Questo arco lancerà {0}." : "This bow will shot {0}." ), entry.Name );
		}

		private AmnoEntry GetEntryByType( Mobile m, Type t )
		{
			AmnoEntry[] entries = GetAmmoEntries( m );
			if( entries == null )
				return null;

			foreach( AmnoEntry entry in entries )
			{
				if( entry.Type == t )
					return entry;
			}

			return entries[ 0 ];
		}

		private AmnoEntry[] GetAmmoEntries( Mobile m )
		{
			List<AmnoEntry> list = new List<AmnoEntry>();

			AmnoEntry[] entries = GetAmmoEntries();
			if( entries != null )
				list.AddRange( entries );

			entries = GetSpecialPoisonAmmoEntries( m );
			if( entries != null )
				list.AddRange( entries );

			entries = GetSpecialAmmoEntries( m );
			if( entries != null )
				list.AddRange( entries );

			return list.ToArray();
		}

		private AmnoEntry[] GetAmmoEntries()
		{
			if( AmmoType == typeof( Arrow ) )
				return m_ArrowAmmoTypes;
			else if( AmmoType == typeof( Bolt ) )
				return m_BoltAmmoTypes;

			return null;
		}

		public virtual AmnoEntry[] GetSpecialPoisonAmmoEntries( Mobile m )
		{
			List<AmnoEntry> list = new List<AmnoEntry>();

			if( m == null || ClassSystem.IsScout( m ) )
				list.AddRange( BaseScoutArrow.ScoutAmmoTypes );

			return list.ToArray();
		}

		public virtual AmnoEntry[] GetSpecialAmmoEntries( Mobile m )
		{
			List<AmnoEntry> list = new List<AmnoEntry>();

			if( m == null || ClassSystem.IsScout( m ) )
			{
				if( AmmoType == typeof( Arrow ) )
					list.AddRange( m_ArrowSAmmoTypes );
				else if( AmmoType == typeof( Bolt ) )
					list.AddRange( m_BoltSAmmoTypes );
			}

			return list.ToArray();
		}

		#region standard entries
		private static readonly AmnoEntry[] m_BoltSAmmoTypes = new AmnoEntry[]
		{
			new AmnoEntry(typeof (LentezzaLesserPoisonedBolt), 0x1BFB, 0x3F, "dardi lentezza minore", 30.0, 5.0, 0xF42, 0x3F),
			new AmnoEntry(typeof (LentezzaNormalPoisonedBolt), 0x1BFB, 0x3F, "dardi lentezza", 50.0, 6.0, 0xF42, 0x3F),
			new AmnoEntry(typeof (LentezzaGreaterPoisonedBolt), 0x1BFB, 0x3F, "dardi lentezza maggiore", 70.0, 7.0, 0xF42, 0x3F),
			new AmnoEntry(typeof (LentezzaDeadlyPoisonedBolt), 0x1BFB, 0x3F, "dardi lentezza mortale", 90.0, 9.0, 0xF42, 0x3F),
			new AmnoEntry(typeof (BloccoLesserPoisonedBolt), 0x1BFB, 0x429, "dardi blocco minore", 30.0, 5.0, 0xF42, 0x429),
			new AmnoEntry(typeof (BloccoNormalPoisonedBolt), 0x1BFB, 0x429, "dardi blocco", 50.0, 6.0, 0xF42, 0x429),
			new AmnoEntry(typeof (BloccoGreaterPoisonedBolt), 0x1BFB, 0x429, "dardi blocco maggiore", 70.0, 7.0, 0xF42, 0x429),
			new AmnoEntry(typeof (BloccoDeadlyPoisonedBolt), 0x1BFB, 0x429, "dardi blocco mortale", 90.0, 9.0, 0xF42, 0x429),
			new AmnoEntry(typeof (ParalisiLesserPoisonedBolt), 0x1BFB, 0x35, "dardi paralisi minore", 30.0, 5.0, 0xF42, 0x35),
			new AmnoEntry(typeof (ParalisiNormalPoisonedBolt), 0x1BFB, 0x35, "dardi paralisi", 50.0, 6.0, 0xF42, 0x35),
			new AmnoEntry(typeof (ParalisiGreaterPoisonedBolt), 0x1BFB, 0x35, "dardi paralisi maggiore", 70.0, 7.0, 0xF42, 0x35),
			new AmnoEntry(typeof (ParalisiDeadlyPoisonedBolt), 0x1BFB, 0x35, "dardi paralisi mortale", 90.0, 9.0, 0xF42, 0x35),
			new AmnoEntry(typeof (StanchezzaLesserPoisonedBolt), 0x1BFB, 0x21, "dardi stanchezza minore", 30.0, 5.0, 0xF42, 0x21),
			new AmnoEntry(typeof (StanchezzaNormalPoisonedBolt), 0x1BFB, 0x21, "dardi stanchezza", 50.0, 6.0, 0xF42, 0x21),
			new AmnoEntry(typeof (StanchezzaGreaterPoisonedBolt), 0x1BFB, 0x21, "dardi stanchezza maggiore", 70.0, 7.0, 0xF42, 0x21),
			new AmnoEntry(typeof (StanchezzaDeadlyPoisonedBolt), 0x1BFB, 0x21, "dardi stanchezza mortale", 90.0, 9.0, 0xF42, 0x21),
			new AmnoEntry(typeof (MagiaLesserPoisonedBolt), 0x1BFB, 0x3, "dardi magia minore", 30.0, 5.0, 0xF42, 0x3),
			new AmnoEntry(typeof (MagiaNormalPoisonedBolt), 0x1BFB, 0x3, "dardi magia", 50.0, 6.0, 0xF42, 0x3),
			new AmnoEntry(typeof (MagiaGreaterPoisonedBolt), 0x1BFB, 0x3, "dardi magia maggiore", 70.0, 7.0, 0xF42, 0x3),
			new AmnoEntry(typeof (MagiaDeadlyPoisonedBolt), 0x1BFB, 0x3, "dardi magia mortale", 90.0, 9.0, 0xF42, 0x3)
		};

		private static readonly AmnoEntry[] m_ArrowSAmmoTypes = new AmnoEntry[]
		{
			new AmnoEntry(typeof (LentezzaLesserPoisonedArrow), 0xF40, 0x3F, "frecce lentezza minore", 30.0, 5.0, 0xF42, 0x3F),
			new AmnoEntry(typeof (LentezzaNormalPoisonedArrow), 0xF40, 0x3F, "frecce lentezza", 50.0, 6.0, 0xF42, 0x3F),
			new AmnoEntry(typeof (LentezzaGreaterPoisonedArrow), 0xF40, 0x3F, "frecce lentezza maggiore", 70.0, 7.0, 0xF42, 0x3F),
			new AmnoEntry(typeof (LentezzaDeadlyPoisonedArrow), 0xF40, 0x3F, "frecce lentezza mortale", 90.0, 9.0, 0xF42, 0x3F),
			new AmnoEntry(typeof (BloccoLesserPoisonedArrow), 0xF40, 0x429, "frecce blocco minore", 30.0, 5.0, 0xF42, 0x429),
			new AmnoEntry(typeof (BloccoNormalPoisonedArrow), 0xF40, 0x429, "frecce blocco", 50.0, 6.0, 0xF42, 0x429),
			new AmnoEntry(typeof (BloccoGreaterPoisonedArrow), 0xF40, 0x429, "frecce blocco maggiore", 70.0, 7.0, 0xF42, 0x429),
			new AmnoEntry(typeof (BloccoDeadlyPoisonedArrow), 0xF40, 0x429, "frecce blocco mortale", 90.0, 9.0, 0xF42, 0x429),
			new AmnoEntry(typeof (ParalisiLesserPoisonedArrow), 0xF40, 0x35, "frecce paralisi minore", 30.0, 5.0, 0xF42, 0x35),
			new AmnoEntry(typeof (ParalisiNormalPoisonedArrow), 0xF40, 0x35, "frecce paralisi", 50.0, 6.0, 0xF42, 0x35),
			new AmnoEntry(typeof (ParalisiGreaterPoisonedArrow), 0xF40, 0x35, "frecce paralisi maggiore", 70.0, 7.0, 0xF42, 0x35),
			new AmnoEntry(typeof (ParalisiDeadlyPoisonedArrow), 0xF40, 0x35, "frecce paralisi mortale", 90.0, 9.0, 0xF42, 0x35),
			new AmnoEntry(typeof (StanchezzaLesserPoisonedArrow), 0xF40, 0x21, "frecce stanchezza minore", 30.0, 5.0, 0xF42, 0x21),
			new AmnoEntry(typeof (StanchezzaNormalPoisonedArrow), 0xF40, 0x21, "frecce stanchezza", 50.0, 6.0, 0xF42, 0x21),
			new AmnoEntry(typeof (StanchezzaGreaterPoisonedArrow), 0xF40, 0x21, "frecce stanchezza maggiore", 70.0, 7.0, 0xF42, 0x21),
			new AmnoEntry(typeof (StanchezzaDeadlyPoisonedArrow), 0xF40, 0x21, "frecce stanchezza mortale", 90.0, 9.0, 0xF42, 0x21),
			new AmnoEntry(typeof (MagiaLesserPoisonedArrow), 0xF40, 0x3, "frecce magia minore", 30.0, 5.0, 0xF42, 0x3),
			new AmnoEntry(typeof (MagiaNormalPoisonedArrow), 0xF40, 0x3, "frecce magia", 50.0, 6.0, 0xF42, 0x3),
			new AmnoEntry(typeof (MagiaGreaterPoisonedArrow), 0xF40, 0x3, "frecce magia maggiore", 70.0, 7.0, 0xF42, 0x3),
			new AmnoEntry(typeof (MagiaDeadlyPoisonedArrow), 0xF40, 0x3, "frecce magia mortale", 90.0, 9.0, 0xF42, 0x3)
		};

		private static readonly AmnoEntry[] m_BoltAmmoTypes = new AmnoEntry[]
		{
			new AmnoEntry(typeof (Bolt), 0x1BFB, 1, "bolts", 0.0, 0.0, 0xF42, 0),
			new AmnoEntry(typeof (LesserPoisonedBolt), 0x1BFB, 1920, "lesser poisoned bolts", 30.0, 5.0, 0xF42, 2130),
			new AmnoEntry(typeof (NormalPoisonedBolt), 0x1BFB, 1920, "regular poisoned bolts", 50.0, 6.0, 0xF42, 2130),
			new AmnoEntry(typeof (GreaterPoisonedBolt), 0x1BFB, 1920, "greater poisoned bolts", 70.0, 7.0, 0xF42, 2130),
			new AmnoEntry(typeof (DeadlyPoisonedBolt), 0x1BFB, 1920, "deadly poisoned bolts", 90.0, 9.0, 0xF42, 2130)
		};

		private static readonly AmnoEntry[] m_ArrowAmmoTypes = new AmnoEntry[]
		{
			new AmnoEntry(typeof (Arrow), 0xF40, 1, "arrows", 0.0, 0.0, 0xF42, 0),
			new AmnoEntry(typeof (LesserPoisonedArrow), 0xF40, 1920, "lesser poisoned arrows", 30.0, 5.0, 0xF42, 2130),
			new AmnoEntry(typeof (NormalPoisonedArrow), 0xF40, 1920, "regular poisoned arrows", 50.0, 6.0, 0xF42, 2130),
			new AmnoEntry(typeof (GreaterPoisonedArrow), 0xF40, 1920, "greater poisoned arrows", 70.0, 7.0, 0xF42, 2130),
			new AmnoEntry(typeof (DeadlyPoisonedArrow), 0xF40, 1920, "deadly poisoned arrows", 90.0, 9.0, 0xF42, 2130)
		};

		public static AmnoEntry[] BoltAmmoTypes
		{
			get { return m_BoltAmmoTypes; }
		}

		public static AmnoEntry[] ArrowAmmoTypes
		{
			get { return m_ArrowAmmoTypes; }
		}
		#endregion

		private static bool CanFireArrowByType( Type t, Mobile from )
		{
			return from != null && from.CanBeginAction( t );
		}

		private static bool CanFireArrowByTypeHue( Type t, Mobile from, int Hue )
		{
			if (from == null || ( !ClassSystem.IsScout( from ) && (Hue == 0x3 || Hue == 0x21 || Hue == 0x35 || Hue == 0x429 || Hue == 0x3F) ) )
				return false;

			return from != null && from.CanBeginAction( t );
		}

		private double GetEffectiveSkill( Mobile m )
		{
			if( CustomAmmoType == typeof( BasePoisonAmmonition ) )
				return m.Skills[ SkillName.Poisoning ].Value;
			else
				return m.Skills[ SkillName.Archery ].Value;
		}

		private void DisplayAmmoGump( Mobile from )
		{
			from.SendMenu( new AmnoMenu( this, from, GetAmmoItemListEntries( from ) ) );
		}

		private ItemListEntry[] GetAmmoItemListEntries( Mobile m )
		{
			AmnoEntry[] amnoEntries = GetAmmoEntries( m );
			if( amnoEntries == null || amnoEntries.Length == 0 )
				return null;

			List<AmnoEntry> list = new List<AmnoEntry>();
			double skill = GetEffectiveSkill( m );

			foreach( AmnoEntry entry in amnoEntries )
			{
				if( skill >= entry.SkillRequired )
					list.Add( entry );
			}

			ItemListEntry[] entries = new ItemListEntry[ list.Count ];
			for( int i = 0; i < list.Count; ++i )
			{
				AmnoEntry entry = list[ i ];
				entries[ i ] = new ItemListEntry( entry.Name, entry.ItemID, entry.Hue - 1 );
			}

			return entries;
		}

		public TimeSpan GetCoolingDelay( Mobile attacker, Type t )
		{
			AmnoEntry entry = GetEntryByType( attacker, t );

			return entry != null ? TimeSpan.FromSeconds( entry.CoolingDelay ) : TimeSpan.Zero;
		}

		public int GetEntryAnimIdByType( Mobile attacker, Type t )
		{
			AmnoEntry entry = GetEntryByType( attacker, t );
			return entry != null ? entry.AnimID : 0;
		}

		public int GetEntryHueByType( Mobile attacker, Type t )
		{
			AmnoEntry entry = GetEntryByType( attacker, t );
			return entry != null ? entry.Hue : 0;
		}

		private class AmnoMenu : ItemListMenu
		{
			private readonly BaseRanged m_BaseRanged;
			private readonly Mobile m_Mobile;

			public AmnoMenu( BaseRanged baseRanged, Mobile from, ItemListEntry[] entries )
				: base( ( from.TrueLanguage == LanguageType.Ita ? "Scegli la nuova munizione" : "Choose thy new ammo:" ), entries )
			{
				m_BaseRanged = baseRanged;
				m_Mobile = from;
			}

			public override void OnResponse( NetState state, int index )
			{
				AmnoEntry[] amnoEntries = m_BaseRanged.GetAmmoEntries( state.Mobile );
				if( index >= 0 && index < amnoEntries.Length )
					m_BaseRanged.ChangeAmno( m_Mobile, amnoEntries[ index ].Type );
			}
		}
		#endregion

		public BaseRanged( int itemID ) : base( itemID )
		{
		}

		public BaseRanged( Serial serial ) : base( serial )
		{
		}
		
		#region Mod by Magius(CHE)
		protected override void SetupHitRateAndEvasionBonus (out double hitrate, out double evasion)
		{
			hitrate = +30.0;
			evasion = -15.0;
		}
		#endregion

		public static double StandStillDelay = 0.75; // mod by Dies Irae

		public override TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
			WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );
			//StandStillDelay = 0.5 + Math.Abs( 6 - attacker.GetDistanceToSqrt( defender ) )/6;

			// Make sure we've been standing still for .25/.5/1 second depending on Era

			if ( DateTime.Now > (attacker.LastMoveTime + TimeSpan.FromSeconds( StandStillDelay ) ) ) // Core.SE ? 0.25 : (Core.AOS ? 0.5 : 1.0) )) || (Core.AOS && WeaponAbility.GetCurrentAbility( attacker ) is MovingShot)
			{
				bool canSwing = true;

				if ( Core.AOS )
				{
					canSwing = ( !attacker.Paralyzed && !attacker.Frozen );

					if ( canSwing )
					{
						Spell sp = attacker.Spell as Spell;

						canSwing = ( sp == null || !sp.IsCasting || !sp.BlocksMovement );
					}
				}

				if ( canSwing && attacker.HarmfulCheck( defender ) )
				{
					attacker.DisruptiveAction();
					attacker.Send( new Swing( 0, attacker, defender ) );

					#region mod by Dies Irae
					if( attacker.Skills[ SkillName.Fletching ].Value >= 30 && DiceRoll.Roll( "1d5" ) == 1 )
						attacker.Stam--;
					#endregion

					Item freccia;
					if ( OnFired( attacker, defender, out freccia ) )
					{
						if ( CheckHit( attacker, defender ) )
							OnHit( attacker, defender, freccia );
						else
							OnMiss( attacker, defender, freccia );
					}
				}

				attacker.RevealingAction( true );

				return GetDelay( attacker );
			}
			else
			{
				attacker.RevealingAction( true );

				return TimeSpan.FromSeconds( 0.25 );
			}
		}

		public void OnHit( Mobile attacker, Mobile defender, Item freccia )
		{
			#region mod by Dies Irae
			if( attacker.Player )
			{
				if( CustomAmmoType != null && freccia is ISpecialAmmo )
					( (ISpecialAmmo)freccia ).OnHit( this, attacker, defender, 1.0 );
				else if( AmmoType != null && !defender.Player && ( defender.Body.IsAnimal || defender.Body.IsMonster ) && 0.4 >= Utility.RandomDouble() )
					defender.AddToBackpack( Ammo );
			}
			#endregion

			if ( Core.ML && m_Velocity > 0 )
			{
				int bonus = (int) attacker.GetDistanceToSqrt( defender );

				if ( bonus > 0 && m_Velocity > Utility.Random( 100 ) )
				{
					AOS.Damage( defender, attacker, bonus * 3, 100, 0, 0, 0, 0 );

					if ( attacker.Player )
						attacker.SendLocalizedMessage( 1072794 ); // Your arrow hits its mark with velocity!

					if ( defender.Player )
						defender.SendLocalizedMessage( 1072795 ); // You have been hit by an arrow with velocity!
				}
			}

			base.OnHit( attacker, defender, 1.0 );
		}

		public void OnMiss( Mobile attacker, Mobile defender, Item freccia )
		{
			#region mod by Dies Irae
			if( attacker.Player )
			{
				if( CustomAmmoType != null && freccia is ISpecialAmmo )
					( (ISpecialAmmo)freccia ).OnMiss( this, attacker, defender );
				else
				{
					if( AmmoType != null && 0.4 >= Utility.RandomDouble() )
					{
						if ( Core.SE )
						{
							PlayerMobile p = attacker as PlayerMobile;

							if ( p != null )
							{
								Type ammo = AmmoType;

								if ( p.RecoverableAmmo.ContainsKey( ammo ) )
									p.RecoverableAmmo[ ammo ]++;
								else
									p.RecoverableAmmo.Add( ammo, 1 );

								if ( !p.Warmode )
								{
									if ( m_RecoveryTimer == null )
										m_RecoveryTimer = Timer.DelayCall( TimeSpan.FromSeconds( 10 ), new TimerCallback( p.RecoverAmmo ) );

									if ( !m_RecoveryTimer.Running )
										m_RecoveryTimer.Start();
								}
							}
						}
						else
							Ammo.MoveToWorld( new Point3D( defender.X + Utility.RandomMinMax( -1, 1 ), defender.Y + Utility.RandomMinMax( -1, 1 ), defender.Z ), defender.Map );
					}
					StoneEnchantItem state = StoneEnchantItem.Find( this );
					if( state != null && state.Definition != null )
						state.Definition.OnMiss( this, attacker, defender );
				}
			}
			#endregion

			base.OnMiss( attacker, defender );
		}

		public bool FindArrow( Container cont, Type type, out Item arrow )
		{
			Item[] items = cont.FindItemsByType( type, true );

			// First pass, compute total
			int total = 0;

			for( int i = 0; i < items.Length; ++i )
				total += items[i].Amount;

			if( total >= 1 )
			{
				for( int i = 0; i < items.Length; ++i )
				{
					Item item = items[i];

					int theirAmount = item.Amount;

					if( theirAmount > 0 )
					{
						arrow = item;
						return true;
					}
				}
			}
			arrow = null;
			return false;
		}

		public virtual bool OnFired( Mobile attacker, Mobile defender, out Item arrow )
		{
			BaseQuiver quiver = attacker.FindItemOnLayer( Core.ML ? Layer.Cloak : Layer.Waist ) as BaseQuiver;
			Container pack = attacker.Backpack;
			Item freccia = null;
			arrow = Ammo;

			if ( attacker.Player )
			{
				if ( quiver == null || quiver.LowerAmmoCost == 0 || quiver.LowerAmmoCost > Utility.Random( 100 ) )
				{
					if ( quiver != null && FindArrow( quiver, CustomAmmoType, out freccia ) ) // mod by Dies Irae
					{
						arrow = freccia;
						quiver.InvalidateWeight();
					}
					else if ( pack != null && FindArrow( pack, CustomAmmoType, out freccia ) ) // mod by Dies Irae
					{
						arrow = freccia;
						//attacker.SendMessage( attacker.Language == "ITA" ? "Hai finito le frecce selezionate." : "You haven't any arrow of the selected kind." );
						//return false;
					}
					else
					{
						//arrow = freccia;
						attacker.SendMessage( attacker.Language == "ITA" ? "Hai finito le frecce selezionate." : "You haven't any arrow of the selected kind." );
						return false;
					}
				}
			}

			#region mod by Dies Irae
			if( attacker.Player && freccia != null )
			{
				if( freccia is ISpecialAmmo )
					return ( (ISpecialAmmo)freccia ).OnFired( this, attacker, defender );
				else
				{
					freccia.Consume();
					StoneEnchantItem state = StoneEnchantItem.Find( this );
					if( state != null && state.Definition != null && state.Definition.OnFired( this, attacker, defender ) )
						return true;
				}
			}
			#endregion

			attacker.MovingEffect( defender, EffectID, 18, 1, false, false );

			return true;
		}

		#region Mondain's Legacy
		public override int ComputeDamage( Mobile attacker, Mobile defender )
		{
			int damage = base.ComputeDamage( attacker, defender );
			
			// add velocity bonus
			if ( m_Velocity > 0 )
			{
				int range = (int) Math.Round( Math.Sqrt( Math.Pow( attacker.X - defender.X, 2 ) + Math.Pow( attacker.Y - defender.Y, 2 ) ) ); 	
				damage += (int) Math.Round( Math.Min( range * 3, 30 ) * ( m_Velocity / (double) 100 ) );
			}	
			
			return damage;	
		}
		
		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			if ( Parent is Mobile )
			{
				Mobile parent = (Mobile) Parent;
			
				BaseQuiver quiver = parent.FindItemOnLayer( Layer.Cloak ) as BaseQuiver;
				
				if ( quiver != null && !quiver.DamageModifier.IsEmpty )
				{
					quiver.GetDamageTypes( out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct );
					
					return;
				}
			}
			
			base.GetDamageTypes( wielder, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct );
		}
		
		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			
			if ( m_Balanced )
				list.Add( 1072792 ); // Balanced
			
			if ( m_Velocity > 0 )
				list.Add( 1072793, m_Velocity.ToString() ); // Velocity ~1_val~%
		}
		#endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 5 ); // version

			if( m_LastAmnoEntry != null && m_LastAmnoEntry.Type != null )
				writer.Write( m_LastAmnoEntry.Type.FullName );
			else
				writer.Write( "" );

			writer.Write( (int) m_Velocity );
			writer.Write( (bool) m_Balanced );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 5:
					{
						string type = reader.ReadString();
						Type t = ScriptCompiler.FindTypeByFullName( type );
						if( t != null )
							m_LastAmnoEntry = GetEntryByType( null, t );
						goto case 4;
					}
				case 4:
					m_Velocity = reader.ReadInt();
					goto case 3;
				case 3:
				{
					m_Balanced = reader.ReadBool();

					goto case 2;
				}
				case 2:
				case 1:
				{
					break;
				}
				case 0:
				{
					/*m_EffectID =*/ reader.ReadInt();
					break;
				}
			}

			if ( version < 2 )
			{
				WeaponAttributes.MageWeapon = 0;
				WeaponAttributes.UseBestSkill = 0;
			}
		}
	}
}