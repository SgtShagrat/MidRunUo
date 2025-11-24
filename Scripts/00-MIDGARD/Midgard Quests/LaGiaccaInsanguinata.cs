using System;
using Midgard.Items;
using Server;
using Server.Engines.Quests;
using Server.Items;

namespace Midgard.Engines.Quests
{
    public class LaGiaccaInsanguinata : BaseQuest
    {
        public LaGiaccaInsanguinata()
        {
            AddObjective( new ObtainObjective( typeof( PiratesJacket ), "Giacca Insanguinata", 1, 0, 0 ) );

            AddReward( new BaseReward( typeof( Gold ), 6000, "gold coins" ) );
            // AddReward( new BaseReward( typeof( RadiantDiamondIngot ), 50, "radiant nimbus diamond ingots" ) );
            AddReward( new BaseReward( typeof( GoldIngot ), 80, "gold ingot" ) );
        }

        public override object Title
        {
            get { return "LaGiaccaInsanguinata"; }
        }

        public override object Description
        {
            get
            {
                return "Bentrovato, uhm..si..tu...perchè no?! Potresti essermi utile sai? Ti ho mai raccontato quando incontrai per mare Haghar, il più temuto tra i pirati? Non fui molto fortunato allora, e dovetti buttarmi tra le onde e le mia nave in fiamme! Tutta sfortuna..già! Quel cane pochi mesi dopo ebbe ciò che si meritava: Alcuni marinai dicono di aver avvistato i resti del suo galeone in un'isola sperduta del grande mare. Zaenon di sicuro l'avrà risparmiato, me lo sento. Se è così, dovete scovarlo e ucciderlo! Portatemi la sua giacca come prova.";
            }
        }

        public override object Refuse
        {
            get { return "Ah-Ah! Chiederò altrove, a chi non abbia mal di mare magari..."; }
        }

        public override object Uncomplete
        {
            get
            {
                return "Allora!? Cosa fate li impalati, non vedo nessuna giacca! Perchè non pensate a procurarvi una nave?";
            }
        }

        public override object Complete
        {
            get { return "Per tutti i mari, la giacca di Haghar! Tenete, vi siete meritati una bella ricompensa..."; }
        }

        public override bool DoneOnce
        {
            get { return false; }
        }

        public override bool AllObjectives
        {
            get { return false; }
        }

        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromHours( 1.0 ); }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class CaptainBlackseal : MondainQuester
    {
        public override Type[] Quests
        {
            get
            {
                return new Type[] 
		        { 
			        typeof( LaGiaccaInsanguinata )
		        };
            }
        }

        [Constructable]
        public CaptainBlackseal()
            : base( "Captain Blackseal", "" )
        {
        }

        public CaptainBlackseal( Serial serial )
            : base( serial )
        {
        }

        public override void InitBody()
        {
            Female = false;

            base.InitBody();
        }

        public override void InitOutfit()
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}