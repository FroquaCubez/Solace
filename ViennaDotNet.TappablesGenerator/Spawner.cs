using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ViennaDotNet.Common.Utils;
using ViennaDotNet.EventBus.Client;

namespace ViennaDotNet.TappablesGenerator
{
    public class Spawner
    {
        private static readonly long SPAWN_INTERVAL = 15 * 1000;

        private readonly ActiveTiles activeTiles;
        private readonly Generator generator;
        private readonly Publisher publisher;

        private readonly int maxTappableLifetimeIntervals;

        private long spawnCycleTime;
        private int spawnCycleIndex;
        private readonly Dictionary<int, int> lastSpawnCycleForTile = new();

        public Spawner(EventBusClient eventBusClient, ActiveTiles activeTiles, Generator generator)
        {
            this.activeTiles = activeTiles;
            this.generator = generator;
            this.publisher = eventBusClient.addPublisher();

            this.maxTappableLifetimeIntervals = (int)(this.generator.getMaxTappableLifetime() / SPAWN_INTERVAL + 1);

            this.spawnCycleTime = U.CurrentTimeMillis();
            this.spawnCycleIndex = maxTappableLifetimeIntervals;
        }

        public void run()
        {
            long nextTime = U.CurrentTimeMillis() + SPAWN_INTERVAL;
            for (; ; )
            {
                try
                {
                    Thread.Sleep(Math.Max(0, (int)(nextTime - U.CurrentTimeMillis())));
                }
                catch (ThreadAbortException)
                {
                    Log.Information("Spawn thread was interrupted, exiting");
                    break;
                }
                nextTime += SPAWN_INTERVAL;

                doSpawnCycle();
            }
        }

        public void spawnTile(int tileX, int tileY)
        {
            Log.Information($"Spawning tappables for tile {tileX},{tileY}");

            long spawnCycleTime = this.spawnCycleTime;
            int spawnCycleIndex = this.spawnCycleIndex;

            while (spawnCycleTime < U.CurrentTimeMillis())
            {
                spawnCycleTime += SPAWN_INTERVAL;
                spawnCycleIndex++;
            }

            doSpawnCyclesForTile(tileX, tileY, spawnCycleTime, spawnCycleIndex);
        }

        private void doSpawnCycle()
        {
            ActiveTiles.ActiveTile[] activeTiles = this.activeTiles.getActiveTiles(spawnCycleTime);

            Log.Information($"Spawning tappables for {activeTiles.Length} tiles");

            while (spawnCycleTime < U.CurrentTimeMillis())
            {
                spawnCycleTime += SPAWN_INTERVAL;
                spawnCycleIndex++;
            }

            foreach (ActiveTiles.ActiveTile activeTile in activeTiles)
                doSpawnCyclesForTile(activeTile.tileX, activeTile.tileY, spawnCycleTime, spawnCycleIndex);
        }

        private void doSpawnCyclesForTile(int tileX, int tileY, long spawnCycleTime, int spawnCycleIndex)
        {
            int lastSpawnCycle = lastSpawnCycleForTile.GetOrDefault((tileX << 16) + tileY, 0);
            int cyclesToSpawn = Math.Min(spawnCycleIndex - lastSpawnCycle, maxTappableLifetimeIntervals);
            for (int index = 0; index < cyclesToSpawn; index++)
                spawnTappablesForTile(tileX, tileY, spawnCycleTime - SPAWN_INTERVAL * (cyclesToSpawn - index - 1));

            lastSpawnCycleForTile[(tileX << 16) + tileY] = spawnCycleIndex;
        }

        private void spawnTappablesForTile(int tileX, int tileY, long currentTime)
        {
            foreach (Tappable tappable in generator.generateTappables(tileX, tileY, currentTime))
            {
                if (!publisher.publish("tappables", "tappableSpawn", JsonConvert.SerializeObject(tappable)).Result)
                    Log.Error("Event bus server rejected tappable spawn event");
            }
        }
    }
}
