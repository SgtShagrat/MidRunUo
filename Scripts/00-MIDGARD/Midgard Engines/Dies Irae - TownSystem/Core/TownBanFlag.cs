/***************************************************************************
 *								  TownBanFlag.cs
 *									--------------
 *  begin					: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
	[Flags]
	public enum TownBanFlag
	{
		None		= 0x00000000,

		SerpentsHold	= 0x00000001,
		Britain		= 0x00000002,
		Trinsic		= 0x00000004,
		BuccaneersDen   = 0x00000008,

		Cove		= 0x00000010,
		Jhelom		= 0x00000020,
		Minoc		= 0x00000040,
		Ocllo		= 0x00000080,

		Vesper		= 0x00000100,
		Yew		= 0x00000200,
		Wind		= 0x00000400,
		SkaraBrae	= 0x00000800,

		Nujelm		= 0x00001000,
		Moonglow	= 0x00002000,
		Magincia	= 0x00004000,
		Delucia		= 0x00008000,

		Papua		= 0x00010000,
		Marble		= 0x00020000,
		Aserark		= 0x00040000,
		Daekerthane	= 0x00080000,

		DalBaraz	= 0x00100000,
		Ahnor		= 0x00200000,
		Sshamath	= 0x00400000,
		Justiceburg	= 0x00800000,

		CalenSul	= 0x01000000,
		Vinyamar	= 0x02000000,
		Wolfsbane	= 0x04000000,
		Darklore	= 0x08000000,
		
		Naglund		= 0x10000000,
	}

	[PropertyObject]
	public class TownBanAttribute
	{
		public Mobile Owner { get; set; }

		public TownBanFlag TownBanFlags { get; set; }

		#region Towns
		[CommandProperty( AccessLevel.GameMaster )]
		public bool SerpentsHoldBanned{get { return this[ TownBanFlag.SerpentsHold ]; } set { this[ TownBanFlag.SerpentsHold ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool BritainBanned{get { return this[ TownBanFlag.Britain ]; } set { this[ TownBanFlag.Britain ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool TrinsicBanned{get { return this[ TownBanFlag.Trinsic ]; } set { this[ TownBanFlag.Trinsic ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool BuccaneersDenBanned{get { return this[ TownBanFlag.BuccaneersDen ]; } set { this[ TownBanFlag.BuccaneersDen ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool CoveBanned{get { return this[ TownBanFlag.Cove ]; } set { this[ TownBanFlag.Cove ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool JhelomBanned{get { return this[ TownBanFlag.Jhelom ]; } set { this[ TownBanFlag.Jhelom ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool MinocBanned{get { return this[ TownBanFlag.Minoc ]; } set { this[ TownBanFlag.Minoc ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool OclloBanned{get { return this[ TownBanFlag.Ocllo ]; } set { this[ TownBanFlag.Ocllo ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool VesperBanned{get { return this[ TownBanFlag.Vesper ]; } set { this[ TownBanFlag.Vesper ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool YewBanned{get { return this[ TownBanFlag.Yew ]; } set { this[ TownBanFlag.Yew ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool WindBanned{get { return this[ TownBanFlag.Wind ]; } set { this[ TownBanFlag.Wind ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool SkaraBraeBanned{get { return this[ TownBanFlag.SkaraBrae ]; } set { this[ TownBanFlag.SkaraBrae ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool NujelmBanned{get { return this[ TownBanFlag.Nujelm ]; } set { this[ TownBanFlag.Nujelm ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool MoonglowBanned{get { return this[ TownBanFlag.Moonglow ]; } set { this[ TownBanFlag.Moonglow ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool MaginciaBanned{get { return this[ TownBanFlag.Magincia ]; } set { this[ TownBanFlag.Magincia ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool DeluciaBanned{get { return this[ TownBanFlag.Delucia ]; } set { this[ TownBanFlag.Delucia ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool PapuaBanned{get { return this[ TownBanFlag.Papua ]; } set { this[ TownBanFlag.Papua ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool MarbleBanned{get { return this[ TownBanFlag.Marble ]; } set { this[ TownBanFlag.Marble ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool AserarkBanned{get { return this[ TownBanFlag.Aserark ]; } set { this[ TownBanFlag.Aserark ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool DaekerthaneBanned{get { return this[ TownBanFlag.Daekerthane ]; } set { this[ TownBanFlag.Daekerthane ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool DalBarazBanned{get { return this[ TownBanFlag.DalBaraz ]; } set { this[ TownBanFlag.DalBaraz ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool AhnorBanned{get { return this[ TownBanFlag.Ahnor ]; } set { this[ TownBanFlag.Ahnor ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool SshamathBanned{get { return this[ TownBanFlag.Sshamath ]; } set { this[ TownBanFlag.Sshamath ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool JusticeburgBanned{get { return this[ TownBanFlag.Justiceburg ]; } set { this[ TownBanFlag.Justiceburg ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool CalenSulBanned{get { return this[ TownBanFlag.CalenSul ]; } set { this[ TownBanFlag.CalenSul ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool VinyamarBanned{get { return this[ TownBanFlag.Vinyamar ]; } set { this[ TownBanFlag.Vinyamar ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool WolfsbaneBanned{get { return this[ TownBanFlag.Wolfsbane ]; } set { this[ TownBanFlag.Wolfsbane ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool DarkloreBanned{get { return this[ TownBanFlag.Darklore ]; } set { this[ TownBanFlag.Darklore ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool NaglundBanned{get { return this[ TownBanFlag.Naglund ]; } set { this[ TownBanFlag.Naglund ] = value; }}
		#endregion

		public bool this[ TownBanFlag flag ]
		{
			get { return GetFlag( flag ); }
			set { SetFlag( flag, value ); }
		}

		public bool GetFlag( TownBanFlag flag )
		{
			return ( ( TownBanFlags & flag ) != 0 );
		}

		public void SetFlag( TownBanFlag flag, bool value )
		{
			if( value )
				TownBanFlags |= flag;
			else
				TownBanFlags &= ~flag;

			TownSystem system = GetTownSystemFromFlag( flag );

			if( Owner != null && system != null )
			{
				if( value )
					system.RegisterBan( Owner );
				else
					system.UnRegisterBan( Owner );
			}
		}

		public TownBanFlag GetFlagFromTown( MidgardTowns town )
		{
			TownSystem t = TownSystem.Find( town );

			if( t != null )
			{
				foreach ( TownSystem system in TownSystem.TownSystems )
				{
					if( system == t )
						return system.Definition.BanFlag;
				}
			}

			return TownBanFlag.None;
		}

		public TownSystem GetTownSystemFromFlag( TownBanFlag flag )
		{
			return TownSystem.Find( flag );
		}

		public override string ToString()
		{
			return "...";
		}

		public TownBanAttribute( Mobile owner )
		{
			Owner = owner;
		}

		public TownBanAttribute( Mobile owner, GenericReader reader )
		{
			Owner = owner;

			int version = reader.ReadInt();

			switch( version )
			{
				case 0:
					{
						TownBanFlags = (TownBanFlag)reader.ReadInt();

						break;
					}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( 0 ); // version

			writer.Write( (int)TownBanFlags );
		}
	}

	[PropertyObject]
	public class TownPermaBanAttribute
	{
		public Mobile Owner { get; set; }

		public TownBanFlag TownBanFlags { get; set; }

		#region Towns
		[CommandProperty( AccessLevel.GameMaster )]
		public bool SerpentsHoldBanned{get { return this[ TownBanFlag.SerpentsHold ]; } set { this[ TownBanFlag.SerpentsHold ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool BritainBanned{get { return this[ TownBanFlag.Britain ]; } set { this[ TownBanFlag.Britain ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool TrinsicBanned{get { return this[ TownBanFlag.Trinsic ]; } set { this[ TownBanFlag.Trinsic ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool BuccaneersDenBanned{get { return this[ TownBanFlag.BuccaneersDen ]; } set { this[ TownBanFlag.BuccaneersDen ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool CoveBanned{get { return this[ TownBanFlag.Cove ]; } set { this[ TownBanFlag.Cove ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool JhelomBanned{get { return this[ TownBanFlag.Jhelom ]; } set { this[ TownBanFlag.Jhelom ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool MinocBanned{get { return this[ TownBanFlag.Minoc ]; } set { this[ TownBanFlag.Minoc ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool OclloBanned{get { return this[ TownBanFlag.Ocllo ]; } set { this[ TownBanFlag.Ocllo ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool VesperBanned{get { return this[ TownBanFlag.Vesper ]; } set { this[ TownBanFlag.Vesper ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool YewBanned{get { return this[ TownBanFlag.Yew ]; } set { this[ TownBanFlag.Yew ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool WindBanned{get { return this[ TownBanFlag.Wind ]; } set { this[ TownBanFlag.Wind ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool SkaraBraeBanned{get { return this[ TownBanFlag.SkaraBrae ]; } set { this[ TownBanFlag.SkaraBrae ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool NujelmBanned{get { return this[ TownBanFlag.Nujelm ]; } set { this[ TownBanFlag.Nujelm ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool MoonglowBanned{get { return this[ TownBanFlag.Moonglow ]; } set { this[ TownBanFlag.Moonglow ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool MaginciaBanned{get { return this[ TownBanFlag.Magincia ]; } set { this[ TownBanFlag.Magincia ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool DeluciaBanned{get { return this[ TownBanFlag.Delucia ]; } set { this[ TownBanFlag.Delucia ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool PapuaBanned{get { return this[ TownBanFlag.Papua ]; } set { this[ TownBanFlag.Papua ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool MarbleBanned{get { return this[ TownBanFlag.Marble ]; } set { this[ TownBanFlag.Marble ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool AserarkBanned{get { return this[ TownBanFlag.Aserark ]; } set { this[ TownBanFlag.Aserark ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool DaekerthaneBanned{get { return this[ TownBanFlag.Daekerthane ]; } set { this[ TownBanFlag.Daekerthane ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool DalBarazBanned{get { return this[ TownBanFlag.DalBaraz ]; } set { this[ TownBanFlag.DalBaraz ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool AhnorBanned{get { return this[ TownBanFlag.Ahnor ]; } set { this[ TownBanFlag.Ahnor ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool SshamathBanned{get { return this[ TownBanFlag.Sshamath ]; } set { this[ TownBanFlag.Sshamath ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool JusticeburgBanned{get { return this[ TownBanFlag.Justiceburg ]; } set { this[ TownBanFlag.Justiceburg ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool CalenSulBanned{get { return this[ TownBanFlag.CalenSul ]; } set { this[ TownBanFlag.CalenSul ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool VinyamarBanned{get { return this[ TownBanFlag.Vinyamar ]; } set { this[ TownBanFlag.Vinyamar ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool WolfsbaneBanned{get { return this[ TownBanFlag.Wolfsbane ]; } set { this[ TownBanFlag.Wolfsbane ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool DarkloreBanned{get { return this[ TownBanFlag.Darklore ]; } set { this[ TownBanFlag.Darklore ] = value; }}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool NaglundBanned{get { return this[ TownBanFlag.Naglund ]; } set { this[ TownBanFlag.Naglund ] = value; }}
		#endregion

		public bool this[ TownBanFlag flag ]
		{
			get { return GetFlag( flag ); }
			set { SetFlag( flag, value ); }
		}

		public bool GetFlag( TownBanFlag flag )
		{
			return ( ( TownBanFlags & flag ) != 0 );
		}

		public void SetFlag( TownBanFlag flag, bool value )
		{
			if( value )
				TownBanFlags |= flag;
			else
				TownBanFlags &= ~flag;

			TownSystem system = GetTownSystemFromFlag( flag );

			if( Owner != null && system != null )
			{
				if( value )
					system.RegisterPermaBan( Owner );
				else
					system.UnRegisterPermaBan( Owner );
			}
		}

		public TownBanFlag GetFlagFromTown( MidgardTowns town )
		{
			TownSystem t = TownSystem.Find( town );

			if( t != null )
			{
				foreach ( TownSystem system in TownSystem.TownSystems )
				{
					if( system == t )
						return system.Definition.BanFlag;
				}
			}

			return TownBanFlag.None;
		}

		public TownSystem GetTownSystemFromFlag( TownBanFlag flag )
		{
			return TownSystem.Find( flag );
		}

		public override string ToString()
		{
			return "...";
		}

		public TownPermaBanAttribute( Mobile owner )
		{
			Owner = owner;
		}

		public TownPermaBanAttribute( Mobile owner, GenericReader reader )
		{
			Owner = owner;

			int version = reader.ReadInt();

			switch( version )
			{
				case 0:
					{
						TownBanFlags = (TownBanFlag)reader.ReadInt();

						break;
					}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( 0 ); // version

			writer.Write( (int)TownBanFlags );
		}
	}
}