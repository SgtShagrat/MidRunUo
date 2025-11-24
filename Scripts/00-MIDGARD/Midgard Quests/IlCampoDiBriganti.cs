using System;
using Server;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.Quests
{
    public class IlCampoDiBriganti : BaseQuest
    {
        public IlCampoDiBriganti()
        {
            AddObjective( new SlayObjective( typeof( Brigand ), "brigand", 10 ) );
            AddObjective( new ObtainObjective( typeof( Bandana ), "Bandana", 5, 0, 0 ) );

            AddReward( new BaseReward( typeof( Gold ), 1000, "gold coins" ) );
        }

        public override object Title
        {
            get { return "Il Campo Di Briganti"; }
        }

        public override object Description
        {
            get
            {
                return "Thal Mith, io sono Finwe, difensore di Calen Sul. Da qualche giorno riceviamo continui attacchi da parte di  un manipolo di briganti, stabilitosi a nord in un accampamento sulla via che porta ai confini di Yew. Se desideri provare il tuo valore, e rendere fedeltà al popolo dei Sindarin, ho un compito da affidarti. Recati al campo dei briganti, sconfiggi almeno dieci di loro e raccogli cinque bandane. ";
            }
        }

        public override object Refuse
        {
            get { return "Avrò altri a cui chiedere, non temere."; }
        }

        public override object Uncomplete
        {
            get
            {
                return "La caparbietà è la virtù dei forti. Ritenta e torna soltanto quando avrai adempito al tuo compito.";
            }
        }

        public override object Complete
        {
            get { return "Un'eccellente dimostrazione di forza, questa è la nostra gratidudine. "; }
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

    public class FinweLeaftear : MondainQuester
    {
        public override Type[] Quests
        {
            get
            {
                return new Type[] 
		        { 
			        typeof( IlCampoDiBriganti )
		        };
            }
        }

        [Constructable]
        public FinweLeaftear()
            : base( "Finwe Leaftear", "the wise" )
        {
        }

        public FinweLeaftear( Serial serial )
            : base( serial )
        {
        }

        public override void InitBody()
        {
            InitStats( 100, 100, 25 );

            Female = false;
            Race = Races.Core.HighElf;
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