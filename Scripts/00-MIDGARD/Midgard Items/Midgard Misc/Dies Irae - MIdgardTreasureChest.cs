/***************************************************************************
 *                                  MidgardTreasureChest.cs
 *                            		-----------------------
 *  begin                	: Febbraio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    public enum ChestHues
    {
        WhiteHues
    }

    public class MidgardTreasureChest : BaseTreasureChest
    {
        private ChestHues m_Hues;

        public MidgardTreasureChest( TreasureLevel level, int itemID, ChestHues hues, bool isTrapale, bool hasRandomId )
            : base( itemID, level )
        {
            IsTrapable = isTrapale;
            HasRandomId = hasRandomId;
            m_Hues = hues;

            if( IsTrapable )
                DoTrap();

            if( HasRandomId )
                RandomizeId();

            RandomizeHue();

            MaxSpawnTime = (short)( (int)Level * 60 );
            MinSpawnTime = (short)( (int)Level * 30 );
        }

        public bool IsTrapable { get; private set; }

        public bool HasRandomId { get; private set; }

        public double ChanceToBeTrapped
        {
            get { return (int)Level * 0.3; }
        }

        public double ChanceOfEmptyChest
        {
            get { return 0.3; }
        }

        public double ChanceToBeRevealed
        {
            get { return (int)Level * 0.15; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        public override bool DisplaysContent
        {
            get { return false; }
        }

        public override string DefaultName
        {
            get { return Locked ? "a locked treasure" : "a treasure"; }
        }

        public override bool HandlesOnMovement
        {
            get { return true; }
        }

        public bool CheckRange( Point3D loc, Point3D oldLoc, int range )
        {
            return CheckRange( loc, range ) && !CheckRange( oldLoc, range );
        }

        public bool CheckRange( Point3D loc, int range )
        {
            return ( ( Z + 8 ) >= loc.Z && ( loc.Z + 16 ) > Z )
                && Utility.InRange( GetWorldLocation(), loc, range );
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            base.OnMovement( m, oldLocation );

            if( CheckRange( m.Location, oldLocation, 6 ) )
            {
                if( m.AccessLevel > AccessLevel.Player )
                    SendMessageTo( m, string.Format( "[Treasure Chest - Level {0}]", Level ), 0x3B2 );
            }
        }

        private void SendMessageTo( Mobile to, string text, int hue )
        {
            if( Deleted || !to.CanSee( this ) )
                return;

            to.Send( new UnicodeMessage( Serial, ItemID, MessageType.Regular, hue, 3, "ENU", "", text ) );
        }

        public override void OnItemLifted( Mobile from, Item item )
        {
            if( Utility.RandomDouble() < ChanceToBeRevealed )
            {
                from.SendMessage( "You made some noise and get revealed!" );
                from.RevealingAction( true );
            }

            base.OnItemLifted( from, item );
        }

        private void DoTrap()
        {
            if( Utility.RandomDouble() < ChanceToBeTrapped )
            {
                switch( Utility.Random( 4 ) )
                {
                    case 0:
                        TrapType = TrapType.MagicTrap;
                        break;
                    case 1:
                        TrapType = TrapType.ExplosionTrap;
                        break;
                    case 2:
                        TrapType = TrapType.DartTrap;
                        break;
                    case 3:
                        TrapType = TrapType.PoisonTrap;
                        break;
                    default:
                        TrapType = TrapType.ExplosionTrap;
                        break;
                }

                TrapPower = (int)Level * Utility.RandomMinMax( 20, 30 );
                TrapLevel = (int)Level;
            }
        }

        private void RandomizeId()
        {
            bool useFirstItemId = Utility.RandomBool();

            switch( Utility.Random( 8 ) )
            {
                case 0: // Large Crate
                    ItemID = ( useFirstItemId ? 0xE3C : 0xE3D );
                    GumpID = 0x44;
                    break;
                case 1: // Medium Crate
                    ItemID = ( useFirstItemId ? 0xE3E : 0xE3F );
                    GumpID = 0x44;
                    break;
                case 2: // Small Crate
                    ItemID = ( useFirstItemId ? 0x9A9 : 0xE7E );
                    GumpID = 0x44;
                    break;
                case 3: // Wooden Chest
                    ItemID = ( useFirstItemId ? 0xE42 : 0xE43 );
                    GumpID = 0x49;
                    break;
                case 4: // Metal Chest
                    ItemID = ( useFirstItemId ? 0x9AB : 0xE7C );
                    GumpID = 0x4A;
                    break;
                case 5: // Metal Golden Chest
                    ItemID = ( useFirstItemId ? 0xE40 : 0xE41 );
                    GumpID = 0x42;
                    break;
                case 6: // Keg
                    ItemID = 0xE7F;
                    GumpID = 0x3E;
                    break;
                case 7: // Barrel
                    ItemID = 0xE77;
                    GumpID = 0x3E;
                    break;
                default:
                    ItemID = ( useFirstItemId ? 0x9AB : 0xE7C );
                    GumpID = 0x4A;
                    break;
            }
        }

        private void RandomizeHue()
        {
            switch( ItemID )
            {
                case 0x9AB:
                case 0xE7C:
                case 0xE40:
                case 0xE41:
                    Hue = Utility.RandomMetalHue();
                    break;
                default:
                    Hue = Utility.RandomNeutralHue();
                    break;
            }
        }

        protected override void SetLockLevel()
        {
            switch( (int)Level )
            {
                case 1:
                    RequiredSkill = 36;
                    break;
                case 2:
                    RequiredSkill = 76;
                    break;
                case 3:
                    RequiredSkill = 84;
                    break;
                case 4:
                    RequiredSkill = 92;
                    break;
                case 5:
                    RequiredSkill = 100;
                    break;
                case 6:
                    RequiredSkill = 100;
                    break;
            }

            LockLevel = RequiredSkill - 10;
            MaxLockLevel = RequiredSkill + 40;
        }

        protected override void GenerateTreasure()
        {
            if( Utility.RandomDouble() > ChanceOfEmptyChest )
                Fill( this, (int)Level );
        }

        public override void Reset()
        {
            StopTimer();

            // le casse in questione non devono essere respawnate negli
            // oggetti ma solo riloccate. Quando il timer delle spawnEntry
            // respawnerà a cassa altrove allora ricreerà anche il contenuto.

            //Locked = true; // fa anche partire il TreasureResetTimer!

            //if( IsTrapable )
            //    DoTrap();

            if( !Deleted )
                Delete();
        }

        public static void Fill( LockableContainer cont, int level )
        {
            if( level == 0 )
            {
                cont.LockLevel = 0; // Can't be unlocked

                if( Utility.RandomDouble() < 0.10 )
                    cont.DropItem( new TreasureMap( 0, Map.Felucca ) );
            }
            else
            {
                for( int i = 0; i < level * 5; ++i )
                    cont.DropItem( Loot.RandomScroll( 0, 63, SpellbookType.Regular ) );

                int itemCount = 0;
                switch( level )
                {
                    case 1:
                        itemCount = 0;
                        break;
                    case 2:
                        itemCount = Utility.Random( 2 );
                        break;
                    case 3:
                        itemCount = Utility.Random( 3 );
                        break;
                    case 4:
                        itemCount = Utility.Random( 4 );
                        break;
                    case 5:
                        itemCount = Utility.Random( 5 );
                        break;
                    case 6:
                        itemCount = Utility.Random( 6 );
                        break;
                }

                for( int i = 0; i < itemCount; ++i )
                {
                    Item item;

                    item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();

                    Engines.SecondAgeLoot.Magics.ApplyBonusTo( item );

                    if( item != null )
                        cont.DropItem( item );
                }
            }

            int reagents;
            if( level == 0 )
                reagents = 1;
            else
                reagents = level * 3;

            for( int i = 0; i < reagents; i++ )
            {
                Item item = Loot.RandomPossibleReagent();
                item.Amount = Utility.RandomMinMax( 40, 60 );
                cont.DropItem( item );
            }

            int gems;
            if( level == 0 )
                gems = 2;
            else
                gems = level * 3;

            for( int i = 0; i < gems; i++ )
            {
                Item item = Loot.RandomGem();
                cont.DropItem( item );
            }

            for( int i = Utility.Random( 1, level ); i > 1; i-- )
            {
                Item potionLoot = Loot.RandomPotion();
                potionLoot.Amount = Utility.Random( 1, level * 4 );
                cont.DropItem( potionLoot );
            }
        }

        #region serialization
        public MidgardTreasureChest( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( IsTrapable );
            writer.Write( HasRandomId );
            writer.Write( (int)m_Hues );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    IsTrapable = reader.ReadBool();
                    HasRandomId = reader.ReadBool();
                    m_Hues = (ChestHues)reader.ReadInt();
                    break;
            }
        }
        #endregion
    }
}