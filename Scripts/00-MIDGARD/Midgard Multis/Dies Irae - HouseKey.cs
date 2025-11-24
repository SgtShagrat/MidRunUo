using Server.Multis;

namespace Server.Items
{
    public class HouseKey : Key
    {
        private bool m_IsInternal;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsInternal
        {
            get { return m_IsInternal; }
            set { m_IsInternal = value; }
        }

        public override int LabelNumber
        {
            get { return m_IsInternal ? 1041063 : base.LabelNumber; } // an interior house door key
        }

        [Constructable]
        public HouseKey( bool isInternal, uint lockVal )
            : this( isInternal, lockVal, null )
        {
        }

        public HouseKey( bool isInternal, uint lockVal, Item link )
            : base( isInternal ? KeyType.Rusty : KeyType.Gold, lockVal, link )
        {
            m_IsInternal = isInternal;
        }

        public override bool UseOn( Mobile from, ILockable o )
        {
            // http://update.uo.com/design_22.html
            // Interior lockable doors will exist. 
            // Use of a house key on a door will allow you to rekey that door 
            // and will generate a new key for it. 
            if( !m_IsInternal && o is BaseHouseDoor && BaseHouse.OldSchoolRules )
            {
                BaseHouseDoor houseDoor = (BaseHouseDoor)o;

                BaseHouse house = houseDoor.FindHouse();
                if( house == null )
                    return false;

                if( house.IsOwner( from ) && houseDoor.IsInternal && IsValidForMainDoorHouse( house ) )
                {
                    // remove old keys
                    RemoveKeys( from, houseDoor.KeyValue );

                    // get a random key value
                    uint value = RandomValue();

                    // set the door
                    houseDoor.KeyValue = value;

                    // make a new kay for that door, assign value and place in owner's pack
                    HouseKey packKey = new HouseKey( true, value, houseDoor.Link );
                    HouseKey bankKey = new HouseKey( true, value, houseDoor.Link );

                    BankBox box = from.BankBox;
				    if ( !box.TryDropItem( from, bankKey, false ) )
					    bankKey.Delete();

                    from.AddToBackpack( packKey );

                    from.SendMessage( "You place a lock on the door. A new key has been placed in your backpack." );
                    return true;
                }
            }

            return base.UseOn( from, o );
        }

        /// <summary>
        /// Check if this key is suitable to open a door of our house
        /// </summary>
        public bool IsValidForMainDoorHouse( BaseHouse house )
        {
            if( IsInternal )
                return false;

            foreach( Item door in house.Doors )
            {
                if( door is BaseHouseDoor )
                {
                    BaseHouseDoor houseDoor = (BaseHouseDoor)door;

                    if( !houseDoor.IsInternal && houseDoor.KeyValue == KeyValue )
                        return true;
                }
            }

            return false;
        }

        #region serialization
        public HouseKey( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_IsInternal );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_IsInternal = reader.ReadBool();
        }
        #endregion
    }
}