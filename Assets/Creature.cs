using System.Collections;
using UnityEngine;

public enum Gender { Male, Female }

public class Creature : MonoBehaviour
{
    public float speed;
    public float hunger;
    public float hungerDecreaseRate;
    public float energy;
    public float energyDecreaseRate;
    public Gender gender;

    public float reproductionCooldown = 5f;
    private bool canReproduce = true;

    private Renderer renderer;

    private void Awake()
    {
        InitializeTraits();
    }

    private void InitializeTraits()
    {
        // 속도와 감소율을 일정 범위에서 랜덤하게 설정
        speed = Random.Range(1f, 10f);
        hungerDecreaseRate = Random.Range(0.1f, 1f);
        energyDecreaseRate = Random.Range(0.1f, 1f);
        hunger = 100f;
        energy = 100f;
        gender = (Gender)Random.Range(0, 2);

        // Renderer 컴포넌트를 가져옵니다.
        renderer = GetComponent<Renderer>();

        // 성별에 따라 색상을 설정합니다.
        if (gender == Gender.Male)
        {
            renderer.material.SetColor("_Color", Color.blue); // 파란색
        }
        else
        {
            renderer.material.SetColor("_Color", new Color(1f, 0.5f, 0f)); // 주황색
        }
    }

    private void Start()
    {
        StartCoroutine(Live());
    }

    private IEnumerator Live()
    {
        while (hunger > 0 && energy > 0)
        {
            hunger -= hungerDecreaseRate * Time.deltaTime;
            energy -= energyDecreaseRate * Time.deltaTime;

            Berry closestBerry = FindClosestBerry();
            if (closestBerry != null)
            {
                MoveTowards(closestBerry.transform.position);

                if (Vector3.Distance(transform.position, closestBerry.transform.position) < 1f)
                {
                    Eat(closestBerry);
                }
            }

            yield return null;
        }

        Die();
    }

    private Berry FindClosestBerry()
    {
        Berry[] berries = FindObjectsOfType<Berry>();
        Berry closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Berry berry in berries)
        {
            float distance = Vector3.Distance(transform.position, berry.transform.position);
            if (distance < minDistance)
            {
                closest = berry;
                minDistance = distance;
            }
        }

        return closest;
    }

    private void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private void Eat(Berry berry)
    {
        hunger = 100f;
        energy = 100f;
        Destroy(berry.gameObject);
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }

    public bool CanReproduce()
    {
        return canReproduce && hunger > 50 && energy > 50;
    }

    public void Reproduce(Creature mate)
    {
        if (CanReproduce() && mate.CanReproduce() && gender != mate.gender)
        {
            GameObject childObject = Instantiate(gameObject, RandomPosition(), Quaternion.identity);
            Creature child = childObject.GetComponent<Creature>();

            // 자식의 유전자를 부모로부터 랜덤한 비율로 받아옴
            float mix = Random.value;
            child.speed = Mathf.Lerp(this.speed, mate.speed, mix);
            child.hungerDecreaseRate = Mathf.Lerp(this.hungerDecreaseRate, mate.hungerDecreaseRate, mix);
            child.energyDecreaseRate = Mathf.Lerp(this.energyDecreaseRate, mate.energyDecreaseRate, mix);

            StartCoroutine(Cooldown());

            mate.StartCoroutine(mate.Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        canReproduce = false;
        yield return new WaitForSeconds(reproductionCooldown);
        canReproduce = true;
    }

    private Vector3 RandomPosition()
    {
        return new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
    }
}
