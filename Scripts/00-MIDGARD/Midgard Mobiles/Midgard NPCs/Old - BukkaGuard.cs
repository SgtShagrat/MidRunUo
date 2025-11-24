using System;
using Server.Items;

namespace Server.Mobiles 
{  
   public class BukkaGuard : BaseCreature 
   { 
      [Constructable] 
      public BukkaGuard() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
      { 
         	SpeechHue = Utility.RandomDyedHue(); 
         	
         	Hue = Utility.RandomSkinHue();
    
         	if ( this.Female = Utility.RandomBool() ) 
         		{ 
            	Body = 0x191; 
            	Name = NameList.RandomName( "female" ); 
            	Title = "the Buccaneer's Den Guard"; 
		            	
		         } 
         	else 
         		{ 
            	Body = 0x190; 
            	Name = NameList.RandomName( "male" ); 
            	Title = "the Buccaneer's Den Guard"; 
         
         		} 

         	SetStr( 1550, 1700 ); 
         	SetDex( 1550, 1750 ); 
         	SetInt( 1300, 2000 ); 

         	SetHits( 4000, 5000 ); 

         	SetDamage( 300, 350 ); 

         	SetSkill( SkillName.EvalInt, 120 ); 
			SetSkill( SkillName.MagicResist,215.5 ); 
         	SetSkill( SkillName.Anatomy, 120 ); 
         	SetSkill( SkillName.Tactics, 120 ); 
         	SetSkill( SkillName.Wrestling, 120 ); 
         	SetSkill( SkillName.Meditation, 120); 
			SetSkill( SkillName.Magery, 400); 
         	Fame = 10000; 
         	Karma = -10000; 

         	VirtualArmor = 70; 

         

         	Item Hair = new Item( 0x203C );
			Hair.Hue = Utility.RandomHairHue();
			Hair.Layer = Layer.Hair;
			Hair.Movable = false;
	         AddItem( Hair );  
	
         	AddItem( new Shirt(Utility.RandomNeutralHue()) );
         	AddItem( new Boots(Utility.RandomNeutralHue()) );
         	AddItem( new LongPants(Utility.RandomNeutralHue()) );
         	AddItem( new BodySash(Utility.RandomNeutralHue()) );
         	AddItem( new Bandana(Utility.RandomNeutralHue()) );
         	
         	AddItem( new Scimitar() );
         	
			new Horse().Rider = this;
	}

		public override bool OnBeforeDeath()
		{
			IMount mount = this.Mount;

			if ( mount != null )
				mount.Rider = null;

			if ( mount is Mobile )
				((Mobile)mount).Delete();

			return base.OnBeforeDeath();
		}

       


      	public override bool AlwaysMurderer{ get{ return true; } } 
      	public override bool ShowFameTitle{ get{ return false; } }
		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

      public BukkaGuard( Serial serial ) : base( serial ) 
      { 
      } 
      
      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 

         writer.Write( (int) 0 ); // version 
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 

         int version = reader.ReadInt(); 
      } 
   } 
}
