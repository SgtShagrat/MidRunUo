using Server;
using Server.Items;

namespace Midgard.Items
{
    internal class TournamentRobe : Robe
    {
        public override string DefaultName
        {
            get { return "a Midgard Tournament Robe"; }
        }

        public override bool DisplayLootType
        {
            get { return false; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        [Constructable]
        public TournamentRobe()
        {
            Hue = 1289;
            LootType = LootType.Blessed;

            Movable = false;

            Attributes.LowerRegCost = 100;
            Attributes.LowerManaCost = 40;
            Attributes.DefendChance = 15;
            Attributes.AttackChance = 15;
            Attributes.RegenMana = 12;
            Attributes.SpellDamage = 15;
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            AddNameProperties( list );
        }

        public override DeathMoveResult OnParentDeath( Mobile parent )
        {
            return DeathMoveResult.RemainEquiped;
        }

        public override DeathMoveResult OnInventoryDeath( Mobile parent )
        {
            Delete();
            return base.OnInventoryDeath( parent );
        }

        #region serialization
        public TournamentRobe( Serial serial )
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
        }
        #endregion
    }
}