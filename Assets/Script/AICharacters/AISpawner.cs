using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour{

    public CharacterStates characterStates { get; protected set; }

    public GameObject aiPrefab;
    public bool spawnPrefab;
    public float delayTime;

    void Start(){
        characterStates = GameObject.Find("Player").GetComponent<CharacterStates>();
    }

    void Update(){
        SpawnEnemy();
    }

    void SpawnEnemy(){
        if(characterStates.locomotions == CharacterStates.Locomotions.CombatLocomotion && !spawnPrefab){
            StartCoroutine(enumerator(delayTime));
            spawnPrefab = true;
        }
    }

    IEnumerator enumerator(float waitTime){
        yield return new WaitForSeconds(delayTime);
        GameObject instance = Instantiate(aiPrefab, transform.position, transform.rotation);
    }
}
