using UnityEngine;

public class CubeColorChanger : MonoBehaviour
{
    public Renderer cubeRenderer;

    public void SetRed()
    {
        cubeRenderer.material.color = Color.red;
    }

    public void SetBlue()
    {
        cubeRenderer.material.color = Color.blue;
    }
}
