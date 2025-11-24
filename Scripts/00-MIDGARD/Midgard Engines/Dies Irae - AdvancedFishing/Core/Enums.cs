/***************************************************************************
 *                               Enums.cs
 *
 *   begin                : 19 settembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

namespace Midgard.Engines.AdvancedFishing
{
    public enum Actions
    {
        None = 0,

        Jump,
        War,
        Down
    }

    public enum Stages
    {
        Quiet = 0,

        FishCatched,
        FishAction,
        FisherReaction
    }

    public enum EndFishResults
    {
        Undefined = 0,

        SuccessedMove,
        WrongMove,
        Fished,
        BadReflex,
        NoMoreFishes,
        MoreStrength,
        FisherMoved,
        UnluckyContest,
        WrongMoveOnCatched
    }

    public enum FishHabitat
    {
        None = 0,

        Sea,
        River,
        Lake
    }
}