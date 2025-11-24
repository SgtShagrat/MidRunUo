using System;
using Server.Gumps;
using Server.Mobiles;
using Midgard.Engines.XmlGumps;
namespace Midgard.Engines.BossSystem.WishSpectre
{
	internal class QuestGump : XmlGump
	{
		public CustomDungeon Dungeon {get;private set;}
		public PlayerMobile Player {get;private set;}
		public QuestGump (CustomDungeon dungeon, PlayerMobile player) : base("quests/wishspectre/bonespeak.xml")
		{
			Dungeon = dungeon;
			Player = player;	
			
			Send( Player );
		}
		
		protected override void BeforeDesign ()
		{
			var stage = Dungeon.GetPlayerStageSet( Player ) as WishSpectreStageSet;

			var shadow = 1175;
			var spectral = SpectralBone.SpectralHue;
			Variables["spectral"] = spectral;
			Variables["shadow"] = shadow;

			/*le 5 ossa*/
			foreach(SpectralBoneTypes bone in Enum.GetValues(typeof(SpectralBoneTypes)))
			{			
				var hasbone = stage==null ? false : stage.HasBone(bone);
				Variables[ bone+".id"] = (stage == null || !hasbone) ? SpectralBone.GetRandomId(bone) : stage.GetBone(bone).ItemID;
				Variables[ bone+".hue"] = (stage == null || !hasbone) ? shadow : spectral;
			}

			var obj = "";
			if (stage == null || stage.Bones==0 )
				 obj = "Trova le 5 ossa differenti di Exurk Alh.";
			else if (stage.Bones<4)
				 obj = "Trova altre " + (5 - stage.Bones) + " ossa differenti di Exurk Alh.";
			else if (stage.Bones<5)
				obj = "Trova l'ultimo osso di Exurk Alh." ;
			
			Variables["objective.text"] = obj;
			
			
			if (stage==null || stage.Bones<5)
			{				
				Variables["message"] = "{bonetext}";
			}
			else
			{
				Variables["message"] = "{bonetext_done}";				
			}			
		}
		
		private string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}
		private string Size( string text, int size )
		{
			return String.Format( "<BASEFONT SIZE={0}>{1}</BASEFONT>", size, text );
		}
	}
}

