using Server;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class MaidenOrgan : BaseOrgan
    {
        [Constructable]
        public MaidenOrgan( BrainTypes type )
            : base( 0x124D, 1066115, 0x487, type ) // an organ
        {
        }

        #region members
        public override void CreateTissue()
        {
            Brain = new Gland( BrainType, this );
            PutGuts( Brain, 20, 30 );

            Gland gland = Brain as Gland;
            gland.CreateTissue( -4, -5 );
        }

        public override void OpenOrganTo( Mobile from )
        {
            PuzzlePlagueBeastLord pbl = RootParent as PuzzlePlagueBeastLord;
            if( pbl == null || pbl.Deleted || pbl.Map == Map.Internal )
                return;

            from.RevealingAction();
            from.Direction = from.GetDirectionTo( pbl );

            if( IsOpened )
            {
                SendLocalizedMessageTo( from, 1066121 ); // * This organ was already opened *
            }
            else
            {
                SendLocalizedMessageTo( from, 1066122 ); // * You open the plague beast's organ *
                from.PlaySound( 0x2AC );

                ItemID = 0x124A;
                IsOpened = true;
            }
        }

        public override void OnSnoop( Mobile from )
        {
            OnDoubleClick( from );
        }
        #endregion

        #region serial-deserial
        public MaidenOrgan( Serial serial )
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