using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogTrigger:MonoBehaviour
{
    [SerializeField] private GameObject[] ToDisable;
    [SerializeField] private GameObject toAct;
    [SerializeField] private Transform standingPoint;

    private Transform avatar;

    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            avatar = other.transform;

            DialogOpen(true);
            
            await Task.Delay(50);

            avatar.SetLocalPositionAndRotation(standingPoint.position, standingPoint.rotation);
        }
    }

    public void DialogOpen(bool IsOpen)
    {
        avatar.GetComponent<PlayerInput>().enabled = !IsOpen;
        foreach (GameObject obj in ToDisable) { obj.SetActive(!IsOpen); }
        toAct.SetActive(IsOpen);
        Cursor.visible = IsOpen;
        Cursor.lockState = IsOpen?CursorLockMode.None:CursorLockMode.Locked;
    }
}