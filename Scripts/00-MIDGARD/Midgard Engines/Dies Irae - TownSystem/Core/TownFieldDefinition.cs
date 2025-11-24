/***************************************************************************
 *                                  TownFieldDefinition.cs
 *                            		----------------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownFieldDefinition
    {
        public TownSystem System { get; private set; }
        public TextDefinition FieldName { get; private set; }
        public Point3D NorthWestCorner { get; private set; }
        public Point3D SouthEstCorner { get; private set; }
        public Point3D BanLocation { get; private set; }
        public Point3D SignLocation { get; private set; }
        public int MinZ { get; private set; }
        public int MaxZ { get; private set; }
        public Point3D ContractLocation { get; private set; }
        public int RentalCost { get; private set; }
        public int NumSecures { get; private set; }
        public int NumLockDowns { get; private set; }

        public TownFieldDefinition( TownSystem system, TextDefinition fieldName, Point3D northWestCorner, Point3D southEstCorner, int minZ, int maxZ,
                                   Point3D banLocation, Point3D signLocation, Point3D contractLocation, int rentalCost, int numSecures,
                                   int numLockDowns )
        {
            System = system;
            FieldName = fieldName;
            NorthWestCorner = northWestCorner;
            SouthEstCorner = southEstCorner;
            MinZ = minZ;
            MaxZ = maxZ;
            BanLocation = banLocation;
            SignLocation = signLocation;
            ContractLocation = contractLocation;
            RentalCost = rentalCost;
            NumSecures = numSecures;
            NumLockDowns = numLockDowns;
        }
    }
}