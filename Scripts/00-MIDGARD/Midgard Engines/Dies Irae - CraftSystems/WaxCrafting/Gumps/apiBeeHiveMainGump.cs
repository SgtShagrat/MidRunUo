using System;

using Midgard.Items;
using Server.Network;

using Server;
using Server.Gumps;
using Server.Items;

namespace Midgard.Engines.Apiculture
{
    public class ApiBeeHiveMainGump : Gump
    {
        ApiBeeHive m_Hive;

        public ApiBeeHiveMainGump( Mobile from, ApiBeeHive hive )
            : base( 20, 20 )
        {
            m_Hive = hive;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );
            AddBackground( 37, 26, 205, 161, 3600 );

            //vines
            AddItem( 12, 91, 3307 );
            AddItem( 11, 24, 3307 );
            AddItem( 206, 87, 3307 );
            AddItem( 205, 20, 3307 );

            AddImage( 101, 66, 1417 );  //circle thing
            AddItem( 118, 89, 2330 );   //beehive

            //potions
            AddItem( 195, 46, 3848 );
            AddItem( 185, 96, 3847 );
            AddItem( 190, 71, 3850 );
            AddItem( 183, 121, 3852 );
            AddItem( 186, 146, 3849 );

            //status icons
            AddItem( -5, 76, 882 ); //little bug thing
            AddItem( 41, 121, 4088 );
            AddItem( 45, 148, 3336 );
            AddItem( 44, 49, 5154 );
            AddItem( 46, 100, 6884 );

            //corner boxes
            AddImage( 34, 20, 210 );
            AddImage( 228, 20, 210 );
            AddImage( 34, 172, 210 );
            AddImage( 228, 172, 210 );

            //boxes around status icons
            AddImage( 58, 71, 212 );  //infestation
            AddImage( 58, 96, 212 );  //disease
            AddImage( 58, 121, 212 ); //water
            AddImage( 58, 146, 212 ); //flower

            //potion lables
            AddLabel( 190, 47, 0x481, hive.PotAgility.ToString() );  //agility
            AddLabel( 190, 71, 0x481, hive.PotPoison.ToString() );   //poison
            AddLabel( 190, 96, 0x481, hive.PotCure.ToString() );     //cure
            AddLabel( 190, 121, 0x481, hive.PotHeal.ToString() );     //heal
            AddLabel( 190, 146, 0x481, hive.PotStrength.ToString() ); //strength	

            //status labels
            switch( hive.ParasiteLevel )  //parasites
            {
                case 1:
                    AddLabel( 81, 71, 52, @"-" );
                    break;
                case 2:
                    AddLabel( 81, 71, 37, @"-" );
                    break;
            }
            switch( hive.DiseaseLevel )  //disease
            {
                case 1:
                    AddLabel( 81, 96, 52, @"-" );
                    break;
                case 2:
                    AddLabel( 81, 96, 37, @"-" );
                    break;
            }
            switch( hive.ScaleWater() ) //water
            {
                case ResourceStatus.None:
                    AddLabel( 81, 121, 37, @"X" );
                    break;
                case ResourceStatus.VeryLow:
                    AddLabel( 81, 121, 37, @"-" );
                    break;
                case ResourceStatus.Low:
                    AddLabel( 81, 121, 52, @"-" );
                    break;
                case ResourceStatus.High:
                    AddLabel( 81, 121, 67, @"+" );
                    break;
                case ResourceStatus.VeryHigh:
                    AddLabel( 81, 121, 52, @"+" );
                    break;
                case ResourceStatus.TooHigh:
                    AddLabel( 81, 121, 37, @"+" );
                    break;
            }
            switch( hive.ScaleFlower() ) //flowers
            {
                case ResourceStatus.None:
                    AddLabel( 81, 145, 37, @"X" );
                    break;
                case ResourceStatus.VeryLow:
                    AddLabel( 81, 145, 37, @"-" );
                    break;
                case ResourceStatus.Low:
                    AddLabel( 81, 145, 52, @"-" );
                    break;
                case ResourceStatus.High:
                    AddLabel( 81, 145, 67, @"+" );
                    break;
                case ResourceStatus.VeryHigh:
                    AddLabel( 81, 145, 52, @"+" );
                    break;
                case ResourceStatus.TooHigh:
                    AddLabel( 81, 145, 37, @"+" );
                    break;
            }

            //corner labels
            AddLabel( 40, 20, 0x481, ( (int)hive.HiveStage ).ToString() ); //top left (stage)

            //last growth
            switch( m_Hive.LastGrowth )
            {
                case HiveGrowthIndicator.PopulationDown:
                    AddLabel( 234, 20, 37, "-" );
                    break; //red -
                case HiveGrowthIndicator.PopulationUp:
                    AddLabel( 234, 20, 67, "+" );
                    break; //green +
                case HiveGrowthIndicator.NotHealthy:
                    AddLabel( 234, 20, 37, "!" );
                    break; //red !
                case HiveGrowthIndicator.LowResources:
                    AddLabel( 234, 20, 52, "!" );
                    break; //yellow !
                case HiveGrowthIndicator.Grown:
                    AddLabel( 234, 20, 92, "+" );
                    break; //blue +
            }

