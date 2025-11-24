/***************************************************************************
 *                               DeveloperRobe.cs
 *
 *   begin                : 01 marzo 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class DeveloperRobe : BaseSuit
    {
        [Constructable]
        public DeveloperRobe()
            : base( AccessLevel.Developer, 0x035, 0x204F )
        {
        }

        #region serialization
        public DeveloperRobe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}