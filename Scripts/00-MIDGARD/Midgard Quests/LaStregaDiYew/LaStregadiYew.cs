using System;

using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class LaStregadiYew : BaseQuest
    {
        public LaStregadiYew()
        {
            AddObjective( new SlayObjective( typeof( GreyWolf ), "Grey Wolf", 10 ) );
            AddObjective( new SlayObjective( typeof( Orc ), "Orc", 10 ) );
        }

        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromMinutes( 10080 ); }
        }

        public override object Title
        {
            get { return "La Strega di Yew"; }
        }

        public override object Description
        {
            get
            {
                return "Oh, quale dono di Ferech! Cercavo proprio qualcuno come voi." +
                       "La mia borsa è vuota, e questa notte la luna splenderà alta sul bosco di Yew." +
                       "E' in momenti come questi che una vecchia e stanca signora ha la sua possibilità." +
                       "Prima del rito, ho bisogno che venga versato il dovuto compenso." +
                       "Sporca la tua lama con il sangue di dieci Lupi dal pelo grigio, e altrettanti orchi." +
                       "Solo con questo sacrificio potrò preparare il mio rito." +
                       "Vuoi aiutarmi?";
            }
        }

        public override object Refuse
        {
            get { return "Andatevene allora! Se non mi servite siete inutile!"; }
        }

        public override object Uncomplete
        {
            get { return "Non avete ancora finito, tornate nel bosco e compite la vosra missione!"; }
        }

        public override object Complete
        {
            get { return "Ah! Le vostre armi parlano per voi, il sacrificio è stato compiuto e ora tutto è pronto! Prendete, prendete.. e lasciatemi sola, ho da fare!"; }
        }

        public override bool CanOffer()
        {
            return true;
        }
    }
}