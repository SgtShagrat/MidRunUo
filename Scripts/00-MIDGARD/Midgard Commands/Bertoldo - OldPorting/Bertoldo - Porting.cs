#region AuthorHeader
//   Bertoldo
//	 29/07/2006
//
//  
//
#endregion AuthorHeader

using System;
using System.IO;
using System.Xml;

using Server;
using Server.Commands;
using Server.Engines.CannedEvil;
using Server.Gumps;
using Server.Items;

namespace Midgard.Engines.OldPorting
{
    public class Porting
    {
        private static bool Enabled = false;

        public static void RegisterCommands()
        {
            if( Enabled )
                CommandSystem.Register( "Porting", AccessLevel.Administrator, new CommandEventHandler( Porting_OnCommand ) );
        }

        [Usage( "Porting <account>" )]
        public static void Porting_OnCommand( CommandEventArgs e )
        {
            string testowarning = "";
            string testogump = "";
            int conta = 0;
            Mobile from = e.Mobile;
            string filePathLog = "porting/" + "Porting.log";
            string filePath = "porting/" + e.GetString( 0 ) + ".xml";
            //			int i = 0;

            if( !File.Exists( filePath ) )
            {
                from.SendMessage( "Non esiste un porting per questo account." );

                var di = new DirectoryInfo( "porting/" );
                FileInfo[] fi = di.GetFiles();

                string elencofile = "";
                foreach( FileInfo fiTemp in fi )
                {
                    //					from.SendMessage(fiTemp.Name+ "  "+Path.GetExtension(fiTemp.Name));
                    if( Path.GetExtension( fiTemp.Name ) == ".xml" )
                    {
                        elencofile += fiTemp.Name + "<br>";
                    }
                }
                from.SendGump( new NoticeGump( 1060635, 30720, elencofile,
                                             0xFFC000, 180, 400, null, null ) ); //1060635

                return;
            }
            TextWriter tw = File.AppendText( filePathLog );
            try
            {
                tw.WriteLine( from.Name + " per Account: " + e.GetString( 0 ) +
                             " ore: " + DateTime.Now.ToShortTimeString() + " del " +
                             DateTime.Now.Date.ToShortDateString() + "." );
            }
            finally
            {
                tw.Close();
            }

            var doc = new XmlTextReader( filePath );

            doc.WhitespaceHandling = WhitespaceHandling.None;

            //			Item tavolo1 = new LargeTable();
            Item cassa1 = new MetalChest();
            Item cassa2 = new MetalChest();
            Item cassa3 = new MetalChest();
            Item cassa4 = new MetalChest();
            cassa1.Hue = Utility.Random( 899 ); //2048;
            cassa2.Hue = Utility.Random( 899 ); //2045;
            cassa3.Hue = Utility.Random( 899 ); //2057;
            cassa4.Hue = Utility.Random( 899 ); //2057;
            ( (Container)cassa1 ).MaxItems = 350;
            ( (Container)cassa2 ).MaxItems = 350;
            ( (Container)cassa3 ).MaxItems = 350;
            ( (Container)cassa4 ).MaxItems = 350;
            //			tavolo1.Hue =96;

            //			tavolo1.DropToWorld(from,from.Location);
            cassa1.DropToWorld( from, from.Location );
            cassa2.DropToWorld( from, from.Location );
            cassa3.DropToWorld( from, from.Location );
            cassa4.DropToWorld( from, from.Location );
            var container1 = (Container)cassa1;
            var container2 = (Container)cassa2;
            var container3 = (Container)cassa3;
            var container4 = (Container)cassa4;
            container1.Name = "Armature";
            container2.Name = "Vesti e Materiale Vario";
            container3.Name = "Risorse";
            container4.Name = "Armi";
            container1.DisplayTo( from );
            container2.DisplayTo( from );
            container3.DisplayTo( from );
            container4.DisplayTo( from );

            while( doc.Read() )
            {
                if( doc.NodeType == XmlNodeType.Element && doc.Name == "type" )
                {
                    Item item = null;

                    bool isSos = false;
                    bool isChampionSkull = false;
                    doc.Read();

                    Type type = ScriptCompiler.FindTypeByName( doc.Value );

                    if( type == null )
                    {
                        TextWriter twerror1 = File.AppendText( filePathLog );
                        try
                        {
                            twerror1.WriteLine( "     ERRORE Oggetto: " + doc.Value + " non trovato!" );
                            testowarning += "ERRORE Oggetto: " + doc.Value + " non trovato!<br>";
                        }
                        finally
                        {
                            twerror1.Close();
                        }

                        from.SendMessage( "Oggetto non identificato." );
                        goto jump;
                    }
                    //					TextWriter twline = File.AppendText(filePathLog);
                    //					try
                    //					{
                    //						twline.WriteLine( "     Oggetto: " + type.ToString());
                    //					}
                    //					finally
                    //					{
                    //						twline.Close();
                    //					}
                    conta++;
                    testogump += conta + " - " + doc.Value + "<br>";

                    if( doc.Value == "RingmailGlovesOfMining" )
                    {
                        //						from.SendMessage ("Creato un RingmailGlovesOfMining");
                        item = new RingmailGlovesOfMining( 5 );
                        from.AddToBackpack( item );
                    }
                    else if( doc.Value == "StuddedGlovesOfMining" )
                    {
                        //						from.SendMessage ("Creato un StuddedGlovesOfMining");
                        item = new StuddedGlovesOfMining( 3 );
                        from.AddToBackpack( item );
                    }
                    else if( doc.Value == "ChampionSkull" )
                    {
                        //						from.SendMessage ("Creato un StuddedGlovesOfMining");
                        isChampionSkull = true;
                    }
                    else if( doc.Value == "RunicHammer" )
                    {
                        //						from.SendMessage ("Creato un RuniHammer");
                        item = new RunicHammer( CraftResource.DullCopper, 50 );
                        from.AddToBackpack( item );
                    }
                    else if( doc.Value == "AncientSmithyHammer" )
                    {
                        //						from.SendMessage ("Creato un AncientSmithyHammer");
                        item = new AncientSmithyHammer( 5 );
                        from.AddToBackpack( item );
                    }
                    else if( doc.Value == "RunicSewingKit" )
                    {
                        //						from.SendMessage ("Creato un RunicSewingKit");
                        item = new RunicSewingKit( CraftResource.SpinedLeather, 50 );
                        from.AddToBackpack( item );
                    }
                    else if( doc.Value == "RunicFletcherKit" )
                    {
                        //						from.SendMessage ("Creato un RunicSewingKit");
                        item = new RunicFletcherKit( CraftResource.Cedar, 10 );
                        from.AddToBackpack( item );
                    }
                    else if( doc.Value == "PowerScroll" ) //run 2.0
                    {
                        //						from.SendMessage ("Creata una SOP");
                        item = new PowerScroll( SkillName.Magery, 105 );
                        from.AddToBackpack( item );
                    }
                    else if( doc.Value == "StatCapScroll" ) //run 2.0
                    {
                        //						from.SendMessage ("Creata una SOS");
                        isSos = true;
                    }
                    else if( doc.Value == "BankCheck" ) //run 2.0
                    {
                        //						from.SendMessage ("Creato un assegno");
                        item = new BankCheck( 10 );
                        from.AddToBackpack( item );
                    }
                    else
                    {
                        try
                        {
                            item = (Item)Activator.CreateInstance( type );
                            //							from.SendMessage ("Creato un Item " + doc.Value);
                            from.AddToBackpack( item );
                            item.Movable = true;
                        }
                        catch
                        {
                            TextWriter twerror = File.AppendText( filePathLog );
                            try
                            {
                                twerror.WriteLine( "     ERRORE durante la crezione. Oggetto: " + type );
                                testowarning += "ERRORE durante la crezione. Oggetto: " + type + "<br>";
                            }
                            finally
                            {
                                twerror.Close();
                            }

                            from.SendMessage( "Un errore creando: " + doc.Value );
                            continue;
                        }
                    }
                    if( item == null && !isSos && !isChampionSkull )
                    {
                        break;
                    }

                    while( doc.Read() )
                    {
                        //						if (doc.NodeType == XmlNodeType.Element && doc.Name=="Attributi")
                        //							continue;

                        if( doc.NodeType == XmlNodeType.Element && doc.Name != "Attributi" )
                        {
                            string attributo = doc.Name;
                            doc.Read();
                            string valore = doc.Value;
                            //Console.WriteLine ("Attributo {0} Valore {1} ",attributo,valore);//type.ToString());

                            if( item is BaseArmor )
                            {
                                CreaBaseArmor( item, attributo, valore );
                            }

                            if( item is BaseWeapon )
                            {
                                CreaBaseWeapon( item, attributo, valore );
                            }

                            if( item is BaseClothing )
                            {
                                CreaBaseClothing( item, attributo, valore );
                            }

                            if( item is BaseJewel )
                            {
                                CreaBaseJewel( item, attributo, valore );
                            }

                            if( item is PetPorting )
                            {
                                CreaPetPorting( item, attributo, valore );
                            }

                            if( item is RecallRune )
                            {
                                CreaRecallRune( item, attributo, valore );
                            }

                            if( item is BaseRunicTool )
                            {
                                CreaBaseRunicTool( item, attributo, valore );
                            }
                            if( item is MonsterStatuette && attributo == "Type" ) //run 2
                            {
                                var itemStatuetta = (MonsterStatuette)item;
                                itemStatuetta.Type = (MonsterStatuetteType)Enum.Parse( typeof( MonsterStatuetteType ), valore );
                                itemStatuetta.IsOwner( from );
                            }
                            if( isChampionSkull && attributo == "Type" ) //run 2
                            {
                                item = new ChampionSkull( (ChampionSkullType)Enum.Parse( typeof( ChampionSkullType ), valore ) );
                                from.AddToBackpack( item );
                                isChampionSkull = false;
                                //								ChampionSkull itemChampionSkull = (ChampionSkull)item;
                                //								ChampionSkull.Type= (ChampionSkullType)Enum.Parse(typeof( ChampionSkullType),valore);
                            }
                            if( item is BankCheck && attributo == "Worth" ) //run 2
                            {
                                var itemAssegno = (BankCheck)item;
                                itemAssegno.Worth = Convert.ToInt32( valore );
                            }
                            if( item is PowerScroll ) //run 2
                            {
                                CreaPowerScroll( item, attributo, valore );
                            }
                            if( item is AncientSmithyHammer ) //run 2
                            {
                                CreaAncientSmithyHammer( item, attributo, valore );
                            }
                            if( isSos && attributo == "Value" ) //run 2
                            {
                                item = new StatCapScroll( Convert.ToInt32( valore ) );
                                from.AddToBackpack( item );
                                isSos = false;
                            }
                            if( attributo == "Serial" ) //run 2
                            {
                                TextWriter seriale = File.AppendText( @"porting/" + "Seriali.txt" );
                                try
                                {
                                    seriale.WriteLine( e.GetString( 0 ) + "," + DateTime.Now.Date.ToShortDateString() + "," + DateTime.Now.ToShortTimeString() + "," + type.Name + "," + valore );
                                }
                                finally
                                {
                                    seriale.Close();
                                }
                            }

                            if( item != null )
                            {
                                switch( attributo ) // generiche per Item
                                {
                                    case "Hue":
                                        item.Hue = Convert.ToInt32( valore );
                                        break;
                                    case "LootType":
                                        item.LootType = (LootType)Enum.Parse( typeof( LootType ), valore );
                                        break;
                                    case "Amount":
                                        item.Amount = Convert.ToInt32( valore );
                                        break;
                                    case "Name":
                                        item.Name = valore;
                                        break;
                                    case "itemID":
                                        item.ItemID = Convert.ToInt32( valore );
                                        break;
                                }
                            }
                        }
                        if( doc.NodeType == XmlNodeType.EndElement && doc.Name == "Attributi" )
                        {
                            break;
                        }
                    }
                    RiempiCasse( item, container1, container2, container3, container4 );
                }
            jump:
                continue;
            }
            from.SendGump( new NoticeGump( 1044055, 30720, testogump,
                                         0xFFC000, 250, 400, null, null ) ); //1060635
            from.SendGump( new NoticeGump( 1060635, 30720, testowarning,
                                         0xFFC000, 400, 400, null, null ) ); //1060635

            from.SendMessage( "Creati " + conta + " oggetti" );
            doc.Close();
            return;
        }