            AddLabel( 40, 172, 0x481, "?" ); //help
            AddLabel( 232, 172, 37, @"\" );   //destroy
            AddItem( 214, 176, 6256, 0 );  //destroy

            //misc labels
            if( hive.HiveStage >= HiveStatus.Producing )
                AddLabel( 100, 42, 92, "Colony : " + hive.Population + "0k" );
            else if( hive.HiveStage >= HiveStatus.Brooding )
                AddLabel( 100, 42, 92, "   Brooding" );
            else
                AddLabel( 100, 42, 92, "  Colonizing" );
            switch( hive.OverallHealth ) //overall health
            {
                case HiveHealth.Dying:
                    AddLabel( 116, 146, 37, "Dying" );
                    break;
                case HiveHealth.Sickly:
                    AddLabel( 116, 146, 52, "Sickly" );
                    break;
                case HiveHealth.Healthy:
                    AddLabel( 116, 146, 67, "Healthy" );
                    break;
                case HiveHealth.Thriving:
                    AddLabel( 116, 146, 92, "Thriving" );
                    break;
            }

            //resource
            AddButton( 58, 46, 212, 212, (int)Buttons.ButResource, GumpButtonType.Reply, 0 );
            //help
            AddButton( 34, 172, 212, 212, (int)Buttons.ButHelp, GumpButtonType.Reply, 0 );
            //destroy
            AddButton( 228, 172, 212, 212, (int)Buttons.ButDestroy, GumpButtonType.Reply, 0 );
            //agility
            AddButton( 202, 46, 212, 212, (int)Buttons.ButAgil, GumpButtonType.Reply, 0 );
            //poison
            AddButton( 202, 71, 212, 212, (int)Buttons.ButPois, GumpButtonType.Reply, 0 );
            //cure
            AddButton( 202, 96, 212, 212, (int)Buttons.ButCure, GumpButtonType.Reply, 0 );
            //heal
            AddButton( 202, 121, 212, 212, (int)Buttons.ButHeal, GumpButtonType.Reply, 0 );
            //strength
            AddButton( 202, 146, 212, 212, (int)Buttons.ButStr, GumpButtonType.Reply, 0 );

        }

        public enum Buttons
        {
            ButResource = 1,
            ButDestroy,
            ButHelp,
            ButAgil,
            ButPois,
            ButCure,
            ButHeal,
            ButStr,
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == 0 || m_Hive.Deleted || !from.InRange( m_Hive.GetWorldLocation(), 3 ) )
                return;

            if( !m_Hive.IsAccessibleTo( from ) )
            {
                m_Hive.LabelTo( from, 1065379 ); // You cannot use that.
                return;
            }

            switch( info.ButtonID )
            {
                case 1: //Resources
                    {
                        from.SendGump( new ApiBeeHiveProductionGump( from, m_Hive ) );
                        break;
                    }
                case 2: //Destroy
                    {
                        from.SendGump( new ApiBeeHiveDestroyGump( from, m_Hive ) );
                        break;
                    }
                case 3: //Help
                    {
                        from.SendGump( new ApiBeeHiveMainGump( from, m_Hive ) );
                        from.SendGump( new ApiBeeHiveHelpGump( from, 0 ) );
                        break;
                    }
                case 4: //Agility Potion
                    {
                        AddPotion( from, PotionEffect.AgilityGreater );

                        break;
                    }
                case 5:	//Poison Potion
                    {
                        AddPotion( from, PotionEffect.PoisonGreater, PotionEffect.PoisonDeadly );

                        break;
                    }
                case 6:	//Cure Potion
                    {
                        AddPotion( from, PotionEffect.CureGreater );

                        break;
                    }
                case 7:	//Heal Potion
                    {
                        AddPotion( from, PotionEffect.HealGreater );

                        break;
                    }
                case 8: //Strength Potion
                    {
                        AddPotion( from, PotionEffect.StrengthGreater );

                        break;
                    }
            }
        }

        private void AddPotion( Mobile from, params PotionEffect[] effects )
        {
            Item item = GetPotion( from, effects );

            if( item != null )
            {
                m_Hive.Pour( from, item );
            }
            else
            {
                from.SendLocalizedMessage( 1061884 ); // You don't have any strong potions of that type in your pack.
            }

            from.SendGump( new ApiBeeHiveMainGump( from, m_Hive ) );
        }

        public static Item GetPotion( Mobile from, PotionEffect[] effects )
        {
            if( from.Backpack == null )
                return null;

            Item[] items = from.Backpack.FindItemsByType( new Type[] { typeof( BasePotion ), typeof( PotionKeg ) } );

            foreach( Item item in items )
            {
                if( item is BasePotion )
                {
                    BasePotion potion = (BasePotion)item;

                    if( Array.IndexOf( effects, potion.PotionEffect ) >= 0 )
                        return potion;
                }
                else
                {
                    PotionKeg keg = (PotionKeg)item;

                    if( keg.Held > 0 && Array.IndexOf( effects, keg.Type ) >= 0 )
                        return keg;
                }
            }

            return null;
        }
    }
}