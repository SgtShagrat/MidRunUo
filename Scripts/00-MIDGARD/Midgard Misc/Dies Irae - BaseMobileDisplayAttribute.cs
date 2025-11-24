/***************************************************************************
 *                               BaseMobileDisplayAttributes.cs
 *
 *   begin                : 10 ottobre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Mobiles;

namespace Midgard
{
    [PropertyObject]
    public abstract class BaseMobileDisplayAttributes
    {
        public Midgard2PlayerMobile Owner { get; private set; }

        protected BaseMobileDisplayAttributes( Midgard2PlayerMobile owner )
        {
            Owner = owner;
        }

        public override string ToString()
        {
            return "...";
        }
    }

    public class QuestAttributes : BaseMobileDisplayAttributes
    {
        public QuestAttributes( Midgard2PlayerMobile owner )
            : base( owner )
        {
        }

        [CommandProperty( AccessLevel.Seer )]
        public int Variable1
        {
            get { return Owner.QuestVariable1; }
            set { Owner.QuestVariable1 = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int Variable2
        {
            get { return Owner.QuestVariable2; }
            set { Owner.QuestVariable2 = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int Variable3
        {
            get { return Owner.QuestVariable3; }
            set { Owner.QuestVariable3 = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int Variable4
        {
            get { return Owner.QuestVariable4; }
            set { Owner.QuestVariable4 = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int Variable5
        {
            get { return Owner.QuestVariable5; }
            set { Owner.QuestVariable5 = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public string VarString
        {
            get { return Owner.QuestString; }
            set { Owner.QuestString = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public TimeSpan Expiration
        {
            get { return Owner.QuestDeltaTimeExpiration; }
            set { Owner.QuestDeltaTimeExpiration = value; }
        }
    }

    public class HardLabourAttribute : BaseMobileDisplayAttributes
    {
        public HardLabourAttribute( Midgard2PlayerMobile owner )
            : base( owner )
        {
        }

        [CommandProperty( AccessLevel.Seer )]
        public int Minerals2Mine
        {
            get { return Owner.Minerals2Mine; }
            set { Owner.Minerals2Mine = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public string Info
        {
            get { return Owner.HardLabourInfo; }
            set { Owner.HardLabourInfo = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public string Condamner
        {
            get { return Owner.HardLabourCondamner; }
            set { Owner.HardLabourCondamner = value; }
        }
    }

    public class NotorietyAttributes : BaseMobileDisplayAttributes
    {
        public NotorietyAttributes( Midgard2PlayerMobile owner )
            : base( owner )
        {
        }

        [CommandProperty( AccessLevel.Seer )]
        public int LifeTimeKills
        {
            get { return Owner.LifeTimeKills; }
            set { Owner.LifeTimeKills = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool TempPermaRed
        {
            get { return Owner.TempPermaRed; }
            set { Owner.TempPermaRed = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool PermaRed
        {
            get { return Owner.PermaRed; }
            set { Owner.PermaRed = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int Karma
        {
            get { return Owner.Karma; }
            set { Owner.Karma = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int Fame
        {
            get { return Owner.Fame; }
            set { Owner.Fame = value; }
        }
    }

    public class CustomResistAttributes : BaseMobileDisplayAttributes
    {
        public CustomResistAttributes( Midgard2PlayerMobile owner )
            : base( owner )
        {
        }

        [CommandProperty( AccessLevel.Seer )]
        public int ElectricResistance
        {
            get { return Owner.CustElectricResistance; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int FireResistance
        {
            get { return Owner.CustFireResistance; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int GeneralResistance
        {
            get { return Owner.CustGeneralResistance; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int ImpactResistance
        {
            get { return Owner.CustImpactResistance; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int MentalResistance
        {
            get { return Owner.CustMentalResistance; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int VenomResistance
        {
            get { return Owner.CustVenomResistance; }
        }
    }

    public class CamuflageAttributes : BaseMobileDisplayAttributes
    {
        public CamuflageAttributes( Midgard2PlayerMobile owner )
            : base( owner )
        {
        }

        [CommandProperty( AccessLevel.Seer )]
        public MidgardTowns TownMod
        {
            get { return Owner.TownMod; }
            set { Owner.TownMod = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int Karma
        {
            get { return Owner.Karma; }
            set { Owner.Karma = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int KarmaMod
        {
            get { return Owner.KarmaMod; }
            set { Owner.KarmaMod = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int Fame
        {
            get { return Owner.Fame; }
            set { Owner.Fame = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int FameMod
        {
            get { return Owner.FameMod; }
            set { Owner.FameMod = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public Mobile Alias
        {
            get { return Owner.Alias; }
            set { Owner.Alias = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool FemaleMod
        {
            get { return Owner.FemaleMod; }
            set { Owner.FemaleMod = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool Female
        {
            get { return Owner.Female; }
            set { Owner.Female = value; }
        }
    }

    public class StaffAttributes : BaseMobileDisplayAttributes
    {
        public StaffAttributes( Midgard2PlayerMobile owner )
            : base( owner )
        {
        }

        [CommandProperty( AccessLevel.Administrator )]
        public bool OnlineVisible
        {
            get { return Owner.OnlineVisible; }
            set { Owner.OnlineVisible = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public bool IsMidgardSpawner
        {
            get { return Owner.IsMidgardSpawner; }
            set { Owner.IsMidgardSpawner = value; }
        }
    }

    public class PlayerFlagsAttributes : BaseMobileDisplayAttributes
    {
        public PlayerFlagsAttributes( Midgard2PlayerMobile owner )
            : base( owner )
        {
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool AcceptPrivateMessages
        {
            get { return Owner.AcceptPrivateMessages; }
            set { Owner.AcceptPrivateMessages = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool IsSmothed
        {
            get { return Owner.IsSmothed; }
            set { Owner.IsSmothed = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool DisplayPvpStatus
        {
            get { return Owner.DisplayPvpStatus; }
            set { Owner.DisplayPvpStatus = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool CrystalHarvesting
        {
            get { return Owner.CrystalHarvesting; }
            set { Owner.CrystalHarvesting = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool DisplayRegionalInfo
        {
            get { return Owner.DisplayRegionalInfo; }
            set { Owner.DisplayRegionalInfo = value; }
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool AcceptTips
        {
            get { return Owner.AcceptTips; }
            set { Owner.AcceptTips = value; }
        }
    }
}