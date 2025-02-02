using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TileVFXManager : MonoBehaviour
{
    public GameObject explosionPrefab; // Assign in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayExplosionEffect()
    {
        GameObject explosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);

        Destroy(explosion, 1f);
    }

}
