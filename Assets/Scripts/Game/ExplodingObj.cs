using UnityEngine;

[System.Serializable]
public struct ExplodingObj
{
    public string tileName;        // Es. "Scimmia Rossa", "Bomba"
    public GameObject tilePrefab;  // Il Prefab già pronto con Sprite, Collider, ecc.
    public ColorData colorData;    // Dati sul colore/indice per la verifica degli input
    public Sprite customButtonSprite; // Sprite personalizzata per il pulsante del tile
}

[System.Serializable]
public struct ColorData
{
    public int ID;
    public Color color;
}