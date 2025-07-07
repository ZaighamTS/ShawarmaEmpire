using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        GameManager.Instance.SaveData();
    }
}