        private static void CreaBaseArmor( Item item, string attributo, string valore )
        {
            var armor = (BaseArmor)item;

            switch( attributo ) // generiche
            {
                case "Hue":
                    armor.Hue = Convert.ToInt32( valore );
                    break;
                case "StrRequirement":
                    armor.StrRequirement = Convert.ToInt32( valore );
                    break;
                case "HitPoints":
                    armor.HitPoints = Convert.ToInt32( valore );
                    break;
                case "MaxHitPoints": // RUN2
                    armor.MaxHitPoints = Convert.ToInt32( valore );
                    break;
                case "FireBonus":
                    armor.FireBonus = Convert.ToInt32( valore );
                    break;
                case "ColdBonus":
                    armor.ColdBonus = Convert.ToInt32( valore );
                    break;
                case "PhysicalBonus":
                    armor.PhysicalBonus = Convert.ToInt32( valore );
                    break;
                case "EnergyBonus":
                    armor.EnergyBonus = Convert.ToInt32( valore );
                    break;
                case "PoisonBonus":
                    armor.PoisonBonus = Convert.ToInt32( valore );
                    break;

                case "Skill_1_Name":
                    armor.SkillBonuses.Skill_1_Name = (SkillName)Enum.Parse( typeof( SkillName ), valore );
                    break;

                case "Skill_1_Value":
                    armor.SkillBonuses.Skill_1_Value = Convert.ToInt32( valore );
                    break;

                case "Skill_2_Name":
                    armor.SkillBonuses.Skill_2_Name = (SkillName)Enum.Parse( typeof( SkillName ), valore );
                    break;

                case "Skill_2_Value":
                    armor.SkillBonuses.Skill_2_Value = Convert.ToInt32( valore );
                    break;

                case "Skill_3_Name":
                    armor.SkillBonuses.Skill_3_Name = (SkillName)Enum.Parse( typeof( SkillName ), valore );
                    break;

                case "Skill_3_Value":
                    armor.SkillBonuses.Skill_3_Value = Convert.ToInt32( valore );
                    break;

                case "Skill_4_Name":
                    armor.SkillBonuses.Skill_4_Name = (SkillName)Enum.Parse( typeof( SkillName ), valore );
                    break;

                case "Skill_4_Value":
                    armor.SkillBonuses.Skill_4_Value = Convert.ToInt32( valore );
                    break;

                case "Skill_5_Name":
                    armor.SkillBonuses.Skill_5_Name = (SkillName)Enum.Parse( typeof( SkillName ), valore );
                    break;

                case "Skill_5_Value":
                    armor.SkillBonuses.Skill_5_Value = Convert.ToInt32( valore );
                    break;
            }

            switch( attributo ) // specifico per basearmor
            {
                case "DurabilityBonus":
                    armor.ArmorAttributes.DurabilityBonus = Convert.ToInt32( valore );
                    break;
                case "LowerStatReq":
                    armor.ArmorAttributes.LowerStatReq = Convert.ToInt32( valore );
                    break;
                case "MageArmor":
                    armor.ArmorAttributes.MageArmor = Convert.ToInt32( valore );
                    break;
                case "SelfRepair":
                    armor.ArmorAttributes.SelfRepair = Convert.ToInt32( valore );
                    break;
            }

            switch( attributo )
            {
                case "AttackChance":
                    armor.Attributes.AttackChance = Convert.ToInt32( valore );
                    break;
                case "BonusDex":
                    armor.Attributes.BonusDex = Convert.ToInt32( valore );
                    break;
                case "BonusInt":
                    armor.Attributes.BonusInt = Convert.ToInt32( valore );
                    break;
                case "BonusStr":
                    armor.Attributes.BonusStr = Convert.ToInt32( valore );
                    break;
                case "BonusMana":
                    armor.Attributes.BonusMana = Convert.ToInt32( valore );
                    break;
                case "BonusStam":
                    armor.Attributes.BonusStam = Convert.ToInt32( valore );
                    break;
                case "BonusHits":
                    armor.Attributes.BonusHits = Convert.ToInt32( valore );
                    break;
                case "CastRecovery":
                    armor.Attributes.CastRecovery = Convert.ToInt32( valore );
                    break;
                case "CastSpeed":
                    armor.Attributes.CastSpeed = Convert.ToInt32( valore );
                    break;
                case "DefendChance":
                    armor.Attributes.DefendChance = Convert.ToInt32( valore );
                    break;
                case "EnhancePotions":
                    armor.Attributes.EnhancePotions = Convert.ToInt32( valore );
                    break;
                case "LowerManaCost":
                    armor.Attributes.LowerManaCost = Convert.ToInt32( valore );
                    break;
                case "LowerRegCost":
                    armor.Attributes.LowerRegCost = Convert.ToInt32( valore );
                    break;
                case "Luck":
                    armor.Attributes.Luck = Convert.ToInt32( valore );
                    break;
                //				case "NightSight":
                //					armor.Attributes.NightSight  = Convert.ToInt32(valore);
                //					break;
                case "ReflectPhysical":
                    armor.Attributes.ReflectPhysical = Convert.ToInt32( valore );
                    break;
                case "RegenHits":
                    armor.Attributes.RegenHits = Convert.ToInt32( valore );
                    break;
                case "RegenMana":
                    armor.Attributes.RegenMana = Convert.ToInt32( valore );
                    break;
                case "RegenStam":
                    armor.Attributes.RegenStam = Convert.ToInt32( valore );
                    break;
                case "SpellChanneling":
                    armor.Attributes.SpellChanneling = Convert.ToInt32( valore );
                    break;
                case "SpellDamage":
                    armor.Attributes.SpellDamage = Convert.ToInt32( valore );
                    break;
                case "WeaponDamage":
                    armor.Attributes.WeaponDamage = Convert.ToInt32( valore );
                    break;
                case "WeaponSpeed":
                    armor.Attributes.WeaponSpeed = Convert.ToInt32( valore );
                    break;

                case "ArmorProtectionLevel":

                    switch( valore )
                    {
                        case "Regular":
                            armor.ProtectionLevel = ArmorProtectionLevel.Regular;
                            break;
                        case "Defense":
                            armor.ProtectionLevel = ArmorProtectionLevel.Defense;
                            break;
                        case "Guarding":
                            armor.ProtectionLevel = ArmorProtectionLevel.Guarding;
                            break;
                        case "Hardening":
                            armor.ProtectionLevel = ArmorProtectionLevel.Hardening;
                            break;
                        case "Fortification":
                            armor.ProtectionLevel = ArmorProtectionLevel.Fortification;
                            break;
                        case "Invulnerability":
                            armor.ProtectionLevel = ArmorProtectionLevel.Invulnerability;
                            break;
                    }
                    break;

                case "Resource":

                    armor.Resource = (CraftResource)Enum.Parse( typeof( CraftResource ), valore );
                    break;
            }
        }

