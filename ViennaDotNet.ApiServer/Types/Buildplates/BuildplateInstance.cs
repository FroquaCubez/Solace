using static ViennaDotNet.ApiServer.Types.Buildplates.BuildplateInstance;
using System.Collections.Concurrent;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ViennaDotNet.ApiServer.Types.Common;

namespace ViennaDotNet.ApiServer.Types.Buildplates
{
    // TODO: actually implement proper snapshot and shutdown behavior in the buildplate server
    public record BuildplateInstance(
        string instanceId,
        string partitionId,
        string fqdn,
        string ipV4Address,
        int port,
        bool serverReady,
        ApplicationStatus applicationStatus,
        ServerStatus serverStatus,
        string metadata,
        GameplayMetadata gameplayMetadata,
        string roleInstance,    // TODO: find out what this is
        Coordinate hostCoordinate
    )
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ApplicationStatus
        {
            [EnumMember(Value = "Unknown")] UNKNOWN,
            [EnumMember(Value = "Ready")] READY
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ServerStatus
        {
            [EnumMember(Value = "Running")] RUNNING
        }

        public record GameplayMetadata(
            string worldId,
            string templateId,
            string spawningPlayerId,
            string spawningClientBuildNumber,
            string playerJoinCode,
            Dimension dimension,
            Offset offset,
            int blocksPerMeter,
            bool isFullSize,
            GameplayMetadata.GameplayMode gameplayMode,
            SurfaceOrientation surfaceOrientation,
            string? augmentedImageSetId,
            Rarity? rarity,
            GameplayMetadata.ShutdownBehavior[] shutdownBehavior,
            GameplayMetadata.SnapshotOptions snapshotOptions,
            Dictionary<string, object> breakableItemToItemLootMap    // TODO: find out what this is
        )
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum GameplayMode
            {
                [EnumMember(Value = "Buildplate")] BUILDPLATE,
                [EnumMember(Value = "Encounter")] ENCOUNTER
            }

            [JsonConverter(typeof(StringEnumConverter))]
            public enum ShutdownBehavior
            {
                [EnumMember(Value = "ServerShutdownWhenAllPlayersQuit")] ALL_PLAYERS_QUIT,
                [EnumMember(Value = "ServerShutdownWhenHostPlayerQuits")] HOST_PLAYER_QUITS
            }

            public record SnapshotOptions(
                SnapshotOptions.SnapshotWorldStorage snapshotWorldStorage,
                SnapshotOptions.SaveState saveState,
                SnapshotOptions.SnapshotTriggerConditions snapshotTriggerConditions,
                SnapshotOptions.TriggerCondition[] triggerConditions,
                string triggerInterval
            )
            {
                [JsonConverter(typeof(StringEnumConverter))]
                public enum SnapshotWorldStorage
                {
                    [EnumMember(Value = "Buildplate")] BUILDPLATE
                }

                public record SaveState(
                    bool boosts,
                    bool experiencePoints,
                    bool health,
                    bool inventory,
                    bool model,
                    bool world
                )
                {
                }

                [JsonConverter(typeof(StringEnumConverter))]
                public enum SnapshotTriggerConditions
                {
                    [EnumMember(Value = "None")] NONE
                }

                [JsonConverter(typeof(StringEnumConverter))]
                public enum TriggerCondition
                {
                    [EnumMember(Value = "Interval")] INTERVAL,
                    [EnumMember(Value = "PlayerExits")] PLAYER_EXITS
                }
            }
        }
    }
}
