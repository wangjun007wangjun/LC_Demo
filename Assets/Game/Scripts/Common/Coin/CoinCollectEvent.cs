using UnityEngine;

public class CoinCollectEvent
{
    public Vector2 size;
    public Vector2 position;

    public CoinCollectEvent(Vector2 size, Vector2 position)
    {
        this.size = size;
        this.position = position;
    }
}