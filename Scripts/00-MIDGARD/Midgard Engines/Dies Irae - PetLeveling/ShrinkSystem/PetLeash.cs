/***************************************************************************
 *                                  PetLeash.cs
 *                            		-----------
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
using Server.Targeting;

namespace Server.Items
{
    public class PetLeash : Item
    {
        #region campi
        private int m_Charges = 10;
        public override int LabelNumber { get { return 1064340; } } // A PetLeash
        #endregion

        #region proprietà
        [CommandProperty( AccessLevel.GameMaster )]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; InvalidateProperties(); }
        }
        #endregion

        #region costruttori
        [Constructable]
        public PetLeash()
            : base( 0x1374 )
        {
            Weight = 1.0;
        }
        public PetLeash( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );
            list.Add( 1060658, "Charges\t{0}", m_Charges.ToString() );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
            else if( from.Skills[ SkillName.AnimalTaming ].Value > 75 )
            {
                from.Target = new LeashTarget( this );
                from.SendLocalizedMessage( 1064342 ); // What do you wish to shrink?
            }
            else
            {
                from.SendLocalizedMessage( 1064343 ); // You must have 75 animal taming to use a pet leash.
            }
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
            writer.Write( m_Charges );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
            m_Charges = reader.ReadInt();
        }
        #endregion

        #region LeashTarget
        private class LeashTarget : Target
        {
            #region campi
            private PetLeash m_PetLeash;
            #endregion

            #region costruttori
            public LeashTarget( PetLeash petleash )
                : base( 10, false, TargetFlags.None )
            {
                m_PetLeash = petleash;
            }
            #endregion

            #region metodi
            protected override void OnTarget( Mobile user, object target )
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

                BaseCreature pet = target as BaseCreature;

                if( pet == null )
                    user.SendLocalizedMessage( 1064345 ); // Only Creatures can be shrinked.

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

                        m_PetLeash.Charges--;
                        if( m_PetLeash.Charges <= 0 )
                            m_PetLeash.Delete();
                    }
                    else
                    {
                        user.SendLocalizedMessage( 1064345 ); // Only Creatures can be shrinked.
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
