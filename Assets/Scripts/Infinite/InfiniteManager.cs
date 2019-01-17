using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class InfiniteManager : MonoBehaviour
{
    public float camSensorZ = 5;
    public List<Chunk> chunks;

    Chunk lastChunk;
    Chunk beforeChunk;
    Chunk currentChunk;
    Chunk aheadChunk;
    Chunk firstChunk;

    private List<Chunk> inUse;

    Character character;
    CharacterMovement characterMovement;
    Transform characterTransform;
    Vector3 startPosition;
    public

    Transform sensor;

    public event System.Action<Chunk> ChunkPlaced;

    public static InfiniteManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        character = Character.GetLocalPlayer();
        characterMovement = character.movement;
        characterTransform = character.transform;
        chunks.Shuffle();
        inUse = new List<Chunk>();
        foreach (var chunk in chunks)
        {
            inUse.Add(chunk);
        }
        Initialize();

        sensor = new GameObject("Sensor").transform;
        sensor.SetParent(characterTransform);
        sensor.localPosition = new Vector3(0, 0, camSensorZ);
        Controller.Instance.gameCamera.GetComponent<GameCamera>().SetTarget(sensor);
    }


    private void Update()
    {
        if(characterTransform.position.x > currentChunk.currentPosition.x + currentChunk.sizeX)
        {
            RearangeChunks();
        }
    }

    private void Initialize()
    {
        lastChunk = GetChunk();
        beforeChunk = GetChunk();
        currentChunk = GetChunk();
        aheadChunk = GetChunk();
        firstChunk = GetChunk();

        currentChunk.transform.position = characterTransform.position + Vector3.down;
        currentChunk.currentPosition = currentChunk.transform.position;
        beforeChunk.PlaceBefore(currentChunk);
        lastChunk.PlaceBefore(beforeChunk);
        aheadChunk.PlaceNextTo(currentChunk);
        firstChunk.PlaceNextTo(aheadChunk);
    }

    void RearangeChunks()
    {
        lastChunk.spawningPlace.RecycleActiveSpawns();
        inUse.Add(lastChunk);
        lastChunk = beforeChunk;
        beforeChunk = currentChunk;
        currentChunk = aheadChunk;
        aheadChunk = firstChunk;
        firstChunk = GetChunk();
        firstChunk.PlaceNextTo(aheadChunk);
        ChunkPlaced?.Invoke(firstChunk);
    }

    Chunk GetChunk()
    {
        Chunk chunk;
        int index = Random.Range(0, inUse.Count);
        chunk = inUse[index];
        inUse.Remove(chunk);
        return chunk;
    }
}