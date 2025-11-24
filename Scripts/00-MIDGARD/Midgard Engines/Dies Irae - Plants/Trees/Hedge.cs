using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class HedgeSeed : BaseSeed
    {
        #region properties from BaseSeed
        public override Type[] PlantTypes { get { return new Type[] { typeof( Hedge1 ), typeof( Hedge2 ) }; } }
        public override string PlantName { get { return "hedge"; } }
        #endregion

        #region properties from ISowable
        public override bool CanGrowFarm { get { return true; } }
        #endregion

        #region costruttori
        [Constructable]
        public HedgeSeed()
            : this( 1 )
        {
        }

        [Constructable]
        public HedgeSeed( int amount )
            : base( amount )
        {
        }

        public HedgeSeed( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }

    public class Hedge1 : BaseTree, IScissorable
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0xCBB, 0x0C91, 0x0DB8 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a hedge sapling", "a young hedge", "a hedge" }; } }
        public override double MinSkillToHarvest { get { return 50; } }
        public override Type TypeOfParentSeed { get { return typeof( HedgeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return string.Empty; } }
        public override string CropPluralName { get { return string.Empty; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public Hedge1( Mobile owner )
            : base( owner )
        {
        }

        public Hedge1( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return null;
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( BasePlant.CheckAccess( from, this ) )
            {
                int oldItemID = ItemID;
                switch( ItemID )
                {
                    case 0xC91:
                        ItemID = 0xC8F;
                        break;
                    case 0xC92:
                        ItemID = 0xC90;
                        break;
                    case 0xDB8:
                        ItemID = 0xC8F;
                        break;
                    case 0xDB9:
                        ItemID = 0xC90;
                        break;
                    default:
                        break;
                }
                if( ItemID != oldItemID )
                {
                    from.SendMessage( "You carefully trim that hedge." );
                    return true;
                }
            }
            else
                from.SendMessage( "You cannot trim that edge." );
            return false;
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }

    public class Hedge2 : BaseTree, IScissorable
    {
        #region properties from BasePlant
        // Variables concerning phases and ids
        public override int[] PhaseIDs { get { return new int[] { 0x913, 0xCBB, 0x0C92, 0x0DB9 }; } }
        public override string[] PhaseName { get { return new string[] { "a pile of dirt", "a hedge sapling", "a young hedge", "a hedge" }; } }
        public override double MinSkillToHarvest { get { return 50; } }
        public override Type TypeOfParentSeed { get { return typeof( HedgeSeed ); } }

        // Variables concerning produce action
        public override bool CanProduce { get { return false; } }
        public override int ProduceTick { get { return 0; } }
        public override int Capacity { get { return 0; } }
        public override string CropName { get { return string.Empty; } }
        public override string CropPluralName { get { return string.Empty; } }
        #endregion

        #region properties from BaseTree
        public override int[] LeavesIDs { get { return new int[] { }; } }
        public override int[] ProductLeavesIDs { get { return new int[] { }; } }
        public override int[] DriedLeavesIDs { get { return new int[] { }; } }
        #endregion

        #region costruttori
        [Constructable]
        public Hedge2( Mobile owner )
            : base( owner )
        {
        }

        public Hedge2( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override Item GetCropObject()
        {
            return null;
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( BasePlant.CheckAccess( from, this ) )
            {
                int oldItemID = ItemID;
                switch( ItemID )
                {
                    case 0xC91:
                        ItemID = 0xC8F;
                        break;
                    case 0xC92:
                        ItemID = 0xC90;
                        break;
                    case 0xDB8:
                        ItemID = 0xC8F;
                        break;
                    case 0xDB9:
                        ItemID = 0xC90;
                        break;
                    default:
                        break;
                }
                if( ItemID != oldItemID )
                {
                    from.SendMessage( "You carefully trim that hedge" );
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }
}