        private static void CreaBaseWeapon( Item item, string attributo, string valore )
        {
            var armor = (BaseWeapon)item;

            switch( attributo ) // generiche
            {
                case "Hue":
                    armor.Hue = Convert.ToInt32( valore );
                    break;
                case "StrRequirement":
                    armor.StrRequirement = Convert.ToInt32( valore );
                    break;

                //				case "Skill_1_Value":
                //					armor.SkillBonuses.Skill_1_Value = Convert.ToInt32(valore);
                //					break;
                //
                //				case "Skill_2_Name":
                //					armor.SkillBonuses.Skill_2_Name = (SkillName)Enum.Parse(typeof( SkillName ),valore);
                //					break;
                //
                //				case "Skill_2_Value":
                //					armor.SkillBonuses.Skill_2_Value = Convert.ToInt32(valore);
                //					break;
                //
                //				case "Skill_3_Name":
                //					armor.SkillBonuses.Skill_3_Name = (SkillName)Enum.Parse(typeof( SkillName ),valore);
                //					break;
                //
                //				case "Skill_3_Value":
                //					armor.SkillBonuses.Skill_3_Value = Convert.ToInt32(valore);
                //					break;
                //
                //				case "Skill_4_Name":
                //					armor.SkillBonuses.Skill_4_Name = (SkillName)Enum.Parse(typeof( SkillName ),valore);
                //					break;
                //
                //				case "Skill_4_Value":
                //					armor.SkillBonuses.Skill_4_Value = Convert.ToInt32(valore);
                //					break;
                //
                //				case "Skill_5_Name":
                //					armor.SkillBonuses.Skill_5_Name = (SkillName)Enum.Parse(typeof( SkillName ),valore);
                //					break;
                //
                //				case "Skill_5_Value":
                //					armor.SkillBonuses.Skill_5_Value = Convert.ToInt32(valore);
                //					break;
            }

            switch( attributo ) // specifico per baseweapon
            {
                case "DurabilityBonus":
                    armor.WeaponAttributes.DurabilityBonus = Convert.ToInt32( valore );
                    break;
                case "HitColdArea":
                    armor.WeaponAttributes.HitColdArea = Convert.ToInt32( valore );
                    break;
                case "HitDispel":
                    armor.WeaponAttributes.HitDispel = Convert.ToInt32( valore );
                    break;
                case "HitEnergyArea":
                    armor.WeaponAttributes.HitEnergyArea = Convert.ToInt32( valore );
                    break;
                case "HitFireArea":
                    armor.WeaponAttributes.HitFireArea = Convert.ToInt32( valore );
                    break;
                case "HitFireball":
                    armor.WeaponAttributes.HitFireball = Convert.ToInt32( valore );
                    break;
                case "HitHarm":
                    armor.WeaponAttributes.HitHarm = Convert.ToInt32( valore );
                    break;
                case "HitLeechHits":
                    armor.WeaponAttributes.HitLeechHits = Convert.ToInt32( valore );
                    break;
                case "HitLeechMana":
                    armor.WeaponAttributes.HitLeechMana = Convert.ToInt32( valore );
                    break;
                case "HitLeechStam":
                    armor.WeaponAttributes.HitLeechStam = Convert.ToInt32( valore );
                    break;
                case "HitLightning":
                    armor.WeaponAttributes.HitLightning = Convert.ToInt32( valore );
                    break;
                case "HitLowerAttack":
                    armor.WeaponAttributes.HitLowerAttack = Convert.ToInt32( valore );
                    break;
                case "HitLowerDefend":
                    armor.WeaponAttributes.HitLowerDefend = Convert.ToInt32( valore );
                    break;
                case "HitMagicArrow":
                    armor.WeaponAttributes.HitMagicArrow = Convert.ToInt32( valore );
                    break;
                case "HitPhysicalArea":
                    armor.WeaponAttributes.HitPhysicalArea = Convert.ToInt32( valore );
                    break;
                case "HitPoisonArea":
                    armor.WeaponAttributes.HitPoisonArea = Convert.ToInt32( valore );
                    break;
                case "LowerStatReq":
                    armor.WeaponAttributes.LowerStatReq = Convert.ToInt32( valore );
                    break;
                case "MageWeapon":
                    armor.WeaponAttributes.MageWeapon = Convert.ToInt32( valore );
                    break;
                case "ResistColdBonus":
                    armor.WeaponAttributes.ResistColdBonus = Convert.ToInt32( valore );
                    break;
                case "ResistEnergyBonus":
                    armor.WeaponAttributes.ResistEnergyBonus = Convert.ToInt32( valore );
                    break;
                case "ResistFireBonus":
                    armor.WeaponAttributes.ResistFireBonus = Convert.ToInt32( valore );
                    break;
                case "ResistPhysicalBonus":
                    armor.WeaponAttributes.ResistPhysicalBonus = Convert.ToInt32( valore );
                    break;
                case "ResistPoisonBonus":
                    armor.WeaponAttributes.ResistPoisonBonus = Convert.ToInt32( valore );
                    break;
                case "SelfRepair":
                    armor.WeaponAttributes.SelfRepair = Convert.ToInt32( valore );
                    break;
                case "UseBestSkill":
                    armor.WeaponAttributes.UseBestSkill = Convert.ToInt32( valore );
                    break;
            }

            switch( attributo )
            {
                case "AttackChance":
                    armor.Attributes.AttackChance = Convert.ToInt32( valore );
                    break;
                case "BonusDex":
                    armor.Attributes.BonusDex = Convert.ToInt32( valore );
                    break;
                case "BonusInt":
                    armor.Attributes.BonusInt = Convert.ToInt32( valore );
                    break;
                case "BonusStr":
                    armor.Attributes.BonusStr = Convert.ToInt32( valore );
                    break;
                case "BonusMana":
                    armor.Attributes.BonusMana = Convert.ToInt32( valore );
                    break;
                case "BonusStam":
                    armor.Attributes.BonusStam = Convert.ToInt32( valore );
                    break;
                case "BonusHits":
                    armor.Attributes.BonusHits = Convert.ToInt32( valore );
                    break;
                case "CastRecovery":
                    armor.Attributes.CastRecovery = Convert.ToInt32( valore );
                    break;
                case "CastSpeed":
                    armor.Attributes.CastSpeed = Convert.ToInt32( valore );
                    break;
                case "DefendChance":
                    armor.Attributes.DefendChance = Convert.ToInt32( valore );
                    break;
                case "EnhancePotions":
                    armor.Attributes.EnhancePotions = Convert.ToInt32( valore );
                    break;
                case "LowerManaCost":
                    armor.Attributes.LowerManaCost = Convert.ToInt32( valore );
                    break;
                case "LowerRegCost":
                    armor.Attributes.LowerRegCost = Convert.ToInt32( valore );
                    break;
                case "Luck":
                    armor.Attributes.Luck = Convert.ToInt32( valore );
                    break;
                //				case "NightSight":
                //					armor.Attributes.NightSight  = Convert.ToInt32(valore);
                //					break;
                case "ReflectPhysical":
                    armor.Attributes.ReflectPhysical = Convert.ToInt32( valore );
                    break;
                case "RegenHits":
                    armor.Attributes.RegenHits = Convert.ToInt32( valore );
                    break;
                case "RegenMana":
                    armor.Attributes.RegenMana = Convert.ToInt32( valore );
                    break;
                case "RegenStam":
                    armor.Attributes.RegenStam = Convert.ToInt32( valore );
                    break;
                case "SpellChanneling":
                    armor.Attributes.SpellChanneling = Convert.ToInt32( valore );
                    break;
                case "SpellDamage":
                    armor.Attributes.SpellDamage = Convert.ToInt32( valore );
                    break;
                case "WeaponDamage":
                    armor.Attributes.WeaponDamage = Convert.ToInt32( valore );
                    break;
                case "WeaponSpeed":
                    armor.Attributes.WeaponSpeed = Convert.ToInt32( valore );
                    break;

                case "Resource":
                    armor.Resource = (CraftResource)Enum.Parse( typeof( CraftResource ), valore );

                    break;
            }
        }

