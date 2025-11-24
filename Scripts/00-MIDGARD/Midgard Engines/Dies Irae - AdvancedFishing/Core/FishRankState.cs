/***************************************************************************
 *                               FishRankState.cs
 *
 *   begin                : 31 ottobre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Xml.Linq;

using Server;

using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.AdvancedFishing
{
    public class FishRankState
    {
        public DateTime TimeOfCreation { get; set; }
        public Mobile Owner { get; set; }
        public Type FishType { get; set; }
        public int FishWeight { get; set; }

        public FishRankState( Mobile owner, BaseAdvancedFish fish )
        {
            Owner = owner;
            TimeOfCreation = DateTime.Now;
            FishType = fish.GetType();
            FishWeight = fish.FishWeight;
        }

        public override string ToString()
        {
            return "...";
        }

        public XElement ToXElement()
        {
            return new XElement( "state", new XAttribute( "owner", Utility.SafeString( Owner.Name ?? "" ) ),
                                 new XAttribute( "account", Utility.SafeString( Owner.Account.Username ?? "" ) ),
                                 new XAttribute( "creation", TimeOfCreation.ToString() ),
                                 new XAttribute( "fishType", Utility.SafeString( FishType.Name ) ),
                                 new XAttribute( "fishWeight", FishWeight.ToString() ) );
        }
    }
}