/***************************************************************************
 *                               BaseAdvancedFish.cs
 *
 *   begin                : 19 settembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;

namespace Midgard.Engines.AdvancedFishing
{
    public abstract class BaseAdvancedFish : Item, ICarvable
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public int FishWeight { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int CarveScalar
        {
            get { return 100; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual FishHabitat Habitat
        {
            get { return FishHabitat.None; }
        }

        public override string DefaultName
        {
            get { return "a fish"; }
        }

        public void UpdateWeight()
        {
            Weight = FishWeight / 1000.0;
            if( Weight < 1.0 )
                Weight = 1.0;
        }

        protected BaseAdvancedFish( int fishWeight, int itemID )
            : base( itemID )
        {
            FishWeight = fishWeight;
            UpdateWeight();
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            string weight = TextHelper.Text( 10020013, from.TrueLanguage ); // "weight {0}gr."
            LabelTo( from, weight, FishWeight );
        }

        #region serialization
        public BaseAdvancedFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( FishWeight );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            FishWeight = reader.ReadInt();
        }
        #endregion

        #region ICarvable members
        public virtual void Carve( Mobile from, Item item )
        {
            int amount = Math.Max( FishWeight / CarveScalar, 1 );

            base.ScissorHelper( from, new RawFishSteak(), amount );
        }
        #endregion
    }
}