        private static void CreaBaseClothing( Item item, string attributo, string valore )
        {
            var armor = (BaseClothing)item;

            //			switch (attributo)  // generiche
            //			{
            //				case "Hue":
            //					armor.Hue  = Convert.ToInt32(valore);
            //					break;
            //				case "StrRequirement":
            //					armor.StrRequirement  = Convert.ToInt32(valore);
            //					break;
            //				case "HitPoints":
            //					armor.HitPoints  = Convert.ToInt32(valore);
            //					break;
            //				case "Skill_1_Value":
            //					armor.SkillBonuses.Skill_1_Value = Convert.ToInt32(valore);
            //					break;
            //
            //				case "Skill_2_Name":
            //					armor.SkillBonuses.Skill_2_Name = (SkillName)Enum.Parse(typeof( SkillName ),valore);
            //					break;
            //
            //				case "Skill_2_Value":
            //					armor.SkillBonuses.Skill_2_Value = Convert.ToInt32(valore);
            //					break;
            //
            //				case "Skill_3_Name":
            //					armor.SkillBonuses.Skill_3_Name = (SkillName)Enum.Parse(typeof( SkillName ),valore);
            //					break;
            //
            //				case "Skill_3_Value":
            //					armor.SkillBonuses.Skill_3_Value = Convert.ToInt32(valore);
            //					break;
            //
            //				case "Skill_4_Name":
            //					armor.SkillBonuses.Skill_4_Name = (SkillName)Enum.Parse(typeof( SkillName ),valore);
            //					break;
            //
            //				case "Skill_4_Value":
            //					armor.SkillBonuses.Skill_4_Value = Convert.ToInt32(valore);
            //					break;
            //
            //				case "Skill_5_Name":
            //					armor.SkillBonuses.Skill_5_Name = (SkillName)Enum.Parse(typeof( SkillName ),valore);
            //					break;
            //
            //				case "Skill_5_Value":
            //					armor.SkillBonuses.Skill_5_Value = Convert.ToInt32(valore);
            //					break;

            //			}

            //			switch (attributo)
            //			{
            //				case "AttackChance":
            //					armor.Attributes.AttackChance  = Convert.ToInt32(valore);
            //					break;
            //				case "BonusDex":
            //					armor.Attributes.BonusDex  = Convert.ToInt32(valore);
            //					break;
            //				case "BonusInt":
            //					armor.Attributes.BonusInt  = Convert.ToInt32(valore);
            //					break;
            //				case "BonusStr":
            //					armor.Attributes.BonusStr  = Convert.ToInt32(valore);
            //					break;
            //				case "BonusMana":
            //					armor.Attributes.BonusMana  = Convert.ToInt32(valore);
            //					break;
            //				case "BonusStam":
            //					armor.Attributes.BonusStam  = Convert.ToInt32(valore);
            //					break;
            //				case "BonusHits":
            //					armor.Attributes.BonusHits  = Convert.ToInt32(valore);
            //					break;
            //				case "CastRecovery":
            //					armor.Attributes.CastRecovery  = Convert.ToInt32(valore);
            //					break;
            //				case "CastSpeed":
            //					armor.Attributes.CastSpeed  = Convert.ToInt32(valore);
            //					break;
            //				case "DefendChance":
            //					armor.Attributes.DefendChance  = Convert.ToInt32(valore);
            //					break;
            //				case "EnhancePotions":
            //					armor.Attributes.EnhancePotions  = Convert.ToInt32(valore);
            //					break;
            //				case "LowerManaCost":
            //					armor.Attributes.LowerManaCost = Convert.ToInt32(valore);
            //					break;
            //				case "LowerRegCost":
            //					armor.Attributes.LowerRegCost  = Convert.ToInt32(valore);
            //					break;
            //				case "Luck":
            //					armor.Attributes.Luck  = Convert.ToInt32(valore);
            //					break;
            //				case "NightSight":
            //					armor.Attributes.NightSight  = Convert.ToInt32(valore);
            //					break;
            //				case "ReflectPhysical":
            //					armor.Attributes.ReflectPhysical  = Convert.ToInt32(valore);
            //					break;
            //				case "RegenHits":
            //					armor.Attributes.RegenHits  = Convert.ToInt32(valore);
            //					break;
            //				case "RegenMana":
            //					armor.Attributes.RegenMana  = Convert.ToInt32(valore);
            //					break;
            //				case "RegenStam":
            //					armor.Attributes.RegenStam  = Convert.ToInt32(valore);
            //					break;
            //				case "SpellChanneling":
            //					armor.Attributes.SpellChanneling  = Convert.ToInt32(valore);
            //					break;
            //				case "SpellDamage":
            //					armor.Attributes.SpellDamage  = Convert.ToInt32(valore);
            //					break;
            //				case "WeaponDamage":
            //					armor.Attributes.WeaponDamage  = Convert.ToInt32(valore);
            //					break;
            //				case "WeaponSpeed":
            //					armor.Attributes.WeaponSpeed  = Convert.ToInt32(valore);
            //					break;
            //
            //
            //				
            //				case "Resource":
            //
            //				armor.Resource = (CraftResource)Enum.Parse(typeof( CraftResource ),valore);
            //				break;

            //			}
        }

