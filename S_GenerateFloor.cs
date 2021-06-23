using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_GenerateFloor : MonoBehaviour {

    public GameObject Foundation;
    public GameObject FloorPlane;
	public GameObject ChosenChunk;
	public GameObject EmptyAnchor;
	public GameObject Door;
	public GameObject Stationary;
	public GameObject StairWell;
	public GameObject ExternalStairWell;
	public GameObject ExternalStairWellRoof;
	public GameObject RoofPiece;

	//Children
	public GameObject LobbyContainer;
	public GameObject ExteriorWallContainer;
	public GameObject DoorContainer;
	public GameObject ChunksContainer;
	public GameObject CorridorContainer;
	public GameObject StairContainer;
	public GameObject FloorContainer;
	public GameObject InternalWallContainer;
	public GameObject MultiFloorContainer;
	GameObject LobbyRoom;

	public Material CorridorFloor;
	public Material LobbyFloor;
	public Material SocialFloor;
	public Material RoomFloor;
	public Material ExteriorWalls;
	public Material InteriorWalls;

    public float CorridoorWidth = 10; //How wide the corridor is
	public float CorridorBoundarySafetiesPercentage = 10; //Protects from placing corridors on edges (Min 10)
	public float DoorWayBoundarySafetiesPercentage = 10; // Protects from placing doors on edges (Min 20)
	public float MinimumRoomX = 10; //Minimum X length of rooms
	public float MinimumRoomZ = 10; //Minimum Z length of rooms
	public float WallWidth = 2; //How wide the walls are
	public float FloorHeight = 5; //How high is each floor going to be
	private float posX; //Position of default piece
	private float posZ; //Position of default piece
	public float StationaryLengthX; //Selected length of stationary piece
	public float StationaryLengthZ; //Selected width of stationary piece
	private float GenerationTimer = 0.5f; //Time delay between each corridor
	private float RanX;
	private float RanZ;
	public float RandomXWallPercentage; //How likely to spawn x wall in %
	public float RandomZWallPercentage; //How likely to spawn z wall in %
	public float XFoundationSize; //X Size of Foundation
	public float ZFoundationSize; //Z Size of Foundation
	public float FloorThickness; //Chunk Thickness of floor
	public float FirstLeftChunkSize;
	public float FirstRightChunkSize;
	public Vector3 FoundationSize;

	public int HowManyCorridors = 2; //Amount of corridors selected
	public int HowManyFloors = 10;
	private int CorridorCounter;


	public Vector3 OriginalPosition;

	public bool FirstPass = true; //Check if creating first corridor
	public bool ExternalStairs = false;
	public bool HorizontalMainCorridor = false;
	public bool VerticalMainCorridor = false;


	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {

	}
		

	//Reset Building to Empty
	public void Reset()
	{
		Debug.Log ("Reset Building");

		//Remove Additional Floors
		if (MultiFloorContainer.transform.childCount > 1) 
		{
			int children = MultiFloorContainer.transform.childCount;

			for (int i = 1; i < children; i++)
			{
				DestroyImmediate (MultiFloorContainer.transform.GetChild (1).gameObject);

			}


		}

		//Remove Additional Chunks
		if (ChunksContainer.transform.childCount > 1) 
		{
			int children = ChunksContainer.transform.childCount;

			for (int i = 0; i < children; i++)
			{
				if (ChunksContainer.transform.GetChild (0).name != "Base Floor") 
				{
					DestroyImmediate (ChunksContainer.transform.GetChild (0).gameObject);
				}
			}
		}

		//Remove Additional External Walls
		if (ExteriorWallContainer.transform.childCount > 0) 
		{
			int children = ExteriorWallContainer.transform.childCount;

			for (int i = 0; i < children; i++)
			{
				DestroyImmediate (ExteriorWallContainer.transform.GetChild (0).gameObject);
			}
		}

		//Remove Additional Doors
		if (DoorContainer.transform.childCount > 0) 
		{
			int children = DoorContainer.transform.childCount;

			for (int i = 0; i < children; i++)
			{
				DestroyImmediate (DoorContainer.transform.GetChild (0).gameObject);
			}
		}

		//Remove Additional Internal Walls
		if (InternalWallContainer.transform.childCount > 0) 
		{
			int children = InternalWallContainer.transform.childCount;

			for (int i = 0; i < children; i++)
			{
				DestroyImmediate (InternalWallContainer.transform.GetChild (0).gameObject);
			}
		}

		//Remove Additional Corridors
		if (CorridorContainer.transform.childCount > 0) 
		{
			int children = CorridorContainer.transform.childCount;

			for (int i = 0; i < children; i++)
			{
				DestroyImmediate (CorridorContainer.transform.GetChild (0).gameObject);
			}
		}

		//Remove Additional Lobby
		if (LobbyContainer.transform.childCount > 0) 
		{
			int children = LobbyContainer.transform.childCount;

			for (int i = 0; i < children; i++)
			{
				DestroyImmediate (LobbyContainer.transform.GetChild (0).gameObject);
			}
		}

		//Remove Additional Stairs
		if (StairContainer.transform.childCount > 0) 
		{
			int children = StairContainer.transform.childCount;

			for (int i = 0; i < children; i++)
			{
				DestroyImmediate (StairContainer.transform.GetChild (0).gameObject);
			}
		}

		//Remove roof
		if (RoofPiece != null) 
		{
			DestroyImmediate (RoofPiece);
		}
	}

	//Re-size Foundation Chunk
	public void ResizeFoundation ()
	{
		Foundation.transform.localScale = new Vector3 (XFoundationSize, FloorThickness, ZFoundationSize);
		FloorPlane.transform.localScale = new Vector3 (FloorPlane.transform.localScale.x, FloorThickness, FloorPlane.transform.localScale.y);
	}

	//Find the next largest chunk
	public void FindNextBigChunk ()
	{


		ChosenChunk = ChunksContainer.transform.GetChild (0).gameObject;
		print ("THIS ONE: " + ChosenChunk);

		for (int i = 0; i < ChunksContainer.transform.childCount; i++)
		{
			if ((ChunksContainer.transform.GetChild (i).transform.localScale.x * ChunksContainer.transform.GetChild (i).transform.localScale.z) > (ChosenChunk.transform.localScale.x * ChosenChunk.transform.localScale.z)) 
			{
				print ("New Chunk Allocated");

				ChosenChunk = ChunksContainer.transform.GetChild (i).gameObject;

			}
			print ("Pass: " + i);
		}

		if (FirstPass == false) 
		{
			if (ChosenChunk.transform.localScale.x >= ChosenChunk.transform.localScale.z) 
			{//directionSwitch == false) 
				//Invoke ("CreateVerticleSlice", GenerationTimer += 0.5f);
				CreateVerticleSlice ();
			} 

			else if (ChosenChunk.transform.localScale.x < ChosenChunk.transform.localScale.z) 
			{//directionSwitch == true) 
				//Invoke ("CreateHorizontalSlice", GenerationTimer += 0.5f);
				CreateHorizontalSlice ();
			}
		} 

		else if (FirstPass == true) 
		{
			if (ChosenChunk.transform.localScale.x >= ChosenChunk.transform.localScale.z) 
			{//directionSwitch == false) 
				//Invoke ("CreateVerticleSlice", GenerationTimer += 0.5f);
				CreateHorizontalSlice ();
			} 

			else if (ChosenChunk.transform.localScale.x < ChosenChunk.transform.localScale.z) 
			{//directionSwitch == true) 
				//Invoke ("CreateHorizontalSlice", GenerationTimer += 0.5f);
				CreateVerticleSlice ();
			}
			FirstPass = false;
		}

	}
		
	//Create Corridor and two chunks vertically
    public void CreateVerticleSlice () //Along X
    {

        //Create measurements
        float Xsize = ChosenChunk.GetComponent<Collider>().bounds.size.x;
        float Zsize = ChosenChunk.GetComponent<Collider>().bounds.size.z;
		float RandomPoint = Random.Range((CorridorBoundarySafetiesPercentage / 100) * Xsize, Xsize - ((CorridorBoundarySafetiesPercentage / 100) * Xsize));
        float LeftChunkSize = RandomPoint - (CorridoorWidth / 2);
        float RightChunkSize = Xsize - (LeftChunkSize + CorridoorWidth);
        float LeftOffset = (Xsize / 2) - (LeftChunkSize / 2);
        float RightOffset = (Xsize / 2) - (RightChunkSize / 2);

        //Build Left Chunk
        GameObject _leftChunk = Instantiate(FloorPlane, ChosenChunk.transform.position - new Vector3 (LeftOffset, 0, 0), ChosenChunk.transform.rotation);
        _leftChunk.transform.localScale = new Vector3(LeftChunkSize, _leftChunk.transform.localScale.y, Zsize);
		_leftChunk.transform.parent = ChunksContainer.transform;
		_leftChunk.name = (LeftChunkSize * Zsize).ToString();

        //Build Right Chunk
        GameObject _rightChunk = Instantiate(FloorPlane, ChosenChunk.transform.position + new Vector3(RightOffset, 0, 0), ChosenChunk.transform.rotation);
        _rightChunk.transform.localScale = new Vector3(RightChunkSize, _rightChunk.transform.localScale.y, Zsize);
		_rightChunk.transform.parent = ChunksContainer.transform;
		_rightChunk.name = (RightChunkSize * Zsize).ToString();


		//Build Corridor
		GameObject _corridor = Instantiate(FloorPlane, ChosenChunk.transform.position , ChosenChunk.transform.rotation);
		_corridor.transform.localScale = new Vector3 (CorridoorWidth,_corridor.transform.localScale.y, Zsize);
		_corridor.transform.position = _rightChunk.transform.position - new Vector3 (RightChunkSize /2 + CorridoorWidth /2,0,0);
		_corridor.GetComponent<MeshRenderer> ().material = CorridorFloor;
		_corridor.transform.parent = CorridorContainer.transform;
		_corridor.name = "Vertical Corridor";

		//Check that foundation chunk is not destroyed
        print("VERTICAL SLICE");
		if (ChosenChunk.name == "Base Floor") 
		{
			Foundation.transform.parent = transform;
			Foundation.SetActive (false);
			FirstLeftChunkSize = LeftChunkSize;
			FirstRightChunkSize = RightChunkSize;
			VerticalMainCorridor = true;
		} 
		else 
		{
			DestroyImmediate(ChosenChunk);
		}
			
    }

	//Create Corridor and two chunks horizontally
    public void CreateHorizontalSlice() //Along Z
    {
        //Create measurements
        float Xsize = ChosenChunk.GetComponent<Collider>().bounds.size.x;
        float Zsize = ChosenChunk.GetComponent<Collider>().bounds.size.z;
		float RandomPoint = Random.Range((CorridorBoundarySafetiesPercentage / 100) * Zsize, Zsize - ((CorridorBoundarySafetiesPercentage / 100) * Zsize));
        float TopChunkSize = RandomPoint - (CorridoorWidth / 2);
        float BottomChunkSize = Zsize - (TopChunkSize + CorridoorWidth);
        float UpOffset = (Zsize / 2) - (TopChunkSize / 2);
        float DownOffset = (Zsize / 2) - (BottomChunkSize / 2);

        //Build Top Chunk
        GameObject _topChunk = Instantiate(FloorPlane, ChosenChunk.transform.position - new Vector3(0, 0, UpOffset), ChosenChunk.transform.rotation);
        _topChunk.transform.localScale = new Vector3(Xsize, _topChunk.transform.localScale.y, TopChunkSize);
		_topChunk.transform.parent = ChunksContainer.transform;
		_topChunk.name = (Xsize * TopChunkSize).ToString();

        //Build Bottom  Chunk
        GameObject _bottomChunk = Instantiate(FloorPlane, ChosenChunk.transform.position + new Vector3(0, 0, DownOffset), ChosenChunk.transform.rotation);
        _bottomChunk.transform.localScale = new Vector3(Xsize, _bottomChunk.transform.localScale.y, BottomChunkSize);
		_bottomChunk.transform.parent = ChunksContainer.transform;
		_bottomChunk.name = (Xsize * BottomChunkSize).ToString();

		//Build Corridor
		GameObject _corridor = Instantiate(FloorPlane, ChosenChunk.transform.position , ChosenChunk.transform.rotation);
		_corridor.transform.localScale = new Vector3 (Xsize,_corridor.transform.localScale.y, CorridoorWidth);
		_corridor.transform.position = _bottomChunk.transform.position - new Vector3 (0,0,BottomChunkSize /2 + CorridoorWidth /2);
		_corridor.GetComponent<MeshRenderer> ().material = CorridorFloor;
		_corridor.transform.parent = CorridorContainer.transform;
		_corridor.name = "Horizontal Corridor";

		//Check that foundation chunk is not destroyed
        print("HORIZONTAL SLICE");
		if (ChosenChunk.name == "Base Floor") 
		{
			Foundation.transform.parent = transform;
			Foundation.SetActive (false);
			FirstLeftChunkSize = TopChunkSize;
			FirstRightChunkSize = BottomChunkSize;
			HorizontalMainCorridor = true;
		} 
		else 
		{
			DestroyImmediate(ChosenChunk);
		}
			
    }

	//Create Lobby room for stairwell
	public void CreateLobby()
	{
		//Find smallest available chunk to fit stairwell
		LobbyRoom = ChunksContainer.transform.GetChild (0).gameObject;

		for (int i = 0; i < ChunksContainer.transform.childCount; i++)
		{
			Vector3 Size = ChunksContainer.transform.GetChild (i).GetComponent<MeshRenderer> ().bounds.size;
			Vector3 LobbySize = LobbyRoom.GetComponent<MeshRenderer> ().bounds.size;

			if (Size.x *  Size.z < LobbySize.x * LobbySize.z && Size.x >= 11 && Size.z >= 7) 
			{
				print ("New Lobby Allocated");

				LobbyRoom = ChunksContainer.transform.GetChild (i).gameObject;

			}

		}

		//Create Internal Lobby if chunk is big enough
		if (LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.x >= 11 && LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.z >= 7) 
		{
			LobbyRoom.transform.parent = LobbyContainer.transform;

			GameObject _Stairs = Instantiate(StairWell, LobbyRoom.transform.position + new Vector3(0,0, 3.5f), transform.rotation);
			_Stairs.transform.parent = StairContainer.transform;
			float Zlength = LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.z - 7;
			float Xlength = LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.x - 11;

			//Build floor around stairwell to connect with corridor
			if (LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.z > 7)
			{
				Debug.Log ("Creating Z Lobby");
				GameObject _TopLobbyChunk = Instantiate(LobbyRoom, LobbyRoom.transform.position + new Vector3 (0,0,LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.z / 2), transform.rotation);
				_TopLobbyChunk.transform.localScale = new Vector3 (LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.x, FloorPlane.transform.localScale.y,Zlength / 2);
				_TopLobbyChunk.transform.position -= new Vector3 (0, 0, _TopLobbyChunk.GetComponent<MeshRenderer> ().bounds.size.z / 2);
				_TopLobbyChunk.transform.parent = LobbyContainer.transform;
				_TopLobbyChunk.GetComponent<MeshRenderer> ().material = LobbyFloor;

				GameObject _BottomLobbyChunk = Instantiate(LobbyRoom, LobbyRoom.transform.position - new Vector3 (0,0,LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.z / 2), transform.rotation);
				_BottomLobbyChunk.transform.localScale = new Vector3 (LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.x, FloorPlane.transform.localScale.y, Zlength / 2);
				_BottomLobbyChunk.transform.position += new Vector3 (0, 0, _TopLobbyChunk.GetComponent<MeshRenderer> ().bounds.size.z / 2);
				_BottomLobbyChunk.transform.parent = LobbyContainer.transform;
				_BottomLobbyChunk.GetComponent<MeshRenderer> ().material = LobbyFloor;
			}
			if (LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.x > 11)
			{
				Debug.Log ("Creating Z Lobby");
				GameObject _RightLobbyChunk = Instantiate(LobbyRoom, LobbyRoom.transform.position + new Vector3 (LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.x / 2,0,0), transform.rotation);
				_RightLobbyChunk.transform.localScale = new Vector3 (Xlength / 2, FloorPlane.transform.localScale.y, 7);
				_RightLobbyChunk.transform.position -= new Vector3 (_RightLobbyChunk.GetComponent<MeshRenderer> ().bounds.size.x / 2, 0, 0);
				_RightLobbyChunk.transform.parent = LobbyContainer.transform;
				_RightLobbyChunk.GetComponent<MeshRenderer> ().material = LobbyFloor;

				GameObject _LeftLobbyChunk = Instantiate(LobbyRoom, LobbyRoom.transform.position - new Vector3 (LobbyRoom.GetComponent<MeshRenderer> ().bounds.size.x / 2,0,0), transform.rotation);
				_LeftLobbyChunk.transform.localScale = new Vector3 (Xlength / 2, FloorPlane.transform.localScale.y, 7);
				_LeftLobbyChunk.transform.position += new Vector3 (_LeftLobbyChunk.GetComponent<MeshRenderer> ().bounds.size.x / 2, 0, 0);
				_LeftLobbyChunk.transform.parent = LobbyContainer.transform;
				_LeftLobbyChunk.GetComponent<MeshRenderer> ().material = LobbyFloor;
			}

			//Remove Uneeded Chunk
			DestroyImmediate( LobbyRoom);
		} 

		//Create External Lobby if chunks are too small
		else 
		{
			Debug.Log ("Chunk Sizes too Small to Create Internal Stairs, Creating External Stairs");

			ExternalStairs = true;

			//Attatch stairwell to end of main corridor
			if (CorridorContainer.transform.GetChild (0).name == "Horizontal Corridor") 
			{
				GameObject _Stairs = Instantiate(ExternalStairWell, CorridorContainer.transform.GetChild(0).position - new Vector3(CorridorContainer.transform.GetChild(0).lossyScale.x / 2 + 3,0, 0), transform.rotation);
				_Stairs.transform.eulerAngles = new Vector3 (0, 90, 0);
				_Stairs.transform.parent = StairContainer.transform;

				GameObject _RoofStairs = Instantiate(ExternalStairWellRoof, CorridorContainer.transform.GetChild(0).position - new Vector3(CorridorContainer.transform.GetChild(0).lossyScale.x / 2 + 3,0, 0) + new Vector3 (0,(FloorHeight * HowManyFloors) - FloorHeight,0), transform.rotation);
				_RoofStairs.transform.eulerAngles = new Vector3 (0, 90, 0);
				_RoofStairs.transform.parent = ExteriorWallContainer.transform;
			}
			else if (CorridorContainer.transform.GetChild (0).name == "Vertical Corridor") 
			{
				GameObject _Stairs = Instantiate(ExternalStairWell, CorridorContainer.transform.GetChild(0).position + new Vector3(0,0,CorridorContainer.transform.GetChild(0).lossyScale.z / 2 + 3), transform.rotation );
				_Stairs.transform.eulerAngles = new Vector3 (0, 180, 0);
				_Stairs.transform.parent = StairContainer.transform;

				GameObject _RoofStairs = Instantiate(ExternalStairWellRoof, CorridorContainer.transform.GetChild(0).position + new Vector3(0,0,CorridorContainer.transform.GetChild(0).lossyScale.z / 2 + 3) + new Vector3 (0,(FloorHeight * HowManyFloors) - FloorHeight,0), transform.rotation);
				_RoofStairs.transform.eulerAngles = new Vector3 (0, 180, 0);
				_RoofStairs.transform.parent = ExteriorWallContainer.transform;
			}

		}


	}

	//Spawn walls to create rooms
	public void CreateRooms()
	{
		FirstPass = false;

		for (int i = 0; i < ChunksContainer.transform.childCount; i++)
		{
			Vector3 Size = ChunksContainer.transform.GetChild (i).GetComponent<MeshRenderer> ().bounds.size;
			ChunksContainer.transform.GetChild (i).name = "Size x: " + Size.x + " Size z: " + Size.z;

			float XSegments = Size.x / MinimumRoomX; 
			float ZSegments = Size.z / MinimumRoomZ;

			Vector3 position = ChunksContainer.transform.GetChild (i).position - new Vector3 (Size.x / 2, 0, Size.z / 2);
			Vector3 Doorposition = ChunksContainer.transform.GetChild (i).position - new Vector3 (0, 0, Size.z / 2);

			if (Mathf.Floor (ZSegments) > 3) 
			{
				RanZ = Random.Range (1, Mathf.Floor (ZSegments) - 1);
			}

			//Walls along X axis
			if (Size.x >= MinimumRoomX) 
			{
				ChunksContainer.transform.GetChild (i).GetComponent<MeshRenderer> ().material = RoomFloor;
				for (int ib = 0; ib < Mathf.Floor (XSegments) + 1; ib++) 
				{ 
					if (Mathf.Floor (XSegments) > 3) 
					{
						RanX = Random.Range (ib, Mathf.Floor (XSegments) - 1);
						print ("RANx " + RanX);
					} 
					else 
					{
						RanX = -1;
					}
						
					    //Create door randomly along x axis and then two walls to fill one side of room
						for (int ic = 0; ic < Mathf.Floor (ZSegments); ic++) 
						{
						if ( ib == 0 ||  ib == Mathf.Floor (XSegments) || Random.Range (1, 99) < RandomXWallPercentage)  
						{
							float RandomDoorPoint = Random.Range ((DoorWayBoundarySafetiesPercentage / 100) * (Size.z / Mathf.Floor (ZSegments)), (Size.z / Mathf.Floor (ZSegments)) - (DoorWayBoundarySafetiesPercentage / 100) * (Size.z / Mathf.Floor (ZSegments)));

							GameObject _DoorWall = Instantiate (Door, position + new Vector3 (((Size.x / Mathf.Floor (XSegments)) * (ib)), 0, ((Size.z / Mathf.Floor (ZSegments)) * (ic + 1))), ChunksContainer.transform.GetChild (i).rotation);
							_DoorWall.transform.position -= new Vector3 (0, 0, RandomDoorPoint);
							_DoorWall.name = "X Door";
							_DoorWall.transform.parent = DoorContainer.transform;

							//Right Wall
							_DoorWall.transform.GetChild (1).localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, (Size.z / Mathf.Floor (ZSegments)) - (_DoorWall.transform.GetChild (0).localScale.z / 2 + RandomDoorPoint));
							_DoorWall.transform.GetChild (1).position -= new Vector3 (0, 0, (_DoorWall.transform.GetChild (1).localScale.z / 2) - 0.21f);
							_DoorWall.transform.GetChild (1).position += new Vector3 (0, (FloorHeight * HowManyFloors) / 2 - (FloorHeight / 2), 0);
							_DoorWall.transform.GetChild (1).name = "X Wall";

							//Left Wall
							_DoorWall.transform.GetChild (2).localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, RandomDoorPoint - _DoorWall.transform.GetChild (0).localScale.z / 2);
							_DoorWall.transform.GetChild (2).position += new Vector3 (0, 0, (_DoorWall.transform.GetChild (2).GetComponent<MeshRenderer> ().bounds.size.z / 2) - 0.21f);
							_DoorWall.transform.GetChild (2).position += new Vector3 (0, (FloorHeight * HowManyFloors) / 2 - (FloorHeight / 2), 0);
							_DoorWall.transform.GetChild (2).name = "X Wall";

							_DoorWall.transform.GetChild (1).parent = InternalWallContainer.transform;
							_DoorWall.transform.GetChild (1).parent = InternalWallContainer.transform;
						}

						}
				}
			} 
			//Dont create walls if chunk too small
			else 
			{
				print ("Room size too large to fit within chunk");
				ChunksContainer.transform.GetChild (i).name += " Social Areal";
				ChunksContainer.transform.GetChild (i).GetComponent<MeshRenderer> ().material = SocialFloor;
			}

			//Walls along Z axis
			if (Size.z >= MinimumRoomZ ) 
			{
				ChunksContainer.transform.GetChild (i).GetComponent<MeshRenderer> ().material = RoomFloor;
				for (int id = 0; id < Mathf.Floor (ZSegments) + 1; id++) { 

					//Create door randomly along z axis and then two walls to fill one side of room 
					for (int ie = 0; ie < Mathf.Floor (XSegments); ie++) 
					{
						if (id == 0 || id == Mathf.Floor (ZSegments) || Random.Range (1, 99) < RandomZWallPercentage) 
						{
							float RandomDoorPoint = Random.Range ((DoorWayBoundarySafetiesPercentage / 100) * (Size.x / Mathf.Floor (XSegments)), (Size.x / Mathf.Floor (XSegments)) - (DoorWayBoundarySafetiesPercentage / 100) * (Size.x / Mathf.Floor (XSegments)));

							GameObject _DoorWall = Instantiate (Door, position + new Vector3 (((Size.x / Mathf.Floor (XSegments)) * (ie + 1)), 0, ((Size.z / Mathf.Floor (ZSegments)) * (id))), ChunksContainer.transform.GetChild (i).rotation);
							_DoorWall.transform.position -= new Vector3 (RandomDoorPoint, 0, 0);
							_DoorWall.name = "Z Door";

							_DoorWall.transform.eulerAngles = new Vector3 (_DoorWall.transform.rotation.x, _DoorWall.transform.rotation.y + 90, _DoorWall.transform.rotation.z);
							_DoorWall.transform.parent = DoorContainer.transform;

							//Bottom Wall
							_DoorWall.transform.GetChild (1).localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, (Size.x / Mathf.Floor (XSegments)) - (_DoorWall.transform.GetChild (0).localScale.z / 2 + RandomDoorPoint)); 
							_DoorWall.transform.GetChild (1).localPosition -= new Vector3 (0, 0, _DoorWall.transform.GetChild (1).localScale.z / 2 - 0.21f);
							_DoorWall.transform.GetChild (1).localPosition += new Vector3 (0, (FloorHeight * HowManyFloors) / 2 - (FloorHeight / 2), 0);
							_DoorWall.transform.GetChild (1).name = "Z Wall";

							//Top Wall
							_DoorWall.transform.GetChild (2).localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, RandomDoorPoint - _DoorWall.transform.GetChild (0).localScale.z / 2);
							_DoorWall.transform.GetChild (2).localPosition += new Vector3 (0, 0, _DoorWall.transform.GetChild (2).localScale.z / 2 - 0.21f);
							_DoorWall.transform.GetChild (2).localPosition += new Vector3 (0, (FloorHeight * HowManyFloors) / 2 - (FloorHeight / 2), 0);
							_DoorWall.transform.GetChild (2).name = "Z Wall";

							_DoorWall.transform.GetChild (1).parent = InternalWallContainer.transform;
							_DoorWall.transform.GetChild (1).parent = InternalWallContainer.transform;
						}

					}
						
				}
			}
			//Dont create walls if chunk too small
			else 
			{
				print ("Room size too large to fit within chunk");
				ChunksContainer.transform.GetChild (i).name += " Social Areal";
				ChunksContainer.transform.GetChild (i).GetComponent<MeshRenderer> ().material = SocialFloor;
			}
		}
	}

	//Remove any internal walls spawned on edges of building and spawn external walls (windows)
	public void ExternalWalls ()
	{
		Debug.Log ("Removing Extra Walls & Doors");

		posX = Foundation.transform.lossyScale.x;
		posZ = Foundation.transform.lossyScale.z;
		OriginalPosition = Foundation.transform.position;

		int children = InternalWallContainer.transform.childCount;

		//Scan Through Walls and delete
		for (int i = 0; i < children; i++)
		{
			if (InternalWallContainer.transform.GetChild (i).name == "Z Wall" && InternalWallContainer.transform.GetChild (i).position.z >= OriginalPosition.z + (posZ / 2) - 2) 
			{
				InternalWallContainer.transform.GetChild (i).name = "External Wall Z+" + InternalWallContainer.transform.GetChild (i).position.z +"/" + posZ;
				DestroyImmediate (InternalWallContainer.transform.GetChild (i).gameObject);
				i--;
				children--;
			}
			else if (InternalWallContainer.transform.GetChild (i).name == "Z Wall" && InternalWallContainer.transform.GetChild (i).position.z <= OriginalPosition.z - (posZ / 2) + 2) 
			{
				InternalWallContainer.transform.GetChild (i).name = "External Wall Z-"+ InternalWallContainer.transform.GetChild (i).position.z+"/" + posZ;
				DestroyImmediate (InternalWallContainer.transform.GetChild (i).gameObject);
				i--;
				children--;
			}
			else if (InternalWallContainer.transform.GetChild (i).name == "X Wall" && InternalWallContainer.transform.GetChild (i).position.x >= OriginalPosition.x + (posX / 2) - 2) 
			{
				InternalWallContainer.transform.GetChild (i).name = "External Wall X+"+ InternalWallContainer.transform.GetChild (i).position.x+"/" + posX;
				DestroyImmediate (InternalWallContainer.transform.GetChild (i).gameObject);
				i--;
				children--;
			}
			else if (InternalWallContainer.transform.GetChild (i).name == "X Wall" && InternalWallContainer.transform.GetChild (i).position.x <= OriginalPosition.x - (posX / 2) + 2) 
			{
				InternalWallContainer.transform.GetChild (i).name = "External Wall X-"+ InternalWallContainer.transform.GetChild (i).position.x+"/" + posX;
				DestroyImmediate (InternalWallContainer.transform.GetChild (i).gameObject);
				i--;
				children--;
			}
		}


		int children2 = DoorContainer.transform.childCount;

		//Scan Through Doors and delete
		for (int i = 0; i < children2; i++)
		{
			if (DoorContainer.transform.GetChild (i).name == "Z Door" && DoorContainer.transform.GetChild (i).position.z >= OriginalPosition.z + (posZ / 2) - 2) //
			{
				DestroyImmediate (DoorContainer.transform.GetChild (i).gameObject);
				i--;
				children2--;
			}
			else if (DoorContainer.transform.GetChild (i).name == "Z Door" && DoorContainer.transform.GetChild (i).position.z <= OriginalPosition.z - (posZ / 2) + 2) 
			{
				DestroyImmediate (DoorContainer.transform.GetChild (i).gameObject);
				i--;
				children2--;
			}
			else if (DoorContainer.transform.GetChild (i).name == "X Door" && DoorContainer.transform.GetChild (i).position.x >= OriginalPosition.x + (posX / 2) - 2) 
			{
				DestroyImmediate (DoorContainer.transform.GetChild (i).gameObject);
				i--;
				children2--;
			}
			else if (DoorContainer.transform.GetChild (i).name == "X Door" && DoorContainer.transform.GetChild (i).position.x <= OriginalPosition.x - (posX / 2) + 2) //
			{
				DestroyImmediate (DoorContainer.transform.GetChild (i).gameObject);
				i--;
				children2--;
			}
		}

		//Instantiate External Walls
		if (ExternalStairs == false) 
		{
			//Create Horizontal Entrance Gap
			if (HorizontalMainCorridor == true) 
			{ 
				Debug.Log ("Creating Internal Stairs on Horizontal Axis");
				float Zposition = CorridorContainer.transform.GetChild (0).position.z;

				GameObject _ExteriorWall1 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, posZ / 2), Foundation.transform.rotation);
				_ExteriorWall1.transform.localScale = new Vector3 (posX, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall1.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall1.transform.parent = ExteriorWallContainer.transform;

				GameObject _ExteriorWall2 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, -posZ / 2), Foundation.transform.rotation);
				_ExteriorWall2.transform.localScale = new Vector3 (posX, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall2.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall2.transform.parent = ExteriorWallContainer.transform;

				//Left of Corridor (Entrance)
				GameObject _ExteriorWall3 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (posX / 2, (FloorHeight * HowManyFloors) / 2,0), Foundation.transform.rotation);
				_ExteriorWall3.transform.position = new Vector3(_ExteriorWall3.transform.position.x, _ExteriorWall3.transform.position.y, Zposition);
				_ExteriorWall3.transform.position -= new Vector3 (0, 0, (CorridoorWidth/2) + (FirstLeftChunkSize/2));
				_ExteriorWall3.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, FirstLeftChunkSize);
				_ExteriorWall3.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall3.transform.parent = ExteriorWallContainer.transform;

				//Right of Corridor (Entrance)
				GameObject _ExteriorWall3b = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (posX / 2, (FloorHeight * HowManyFloors) / 2,0), Foundation.transform.rotation);
				_ExteriorWall3b.transform.position = new Vector3(_ExteriorWall3b.transform.position.x, _ExteriorWall3b.transform.position.y, Zposition);
				_ExteriorWall3b.transform.position += new Vector3 (0, 0, (CorridoorWidth/2) + (FirstRightChunkSize/2));
				_ExteriorWall3b.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, FirstRightChunkSize);
				_ExteriorWall3b.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall3b.transform.parent = ExteriorWallContainer.transform;

				//Above Corridor (Entrance)
				GameObject _ExteriorWall3c = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (posX / 2, ((FloorHeight * HowManyFloors) / 2) + FloorHeight / 2,0), Foundation.transform.rotation);
				_ExteriorWall3c.transform.position = new Vector3(_ExteriorWall3c.transform.position.x, _ExteriorWall3c.transform.position.y, Zposition);
				_ExteriorWall3c.transform.localScale = new Vector3 (WallWidth, (FloorHeight * HowManyFloors) - FloorHeight, CorridoorWidth);
				_ExteriorWall3c.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall3c.transform.parent = ExteriorWallContainer.transform;

				GameObject _ExteriorWall4 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (-posX / 2, (FloorHeight * HowManyFloors) / 2, 0), Foundation.transform.rotation);
				_ExteriorWall4.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, posZ);
				_ExteriorWall4.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall4.transform.parent = ExteriorWallContainer.transform;
			} 
			//Create Vertical Stair Gap
			else if (VerticalMainCorridor == true) 
			{ 
				Debug.Log ("Creating Internal Stairs on Vertical Axis");
				float Xposition = CorridorContainer.transform.GetChild (0).position.x;

				GameObject _ExteriorWall1 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, posZ / 2), Foundation.transform.rotation);
				_ExteriorWall1.transform.localScale = new Vector3 (posX, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall1.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall1.transform.parent = ExteriorWallContainer.transform;

				//Left of Corridor (Entrance)
				GameObject _ExteriorWall2 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, -posZ / 2), Foundation.transform.rotation);
				_ExteriorWall2.transform.position = new Vector3(Xposition, _ExteriorWall2.transform.position.y, _ExteriorWall2.transform.position.z);
				_ExteriorWall2.transform.position -= new Vector3 ((CorridoorWidth/2) + (FirstLeftChunkSize/2), 0, 0);
				_ExteriorWall2.transform.localScale = new Vector3 (FirstLeftChunkSize, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall2.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall2.transform.parent = ExteriorWallContainer.transform;
				_ExteriorWall2.name = "Left External Wall (Entrance)";

				//Right of Corridor (Entrance)
				GameObject _ExteriorWall2b = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, -posZ / 2), Foundation.transform.rotation);
				_ExteriorWall2b.transform.position = new Vector3(Xposition, _ExteriorWall2b.transform.position.y, _ExteriorWall2b.transform.position.z);
				_ExteriorWall2b.transform.position += new Vector3 ((CorridoorWidth/2) + (FirstRightChunkSize/2), 0, 0);
				_ExteriorWall2b.transform.localScale = new Vector3 (FirstRightChunkSize, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall2b.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall2b.transform.parent = ExteriorWallContainer.transform;
				_ExteriorWall2b.name = "Right External Wall (Entrance)";

				//Above Corridor (Entrance)
				GameObject _ExteriorWall2c = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, ((FloorHeight * HowManyFloors) / 2) + FloorHeight / 2, -posZ / 2), Foundation.transform.rotation);
				_ExteriorWall2c.transform.position = new Vector3(Xposition, _ExteriorWall2c.transform.position.y, _ExteriorWall2c.transform.position.z);
				_ExteriorWall2c.transform.localScale = new Vector3 (CorridoorWidth, (FloorHeight * HowManyFloors) - FloorHeight, WallWidth);
				_ExteriorWall2c.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall2c.transform.parent = ExteriorWallContainer.transform;
				_ExteriorWall2c.name = "Above External Wall (Entrance)";

				GameObject _ExteriorWall3 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (posX / 2, (FloorHeight * HowManyFloors) / 2, 0), Foundation.transform.rotation);
				_ExteriorWall3.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, posZ);
				_ExteriorWall3.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall3.transform.parent = ExteriorWallContainer.transform;

				GameObject _ExteriorWall4 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (-posX / 2, (FloorHeight * HowManyFloors) / 2, 0), Foundation.transform.rotation);
				_ExteriorWall4.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, posZ);
				_ExteriorWall4.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall4.transform.parent = ExteriorWallContainer.transform;
			}
		} 
		else if (ExternalStairs == true) 
		{
			if (HorizontalMainCorridor == true) //Create Horizontal Stair Gap
			{
				Debug.Log ("Creating External Stairs on Horizontal Axis");
				float Zposition = CorridorContainer.transform.GetChild (0).position.z;

				GameObject _ExteriorWall1 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, posZ / 2), Foundation.transform.rotation);
				_ExteriorWall1.transform.localScale = new Vector3 (posX, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall1.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall1.transform.parent = ExteriorWallContainer.transform;

				GameObject _ExteriorWall2 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, -posZ / 2), Foundation.transform.rotation);
				_ExteriorWall2.transform.localScale = new Vector3 (posX, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall2.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall2.transform.parent = ExteriorWallContainer.transform;

				//Left of Corridor (Entrance)
				GameObject _ExteriorWall3 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (posX / 2, (FloorHeight * HowManyFloors) / 2,0), Foundation.transform.rotation);
				_ExteriorWall3.transform.position = new Vector3(_ExteriorWall3.transform.position.x, _ExteriorWall3.transform.position.y, Zposition);
				_ExteriorWall3.transform.position -= new Vector3 (0, 0, (CorridoorWidth/2) + (FirstLeftChunkSize/2));
				_ExteriorWall3.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, FirstLeftChunkSize);
				_ExteriorWall3.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall3.transform.parent = ExteriorWallContainer.transform;

				//Right of Corridor (Entrance)
				GameObject _ExteriorWall3b = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (posX / 2, (FloorHeight * HowManyFloors) / 2,0), Foundation.transform.rotation);
				_ExteriorWall3b.transform.position = new Vector3(_ExteriorWall3b.transform.position.x, _ExteriorWall3b.transform.position.y, Zposition);
				_ExteriorWall3b.transform.position += new Vector3 (0, 0, (CorridoorWidth/2) + (FirstRightChunkSize/2));
				_ExteriorWall3b.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, FirstRightChunkSize);
				_ExteriorWall3b.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall3b.transform.parent = ExteriorWallContainer.transform;

				//Above Corridor (Entrance)
				GameObject _ExteriorWall3c = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (posX / 2, ((FloorHeight * HowManyFloors) / 2) + FloorHeight / 2,0), Foundation.transform.rotation);
				_ExteriorWall3c.transform.position = new Vector3(_ExteriorWall3c.transform.position.x, _ExteriorWall3c.transform.position.y, Zposition);
				_ExteriorWall3c.transform.localScale = new Vector3 (WallWidth, (FloorHeight * HowManyFloors) - FloorHeight, CorridoorWidth);
				_ExteriorWall3c.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall3c.transform.parent = ExteriorWallContainer.transform;

				//Left of Corridor (Stairwell)
				GameObject _ExteriorWall4 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (-posX / 2, (FloorHeight * HowManyFloors) / 2,0), Foundation.transform.rotation);
				_ExteriorWall4.transform.position = new Vector3(_ExteriorWall4.transform.position.x, _ExteriorWall4.transform.position.y, Zposition);
				_ExteriorWall4.transform.position -= new Vector3 (0, 0, (CorridoorWidth/2) + (FirstLeftChunkSize/2));
				_ExteriorWall4.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, FirstLeftChunkSize);
				_ExteriorWall4.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall4.transform.parent = ExteriorWallContainer.transform;
				_ExteriorWall4.name = "Left External Wall (Stairwell)";

				//Right of Corridor (Stairwell)
				GameObject _ExteriorWall4b = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (-posX / 2, (FloorHeight * HowManyFloors) / 2, 0), Foundation.transform.rotation);
				_ExteriorWall4b.transform.position = new Vector3(_ExteriorWall4b.transform.position.x, _ExteriorWall4b.transform.position.y, Zposition);
				_ExteriorWall4b.transform.position += new Vector3 (0, 0, (CorridoorWidth/2) + (FirstRightChunkSize/2));
				_ExteriorWall4b.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, FirstRightChunkSize);
				_ExteriorWall4b.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall4b.transform.parent = ExteriorWallContainer.transform;
				_ExteriorWall4b.name = "Right External Wall (Stairwell)";


			} 
			else if (VerticalMainCorridor == true) //Create Vertical Stair Gap
			{
				Debug.Log ("Creating External Stairs on Vertical Axis");
				float Xposition = CorridorContainer.transform.GetChild (0).position.x;

				//Left of Corridor (Stairwell)
				GameObject _ExteriorWall1 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, posZ / 2), Foundation.transform.rotation);
				_ExteriorWall1.transform.position = new Vector3(Xposition, _ExteriorWall1.transform.position.y, _ExteriorWall1.transform.position.z);
				_ExteriorWall1.transform.position -= new Vector3 ((CorridoorWidth/2) + (FirstLeftChunkSize/2), 0, 0);
				_ExteriorWall1.transform.localScale = new Vector3 (FirstLeftChunkSize, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall1.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall1.transform.parent = ExteriorWallContainer.transform;
				_ExteriorWall1.name = "Left External Wall (Stairwell)";

				//Right of Corridor (Stairwell)
				GameObject _ExteriorWall1b = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, posZ / 2), Foundation.transform.rotation);
				_ExteriorWall1b.transform.position = new Vector3(Xposition, _ExteriorWall1b.transform.position.y, _ExteriorWall1b.transform.position.z);
				_ExteriorWall1b.transform.position += new Vector3 ((CorridoorWidth/2) + (FirstRightChunkSize/2), 0, 0);
				_ExteriorWall1b.transform.localScale = new Vector3 (FirstRightChunkSize, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall1b.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall1b.transform.parent = ExteriorWallContainer.transform;
				_ExteriorWall1b.name = "Right External Wall (Stairwell)";

				//Left of Corridor (Entrance)
				GameObject _ExteriorWall2 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, -posZ / 2), Foundation.transform.rotation);
				_ExteriorWall2.transform.position = new Vector3(Xposition, _ExteriorWall2.transform.position.y, _ExteriorWall2.transform.position.z);
				_ExteriorWall2.transform.position -= new Vector3 ((CorridoorWidth/2) + (FirstLeftChunkSize/2), 0, 0);
				_ExteriorWall2.transform.localScale = new Vector3 (FirstLeftChunkSize, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall2.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall2.transform.parent = ExteriorWallContainer.transform;
				_ExteriorWall2.name = "Left External Wall (Entrance)";

				//Right of Corridor (Entrance)
				GameObject _ExteriorWall2b = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors) / 2, -posZ / 2), Foundation.transform.rotation);
				_ExteriorWall2b.transform.position = new Vector3(Xposition, _ExteriorWall2b.transform.position.y, _ExteriorWall2b.transform.position.z);
				_ExteriorWall2b.transform.position += new Vector3 ((CorridoorWidth/2) + (FirstRightChunkSize/2), 0, 0);
				_ExteriorWall2b.transform.localScale = new Vector3 (FirstRightChunkSize, FloorHeight * HowManyFloors, WallWidth);
				_ExteriorWall2b.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall2b.transform.parent = ExteriorWallContainer.transform;
				_ExteriorWall2b.name = "Right External Wall (Entrance)";

				//Above Corridor (Entrance)
				GameObject _ExteriorWall2c = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, ((FloorHeight * HowManyFloors) / 2) + FloorHeight / 2, -posZ / 2), Foundation.transform.rotation);
				_ExteriorWall2c.transform.position = new Vector3(Xposition, _ExteriorWall2c.transform.position.y, _ExteriorWall2c.transform.position.z);
				_ExteriorWall2c.transform.localScale = new Vector3 (CorridoorWidth, (FloorHeight * HowManyFloors) - FloorHeight, WallWidth);
				_ExteriorWall2c.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall2c.transform.parent = ExteriorWallContainer.transform;
				_ExteriorWall2c.name = "Above External Wall (Entrance)";

				GameObject _ExteriorWall3 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (posX / 2, (FloorHeight * HowManyFloors) / 2, 0), Foundation.transform.rotation);
				_ExteriorWall3.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, posZ);
				_ExteriorWall3.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall3.transform.parent = ExteriorWallContainer.transform;

				GameObject _ExteriorWall4 = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (-posX / 2, (FloorHeight * HowManyFloors) / 2, 0), Foundation.transform.rotation);
				_ExteriorWall4.transform.localScale = new Vector3 (WallWidth, FloorHeight * HowManyFloors, posZ);
				_ExteriorWall4.GetComponent<MeshRenderer> ().material = ExteriorWalls;
				_ExteriorWall4.transform.parent = ExteriorWallContainer.transform;


			}
		}

	}

	//Create lines of stationary gameobjects in each room
	public void InsertStationary()
	{
		//Scan through chunks
		for (int i = 0; i < ChunksContainer.transform.childCount; i++)
		{
			Vector3 Size = ChunksContainer.transform.GetChild (i).GetComponent<MeshRenderer> ().bounds.size;

			float XSegments = Size.x / MinimumRoomX; 
			float ZSegments = Size.z / MinimumRoomZ;

			Vector3 position = ChunksContainer.transform.GetChild (i).position - new Vector3 (Size.x / 2, 0, Size.z / 2);

			for (int ib = 0; ib < Mathf.Floor (XSegments) ; ib++) 
			{ 

				for (int ic = 0; ic < Mathf.Floor (ZSegments); ic++) 
				{
					Vector3 SavedPos = position + new Vector3 (((Size.x / Mathf.Floor (XSegments)) * (ib)), 0, ((Size.z / Mathf.Floor (ZSegments)) * (ic + 1)));

					//Create stationary along x axis
					for (int id = 1; id < Mathf.Floor ((Size.x / Mathf.Floor (XSegments)) / StationaryLengthX); id++) 
					{
						//Create stationary along z axis
						for (int ie = 1; ie < Mathf.Floor ((Size.z / Mathf.Floor (ZSegments)) / StationaryLengthZ); ie++) 
						{
							GameObject _Stationary = Instantiate (Stationary, SavedPos + new Vector3 (StationaryLengthX  * id, 0, -StationaryLengthZ  * ie), ChunksContainer.transform.GetChild (i).rotation);
							_Stationary.name = "Stationary";
							_Stationary.transform.parent = ChunksContainer.transform.GetChild(i);
						}	
					}
				}
			}
		}
	}

	//Copy template floor and stack atop each other
	public void FloorDuplication ()
	{
		for (int i = 1; i < HowManyFloors; i++)
		{
			GameObject _floor = Instantiate (FloorContainer, FloorContainer.transform.position + new Vector3(0,FloorHeight * i,0), FloorContainer.transform.rotation, transform);
			_floor.transform.parent = MultiFloorContainer.transform;
		}

		DestroyImmediate (MultiFloorContainer.transform.GetChild (HowManyFloors - 1).GetChild (4).gameObject);
		GameObject _roof = Instantiate (FloorPlane, Foundation.transform.position + new Vector3 (0, (FloorHeight * HowManyFloors), 0), Foundation.transform.rotation);
		_roof.transform.localScale = FoundationSize;// new Vector3 (Foundation.transform.lossyScale.x, Foundation.transform.lossyScale.y, Foundation.transform.lossyScale.z);
		_roof.GetComponent<MeshRenderer> ().material = CorridorFloor;
		_roof.transform.parent = transform;
		RoofPiece = _roof;
	}

	public void CapRoof ()
	{

	}
		
		
}
