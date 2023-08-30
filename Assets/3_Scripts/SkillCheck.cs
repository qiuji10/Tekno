using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class SkillCheck : MonoBehaviour
{
    [Header("Charging values")]
    [SerializeField] private int numOfNodes = 0;
    [SerializeField] private int successPress = 0;
    [SerializeField] private Slider chargeSlider;
    public Image[] targetGameObject;
    public Image targetImage;
    public Image barNormal;
    public Image barSuccess;
    public Image barFail;
    public int previousValue = 0;
    private int spawnNum = 5;
    public int counter;
    public int randIndex;
    private bool success;

    
    private void Start()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
        counter = -1;
        randIndex = Random.Range(1, spawnNum);

    }
    private void OnDestroy()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    private void TempoManager_OnBeat()
    {
        counter++;
        Debug.Log(counter);

        if(counter == 0 || counter == 5)
        {
            targetImage.gameObject.SetActive(false);
            targetImage.rectTransform.position = targetGameObject[counter].transform.position;
        }
        else
        {
            targetImage.gameObject.SetActive(true);
            targetImage.rectTransform.position = targetGameObject[counter].transform.position;
        }
        

        for (int i = 0; i < targetGameObject.Length; i++)
        {

            if (i != randIndex)
            {
                targetGameObject[i].color = Color.white;
            }

        }

        

        targetGameObject[randIndex].color = Color.green;

        if (counter == 5)
        {
            counter = -1;
        }


    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (counter == randIndex)
            {
                success = true;
                successPress++;
                Debug.Log("Success");
                StartCoroutine(ChangeSprite());
                StartCoroutine(ChangeTarget());
            }
            else
            {
                success = false;
                
                Debug.Log("Fail");
                StartCoroutine(ChangeSprite());
                StartCoroutine(ChangeTarget());
                
            }
            previousValue = randIndex;

        }

    }

    IEnumerator ChangeTarget()
    {
        yield return new WaitForSeconds(0.6f);
        
        randIndex = Random.Range(1, spawnNum);
        while (randIndex == previousValue)
        {
            randIndex = Random.Range(1, spawnNum);
        }
    }

    IEnumerator ChangeSprite()
    {
        if(success)
        {
            gameObject.GetComponent<Image>().sprite = barSuccess.sprite;
            gameObject.GetComponent<Image>().color = Color.green;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = barFail.sprite;
            gameObject.GetComponent<Image>().color = Color.red;
        }
        yield return new WaitForSeconds(0.6f);

        gameObject.GetComponent<Image>().sprite = barNormal.sprite;
        gameObject.GetComponent<Image>().color = Color.green;
    }
}
