using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DemoGame.DbObjs;
using log4net;
using NetGore;
using NetGore.World;
using SFML.Graphics;

namespace DemoGame.Server
{
    /// <summary>
    /// Uses the information from a <see cref="MapSpawnValues"/> to spawn non-persistent NPCs on a Map.
    /// </summary>
    public class NPCSpawner
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static readonly CharacterTemplateManager _characterTemplateManager = CharacterTemplateManager.Instance;
        static readonly SafeRandom _rnd = new SafeRandom();

        readonly byte _amount;
        readonly Rectangle _area;
        readonly CharacterTemplate _characterTemplate;
        readonly Map _map;

        NPCSpawnerNPC[] _npcs;

        /// <summary>
        /// Initializes a new instance of the <see cref="NPCSpawner"/> class.
        /// </summary>
        /// <param name="mapSpawnValues">The MapSpawnValues containing the values to use to create this NPCSpawner.</param>
        /// <param name="map">The Map instance to do the spawning on. The <see cref="MapID"/> of this Map must be equal to the
        /// <see cref="MapID"/> of the <paramref name="mapSpawnValues"/>.</param>
        /// <exception cref="ArgumentException">The <paramref name="map"/>'s <see cref="MapID"/> does not match the
        /// <paramref name="mapSpawnValues"/>'s <see cref="MapID"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="map" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="mapSpawnValues" /> is <c>null</c>.</exception>
        public NPCSpawner(IMapSpawnTable mapSpawnValues, Map map)
        {
            if (map == null)
                throw new ArgumentNullException("map");
            if (mapSpawnValues == null)
                throw new ArgumentNullException("mapSpawnValues");

            if (map.ID != mapSpawnValues.MapID)
                throw new ArgumentException("The map's MapID and mapSpawnValues's MapID do not match.", "map");

            _map = map;
            _characterTemplate = _characterTemplateManager[mapSpawnValues.CharacterTemplateID];
            _amount = mapSpawnValues.Amount;
            _area = new MapSpawnRect(mapSpawnValues).ToRectangle(map);

            if (_characterTemplate == null)
            {
                const string errmsg = "Failed to find the CharacterTemplate for CharacterTemplateID `{0}`.";
                var err = string.Format(errmsg, mapSpawnValues.CharacterTemplateID);
                if (log.IsFatalEnabled)
                    log.Fatal(err);
                Debug.Fail(err);
                throw new ArgumentException(err);
            }

            SpawnNPCs();
        }

        /// <summary>
        /// Gets the number of Characters being spawned by this <see cref="NPCSpawner"/>.
        /// </summary>
        public byte Amount
        {
            get { return _amount; }
        }

        /// <summary>
        /// Gets the area that the spawning takes place in.
        /// </summary>
        public Rectangle Area
        {
            get { return _area; }
        }

        /// <summary>
        /// Gets the <see cref="CharacterTemplate"/> used for the Characters spawned by this <see cref="NPCSpawner"/>.
        /// </summary>
        public CharacterTemplate CharacterTemplate
        {
            get { return _characterTemplate; }
        }

        /// <summary>
        /// Gets the Map that this NPCSpawner is on.
        /// </summary>
        public Map Map
        {
            get { return _map; }
        }

        /// <summary>
        /// Gets the <see cref="NPCs"/> that were spawned by this <see cref="NPCSpawner"/>.
        /// </summary>
        public IEnumerable<NPC> NPCs
        {
            get { return _npcs; }
        }

        /// <summary>
        /// Loads the NPCSpawners for a Map.
        /// </summary>
        /// <param name="map">Map to load the spawners for.</param>
        /// <returns>IEnumerable of the <see cref="NPCSpawner"/>s that were loaded.</returns>
        public static IEnumerable<NPCSpawner> LoadSpawners(Map map)
        {
            var queryValues = MapSpawnValues.Load(map.DbController, map.ID);
            var ret = new List<NPCSpawner>();

            foreach (var queryValue in queryValues)
            {
                var spawner = new NPCSpawner(queryValue, map);
                ret.Add(spawner);
            }

            return ret;
        }

