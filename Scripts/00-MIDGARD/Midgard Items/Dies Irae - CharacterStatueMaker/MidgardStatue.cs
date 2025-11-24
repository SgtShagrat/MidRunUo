/***************************************************************************
 *                               MidgardStatue.cs
 *
 *   begin                : 07 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items.StatueSystem
{
    public class MidgardStatue : Mobile
    {
        private int m_Animation;
        private int m_Frames;
        private CraftResource m_Material;
        private MidgardStatuePose m_Pose;

        public MidgardStatue( Mobile from )
        {
            m_Pose = MidgardStatuePose.Ready;
            m_Material = CraftResource.OldBronze;

            SculptedBy = from;
            SculptedOn = DateTime.Now;

            Direction = Direction.South;
            AccessLevel = AccessLevel.Counselor;

            Hits = HitsMax;
            Blessed = true;
            Frozen = true;

            CloneBody( from );
            CloneClothes( from );
            InvalidateHues();
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public MidgardStatuePose Pose
        {
            get { return m_Pose; }
            set
            {
                m_Pose = value;
                InvalidatePose();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource Material
        {
            get { return m_Material; }
            set
            {
                m_Material = value;

                InvalidateHues();
                InvalidatePose();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile SculptedBy { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime SculptedOn { get; private set; }

        public MidgardStatuePlinth Plinth { get; set; }

        public override bool ShowFrozenStatus
        {
            get { return false; }
        }

        public override bool ShowInvulStatus
        {
            get { return false; }
        }

        public override void OnDoubleClick( Mobile from )
        {
            DisplayPaperdollTo( from );
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if( Plinth != null && !Plinth.Deleted )
                Plinth.Delete();
        }

        protected override void OnMapChange( Map oldMap )
        {
            InvalidatePose();

            if( Plinth != null )
                Plinth.Map = Map;
        }

        protected override void OnLocationChange( Point3D oldLocation )
        {
            InvalidatePose();

            if( Plinth != null )
                Plinth.Location = new Point3D( X, Y, Z - 5 );
        }

        public override bool CanBeRenamedBy( Mobile from )
        {
            return false;
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public void OnRequestedAnimation( Mobile from )
        {
            from.Send( new UpdateMidgardStatueAnimation( this, 1, m_Animation, m_Frames ) );
        }

        public void Sculpt( Mobile by )
        {
            SculptedBy = by;
            SculptedOn = DateTime.Now;

            InvalidateProperties();
        }

        public void Demolish( Mobile by )
        {
            MidgardStatueDeed deed = new MidgardStatueDeed( null );

            if( by.PlaceInBackpack( deed ) )
            {
                Internalize();

                deed.Statue = this;

                if( Plinth != null )
                    Plinth.Internalize();
            }
            else
            {
                by.SendLocalizedMessage( 500720 ); // You don't have enough room in your backpack!
                deed.Delete();
            }
        }

        public void Restore( MidgardStatue from )
        {
            m_Material = from.Material;
            m_Pose = from.Pose;

            Direction = from.Direction;

            CloneBody( from );
            CloneClothes( from );

            InvalidateHues();
            InvalidatePose();
        }

        public void CloneBody( Mobile from )
        {
            Name = from.Name;
            BodyValue = from.BodyValue;
            HairItemID = from.HairItemID;
            FacialHairItemID = from.FacialHairItemID;
        }

        public void CloneClothes( Mobile from )
        {
            for( int i = Items.Count - 1; i >= 0; i-- )
                Items[ i ].Delete();

            for( int i = from.Items.Count - 1; i >= 0; i-- )
            {
                Item item = from.Items[ i ];

                if( item.Layer != Layer.Backpack && item.Layer != Layer.Mount && item.Layer != Layer.Bank )
                    AddItem( CloneItem( item ) );
            }
        }

        public Item CloneItem( Item item )
        {
            var cloned = new Item( item.ItemID );
            cloned.Layer = item.Layer;
            cloned.Name = item.Name;
            cloned.Hue = item.Hue;
            cloned.Weight = item.Weight;
            cloned.Movable = false;

            return cloned;
        }

        public void InvalidateHues()
        {
            Hue = CraftResources.GetHue( Material );

            HairHue = Hue;

            if( FacialHairItemID > 0 )
                FacialHairHue = Hue;

            for( int i = Items.Count - 1; i >= 0; i-- )
                Items[ i ].Hue = Hue;

            if( Plinth != null )
                Plinth.InvalidateHue();
        }

        public void InvalidatePose()
        {
            switch( m_Pose )
            {
                case MidgardStatuePose.Ready:
                    m_Animation = 4;
                    m_Frames = 0;
                    break;
                case MidgardStatuePose.Casting:
                    m_Animation = 16;
                    m_Frames = 2;
                    break;
                case MidgardStatuePose.Salute:
                    m_Animation = 33;
                    m_Frames = 1;
                    break;
                case MidgardStatuePose.AllPraiseMe:
                    m_Animation = 17;
                    m_Frames = 4;
                    break;
                case MidgardStatuePose.Fighting:
                    m_Animation = 31;
                    m_Frames = 5;
                    break;
                case MidgardStatuePose.HandsOnHips:
                    m_Animation = 6;
                    m_Frames = 1;
                    break;
            }

            if( Map != null )
            {
                ProcessDelta();

                Packet p = null;

                IPooledEnumerable eable = Map.GetClientsInRange( Location );

                foreach( NetState state in eable )
                {
                    state.Mobile.ProcessDelta();

                    if( p == null )
                        p = Packet.Acquire( new UpdateMidgardStatueAnimation( this, 1, m_Animation, m_Frames ) );

                    state.Send( p );
                }

                Packet.Release( p );

                eable.Free();
            }
        }

        #region serialization
        public MidgardStatue( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            writer.Write( (int)m_Pose );
            writer.Write( (int)m_Material );

            writer.Write( SculptedBy );
            writer.Write( SculptedOn );
            
            writer.Write( Plinth );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            m_Pose = (MidgardStatuePose)reader.ReadInt();
            m_Material = (CraftResource)reader.ReadInt();

            SculptedBy = reader.ReadMobile();
            SculptedOn = reader.ReadDateTime();
            
            Plinth = reader.ReadItem() as MidgardStatuePlinth;

            InvalidatePose();

            Frozen = true;
        }
        #endregion
    }
}