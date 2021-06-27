using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public ParticleSystem allCleanPrefab;
    public ParticleSystem sparksPrefab;
    public ParticleSystem dustPrefab;
    ParticleSystem myAllCleanParticle;
    ParticleSystem mySparksParticle;
    ParticleSystem myDustParticle;
    public GameObject myMurphyDirt;
    public bool canIncrease;
    public Text SpeedAmount;
    public GameObject numbersCanvas;
    public Text averageMovesText, simAmountText, currentMovesText;
    public int simAmountNum = 1, currentMovesNum = 0, totalMovesNum;
    public float averageMovesNum;
    public bool vacCurrentlyMoving = false;
    public bool isLocked;
    public GameObject theVacuum;
    public Rigidbody vacRigidbody;
    public GameObject dirtPilePrefab, murphysDirtPrefab;
    public int dirtPileMax = 1;
    public Text dirtPileDisplay;
    public bool reflexAgent = true;
    public bool randomAgent;
    public bool murphysMode;
    public List<GameObject> myDirtPilesList;
    public bool[,] markDirtPlacements;
    public bool buildingDone = false, dirtToBelow = false, dirtToLeft = false, dirtToRight = false, dirtToUp = false, dirtToDown = false;
    void Start()
    {
        if (theVacuum != null)
            vacRigidbody = theVacuum.GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (dirtPileDisplay != null)
            dirtPileDisplay.text = dirtPileMax.ToString();
        bool wasLocked = isLocked;
        isLocked = IsLocked(0.01f);
        //If vac just left a square
        if (wasLocked && !isLocked) {
            UpdateFigures();
            UpdateText();
            MakeAPoopie();
        }
        //If vac just landed on a square
        //} else if (!wasLocked && isLocked) {
        //    MakeAPoopie();
        //}
        if (buildingDone) {
            if (isLocked) {
                dirtToBelow = LookForDirtUnderMe();
                dirtToRight = LookForDirtRight();
                dirtToLeft = LookForDirtLeft();
                dirtToUp = LookForDirtUp();
                dirtToDown = LookForDirtDown();
                int curPosX = (int) Mathf.Round(theVacuum.transform.position.x);
                int curPosZ = (int) Mathf.Round(theVacuum.transform.position.z);
                if (reflexAgent) {
                    ReflexAgentChoiceList(curPosX, curPosZ);
                } else {
                    RandomAgentChoiceList(curPosX, curPosZ);
                }
            }
        }
    }
    public void SuckUpDirt(int curPosX, int curPosZ) {
        if (markDirtPlacements[curPosX,curPosZ] == true) {
            foreach (GameObject dirt in myDirtPilesList) {
                if (dirt != null) {
                    //Looks at each dirt pile and locates one with the same coordinates
                    if (dirt.transform.position.x == curPosX && dirt.transform.position.z == curPosZ) {
                        if (murphysMode) {
                            int coinFlip = Random.Range(0,100);
                            if (coinFlip > 25) {
                                myDirtPilesList.Remove(dirt);
                                Destroy(dirt.gameObject);
                                markDirtPlacements[curPosX, curPosZ] = false;
                                break;
                            //} else {
                            //    StartCoroutine(PlayBrokeAnim());
                            }
                        } else {
                            myDirtPilesList.Remove(dirt);
                            Destroy(dirt.gameObject);
                            markDirtPlacements[curPosX, curPosZ] = false;
                            break;
                        }
                    }
                }
            }
        }
    }
    public bool LookForDirtUnderMe()
    {
        int curPosX = (int) Mathf.Round(theVacuum.transform.position.x);
        int curPosZ = (int) Mathf.Round(theVacuum.transform.position.z);
        if (markDirtPlacements[curPosX,curPosZ] == true)
            return true;
        return false;
    }
    public bool LookForDirtRight()
    {
        int curPosX = (int) Mathf.Round(theVacuum.transform.position.x)+1;
        for (int i = curPosX; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                if (markDirtPlacements[i,j] == true)
                    return true;
            }
        }
        return false;
    }
    public bool LookForDirtUp()
    {
        int curPosZ = (int) Mathf.Round(theVacuum.transform.position.z)+1;
        for (int i = 0; i < 3; ++i) {
            for (int j = curPosZ; j < 3; ++j) {
                if (markDirtPlacements[i,j] == true)
                    return true;
            }
        }
        return false;
    }
    public bool LookForDirtLeft()
    {
        int curPosX = (int) Mathf.Round(theVacuum.transform.position.x);
        for (int i = 0; i < curPosX; ++i) {
            for (int j = 0; j < 3; ++j) {
                if (markDirtPlacements[i,j] == true)
                    return true;
            }
        }
        return false;
    }
    public bool LookForDirtDown()
    {
        int curPosZ = (int) Mathf.Round(theVacuum.transform.position.z);
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < curPosZ; ++j) {
                if (markDirtPlacements[i,j] == true)
                    return true;
            }
        }
        return false;
    }
    public IEnumerator StayAndWait()
    {
        vacRigidbody.isKinematic = true;
        yield return new WaitForSeconds(2f);
    }
    public void MakeAPoopie()
    {
        if (murphysMode) {
            int curPosX = (int) Mathf.Round(theVacuum.transform.position.x);
            int curPosZ = (int) Mathf.Round(theVacuum.transform.position.z);
            if (markDirtPlacements[curPosX,curPosZ] == false) {
                int coinFlip = Random.Range(0,100);
                if (coinFlip <= 10) {
                    myMurphyDirt = Instantiate(murphysDirtPrefab, new Vector3(curPosX, 0.5f, curPosZ), Quaternion.identity) as GameObject;
                    myDirtPilesList.Add(myMurphyDirt);
                    markDirtPlacements[curPosX,curPosZ] = true;
                }
            }
        }
    }
    public IEnumerator MoveAndWait(int i, int j)
    {
        canIncrease = true;
        vacCurrentlyMoving = true;
        yield return new WaitForSeconds(1f);
        vacRigidbody.isKinematic = false;
        Vector3 startPos = theVacuum.transform.position;
        Vector3 desPos = new Vector3(i, 1, j);
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) { 
            currentTime += Time.deltaTime; 
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 
            theVacuum.transform.position = Vector3.Lerp(startPos,desPos, normalizedValue); 
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        if (Vector3.Distance(desPos, theVacuum.transform.position) <= 0.1f) {
            theVacuum.transform.position = desPos;
            vacCurrentlyMoving = false;
        }
        yield return null;
    }
    public bool IsLocked(float range)
    {
        int lockedOnX = (int) Mathf.Round(theVacuum.transform.position.x);
        int lockedOnZ = (int) Mathf.Round(theVacuum.transform.position.z);
        Vector3 destVec = new Vector3(lockedOnX, 1, lockedOnZ);
        if (Vector3.Distance(destVec, theVacuum.transform.position) <= range) {
            theVacuum.transform.position = destVec;
            return true;
        }
        return false;
    }
    public void SetDirtPileMax(float newdirtPileMax)
    {
        dirtPileMax = (int) Mathf.Round(newdirtPileMax);
    }
    public void SetReflexAgent(bool newReflexAgent)
    {
        if (newReflexAgent == true)
            randomAgent = false;
        else
            randomAgent = true;

        reflexAgent = newReflexAgent;
    }
    public void SetMurphysMode(bool newMurphysMode)
    {
        murphysMode = newMurphysMode;
    }
    public void BeginBuild()
    {
        numbersCanvas.SetActive(true);
        MarkDirtPlacements();
        RandomizeVacuumStartLocation();
        myDirtPilesList = new List<GameObject>();
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                if (dirtPilePrefab != null) {
                    if (markDirtPlacements[i,j] == true) {
                        GameObject myNewDirt = Instantiate(dirtPilePrefab, new Vector3(i, 0.5f, j), Quaternion.identity) as GameObject;
                        // We can then access all dirt piles from this list in the future
                        myDirtPilesList.Add(myNewDirt);
                    }
                }
            }
        }
        buildingDone = true;
    }
    public void MarkDirtPlacements()
    {
        markDirtPlacements = new bool[3, 3];
        //Initializes all values to false
        for (int i = 0; i < 3; ++i)
            for (int j = 0; j < 3; ++j)
                markDirtPlacements[i,j] = false;
        // Creates marks lists
        int currentMarks = 0;
        while (currentMarks < dirtPileMax) {
            //Debug.Log("CurrentMarks = " + currentMarks.ToString() + " and dirtPileMax = " + dirtPileMax.ToString());
            for (int i = 0; i < 3; ++i) {
                for (int j = 0; j < 3; ++j) {
                    if (markDirtPlacements[i,j] == false) {
                        int coinFlip = Random.Range(0,100);
                        if (coinFlip > 50) {
                            markDirtPlacements[i,j] = true;
                            ++currentMarks;
                            if (currentMarks >= dirtPileMax)
                                return;
                        }
                    }    
                }
            }
        }
    }
    public void RandomizeVacuumStartLocation()
    {
        bool hasBeenPlaced = false;
        if (theVacuum != null) {
            while (!hasBeenPlaced) {
                for (int i = 0; i < 3; ++i) {
                    for (int j = 0; j < 3; ++j) {
                        int coinFlip = Random.Range(0,100);
                        if (coinFlip > 70) {
                            theVacuum.transform.position = new Vector3(i,1,j);
                            hasBeenPlaced = true;
                            return;
                        }
                    }
                }
            }
        }
    }
    public void UpdateText()
    {
        if (averageMovesText != null && simAmountText != null && currentMovesText != null) {
            averageMovesText.text = averageMovesNum.ToString("F3");
            simAmountText.text = simAmountNum.ToString() + "/100";
            currentMovesText.text = currentMovesNum.ToString();
        }
    }
    public void UpdateFigures()
    {
        averageMovesNum = totalMovesNum / (float) simAmountNum;
        ++totalMovesNum;
        ++currentMovesNum;
    }
    public IEnumerator AllClean()
    {
        buildingDone = false;
        int waitCount = 0;
        myAllCleanParticle = Instantiate(allCleanPrefab, theVacuum.transform.position, Quaternion.identity) as ParticleSystem;
        while (!isLocked || waitCount < 10) {
            yield return new WaitForSeconds(1f); 
            ++waitCount;
        }
        Destroy(myAllCleanParticle);
        bool wasBuilt = buildingDone;
        ClearEverything();
        BeginBuild();
        if (!wasBuilt && buildingDone)
            ResetFigures();
        yield return null;
    }
    public void ClearEverything()
    {
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                markDirtPlacements[i,j] = false;
            }
        }
       /* 
        for (int i = 0; i < myDirtPilesList.Count; ++i) {
            if (myDirtPilesList[i] != null) {
                myDirtPilesList.Remove(myDirtPilesList[i]);
                Destroy(myDirtPilesList[i].gameObject);
            }
        }
        */
        
        foreach (GameObject dirt in myDirtPilesList.ToArray()) {
            if (dirt != null) {
                myDirtPilesList.Remove(dirt);
                Destroy(dirt.gameObject);
            }
        }
    }
    public void ResetFigures()
    {
        totalMovesNum += currentMovesNum;
        currentMovesNum = 0;
        simAmountNum++;
    }
    public void UpdateTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
        SpeedAmount.text = newTimeScale.ToString();
    }
    public void ReflexAgentChoiceList(int curPosX, int curPosZ)
    {
        // Clockwise search
        if (currentMovesNum >= 100) {
            buildingDone = false;
            StartCoroutine(AllClean());
        } else if (currentMovesNum % 2 == 0) {
            if (dirtToBelow) {
                SuckUpDirt(curPosX,curPosZ);
            } else if (dirtToRight) {
                StartCoroutine(MoveAndWait(curPosX+1,curPosZ));
            } else if (dirtToDown) {
                StartCoroutine(MoveAndWait(curPosX,curPosZ-1));
            } else if (dirtToLeft) {
                StartCoroutine(MoveAndWait(curPosX-1,curPosZ));
            } else if (dirtToUp) {
                StartCoroutine(MoveAndWait(curPosX,curPosZ+1));
            } else if (currentMovesNum >= 100) {
                buildingDone = false;
                StartCoroutine(AllClean());
            } else {
                buildingDone = false;
                StartCoroutine(AllClean());
            }
        // Counter clockwise search
        } else if (currentMovesNum % 3 == 0) {
            if (dirtToBelow) {
                SuckUpDirt(curPosX,curPosZ);
            } else if (dirtToUp) {
                StartCoroutine(MoveAndWait(curPosX,curPosZ+1));
            } else if (dirtToRight) {
                StartCoroutine(MoveAndWait(curPosX+1,curPosZ));
            } else if (dirtToLeft) {
                StartCoroutine(MoveAndWait(curPosX-1,curPosZ));
            } else if (dirtToDown) {
                StartCoroutine(MoveAndWait(curPosX,curPosZ-1));
            } else if (currentMovesNum >= 100) {
                buildingDone = false;
                StartCoroutine(AllClean());
            } else {
                buildingDone = false;
                StartCoroutine(AllClean());
            }
        // Mixed Up
        } else if (currentMovesNum % 5 == 0) {
            if (dirtToBelow) {
                SuckUpDirt(curPosX,curPosZ);
            } else if (dirtToLeft) {
                StartCoroutine(MoveAndWait(curPosX-1,curPosZ));
            } else if (dirtToUp) {
                StartCoroutine(MoveAndWait(curPosX,curPosZ+1));
            } else if (dirtToDown) {
                StartCoroutine(MoveAndWait(curPosX,curPosZ-1));
            } else if (dirtToRight) {
                StartCoroutine(MoveAndWait(curPosX+1,curPosZ));
            } else if (currentMovesNum >= 100) {
                buildingDone = false;
                StartCoroutine(AllClean());
            } else {
                buildingDone = false;
                StartCoroutine(AllClean());
            }
        // Mixed Up
        } else {
            if (dirtToBelow) {
                SuckUpDirt(curPosX,curPosZ);
            } else if (dirtToUp) {
                StartCoroutine(MoveAndWait(curPosX,curPosZ+1));
            } else if (dirtToDown) {
                StartCoroutine(MoveAndWait(curPosX,curPosZ-1));
            } else if (dirtToLeft) {
                StartCoroutine(MoveAndWait(curPosX-1,curPosZ));
            } else if (dirtToRight) {
                StartCoroutine(MoveAndWait(curPosX+1,curPosZ));
            } else if (currentMovesNum >= 100) {
                buildingDone = false;
                StartCoroutine(AllClean());
            } else {
                buildingDone = false;
                StartCoroutine(AllClean());
            }
        }
    }
    public void RandomAgentChoiceList(int curPosX, int curPosZ)
    {
        int myChoice = Random.Range(1,5);

            if (currentMovesNum >= 100) {
                buildingDone = false;
                StartCoroutine(AllClean());
            } else if (dirtToBelow) {
                SuckUpDirt(curPosX,curPosZ);
            } else if (!dirtToUp && !dirtToRight && !dirtToLeft && !dirtToBelow && !dirtToDown) {
                buildingDone = false;
                StartCoroutine(AllClean());
            } else if (myChoice == 1 && curPosX < 2) {
                StartCoroutine(MoveAndWait(curPosX+1,curPosZ));
            } else if (myChoice == 2 && curPosZ > 0) {
                StartCoroutine(MoveAndWait(curPosX,curPosZ-1));
            } else if (myChoice == 3 && curPosX > 0) {
                StartCoroutine(MoveAndWait(curPosX-1,curPosZ));
            } else if (myChoice == 4 && curPosZ < 2) {
                StartCoroutine(MoveAndWait(curPosX,curPosZ+1));
            }
    }
    public IEnumerator PlayBrokeAnim()
    {
        mySparksParticle = Instantiate(sparksPrefab, theVacuum.transform.position, Quaternion.identity) as ParticleSystem;
        myDustParticle = Instantiate(dustPrefab, theVacuum.transform.position, Quaternion.identity) as ParticleSystem;
        yield return new WaitForSeconds(2f);
        mySparksParticle.Stop();
        myDustParticle.Stop();
        yield return new WaitForSeconds(3f);
        Destroy(mySparksParticle);
        Destroy(myDustParticle);
        yield return null;
    }
}
