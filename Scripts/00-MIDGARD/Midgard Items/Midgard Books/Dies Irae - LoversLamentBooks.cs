/***************************************************************************
 *                               Dies Irae - LoversLamentBooks.cs
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
    public class LoversLamentBook : BaseBook
    {
        [Constructable]
        public LoversLamentBook()
            : base( Utility.RandomList( 0xFEF, 0xFF0, 0xFF1, 0xFF2 ), "The Lover's Lament", "Croll Baumoval", 11, false )
        {
            // NOTE: There are 8 lines per page and
            // approx 22 to 24 characters per line!
            //  0----+----1----+----2----+
            int cnt = 0;
            string[] lines;

            lines = new string[]
            {
                "The night is very dark.", "Wind moves in the ", "willow trees. All is ",
                "quiet around the shores ", "of the small lake. The ", "moons reflect in the ",
                "rippling surface of the ", "water. An owl's call",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "echoes. No lights are ", "shining from the castle", "nearby; it appears ",
                "deserted. As the night ", "wears on, a faint glow ", "appears near the ", "castle. The light "
                , "slowly moves towards ",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "the lake, and upon ", "reaching the shore, ", "stops. A figure, a ", "beautiful woman by any "
                , "measure, stands looking ", "wistfully into the dark ", "water. Her lantern ",
                "flickers in the breeze,",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "illuminating only her. ", "Tears stream down her ", "cheeks; her gown, once ",
                "beautiful, is now ", "tattered and stained. ", "The surface of the lake ",
                "becomes agitated, but ", "not from a wind as the",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "night has become still. ", "Slowly from the water ", "emerges the figure of a ",
                "man, a warrior, fully ", "adorned in the armor of ", "a knight on the field ",
                "of battle. He seems to ", "float over the water",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "towards the woman and ", "stop just short of her. ", "'Madylina,' the ghostly ",
                "warrior intones. 'My ", "Lord, Gerthland,' ", "whispers the lovely ",
                "Madylina as she kneels. ", "'You have come to me",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "again.'  'Yes,' ", "Gerthland responds, 'My ", "days are long waiting ",
                "for the night in which ", "I can see my love.' The ", "lovers stand looking ",
                "wistfully at each ", "other, unable to touch,",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "unable to kiss, unable", "to satisfy their ", "unrequited love until ", "the first tinges of "
                , "dawn start to color the ", "western sky. Gerthland ", "drops something to the ",
                "ground as does Madylina.",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "Each departing, the ", "lake again takes ", "possession of the ", "handsome knight, while ",
                "the beautiful maiden ", "turns slowly to the ", "castle. The waters ", "settle into a gentle",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "quiet and the light ", "of Madylina’s lantern ", "disappears. ", "Dawn breaks over the ",
                "lake. On the shore are ", "two beautiful roses;", "one crimson and the ", "other white.",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "Ripples from the lake ", "quickly overtake the ", "flowers pulling them ",
                "into the water--leaving ", "the shore bare as it ", "was in the hours before ",
                "darkness fell.", "",
            };
            Pages[ cnt++ ].Lines = lines;
        }

        #region serialization
        public LoversLamentBook( Serial serial )
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

    public class GerthlandBook : BaseBook
    {
        [Constructable]
        public GerthlandBook()
            : base( Utility.RandomList( 0xFEF, 0xFF0, 0xFF1, 0xFF2 ), "Gerthland Manor", "Croll Baumoval", 6, false )
        {
            // NOTE: There are 8 lines per page and
            // approx 22 to 24 characters per line!
            //  0----+----1----+----2----+
            int cnt = 0;
            string[] lines;

            lines = new string[]
            {
                "The townfolk around ", "Gerthland Manor tell ", "often of seeing two ", "ghostly lovers in a "
                , "nightly meeting. The ", "Boar's Bristle Inn is ", "always rumbling with ", "conversation ",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "about them. The locals ", "speak of Lord Gerthland ", "and Lady Madylina who ",
                "were once betrothed. ", "Lord Gerthland was ", "called to battle to ",
                "defend the land. While ", "Hergen, the castle's ",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "resident sorcerer, ", "became enflamed with ", "love and lust for the ",
                "Lady Madylina; only to ", "be rebuked by her. The ", "sad story tells of Lord ",
                "Gerthland's death on ", "the field of battle ",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "and of Lady Madylina's ", "death by her own hand ", "at the news. Hergen's ",
                "curse on both their ", "souls that will not ", "allow them to rest ", "until Madylina will ",
                "agree to become his ",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "consort even in death. ", "In hushed whispers they ", "say Hergen, to this ",
                "day, wanders the ", "deserted halls of ", "Gerthland Manor hoping ", "that Madylina will ",
                "agree to his demands.",
            };
            Pages[ cnt++ ].Lines = lines;

            lines = new string[]
            {
                "And the lovers continue ", "to meet for a few ", "moments each night on ",
                "the shores of the lake ", "now known as Lover's ", "Lament.", "", "",
            };
            Pages[ cnt++ ].Lines = lines;
        }

        #region serialization
        public GerthlandBook( Serial serial )
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