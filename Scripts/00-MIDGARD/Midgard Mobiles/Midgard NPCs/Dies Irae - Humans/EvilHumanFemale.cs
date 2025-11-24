/***************************************************************************
 *                               EvilHumanFemale.cs
 *
 *   begin                : 21 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;

namespace Midgard.Mobiles
{
    public class EvilHumanFemale : BaseHuman
    {
        public override bool AlwaysMurderer { get { return true; } }

        [Constructable]
        public EvilHumanFemale()
            : this( false )
        {
        }

        [Constructable]
        public EvilHumanFemale( bool mounted )
            : base( false, mounted )
        {
            FightMode = Server.Mobiles.FightMode.Closest;
        }

        #region serial-deserial
        public EvilHumanFemale( Serial serial )
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
