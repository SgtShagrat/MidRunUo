using Server;
using Server.Network;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3db5 Pipe trad. pipa - ( da inserire nel vendor e craft tinker )
    /// Se utilizzata con doppioclick sull'immagine del gump - quindi pipa indossata - fa comparire un emote "fuma la pipa" 
    /// </summary>
    public class Pipe : Item
    {
        public override string DefaultName { get { return "pipe"; } }

        [Constructable]
        public Pipe()
        {
            ItemID = 0x3db5;
            Weight = 1.0;
            Layer = Layer.OneHanded;
        }

        public override void OnDoubleClick( Mobile m )
        {
            if( Parent != m )
            {
                m.SendMessage( "You must be wearing the pipe to use it!" );
            }
            else
            {
                m.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "*fuma la pipa*" );
                Effects.SendLocationEffect( new Point3D( m.X, m.Y, m.Z + 20 ), m.Map, 0x3735, 30, 1 );
            }

            base.OnDoubleClick( m );
        }

        #region serialization
        public Pipe( Serial serial )
            : base( serial )
        {
        }

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