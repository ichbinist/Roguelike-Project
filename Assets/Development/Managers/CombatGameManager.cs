using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CombatGameManager : Singleton<CombatGameManager>
{
    [ReadOnly]
    public bool isPaused = false;
    [ReadOnly]
    public bool isDead = false;

    [HideInInspector]
    public UnityEvent OnGameReset = new UnityEvent();

    private RoomSettings currentRoom;
    public RoomSettings CurrentRoom { get { return (currentRoom == null || IsPlayerOutsideCurrentRoom()) ? currentRoom = GetNearestRoom() :  currentRoom; } }

    private CharacterSettings playerCharacter;
    public CharacterSettings PlayerCharacter { get { return (playerCharacter == null) ? playerCharacter = FindPlayerCharacter() : playerCharacter; } }

    private bool IsPlayerOutsideCurrentRoom()
    {
        return Vector3.Distance(PlayerCharacter.transform.position, currentRoom.transform.position) > RoomManager.Instance.DungeonGenerator.offset.x / 2f;
    }

    private RoomSettings GetNearestRoom()
    {
        return RoomManager.Instance.DungeonGenerator.InstantiatedRooms
            .OrderBy(x => Vector3.Distance(PlayerCharacter.transform.position, x.transform.position))
            .First()
            .GetComponent<RoomSettings>();
    }

    public void ResetGame()
    {
        //This is the basic version of everything.

        PlayerCharacter.CurrentHealth = PlayerCharacter.MaximumHealth;
        PlayerCharacter.CurrentAmmo = ItemManager.Instance.GetInventoryItemAsWeapon(PlayerCharacter.GetComponent<CharacterWeaponController>().CurrentWeaponID).AmmoCapacity;
        PlayerCharacter.transform.position = Vector3.zero;
        PlayerCharacter.GetComponent<CharacterWeaponController>().EquipWeapon("Pistol");
        PlayerCharacter.GetComponent<CharacterPowerController>().LocalCooldown = 0;
        PlayerCharacter.GetComponent<CharacterPowerController>().CurrentPowerID = null;
        RoomManager.Instance.DungeonGenerator.ResetMaze();
        MapManager.Instance.StartCoroutine(MapManager.Instance.CreateMapCoroutine());
        FloorManager.Instance.ResetFloors();
        OnGameReset.Invoke();
    }

    private CharacterSettings FindPlayerCharacter()
    {
        List<CharacterSettings> AllCharacters = new List<CharacterSettings>();
        CharacterSettings[] settingsArray = FindObjectsOfType<MonoBehaviour>().OfType<CharacterSettings>().ToArray();
        AllCharacters.AddRange(settingsArray);
        return AllCharacters.Find(x => x.CharacterType == CharacterType.Player);
    }
}
