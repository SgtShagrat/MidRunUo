/***************************************************************************
 *                               LureStone.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.SpellSystem
{
    public class LureStone : Item
    {
        private InternalItem m_Item;
        private Mobile m_Owner;
        private List<BaseCreature> m_CalledList;

        public override bool BlocksFit
        {
            get { return true; }
        }

        public override bool HandlesOnMovement
        {
            get { return true; }
        }

        [CommandProperty( AccessLevel.Developer )]
        public bool IsInDebugMode { get; set; }

        public LureStone( Mobile owner )
            : base( 0x1355 )
        {
            Movable = false;

            m_Item = new InternalItem( this );
            m_Owner = owner;
            m_CalledList = new List<BaseCreature>();

            IsInDebugMode = false;
        }

        public LureStone( Serial serial )
            : base( serial )
        {
        }

        public override void OnLocationChange( Point3D oldLocation )
        {
            if( m_Item != null )
                m_Item.Location = new Point3D( X, Y - 1, Z );
        }

        public override void OnMapChange()
        {
            if( m_Item != null )
                m_Item.Map = Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if( m_Item != null )
                m_Item.Delete();
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            if( m_Owner == null && !Deleted )
                Delete();
            else
            {
                BaseCreature bc = m as BaseCreature;
                if( bc == null )
                    return;

                if( !bc.Body.IsAnimal )
                    return;

                if( bc.Controlled || bc.Summoned || bc.IsParagon )
                    return;

                if( m_Owner != null )
                {
                    double lore = m_Owner.Skills[ SkillName.AnimalLore ].Value;
                    double taming = m_Owner.Skills[ SkillName.Spellweaving ].Value;

                    if( m.InRange( this, (int)lore + 5 ) )
                    {
                        if( bc.Combatant == null || !bc.Combatant.Alive || bc.Combatant.Deleted )
                        {
                            double chanceToCall = ( lore + taming ) / 240.0;

                            if( Utility.RandomDouble() < chanceToCall )
                            {
                                if( !m_CalledList.Contains( bc ) )
                                {
                                    m_CalledList.Add( bc );
                                    bc.TargetLocation = new Point2D( X, Y );
                                    bc.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "* Fells the call of a Lure Stone *" );
                                    m_Owner.SendMessage( "A {0} is coming to your LureStone", bc.GetType().Name );
                                }
                            }
                        }
                    }
                }
            }
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_Item );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Item = reader.ReadItem() as InternalItem;
        }
        #endregion

        #region InternalItem
        private class InternalItem : Item
        {
            private LureStone m_Stone;

            public InternalItem( LureStone stone )
                : base( 0x1356 )
            {
                Movable = false;
                m_Stone = stone;
            }

            public override void OnLocationChange( Point3D oldLocation )
            {
                if( m_Stone != null )
                    m_Stone.Location = new Point3D( X, Y + 1, Z );
            }

            public override void OnMapChange()
            {
                if( m_Stone != null )
                    m_Stone.Map = Map;
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if( m_Stone != null )
                    m_Stone.Delete();
            }

            #region serial-deserial
            public InternalItem( Serial serial )
                : base( serial )
            {
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( 0 ); // version

                writer.Write( m_Stone );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();

                m_Stone = reader.ReadItem() as LureStone;
            }
            #endregion
        }
        #endregion
    }
}