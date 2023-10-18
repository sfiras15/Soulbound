using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class Chest : MonoBehaviour,IDataPersistence, IInteractable
{
    [SerializeField] private int chestID;
    [SerializeField] private List<Item> items;
    private InteractableUI interactableUI;

    private GameObject player;

    [SerializeField] private GameObject chestCanvas;
    [SerializeField] private float interactDistance = 2f;
    public void SaveData(ref GameData data)
    {
        if (data.chestDictionary.ContainsKey(chestID))
        {
            data.chestDictionary.Remove(chestID);
        }
        ChestData chestData = new ChestData();

        chestData.chestPosition = this.transform.position;
        chestData.chestRotation = this.transform.eulerAngles;
        //To add , saves the state of the items inside the chest
        data.chestDictionary.Add(chestID, chestData);
    }
    public void LoadData(GameData data)
    {
        data.chestDictionary.TryGetValue(chestID, out ChestData chest);
        if (chest != null)
        {
            this.transform.position = chest.chestPosition;
            this.transform.eulerAngles = chest.chestPosition;

        }
    }

    private void Awake()
    {
        interactableUI = GetComponentInChildren<InteractableUI>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        interactableUI.InitializeItemName("Chest");
    }
    private void Update()
    {
        if (Vector3.Distance(player.transform.position,transform.position) >= interactDistance)
        {
            CloseUI();
        }
    }

    public void ShowUI()
    {
        interactableUI.Show();
    }
    public void HideUI()
    {
        interactableUI.Hide();
    }

    public void Interact()
    {
        OpenUI();
    }

    public void OpenUI()
    {
        chestCanvas.gameObject.SetActive(true);
    }
    public void CloseUI()
    {
        chestCanvas.gameObject.SetActive(false);
    }

    public ChestData ChestInfo()
    {
        ChestData chestData = new ChestData();
        chestData.chestPosition = transform.position;
        chestData.chestRotation = transform.eulerAngles;

        return chestData;

    }
    public int GetChestID
    {
        get { return chestID; }
    }
}
