using Server;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class MutationCore : Item
    {
        #region properties
        public override int LabelNumber { get { return 1066113; } } // a plague beast mutation core
        public override bool DisplayWeight { get { return false; } }
        #endregion

        [Constructable]
        public MutationCore()
            : base( 0x1CF0 )
        {
            Movable = false;
            Visible = false;

            Hue = 0x480;
            Weight = 1;
        }

        public MutationCore( Serial serial )
            : base( serial )
        {
        }

        public void OnAfterCut( Mobile from )
        {
            if( !Movable )
            {
                PuzzlePlagueBeastLord pbl = RootParent as PuzzlePlagueBeastLord;

                if( pbl != null )
                {
                    from.PlaySound( 0x248 );

                    if( pbl.IsInDebugMode )
                        from.SendLocalizedMessage( 1066138 ); // Congratulations! You solved the plague beast lord test puzzle!

                    pbl.Say( 1066132 ); // * You place a plague beast mutation core in your backpack. *

                    Movable = true;

                    from.AddToBackpack( this );
                    from.PlaySound( 0x248 );

                    pbl.Kill();
                }
            }
        }

        #region serial-deserial
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