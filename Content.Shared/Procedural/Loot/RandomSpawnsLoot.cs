using Content.Shared.Random;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Procedural.Loot;

/// <summary>
/// Randomly places loot in free areas inside the dungeon.
/// </summary>
public sealed partial class RandomSpawnsLoot : IDungeonLoot
{
    [ViewVariables(VVAccess.ReadWrite), DataField("entries", required: true)]
    public List<RandomSpawnLootEntry> Entries = new();
}

[DataDefinition]
public partial record struct RandomSpawnLootEntry : IBudgetEntry
{
    public RandomSpawnLootEntry(string proto, float cost, float prob) //Imperial edit start
    {
        Proto = proto;
        Cost = cost;
        Prob = prob;
    } //imperial edit end

    [ViewVariables(VVAccess.ReadWrite), DataField("proto", required: true, customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Proto { get; set; } = string.Empty;
    
    [ViewVariables(VVAccess.ReadWrite), DataField("cost")]
    public float Cost { get; set; } = 1f;

    [ViewVariables(VVAccess.ReadWrite), DataField("prob")]
    public float Prob { get; set; } = 1f;
}
