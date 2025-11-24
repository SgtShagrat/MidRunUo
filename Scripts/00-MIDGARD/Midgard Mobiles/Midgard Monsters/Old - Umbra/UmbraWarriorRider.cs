using System;
using Server.Items;

namespace Server.Mobiles 
{ 
   public class UmbraWarriorRider : BaseCreature , IUmbraFolk
   { 
      [Constructable] 
      public UmbraWarriorRider() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
      { 
         	SpeechHue = Utility.RandomDyedHue(); 
         	Hue = 802; 

         	if ( this.Female = Utility.RandomBool() ) 
         		{ 
            	Body = 0x191; 
            	Name = NameList.RandomName( "female" ); 		            	
		        } 
         	else 
         		{ 
            	Body = 0x190; 
            	Name = NameList.RandomName( "male" );        
         		} 

           	Title = "the Umbra Warrior"; 

	        SetStr( 155, 170 ); 
         	SetDex( 95, 135 ); 
         	SetInt( 130, 200 ); 

         	SetHits( 250, 300 );

         	SetDamage( 10, 23 ); 

	        SetSkill( SkillName.Fencing, 90.0, 105.5 ); 
        	SetSkill( SkillName.Macing, 90.0, 105.5 ); 
    	    SetSkill( SkillName.MagicResist, 165.0, 195.5 ); 
	        SetSkill( SkillName.Swords, 90.0, 105.5 ); 
         	SetSkill( SkillName.MagicResist, 175.0, 215.5 ); 
         	SetSkill( SkillName.Anatomy, 85.0, 95.5 ); 
         	SetSkill( SkillName.Tactics, 95.0, 107.5 ); 
         	SetSkill( SkillName.Wrestling, 105.0, 117.5 ); 
	        SetSkill( SkillName.Parry, 105.0, 110.0 ); 
    	    SetSkill( SkillName.Lumberjacking, 100.0, 120.0 ); 
        	SetSkill( SkillName.Poisoning, 70.1, 100.0 );         	 
         	
         	Fame = 0; 
         	Karma = -10000; 

         	VirtualArmor = 45; 

         

         	Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x203C ) ); 
         	hair.Hue = 1153; 
         	hair.Layer = Layer.Hair; 
         	hair.Movable = false; 
         	AddItem( hair );

			BoneArms arms = new BoneArms();
			arms.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
			arms.LootType = LootType.Blessed;
			AddItem( arms );

			BoneGloves gloves = new BoneGloves();
			gloves.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
			gloves.LootType = LootType.Blessed;
			AddItem( gloves );

			BoneChest tunic = new BoneChest();
			tunic.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
			tunic.LootType = LootType.Blessed;
			AddItem( tunic );
				
			BoneLegs legs = new BoneLegs();
			legs.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
			legs.LootType = LootType.Blessed;
			AddItem( legs );

			BoneHelm helm = new BoneHelm();
			helm.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
			helm.LootType = LootType.Blessed;
			AddItem( helm );
				
			Cloak cloak = new Cloak();
			cloak.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
			cloak.LootType = LootType.Blessed;
			AddItem( cloak );										

			Boots boots = new Boots();
			boots.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
			boots.LootType = LootType.Blessed;
			AddItem( boots );								
				
			BronzeShield shield = new BronzeShield();
			shield.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
			shield.LootType = LootType.Blessed;
			AddItem( shield );

	        switch ( Utility.Random( 8 )) 
         		{ 
	            case 0: AddItem( new Longsword() ); break; 
	            case 1: AddItem( new BladedStaff() ); break; 
	            case 2: AddItem( new Scythe() ); break; 
	            case 3: AddItem( new Scepter() ); break; 
	            case 4: AddItem( new BoneHarvester() ); break; 
	            case 5: AddItem( new DoubleBladedStaff() ); break; 
	            case 6: AddItem( new Pitchfork() ); break;
	            case 7: AddItem( new CrescentBlade() ); break;             
		        }       
        	
	      	PackGem(); 
	        PackGold( 325, 375 ); 
	        PackMagicItems( 1, 5 ); 
	        PackSlayer(); 		
   				if ( 0.5 >= Utility.RandomDouble() )
						PackItem( new ExecutionersCap(3) );

			new Horse().Rider=this;
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

       
//      public override Poison HitPoison{ get{ return Poison.Regular; } } 
//	    public override double HitPoisonChance{ get{ return 50.0; } }   

      	public override bool AlwaysMurderer{ get{ return true; } } 
//		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public UmbraWarriorRider( Serial serial ) : base( serial ) 
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
