#define OSI

using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using System.IO;
using System.Text;
using Midgard.Items;

namespace Server.Items
{
	public class TreasureMapChest : LockableContainer
	{
		public override int LabelNumber{ get{ return 3000541; } }

		public static Type[] Artifacts { get { return Core.AOS ? m_Artifacts : m_OldArtifacts ; } }

		private static Type[] m_Artifacts = new Type[]
		{
			typeof( CandelabraOfSouls ), typeof( GoldBricks ), typeof( PhillipsWoodenSteed ),
			typeof( ArcticDeathDealer ), typeof( BlazeOfDeath ), typeof( BurglarsBandana ),
			typeof( CavortingClub ), typeof( DreadPirateHat ),
			typeof( EnchantedTitanLegBone ), typeof( GwennosHarp ), typeof( IolosLute ),
			typeof( LunaLance ), typeof( NightsKiss ), typeof( NoxRangersHeavyCrossbow ),
			typeof( PolarBearMask ), typeof( VioletCourage ), typeof( HeartOfTheLion ),
			typeof( ColdBlood ), typeof( AlchemistsBauble )
		};

        public static Type[] m_OldArtifacts = new Type[]
        {
            typeof( GoldBricks ),
            typeof( GlovesOfDexterityOne ),
            typeof( GlovesOfDexterityTwo ),
            typeof( GlovesOfDexterityThree ),
            typeof( OldMagicWizardsHatOne ),
            typeof( OldMagicWizardsHatTwo ),
            typeof( OldMagicWizardsHatThree ),
            typeof( BodySashOfStrengthOne ),
            typeof( BodySashOfStrengthTwo ),
            typeof( BodySashOfStrengthThree ),
            typeof( RingOfProtectionOne ),
            typeof( RingOfProtectionTwo ),
            typeof( RingOfProtectionThree ),
            typeof( RingOfProtectionFour ),
            typeof( RingOfProtectionFive ),
            typeof( RingOfProtectionSix )
        };

		private int m_Level;
		private DateTime m_DeleteTime;
		private Timer m_Timer;
		private Mobile m_Owner;
		private bool m_Temporary;

