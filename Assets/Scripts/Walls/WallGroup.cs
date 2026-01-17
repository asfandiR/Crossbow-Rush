using UnityEngine;

public class WallGroup : MonoBehaviour
{
    public Wall[] walls;
public int MaxWallLevel { get { return walls.Length>0?walls[0].MaxWallLevel:3; } }
    private void Awake()
    {
        walls = GetComponentsInChildren<Wall>();
    }

    public void ActivateWalls()
    {
        foreach (var wall in walls)
        {
            wall.gameObject.SetActive(true);
        }
    }

    public void DeactivateWalls()
    {
        foreach (var wall in walls)
        {
            wall.gameObject.SetActive(false);
        }
    }
    public void UpgradeWalls()
    {
        foreach (var wall in walls)
        {
            // Предполагается, что у стены есть метод для улучшения
            wall.UpgradeWall(); 
        }
    }
}