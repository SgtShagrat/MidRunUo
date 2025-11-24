/*
 using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class MidgardArchitectA : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override ArrayList SBInfos
        {
            get
            {
                return m_SBInfos;
            }
        }

        [Constructable]
        public MidgardArchitectA()
            : base( null )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Architetto di Midgard" );
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add( new SBMidgardArchitectA() );
        }

        // qui copiamo i dati sul deed che verrà veramente passato al PG
        // perchè quello usato per la transazione viene cancellato da BaseVendor
        public override void InitializeBuyEntity( IEntity o, Mobile buyer )
        {
            if( !( o is HouseCommittmentDeed ) )
                return;

            HouseCommittmentDeed deed = (HouseCommittmentDeed)o;
            deed.Committant = this;
            deed.Committed = buyer;
        }

        public MidgardArchitectA( Serial serial )
            : base( serial )
        {
        }
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class MidgardArchitectB : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override ArrayList SBInfos
        {
            get
            {
                return m_SBInfos;
            }
        }

        [Constructable]
        public MidgardArchitectB()
            : base( null )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Geometra" );
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add( new SBMidgardArchitectB() );
        }

        // qui copiamo i dati sul deed che verrà veramente passato al PG
        // perchè quello usato per la transazione viene cancellato da BaseVendor
        public override void InitializeBuyEntity( IEntity o, Mobile buyer )
        {
            if( !( o is HouseCommittmentDeed ) )
                return;

            HouseCommittmentDeed deed = (HouseCommittmentDeed)o;
            deed.Committant = this;
            deed.Committed = buyer;
        }

        public MidgardArchitectB( Serial serial )
            : base( serial )
        {
        }
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class MidgardArchitectC : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override ArrayList SBInfos
        {
            get
            {
                return m_SBInfos;
            }
        }

        [Constructable]
        public MidgardArchitectC()
            : base( null )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Gran Geometra" );
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add( new SBMidgardArchitectC() );
        }

        // qui copiamo i dati sul deed che verrà veramente passato al PG
        // perchè quello usato per la transazione viene cancellato da BaseVendor
        public override void InitializeBuyEntity( IEntity o, Mobile buyer )
        {
            if( !( o is HouseCommittmentDeed ) )
                return;

            HouseCommittmentDeed deed = (HouseCommittmentDeed)o;
            deed.Committant = this;
            deed.Committed = buyer;
        }

        public MidgardArchitectC( Serial serial )
            : base( serial )
        {
        }
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}*/