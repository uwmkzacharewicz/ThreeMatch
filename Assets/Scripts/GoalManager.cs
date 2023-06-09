using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numberNedded;
    public int numberCollected;
    public string matchValue;
    public Sprite goalSprite;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();

    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;


    // Start is called before the first frame update
    void Start()
    {
        SetUpGoals();

    }

    void SetUpGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform);

            // Set image and text
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNedded;


            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPanel>();

            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNedded;
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = levelGoals[i].numberCollected + "/" + levelGoals[i].numberNedded;

            if (levelGoals[i].numberCollected >= levelGoals[i].numberNedded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = levelGoals[i].numberNedded + "/" + levelGoals[i].numberNedded;
            }           
        }

        if (goalsCompleted >= levelGoals.Length)
        {
            Debug.Log("You win!");
        }


    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
