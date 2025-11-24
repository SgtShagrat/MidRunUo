/***************************************************************************
 *                               BountyBoardEntry.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Server;
using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.BountySystem
{
    public class BountyBoardEntry
    {
        public Mobile Owner { get; private set; }
        public Mobile Wanted { get; private set; }

        public DateTime ExpireTime { get; set; }

        public int Price { get; set; }

        public bool Expired
        {
            get { return ( DateTime.Now >= ExpireTime ); }
        }

        public List<Mobile> Requested { get; private set; }
        public List<Mobile> Accepted { get; private set; }

        public BountyBoardEntry( Mobile owner, Mobile wanted, int price, DateTime expireTime )
        {
            Owner = owner;
            Wanted = wanted;
            Price = price;
            ExpireTime = expireTime;

            Requested = new List<Mobile>();
            Accepted = new List<Mobile>();
        }

        public XElement ToXElement()
        {
            return new XElement( "bounty", new XAttribute( "owner", Utility.SafeString( Owner.Name ?? "" ) ),
                                       new XAttribute( "wanted", Utility.SafeString( Wanted.Name ?? "" ) ),
                                       new XAttribute( "expires", ExpireTime.ToString() ),
                                       new XAttribute( "price", Price.ToString() ),
                                       new XElement( "requested",
                                                    from r in Requested
                                                    where r != null && !r.Deleted
                                                    select new XElement( "requester", Utility.SafeString( r.Name ?? "" ) ) ),
                                       new XElement( "accepted",
                                                    from a in Accepted
                                                    where a != null && !a.Deleted
                                                    select new XElement( "acceptors", Utility.SafeString( a.Name ?? "" ) ) ) );
        }
    }
}