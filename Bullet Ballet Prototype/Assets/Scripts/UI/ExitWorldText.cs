using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitWorldText : MonoBehaviour
{
    public Material mat;
    Color color;

    public List<GameObject> enemies = new List<GameObject>();

    private void Start()
    {
        if(mat == null) {
            mat = GetComponent<MeshRenderer>().material;
        }
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        color = mat.color;
        color.a = 0;
        mat.color = color;
    }

    void Update()
    {
        enemies.RemoveAll(item => item == null);

        if (enemies.Count <= 0 && color.a <= 1)
        {
            color.a += Time.deltaTime;

        }
        mat.color = color;
        Debug.Log(color.a);
    }
}
