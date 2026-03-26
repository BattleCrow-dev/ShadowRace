using System;
using System.Collections.Generic;
using UnityEngine;
using YG;


[Serializable]
public struct GhostFrame
{
    public float time;
    public float x;
    public float y;
    public float rot;
}

[Serializable]
public class GhostLap
{
    public int trackIndex;

    public int skinIndex;
    public int colorIndex;

    public float lapTime;

    public GhostFrame[] frames;
}

public class GhostsManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform playerCar;
    [SerializeField] private GameObject ghostPrefab;

    [Header("Parameters")]
    [SerializeField] private int ghostsToSpawn = 2;
    [SerializeField] private float sampleRate = 0.08f;
    [SerializeField] private int trackIndex;
    [SerializeField] private int skinIndex;
    [SerializeField] private int colorIndex;

    private float timer;
    private float nextSample;
    private bool recording;

    private List<GhostFrame> frames = new();
    private List<Session> sessions = new();
    private List<GhostReplay> activeGhosts = new();

    public void StartGame()
    {
        YG2.MultiplayerSessions.onSessionsLoaded += OnSessionsLoaded;

        InitConfig config = new()
        {
            count = 20,
            meta = new MetaFilter()
        };

        config.meta.meta1 = new YG.Range
        {
            min = 0,
            max = 300
        };

        YG2.MultiplayerSessions.Init(config);
    }

    private void Update()
    {
        if (!recording)
            return;

        timer += Time.deltaTime;

        if (timer >= nextSample)
        {
            nextSample += sampleRate;

            frames.Add(new GhostFrame
            {
                time = timer,
                x = playerCar.position.x,
                y = playerCar.position.y,
                rot = playerCar.eulerAngles.z
            });
        }
    }

    public void StartLap()
    {
        frames.Clear();
        timer = 0;
        nextSample = 0;
        recording = true;

        SpawnGhosts();
    }

    public void FinishLap()
    {
        recording = false;

        GhostLap lap = new()
        {
            trackIndex = trackIndex,
            skinIndex = skinIndex,
            colorIndex = colorIndex,
            lapTime = timer,
            frames = frames.ToArray()
        };

        string json = JsonUtility.ToJson(lap);

        Payload payload = new() { ghostLap = json };
        YG2.MultiplayerSessions.Commit(payload);

        Meta meta = new()
        {
            meta1 = (long) timer
        };

        YG2.MultiplayerSessions.Push(meta);
    }

    private void OnSessionsLoaded(List<Session> loadedSessions)
    {
        sessions = loadedSessions;

        Debug.Log($"[MP] Загружено сессий: {sessions.Count}");

        for (int i = 0; i < sessions.Count; i++)
        {
            var s = sessions[i];

            string playerName = s.player != null ? s.player.name : "unknown";
            int timelineCount = s.timeline != null ? s.timeline.Count : 0;

            Debug.Log($"[MP] Сессия {i}: playerName={playerName}, записей={timelineCount}");

            if (s.timeline != null && s.timeline.Count > 0)
            {
                var payload = s.timeline[s.timeline.Count - 1].payload;

                if (!string.IsNullOrEmpty(payload.ghostLap))
                {
                    Debug.Log($"[MP] Есть ghostLap, размер={payload.ghostLap.Length} символов");
                }
                else
                {
                    Debug.Log("[MP] ghostLap пустой");
                }
            }
        }
    }

    private void SpawnGhosts()
    {
        foreach (var g in activeGhosts)
            Destroy(g.gameObject);

        activeGhosts.Clear();

        if (sessions == null || sessions.Count == 0)
            return;

        List<int> usedIndexes = new();
        int attempts = 0;

        while (activeGhosts.Count < ghostsToSpawn && attempts < 20)
        {
            attempts++;

            int random = UnityEngine.Random.Range(0, sessions.Count);

            if (usedIndexes.Contains(random))
                continue;

            usedIndexes.Add(random);

            var timeline = sessions[random].timeline;
            if (timeline == null || timeline.Count == 0)
                continue;

            string json = timeline[timeline.Count - 1].payload.ghostLap;
            if (string.IsNullOrEmpty(json))
                continue;

            GhostLap lap = JsonUtility.FromJson<GhostLap>(json);

            if (lap.trackIndex != trackIndex)
                continue;

            GameObject ghost = Instantiate(ghostPrefab);

            GhostReplay replay = ghost.AddComponent<GhostReplay>();
            replay.Init(lap);

            activeGhosts.Add(replay);
        }
    }

    class GhostReplay : MonoBehaviour
    {
        GhostFrame[] frames;

        int index;
        float timer;

        public void Init(GhostLap lap)
        {
            frames = lap.frames;
        }

        private void Update()
        {
            if (frames == null || frames.Length < 2)
                return;

            timer += Time.deltaTime;

            while (index < frames.Length - 2 && frames[index + 1].time < timer)
                index++;

            GhostFrame a = frames[index];
            GhostFrame b = frames[index + 1];

            float lerp = Mathf.InverseLerp(a.time, b.time, timer);

            Vector2 posA = new(a.x, a.y);
            Vector2 posB = new(b.x, b.y);

            transform.position = Vector2.Lerp(posA, posB, lerp);

            float rot = Mathf.LerpAngle(a.rot, b.rot, lerp);
            transform.rotation = Quaternion.Euler(0, 0, rot);
        }
    }
}
