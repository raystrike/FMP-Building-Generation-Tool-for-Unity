using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(S_GenerateFloor))]
public class S_GFEditor : Editor {

	private bool BuildingEditing = false;

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI ();

		S_GenerateFloor floorgen = (S_GenerateFloor)target;

		floorgen.CorridorFloor = (Material)EditorGUILayout.ObjectField ("Corridor Floor Material", floorgen.CorridorFloor, typeof(Material), false);
		floorgen.LobbyFloor = (Material)EditorGUILayout.ObjectField ("Stairwell Lobby Floor Material", floorgen.LobbyFloor, typeof(Material), false);
		floorgen.SocialFloor = (Material)EditorGUILayout.ObjectField ("Social Floor Material", floorgen.SocialFloor, typeof(Material), false);
		floorgen.RoomFloor = (Material)EditorGUILayout.ObjectField ("Room Floor Material", floorgen.RoomFloor, typeof(Material), false);
		floorgen.ExteriorWalls = (Material)EditorGUILayout.ObjectField ("External Walls Material", floorgen.ExteriorWalls, typeof(Material), false);
		floorgen.InteriorWalls = (Material)EditorGUILayout.ObjectField ("Internal Walls Material", floorgen.InteriorWalls, typeof(Material), false);

		floorgen.FloorThickness = EditorGUILayout.Slider ("Floor Thickness", floorgen.FloorThickness, 0.1f, 1);
		floorgen.XFoundationSize = EditorGUILayout.Slider ("X Size", floorgen.XFoundationSize, 1, 10);
		floorgen.ZFoundationSize = EditorGUILayout.Slider ("Z Size", floorgen.ZFoundationSize, 1, 10);

		if (GUILayout.Button ("Resize Base Foundation")) {
			floorgen.Reset ();
			floorgen.ResizeFoundation ();
		}


		floorgen.HowManyCorridors = Mathf.RoundToInt( EditorGUILayout.Slider ("Amount of Corridors", floorgen.HowManyCorridors, 1, 20));
		floorgen.CorridoorWidth = EditorGUILayout.Slider ("Corridor Width", floorgen.CorridoorWidth, 1, 100);
		floorgen.CorridorBoundarySafetiesPercentage = EditorGUILayout.Slider ("Corridor Boundary", floorgen.CorridorBoundarySafetiesPercentage, 10, 50);

		if (GUILayout.Button ("Generate Corridors")) 
		{
			//Reset Chunk
			floorgen.Reset ();
			floorgen.Foundation.transform.parent = floorgen.ChunksContainer.transform;
			floorgen.Foundation.SetActive (true);
			floorgen.FirstPass = true;
			floorgen.ExternalStairs = false;
			floorgen.HorizontalMainCorridor = false;
			floorgen.VerticalMainCorridor = false;
			floorgen.FoundationSize = floorgen.Foundation.transform.lossyScale;

			//Generate Floor
			for (int i = 0; i < floorgen.HowManyCorridors; i++)
			{
				floorgen.FindNextBigChunk ();
			}
				
		}
			
		floorgen.MinimumRoomX = EditorGUILayout.Slider ("Minimum X Room Size", floorgen.MinimumRoomX, 5, 100);
		floorgen.MinimumRoomZ = EditorGUILayout.Slider ("Minimum Z Room Size", floorgen.MinimumRoomZ, 5, 100);
		floorgen.WallWidth = EditorGUILayout.Slider ("Wall Width", floorgen.WallWidth, 0.1f, 5);
		floorgen.DoorWayBoundarySafetiesPercentage = EditorGUILayout.Slider ("Doorway Boundary", floorgen.DoorWayBoundarySafetiesPercentage, 10, 50);
		floorgen.RandomXWallPercentage = EditorGUILayout.Slider ("Likeliness (%) to Spawn X Walls", floorgen.RandomXWallPercentage, 0, 100);
		floorgen.RandomZWallPercentage = EditorGUILayout.Slider ("Likeliness (%) to Spawn Z Walls", floorgen.RandomZWallPercentage, 0, 100);


		if (GUILayout.Button ("Generate Rooms (Studio Layout)")) 
		{
			floorgen.CreateLobby ();
			floorgen.CreateRooms ();
		}

		floorgen.StationaryLengthX = EditorGUILayout.Slider ("Stationary X Length", floorgen.StationaryLengthX, 1, 20);
		floorgen.StationaryLengthZ = EditorGUILayout.Slider ("Stationary Z Length", floorgen.StationaryLengthZ, 1, 20);

		if (GUILayout.Button ("Generate Stationary (Furnished)")) 
		{
			
			floorgen.InsertStationary ();
		}

		if (GUILayout.Button ("Generate External Walls")) 
		{
			floorgen.ExternalWalls ();

		}

		floorgen.HowManyFloors = Mathf.RoundToInt( EditorGUILayout.Slider ("Amount of Floors", floorgen.HowManyFloors, 1, 100));

		if (GUILayout.Button ("Generate Floors")) 
		{
			//floorgen.ExternalWalls ();
			floorgen.FloorDuplication ();
		}

		if (GUILayout.Button ("Reset")) 
		{
			floorgen.Reset ();
			floorgen.Foundation.transform.parent = floorgen.ChunksContainer.transform;
			floorgen.Foundation.SetActive (true);
		}
	}


}
