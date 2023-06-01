using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeControl : MonoBehaviour
{
    public GameObject[] bridgePrefabs;

    enum enType
    {
        L_Corner,
        Straight,
        R_Corner
    }
    

    enum enDirection
    {
        North,
        East,
        West,
    }

    class Segments
    {
        public GameObject segPrefab;
        public enType segType;

        public Segments(GameObject segPrefab, enType segType)
        {
            this.segPrefab = segPrefab;
            this.segType = segType;
        }

    };

    List<GameObject> activeSegments = new List<GameObject>(); // new yazdýðýmýz için bu komutlarla RAM içerisine activeSegments için yer açmýþ olduk.
    Segments segment; // Þu anda RAM içerisinde null.
    Vector3 spawnCoord = new Vector3(0, 0, -6f);
    enDirection segCurrentDireciton = enDirection.North;
    enDirection segNextDireciton = enDirection.North;
    Transform playerTransform;

    float segLength = 6f;
    float segWidth = 3f;
    int segOnScreen = 5;
    bool stopGame = false;

    void Start()
    {
        segment = new Segments(bridgePrefabs[0], enType.Straight); // Ram da yer açmış olduk.
       
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        InitializeSegments();
    }

    void Update()
    {
        PlayerTrigger();
    }

    void InitializeSegments()
    {
        for (int i = 0; i < segOnScreen; i++)
        {
           SpawnSegments();
        }

    }

    void CreateSegments()
    {
        switch (segCurrentDireciton)
        {
            case enDirection.North:
                segment.segType = (enType)Random.Range(0, 3);
                if (segment.segType == enType.Straight)
                {
                    segment.segPrefab = bridgePrefabs[Random.Range(0, 11)];
                }
                else if (segment.segType == enType.L_Corner)
                {
                    segment.segPrefab = bridgePrefabs[11];
                }
                else if (segment.segType == enType.R_Corner)
                {
                    segment.segPrefab = bridgePrefabs[12];
                }
                break;

            case enDirection.East:
                segment.segType = (enType)Random.Range(0, 2);
                if (segment.segType == enType.Straight)
                {
                    segment.segPrefab = bridgePrefabs[Random.Range(0, 11)];
                }
                else if (segment.segType == enType.L_Corner)
                {
                    segment.segPrefab = bridgePrefabs[11];
                }                
                break;

            case enDirection.West:
                segment.segType = (enType)Random.Range(1, 3);
                if (segment.segType == enType.Straight)
                {
                    segment.segPrefab = bridgePrefabs[Random.Range(0, 11)];
                }
                else if (segment.segType == enType.R_Corner)
                {
                    segment.segPrefab = bridgePrefabs[12];
                }
                break;
        }

    }

    void SpawnSegments()
    {
        GameObject prefabToInstantiate = segment.segPrefab;
        Quaternion prefabRotation = Quaternion.identity;
        Vector3 offSet = new Vector3(0, 0, 0);

        switch (segCurrentDireciton)
        {
            case enDirection.North:
                if (segment.segType == enType.Straight)
                {
                    prefabRotation = Quaternion.Euler(0, 0, 0);
                    segNextDireciton = enDirection.North;
                    spawnCoord.z += segLength;
                }
                else if (segment.segType == enType.R_Corner)
                {
                    prefabRotation = Quaternion.Euler(0, 0, 0);
                    segNextDireciton = enDirection.East;
                    spawnCoord.z += segLength;
                    offSet.z += segLength + segWidth/2;
                    offSet.x += segWidth/2;
                }
                else if (segment.segType == enType.L_Corner)
                {
                    prefabRotation = Quaternion.Euler(0, 0, 0);
                    segNextDireciton = enDirection.West;
                    spawnCoord.z += segLength;
                    offSet.z += segLength + segWidth / 2;
                    offSet.x -= segWidth / 2;
                }
                break;

            case enDirection.East:
                if (segment.segType == enType.Straight)
                {
                    prefabRotation = Quaternion.Euler(0, 90, 0);
                    segNextDireciton = enDirection.East;
                    spawnCoord.x += segLength;
                }
                else if ( segment.segType == enType.L_Corner)
                {
                    prefabRotation = Quaternion.Euler(0, 90, 0);
                    segNextDireciton = enDirection.North;
                    spawnCoord.x += segLength;
                    offSet.z += segWidth / 2;
                    offSet.x += segLength + segWidth / 2;
                }
                break;

            case enDirection.West:
                if (segment.segType == enType.Straight)
                {
                    prefabRotation = Quaternion.Euler(0, -90, 0);
                    segNextDireciton = enDirection.West;
                    spawnCoord.x -= segLength;
                }
                else if (segment.segType == enType.R_Corner)
                {
                    prefabRotation = Quaternion.Euler(0, -90, 0);
                    segNextDireciton = enDirection.North;
                    spawnCoord.x -= segLength;
                    offSet.z += segWidth / 2;
                    offSet.x -= segLength + segWidth / 2;
                }
                break;

        }

        if (prefabToInstantiate != null)
        {
            GameObject spawnPrefab = Instantiate(prefabToInstantiate, spawnCoord, prefabRotation) as GameObject;
            activeSegments.Add(spawnPrefab);
            spawnPrefab.transform.parent = this.transform;
        }

        segCurrentDireciton = segNextDireciton;
        spawnCoord += offSet;
    }

    void RemoveSegments()
    {
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
    }

    void PlayerTrigger()
    {
        if (stopGame)
            return;

        GameObject go = activeSegments[0];
        if (Mathf.Abs(Vector3.Distance(playerTransform.position, go.transform.position)) > 16f)
        {
            CreateSegments();
            SpawnSegments();
            RemoveSegments();
        }
    }

    public void CleanTheScene()
    {
        stopGame = true;
        for(int j = activeSegments.Count -1; j >= 0; j--)
        {
            Destroy(activeSegments[j]);
            activeSegments.RemoveAt(j);
        }

        spawnCoord = new Vector3(0, 0, -6);
        segCurrentDireciton = enDirection.North;
        segNextDireciton = enDirection.North;
        segment = new Segments(bridgePrefabs[0], enType.Straight);
        InitializeSegments();
        stopGame = false;
    }

}
