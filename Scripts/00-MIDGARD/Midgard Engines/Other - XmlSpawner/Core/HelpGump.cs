using Server.Gumps;

namespace Server.Mobiles
{
    public class HelpGump : Gump
    {
        public HelpGump( XmlSpawner spawner, int x, int y )
            : base( x, y )
        {
            if( spawner == null || spawner.Deleted )
                return;

            AddPage( 0 );

            int width = 370;

            AddBackground( 20, 0, width, 480, 5054 );

            AddPage( 1 );
            //AddAlphaRegion( 20, 0, 220, 554 );
            AddImageTiled( 20, 0, width, 480, 0x52 );
            //AddImageTiled( 24, 6, 213, 261, 0xBBC );

            AddLabel( 27, 2, 0x384, "Standalone Keywords" );
            AddHtml( 25, 20, width - 10, 440,
                    "spawntype[,arg1,arg2,...]\n" +
                    "SET[,itemname or serialno][,itemtype]/property/value/...\n" +
                    "SETVAR,varname/value\n" +
                    "SETONMOB,mobname[,mobtype]/property/value/...\n" +
                    "SETONTRIGMOB/property/value/...\n" +
                    "SETONTHIS/property/value/...\n" +
                    "SETONPARENT/property/value/...\n" +
                    "SETONNEARBY,range,name[,type][,searchcontainers]/prop/value/prop/value...\n" +
                    "SETONPETS,range/prop/value/prop/value...\n" +
                    "SETONCARRIED,itemname[,itemtype][,equippedonly]/property/value/...\n" +
                    "SETONSPAWN[,spawnername],subgroup/property/value/...\n" +
                    "SETONSPAWNENTRY[,spawnername],entrystring/property/value/...\n" +
                    "SPAWN[,spawnername],subgroup\n" +
                    "DESPAWN[,spawnername],subgroup\n" +
                    "{GET or RND keywords}\n" +
                    "GIVE[,prob]/itemtype\n" +
                    "GIVE[,prob]/&lt;itemtype/property/value...>\n" +
                    "TAKE[,prob[,quantity[,true[,itemtype]]]]/itemname\n" +
                    "TAKEBYTYPE[,prob[,quantity[,true]]]/itemtype\n" +
                    "IF/condition/thenspawn[/elsespawn]\n" +
                    "WAITUNTIL[,duration][,timeout][/condition][/spawngroup]\n" +
                    "WHILE/condition/spawngroup\n" +
                    "GOTO/subgroup\n" +
                    "BROWSER/url\n" +
                    "MUSIC,musicname[,range]\n" +
                    "SOUND,value\n" +
                    "EFFECT,itemid,duration[,x,y,z]\n" +
                    "MEFFECT,itemid[,speed][,x,y,z][,x2,y2,z2]" +
                    "RESURRECT[,range][,PETS]\n" +
                    "POISON,level[,range][,playeronly]\n" +
                    "DAMAGE,dmg,phys,fire,cold,pois,energy[,range][,playeronly]\n" +
                    "CAST,spellname[,arg] or CAST,spellnumber[,arg]\n" +
                    "SENDMSG/text\n" +
                    "BCAST[,hue][,font]/text\n" +
                    "GUMP,title,number[,gumpconstructor]/text",
                    false, true );
            AddButton( width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 2 );
            AddLabel( width - 38, 2, 0x384, "1" );
            AddButton( width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 4 );

            AddPage( 2 );
            AddLabel( 27, 2, 0x384, "Value and Itemtype Keywords" );
            AddHtml( 25, 20, width - 10, 440,
                    "property/@value\n" +
                    "ARMOR,minlevel,maxlevel\n" +
                    "WEAPON,minlevel,maxlevel\n" +
                    "JARMOR,minlevel,maxlevel\n" +
                    "JWEAPON,minlevel,maxlevel\n" +
                    "JEWELRY,minlevel,maxlevel\n" +
                    "SARMOR,minlevel,maxlevel\n" +
                    "SHIELD,minlevel,maxlevel\n" +
                    "POTION\n" +
                    "SCROLL,mincircle,maxcircle\n" +
                    "NECROSCROLL,index\n" +
                    "LOOT,methodname\n" +
                    "LOOTPACK,loottype\n" +
                    "MOB,name[,mobtype]\n" +
                    "TRIGMOB\n" +
                    "TAKEN\n" +
                    "GIVEN\n" +
                    "GET,itemname or serialno[,itemtype],property\n" +
                    "GETVAR,varname\n" +
                    "GETONCARRIED,itemname[,itemtype][,equippedonly],property\n" +
                    "GETONNEARBY,range,name[,type][,searchcontainers],property\n" +
                    "GETONMOB,mobname[,mobtype],property\n" +
                    "GETONGIVEN,property\n" +
                    "GETONTAKEN,property\n" +
                    "GETONPARENT,property\n" +
                    "GETONSPAWN[,spawnername],subgroup,property\n" +
                    "GETONSPAWN[,spawnername],subgroup,COUNT\n" +
                    "GETONTHIS,property\n" +
                    "GETONTRIGMOB,property\n" +
                    "GETONATTACH,type[,name],property\n" +
                    "...&lt;ATTACHMENT,type,name,property> as GET property\n" +
                    "{GET or RND keywords}\n" +
                    "RND,min,max\n" +
                    "RNDLIST,int1[,int2,...]\n" +
                    "RNDSTRLIST,str1[,str2,...]\n" +
                    "RNDBOOL\n" +
                    "RANDNAME,nametype\n" +
                    "PLAYERSINRANGE,range\n" +
                    "AMOUNTCARRIED,itemtype\n" +
                    "OFFSET,x,y,[,z]\n" +
                    "ANIMATE,action[,nframes][,nrepeat][,forward][,repeat][delay]\n" +
                    "MSG[,prob]/text\n" +
                    "SENDASCIIMSG[,probability][,hue][,font/text\n" +
                    "SENDMSG[,probability][,hue]/text\n" +
                    "SAY[,prob]/text\n" +
                    "SKILL,skillname\n" +
                    "TRIGSKILL,name|value|base|cap\n" +
                    "MUSIC,musicname[,range]\n" +
                    "SOUND,value\n" +
                    "EFFECT,itemid,duration[,x,y,z]\n" +
                    "MEFFECT,itemid[,speed][,x,y,z]" +
                    "POISON,level[,range][,playeronly]\n" +
                    "DAMAGE,dmg,phys,fire,cold,pois,energy[,range][,playeronly]\n" +
                    "INC,value or INC,min,max\n" +
                    "MUL,value or MUL,min,max\n" +
                    "ATTACH[,prob]/attachmenttype[,args]\n" +
                    "ATTACH[,prob]/&lt;attachmenttype[,args]/property/value...>\n" +
                    "ADD[,prob]/itemtype[,args]\n" +
                    "ADD[,prob]/&lt;itemtype[,args]/property/value...>\n" +
                    "DELETE\n" +
                    "KILL\n" +
                    "UNEQUIP,layer[,delete]\n" +
                    "EQUIP[,prob]/itemtype[,args]\n" +
                    "EQUIP[,prob]/&lt;itemtype[,args]/property/value...>",
                    false, true );
            AddButton( width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 3 );
            AddLabel( width - 41, 2, 0x384, "2" );
            AddButton( width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 1 );

            AddPage( 3 );
            AddLabel( 27, 2, 0x384, "[ Commands" );

            AddHtml( 25, 20, width - 10, 440,
                    "XmlAdd [-defaults]\n" +
                    "XmlShow\n" +
                    "XmlHide\n" +
                    "XmlFind\n" +
                    "AddAtt type [args]\n" +
                    "GetAtt [type]\n" +
                    "DelAtt [type][serialno]\n" +
                    "AvailAtt\n" +
                    "SmartStat [accesslevel AccessLevel]\n" +
                    "OptimalSmartSpawning [maxdiff]\n" +
                    "XmlSpawnerWipe [prefix]\n" +
                    "XmlSpawnerWipeAll [prefix]\n" +
                    "XmlSpawnerRespawn [prefix]\n" +
                    "XmlSpawnerRespawnAll [prefix]\n" +
                    "XmlHome [go][gump][send]\n" +
                    "XmlUnLoad filename [prefix]\n" +
                    "XmlLoad filename [prefix]\n" +
                    "XmlLoadHere filename [prefix][-maxrange range]\n" +
                    "XmlNewLoad filename [prefix]\n" +
                    "XmlNewLoadHere filename [prefix][-maxrange range]\n" +
                    "XmlSave filename [prefix]\n" +
                    "XmlSaveAll filename [prefix]\n" +
                    "XmlSaveOld filename [prefix]\n" +
                    "XmlSpawnerSaveAll filename [prefix]\n" +
                    "XmlImportSpawners filename\n" +
                    "XmlImportMap filename\n" +
                    "XmlImportMSF filename\n" +
                    "XmlDefaults [defaultpropertyname value]\n" +
                    "XmlGet property\n" +
                    "XmlSet property value",
                    false, true );

            AddButton( width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 4 );
            AddLabel( width - 41, 2, 0x384, "3" );
            AddButton( width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 2 );

            AddPage( 4 );
            AddLabel( 27, 2, 0x384, "Quest types" );
            AddHtml( 25, 20, width - 10, 180,
                    "KILL,mobtype[,count][,proptest]\n" +
                    "KILLNAMED,mobname[,type][,count][,proptest]\n" +
                    "GIVE,mobname,itemtype[,count][,proptest]\n" +
                    "GIVENAMED,mobname,itemname[,type][,count][,proptest]\n" +
                    "COLLECT,itemtype[,count][,proptest]\n" +
                    "COLLECTNAMED,itemname[,itemtype][,count][,proptest]\n" +
                    "ESCORT[,mobname][,proptest]\n",
                    false, true );

            AddLabel( 27, 200, 0x384, "Trigger/NoTriggerOnCarried" );
            AddHtml( 25, 220, width - 10, 50,
                    "ATTACHMENT,name,type\n" +
                    "itemname[,type][,EQUIPPED][,objective#,objective#,...]\n",
                    false, true );

            AddLabel( 27, 300, 0x384, "GUMPITEMS" );
            AddHtml( 25, 320, width - 10, 150,
                    "BUTTON,gumpid,x,y\n" +
                    "HTML,x,y,width,height,text\n" +
                    "IMAGE,gumpid,x,y[,hue]\n" +
                    "IMAGETILED,gumpid,x,y,width,height\n" +
                    "ITEM,itemid,x,y[,hue]\n" +
                    "LABEL,x,y,labelstring[,labelcolor]\n" +
                    "RADIO,gumpid1,gumpid2,x,y[,initialstate]\n" +
                    "TEXTENTRY,x,y,width,height[,text][,textcolor]\n",
                    false, true );

            AddButton( width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 1 );
            AddLabel( width - 41, 2, 0x384, "4" );
            AddButton( width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 3 );
        }
    }
}