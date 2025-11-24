using System;

using Server.Targeting;

namespace Server.Mobiles
{
    public class Herding
    {
        private static readonly double HerdingRequiredToMate = 80.0;
        private static readonly double TamingRequiredToMate = 80.0;
        private static readonly double LoreRequiredToMate = 80.0;

        public static void DoHerdingAction( Mobile from, object targ )
        {
            if( from.Skills[ SkillName.Herding ].Value < HerdingRequiredToMate )
            {
                from.SendMessage( "You are not skilled enough in herding to mate this creature." );
                return;
            }
            else if( from.Skills[ SkillName.AnimalTaming ].Value < TamingRequiredToMate )
            {
                from.SendMessage( "You are not skilled enough in animal taming to mate this creature." );
                return;
            }
            else if( from.Skills[ SkillName.AnimalLore ].Value < LoreRequiredToMate )
            {
                from.SendMessage( "You are not skilled enough in animal lore to mate this creature." );
                return;
            }

            if( !( targ is BaseCreature ) )
            {
                from.SendLocalizedMessage( 502472 ); // You don't seem to be able to persuade that to move.
                return;
            }

            BaseCreature bc = (BaseCreature)targ;

            if( !IsHerdable( bc ) )
            {
                from.SendLocalizedMessage( 502468 ); // That is not a herdable animal.
            }
            else if( !bc.Controlled )
            {
                from.SendLocalizedMessage( 502475 ); // Click where you wish the animal to go.
                from.Target = new InternalTarget( bc );
            }
            else if( bc.ControlMaster != from )
            {
                from.SendMessage( "That animal does not belong to you." );
            }
            else if( bc.Warmode )
            {
                from.SendMessage( "That animal is not calm enough." );
            }
            else if( !( bc.IsGeneticCreature ) )
            {
                from.SendMessage( "That animal cannot breed." );
            }
            else if( bc.CanBePregnant )
            {
                from.SendMessage( "This female is ready for pregnancy, select the father." );
                from.Target = new SelectFatherTarget( bc );
            }
            else
            {
                from.SendMessage( bc.Female ? "That female is too young to get pregnant." : "That's a male animal." );
            }
        }

        public static bool IsHerdable( BaseCreature bc )
        {
            return bc.Body.IsAnimal;
        }

        private class InternalTarget : Target
        {
            private readonly BaseCreature m_Creature;

            public InternalTarget( BaseCreature c )
                : base( 10, true, TargetFlags.None )
            {
                m_Creature = c;
            }

            protected override void OnTarget( Mobile from, object targ )
            {
                if( targ is IPoint2D )
                {
                    if( from.CheckTargetSkill( SkillName.Herding, m_Creature, 0, 100 ) )
                    {
                        m_Creature.TargetLocation = new Point2D( (IPoint2D)targ );
                        from.SendLocalizedMessage( 502479 ); // The animal walks where it was instructed to.
                    }
                    else
                    {
                        from.SendLocalizedMessage( 502472 ); // You don't seem to be able to persuade that to move.
                    }
                }
            }
        }

        private class SelectFatherTarget : Target
        {
            private readonly BaseCreature m_Creature;

            public SelectFatherTarget( BaseCreature c )
                : base( 10, true, TargetFlags.None )
            {
                m_Creature = c;
            }

            protected override void OnTarget( Mobile from, object targ )
            {
                BaseCreature father = targ as BaseCreature;
                if( father == null )
                    return;

                if( m_Creature == null )
                    return;

                if( father == m_Creature )
                {
                    from.SendMessage( "Hey, don't yoke with that poor creature!" );
                    return;
                }

                AnimalBreeder animalBreeder = AnimalBreeder.FindBreeder( from );

                if( animalBreeder == null )
                {
                    from.SendMessage( "You must be near the animal breeder." );
                }
                else if( m_Creature.GetDistanceToSqrt( father ) > 2 )
                {
                    from.SendMessage( "The father is too far away." );
                }
                else if( !m_Creature.InLOS( father ) )
                {
                    from.SendMessage( "The father is out of reach." );
                }
                else if( father.Warmode )
                {
                    from.SendMessage( "The father is too aggressive." );
                }
                else if( !animalBreeder.CanStable( from ) )
                {
                    animalBreeder.SayTo( from, 1042565 ); // You have too many pets in the stables!
                }
                else
                {
                    double herding = from.Skills[ SkillName.Herding ].Value;
                    double taming = from.Skills[ SkillName.AnimalTaming ].Value;
                    double lore = from.Skills[ SkillName.AnimalLore ].Value;

                    double mean = ( herding + taming + lore ) / 3.0; // from 80 to 100

                    double malus = 100 - mean; // from 0 to 20
                    if( malus < 0 )
                        malus = 0;
                    else if( malus > 20 )
                        malus = 20;

                    double min = m_Creature.MinTameSkill - 30;
                    double max = m_Creature.MinTameSkill + 30 + Utility.Random( 10 ) + malus;

                    if( max <= from.Skills[ SkillName.Herding ].Value )
                        m_Creature.PrivateOverheadMessage( Network.MessageType.Regular, 0x3B2, 502471, from.NetState ); // That wasn't even challenging.

                    if( from.CheckTargetSkill( SkillName.Herding, m_Creature, min, max ) )
                    {
                        from.SendMessage( m_Creature.GetPregnant( father ) ? "The female is now pregnant." : "That cannot be the father!" );

                        if( m_Creature.IsPregnant )
                        {
                            animalBreeder.EndStable( from, m_Creature );
                            animalBreeder.SayTo( from, true, "I will take care of this pregnant creature. Return here in {0} days to get your youngling.", m_Creature.DeliveryTimeInUoDays );
                        }
                    }
                    else
                    {
                        from.SendMessage( "You don't seem able to persuade her ..." );
                    }
                }
            }
        }
    }
}