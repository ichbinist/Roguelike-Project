using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : Singleton<RoomManager>
{
    [HideInInspector]
    public UnityEvent OnRoomsLocked = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnRoomsUnlocked = new UnityEvent();
    [HideInInspector]
    public UnityEvent<RoomSettings> OnRoomEntered = new UnityEvent<RoomSettings>();

    private DungeonGenerator dungeonGenerator;
    public DungeonGenerator DungeonGenerator { get { return (dungeonGenerator == null) ? dungeonGenerator = GetComponent<DungeonGenerator>(): dungeonGenerator; } }
}