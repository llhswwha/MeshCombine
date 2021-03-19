using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcPreSpawner : MonoBehaviour
{
    [SerializeField] GameObject _orcPrefab;
    [SerializeField] int _numToSpawn=20;
    // Start is called before the first frame update
    void Start()
    {
        PrefabPoolingSystem.Prespawn(_orcPrefab,_numToSpawn);
    }
}
