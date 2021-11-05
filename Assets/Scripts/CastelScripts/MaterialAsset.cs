/* Класс содержащий набор материалов */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaterialAsset : MonoBehaviour
{
    private static MaterialAsset instance;
    public static MaterialAsset Instance => instance;

    [Header("Значения от меньшего к большему")]
    [SerializeField] private MaterialCollection[] dressMaterials = new MaterialCollection[7];

    private void Awake()
    {
        instance = this;

        //start sorting
        for (int i = 0; i < dressMaterials.Length; i++)
        {
            if (i < (dressMaterials.Length - 1))
            {
                if (i == 0) dressMaterials[i].minPoint = 0;

                dressMaterials[i].maxPoint = dressMaterials[i + 1].minPoint - 1;
            }
            else
                dressMaterials[i].maxPoint = 1000;
        }

    }

    public Material GetRandomMaterials()
    {
        return dressMaterials[UnityEngine.Random.Range(0, dressMaterials.Length - 1)].material;
    }

    public Material SelectColor(int point)
    {
        for (int i = 0; i < dressMaterials.Length; i++)
        {
            if ((point >= dressMaterials[i].minPoint) && (point <= dressMaterials[i].maxPoint))
                return dressMaterials[i].material;
        }

        return dressMaterials[dressMaterials.Length - 1].material;
    }

}

[Serializable]
public class MaterialCollection
{
    public int minPoint;
    [HideInInspector] public int maxPoint;

    public Material material;
}