        private static void CreaBaseJewel( Item item, string attributo, string valore )
        {
            var armor = (BaseJewel)item;

            // METTERE SKILL BONUS

            switch( attributo ) // generiche
            {
                case "FireResistance":
                    armor.Resistances.Fire = Convert.ToInt32( valore );
                    break;
                case "ColdResistance":
                    armor.Resistances.Cold = Convert.ToInt32( valore );
                    break;
                case "PhysicalResistance":
                    armor.Resistances.Physical = Convert.ToInt32( valore );
                    break;
                case "EnergyResistance":
                    armor.Resistances.Energy = Convert.ToInt32( valore );
                    break;
                case "PoisonResistance":
                    armor.Resistances.Poison = Convert.ToInt32( valore );
                    break;

                case "GemType":
                    armor.GemType = (GemType)Enum.Parse( typeof( GemType ), valore );
                    break;

                case "Skill_1_Name":
                    armor.SkillBonuses.Skill_1_Name = (SkillName)Enum.Parse( typeof( SkillName ), valore );
                    break;

                case "Skill_1_Value":
                    armor.SkillBonuses.Skill_1_Value = Convert.ToInt32( valore );
                    break;

                case "Skill_2_Name":
                    armor.SkillBonuses.Skill_2_Name = (SkillName)Enum.Parse( typeof( SkillName ), valore );
                    break;

                case "Skill_2_Value":
                    armor.SkillBonuses.Skill_2_Value = Convert.ToInt32( valore );
                    break;

                case "Skill_3_Name":
                    armor.SkillBonuses.Skill_3_Name = (SkillName)Enum.Parse( typeof( SkillName ), valore );
                    break;

                case "Skill_3_Value":
                    armor.SkillBonuses.Skill_3_Value = Convert.ToInt32( valore );
                    break;

                case "Skill_4_Name":
                    armor.SkillBonuses.Skill_4_Name = (SkillName)Enum.Parse( typeof( SkillName ), valore );
                    break;

                case "Skill_4_Value":
                    armor.SkillBonuses.Skill_4_Value = Convert.ToInt32( valore );
                    break;

                case "Skill_5_Name":
                    armor.SkillBonuses.Skill_5_Name = (SkillName)Enum.Parse( typeof( SkillName ), valore );
                    break;

                case "Skill_5_Value":
                    armor.SkillBonuses.Skill_5_Value = Convert.ToInt32( valore );
                    break;
            }

            switch( attributo )
            {
                case "AttackChance":
                    armor.Attributes.AttackChance = Convert.ToInt32( valore );
                    break;
                case "BonusDex":
                    armor.Attributes.BonusDex = Convert.ToInt32( valore );
                    break;
                case "BonusInt":
                    armor.Attributes.BonusInt = Convert.ToInt32( valore );
                    break;
                case "BonusStr":
                    armor.Attributes.BonusStr = Convert.ToInt32( valore );
                    break;
                case "BonusMana":
                    armor.Attributes.BonusMana = Convert.ToInt32( valore );
                    break;
                case "BonusStam":
                    armor.Attributes.BonusStam = Convert.ToInt32( valore );
                    break;
                case "BonusHits":
                    armor.Attributes.BonusHits = Convert.ToInt32( valore );
                    break;
                case "CastRecovery":
                    armor.Attributes.CastRecovery = Convert.ToInt32( valore );
                    break;
                case "CastSpeed":
                    armor.Attributes.CastSpeed = Convert.ToInt32( valore );
                    break;
                case "DefendChance":
                    armor.Attributes.DefendChance = Convert.ToInt32( valore );
                    break;
                case "EnhancePotions":
                    armor.Attributes.EnhancePotions = Convert.ToInt32( valore );
                    break;
                case "LowerManaCost":
                    armor.Attributes.LowerManaCost = Convert.ToInt32( valore );
                    break;
                case "LowerRegCost":
                    armor.Attributes.LowerRegCost = Convert.ToInt32( valore );
                    break;
                case "Luck":
                    armor.Attributes.Luck = Convert.ToInt32( valore );
                    break;
                //				case "NightSight":
                //					armor.Attributes.NightSight  = Convert.ToInt32(valore);
                //					break;
                case "ReflectPhysical":
                    armor.Attributes.ReflectPhysical = Convert.ToInt32( valore );
                    break;
                case "RegenHits":
                    armor.Attributes.RegenHits = Convert.ToInt32( valore );
                    break;
                case "RegenMana":
                    armor.Attributes.RegenMana = Convert.ToInt32( valore );
                    break;
                case "RegenStam":
                    armor.Attributes.RegenStam = Convert.ToInt32( valore );
                    break;
                case "SpellChanneling":
                    armor.Attributes.SpellChanneling = Convert.ToInt32( valore );
                    break;
                case "SpellDamage":
                    armor.Attributes.SpellDamage = Convert.ToInt32( valore );
                    break;
                case "WeaponDamage":
                    armor.Attributes.WeaponDamage = Convert.ToInt32( valore );
                    break;
                case "WeaponSpeed":
                    armor.Attributes.WeaponSpeed = Convert.ToInt32( valore );
                    break;
            }
        }

