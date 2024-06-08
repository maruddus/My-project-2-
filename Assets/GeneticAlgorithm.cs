using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public GameObject creaturePrefab;
    public int initialPopulation = 10;
    public float mutationRate = 0.01f;

    private List<Creature> creatures = new List<Creature>();

    private void Start()
    {
        for (int i = 0; i < initialPopulation; i++)
        {
            SpawnCreature();
        }
    }

    private void Update()
    {
        if (creatures.Count == 0)
        {
            BreedNewGeneration();
        }
    }

    private void SpawnCreature()
    {
        GameObject creatureObject = Instantiate(creaturePrefab, RandomPosition(), Quaternion.identity);
        Creature creature = creatureObject.GetComponent<Creature>();
        creatures.Add(creature);
    }

    private Vector3 RandomPosition()
    {
        return new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
    }

    private void BreedNewGeneration()
    {
        List<Creature> newGeneration = new List<Creature>();

        // 간단한 유전 알고리즘: 상위 절반의 생명체를 선택하여 번식
        creatures.Sort((a, b) => b.energy.CompareTo(a.energy));
        int survivors = creatures.Count / 2;

        for (int i = 0; i < survivors; i++)
        {
            Creature parent1 = creatures[i];
            Creature parent2 = creatures[Random.Range(0, survivors)];
            newGeneration.Add(Breed(parent1, parent2));
        }

        creatures = newGeneration;
    }

    private Creature Breed(Creature parent1, Creature parent2)
    {
        GameObject creatureObject = Instantiate(creaturePrefab, RandomPosition(), Quaternion.identity);
        Creature child = creatureObject.GetComponent<Creature>();

        // 부모의 유전자를 조합하여 자식을 생성
        child.speed = Random.value > 0.5f ? parent1.speed : parent2.speed;
        child.hungerDecreaseRate = Random.value > 0.5f ? parent1.hungerDecreaseRate : parent2.hungerDecreaseRate;

        // 돌연변이
        if (Random.value < mutationRate)
        {
            child.speed += Random.Range(-0.1f, 0.1f);
        }

        if (Random.value < mutationRate)
        {
            child.hungerDecreaseRate += Random.Range(-0.1f, 0.1f);
        }

        return child;
    }
}
