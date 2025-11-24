using System;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.CheeseCrafting
{
    public enum MilkTypes
    {
        None,
        Sheep,
        Goat,
        Cow
    }

    public enum CheeseQuality
    {
        Low,
        Regular,
        Exceptional
    }

    public class CheeseCrafingHelper
    {
        public static string GetMilkName( MilkTypes type )
        {
            switch( type )
            {
                case MilkTypes.Cow:
                    return "cow";
                case MilkTypes.Goat:
                    return "goat";
                case MilkTypes.Sheep:
                    return "sheep";
                default:
                    return "undefined";
            }
        }

        public static MilkTypes GetMilkType( Type t )
        {
            if( t == typeof( Sheep ) )
                return MilkTypes.Sheep;
            else if( t == typeof( Goat ) )
                return MilkTypes.Goat;
            else if( t == typeof( Cow ) )
                return MilkTypes.Cow;
            else
                return MilkTypes.None;
        }

        public static Item GetMilkBottle( MilkTypes type, Mobile crafter, bool exceptional )
        {
            CheeseQuality quality = exceptional ? CheeseQuality.Exceptional : CheeseQuality.Regular;

            switch( type )
            {
                case MilkTypes.Cow:
                    return new CowMilkBottle( crafter, quality );
                case MilkTypes.Goat:
                    return new GoatMilkBottle( crafter, quality );
                case MilkTypes.Sheep:
                    return new SheepMilkBottle( crafter, quality );
                default:
                    return null;
            }
        }
    }
}