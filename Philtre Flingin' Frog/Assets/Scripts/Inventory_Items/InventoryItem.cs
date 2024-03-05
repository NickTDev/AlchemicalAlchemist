using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public enum itemState
    {
        IN_SLOT,
        SELECTED,
        IN_RECIPE
    }
    [SerializeField] protected string itemName = "NULL NAME";
    [SerializeField] string itemSpecs = "NULL SPECS";
    [SerializeField] string itemFlavor = "NULL FLAVOR";
    [SerializeField] protected InventoryScript.InventoryType itemtype;
    [SerializeField] protected GameObject elementDisplaySprite; //the sprite that displays this items secondary type (and primary if ingredient)
    protected string hierarchyParentName = "Item_Parent";
    protected itemState currentState = itemState.IN_SLOT;
    protected int itemInventoryX;
    protected int itemInventoryY;
    [SerializeField] protected TMP_Text amount_display;
    protected int amount = 0;
    bool initialized = false;
    protected PlayerInventory playerInventory;
    [SerializeField] bool unlimited = false;
    [Header("Menu SFX")]
    [SerializeField] FMODUnity.EventReference interactSFX;
    [SerializeField] FMODUnity.EventReference confirmSFX;
    [SerializeField] FMODUnity.EventReference cancelSFX;
    float clickCooldown = 0.25f;
    float clickTimer = 0.0f;
    protected InventoryItemScrollOver scrollOver;
    [SerializeField] float itemScale; //because different monitors are real silly stupid

    protected bool destroyOnZero = false;//only potions self destruct when none available

    public void SetScale(float scale = 0f)
    {
        if(scale != 0f) //if script passed in a scale, uses that
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else //otherwise uses serialized field
        {
            transform.localScale = new Vector3(itemScale, itemScale, itemScale);
        }
    }

    void Start()
    {
        FindPlayer(true);
        GameEvents.current.onPlayerRespawn += FindPlayer;
        Initialize();
    }

    void FindPlayer(bool yes)
    {
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
    }

    protected void Initialize()
    {
        if(!initialized)
        {
            //and the scrollover text
            scrollOver = GetComponent<InventoryItemScrollOver>();
            string[] hoverTexts = { itemName, itemFlavor };
            scrollOver.SetTexts(hoverTexts);
            //finishes
            initialized = true;
            InitElementSprite();
        }
        InitItemPosition();
    }

    public void ScaleUp()//to make sure these scale up on bigger screen sizes
    {
        float scale = Screen.width / 1920f;
        //transform.localScale = new Vector3(scale, scale, scale);
    }

    protected virtual void InitItemPosition() { } //determines the items position in the inventory Arrays based on different factors per item type
    protected virtual void InitElementSprite() { } //determines the element sprite's shapes & color

    void Update()
    {
        FindPlayer(true);
        clickTimer += Time.deltaTime;
        SetAmountText();
        if (currentState == itemState.SELECTED)
        {
            transform.position = Input.mousePosition;
            scrollOver.WhileSelected(); //hover text doesn't display while selected
        }
        CheckForDestroy();//only used for potions
    }

    void SetAmountText() //sets the attached text to the correct value based on current state
    {
        if(unlimited)
        {
            amount_display.text = "∞";
        }
        else if(currentState == itemState.IN_SLOT) //while in the inventory slot uses inventory amount
        {
            amount_display.text = "" + GetInventoryAmount().ToString();
        }
        else
        {
            amount_display.text = "" + amount.ToString();
        }
    }

    public void Click()
    {
        FMOD.Studio.EventInstance e = AudioEngineManager.CreateSound(interactSFX, 1f);
        e.start();

        if(clickTimer > clickCooldown)
        {
            if (currentState == itemState.IN_SLOT) //selects if not SELECTED
            {
                if (GetInventoryAmount() > 0)
                {
                    Select();
                    clickTimer = 0.0f;
                }
            }
            else if (currentState == itemState.SELECTED) //if SELECTED, checked whether you can put in slot
            {
                ClickedWhileSelected();
            }
            else if (currentState == itemState.IN_RECIPE) //if in recipe, returns to inventory on right click
            {
                if (Input.GetMouseButton(1))
                {
                    Deselect();
                }
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        FMOD.Studio.EventInstance e = AudioEngineManager.CreateSound(interactSFX, 1f);
        e.start();
        if (eventData.button == PointerEventData.InputButton.Right && (currentState == itemState.SELECTED || currentState == itemState.IN_RECIPE) && clickTimer > clickCooldown)
        {
            Deselect();
            clickTimer = 0.0f;
        }
    }

    protected virtual void ClickedWhileSelected() { } //deals with what happens when the item is clicked while it is SELECTED

    protected virtual void Select() //creates a copy of this item that the player can manipulate
    {
    }

    public virtual void Deselect() //destroys and returns to inventory
    {
    }

    protected bool RectOverlaps(RectTransform recttrans1, RectTransform recttrans2, Vector3 scale1 = default(Vector3), Vector3 scale2 = default(Vector3))
    {
        if(scale1 == default(Vector3))
        {
            scale1 = new Vector3(1f, 1f, 1f);
            scale2 = new Vector3(1f, 1f, 1f);
        }
        Vector3 pos1 = transform.TransformPoint(recttrans1.position);
        Vector3 pos2 = transform.TransformPoint(recttrans2.position);
        Rect rect1 = new Rect(pos1.x, pos1.y, recttrans1.rect.width * scale1.x, recttrans1.rect.height * scale1.y);
        Rect rect2 = new Rect(pos2.x, pos2.y, recttrans2.rect.width * scale2.x, recttrans2.rect.height * scale2.y);
        Debug.Log("size = " + rect1.width);
        return rect1.Overlaps(rect2, true);
    }
    protected bool ItemInSlot(GameObject slot) //the normal rect overlaps got fucked up by the scaling changes
    {
        Vector3 pos1 = transform.position;
        Vector3 pos2 = slot.transform.position;
        Rect rect1 = new Rect(pos1.x, pos1.y, 100, 100);
        Rect rect2 = new Rect(pos2.x, pos2.y, 100, 100);
        return rect1.Overlaps(rect2, true);
    }

    protected virtual void ModifyInventory(int amnt)//changes the amount of this item in the players inventory
    {
    }
    protected virtual GameObject FindHomeObject()//use when SELECTED: finds the copy of this item in the inventory itself
    {
        return null;
    }

    private void CheckForDestroy() //potions will self destruct on reaching 0
    {
        if (currentState == itemState.IN_SLOT) //while in the inventory slot, uses inventory amount
        {
            amount = GetInventoryAmount();
        }
        if (amount <= 0 && destroyOnZero && !unlimited)
        {
            Destroy(gameObject);
        }
    }

    protected void SetState(itemState newState)
    {
        currentState = newState;
    }
    public itemState GetCurrentState()
    {
        return currentState;
    }
    void SetAmount(int amnt)
    {
        amount = amnt;
    }
    public void AddAmount(int amnt)
    {
        amount += amnt;
    }
    public int GetAmount()
    {
        return amount;
    }
    public string GetName()
    {
        return itemName;
    }
    public string GetSpecs()
    {
        return itemSpecs;
    }
    public string GetFlavor()
    {
        return itemFlavor;
    }
    public bool GetUnlimited()
    {
        return unlimited;
    }

    public Sprite GetItemSprite()
    {
        return transform.GetChild(0).GetComponent<Image>().sprite;
    }

    protected virtual int GetInventoryAmount() { return -1; }//returns the amount of this item in the inventory

}