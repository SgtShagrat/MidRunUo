/*using System;
using System.Collections;
using System.Collections.Generic;
using Server.Targeting;

namespace Server.Items
{
    // classe comoda per infilarci dentro tipo e quantità
    // Può essere riciclata per molti script
    public class TypeAmountEntry
    {
        public TypeAmountEntry( Type t, int a )
        {
            ItemType = t;
            Amount = a;
        }

        public TypeAmountEntry( GenericReader reader )
        {
            Deserialize( reader );
        }

        public Type ItemType { get; set; }
        public int Amount { get; set; }

        public void Serialize( GenericWriter writer )
        {
            // salvare la versione non serve tanto questa classe non verrà mai cambiata
            writer.Write( ItemType == null ? null : ItemType.FullName );
            writer.Write( Amount );
        }

        public void Deserialize( GenericReader reader )
        {
            string type = reader.ReadString();
            if( type != null )
                ItemType = ScriptCompiler.FindTypeByFullName( type );

            Amount = reader.ReadInt();
        }

        public override string ToString()
        {
            #region casi speciali...
            if( ItemType == typeof( Log ) )
                return String.Format( "{0} Logs or Boards", Amount );
            #endregion

            return Amount + " " + ItemType.Name;
        }
    }

    IMPORTANTE:
    per fare in modo che il CommittmentDeed funzioni col drag-drop bisogna che in tiledata.mul l'ID corrispondente
    sia segnato "stackable" o "container", altrimenti il metodo OnDragDrop() non viene nemmeno chiamato

    Al momento usa l'ID del deed ma non è stackabile, quindi o si modifica il tiledata.mul o si usa temporaneamente
    un altro ID. Tipo quello dei lingotti, o del libro dei bulk, o di uno zaino ... qualcosa che già usi il dragdrop

    public class CommittmentDeed : Item
    {
        public static bool UseDragDrop { get; set; }
        public static bool UseTarget { get { return !UseDragDrop; } set { UseDragDrop = !value; } }

        public class Requirement
        {
            public TypeAmountEntry PubRequirement { get; private set; }

            public ArrayList Deeds { get; private set; }

            public Requirement( Type t, int a ) { PubRequirement = new TypeAmountEntry( t, a ); Deeds = new ArrayList(); }

            public Requirement( GenericReader reader ) { Deserialize( reader ); }

            // Se il deed contiene la quantità giusta di risorse richieste
            // il deed viene internalizzato e la richiesta risulta soddisfatta
            public bool Fulfill( Item i )
            {
                if( i is CommodityDeed )
                    return Fulfill( (CommodityDeed)i );
                if( i is BankCheck )
                    return Fulfill( (BankCheck)i );
                return false;
            }

            public bool Fulfill( CommodityDeed deed )
            {
                if( deed == null || deed.Commodity == null || IsFulfilled )
                    return false;

                #region mod by Dies Irae
                bool shouldAdd = true;

                Item i = deed.Commodity;

                if( i.GetType() != PubRequirement.ItemType )
                {
                    shouldAdd = i is BaseBoards && PubRequirement.ItemType == typeof( Log );
                }

                if( shouldAdd )
                {
                    Deeds.Add( deed );
                    deed.Internalize();
                }

                return shouldAdd;
                #endregion
            }

            public bool Fulfill( BankCheck check )
            {
                if( check == null || IsFulfilled || ( PubRequirement.ItemType != typeof( Gold ) ) )
                    return false;

                Deeds.Add( check );

                check.Internalize();
                return true;
            }

            public int ComputeAmount()
            {
                int sum = 0;
                if( PubRequirement.ItemType == typeof( Gold ) )
                    foreach( BankCheck check in Deeds )
                        sum += check.Worth;
                else
                    foreach( CommodityDeed deed in Deeds )
                        sum += deed.Commodity.Amount;
                return sum;
            }

            public bool IsFulfilled { get { return ComputeAmount() >= PubRequirement.Amount; } }

            public override string ToString()
            {
                string s = "...";

                if( PubRequirement != null )
                {
                    if( IsFulfilled )
                        s = string.Format( "{0} (X)", PubRequirement );
                    else
                        s = string.Format( "{0} ({1})", PubRequirement, ComputeAmount() );
                }

                return s;
            }

            public void Serialize( GenericWriter writer )
            {
                writer.WriteItemList( Deeds );
                PubRequirement.Serialize( writer );
            }

            public void Deserialize( GenericReader reader )
            {
                Deeds = reader.ReadItemList();
                PubRequirement = new TypeAmountEntry( reader );
            }
        }

        private class InternalTarget : Target
        {
            private CommittmentDeed m_Deed;

            public InternalTarget( CommittmentDeed deed )
                : base( 2, false, TargetFlags.None )
            {
                m_Deed = deed;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( m_Deed.TryFulfill( from, o as Item ) )
                    from.SendMessage( "The requirement has been fulfilled" );
                else
                    from.SendMessage( "The requirement could not be fulfilled" );
            }
        }

        public List<Requirement> Requirements { get; set; }
        public List<Item> Rewards { get; set; }

        public static int ID = 0x14F0;

        [Constructable]
        public CommittmentDeed()
            : base( ID )
        {
            Rewards = new List<Item>();
            Requirements = new List<Requirement>();
            Name = "Committment Deed";

            Weight = 1.0;
        }

        public CommittmentDeed( Serial serial )
            : base( serial )
        {
            Rewards = new List<Item>();
            Requirements = new List<Requirement>();
        }

        static CommittmentDeed()
        {
            UseDragDrop = false;
        }

        public void AddRequirement( Requirement r )
        {
            Requirements.Add( r );
        }

        public void AddRequirement( Type t, int amount )
        {
            if( t == null || amount == 0 )
                return;
            Requirements.Add( new Requirement( t, amount ) );
        }

        public void AddReward( Item i )
        {
            if( i == null )
                return;
            Rewards.Add( i );
            i.Internalize();
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Committant { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Committed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsFulfilled { get { foreach( Requirement r in Requirements ) if( !r.IsFulfilled ) return false; return true; } }

        public virtual bool TryFulfill( Mobile m, Item i )
        {
            foreach( Requirement r in Requirements )
                if( r.Fulfill( i ) )
                {
                    InvalidateProperties();
                    return true;
                }
            return false;
        }

        // Un CommodityDeed contenente gli oggetti richiesti deve essere
        // trascinato sopra al CommittmentDeed per soddisfare una richiesta
        public override bool OnDragDrop( Mobile m, Item dropped )
        {
            if( UseDragDrop )
                return TryFulfill( m, dropped );
            return true;
        }

        // facendo doppio click il CommittmentDeed rilascia tutti i
        // CommodityDeed che contiene
        public override void OnDoubleClick( Mobile from )
        {
            if( UseDragDrop )
            {
                foreach( Requirement r in Requirements )
                {
                    foreach( CommodityDeed deed in r.Deeds )
                        from.AddToBackpack( deed );
                    r.Deeds.Clear();
                }
                return;
            }

            if( UseTarget )
            {
                from.SendMessage( "Which Commodity Deed do you want to fill this deed with?" );
                from.Target = new InternalTarget( this );
                return;
            }
        }

        public bool GiveReward()
        {
            if( Committed == null || Rewards.Count == 0 )
                return false;

            foreach( Item i in Rewards )
                Committed.AddToBackpack( i );

            Rewards.Clear();

            return true;
        }

        public override void OnDelete()
        {
            if( Requirements != null )
            {
                foreach( Requirement r in Requirements )
                {
                    foreach( Item deed in r.Deeds )
                        deed.Delete();
                    r.Deeds.Clear();
                }
                Requirements.Clear();
            }

            if( Rewards != null )
            {
                foreach( Item i in Rewards )
                    i.Delete();
                Rewards.Clear();
            }

            base.OnDelete();
        }

        public override bool DisplayWeight { get { return false; } }

        public override void GetProperties( ObjectPropertyList list )
        {
            list.Add( Name );

            string req = "";
            foreach( Requirement r in Requirements )
                req = req + r + "  ";
            list.Add( req );

            #region mod by Dies Irae
            if( Committed != null && !String.IsNullOrEmpty( Committed.Name ) )
                list.Add( 1041602, Committed.Name ); // Owner: ~1_val~

            if( IsFulfilled )
                list.Add( 1060747 ); // filled
            #endregion
        }

        #region Serialization
        enum SaveFlag : byte
        {
            None,
            Committant,
            Committed,
            Requirements,
            Rewards
        }

        private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf ) { if( setIf ) flags |= toSet; }

        private static bool GetSaveFlag( ref SaveFlag flags, SaveFlag toGet ) { return ( ( flags & toGet ) != 0 ); }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag( ref flags, SaveFlag.Committant, Committant != null );
            SetSaveFlag( ref flags, SaveFlag.Committed, Committed != null );
            SetSaveFlag( ref flags, SaveFlag.Requirements, Requirements.Count > 0 );
            SetSaveFlag( ref flags, SaveFlag.Rewards, Rewards.Count > 0 );

            writer.Write( (byte)flags );

            if( GetSaveFlag( ref flags, SaveFlag.Committant ) )
                writer.Write( Committant );
            if( GetSaveFlag( ref flags, SaveFlag.Committed ) )
                writer.Write( Committed );
            if( GetSaveFlag( ref flags, SaveFlag.Requirements ) )
            {
                writer.Write( Requirements.Count );
                foreach( Requirement r in Requirements )
                    r.Serialize( writer );
            }

            if( GetSaveFlag( ref flags, SaveFlag.Rewards ) )
                writer.WriteItemList( Rewards );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            SaveFlag flags = (SaveFlag)reader.ReadByte();

            if( GetSaveFlag( ref flags, SaveFlag.Committant ) )
                Committant = reader.ReadMobile();
            if( GetSaveFlag( ref flags, SaveFlag.Committed ) )
                Committed = reader.ReadMobile();
            if( GetSaveFlag( ref flags, SaveFlag.Requirements ) )
            {
                int N = reader.ReadInt();
                Requirements = new List<Requirement>();
                for( int i = 0; i < N; i++ )
                    Requirements.Add( new Requirement( reader ) );
            }

            if( GetSaveFlag( ref flags, SaveFlag.Rewards ) )
                Rewards = reader.ReadStrongItemList();
        }
        #endregion
    }
}*/