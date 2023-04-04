using System;

namespace FmvMaker.Graph {
    [Serializable]
    public enum FmvVideoEnum {
        None,
        UniqueVideoName,
        NextUniqueVideoName,
        AnotherUniqueVideoName,
        DifferentUniqueVideoName,
        UniqueToDifferentVideoName,
        UniqueIdleVideoName,
        DialogIntro,
        DialogIdle,
        DialogOption1,
        DialogOption2,
        DialogOption3,
        DialogOption4,
        DialogOption5,
    }
}