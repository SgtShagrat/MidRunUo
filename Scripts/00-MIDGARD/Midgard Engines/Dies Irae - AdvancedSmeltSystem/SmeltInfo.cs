/***************************************************************************
 *                              SmeltInfo.cs
 *                            ----------------
 *   begin                : 02 gennaio, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server.Items;

namespace Midgard.Engines.AdvancedSmelting
{
    public class SmeltInfo
    {
        private static readonly SmeltInfo[] m_SmeltList = new SmeltInfo[]
          {
              new SmeltInfo(CraftResource.Iron, 2, CraftResource.OldDullCopper, 1, CraftResource.OldVerite, typeof (VeriteIngot), Temperature.Warm ),
              new SmeltInfo(CraftResource.Iron, 2, CraftResource.OldShadowIron, 1, CraftResource.OldValorite, typeof (ValoriteIngot), Temperature.Warm ),
              new SmeltInfo(CraftResource.OldDullCopper, 2, CraftResource.OldShadowIron, 1, CraftResource.OldGraphite, typeof (GraphiteIngot), Temperature.Warm ),
              new SmeltInfo(CraftResource.OldShadowIron, 2, CraftResource.OldCopper, 1, CraftResource.OldPyrite, typeof (PyriteIngot), Temperature.Hot ),
              new SmeltInfo(CraftResource.OldCopper, 2, CraftResource.OldBronze, 1, CraftResource.OldAzurite, typeof (AzuriteIngot), Temperature.Hot ),
              new SmeltInfo(CraftResource.OldBronze, 2, CraftResource.OldGold, 1, CraftResource.OldVanadium, typeof (VanadiumIngot), Temperature.Hot ),
              new SmeltInfo(CraftResource.OldAgapite, 2, CraftResource.OldVerite, 1,CraftResource.OldSilver, typeof (SilverIngot), Temperature.Hot ),
              new SmeltInfo(CraftResource.OldVerite, 2, CraftResource.OldValorite, 1, CraftResource.OldPlatinum, typeof (PlatinumIngot), Temperature.ReallyHot ),
              new SmeltInfo(CraftResource.OldValorite, 2, CraftResource.OldGraphite, 1, CraftResource.OldAmethyst, typeof (AmethystIngot), Temperature.ReallyHot ),
              new SmeltInfo(CraftResource.OldGraphite, 2, CraftResource.OldPyrite, 1, CraftResource.OldTitanium, typeof (TitaniumIngot), Temperature.ReallyHot ),
              new SmeltInfo(CraftResource.OldPyrite, 2, CraftResource.OldAzurite, 1, CraftResource.OldXenian, typeof (XenianIngot), Temperature.ReallyHot ),
              new SmeltInfo(CraftResource.OldAzurite, 2, CraftResource.OldVanadium, 1, CraftResource.OldRubidian, typeof (RubidianIngot), Temperature.Incandescent ),
              new SmeltInfo(CraftResource.OldVanadium, 2, CraftResource.OldSilver, 1, CraftResource.OldObsidian, typeof (ObsidianIngot), Temperature.Incandescent ),
              new SmeltInfo(CraftResource.OldTitanium, 2, CraftResource.OldXenian, 1, CraftResource.OldEbonSapphire, typeof (EbonSapphireIngot), Temperature.Incandescent ),
              new SmeltInfo(CraftResource.OldAmethyst, 2, CraftResource.OldRubidian, 1, CraftResource.OldDarkRuby, typeof (DarkRubyIngot), Temperature.Incandescent ),
              new SmeltInfo(CraftResource.OldPlatinum, 2, CraftResource.OldObsidian, 1, CraftResource.OldRadiantDiamond, typeof (RadiantDiamondIngot), Temperature.Incandescent ),
          };

        public SmeltInfo( CraftResource infoType1, int resAm1, CraftResource infoType2, int resAm2, CraftResource result, Type resultType, Temperature temperature )
        {
            Info1 = infoType1;
            ResAmount1 = resAm1;
            Info2 = infoType2;
            ResAmount2 = resAm2;
            ResultRes = result;
            ResultType = resultType;
            Temperature = temperature;
        }

        public int ResAmount1 { get; private set; }

        public int ResAmount2 { get; private set; }

        public CraftResource Info1 { get; private set; }

        public CraftResource Info2 { get; private set; }

        public Type ResultType { get; private set; }

        public Temperature Temperature { get; set; }

        public CraftResource ResultRes { get; private set; }

        public static SmeltInfo[] SmeltList
        {
            get { return m_SmeltList; }
        }

        public static SmeltInfo GetRecipe( AdvancedForge forge, out int amount )
        {
            amount = 0;

            if( forge.ForgeInfo1 == CraftResource.None || forge.ForgeInfo2 == CraftResource.None )
            {
                return null;
            }

            SmeltInfo smeltInfo = null;

            for( int i = 0; i < SmeltList.Length && smeltInfo == null; i++ )
            {
                SmeltInfo info = SmeltList[ i ];

                if( info.Info1 == forge.ForgeInfo1 && info.Info2 == forge.ForgeInfo2 )
                {
                    double dummy1 = forge.ForgeResourceAmount1 / (double)info.ResAmount1;
                    double dummy2 = forge.ForgeResourceAmount2 / (double)info.ResAmount2;

                    if( dummy1 == dummy2 )
                    {
                        amount = (int)dummy1;
                        smeltInfo = info;
                    }
                }
                else if( info.Info1 == forge.ForgeInfo2 && info.Info2 == forge.ForgeInfo1 )
                {
                    double dummy1 = forge.ForgeResourceAmount1 / (double)info.ResAmount2;
                    double dummy2 = forge.ForgeResourceAmount2 / (double)info.ResAmount1;

                    if( dummy1 == dummy2 )
                    {
                        amount = (int)dummy1;
                        smeltInfo = info;
                    }
                }
            }

            return smeltInfo;
        }

        public BaseIngot GetIngot()
        {
            try
            {
                return Activator.CreateInstance( ResultType ) as BaseIngot;
            }
            catch
            {
                return null;
            }
        }
    }
}