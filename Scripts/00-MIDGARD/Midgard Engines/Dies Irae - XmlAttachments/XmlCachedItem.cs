/***************************************************************************
 *                                  XmlCachedItem.cs
 *                            		----------------
 *  begin                	: Maggio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 *          Attach utile per contrassegnare item che devono essere deletati
 *          al delete dell' Attachment.
 * 
 ***************************************************************************/

namespace Server.Engines.XmlSpawner2
{
    public class XmlCachedItem : XmlAttachment
    {
        [Attachable]
        public XmlCachedItem()
        {
            Name = "Cached Item";
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if( AttachedTo != null )
            {
                if( AttachedTo is Item )
                {
                    if( !( (Item)( AttachedTo ) ).Deleted )
                        ( (Item)( AttachedTo ) ).Delete();
                }
                if( AttachedTo is Mobile )
                {
                    if( !( (Mobile)( AttachedTo ) ).Deleted )
                        ( (Mobile)( AttachedTo ) ).Delete();
                }
            }
        }

        public XmlCachedItem( ASerial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            // version 0
            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            // version 0
            reader.ReadInt();
        }
    }
}