		private List<Mobile> m_Guardians;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Level{ get{ return m_Level; } set{ m_Level = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner{ get{ return m_Owner; } set{ m_Owner = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime DeleteTime{ get{ return m_DeleteTime; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Temporary{ get{ return m_Temporary; } set{ m_Temporary = value; } }

		public List<Mobile> Guardians { get { return m_Guardians; } }

		[Constructable]
		public TreasureMapChest( int level ) : this( null, level, false )
		{
		}

		public TreasureMapChest( Mobile owner, int level, bool temporary ) : base( 0xE40 )
		{
			m_Owner = owner;
			m_Level = level;
			m_DeleteTime = DateTime.Now + TimeSpan.FromHours( 3.0 );

			m_Temporary = temporary;
			m_Guardians = new List<Mobile>();

			m_Timer = new DeleteTimer( this, m_DeleteTime );
			m_Timer.Start();

			Fill( this, level );
		}

		private static void GetRandomAOSStats( out int attributeCount, out int min, out int max )
		{
			int rnd = Utility.Random( 15 );

			if ( rnd < 1 )
			{
				attributeCount = Utility.RandomMinMax( 2, 6 );
				min = 20; max = 70;
			}
			else if ( rnd < 3 )
			{
				attributeCount = Utility.RandomMinMax( 2, 4 );
				min = 20; max = 50;
			}
			else if ( rnd < 6 )
			{
				attributeCount = Utility.RandomMinMax( 2, 3 );
				min = 20; max = 40;
			}
			else if ( rnd < 10 )
			{
				attributeCount = Utility.RandomMinMax( 1, 2 );
				min = 10; max = 30;
			}
			else
			{
				attributeCount = 1;
				min = 10; max = 20;
			}
		}

		public static void Fill( LockableContainer cont, int level )
		{
			cont.Movable = false;
			cont.Locked = true;

			if ( level == 0 )
			{
				cont.LockLevel = 0; // Can't be unlocked

				cont.DropItem( new Gold( Utility.RandomMinMax( 50, 100 ) ) );

				if ( Utility.RandomDouble() < 0.75 )
				{
					#region modifica by Dies Irae per spawnare solo mappe su Felucca
					cont.DropItem( new TreasureMap( 0, Map.Felucca ) );
//					cont.DropItem( new TreasureMap( 0, Map.Trammel ) );
					#endregion
				}
			}
			else
			{
				cont.TrapType = TrapType.ExplosionTrap;
				cont.TrapPower = level * 25;
				cont.TrapLevel = level;

				switch ( level )
				{
					case 1: cont.RequiredSkill = 36; break;
					case 2: cont.RequiredSkill = 76; break;
					case 3: cont.RequiredSkill = 84; break;
					case 4: cont.RequiredSkill = 92; break;
					case 5: cont.RequiredSkill = 100; break;
					case 6: cont.RequiredSkill = 100; break;
				}

				cont.LockLevel = cont.RequiredSkill - 10;
				cont.MaxLockLevel = cont.RequiredSkill + 40;

#if OSI
				cont.DropItem( new Gold( level * 1000 ) );
#else
				cont.DropItem( new Gold( Utility.Random( level * 500, level * 4000 ) ) );
#endif

				for ( int i = 0; i < level * 5; ++i )
					cont.DropItem( Loot.RandomScroll( 0, 63, SpellbookType.Regular ) );

                #region mod by Dies Irae
			    int itemCount = 0;
                switch( level )
                {
                    case 1: itemCount = 5; break;
                    case 2: itemCount = 10; break;
                    case 3: itemCount = 15; break;
                    case 4: itemCount = 38; break;
                    case 5: itemCount = 50; break;
                    case 6: itemCount = 50; break;
                }
                #endregion

                for( int i = 0; i < itemCount; ++i ) // mod by Dies Irae
				{
					Item item;

					//if ( Core.AOS )
						item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();
					//else
					//	item = Loot.RandomArmorOrShieldOrWeapon();

                    Midgard.Engines.SecondAgeLoot.Magics.ApplyBonusTo( item );

					if ( item is BaseWeapon )
					{
						BaseWeapon weapon = (BaseWeapon)item;

						if ( Core.AOS )
						{
							int attributeCount;
							int min, max;

							GetRandomAOSStats( out attributeCount, out min, out max );

							BaseRunicTool.ApplyAttributesTo( weapon, attributeCount, min, max );
						}
						else
                        {
                            #region mod by Dies Irae
                            //weapon.DamageLevel = (WeaponDamageLevel)Utility.Random( 6 );
                            //weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random( 6 );
                            //weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random( 6 );
                            #endregion
                        }

						cont.DropItem( item );
					}
					else if ( item is BaseArmor )
					{
						BaseArmor armor = (BaseArmor)item;

						if ( Core.AOS )
						{
							int attributeCount;
							int min, max;

							GetRandomAOSStats( out attributeCount, out min, out max );

							BaseRunicTool.ApplyAttributesTo( armor, attributeCount, min, max );
						}
						else
						{
                            #region mod by Dies Irae
                            //armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random( 6 );
                            //armor.Durability = (ArmorDurabilityLevel)Utility.Random( 6 );
                            #endregion
                        }

						cont.DropItem( item );
					}
					else if( item is BaseHat )
					{
						BaseHat hat = (BaseHat)item;

						if( Core.AOS )
						{
							int attributeCount;
							int min, max;

							GetRandomAOSStats( out attributeCount, out min, out  max );

							BaseRunicTool.ApplyAttributesTo( hat, attributeCount, min, max );
						}

						cont.DropItem( item );
					}
					else if( item is BaseJewel )
                    {
                        cont.DropItem( item );
					}
				}
			}

#if OSI
			int reagents;
			if ( level == 0 )
				reagents = 1;
			else
				reagents = level * 3;

			for ( int i = 0; i < reagents; i++ )
			{
				Item item = Loot.RandomPossibleReagent();
				item.Amount = Utility.RandomMinMax( 40, 60 );
				cont.DropItem( item );
			}
#else
			for( int i = Utility.Random( 2, 2+(int)level ); i > 1; i-- )
			{
				Item ReagentLoot = Loot.RandomReagent();
				ReagentLoot.Amount = Utility.Random(level*100,level*200 );
				cont.DropItem( ReagentLoot );
			}
#endif

#if OSI
			int gems;
			if ( level == 0 )
				gems = 2;
			else
				gems = level * 3;

			for ( int i = 0; i < gems; i++ )
			{
				Item item = Loot.RandomGem();
				cont.DropItem( item );
			}

#else
			for( int i = Utility.Random( 1, (int)level ); i > 1; i-- )
			{
				Item GemLoot = Loot.RandomGem();
				GemLoot.Amount = Utility.Random( level, level*4);
				cont.DropItem( GemLoot );
			}

			for ( int i = Utility.Random( 1, (int)level ); i > 1; i-- )
			{
				Item PotionLoot = Loot.RandomPotion();			        
				PotionLoot.Amount=Utility.Random( 1, (int)level*4 );
				cont.DropItem( PotionLoot );
			}

			if (level==5)
			{	
				//*** uscita sop
				if (0.25 > Utility.RandomDouble()) //
				{
					int s_rnd = Utility.Random( 100 );
					if ( 50 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Blacksmith, 110 ) );
					else if ( 75 > s_rnd )// ( 5 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Blacksmith, 115 ) );
					else if ( 90 > s_rnd )// ( 5 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Blacksmith, 120 ) );
				}
				if (0.25 > Utility.RandomDouble()) //
				{
					int s_rnd = Utility.Random( 100 );
					if ( 50 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Tailoring, 110 ) );
					else if ( 75 > s_rnd )// ( 5 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Tailoring, 115 ) );
					else if ( 90 > s_rnd )// ( 5 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Tailoring, 120 ) );
				}
				//***
							
				cont.DropItem(new ValoriteIngot( 100 ) );
				cont.DropItem(new AquaIngot( 35 ) );
				cont.DropItem(new VulcanIngot( 50 ) );

				// Runic Hammer 50% che ne esca uno
				if (0.50 > Utility.RandomDouble())
				{
					int h_rnd = Utility.Random( 100 );

					if (h_rnd < 25) //25%
						cont.DropItem(new RunicHammer( CraftResource.DullCopper, Core.AOS ? 20 : 20 ) );           
					else if (h_rnd < 50) //25 %
						cont.DropItem(new RunicHammer( CraftResource.ShadowIron, Core.AOS ? 15 : 20 ) );           
					else if (h_rnd < 75) //25 %
						cont.DropItem( new RunicHammer( CraftResource.Copper, Core.AOS ? 14 : 20 ) );               
					else if (h_rnd < 85) //10%
						cont.DropItem( new RunicHammer( CraftResource.Bronze, Core.AOS ? 13 : 20 ) );               
					else if (h_rnd < 93) //8%
						cont.DropItem( new RunicHammer( CraftResource.Gold, Core.AOS ? 10 : 20 ) );                 
					else if (h_rnd < 97) // 4%
						cont.DropItem( new RunicHammer( CraftResource.Agapite, Core.AOS ? 10 : 20 ) );              
					else if (h_rnd < 99) // 2%
						cont.DropItem( new RunicHammer( CraftResource.Verite, Core.AOS ? 8 : 20 ) );               
					else // 1%
						cont.DropItem( new RunicHammer( CraftResource.Valorite, Core.AOS ? 5 : 20 ) ); 
					
				}

				// Runic SewingKit 50% che ne esca uno
				if (0.50 > Utility.RandomDouble())
				{
					int s_rnd = Utility.Random( 100 );

					if (s_rnd < 70) // 75%
						cont.DropItem( new RunicSewingKit( CraftResource.SpinedLeather, 20 ) );
					else if (s_rnd < 90) // 20%
						cont.DropItem( new RunicSewingKit( CraftResource.HornedLeather, 14 ) );
					else // 10%
						cont.DropItem( new RunicSewingKit( CraftResource.BarbedLeather, 7 ) );
				}
			}

			else if (level==6)
			{	
				//*** uscita sop
				if (0.50 > Utility.RandomDouble()) //
				{
					int s_rnd = Utility.Random( 100 );
					if ( 50 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Blacksmith, 110 ) );
					else if ( 75 > s_rnd )// ( 5 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Blacksmith, 115 ) );
					else if ( 90 > s_rnd )// ( 5 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Blacksmith, 120 ) );
				}
				if (0.50 > Utility.RandomDouble()) //
				{
					int s_rnd = Utility.Random( 100 );
					if ( 50 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Tailoring, 110 ) );
					else if ( 75 > s_rnd )// ( 5 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Tailoring, 115 ) );
					else if ( 90 > s_rnd )// ( 5 > s_rnd )
						cont.DropItem( new PowerScroll( SkillName.Tailoring, 120 ) );
				}

				//***
				
				cont.DropItem(new EldarIngot( 200 ) );
				cont.DropItem(new CrystalineIngot( 200 ) );
				cont.DropItem(new AquaIngot( 125 ) );
				cont.DropItem(new VulcanIngot( 150 ) );

//				// 100% di avere un fletch kit Runico
//				cont.DropItem(new RunicFletcherKit( (CraftResource)Utility.Random( 11 )+113, 5 + Utility.Random( 10 ) ) );
				
				// Runic Hammer 85% che ne esca uno
				if (0.85 > Utility.RandomDouble()) //
				{
					int h_rnd = Utility.Random( 100 );

					if (h_rnd < 16) // 17% // Sfigato !!!
						cont.DropItem(new RunicHammer( CraftResource.DullCopper, Core.AOS ? 50 : 50 ) );           
					else if (h_rnd < 36) // 20 %
						cont.DropItem(new RunicHammer( CraftResource.ShadowIron, Core.AOS ? 45 : 50 ) );           
					else if (h_rnd < 56) // 20 %
						cont.DropItem( new RunicHammer( CraftResource.Copper, Core.AOS ? 40 : 50 ) );               
					else if (h_rnd < 76) // 10%
						cont.DropItem( new RunicHammer( CraftResource.Bronze, Core.AOS ? 35 : 40 ) );               
					else if (h_rnd < 86) // 10%
						cont.DropItem( new RunicHammer( CraftResource.Gold, Core.AOS ? 30 : 40 ) );                 
					else if (h_rnd < 94) // 8%
						cont.DropItem( new RunicHammer( CraftResource.Agapite, Core.AOS ? 25 : 30 ) );              
					else if (h_rnd < 98) // 4%
						cont.DropItem( new RunicHammer( CraftResource.Verite, Core.AOS ? 20 : 30 ) );               
					else // 2%
						cont.DropItem( new RunicHammer( CraftResource.Valorite, Core.AOS ? 15 : 30 ) ); 
					
				}

				// Runic SewingKit 80% che ne esca uno
				if (0.80 > Utility.RandomDouble())
				{
					int s_rnd = Utility.Random( 100 );

					if (s_rnd < 70) // 75%
						cont.DropItem( new RunicSewingKit( CraftResource.SpinedLeather, 20 ) );
					else if (s_rnd < 90) // 20%
						cont.DropItem( new RunicSewingKit( CraftResource.HornedLeather, 14 ) );
					else // 10%
						cont.DropItem( new RunicSewingKit( CraftResource.BarbedLeather, 7 ) );

				}				
			}
#endif

#if OSI
			if ( level == 6 && Core.AOS )
			{
				Item item = (Item)Activator.CreateInstance( Artifacts[Utility.Random(Artifacts.Length)] );
				cont.DropItem( item );
				
				#region modifica by Dies Irae per il drop artefatti
				try
				{
					TextWriter tw = File.AppendText("Logs/Midgard2ArtifactsLog.txt");
					tw.WriteLine( "Artefatto {0} (seriale {1}) droppato in data {2} in cassa del tesoro.",
					              item.GetType().Name, item.Serial.ToString(), DateTime.Now.ToString() );
					tw.Close();
				}
				catch(Exception ex)
				{
					Console.Write("Log del drop di un'artefatto fallito: {0}", ex);
				}
				#endregion
			}
#else
			#region modifica By Dies Irae per i Talismani
			double randomTalismanChance;
			switch ( level )
			{
				case 1: randomTalismanChance = 0.0; break;
				case 2: randomTalismanChance = 0.0; break;
				case 3: randomTalismanChance = 0.0; break;
				case 4: randomTalismanChance = 0.2; break;
				case 5: randomTalismanChance = 0.5; break;
				case 6: randomTalismanChance = 1.0; break;
				default: randomTalismanChance = 0.0; break;
			}
			
			if( Utility.RandomDouble() < randomTalismanChance )
				cont.DropItem( new RandomTalisman() );
			#endregion
#endif
		}

		public override bool CheckLocked( Mobile from )
		{
			if ( !this.Locked )
				return false;

			if ( this.Level == 0 && from.AccessLevel < AccessLevel.GameMaster )
			{
				foreach ( Mobile m in this.Guardians )
				{
					if ( m.Alive )
					{
						from.SendLocalizedMessage( 1046448 ); // You must first kill the guardians before you may open this chest.
						return true;
					}
				}

				LockPick( from );
				return false;
			}
			else
			{
				return base.CheckLocked( from );
			}
		}

		private List<Item> m_Lifted = new List<Item>();

		private bool CheckLoot( Mobile m, bool criminalAction )
		{
			if ( m_Temporary )
				return false;

			if ( m.AccessLevel >= AccessLevel.GameMaster || m_Owner == null || m == m_Owner )
				return true;

			Party p = Party.Get( m_Owner );

			if ( p != null && p.Contains( m ) )
				return true;

			Map map = this.Map;

			if ( map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0 )
			{
				if ( criminalAction )
					m.CriminalAction( true );
				else
					m.SendLocalizedMessage( 1010630 ); // Taking someone else's treasure is a criminal offense!

				return true;
			}

			m.SendLocalizedMessage( 1010631 ); // You did not discover this chest!
			return false;
		}

		public override bool IsDecoContainer
		{
			get{ return false; }
		}

		public override bool CheckItemUse( Mobile from, Item item )
		{
			return CheckLoot( from, item != this ) && base.CheckItemUse( from, item );
		}

		public override bool CheckLift( Mobile from, Item item, ref LRReason reject )
		{
			return CheckLoot( from, true ) && base.CheckLift( from, item, ref reject );
		}

		public override void OnItemLifted( Mobile from, Item item )
		{
			bool notYetLifted = !m_Lifted.Contains( item );

			from.RevealingAction();

			if ( notYetLifted )
			{
				m_Lifted.Add( item );

				if ( 0.1 >= Utility.RandomDouble() ) // 10% chance to spawn a new monster
					TreasureMap.Spawn( m_Level, GetWorldLocation(), Map, from, false );
			}

			base.OnItemLifted( from, item );
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			if ( m.AccessLevel < AccessLevel.GameMaster )
			{
				m.SendLocalizedMessage( 1048122, "", 0x8A5 ); // The chest refuses to be filled with treasure again.
				return false;
			}

			return base.CheckHold( m, item, message, checkItems, plusItems, plusWeight );
		}

		public TreasureMapChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			writer.Write( m_Guardians, true );
			writer.Write( (bool) m_Temporary );

			writer.Write( m_Owner );

			writer.Write( (int) m_Level );
			writer.WriteDeltaTime( m_DeleteTime );
			writer.Write( m_Lifted, true );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				{
					m_Guardians = reader.ReadStrongMobileList();
					m_Temporary = reader.ReadBool();

					goto case 1;
				}
				case 1:
				{
					m_Owner = reader.ReadMobile();

					goto case 0;
				}
				case 0:
				{
					m_Level = reader.ReadInt();
					m_DeleteTime = reader.ReadDeltaTime();
					m_Lifted = reader.ReadStrongItemList();

					if ( version < 2 )
						m_Guardians = new List<Mobile>();

					break;
				}
			}

			if ( !m_Temporary )
			{
				m_Timer = new DeleteTimer( this, m_DeleteTime );
				m_Timer.Start();
			}
			else
			{
				Delete();
			}
		}

