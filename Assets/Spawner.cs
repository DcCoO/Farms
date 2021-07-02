using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject farm, field;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Instantiate(farm);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Instantiate(field);
        }
    }
}
