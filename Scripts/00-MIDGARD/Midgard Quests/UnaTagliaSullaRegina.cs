using System;
using Midgard.Mobiles;
using Server;
using Server.Engines.Quests;
using Server.Items;

namespace Midgard.Engines.Quests
{
    public class UnaTagliaSullaRegina : BaseQuest
    {
        public UnaTagliaSullaRegina()
        {
            AddObjective( new SlayObjective( typeof( EvilHumanFemale ), "Elora Regina di Ahnor", 1, "Ahnor", 0 ) );

            AddReward( new BaseReward( typeof( GoldIngot ), 80, "gold ingot" ) );
            AddReward( new BaseReward( typeof( Gold ), 5000, "gold coins" ) );
        }

        public override object Title
        {
            get { return "UnaTagliaSullaRegina"; }
        }

        public override object Description
        {
            get
            {
                return "E' passato molto tempo da quando vigevano le vecchie e gloriose alleanze." +
                       "Da allora molte cose sono cambiate, compreso il rapporto con la lontana Ahnor. I cavalieri di Elora, anche noti come Dragoni," +
                       "sembrano tornati alla cittadella, ma sotto le sembianze di spettri e fantasmi." +
                       "Le terre perdute sono un posto pericoloso, Malik solo sà cosa è successo a queste anime dannate per l'eternita e costrette a" +
                       "commettere crimini efferati contro chiunque oltrepassi i loro confini." +
                       "Devi porre fine a questo scempio, trova Elora, regina di Ahnor e sconfiggila. Per Britain! ";
            }
        }

        public override object Refuse
        {
            get { return "Torna quando sarai pronto, io attenderò."; }
        }

        public override object Uncomplete
        {
            get { return "Sei già qui? Non hai ancora terminato la tua missione cavaliere."; }
        }

        public override object Complete
        {
            get
            {
                return
                    "Quello che vedo d'innanzi a me è un coraggioso elemento da segnalare a Sir Kevorn. I tuoi servigi hanno lasciato il segno. Questo è per te...";
            }
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
            get { return TimeSpan.Zero; }
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

    public class Kiryan : MondainQuester
    {
        public override Type[] Quests
        {
            get
            {
                return new Type[] 
		        { 
			        typeof( UnaTagliaSullaRegina )
		        };
            }
        }

        [Constructable]
        public Kiryan()
            : base( "Kiryan", "" )
        {
        }

        public Kiryan( Serial serial )
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