		public override void OnAfterDelete()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;

			base.OnAfterDelete();
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive )
				list.Add( new RemoveEntry( from, this ) );
		}

		public void BeginRemove( Mobile from )
		{
			if ( !from.Alive )
				return;

			from.CloseGump( typeof( RemoveGump ) );
			from.SendGump( new RemoveGump( from, this ) );
		}

		public void EndRemove( Mobile from )
		{
			if ( Deleted || from != m_Owner || !from.InRange( GetWorldLocation(), 3 ) )
				return;

			from.SendLocalizedMessage( 1048124, "", 0x8A5 ); // The old, rusted chest crumbles when you hit it.
			this.Delete();
		}

		private class RemoveGump : Gump
		{
			private Mobile m_From;
			private TreasureMapChest m_Chest;

			public RemoveGump( Mobile from, TreasureMapChest chest ) : base( 15, 15 )
			{
				m_From = from;
				m_Chest = chest;

				Closable = false;
				Disposable = false;

				AddPage( 0 );

				AddBackground( 30, 0, 240, 240, 2620 );

				AddHtmlLocalized( 45, 15, 200, 80, 1048125, 0xFFFFFF, false, false ); // When this treasure chest is removed, any items still inside of it will be lost.
				AddHtmlLocalized( 45, 95, 200, 60, 1048126, 0xFFFFFF, false, false ); // Are you certain you're ready to remove this chest?

				AddButton( 40, 153, 4005, 4007, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 155, 180, 40, 1048127, 0xFFFFFF, false, false ); // Remove the Treasure Chest

				AddButton( 40, 195, 4005, 4007, 2, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 197, 180, 35, 1006045, 0xFFFFFF, false, false ); // Cancel
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( info.ButtonID == 1 )
					m_Chest.EndRemove( m_From );
			}
		}

		private class RemoveEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private TreasureMapChest m_Chest;

			public RemoveEntry( Mobile from, TreasureMapChest chest ) : base( 6149, 3 )
			{
				m_From = from;
				m_Chest = chest;

				Enabled = ( from == chest.Owner );
			}

			public override void OnClick()
			{
				if ( m_Chest.Deleted || m_From != m_Chest.Owner || !m_From.CheckAlive() )
					return;

				m_Chest.BeginRemove( m_From );
			}
		}

		private class DeleteTimer : Timer
		{
			private Item m_Item;

			public DeleteTimer( Item item, DateTime time ) : base( time - DateTime.Now )
			{
				m_Item = item;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m_Item.Delete();
			}
		}
	}
}
