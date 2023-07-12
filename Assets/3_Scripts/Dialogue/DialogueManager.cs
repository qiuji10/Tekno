using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using NaughtyAttributes;
using System;
using UnityEngine.InputSystem;

[System.Serializable]
public class AnimationPattern
{
    public enum AnimPattern { UpDown, Shaky }

    public AnimPattern pattern;
    public AnimationCurve xPosCurve;
    public AnimationCurve yPosCurve;

    public AnimationCurve GetLongestCurve()
    {
        AnimationCurve longestCurve = null;
        float longestTime = 0f;

        if (xPosCurve.length > 1)
        {
            for (int i = 0; i < xPosCurve.length; i++)
            {
                float xPosCurveTime = xPosCurve[i].time;
                if (xPosCurveTime > longestTime)
                {
                    longestTime = xPosCurveTime;
                    longestCurve = xPosCurve;
                }
            }
        }

        if (yPosCurve.length > 1)
        {
            for (int i = 0; i < yPosCurve.length; i++)
            {
                float yPosCurveTime = yPosCurve[i].time;
                if (yPosCurveTime > longestTime)
                {
                    longestTime = yPosCurveTime;
                    longestCurve = yPosCurve;
                }
            }
        }

        return longestCurve;
    }
}

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private AnimationCurve dialogueAnimCruve;
    [SerializeField] private InputActionReference interactAction;

    [Header("Data References")]
    [SerializeField] private List<DialogueCharacterSprite> characterSpDatas = new List<DialogueCharacterSprite>();
    [SerializeField] private List<DialogueCharacterVisual> characters = new List<DialogueCharacterVisual>();
    [SerializeField] private List<DialogueData> dialogueDatas = new List<DialogueData>();
    [SerializeField] private List<AnimationPattern> animations = new List<AnimationPattern>();

    [Header("UI References")]
    [SerializeField] private RectTransform dialogueBoxRect;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text dialogueText;

    private bool dialogueIsOn;
    private int curCharacterIndex;

    private Dialogue curDialogue;
    private Coroutine typingCoroutine, endCoroutine;
    private Queue<Dialogue> dialogues = new Queue<Dialogue>();

    public static event Action OnDialogueStart;
    public static event Action OnDialogueEnd;
    public static bool IsRunning;

    private void OnEnable()
    {
        interactAction.action.performed += Action_performed;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= Action_performed;
    }

    private void Action_performed(InputAction.CallbackContext context)
    {
        if (dialogueIsOn)
            DisplayNextSentence();
    }

    [Button]
    public void test()
    {
        StartDialogue("Test");
    }

    [Button]
    public void next()
    {
        DisplayNextSentence();
    }

    public void StartDialogue(string dialogueName)
    {
        IsRunning = true;

        DialogueData dialogueData = GetDialogue(dialogueName);

        if (dialogues.Count > 0) dialogues.Clear();

        foreach (Dialogue dialogue in dialogueData.dialogues)
        {
            dialogues.Enqueue(dialogue);
        }

        string[] charactersNamelist = dialogueData.GetAllCharacter();

        for (int i = 0; i < charactersNamelist.Length; i++)
        {
            characters[i].spriteData = GetCharacterData(charactersNamelist[i]);
            characters[i].SetImageSprite("normal");
        }

        characterNameText.text = "";
        dialogueText.text = "";

        StartCoroutine(StartSequence());
    }

    public void DisplayNextSentence()
    {
        characters[curCharacterIndex].StopAnimate();

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            characters[curCharacterIndex].SetImageSprite("normal");
            dialogueText.text = curDialogue.RemoveRegexFromString();
            typingCoroutine = null;
            return;
        }

        if (dialogues.Count == 0)
        {
            EndDialogue();
            return;
        }

        curDialogue = dialogues.Dequeue();

        curCharacterIndex = SetCharacterActive(curDialogue.characterName);
        
        characterNameText.text = curDialogue.characterName;
        dialogueText.text = "";

        typingCoroutine = StartCoroutine(TypeSentence_Regex(curDialogue));
    }

    private void EndDialogue()
    {
        if (endCoroutine != null)
        {
            return;
        }

        curCharacterIndex = 0;
        dialogueIsOn = false;
        IsRunning = false;

        StopAllCoroutines();
        OnDialogueEnd?.Invoke();
        characterNameText.text = "";
        dialogueText.text = "";
        endCoroutine = StartCoroutine(IntroOutroAnimation(false));
    }

    private IEnumerator TypeSentence_Regex(Dialogue dialogue)
    {
        string conversation = dialogue.conversation;
        DialogueCharacterVisual curCharacter = characters[curCharacterIndex];

        Regex waitRegex = new Regex(@"\$wait/([\d.]+)");
        Regex playRegex = new Regex(@"\$play/(\w+)/([\d.]+)");
        Regex emotionRegex = new Regex(@"\$emotion/(\w+)");

        for (int i = 0; i < conversation.Length; i++)
        {
            char letter = conversation[i];

            if (letter == '$' && i + 9 <= conversation.Length) // Modified condition
            {
                // Check for $wait/
                if (conversation.Substring(i, 6) == "$wait/")
                {
                    // Extract the time value using regular expression
                    Match match = waitRegex.Match(conversation.Substring(i));
                    if (match.Success && float.TryParse(match.Groups[1].Value, out float waitTime))
                    {
                        yield return new WaitForSeconds(waitTime - textSpeed);
                    }

                    int endIndex = i + match.Length - 1;
                    i = endIndex; // Skip the special keyword and time value
                }
                else if (conversation.Substring(i, 6) == "$play/")
                {
                    // Extract the animation string and time value using regular expression
                    Match match = playRegex.Match(conversation.Substring(i));
                    if (match.Success && match.Groups.Count == 3 &&
                        float.TryParse(match.Groups[2].Value, out float playTime))
                    {
                        string animationString = match.Groups[1].Value;
                        // Perform the desired action for $play/
                        // For example, you can call a method to play an animation
                        //PlayAction(animationString, playTime);

                        AnimationPattern animPattern = GetAnimation(animationString);
                        curCharacter.Animate(animPattern, playTime);
                    }

                    int endIndex = i + match.Length - 1;
                    i = endIndex; // Skip the special keyword, animation string, and time value
                }
                else if (conversation.Substring(i, 9) == "$emotion/")
                {
                    // Extract the emotion name using regular expression
                    Match match = emotionRegex.Match(conversation.Substring(i));
                    if (match.Success && match.Groups.Count == 2)
                    {
                        string emotionName = match.Groups[1].Value;
                        // Perform the desired action for $emotion/
                        // For example, you can change the character's emotion
                        //ChangeEmotion(emotionName);

                        curCharacter.SetImageSprite(emotionName);
                    }

                    int endIndex = i + match.Length - 1;
                    i = endIndex; // Skip the special keyword and emotion name
                }

                // Check if there is a space after the regex
                if (i + 1 < conversation.Length && conversation[i + 1] == ' ')
                {
                    conversation = conversation.Remove(i + 1, 1);
                }
            }
            else
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(textSpeed);
            }
        }

        curCharacter.StopAnimate();
        typingCoroutine = null;
    }

    private int SetCharacterActive(string talkingCharacter)
    {
        int index = 0;

        for (int i = 0; i < characters.Count; i++)
        {
            bool isCharacter = characters[i].spriteData.characterName == talkingCharacter;

            if (isCharacter)
                index = i;

            characters[i].SetImageActive(isCharacter);
        }

        return index;
    }

    private DialogueCharacterSprite GetCharacterData(string characterName)
    {
        return characterSpDatas.Find(data => data.characterName == characterName);
    }

    private DialogueData GetDialogue(string dialogueName)
    {
        return dialogueDatas.Find(data => data.dialogueName == dialogueName);
    }

    private AnimationPattern GetAnimation(string animationName)
    {
        return animations.Find(data => data.pattern.ToString().ToLower() == animationName);
    }

    private IEnumerator StartSequence()
    {
        yield return IntroOutroAnimation(true);
        dialogueIsOn = true;
        DisplayNextSentence();
    }

    private IEnumerator IntroOutroAnimation(bool isIntro)
    {
        float elapsedTime = 0f;
        float curveTime = dialogueAnimCruve[dialogueAnimCruve.length - 1].time;

        RectTransform c1 = characters[0].transform as RectTransform;
        RectTransform c2 = characters[1].transform as RectTransform;

        if (isIntro)
        {
            characters[0].gameObject.SetActive(true);
            characters[1].gameObject.SetActive(true);
            dialogueBoxRect.gameObject.SetActive(true);
        }

        Vector2 leftOut = new Vector2(-600, 0);
        Vector2 rightOut = new Vector2(600, 0);

        while (elapsedTime < 0.6f)
        {
            float t = 0;

            if (isIntro)
                t = elapsedTime / curveTime;
            else
                t = 1 - (elapsedTime / curveTime);

            // Evaluate the animation curves for x and y positions
            float yPos = dialogueAnimCruve.Evaluate(t);

            // Apply the positions to the rect's anchoredPosition
            dialogueBoxRect.anchoredPosition = new Vector2(dialogueBoxRect.anchoredPosition.x, yPos);
            c1.anchoredPosition = Vector2.Lerp(leftOut, Vector2.zero, t * 1.1f);
            c2.anchoredPosition = Vector2.Lerp(rightOut, Vector2.zero, t * 1.1f);

            elapsedTime += 1f / 60f; // Use a fixed delta time of 1/60th of a second

            yield return new WaitForSecondsRealtime(1f / 60f);
        }

        if (!isIntro)
        {
            characters[0].gameObject.SetActive(false);
            characters[1].gameObject.SetActive(false);
            dialogueBoxRect.gameObject.SetActive(false);

            endCoroutine = null;
        }
    }
}
