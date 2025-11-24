/***************************************************************************
 *                               CraftObtainObjective.cs
 *
 *   begin                : 11 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.Quests;
using Server.Items;

namespace Midgard.Engines.CraftQuests
{
    public class CraftObtainObjective : ObtainObjective
    {
        public CraftResource Resource { get; set; }

        public CraftObtainObjective( Type obtain, string name, CraftResource resource, int amount )
            : this( obtain, name, resource, amount, 0, 0 )
        {
        }

        public CraftObtainObjective( Type obtain, string name, CraftResource resource, int amount, int image )
            : this( obtain, name, resource, amount, image, 0 )
        {
        }

        public CraftObtainObjective( Type obtain, string name, CraftResource resource, int amount, int image, int seconds )
            : base( obtain, name, amount, image, seconds )
        {
            Resource = resource;
        }

        public override bool IsObjective( Item item )
        {
            if( base.IsObjective( item ) )
            {
                Pitcher pitcher = (Pitcher)item;

                if( pitcher.Content == BeverageType.Water && !pitcher.IsEmpty )
                    return true;
            }

            return false;
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            writer.Write( (int)Resource );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            Resource = (CraftResource)reader.ReadInt();
        }
        #endregion
    }
}