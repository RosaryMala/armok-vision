using System;

namespace UnitFlags
{
    [Flags]
    enum UnitFlags1 : uint
    {
        move_state = 0x1U,
        dead = 0x2U,
        has_mood = 0x4U,
        had_mood = 0x8U,
        marauder = 0x10U,
        drowning = 0x20U,
        merchant = 0x40U,
        forest = 0x80U,
        left = 0x100U,
        rider = 0x200U,
        incoming = 0x400U,
        diplomat = 0x800U,
        zombie = 0x1000U,
        skeleton = 0x2000U,
        can_swap = 0x4000U,
        on_ground = 0x8000U,
        projectile = 0x10000U,
        active_invader = 0x20000U,
        hidden_in_ambush = 0x40000U,
        invader_origin = 0x80000U,
        coward = 0x100000U,
        hidden_ambusher = 0x200000U,
        invades = 0x400000U,
        check_flows = 0x800000U,
        ridden = 0x1000000U,
        caged = 0x2000000U,
        tame = 0x4000000U,
        chained = 0x8000000U,
        royal_guard = 0x10000000U,
        fortress_guard = 0x20000000U,
        suppress_wield = 0x40000000U,
        important_historical_figure = 0x80000000U
    }

    [Flags]
    enum UnitFlags2 : uint
    {
        swimming = 0x1U,
        sparring = 0x2U,
        no_notify = 0x4U,
        unused = 0x8U,
        calculated_nerves = 0x10U,
        calculated_bodyparts = 0x20U,
        important_historical_figure = 0x40U,
        killed = 0x80U,
        cleanup_1 = 0x100U,
        cleanup_2 = 0x200U,
        cleanup_3 = 0x400U,
        for_trade = 0x800U,
        trade_resolved = 0x1000U,
        has_breaks = 0x2000U,
        gutted = 0x4000U,
        circulatory_spray = 0x8000U,
        locked_in_for_trading = 0x10000U,
        slaughter = 0x20000U,
        underworld = 0x40000U,
        resident = 0x80000U,
        cleanup_4 = 0x100000U,
        calculated_insulation = 0x200000U,
        visitor_uninvited = 0x400000U,
        visitor = 0x800000U,
        calculated_inventory = 0x1000000U,
        vision_good = 0x2000000U,
        vision_damaged = 0x4000000U,
        vision_missing = 0x8000000U,
        breathing_good = 0x10000000U,
        breathing_problem = 0x20000000U,
        roaming_wilderness_population_source = 0x40000000U,
        roaming_wilderness_population_source_not_a_map_feature = 0x80000000U
    }

    [Flags]
    enum UnitFlags3 : uint
    {
        body_part_relsize_computed = 0x1U,
        size_modifier_computed = 0x2U,
        stuck_weapon_computed = 0x4U,
        compute_health = 0x8U,
        announce_titan = 0x10U,
        unk5 = 0x20U,
        on_crutch = 0x40U,
        weight_computed = 0x80U,
        body_temp_in_range = 0x100U,
        wait_until_reveal = 0x200U,
        scuttle = 0x400U,
        unk11 = 0x800U,
        ghostly = 0x1000U,
        unk13 = 0x2000U,
        unk14 = 0x4000U,
        unk15 = 0x8000U,
        unk16 = 0x10000U,
        no_meandering = 0x20000U,
        floundering = 0x40000U,
        exit_vehicle1 = 0x80000U,
        exit_vehicle2 = 0x100000U,
        dangerous_terrain = 0x200000U,
        adv_yield = 0x400000U,
        unk23 = 0x800000U,
        unk24 = 0x1000000U,
        emotionally_overloaded = 0x2000000U,
        unk26 = 0x4000000U,
        unk27 = 0x8000000U,
        gelded = 0x10000000U
    }
}
