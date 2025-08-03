using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Cursor = UnityEngine.Cursor;

public class Inspect : MonoBehaviour
{
    public GameObject offset;
    public TextMeshProUGUI inspectText; // Assign this in the inspector
    public TextMeshProUGUI inspectText2;

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public GameObject itemInfo;
    public GameObject bgPanel;

    public float rotationSpeed = 1.0f; // Adjustable from Inspector
    public MonoBehaviour scriptToToggle; // Drag your target script here via Inspector
    public MonoBehaviour scriptToToggle2; // Drag your target script here via Inspector
    public GameObject crosshair;

    private Player player;
    private PlayerInput playerInput;

    private Transform examinedObject;
    private Vector3 lastMousePosition;
    private bool isExamining = false;
    public bool IsExamining => isExamining;

    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    void Start()
    {
        player = FindFirstObjectByType<Player>();
        playerInput = player.GetComponent<PlayerInput>();
        inspectText.text = "";
        inspectText2.text = "";
    }

    void Update()
    {
        if (!player.isHandsFree)
        {
            Transform currentItem = player.GetCurrentItem();
            Items itemScript = currentItem.GetComponent<Items>();

            inspectText.text = isExamining ? "Back: 'F'" : "Inspect: 'F'";
            inspectText2.text = "Throw: 'Q'";

            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleExamination();

                if (isExamining)
                {
                    examinedObject = currentItem;

                    // Reset rotation on inspection start
                    examinedObject.localRotation = Quaternion.identity;

                    // Show item info
                    if (itemScript != null)
                    {
                        itemNameText.text = itemScript.itemName;
                        itemDescriptionText.text = itemScript.itemDescription;
                    }

                    // Show info panel and label
                    if (itemInfo != null) itemInfo.SetActive(true);
                    if (bgPanel != null) bgPanel.SetActive(true);

                    if (!originalPositions.ContainsKey(examinedObject))
                    {
                        originalPositions[examinedObject] = examinedObject.localPosition;
                        originalRotations[examinedObject] = examinedObject.localRotation;
                    }
                }
                else
                {
                    // Clear info when done inspecting
                    itemNameText.text = "";
                    itemDescriptionText.text = "";
                    if (itemInfo != null) itemInfo.SetActive(false);
                    if (bgPanel != null) bgPanel.SetActive(false);
                }
            }

            if (isExamining)
            {
                Examine();
                StartExamination();
            }
            else
            {
                NonExamine();
                StopExamination();
            }
        }
        else
        {
            inspectText.text = "";
            inspectText2.text = "";
        }
    }

    void ToggleExamination()
    {
        isExamining = !isExamining;
    }

    void StartExamination()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerInput.enabled = false;
        lastMousePosition = Input.mousePosition;

        if (scriptToToggle != null)
            scriptToToggle.enabled = false;
        scriptToToggle2.enabled = false;
        crosshair.SetActive(false);
    }

    void StopExamination()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput.enabled = true;

        if (scriptToToggle != null)
            scriptToToggle.enabled = true;
        scriptToToggle2.enabled = true;
        crosshair.SetActive(true);
    }

    void Examine()
    {
        if (examinedObject != null)
        {
            examinedObject.position = Vector3.Lerp(examinedObject.position, offset.transform.position, 0.2f);

            if (Input.GetMouseButton(0))
            {
                Vector3 deltaMouse = Input.mousePosition - lastMousePosition;

                examinedObject.Rotate(Vector3.up, deltaMouse.x * rotationSpeed, Space.World);
                examinedObject.Rotate(Vector3.right, deltaMouse.y * rotationSpeed, Space.Self);
            }

            lastMousePosition = Input.mousePosition;
        }
    }

    void NonExamine()
    {
        if (examinedObject != null && originalPositions.ContainsKey(examinedObject))
        {
            examinedObject.localPosition = originalPositions[examinedObject];
            examinedObject.localRotation = originalRotations[examinedObject];

            originalPositions.Remove(examinedObject);
            originalRotations.Remove(examinedObject);
        }
    }
}