        private static void CreaPetPorting( Item item, string attributo, string valore )
        {
            var pp = (PetPorting)item;

            switch( attributo ) // generiche
            {
                case "Filled":
                    pp.Filled = Convert.ToBoolean( valore );
                    break;
                case "PetBonded":
                    pp.PetBonded = Convert.ToBoolean( valore );
                    break;
                case "PetControled":
                    pp.PetControled = Convert.ToBoolean( valore );
                    break;
                case "PetControlMasterName":
                    pp.PetControlMasterName = valore;
                    break;
                case "PetHue":
                    pp.PetHue = Convert.ToInt32( valore );
                    break;
                case "PetName":
                    pp.PetName = valore;
                    break;
                case "PetTypeString":
                    pp.PetTypeString = valore;
                    break;
            }
        }

        private static void CreaRecallRune( Item item, string attributo, string valore )
        {
            var rr = (RecallRune)item;

            switch( attributo ) // generiche
            {
                case "Description":
                    rr.Description = valore;
                    break;
                //				case "House":
                //					rr.House= valore;
                //					break;
                case "Marked":
                    rr.Marked = Convert.ToBoolean( valore );
                    break;
                case "Target":
                    rr.Target = Point3D.Parse( valore );
                    break;
                case "TargetMap":
                    rr.TargetMap = Map.Parse( valore );
                    break;
            }
        }

