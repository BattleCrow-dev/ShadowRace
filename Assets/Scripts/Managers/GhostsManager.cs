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
    public int version;
    public bool isMobile;

    public string playerName;

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

    [Header("Sprites")]
    [SerializeField] private List<Sprite> commonGhostSprites;
    [SerializeField] private List<Sprite> fastGhostSprites;
    [SerializeField] private List<Sprite> pickupGhostSprites;

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

    public void StartGame(int trackIndex, int skinIndex, int colorIndex)
    {
        this.trackIndex = trackIndex;
        this.skinIndex = skinIndex;
        this.colorIndex = colorIndex;

        YG2.MultiplayerSessions.onSessionsLoaded -= OnSessionsLoaded;
        YG2.MultiplayerSessions.onSessionsLoaded += OnSessionsLoaded;

        InitConfig config = new()
        {
            count = ghostsToSpawn,
            meta = new MetaFilter()
            {
                meta1 = new YG.Range() // lapTime
                {
                    min = 0,
                    max = 50
                },
                meta2 = new YG.Range() // version
                {
                    min = 0,
                    max = 2
                },
                meta3 = new YG.Range() // trackIndex
                {
                    min = trackIndex - 1,
                    max = trackIndex + 1
                },
            }
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

    public void SetPlayerCar(Transform car)
    {
        playerCar = car;
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
            isMobile = YG2.envir.isMobile,
            version = 1,
            playerName = YG2.player.name,
            lapTime = timer,
            frames = frames.ToArray()
        };

        string json = JsonUtility.ToJson(lap);

        Payload payload = new() { ghostLap = json };
        YG2.MultiplayerSessions.Commit(payload);

        Meta meta = new()
        {
            meta1 = (long) timer,
            meta2 = 1,
            meta3 = trackIndex
        };

        SavesManager.Instance.SetBestResult(trackIndex, timer);

        YG2.MultiplayerSessions.Push(meta);
    }

    private void OnSessionsLoaded(List<Session> loadedSessions)
    {
        sessions = loadedSessions;
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
            GameObject ghost = Instantiate(ghostPrefab);

            ghost.GetComponent<SpriteRenderer>().sprite = lap.skinIndex switch
            {
                0 => commonGhostSprites[lap.colorIndex],
                1 => fastGhostSprites[lap.colorIndex],
                2 => pickupGhostSprites[lap.colorIndex],
                _ => commonGhostSprites[lap.colorIndex],
            };

            GhostReplay replay = ghost.AddComponent<GhostReplay>();
            replay.Init(lap);

            activeGhosts.Add(replay);
        }
    }

    public List<float> GetGhostsTimes()
    {
        List<float> times = new();

        foreach (var ghost in activeGhosts)
            times.Add(ghost.GetFinishTime());

        return times;
    }

    public List<string> GetGhostsNames()
    {
        List<string> names = new();

        foreach (var ghost in activeGhosts)
            names.Add(ghost.GetPlayerName());

        return names;
    }

    public List<Sprite> GetGhostsSprites()
    {
        List<Sprite> cars = new();

        foreach (var ghost in activeGhosts)
            cars.Add(ghost.gameObject.GetComponent<SpriteRenderer>().sprite);

        return cars;
    }

    class GhostReplay : MonoBehaviour
    {
        GhostLap lap;
        GhostFrame[] frames;

        int index;
        float timer;

        public void Init(GhostLap lap)
        {
            this.lap = lap;
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

        public float GetFinishTime() => lap.frames[lap.frames.Length - 1].time;
        public string GetPlayerName() => lap.playerName;
    }
}
