/***************************************************************************
 *                                  PetShrinkPotion.cs
 *                            		-----------------
 *  begin                	: Febbraio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using Midgard.Engines.PetSystem;
using Server.Mobiles;

namespace Server.Items
{
    public class PetShrinkPotion : BasePetHealthPotion
    {
        #region campi
        public override double PercProperFun { get { return 0.90; } }
        public override int DelayUse { get { return 10; } }
        public override int LabelNumber { get { return 1064341; } } // A Shrink Potion
        #endregion

        #region costruttori
        [Constructable]
        public PetShrinkPotion( int amount )
            : base( PotionEffect.PetShrink, amount )
        {
            Hue = 1152;
        }

        [Constructable]
        public PetShrinkPotion()
            : this( 1 )
        {
        }

        public PetShrinkPotion( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void DoPetHealthEffect( Mobile user, BaseCreature pet )
        {
            /*
            bool NearAnimalTrainer = false;
            foreach( Mobile m in user.GetMobilesInRange( 5 ) )
            {
                if ( m is AnimalTrainer )
                    NearAnimalTrainer = true;
            }
			
            if( !NearAnimalTrainer )
            {
                user.SendLocalizedMessage( 1064344 ); // You must be near an animaltrainer to shrink your pet.
                return;
            }
            */

            if( pet == user )
                user.SendLocalizedMessage( 1064346 ); // You cant shrink yourself!

            else if( Spells.SpellHelper.CheckCombat( user ) )
                user.SendLocalizedMessage( 1064347 ); // You cannot shrink your pet while your fighting.

            else if( pet != null )
            {
                BaseCreature c = pet;

                if( c.BodyValue == 400 || c.BodyValue == 401 && c.Controlled == false )
                {
                    user.SendLocalizedMessage( 1064348 ); // That person gives you a dirty look.
                }
                else if( c.ControlMaster != user && c.Controlled == false )
                {
                    user.SendLocalizedMessage( 1064349 ); // This is not your pet.
                }
                else if( PetUtility.IsPackAnimal( pet ) && ( c.Backpack != null && c.Backpack.Items.Count > 0 ) )
                {
                    user.SendLocalizedMessage( 1064350 ); // You must unload your pets backpack first.
                }
                else if( c.IsDeadPet )
                {
                    user.SendLocalizedMessage( 1064351 ); // You cannot shrink the dead.
                }
                else if( c.Summoned )
                {
                    user.SendLocalizedMessage( 1064352 ); // You cannot shrink a summoned creature.
                }
                else if( c.Combatant != null && c.InRange( c.Combatant, 12 ) && c.Map == c.Combatant.Map )
                {
                    user.SendLocalizedMessage( 1064353 ); // Your pet is fighting, You cannot shrink it yet.
                }
                else if( c.BodyMod != 0 )
                {
                    user.SendLocalizedMessage( 1064354 ); // You cannot shrink your pet while its polymorphed.
                }
                else if( c.Controlled && c.ControlMaster == user )
                {
                    Type type = c.GetType();
                    ShrinkItem si = new ShrinkItem();
                    si.MobType = type;
                    si.Pet = c;
                    si.PetOwner = user;

                    if( c is BaseMount )
                    {
                        BaseMount mount = (BaseMount)c;
                        si.MountID = mount.ItemID;
                    }

                    user.AddToBackpack( si );

                    IEntity p1 = new Entity( Serial.Zero, new Point3D( user.X, user.Y, user.Z ), user.Map );
                    IEntity p2 = new Entity( Serial.Zero, new Point3D( user.X, user.Y, user.Z + 50 ), user.Map );

                    Effects.SendMovingParticles( p2, p1, ShrinkTable.Lookup( c ), 1, 0, true, false, 0, 3, 1153, 1, 0, EffectLayer.Head, 0x100 );
                    user.PlaySound( 492 );

                    c.Controlled = true;
                    c.ControlMaster = null;
                    c.Internalize();

                    c.OwnerAbandonTime = DateTime.MinValue;
                    c.IsStabled = true;
                    c.IsShrinked = true;
                    c.ShrinkItem = si;

                    Consume();
                }
            }
            else
            {
                user.SendLocalizedMessage( 1064345 ); // Only Creatures can be shrinked.
            }
        }
        #endregion

        #region serial deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}