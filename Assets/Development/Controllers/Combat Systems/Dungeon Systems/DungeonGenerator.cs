using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }

    }

    public Vector2Int size;
    public int startPos = 0;
    public Rule[] rooms;
    public Rule[] Bossrooms;
    public Vector2 offset;

    [ReadOnly]
    public List<RoomBehaviour> InstantiatedRooms = new List<RoomBehaviour>();
    List<Cell> board;

    // Start is called before the first frame update
    void Start()
    {
        MazeGenerator();
    }

    public void ResetMaze()
    {
        foreach (RoomBehaviour room in InstantiatedRooms)
        {
            Destroy(room.gameObject);
        }
        InstantiatedRooms.Clear();
        MazeGenerator();
    }

    void GenerateDungeon()
    {

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 1; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j);

                        if (p == 2)
                        {
                            randomRoom = k;
                            break;
                        }
                        else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                    }

                    if (randomRoom == -1)
                    {
                        if (availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }

                    if(i == 0 && j == 0)
                    {
                        var newRoom = Instantiate(rooms[0].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name += "LOBBY";
                        InstantiatedRooms.Add(newRoom);
                    }
                    else
                    {
                        var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name += " " + i + "-" + j;
                        InstantiatedRooms.Add(newRoom);
                    }
                }
            }
        }

        ReplaceFarthestRoomWithBossRoom();
    }

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k < 1000)
        {
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
            {
                break;
            }

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }

            }

        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell - size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].visited)
        {
            neighbors.Add((cell + 1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell - 1));
        }

        return neighbors;
    }

    void ReplaceFarthestRoomWithBossRoom()
    {
        float maxDistance = 0f;
        RoomBehaviour farthestRoom = null;

        // Iterate through all instantiated rooms to find the farthest one
        foreach (var room in InstantiatedRooms)
        {
            float distance = Vector3.Distance(Vector3.zero, room.transform.position);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestRoom = room;
            }
        }

        if (farthestRoom != null)
        {
            // Calculate cell index based on the position of the farthest room
            int i = Mathf.FloorToInt(farthestRoom.transform.position.x / offset.x);
            int j = Mathf.FloorToInt(-farthestRoom.transform.position.z / offset.y);
            int farthestCellIndex = i + j * size.x;

            // Get the corresponding Cell from the board array
            Cell farthestCell = board[farthestCellIndex];

            // Remove the farthest room
            InstantiatedRooms.Remove(farthestRoom);
            Destroy(farthestRoom.gameObject);

            // Instantiate a random Bossroom to replace it
            int randomBossRoomIndex = Random.Range(0, Bossrooms.Length);
            var newBossRoom = Instantiate(Bossrooms[randomBossRoomIndex].room, farthestRoom.transform.position, Quaternion.identity, transform).GetComponent<RoomBehaviour>();

            // Update the new Bossroom with the status from the farthest Cell
            newBossRoom.UpdateRoom(farthestCell.status);

            newBossRoom.name += " BOSS_ROOM";
            InstantiatedRooms.Add(newBossRoom);
        }
    }
}