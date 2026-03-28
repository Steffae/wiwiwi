using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float x;
    public float y;
    public float z;
    public float health;
    public float maxHealth;

    public Vector3 position
    {
        get => new Vector3(x, y, z);
        set
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }
}