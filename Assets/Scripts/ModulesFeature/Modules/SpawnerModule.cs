using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerModule : Module
{
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private AssemblyTravelingObject objectToSpawn;

    private float counter;

    private void Start() {
        counter = 0;
    }

    private void Update() {
        counter += Time.deltaTime;
        if(counter > timeBetweenSpawns) {
            counter -= timeBetweenSpawns;
            SpawnObject();
        }
    }

    private void SpawnObject() {
        if (!OutputHasRoom()) {
            return;
        }
        GameObject obj = Instantiate(objectToSpawn.gameObject);
        AssemblyTravelingObject travelingObject = obj.GetComponent<AssemblyTravelingObject>();
        obj.SetActive(false);
        SendObjectOut(travelingObject);
    }
}

[System.Serializable]
public class SpawnWave {
    [field: SerializeField] public int TotalNumSpawns { get; private set; }

    public SpawnWave(int numSpawns) {
        this.TotalNumSpawns = numSpawns;
    }
}

public class Spawner {

    public bool IsFinished { get; protected set; }

    protected List<SpawnWave> wavesToSpawn;

    protected SpawnWave currentWave;
    protected int currentWaveNumber;
    protected int spawnLeftThisWave;
    protected GameObject spawnPrefab;

    public Spawner(List<SpawnWave> waves, GameObject prefab) {
        this.wavesToSpawn = waves;
        this.spawnPrefab = prefab;
        Reset();
    }

    public void Reset() {
        StartWaveNumber(0);
        IsFinished = false;
    }

    protected void StartWaveNumber(int number) {
        currentWave = wavesToSpawn[number];
        currentWaveNumber = number;
        spawnLeftThisWave = currentWave.TotalNumSpawns;
    }
    
    protected AssemblyTravelingObject CreateObject() {
        AssemblyTravelingObject assemblyLineObject = MonoBehaviour.Instantiate(spawnPrefab).GetComponent<AssemblyTravelingObject>();
        //Properties props = currentWave.CreateProperties();
        //assemblyLineObject.Properties = props;
        return assemblyLineObject;
    }

    public virtual AssemblyTravelingObject GetNextObject() {
        AssemblyTravelingObject aObject = CreateObject();
        spawnLeftThisWave -= 1;

        if (spawnLeftThisWave <= 0) {
            if(currentWaveNumber + 1 < wavesToSpawn.Count) {
                StartWaveNumber(currentWaveNumber + 1);
            }
            else {
                IsFinished = true;
            }
            
        }
        
        return aObject;
    }
}
