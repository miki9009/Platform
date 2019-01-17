
using Engine;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager: MonoBehaviour
{

    public int prefabsCount;
    public int prewarmNumber = 8;
    public int startTrackIndex = 5;
    public GameObject[] prefabs;

    Track foreTrack;
    List<Track> spare;
    Queue<Track> queue;
    bool initialized = false;
    Track startTrack;
    public int index = 0;
    public GameObject coinPrefab;

    public static TrackManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Level.LevelLoaded += Init;

        Track.TrackReached += OnNewTrackReached;
        Character.CharacterCreated += SetToTrack;
        GameManager.LevelClear += Restart;


    }
    public void SetIndex(int index)
    {
        this.index = index;
    }

    private void Init()
    {
        SpawnManager.AddSpawn("Coin", coinPrefab, 150);
        spare = new List<Track>();
        queue = new Queue<Track>();
        index = 0;
        int k = 0;
        for (int i = 0; i < prefabs.Length; i++)
        {
            for (int j = 0; j < prefabsCount; j++)
            {
                spare.Add(Instantiate(prefabs[i], new Vector3(-100000,-100000,-100000),prefabs[i].transform.rotation).GetComponent<Track>());
                spare[spare.Count - 1].name = k.ToString();
                k++;
            }
        }
        for (int i = 0; i < prewarmNumber; i++)
        {
            InitTrack(i);
        }

        initialized = true;

    }

    void SetToTrack(Character character)
    {
        character.transform.position = startTrack.transform.position;
    }

    private void OnDestroy()
    {
        Track.TrackReached -= OnNewTrackReached;
        Character.CharacterCreated -= SetToTrack;
        GameManager.LevelClear -= Restart;
        Level.LevelLoaded -= Init;
    }

    void OnNewTrackReached(Track track)
    {
        index++;
        var t = Dequeue();
        spare.Add(t);
        t.active = true;
        t.gameObject.SetActive(false);
        var newLast = spare[Random.Range(0, spare.Count-1)];
        newLast.gameObject.SetActive(true);
        newLast.index = index;
        Transform anchor = foreTrack.endAnchor;      
        newLast.transform.position = anchor.position;
        newLast.transform.rotation = anchor.rotation;
        foreTrack = newLast;
        spare.Remove(foreTrack);
        Enqueue(foreTrack);
        PlaceCollection(foreTrack);
    }

    void InitTrack(int i)
    {
        index++;
        Transform anchor = null;
        if (foreTrack == null)
        {
            anchor = transform;
        }
        else
        {
            anchor = foreTrack.endAnchor;
        }


        var newLast = Instantiate(prefabs[0]).GetComponent<Track>();
        newLast.gameObject.SetActive(true);
        newLast.index = index;


        newLast.transform.position = anchor.position;
        newLast.transform.rotation = anchor.rotation;
        foreTrack = newLast;
        spare.Remove(foreTrack);
        Enqueue(foreTrack);
        if (i == startTrackIndex)
            startTrack = foreTrack;
    }

    void Enqueue(Track track)
    {
        //Debug.Log("Enqueue " + track.name);
        queue.Enqueue(track);
    }

    void Restart()
    {
        foreTrack = null;
        SpawnManager.ClearSpawns();
        initialized = false;
        foreach (var track in spare)
        {
            if (track != null)
                Destroy(track.gameObject);
        }
        foreach (var track in queue)
        {
            if (track != null)
                Destroy(track.gameObject);
        }

    }

    Track Dequeue()
    {
        var t = queue.Dequeue();
        return t;
    }

    void PlaceCollection(Track track)
    {
        int coins = Random.Range(1, 10);
        float factor = 1f / coins;
        bool left = Math.Probability(0.5f);
        for (int i = 1; i < coins; i++)
        {
            if(Math.Probability(0.25f))
            {
                Vector3 pos = track.GetPosition(factor * i);
                var rot = track.GetRotation(factor * i);
                var coin = SpawnManager.GetSpawn("Coin");
                if (coin != null)
                {
                    coin.transform.rotation = rot;                 
                    coin.transform.position = pos + (left ? -coin.transform.right*2f : coin.transform.right * 2f);
                    coin.transform.SetParent(track.transform);
                }
            }


        }
    }
}