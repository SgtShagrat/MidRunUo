using Server;

namespace Midgard.Engines.SpellSystem
{
    public class IconInfo
    {
        public int Icon { get; private set; }
        public int Spell { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Background { get; private set; }

        public IconInfo( int icon, int spell, int x, int y, int background )
        {
            Icon = icon;
            Spell = spell;
            X = x;
            Y = y;
            Background = background;
        }

        #region serialization
        public IconInfo( GenericReader reader )
        {
            Deserialize( reader );
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( 0 );

            writer.Write( Background );
            writer.Write( Icon );
            writer.Write( Spell );
            writer.Write( X );
            writer.Write( Y );
        }

        public void Deserialize( GenericReader reader )
        {
            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        Background = reader.ReadInt();
                        Icon = reader.ReadInt();
                        Spell = reader.ReadInt();
                        X = reader.ReadInt();
                        Y = reader.ReadInt();
                        break;
                    }
            }
        }
        #endregion
    }
}