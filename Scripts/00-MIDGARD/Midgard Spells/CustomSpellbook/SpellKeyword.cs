using Server;

namespace Midgard.Engines.SpellSystem
{
    public class SpellKeyword
    {
        public int SpellID { get; private set; }
        public string Keyword { get; private set; }

        public SpellKeyword( int spell, string keyword )
        {
            SpellID = spell;
            Keyword = keyword;
        }

        public SpellKeyword( GenericReader reader )
        {
            Deserialize( reader );
        }

        #region serialization
        public void Deserialize( GenericReader reader )
        {
            SpellID = reader.ReadInt();
            Keyword = reader.ReadString();
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( SpellID );
            writer.Write( Keyword );
        }
        #endregion
    }
}