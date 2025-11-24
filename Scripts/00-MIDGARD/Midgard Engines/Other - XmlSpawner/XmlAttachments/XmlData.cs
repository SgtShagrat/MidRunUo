using System;
using System.Collections.Generic;

namespace Server.Engines.XmlSpawner2
{
    public class XmlData : XmlAttachment
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public string Data { get; set; }

        public XmlData( ASerial serial )
            : base( serial )
        {
        }

        [Attachable]
        public XmlData( string name )
        {
            Name = name;
            Data = String.Empty;
        }

        [Attachable]
        public XmlData( string name, string data )
        {
            Name = name;
            Data = data;
        }

        [Attachable]
        public XmlData( string name, string data, double expiresin )
        {
            Name = name;
            Data = data;
            Expiration = TimeSpan.FromMinutes( expiresin );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( Data );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Data = reader.ReadString();
        }

        public override string OnIdentify( Mobile from )
        {
            if( from == null || from.AccessLevel == AccessLevel.Player )
                return null;

            return Expiration > TimeSpan.Zero ? String.Format( "{2}: Data {0} expires in {1} mins", Data, Expiration.TotalMinutes, Name ) : String.Format( "{1}: Data {0}", Data, Name );
        }
    }

    public class XmlMobilesListData : XmlAttachment
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public List<Mobile> Data { get; set; }

        public XmlMobilesListData( ASerial serial )
            : base( serial )
        {
        }

        [Attachable]
        public XmlMobilesListData( string name )
            : this( name, new List<Mobile>() )
        {
        }

        [Attachable]
        public XmlMobilesListData( string name, List<Mobile> data )
            : this( name, data, 0.0 )
        {
        }

        [Attachable]
        public XmlMobilesListData( string name, List<Mobile> data, double expiresin )
        {
            Name = name;
            Data = data;
            Expiration = TimeSpan.FromMinutes( expiresin );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( Data );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Data = reader.ReadStrongMobileList();
        }

        public override string OnIdentify( Mobile from )
        {
            return "";
        }
    }
}