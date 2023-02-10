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
    }
}