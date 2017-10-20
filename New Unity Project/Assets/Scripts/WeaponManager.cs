using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    public List<Weapon> weaponList = new List<Weapon>();

	public class Weapon
    {
        public int weaponId = 0;
        public string name = "name";
        public int range = 0;
        public int minDamage = 1;
        public int maxDamage = 6;
    }

    private void Start()
    {
        weaponList.Add(new Weapon() { weaponId = 0, name = "Bow", minDamage = 1, maxDamage = 4, range = 6 });
        weaponList.Add(new Weapon() { weaponId = 1, name = "Sword", minDamage = 1, maxDamage = 6, range = 1 });
    }
}
