using UnityEngine;

// Template to access room prefabs in an organized form for level generation
public class RoomTemplates : MonoBehaviour
{
    [Header("Basic Door Rooms")]
    public GameObject[] m_topDoorRooms;
    public GameObject[] m_leftDoorRooms;
    public GameObject[] m_rightDoorRooms;
    public GameObject[] m_bottomDoorRooms;

    [Header("Specific Door Combos")]
    public GameObject[] m_topLeftDoorRooms;
    public GameObject[] m_topRightDoorRooms;
    public GameObject[] m_bottomLeftDoorRooms;
    public GameObject[] m_bottomRightDoorRooms;
    public GameObject[] m_topBottomDoorRooms;
    public GameObject[] m_leftRightDoorRooms;

    [Header("Dead End Door Rooms")]
    public GameObject m_topDeadEndRoom;
    public GameObject m_leftDeadEndRoom;
    public GameObject m_rightDeadEndRoom;
    public GameObject m_bottomDeadEndRoom;
}
