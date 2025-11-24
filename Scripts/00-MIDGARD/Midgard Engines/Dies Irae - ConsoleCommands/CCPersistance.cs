/***************************************************************************
*                               ConsoleCommandsPersistance.cs
*
*   begin                : 03 October, 2009
*   author               :	Dies Irae	
*   email                : tocasia@alice.it
*   copyright            : (C) Midgard Shard - Dies Irae			
*
***************************************************************************/

using Server;

namespace Midgard.Engines.ConsoleCommands
{
    public class CCPersistance : Mobile
    {
        public static CCPersistance Instance { get; private set; }

        [Constructable]
        public CCPersistance()
        {
            AccessLevel = Config.ConsoleAccessLevel;
            Name = "Midgard Staff";

            if( Instance == null || Instance.Deleted )
                Instance = this;
            else
                base.Delete();
        }

        public CCPersistance( Serial serial )
            : base( serial )
        {
            Instance = this;
        }

        public static void EnsureExistence()
        {
            if( Instance == null )
                Instance = new CCPersistance();
        }

        public override void Delete()
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( Name != "Midgard Staff" )
                Name = "Midgard Staff";

            if( AccessLevel != Config.ConsoleAccessLevel )
                AccessLevel = Config.ConsoleAccessLevel;
        }
        #endregion
    }
}