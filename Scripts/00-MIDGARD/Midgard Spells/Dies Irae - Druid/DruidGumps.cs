/***************************************************************************
 *							   Dies Irae - MidgardSaveGump.cs
 *
 *   begin				: 07 November, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

//#define Gump1
// #define Gump2
#define Gump3

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
	public class DruidCircleGump : Gump
	{
		public DruidCircleGump() : base( 30, 30 )
		{
			Closable = false;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImage(0, 0, 150);
		}
	}
	public class DruidEmpowermentGump : Gump
	{
		public DruidEmpowermentGump() : base( 30, 66 )
		{
			Closable = false;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImage(0, 0, 128);
		}
	}
	public class GiftOfLifeGump : Gump
	{
		public GiftOfLifeGump() : base( 30, 106 )
		{
			Closable = false;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImage(0, 0, 139);
		}
	}
	public class GiftOfRenewalGump : Gump
	{
		public GiftOfRenewalGump() : base( 30, 146 )
		{
			Closable = false;
			Disposable = false;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImage(0, 0, 140);
		}
	}
}
