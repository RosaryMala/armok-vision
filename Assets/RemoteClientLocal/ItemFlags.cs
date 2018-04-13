using System;

namespace DF.Flags
{
    [Flags]
    enum ItemFlags : UInt32
    {
        on_ground = 0x1U,
        in_job = 0x2U,
        hostile = 0x4U,
        in_inventory = 0x8U,
        removed = 0x10U,
        in_building = 0x20U,
        container = 0x40U,
        dead_dwarf = 0x80U,
        rotten = 0x100U,
        spider_web = 0x200U,
        construction = 0x400U,
        encased = 0x800U,
        unk12 = 0x1000U,
        murder = 0x2000U,
        foreign = 0x4000U,
        trader = 0x8000U,
        owned = 0x10000U,
        garbage_collect = 0x20000U,
        artifact = 0x40000U,
        forbid = 0x80000U,
        already_uncategorized = 0x100000U,
        dump = 0x200000U,
        on_fire = 0x400000U,
        melt = 0x800000U,
        hidden = 0x1000000U,
        in_chest = 0x2000000U,
        use_recorded = 0x4000000U,
        artifact_mood = 0x8000000U,
        temps_computed = 0x10000000U,
        weight_computed = 0x20000000U,
        unk30 = 0x40000000U,
        from_worldgen = 0x80000000U
    }

    [Flags]
    enum ItemFlags2 : UInt32
    {
        has_rider = 0x1U,
        unk1 = 0x2U,
        grown = 0x4U,
        has_written_content = 0x8U
    }
}