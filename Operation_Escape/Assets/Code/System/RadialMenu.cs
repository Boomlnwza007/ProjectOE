using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    private Camera mainCam;
    public Transform center;
    public Transform selectObject;
    public float offset;
    public float radius;
    public List<GameObject> itemPrefab = new List<GameObject>();
    public int numberOfItems;

    public GameObject Wheel;
    [SerializeField] private PlayerCombat Gun;

    bool isMenuActive;
    private List<BaseGun> items;
    private int select;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        isMenuActive = false;
        this.items = Gun.gunList;
        
        //CreateItems();
        Wheel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (items.Count <= 0)
        {
            return;
        }

        if (Input.GetButtonDown("Wheel"))
        {
            CreateItems();
            Wheel.SetActive(true);
            isMenuActive = true;
        }

        if (Input.GetButtonUp("Wheel"))
        {
            Gun.SwapGun(select);
            //Debug.Log(select);
            Wheel.SetActive(false);
            isMenuActive = false;
        }

        if (isMenuActive)
        {
            numberOfItems = items.Count;
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector2 delta = mousePos - center.position;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            angle += (360 / numberOfItems)/2;
            angle = (angle + 360) % 360;
            //Debug.Log(angle);

            selectObject.rotation = Quaternion.Euler(0, 0, angle);
            int segment = Mathf.FloorToInt(angle / (360 / numberOfItems));
            selectObject.rotation = Quaternion.Euler(0, 0, segment * (360 / numberOfItems));
            GameObject selectedItem = items[segment].gunPrefab;

            select = segment;
            //Debug.Log(segment + " : "+ select);

            HighlightSelectedItem(selectedItem);
        }

        
    }

    void CreateItems()
    {
        if (items.Count <= itemPrefab.Count)
        {
            return;
        }

        foreach (GameObject item in itemPrefab)
        {
            Destroy(item);
        }
        itemPrefab.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            float angle = i * (360f / items.Count);
            Vector3 itemPosition = new Vector3(
                center.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                center.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
                0f
            );
            GameObject newIten = Instantiate(items[i].iconGun, itemPosition, Quaternion.identity, Wheel.transform);
            itemPrefab.Add(newIten);
        }
    }

    void HighlightSelectedItem(GameObject selectedItem)
    {
        // Add your logic to highlight the selected item
        // For example, you can change its color or scale
        foreach (GameObject item in itemPrefab)
        {
            if (item == selectedItem)
            {
                item.transform.localScale = new Vector3(150f, 150f, 0f); // Example highlight: enlarge the selected item
            }
            else
            {
                item.transform.localScale = new Vector3(100f, 100f, 0f); // Reset scale for non-selected items
            }
        }
    }
}
