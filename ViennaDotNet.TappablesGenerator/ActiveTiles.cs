using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViennaDotNet.Common.Utils;
using ViennaDotNet.EventBus.Client;
using ViennaDotNet.TappablesGenerator;

namespace ViennaDotNet.TappablesGenerator
{
    public class ActiveTiles
    {
        private static readonly int ACTIVE_TILE_RADIUS = 3;
        private static readonly long ACTIVE_TILE_EXPIRY_TIME = 2 * 60 * 1000;

        private readonly Dictionary<int, ActiveTile> activeTiles = new();
        private readonly IActiveTileListener activeTileListener;

        public ActiveTiles(EventBusClient eventBusClient, IActiveTileListener activeTileListener)
        {
            this.activeTileListener = activeTileListener;

            eventBusClient.addRequestHandler("tappables", new RequestHandler.Handler(request =>
            {
                if (request.type == "activeTile")
                {
                    ActiveTileNotification activeTileNotification;
                    try
                    {
                        activeTileNotification = JsonConvert.DeserializeObject<ActiveTileNotification>(request.data)!;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Could not deserialise active tile notification event: {ex}");
                        return null;
                    }

                    long currentTime = U.CurrentTimeMillis();
                    pruneActiveTiles(currentTime);
                    for (int tileX = activeTileNotification.x - ACTIVE_TILE_RADIUS; tileX < activeTileNotification.x + ACTIVE_TILE_RADIUS + 1; tileX++)
                    {
                        for (int tileY = activeTileNotification.y - ACTIVE_TILE_RADIUS; tileY < activeTileNotification.y + ACTIVE_TILE_RADIUS + 1; tileY++)
                            markTileActive(tileX, tileY, currentTime);
                    }

                    return string.Empty;
                }
                else
                    return null;
            }, () =>
            {
                Log.Error("Event bus subscriber error");
                Environment.Exit(1);
            }));
        }

        public ActiveTile[] getActiveTiles(long currentTime)
        {
            return activeTiles.Values.Where(activeTile => currentTime < activeTile.latestActiveTime + ACTIVE_TILE_EXPIRY_TIME).ToArray();
        }

        private void markTileActive(int tileX, int tileY, long currentTime)
        {
            ActiveTile? activeTile = activeTiles.GetOrDefault((tileX << 16) + tileY, null);
            if (activeTile == null)
            {
                Log.Information($"Tile {tileX},{tileY} is becoming active");
                activeTile = new ActiveTile(tileX, tileY, currentTime, currentTime);
                activeTileListener.active(activeTile);
            }
            else
                activeTile = new ActiveTile(tileX, tileY, activeTile.firstActiveTime, currentTime);

            activeTiles[(tileX << 16) + tileY] = activeTile;
        }

        private void pruneActiveTiles(long currentTime)
        {
            List<int> entriesToRemove = new();

            foreach (var entry in activeTiles)
            {
                ActiveTile activeTile = entry.Value;
                if (activeTile.latestActiveTime + ACTIVE_TILE_EXPIRY_TIME <= currentTime)
                {
                    Log.Information($"Tile {activeTile.tileX},{activeTile.tileY} is inactive");
                    entriesToRemove.Add(entry.Key);
                }
            }

            foreach (var key in entriesToRemove)
            {
                ActiveTile activeTile = activeTiles.JavaRemove(key)!;
                activeTileListener.inactive(activeTile);
            }
        }

        public record ActiveTile(
            int tileX,
            int tileY,
            long firstActiveTime,
            long latestActiveTime
        )
        {
        }

        record ActiveTileNotification(
            int x,
            int y,
            string playerId
        )
        {
        }

        public interface IActiveTileListener
        {
            void active(ActiveTile activeTile);

            void inactive(ActiveTile activeTile);
        }

        public class ActiveTileListener : IActiveTileListener
        {
            public Action<ActiveTile>? Active;
            public Action<ActiveTile>? Inactive;

            public ActiveTileListener(Action<ActiveTile>? _active, Action<ActiveTile>? _inactive)
            {
                Active = _active;
                Inactive = _inactive;
            }

            public void active(ActiveTile activeTile)
                => Active?.Invoke(activeTile);

            public void inactive(ActiveTile activeTile)
                => Inactive?.Invoke(activeTile);
        }
    }
}
