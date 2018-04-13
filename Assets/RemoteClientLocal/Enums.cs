namespace DF.Enums
{
    public enum building_type
    {
        NONE = -1,
        Chair,
        Bed,
        Table,
        Coffin,
        FarmPlot,
        Furnace,
        TradeDepot,
        Shop,
        Door,
        Floodgate,
        Box,
        Weaponrack,
        Armorstand,
        Workshop,
        Cabinet,
        Statue,
        WindowGlass,
        WindowGem,
        Well,
        Bridge,
        RoadDirt,
        RoadPaved,
        SiegeEngine,
        Trap,
        AnimalTrap,
        Support,
        ArcheryTarget,
        Chain,
        Cage,
        Stockpile,
        Civzone,
        Weapon,
        Wagon,
        ScrewPump,
        Construction,
        Hatch,
        GrateWall,
        GrateFloor,
        BarsVertical,
        BarsFloor,
        GearAssembly,
        AxleHorizontal,
        AxleVertical,
        WaterWheel,
        Windmill,
        TractionBench,
        Slab,
        Nest,
        NestBox,
        Hive,
        Rollers,
        Instrument,
        Bookcase
    }

    public enum profession
    {
        NONE = -1,
        MINER,
        WOODWORKER,
        CARPENTER,
        BOWYER,
        WOODCUTTER,
        STONEWORKER,
        ENGRAVER,
        MASON,
        RANGER,
        ANIMAL_CARETAKER,
        ANIMAL_TRAINER,
        HUNTER,
        TRAPPER,
        ANIMAL_DISSECTOR,
        METALSMITH,
        FURNACE_OPERATOR,
        WEAPONSMITH,
        ARMORER,
        BLACKSMITH,
        METALCRAFTER,
        JEWELER,
        GEM_CUTTER,
        GEM_SETTER,
        CRAFTSMAN,
        WOODCRAFTER,
        STONECRAFTER,
        LEATHERWORKER,
        BONE_CARVER,
        WEAVER,
        CLOTHIER,
        GLASSMAKER,
        POTTER,
        GLAZER,
        WAX_WORKER,
        STRAND_EXTRACTOR,
        FISHERY_WORKER,
        FISHERMAN,
        FISH_DISSECTOR,
        FISH_CLEANER,
        FARMER,
        CHEESE_MAKER,
        MILKER,
        COOK,
        THRESHER,
        MILLER,
        BUTCHER,
        TANNER,
        DYER,
        PLANTER,
        HERBALIST,
        BREWER,
        SOAP_MAKER,
        POTASH_MAKER,
        LYE_MAKER,
        WOOD_BURNER,
        SHEARER,
        SPINNER,
        PRESSER,
        BEEKEEPER,
        ENGINEER,
        MECHANIC,
        SIEGE_ENGINEER,
        SIEGE_OPERATOR,
        PUMP_OPERATOR,
        CLERK,
        ADMINISTRATOR,
        TRADER,
        ARCHITECT,
        ALCHEMIST,
        DOCTOR,
        DIAGNOSER,
        BONE_SETTER,
        SUTURER,
        SURGEON,
        MERCHANT,
        HAMMERMAN,
        MASTER_HAMMERMAN,
        SPEARMAN,
        MASTER_SPEARMAN,
        CROSSBOWMAN,
        MASTER_CROSSBOWMAN,
        WRESTLER,
        MASTER_WRESTLER,
        AXEMAN,
        MASTER_AXEMAN,
        SWORDSMAN,
        MASTER_SWORDSMAN,
        MACEMAN,
        MASTER_MACEMAN,
        PIKEMAN,
        MASTER_PIKEMAN,
        BOWMAN,
        MASTER_BOWMAN,
        BLOWGUNMAN,
        MASTER_BLOWGUNMAN,
        LASHER,
        MASTER_LASHER,
        RECRUIT,
        TRAINED_HUNTER,
        TRAINED_WAR,
        MASTER_THIEF,
        THIEF,
        STANDARD,
        CHILD,
        BABY,
        DRUNK,
        MONSTER_SLAYER,
        SCOUT,
        BEAST_HUNTER,
        SNATCHER,
        MERCENARY,
        GELDER,
        PERFORMER,
        POET,
        BARD,
        DANCER,
        SAGE,
        SCHOLAR,
        PHILOSOPHER,
        MATHEMATICIAN,
        HISTORIAN,
        ASTRONOMER,
        NATURALIST,
        CHEMIST,
        GEOGRAPHER,
        SCRIBE,
        PAPERMAKER,
        BOOKBINDER,
        TAVERN_KEEPER
    }

    enum item_type
    {
        NONE = -1,
        /**
         * Bars, such as metal, fuel, or soap.
         */
        BAR,
        /**
         * Cut gemstones usable in jewelers workshop
         */
        SMALLGEM,
        /**
         * Blocks of any kind.
         */
        BLOCKS,
        /**
         * Rough gemstones.
         */
        ROUGH,
        /**
         * Raw mined stone.
         */
        BOULDER,
        /**
         * Wooden logs.
         */
        WOOD,
        /**
         * Doors.
         */
        DOOR,
        /**
         * Floodgates.
         */
        FLOODGATE,
        /**
         * Beds.
         */
        BED,
        /**
         * Chairs and thrones.
         */
        CHAIR,
        /**
         * Restraints.
         */
        CHAIN,
        /**
         * Flasks.
         */
        FLASK,
        /**
         * Goblets.
         */
        GOBLET,
        /**
         * Musical instruments.
         */
        INSTRUMENT,
        /**
         * Toys.
         */
        TOY,
        /**
         * Glass windows.
         */
        WINDOW,
        /**
         * Cages.
         */
        CAGE,
        /**
         * Barrels.
         */
        BARREL,
        /**
         * Buckets.
         */
        BUCKET,
        /**
         * Animal traps.
         */
        ANIMALTRAP,
        /**
         * Tables.
         */
        TABLE,
        /**
         * Coffins.
         */
        COFFIN,
        /**
         * Statues.
         */
        STATUE,
        /**
         * Corpses. Does not have a material.
         */
        CORPSE,
        /**
         * Weapons.
         */
        WEAPON,
        /**
         * Armor and clothing worn on the upper body.
         */
        ARMOR,
        /**
         * Armor and clothing worn on the feet.
         */
        SHOES,
        /**
         * Shields and bucklers.
         */
        SHIELD,
        /**
         * Armor and clothing worn on the head.
         */
        HELM,
        /**
         * Armor and clothing worn on the hands.
         */
        GLOVES,
        /**
         * Chests (wood), coffers (stone), boxes (glass), and bags (cloth or leather).
         */
        BOX,
        /**
         * Bins.
         */
        BIN,
        /**
         * Armor stands.
         */
        ARMORSTAND,
        /**
         * Weapon racks.
         */
        WEAPONRACK,
        /**
         * Cabinets.
         */
        CABINET,
        /**
         * Figurines.
         */
        FIGURINE,
        /**
         * Amulets.
         */
        AMULET,
        /**
         * Scepters.
         */
        SCEPTER,
        /**
         * Ammunition for hand-held weapons.
         */
        AMMO,
        /**
         * Crowns.
         */
        CROWN,
        /**
         * Rings.
         */
        RING,
        /**
         * Earrings.
         */
        EARRING,
        /**
         * Bracelets.
         */
        BRACELET,
        /**
         * Large gems.
         */
        GEM,
        /**
         * Anvils.
         */
        ANVIL,
        /**
         * Body parts. Does not have a material.
         */
        CORPSEPIECE,
        /**
         * Dead vermin bodies. Material is CREATURE_ID:CASTE.
         */
        REMAINS,
        /**
         * Butchered meat.
         */
        MEAT,
        /**
         * Prepared fish. Material is CREATURE_ID:CASTE.
         */
        FISH,
        /**
         * Unprepared fish. Material is CREATURE_ID:CASTE.
         */
        FISH_RAW,
        /**
         * Live vermin. Material is CREATURE_ID:CASTE.
         */
        VERMIN,
        /**
         * Tame vermin. Material is CREATURE_ID:CASTE.
         */
        PET,
        /**
         * Seeds from plants.
         */
        SEEDS,
        /**
         * Plants.
         */
        PLANT,
        /**
         * Tanned skins.
         */
        SKIN_TANNED,
        /**
         * Assorted plant growths, including leaves and berries
         */
        PLANT_GROWTH,
        /**
         * Thread gathered from webs or made at the farmers workshop.
         */
        THREAD,
        /**
         * Cloth made at the loom.
         */
        CLOTH,
        /**
         * Skull totems.
         */
        TOTEM,
        /**
         * Armor and clothing worn on the legs.
         */
        PANTS,
        /**
         * Backpacks.
         */
        BACKPACK,
        /**
         * Quivers.
         */
        QUIVER,
        /**
         * Catapult parts.
         */
        CATAPULTPARTS,
        /**
         * Ballista parts.
         */
        BALLISTAPARTS,
        /**
         * Siege engine ammunition.
         */
        SIEGEAMMO,
        /**
         * Ballista arrow heads.
         */
        BALLISTAARROWHEAD,
        /**
         * Mechanisms.
         */
        TRAPPARTS,
        /**
         * Trap components.
         */
        TRAPCOMP,
        /**
         * Alcoholic drinks.
         */
        DRINK,
        /**
         * Powders such as flour, gypsum plaster, dye, or sand.
         */
        POWDER_MISC,
        /**
         * Pieces of cheese.
         */
        CHEESE,
        /**
         * Prepared meals. Subtypes come from item_food.txt
         */
        FOOD,
        /**
         * Liquids such as water, lye, and extracts.
         */
        LIQUID_MISC,
        /**
         * Coins.
         */
        COIN,
        /**
         * Fat, tallow, pastes/pressed objects, and small bits of molten rock/metal.
         */
        GLOB,
        /**
         * Small rocks (usually sharpened and/or thrown in adventurer mode)
         */
        ROCK,
        /**
         * Pipe sections.
         */
        PIPE_SECTION,
        /**
         * Hatch covers.
         */
        HATCH_COVER,
        /**
         * Grates.
         */
        GRATE,
        /**
         * Querns.
         */
        QUERN,
        /**
         * Millstones.
         */
        MILLSTONE,
        /**
         * Splints.
         */
        SPLINT,
        /**
         * Crutches.
         */
        CRUTCH,
        /**
         * Traction benches.
         */
        TRACTION_BENCH,
        /**
         * Casts
         */
        ORTHOPEDIC_CAST,
        /**
         * Tools.
         */
        TOOL,
        /**
         * Slabs.
         */
        SLAB,
        /**
         * Eggs. Material is CREATURE_ID:CASTE.
         */
        EGG,
        /**
         * Books.
         */
        BOOK,
        /**
         * Sheets of paper
         */
        SHEET,
        /**
         * Tree branches
         */
        BRANCH
    }
}
