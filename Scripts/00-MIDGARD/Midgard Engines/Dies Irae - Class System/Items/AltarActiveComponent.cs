/***************************************************************************
 *                               AltarActiveComponent.cs
 *
 *   begin                : 10 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.Classes
{
    public abstract class AltarActiveComponent : AddonComponent
    {
        public AltarActiveComponent( int itemID )
            : base( itemID )
        {
            Movable = false;

            Light = LightType.Circle300;
        }

        public abstract ClassSystem AltarSystem { get; }

        public override void OnDoubleClick( Mobile from )
        {
            if( Addon != null )
            {
                if( from.InRange( Location, 1 ) )
                {
                    foreach( AddonComponent c in Addon.Components )
                    {
                        if( c is AltarActiveComponent )
                        {
                            ClassPlayerState state = ClassPlayerState.Find( from );
                            if( state == null || state.ClassSystem != AltarSystem )
                            {
                                from.SendMessage( from.Language == "ITA" ? "Non ti è permesso usarlo." : "Thou are not allowed to do that." );
                                return;
                            }
                            else
                            {
                                from.CloseGump( typeof( ClassRitualGump ) );
                                from.SendGump( new ClassRitualGump( state, this, AltarSystem ) );
                            }
                        }
                    }
                }
                else
                    from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
            }
        }

        #region serialization
        public AltarActiveComponent( Serial serial )
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

            Light = LightType.Circle300;
        }
        #endregion
    }
}