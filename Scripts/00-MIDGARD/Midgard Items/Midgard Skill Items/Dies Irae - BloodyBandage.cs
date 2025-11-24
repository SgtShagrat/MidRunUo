/***************************************************************************
 *                               Dies Irae - BloodyBandage.cs
 *
 *   begin                : 27 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Engines.Harvest;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Midgard.Items
{
    public class BloodyBandage : Item
    {
        public override string DefaultName { get { return "bloody bandage"; } }

        public override double DefaultWeight
        {
            get { return 0.1; }
        }

        [Constructable]
        public BloodyBandage()
            : this( 1 )
        {
        }

        [Constructable]
        public BloodyBandage( int amount )
            : base( 0xE20 )
        {
            Stackable = true;
            Amount = amount;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( GetWorldLocation(), 2 ) )
            {
                from.RevealingAction( true );
                from.SendMessage( "Where will thou clean this bloody bandage?" );
                from.Target = new InternalTarget( this );
            }
            else
                from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
        }

        public void OnTarget( Mobile from, object targ )
        {
            bool clean = false;

            int tileID;
            Map map;
            Point3D loc;

            if( targ is BaseBeverage )
            {
                BaseBeverage bev = (BaseBeverage)targ;

                if( bev.IsEmpty || !bev.ValidateUse( from, true ) || bev.Content != BeverageType.Water )
                    return;

                clean = true;
            }
            else if( targ is Item )
            {
                Item item = (Item)targ;
                IWaterSource src;

                src = ( item as IWaterSource );

                if( src == null && item is AddonComponent )
                    src = ( ( (AddonComponent)item ).Addon as IWaterSource );

                if( src == null || src.Quantity <= 0 )
                    return;

                if( from.Map != item.Map || !from.InRange( item.GetWorldLocation(), 2 ) || !from.InLOS( item ) )
                {
                    from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
                    return;
                }

                clean = true;
            }
            else if( targ is LandTarget && IsWaterLandTile( targ ) )
            {
                clean = true;
            }
            else if( Fishing.System.GetHarvestDetails( from, null, targ, out tileID, out map, out loc ) )
            {
                int[] tiles = Fishing.System.Definition.Tiles;

				bool contains = false;

				for ( int i = 0; !contains && i < tiles.Length; i += 2 )
					contains = ( tileID >= tiles[i] && tileID <= tiles[i + 1] );

				clean = contains;
            }

            if( !clean )
                return;

            int amount = Amount / 2;
            Consume( Amount );

            if( amount > 0 )
            {
                from.AddToBackpack( new Bandage( amount ) );
                from.SendMessage( "You clean the bloody bandage." );
                from.PlaySound( Utility.RandomList( new int[] { 1233, 37 } ) );
            }
        }

        private static int[] m_WaterTiles = new int[]
			{
				0x00A8, 0x00AB,
				0x0136, 0x0137,
				0x5797, 0x579C,
				0x746E, 0x7485,
				0x7490, 0x74AB,
				0x74B5, 0x75D5
			};

        private static bool IsWaterLandTile( object targ )
        {
            if( targ is LandTarget )
            {
                int tileID = ( (LandTarget)targ ).TileID;
                bool contains = false;

                for( int i = 0; !contains && i < m_WaterTiles.Length; i += 2 )
                    contains = ( tileID >= m_WaterTiles[ i ] && tileID <= m_WaterTiles[ i + 1 ] );

                return contains;
            }

            return false;
        }

        #region serialization
        public BloodyBandage( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        private class InternalTarget : Target
        {
            private BloodyBandage m_Bandage;

            public InternalTarget( BloodyBandage bandage )
                : base( 2, false, TargetFlags.None )
            {
                m_Bandage = bandage;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Bandage.Deleted )
                    return;

                m_Bandage.OnTarget( from, targeted );
            }
        }
    }
}