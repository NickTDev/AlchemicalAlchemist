using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectiblesManager : MonoBehaviour
{
    public Canvas popupCanvas;
    public TextMeshProUGUI sample;

    List<TextMeshProUGUI> currentPopups;


    // Start is called before the first frame update
    void Start()
    {
        currentPopups = new List<TextMeshProUGUI>();
        sample.enabled = false;
    }

    public void Popup(Collectible ingredient) //called by Collect()
    {
        TextMeshProUGUI newPopup = Instantiate(sample, sample.transform.position, Quaternion.identity);
        newPopup.enabled = true;
        newPopup.text = "Collected " + ingredient.ingredientText.text;
        newPopup.transform.SetParent(popupCanvas.transform);
        currentPopups.Add(newPopup);
        
        StartCoroutine(DestroyPopup(newPopup));
    }

    IEnumerator DestroyPopup(TextMeshProUGUI toRemove)
    {
        yield return new WaitForSeconds(2.0f);
        currentPopups.Remove(toRemove);
        Destroy(toRemove.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(TextMeshProUGUI popup in currentPopups)
        {
            float newPos = sample.transform.position.y + (currentPopups.IndexOf(popup) * 50);
            popup.transform.position = new Vector3(sample.transform.position.x, newPos, sample.transform.position.z);
        }
    }
}