        private static void CreaBaseRunicTool( Item item, string attributo, string valore )
        {
            var armor = (BaseRunicTool)item;

            if( attributo == "Resource" )
            {
                armor.Resource = (CraftResource)Enum.Parse( typeof( CraftResource ), valore );
            }
            if( attributo == "UsesRemaining" )
            {
                armor.UsesRemaining = Convert.ToInt32( valore );
            }
        }

        private static void CreaAncientSmithyHammer( Item item, string attributo, string valore )
        {
            var armor = (AncientSmithyHammer)item;

            if( attributo == "Bonus" )
            {
                armor.Bonus = Convert.ToInt32( valore );
            }
            if( attributo == "UsesRemaining" )
            {
                armor.UsesRemaining = Convert.ToInt32( valore );
            }
        }

        private static void CreaPowerScroll( Item item, string attributo, string valore ) // run 2
        {
            var armor = (PowerScroll)item;

            if( attributo == "Skill" )
            {
                armor.Skill = (SkillName)Enum.Parse( typeof( SkillName ), valore );
            }
            if( attributo == "Value" )
            {
                armor.Value = Convert.ToInt32( valore );
            }
        }

        private static void RiempiCasse( Item item, Container container1, Container container2, Container container3, Container container4 )
        {
            if( item is BaseArmor )
            {
                container1.DropItem( item );
            }
            else if( item is BaseWeapon )
            {
                container4.DropItem( item );
            }
            else if( item is BaseJewel || item is BaseClothing )
            {
                container2.DropItem( item );
            }
            else
            {
                container3.DropItem( item );
            }
        }
    }
}