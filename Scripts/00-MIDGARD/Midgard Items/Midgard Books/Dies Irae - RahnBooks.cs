/***************************************************************************
 *                               Dies Irae - RahnBooks.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class RhanBook1 : BaseBook
    {
        [Constructable]
        public RhanBook1()
            : base( Utility.RandomList( 0xFEF, 0xFF0, 0xFF1, 0xFF2 ), "The Death of Rhan p.I", "Rhan Tal'vr", 2, false )
        {
            // NOTE: There are 8 lines per page and
            // approx 22 to 24 characters per line!
            //  0----+----1----+----2----+
            int cnt = 0;
            string[] lines;
            lines = new string[]
            {
                "I see also my", "inevitable death from", "these damned wounds.", "No potions left, the",
                "healing crystal and", "ring are used up, and", "me, with not even magic", "enough to light a",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "candle. Oh but the gods", "did give me other gifts,", "my precious books,",
                "the gift of Sword", "Singing, the thrill of", "battle. Ah but then", "that is my story, I get"
                , "ahead of myself...",
            };
            Pages[ cnt++ ].Lines = lines;
        }

        #region serialization
        public RhanBook1( Serial serial )
            : base( serial )
        {
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }
        #endregion
    }

    public class RhanBook2 : BaseBook
    {
        [Constructable]
        public RhanBook2()
            : base( Utility.RandomList( 0xFEF, 0xFF0, 0xFF1, 0xFF2 ), "The Death of Rhan p.II", "Rhan Tal'vr", 5, false )
        {
            // NOTE: There are 8 lines per page and
            // approx 22 to 24 characters per line!
            //  0----+----1----+----2----+
            int cnt = 0;
            string[] lines;
            lines = new string[]
            {
                "I am a simple warrior.", "I grew up as a Maiden", "of the Spirit Blade.", "As early as I can",
                "remember I wanted to be", "a Singer, to feel the", "hunger of the blade in",
                "my hands, to feel it",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "come alive and take my", "enemies. To me there is", "but ONE WAY. THE WAY",
                "of the SWORD. Ah this", "is hard to tell. I grew", "up in my noble family,",
                "the only one of three", "brothers and two",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "sisters that felt the", "calling, the Song of", "the Sword. Father", "understood, for he too",
                "had felt the call. At", "eleven, I entered the", "Hall of the Virtues of",
                "War and joined the",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "Maidens of the Spirit", "Sword. In my band there", "were six. We drank",
                "together, we fought, we", "wept, and we grew in", "the way of the sword.",
                "All are gone now, save", "me, and soon I will",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "join them. Join them in", "the unknown halls of", "the gods of war.", "",
                "...The pain is hungry,", "consuming what's left", "of me. I have not much", "time...",
            };
            Pages[ cnt++ ].Lines = lines;
        }

        #region serialization
        public RhanBook2( Serial serial )
            : base( serial )
        {
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }
        #endregion
    }

    public class RhanBook3 : BaseBook
    {
        [Constructable]
        public RhanBook3()
            : base( Utility.RandomList( 0xFEF, 0xFF0, 0xFF1, 0xFF2 ), "The Death of Rhan p.III", "Rhan Tal'vr", 8, false )
        {
            // NOTE: There are 8 lines per page and
            // approx 22 to 24 characters per line!
            //  0----+----1----+----2----+
            int cnt = 0;
            string[] lines;
            lines = new string[]
            {
                "I guess I had better", "tell of the final", "battle, the one that", "has left me here to",
                "die. Where to start, in", "the middle. Yes. We", "Maidens grew, learned,", "mastered the Way,"
                ,
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "We were in the Hall of", "the Virtues of War,", "fasting and praying", "when there came a",
                "knocking on the door. I", "opened it to find one", "of the guardians of the",
                "mountain pass, wounded",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "and near death. He told", "us of betrayal from the", "north. An invasion led",
                "by King Joile whom we", "thought an ally. We", "grabbed our weapons and",
                "armor and such potions", "as we could carry.",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "We flew to the pass", "hoping upon hope that", "we would not be too", "late. Our journey was",
                "not in vain, for we", "arrived just at the", "very point where the", "last three guardians",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "were overwhelmed by the", "horde. Into the pass we", "ran forming the old", "battle line, six"
                , "abreast. OH did we", "FIGHT. The Song of", "the Sword was a joyous", "noise slicing through"
                ,
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "the ranks of evil.  We", "fought for hours. Julia", "was the first to fall,",
                "a cowardly poisoned", "dagger finding a rent", "in her armor. And one",
                "by one all fell, save", "me. ",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "Then my beloved sword,", "the sword of my father,", "broke in my hands. All",
                "was lost, our six lives", "spent in vain. Now,", "many many of them would",
                "pour through the pass.", "I would be easy prey.",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "Like a newborn child, I", "wept in frustration.", "Then I remembered the",
                "hearth in our home -", "the book; the Way of", "Strategy. I reached", "within for the Shehai,"
                , "the Spirit Sword.",
            };
            Pages[ cnt++ ].Lines = lines;
        }

        #region serialization
        public RhanBook3( Serial serial )
            : base( serial )
        {
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }
        #endregion
    }

    public class RhanBook4 : BaseBook
    {
        [Constructable]
        public RhanBook4()
            : base( Utility.RandomList( 0xFEF, 0xFF0, 0xFF1, 0xFF2 ), "The Death of Rhan p.IV", "Rhan Tal'vr", 7, false )
        {
            // NOTE: There are 8 lines per page and
            // approx 22 to 24 characters per line!
            //  0----+----1----+----2----+
            int cnt = 0;
            string[] lines;
            lines = new string[]
            {
                "It was as if I was in a", "dream. I looked to my", "right and my left, my",
                "sisters, Maidens of the", "Sword, lay dead. Piled", "high around them were",
                "their fallen foes. Only", "I still stood, bloodied",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "and battered. In my", "hand I found my Spirit", "Sword, that which I",
                "could never form when I", "needed it, and behold...", "it was alive.", "Alive with fire.",
                "Ablaze with power.",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "OH I slew mightily,", "right and left, like a", "scythe through wheat.",
                "All the way to the foul", "and deceitful Lord of", "the North I fought.",
                "With one blow I cut his", "magical armor asunder,",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "one more took his head.", "But to do that deed", "cost me dearly, wounds", "by the dozen, for"
                , "although I had magical", "armor, it was not", "formed of spirit like", "my blade. And I was"
                ,
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "sorely wounded. With", "the felling of King", "Joile, his army", "crumbled. They fled",
                "before my wrath. All", "ran back through the", "pass not even pausing", "for their dead.",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "All who could stand ran", "for their lives, and I", "slew all I could reach,",
                "but my breath was", "coming short, and the", "pain... Finally I rested,",
                "on this rock where you", "find me now.",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "First a little sip of ", "water and ... then I ", "will ... I wish too ... ", "",
                "I fear the eternal ", "night is descending", "on me now...", "",
            };
            Pages[ cnt++ ].Lines = lines;
        }

        #region serialization
        public RhanBook4( Serial serial )
            : base( serial )
        {
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }
        #endregion
    }
}