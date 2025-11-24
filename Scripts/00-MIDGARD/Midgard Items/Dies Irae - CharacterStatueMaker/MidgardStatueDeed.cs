/***************************************************************************
 *                               MidgardStatueDeed.cs
 *
 *   begin                : 07 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items.StatueSystem
{
    public class MidgardStatueDeed : Item
    {
        public MidgardStatueDeed( MidgardStatue statue )
            : base( 0x14F0 )
        {
            Statue = statue;

            LootType = LootType.Blessed;
            Weight = 1.0;

            if( Statue != null )
                Hue = CraftResources.GetHue( statue.Material );
        }

        public override string DefaultName
        {
            get { return "a midgard statue deed"; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public MidgardStatue Statue { get; set; }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsChildOf( from.Backpack ) )
            {
                if( !from.IsBodyMod )
                {
                    from.SendLocalizedMessage( 1076194 ); // Select a place where you would like to put your statue.
                    from.Target = new MidgardStatueTarget( Statue == null ? CraftResource.OldBronze : Statue.Material, this );
                }
                else
                    from.SendLocalizedMessage( 1073648 ); // You may only proceed while in your original state...
            }
            else
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if( Statue != null )
                Statue.Delete();
        }

        #region serialization
        public MidgardStatueDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            writer.Write( Statue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            Statue = reader.ReadMobile() as MidgardStatue;
        }
        #endregion
    }
}