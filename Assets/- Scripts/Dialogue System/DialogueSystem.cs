using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MyGame.Dialogue
{
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(2, 5)]
        public string text;

        public AudioClip voiceClip;
        public UnityEvent onDialogueEvent;

        [Header("Branching Choice")]
        public bool hasChoices = false;

        public string choiceQuestion;
        public string choiceA;
        public string choiceB;

        public UnityEvent onChoiceA;
        public UnityEvent onChoiceB;
    }

    public class DialogueSystem : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [SerializeField] private DialogueLine[] dialogueLines;
        [SerializeField] private float dialogueInterval = 2f;
        [SerializeField] private float typingSpeed = 0.04f;

        [Header("World Space UI")]
        [SerializeField] private Canvas dialogueCanvas;
        [SerializeField] private TextMeshProUGUI dialogueText;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;

        [Header("Choice UI")]
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private TextMeshProUGUI choiceQuestionText;
        [SerializeField] private Button choiceAButton;
        [SerializeField] private Button choiceBButton;
        [SerializeField] private TextMeshProUGUI choiceAText;
        [SerializeField] private TextMeshProUGUI choiceBText;

        private int currentDialogueIndex = 0;
        private Coroutine dialogueCoroutine;

        void Start()
        {
            dialogueCanvas.worldCamera = Camera.main;

            choicePanel.SetActive(false);
            dialogueCoroutine = StartCoroutine(PlayDialogue());
        }

        IEnumerator PlayDialogue()
        {
            while (currentDialogueIndex < dialogueLines.Length)
            {
                DialogueLine line = dialogueLines[currentDialogueIndex];

                // Play voice clip
                if (line.voiceClip != null)
                    audioSource.PlayOneShot(line.voiceClip);

                // Type text
                yield return StartCoroutine(TypeDialogue(line.text));

                // Trigger normal dialogue event
                line.onDialogueEvent.Invoke();

                // == HANDLE BRANCHING ==
                if (line.hasChoices)
                {
                    yield return StartCoroutine(ShowChoices(line));
                }

                // Wait for voice clip to finish
                if (line.voiceClip != null)
                    yield return new WaitWhile(() => audioSource.isPlaying);

                currentDialogueIndex++;
                yield return new WaitForSeconds(dialogueInterval);
            }
        }

        IEnumerator TypeDialogue(string text)
        {
            dialogueText.text = "";

            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        IEnumerator ShowChoices(DialogueLine line)
        {
            // Activate UI
            choicePanel.SetActive(true);

            choiceQuestionText.text = line.choiceQuestion;
            choiceAText.text = line.choiceA;
            choiceBText.text = line.choiceB;

            bool choiceMade = false;

            // Remove old listeners
            choiceAButton.onClick.RemoveAllListeners();
            choiceBButton.onClick.RemoveAllListeners();

            // UI Button Choice A
            choiceAButton.onClick.AddListener(() =>
            {
                if (!choiceMade)
                {
                    choiceMade = true;
                    choicePanel.SetActive(false);
                    line.onChoiceA.Invoke();
                }
            });

            // UI Button Choice B
            choiceBButton.onClick.AddListener(() =>
            {
                if (!choiceMade)
                {
                    choiceMade = true;
                    choicePanel.SetActive(false);
                    line.onChoiceB.Invoke();
                }
            });

            // Await player key input OR UI clicks
            while (!choiceMade)
            {
                // Keybind: E → Choice A
                if (Input.GetKeyDown(KeyCode.E))
                {
                    choicePanel.SetActive(false);
                    line.onChoiceA.Invoke();
                    choiceMade = true;
                }

                // Keybind: F → Choice B
                if (Input.GetKeyDown(KeyCode.F))
                {
                    choicePanel.SetActive(false);
                    line.onChoiceB.Invoke();
                    choiceMade = true;
                }

                yield return null;
            }
        }

    }
}