        /// <summary>
        /// Gets a random position that fits inside the <see cref="NPCSpawner.Area"/>.
        /// </summary>
        /// <returns>A random position that fits inside the <see cref="NPCSpawner.Area"/>.</returns>
        public Vector2 RandomSpawnPosition()
        {
            var x = _rnd.Next(Area.Left, Area.Right + 1);
            var y = _rnd.Next(Area.Top, Area.Bottom + 1);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Spawns the NPCs for this <see cref="NPCSpawner"/>.
        /// </summary>
        void SpawnNPCs()
        {
            if (_npcs != null)
            {
                foreach (var npc in _npcs)
                {
                    npc.DelayedDispose();
                }
            }

            _npcs = new NPCSpawnerNPC[Amount];

            for (var i = 0; i < Amount; i++)
            {
                var pos = RandomSpawnPosition();
                var npc = new NPCSpawnerNPC(this, _map.World, _characterTemplate, _map, pos);
                _npcs[i] = npc;
            }
        }

        /// <summary>
        /// An <see cref="NPC"/> that is created from an <see cref="NPCSpawner"/>.
        /// </summary>
        class NPCSpawnerNPC : NPC
        {
            /// <summary>
            /// The minimum amount of time to wait, in milliseconds, before attempting to find a new legal position for a
            /// <see cref="NPCSpawnerNPC"/>.
            /// </summary>
            const int _findNewPositionTimeout = 1000;

            readonly NPCSpawner _spawner;

            /// <summary>
            /// The <see cref="TickCount"/> at which we last requested a new respawn position for this <see cref="NPCSpawnerNPC"/>.
            /// Will only attempt a new position after <see cref="_findNewPositionTimeout"/> milliseconds have elapsed.
            /// </summary>
            TickCount _reqNewPositionStartTime = TickCount.MinValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="NPCSpawnerNPC"/> class.
            /// </summary>
            /// <param name="spawner">The spawner.</param>
            /// <param name="parent">World that the NPC belongs to.</param>
            /// <param name="template">NPCTemplate used to create the NPC.</param>
            /// <param name="map">The map.</param>
            /// <param name="position">The position.</param>
            /// <exception cref="ArgumentNullException"><paramref name="spawner" /> is <c>null</c>.</exception>
            public NPCSpawnerNPC(NPCSpawner spawner, World parent, CharacterTemplate template, Map map, Vector2 position)
                : base(parent, template, map, position)
            {
                if (spawner == null)
                    throw new ArgumentNullException("spawner");

                _spawner = spawner;
            }

            /// <summary>
            /// Handles when no legal position could be found for this <see cref="Character"/>.
            /// This will usually occur when performing a teleport into an area that is completely blocked off, and no near-by
            /// position can be found. Moving a <see cref="Character"/> too far from the original position can result in them
            /// going somewhere that they are not supposed to, so it is best to send them to a predefined location.
            /// </summary>
            /// <param name="position">The position that the <see cref="Character"/> tried to go to, but failed to.</param>
            /// <returns>
            /// The position to warp the <see cref="Character"/> to.
            /// </returns>
            protected override Vector2 HandleNoLegalPositionFound(Vector2 position)
            {
                // Set to not alive, but do NOT actually kill them!
                IsAlive = false;

                if (log.IsDebugEnabled)
                    log.DebugFormat("Failed to find legal spawn position for NPC `{0}`. Will attempt again next update.", this);

                // Only set the time if it was not set already, otherwise we just might end up pushing the delay time up constantly
                // and making it so they never try to actually respawn
                if (_reqNewPositionStartTime == TickCount.MinValue)
                    _reqNewPositionStartTime = TickCount.Now;

                return position;
            }

            /// <summary>
            /// Handles updating this <see cref="Entity"/>.
            /// </summary>
            /// <param name="imap">The map the <see cref="Entity"/> is on.</param>
            /// <param name="deltaTime">The amount of time (in milliseconds) that has elapsed since the last update.</param>
            protected override void HandleUpdate(IMap imap, int deltaTime)
            {
                // Check if they have requested a new spawn position
                if (_reqNewPositionStartTime != TickCount.MinValue)
                {
                    // Check if enough time has elapsed to try and find the new position
                    if (TickCount.Now - _reqNewPositionStartTime > _findNewPositionTimeout)
                    {
                        _reqNewPositionStartTime = TickCount.MinValue;

                        RespawnMapID = _spawner.Map.ID;
                        RespawnPosition = _spawner.RandomSpawnPosition();

                        Teleport(_spawner.Map, RespawnPosition);

                        // Check that our attempt to teleport actually succeeded, not just put them at another invalid position
                        if (_reqNewPositionStartTime != TickCount.MinValue)
                            return;
                    }
                    else
                    {
                        // Not enough time has elapsed...
                        return;
                    }

                    // They managed to teleport to a valid position - bring to life and clear the timer
                    _reqNewPositionStartTime = TickCount.MinValue;
                    IsAlive = true;

                    EventCounterManager.Map.Increment(_spawner.Map.ID, MapEventCounterType.NPCSpawned);
                }

                base.HandleUpdate(imap, deltaTime);
            }

            /// <summary>
            /// When overridden in the derived class, allows for additional handling of the
            /// <see cref="Character.Killed"/> event. It is recommended you override this method instead of
            /// using the corresponding event when possible.
            /// </summary>
            protected override void OnKilled()
            {
                EventCounterManager.Map.Increment(_spawner.Map.ID, MapEventCounterType.SpawnedNPCKilled);

                var respawnPos = _spawner.RandomSpawnPosition();

                RespawnMapID = _spawner.Map.ID;
                RespawnPosition = respawnPos;

                base.OnKilled();
            }
        }
    }
}