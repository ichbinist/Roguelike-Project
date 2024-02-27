using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomSettings : MonoBehaviour
{
    [FoldoutGroup("Room Settings")]
    public RoomType RoomType;
    [FoldoutGroup("Room Settings")]
    public GameObject Ceiling;
    //[FoldoutGroup("Room Settings")]
    //public GameObject PlanarReflection;
    [FoldoutGroup("Room Settings/Door Settings")]
    public float DoorMovementSpeed = 4f;
    [FoldoutGroup("Room Settings/Door Settings")]
    public float PlayerDetectionDistance = 12f;

    [FoldoutGroup("Statistics")]
    [ReadOnly]
    public List<CharacterSettings> Enemies = new List<CharacterSettings>();
    [FoldoutGroup("Statistics")]
    [ReadOnly]
    public List<DoorBehaviour> Doors = new List<DoorBehaviour>();
    [FoldoutGroup("Statistics")]
    [ReadOnly]
    public bool IsPlayerInside;
    [FoldoutGroup("Statistics")]
    [ReadOnly]
    public bool IsRoomLocked;

    private CharacterSettings playerCharacter;
    public CharacterSettings PlayerCharacter { get { return (playerCharacter == null) ? playerCharacter = CombatGameManager.Instance.PlayerCharacter : playerCharacter; } }

    private BoxCollider boxCollider;
    public BoxCollider BoxCollider { get { return (boxCollider == null) ? boxCollider = GetComponent<BoxCollider>() : boxCollider; } }

    [FoldoutGroup("Room Settings/Reward Settings")]
    [ShowIf("IsNotLobby")]
    public List<PickUpController> Rewards = new List<PickUpController>();
    [FoldoutGroup("Room Settings/Reward Settings")]
    [ShowIf("IsNotLobby")]
    public int RewardCount = 0;
    [FoldoutGroup("Room Settings/Reward Settings")]
    [ShowIf("IsNotLobby")]
    public float RewardProbability = 50f; // %'lik Ýhtimal
    [FoldoutGroup("Room Settings/Reward Settings")]
    [ShowIf("IsNotLobby")]
    public Transform RewardHolder;
    [HideInInspector]
    public UnityEvent<CharacterSettings> OnEnemyKilled = new UnityEvent<CharacterSettings>();
    [HideInInspector]
    public UnityEvent OnEnteredRoom = new UnityEvent();
    [ShowIf("IsNotLobby")]
    public AudioClip UnlockRoomClip;
    [ShowIf("IsLobby")]
    public TMPro.TextMeshProUGUI LobbyText;
    [ShowIf("IsBossRoom")]
    public GameObject NextFloorObject;

    private bool IsNotLobby { get { return RoomType != RoomType.Lobby; } }
    private bool IsLobby { get { return RoomType == RoomType.Lobby; } }
    private bool IsBossRoom { get { return RoomType == RoomType.Boss_Combat; } }

    private void OnEnable()
    {
        OnEnemyKilled.AddListener(CheckCombat);
        RoomManager.Instance.OnRoomsUnlocked.AddListener(UnlockRoom);
        RoomManager.Instance.OnRoomsLocked.AddListener(LockRoom);
    }

    private void OnDisable()
    {
        OnEnemyKilled.RemoveListener(CheckCombat);
        RoomManager.Instance.OnRoomsUnlocked.RemoveListener(UnlockRoom);
        RoomManager.Instance.OnRoomsLocked.RemoveListener(LockRoom);
    }

    private void CheckCombat(CharacterSettings characterSettings)
    {
        if (Enemies.Contains(characterSettings))
        {
            Enemies.Remove(characterSettings);

            if(Enemies.Count == 0)
            {
                UnlockRoom();
                SpawnReward();
                if (IsBossRoom)
                {
                    NextFloorObject.SetActive(true);
                }
            }
        }
    }

    public void UnlockRoom()
    {
        if(IsRoomLocked == true)
        {
            IsRoomLocked = false;

            foreach (DoorBehaviour door in Doors)
            {
                door.IsDoorControlable = true;
            }
            if (IsPlayerInside && !IsThereCombat())
            {
                RoomManager.Instance.OnRoomsUnlocked.Invoke();
                AudioPoolManager.Instance.PlaySound(UnlockRoomClip, 1f);
                DespawnEnemies();
            }
        }
    }

    public void SpawnReward()
    {
        float chance = Random.Range(0f, 100f);
        
        if(chance <= RewardProbability)
        {
            if(Rewards.Count > 0)
            {
                List<PickUpController> localRewards = new List<PickUpController>();
                localRewards.AddRange(Rewards);
                //localRewards.RemoveAll(x => (x as PowerPickUpController) != null && (x as PowerPickUpController).ID == CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterPowerController>().CurrentPowerID);
                for(int i = 0; i < RewardCount; i++)
                {
                    Instantiate(localRewards[Random.Range(0, localRewards.Count - 1)], RewardHolder);
                }
            }
        }
    }

    public void LockRoom()
    {
        if(IsRoomLocked == false)
        {
            IsRoomLocked = true;

            foreach (DoorBehaviour door in Doors)
            {
                door.IsDoorControlable = false;
            }

            if(IsPlayerInside && IsThereCombat())
            {
                RoomManager.Instance.OnRoomsLocked.Invoke();
                SpawnEnemies();
            }
        }
    }

    private void SpawnEnemies()
    {
        foreach(CharacterSettings character in Enemies)
        {
            (character as EnemyCharacterSettings).Spawn();
        }
    }

    private void DespawnEnemies()
    {
        foreach (CharacterSettings character in Enemies)
        {
            (character as EnemyCharacterSettings).Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == PlayerCharacter.gameObject)
        {
            IsPlayerInside = true;
            Ceiling.SetActive(false);
            //PlanarReflection.SetActive(true);
            OnEnteredRoom.Invoke();
            RoomManager.Instance.OnRoomEntered.Invoke(this);

            if(LobbyText!= null && LobbyText.text == string.Empty)
            {
                LobbyText.SetText(FloorManager.Instance.Company.CompanyName + "\n" + FloorManager.Instance.GetCurrentFloor().SectionName + "\n" + FloorManager.Instance.GetCurrentFloor().FloorIndexName);
            }

            if (IsThereCombat())
            {
                LockRoom();
            }
            else
            {
                UnlockRoom();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerCharacter.gameObject)
        {
            Ceiling.SetActive(true);
            //PlanarReflection.SetActive(false);
            IsPlayerInside = false;
        }
    }

    public bool IsThereCombat()
    {
        return Enemies.Count > 0;
    }

    public Vector4 GetRoomDimensionsAsWorldPosition()
    {
        Vector4 dimensions = Vector4.zero;
        dimensions = new Vector4(BoxCollider.bounds.min.x, BoxCollider.bounds.min.z, BoxCollider.bounds.max.x, BoxCollider.bounds.max.z);
        return dimensions;
    }
}

public enum RoomType
{
    Lobby,
    Combat,
    Boss_Combat
}