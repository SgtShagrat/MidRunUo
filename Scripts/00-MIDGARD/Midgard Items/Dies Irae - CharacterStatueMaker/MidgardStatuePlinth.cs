/***************************************************************************
 *                               MidgardStatuePlinth.cs
 *
 *   begin                : 07 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Multis;

namespace Midgard.Items.StatueSystem
{
    public class MidgardStatuePlinth : Static, IAddon, IEngravable, IChopable
    {
        public MidgardStatuePlinth( MidgardStatue statue )
            : base( 0x32F2 )
        {
            Statue = statue;

            InvalidateHue();
        }

        public override string DefaultName
        {
            get { return "a statue plint"; }
        }

        public MidgardStatue Statue { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public string EngravedText { get; set; }

        #region IAddon Members
        public Item Deed
        {
            get { return new MidgardStatueDeed( Statue ); }
        }

        public virtual bool CouldFit( IPoint3D p, Map map )
        {
            Point3D point = new Point3D( p.X, p.Y, p.Z );

            return map != null && map.CanFit( point, 20 );
        }
        #endregion

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if( Statue != null && !Statue.Deleted )
                Statue.Delete();
        }

        public override void OnMapChange()
        {
            if( Statue != null )
                Statue.Map = Map;
        }

        public override void OnLocationChange( Point3D oldLocation )
        {
            if( Statue != null )
                Statue.Location = new Point3D( X, Y, Z + 5 );
        }

        public void InvalidateHue()
        {
            if( Statue != null )
                Hue = CraftResources.GetHue( Statue.Material );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Statue == null )
            {
                base.OnSingleClick( from );
                return;
            }

            LabelTo( from, "a {0} statue of {1}", GetMaterialName( Statue.Material ).ToLower(), Statue.Name );

            if( Statue.SculptedBy != null && Statue.SculptedBy.Name != null )
                LabelTo( from, "(sculpted by {0} - {1})", Statue.SculptedBy.Name, Statue.SculptedOn.ToShortDateString() );

            if( EngravedText != null )
                LabelTo( from, EngravedText );
        }

        private static string GetMaterialName( CraftResource material )
        {
            CraftResourceInfo info = CraftResources.GetInfo( material );
            return info != null ? info.Name : "";
        }

        public virtual void OnChop( Mobile from )
        {
            BaseHouse house = BaseHouse.FindHouseAt( this );

            if( house == null || !house.IsOwner( from ) || !house.Addons.Contains( this ) )
                return;

            Effects.PlaySound( GetWorldLocation(), Map, 0x3B3 );
            from.SendLocalizedMessage( 500461 ); // You destroy the item.

            Statue.Demolish( from );

            house.Addons.Remove( this );
        }

        #region serialization
        public MidgardStatuePlinth( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            writer.Write( Statue );
            writer.Write( EngravedText );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            Statue = reader.ReadMobile() as MidgardStatue;
            EngravedText = reader.ReadString();
        }
        #endregion
    }
}