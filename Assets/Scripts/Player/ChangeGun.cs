using UnityEngine;

public class ChangeGun : MonoBehaviour
{
    public GameObject weapon1;
    public GameObject weapon2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(2);
        }
    }

    void SwitchWeapon(int weaponNumber)
    {
        if (weaponNumber == 1)
        {
            weapon1.SetActive(true);
            weapon2.SetActive(false);
        }
        else if (weaponNumber == 2)
        {
            weapon1.SetActive(false);
            weapon2.SetActive(true);
        }
    }
}
