using UnityEngine;

public class Combo1_PM : MonoBehaviour
{
    [Header("Puzzle Cubes")]
    [SerializeField] CubeRotation[] cubes;

    [Header("Drawer System")]
    [SerializeField] DrawerSlide drawer;

    private bool puzzleSolved = false;

    void OnEnable()
    {
        CubeRotation.OnCubeRotated += CheckPuzzleCompletion;
    }

    void OnDisable()
    {
        CubeRotation.OnCubeRotated -= CheckPuzzleCompletion;
    }

    void CheckPuzzleCompletion()
    {
        if (puzzleSolved) return;

        foreach (var cube in cubes)
        {
            if (!cube.IsCorrectlyAligned())
                return;
        }

        // All cubes aligned correctly
        puzzleSolved = true;
        drawer.UnlockDrawer();
    }
}
