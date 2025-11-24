using System;
using Server;
using Server.Engines.Craft;
using Server.Items;
using Server.Targeting;
using Midgard.Misc;

using Server.Network;
namespace Midgard.Engines.OldCraftSystem
{
	public class AlchemyTarget : Target
	{
		private BaseReagent m_Firstreagent;
		private BaseTool m_Tool;

		public AlchemyTarget( BaseTool tool, BaseReagent firstreagent ): base( 0, false, TargetFlags.None )
		{
			m_Firstreagent = firstreagent;
			m_Tool = tool;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if( targeted is BaseReagent )
			{
				bool table;
				DefAlchemy.CheckAlchemyTable( from, 2, out table );

				if( !table )
				{
					from.SendMessage( (from.Language == "ITA" ? "Devi essere vicino ad un tavolo alchemico per la combinazione." : "You must be near an alchemic table for the combination.") );
					return;
				}
				else
				{
					BaseReagent Secondreagent = (BaseReagent)targeted;

					if ( Secondreagent.PotionType != 0 )
					{
						if (Secondreagent.PotionType == m_Firstreagent.PotionType && m_Firstreagent.PotionStrenght != Secondreagent.PotionStrenght)
						{
							int potstr = m_Firstreagent.PotionStrenght + Secondreagent.PotionStrenght;
							if ( potstr < 5 )//lesser
							{
								if( from.CheckSkill( SkillName.Alchemy, -5, 45 ) )
								{
									switch( m_Firstreagent.PotionType )//1 Magia, 2 Stanchezza, 3 Paralisi, 4 Blocco, 5 Lentezza
									{
										case 1: from.AddToBackpack( new MagiaLesserPoisonPotion() ); break;
										case 2: from.AddToBackpack( new StanchezzaLesserPoisonPotion() ); break;
										case 3: from.AddToBackpack( new ParalisiLesserPoisonPotion() ); break;
										case 4: from.AddToBackpack( new BloccoLesserPoisonPotion() ); break;
										case 5: from.AddToBackpack( new LentezzaLesserPoisonPotion() ); break;
									}
									from.SendMessage( (from.Language == "ITA" ? "Hai creato una pozione minore." : "You created a lesser potion.") );
								}
								else
									from.SendMessage( (from.Language == "ITA" ? "Hai fallito la combinazione." : "You failed the combination.") );
							}
							else if ( potstr < 7 )//normal
							{
								if( from.CheckSkill( SkillName.Alchemy, 15, 65 ) )
								{
									switch( m_Firstreagent.PotionType )//1 Magia, 2 Stanchezza, 3 Paralisi, 4 Blocco, 5 Lentezza
									{
										case 1: from.AddToBackpack( new MagiaPoisonPotion() ); break;
										case 2: from.AddToBackpack( new StanchezzaPoisonPotion() ); break;
										case 3: from.AddToBackpack( new ParalisiPoisonPotion() ); break;
										case 4: from.AddToBackpack( new BloccoPoisonPotion() ); break;
										case 5: from.AddToBackpack( new LentezzaPoisonPotion() ); break;
									}
									from.SendMessage( (from.Language == "ITA" ? "Hai creato una pozione." : "You created a potion.") );
								}
								else
									from.SendMessage( (from.Language == "ITA" ? "Hai fallito la combinazione." : "You failed the combination.") );
							}
							else if ( potstr < 10 )//greater
							{
								if( from.CheckSkill( SkillName.Alchemy, 55, 105 ) )
								{
									switch( m_Firstreagent.PotionType )//1 Magia, 2 Stanchezza, 3 Paralisi, 4 Blocco, 5 Lentezza
									{
										case 1: from.AddToBackpack( new MagiaGreaterPoisonPotion() ); break;
										case 2: from.AddToBackpack( new StanchezzaGreaterPoisonPotion() ); break;
										case 3: from.AddToBackpack( new ParalisiGreaterPoisonPotion() ); break;
										case 4: from.AddToBackpack( new BloccoGreaterPoisonPotion() ); break;
										case 5: from.AddToBackpack( new LentezzaGreaterPoisonPotion() ); break;
									}
									from.SendMessage( (from.Language == "ITA" ? "Hai creato una pozione maggiore." : "You created a greater potion.") );
								}
								else
									from.SendMessage( (from.Language == "ITA" ? "Hai fallito la combinazione." : "You failed the combination.") );
							}
							else //deadly
							{
								if( from.CheckSkill( SkillName.Alchemy, 90, 120 ) )
								{
									switch( m_Firstreagent.PotionType )//1 Magia, 2 Stanchezza, 3 Paralisi, 4 Blocco, 5 Lentezza
									{
										case 1: from.AddToBackpack( new MagiaDeadlyPoisonPotion() ); break;
										case 2: from.AddToBackpack( new StanchezzaDeadlyPoisonPotion() ); break;
										case 3: from.AddToBackpack( new ParalisiDeadlyPoisonPotion() ); break;
										case 4: from.AddToBackpack( new BloccoDeadlyPoisonPotion() ); break;
										case 5: from.AddToBackpack( new LentezzaDeadlyPoisonPotion() ); break;
									}
									from.SendMessage( (from.Language == "ITA" ? "Hai creato una pozione mortale." : "You created a deadly potion.") );
								}
								else
									from.SendMessage( (from.Language == "ITA" ? "Hai fallito la combinazione." : "You failed the combination.") );
							}
						}
						else
						{
							from.SendMessage( (from.Language == "ITA" ? "Hai fallito la combinazione." : "You failed the combination.") );
						}
					}
					else
					{
						from.SendMessage( (from.Language == "ITA" ? "Non sembrano compatibili." : "They aren't compatible.") );
					}

					from.PlaySound( 0x242 );

					if( from.Body.IsHuman )
						from.Animate( AnimsOnMount.GetAnim( 0x21, from.Mounted ), 7, 1, true, false, 0 );

					m_Firstreagent.Consume( 1 );
					Secondreagent.Consume( 1 );
					m_Tool.UsesRemaining--;

					if ( m_Tool.UsesRemaining < 1 )
					{
						Mobile parent = m_Tool.RootParent as Mobile;

						if( parent != null )
							parent.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "Houch!! My hands!" );

						m_Tool.Delete();
					}
				}
			}
		}
	}
}