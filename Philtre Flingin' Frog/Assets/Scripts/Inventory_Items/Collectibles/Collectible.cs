using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Collectible : MonoBehaviour
{
    public string collectibleName;
    public RecipeManager.primaryElement prmElm;
    public RecipeManager.secondaryElement scdElm;
    public int count;

    public Canvas collectibleCanvas;
    public TextMeshProUGUI ingredientText;

    GameObject player;
    [SerializeField] GameObject particleEffect;
    [SerializeField] collectibleType colType;
    bool respawnable = true;
    bool collected = false;


    public enum collectibleType
    {
        INGREDIENT,
        ARTIFACT,
        TELEPORTATION_INGREDIENT,
        NUM_COLLECTIBLE_TYPES
    }

    void Start()
    {
        Deselect();
        FindPlayer();
        ingredientText.text = collectibleName;
        GameEvents.current.onPlayerRespawn += FindPlayer;
        GameEvents.current.onPlayerRespawn += Respawn;
        SpawnParticles();
    }

    void FindPlayer(bool yes = true)
    {
        player = GameObject.FindWithTag("Player");
    }

    private void SpawnParticles()
    {
        GameObject particles = Instantiate(particleEffect, transform.position, Quaternion.identity);
        particles.GetComponent<ParticleEffectScript>().Initialize(RecipeManager.colorDictionary[prmElm], false);
        particles.transform.parent = transform;
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);
        ingredientText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        if (player != null && Vector3.Distance(player.transform.position, this.gameObject.transform.position) > player.GetComponent<CollectibleInteraction>().range)
        {
            Deselect();
        }
    }

    public void Select()
    {
        if(!collectibleCanvas.gameObject.activeSelf)
        {
            collectibleCanvas.gameObject.SetActive(true);
        }
        

    }

    public void Deselect()
    {
        if (collectibleCanvas.gameObject.activeSelf)
        {
            collectibleCanvas.gameObject.SetActive(false);
        }
    }

    public bool Compare(Collectible other)
    {
        if(this.collectibleName == other.collectibleName && this.prmElm == other.prmElm && this.count == other.count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public collectibleType GetCollectibleType()
    {
        return colType;
    }

    public void SetRespawnable(bool respawn)
    {
        respawnable = respawn;
    }

    public void Despawn()
    {
        collected = true;
        transform.position = new Vector3(transform.position.x, transform.position.y - 100, transform.position.z);
    }
    public void Respawn(bool yes = true)
    {
        if (collected)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
            collected = false;
        }
    }

    public virtual void Collect()
    {
        HandleCollection();
    }
    protected void HandleCollection()
    {
        if(respawnable)
        {
            Despawn();
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
