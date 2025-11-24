using System;

using Midgard.Engines.Classes;

using Server;

namespace Server.Items
{
	public class SmokeBomb : Item
	{
	    public override string DefaultName
	    {
	        get { return "a smoke bomb"; }
	    }

		[Constructable]
		public SmokeBomb() : base( Core.AOS ? 0x2808 : 0xE29 )
		{
            Stackable = Core.ML;
			Weight = 1.0;

		    Hue = 2492;
		}

		public SmokeBomb( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				// The item must be in your backpack to use it.
				from.SendLocalizedMessage( 1060640 );
			}
            #region mod by Dies Irae
            if( !ClassSystem.IsThief( from ) )
            {
                from.SendMessage( "You are not allowed to use this item." );
            }
            /*
            else if ( from.Skills.Ninjitsu.Value < 50.0 )
            {
                // You need at least ~1_SKILL_REQUIREMENT~ ~2_SKILL_NAME~ skill to use that ability.
                from.SendLocalizedMessage( 1063013, "50\tNinjitsu" );
            }
            */
            #endregion
			else if ( from.NextSkillTime > DateTime.Now )
			{
				// You must wait a few seconds before you can use that item.
				from.SendLocalizedMessage( 1070772 );
			}
			else if ( from.Mana < 10 )
			{
				// You don't have enough mana to do that.
				from.SendLocalizedMessage( 1049456 );
			}
			else
			{
				SkillHandlers.Hiding.CombatOverride = true;

				if ( from.UseSkill( SkillName.Hiding ) )
				{
					from.Mana -= 10;

			        Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z + 4 ), from.Map, 0x3728, 13 );
			        Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z ), from.Map, 0x3728, 13 );
			        Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z - 4 ), from.Map, 0x3728, 13 );
			        Effects.SendLocationEffect( new Point3D( from.X, from.Y + 1, from.Z + 4 ), from.Map, 0x3728, 13 );
			        Effects.SendLocationEffect( new Point3D( from.X, from.Y + 1, from.Z ), from.Map, 0x3728, 13 );
			        Effects.SendLocationEffect( new Point3D( from.X, from.Y + 1, from.Z - 4 ), from.Map, 0x3728, 13 );

			        Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z + 11 ), from.Map, 0x3728, 13 );
			        Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z + 7 ), from.Map, 0x3728, 13 );
			        Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z + 3 ), from.Map, 0x3728, 13 );
			        Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y + 1, from.Z - 1 ), from.Map, 0x3728, 13 );

			        from.PlaySound( 0x228 );

                    //from.FixedParticles( 0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot );
                    //from.PlaySound( 0x22F );

					Consume();
				}

				SkillHandlers.Hiding.CombatOverride = false;
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}