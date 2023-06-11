using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Xml.Schema;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using System.Security.Cryptography;
using UnityEngine.AI;
using System.Xml.Serialization;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.Device;
using System.Drawing;
using System.IO;
using System.Reflection;

public class LevelGenerator : MonoBehaviour
{
    enum LevelTile {empty, floor, wall, bottomWall, light, shop};
    LevelTile[,] grid;
    struct RandomWalker {
		public Vector2 dir;
		public Vector3 pos;
	}
    List<RandomWalker> walkers;
	List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
	List<Vector2> enemySpawnPoints = new List<Vector2>();
	List<List<(int, int)>> rooms = new List<List<(int,int)>>();
	List<List<int>> gridValues = new List<List<int>>();

	public GameObject LightProbeGroup;
	public GameObject[] smallWallDecorations;
    public GameObject[] largeWallDecorations;
	public GameObject[] bedProp;
    public GameObject[] bedroomSmallProps;
    public GameObject[] bedroomWallProps;
    public GameObject[] studyProps;
    public GameObject[] studyWallProps;
    public GameObject[] eatinghallTableProps;
    public GameObject[] eatinghallSmallProps;
    public GameObject[] libraryProps;
    public GameObject[] libraryWallProps;
    public GameObject[] cobwebs;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] bottomWallTiles;
	public GameObject[] lights;
	public GameObject[] enemyTypes;
	public GameObject[] doors;
    public GameObject exit;
    public GameObject player;
	public GameObject level;
	public GameObject chest;
	public GameObject shop;

    public GameObject virtualCamera;

    public int levelWidth;
    public int levelHeight;
	public float percentToFill = 0.2f; 
	public float chanceWalkerChangeDir = 0.5f;
    public float chanceWalkerSpawn = 0.05f;
    public float chanceWalkerDestoy = 0.05f;
    public int maxWalkers = 10;
    public int iterationSteps = 100000;
	public int lightRange = 5;
	public int minEnemySpawns = 1;
	public int maxEnemySpawns = 5;
	int shopx;
	int shopz;

	int chestsToSpawn;
    int currentChests = 0;

    void Awake() {
        Setup();
		CreateWalls();
        CreateFloors();
		FindRooms();
		FixRooms();
        //CreateBottomWalls();
        CreateLights();
		CreateEnemySpawns();
        SpawnShop();
        SpawnLevel();
        DecorateRooms();
        SpawnPlayer();
        SpawnExit();
		SpawnEnemies();
		SpawnChest();
		BuildSurfaces();
		BuildNavMesh();
		LightProbeGroup.SetActive(true);
    }

    void Setup() {
        // prepare grid
        grid = new LevelTile[levelWidth, levelHeight];
        for (int x = 0; x <= levelWidth - 1; x++)
        {
			gridValues.Add(new List<int>());
            for (int z = 0; z <= levelHeight - 1; z++)
            {
				gridValues[x].Add(0);
            }
        }
		Debug.Log("Rows = " +gridValues.Count);
        for (int x = 0; x < levelWidth - 1; x++){
			for (int z = 0; z < levelHeight - 1; z++){ 
				grid[x, z] = LevelTile.empty;
			}
		}

        //generate first walker
		walkers = new List<RandomWalker>();
		RandomWalker walker = new RandomWalker();
		walker.dir = RandomDirection();
		Vector3 pos = new Vector3(Mathf.RoundToInt(levelWidth/ 2.0f),0, Mathf.RoundToInt(levelHeight/ 2.0f));
		walker.pos = pos;
		walkers.Add(walker);
        chestsToSpawn = ((levelHeight * levelWidth) / 200);
    }

	void RemoveFloors()
	{
        for (int x = 0; x < levelWidth - 1; x++)
        {
            for (int z = 0; z < levelHeight - 1; z++)
            {
				if (gridValues[x][z] >= 2)
					grid[x, z] = LevelTile.empty;
            }
        }
    }

	void SpawnShop()
	{
        int x = 0;
		int z = 1;
		bool shopSpawned = false;
        if (grid[x + 1, z] == LevelTile.floor)
        {
            grid[x, z] = LevelTile.shop;
            shopSpawned = true;
        }
        else
        {
            grid[x + 1, z] = LevelTile.floor;
			if (grid[x + 2, z] == LevelTile.wall)
				grid[x + 2, z] = LevelTile.floor;
            if (grid[x + 3, z] == LevelTile.wall)
                grid[x + 3, z] = LevelTile.floor;
            if (grid[x + 4, z] == LevelTile.wall)
                grid[x + 4, z] = LevelTile.floor;
			//
            if (grid[x + 1, z + 1] == LevelTile.wall)
                grid[x + 1, z + 1] = LevelTile.floor;
            if (grid[x + 2, z + 1] == LevelTile.wall)
                grid[x + 2, z + 1] = LevelTile.floor;
            if (grid[x + 3, z + 1] == LevelTile.wall)
                grid[x + 3, z + 1] = LevelTile.floor;
            if (grid[x + 4, z + 1] == LevelTile.wall)
                grid[x + 4, z + 1] = LevelTile.floor;
            //
            if (grid[x + 1, z + 2] == LevelTile.wall)
                grid[x + 1, z + 2] = LevelTile.floor;
            if (grid[x + 2, z + 2] == LevelTile.wall)
                grid[x + 2, z + 2] = LevelTile.floor;
            if (grid[x + 3, z + 2] == LevelTile.wall)
                grid[x + 3, z + 2] = LevelTile.floor;
            if (grid[x + 4, z + 2] == LevelTile.wall)
                grid[x + 4, z + 2] = LevelTile.floor;
			SpawnShop();
        }
        /*
		bool shopSpawned = false;
        for (int z = 1; z < levelHeight; z++)
        {
            int x = 0;
            if (grid[x+1, z] == LevelTile.floor)
			{
				grid[x, z] = LevelTile.shop;
				shopSpawned = true;
				break;
			}
			else
			{
				grid[x + 1, z] = LevelTile.floor;
				z--;
			}
        }*/
        /*
		if(!shopSpawned)
		{
			int x = levelWidth - 1;
            for (int z = 0; z < levelHeight; z++)
            {
                if (grid[x - 1, z] == LevelTile.floor)
                {
                    grid[x, z] = LevelTile.shop;
                    shopSpawned = true;
                    break;
                }
            }
        }*/
    }

	void DecorateRooms()
	{
		for(int i = 0; i < rooms.Count; i++)
		{
			if (rooms[i].Count > 60)
			{
				DecorateEatingHall(rooms[i]);
                DecorateWalls(rooms[i], smallWallDecorations);
            }
			else {
				//Small rooms
				switch (Random.Range(0, 3))
				{
					case 0:
                        DecorateBedroom(rooms[i]);
                        DecorateWalls(rooms[i], bedroomWallProps);
                        break;
					case 1:
						DecorateStudy(rooms[i]);
						DecorateWalls(rooms[i], studyWallProps);
						break;
					case 2:
						DecorateLibrary(rooms[i]);
                        DecorateWalls(rooms[i], libraryWallProps);
                        break;
				}
			}
        }
	}

    void DecorateBedroom(List<(int, int)> room)
	{
		bool hasBed = false;
        for (int i = 0; i < room.Count; i++)
        {
            if (Random.Range(0, 100) > 90)
            {
				if (Random.Range(0, 100) > 50 && !hasBed)
				{
					PlaceProp(room[i].Item1, room[i].Item2, bedProp);
					hasBed = true;
				}
				else
					PlaceProp(room[i].Item1, room[i].Item2, bedroomSmallProps);
            }
        }
    }
    void DecorateStudy(List<(int, int)> room)
	{
        for (int i = 0; i < room.Count; i++)
        {
            if (Random.Range(0, 100) > 80)
            {
                PlaceProp(room[i].Item1, room[i].Item2, studyProps);
            }
        }
    }
    void DecorateLibrary(List<(int, int)> room)
	{
        for (int i = 0; i < room.Count; i++)
        {
            if (Random.Range(0, 100) > 80)
            {
                PlaceProp(room[i].Item1, room[i].Item2, libraryProps);
            }
        }
    }


    void DecorateEatingHall(List<(int,int)> room)
	{
        for (int i = 0; i < room.Count; i++)
        {
			if (Random.Range(0, 100) > 90)
			{
				if (Random.Range(0, 100) > 50)
					PlaceProp(room[i].Item1, room[i].Item2, eatinghallTableProps);
				else
					PlaceProp(room[i].Item1, room[i].Item2, eatinghallSmallProps);
			}
        }
    }

    void PlaceProp(int x, int z, GameObject[] props)
	{
		int index = Random.Range(0, props.Length);
        if (x > 0 && x < levelWidth-1 && z > 0 && z < levelHeight-1)
		{
            if (grid[x, z + 1] == LevelTile.floor)
            {
                if (grid[x, z - 1] == LevelTile.floor)
                {
					Instantiate(props[index], new Vector3((x * 2.5f), -1, (z * 2.5f) + 1.15f), Quaternion.LookRotation(Vector3.right));
                }
            }
            if (grid[x + 1, z] == LevelTile.wall)
            {
                if (grid[x - 1, z] == LevelTile.wall)
                {
                    Instantiate(props[index], new Vector3((x * 2.5f), -1, (z * 2.5f) + 1.15f), Quaternion.LookRotation(Vector3.right));
                }
            }
        }
    }

	void DecorateWalls(List<(int,int)> room, GameObject[] props)
	{
		//Find borders
		for(int i = 0; i < room.Count; i++)
		{
			DecorateBorder(room[i].Item1, room[i].Item2, props);
		}
	}

	void DecorateBorder(int x,int z, GameObject[] props)
	{
		if (x > 0 && x < levelWidth && z > 0 && z < levelHeight)
		{
			bool decorate = (Random.Range(0, 100) > 80);
			bool cobweb = (Random.Range(0, 100) > 65);
			int index = Random.Range(0, props.Count());
			int cobwebIndex = Random.Range(0, cobwebs.Count());
			//check neighbors for wall
			if (grid[x, z + 1] == LevelTile.wall)
			{
				if(decorate)
					Instantiate(props[index], new Vector3((x*2.5f), .5f, (z*2.5f)+1.15f), Quaternion.LookRotation(Vector3.right));
				if(cobweb)
					Instantiate(cobwebs[cobwebIndex], new Vector3((x * 2.5f), 3, (z * 2.5f) + 1.15f), Quaternion.LookRotation(Vector3.left));
            }
			if (grid[x, z - 1] == LevelTile.wall)
			{
                if (decorate)
					Instantiate(props[index], new Vector3((x * 2.5f), .5f, (z * 2.5f) - 1.15f), Quaternion.LookRotation(Vector3.left));
                if (cobweb)
                    Instantiate(cobwebs[cobwebIndex], new Vector3((x * 2.5f), 3, (z * 2.5f) - 1.15f), Quaternion.LookRotation(Vector3.right));
            }
            if (grid[x + 1, z] == LevelTile.wall)
			{
                if (decorate)
					Instantiate(props[index], new Vector3((x * 2.5f) + 1.15f, .5f, (z * 2.5f)), Quaternion.LookRotation(Vector3.back));
                if (cobweb)
                    Instantiate(cobwebs[cobwebIndex], new Vector3((x * 2.5f) + 1.15f, 3, (z * 2.5f)), Quaternion.LookRotation(Vector3.forward));
            }
			if (grid[x - 1, z] == LevelTile.wall)
			{
                if (decorate)
					Instantiate(props[index], new Vector3((x * 2.5f) - 1.15f, .5f, (z * 2.5f)), Quaternion.LookRotation(Vector3.forward));
                if (cobweb)
                    Instantiate(cobwebs[cobwebIndex], new Vector3((x * 2.5f) - 1.15f, 3, (z * 2.5f)), Quaternion.LookRotation(Vector3.back));
            }
		}
	}

	void OpenRooms(List<(int,int)> nodes)
		{
			//Open paths between rooms
		bool[] isRoomConnected = new bool[nodes.Count - 1];
			for (int i = 0; i < nodes.Count-1; i++)
			{
				bool isDiagonal = false;
			Vector3 start = new Vector3(nodes[i].Item1, 0.5f, nodes[i].Item2);
			Vector3 end = new Vector3(nodes[i+1].Item1, 0.5f, nodes[i+1].Item2);
			Vector3 path = new Vector3();
			float angle = Vector3.Angle(start, end);
			if((angle > 30 && angle < 60) || (angle > 120 && angle < 150))
			{
				isDiagonal = true;
			}
			while (path != end)
			{
				path = Vector3.MoveTowards(start, end, 1);
				if (grid[(int)path.x, (int)path.z] == LevelTile.wall)
				{
                    grid[(int)path.x, (int)path.z] = LevelTile.floor;
                }
				if(isDiagonal)
				{
                    if (path.x > 0 && path.x < levelWidth && path.z > 0 && path.z < levelHeight)
					{
                        //check neighbors for wall
                        if (grid[(int)path.x, (int)path.z + 1] == LevelTile.wall)
                        {
                            grid[(int)path.x, (int)path.z] = LevelTile.floor;
                        }
                        if (grid[(int)path.x, (int)path.z - 1] == LevelTile.wall)
                        {
                            grid[(int)path.x, (int)path.z] = LevelTile.floor;
                        }
                        if (grid[(int)path.x + 1, (int)path.z] == LevelTile.wall)
                        {
                            grid[(int)path.x, (int)path.z] = LevelTile.floor;
                        }
                        if (grid[(int)path.x - 1, (int)path.z] == LevelTile.wall)
                        {
                            grid[(int)path.x, (int)path.z] = LevelTile.floor;
                        }
                    }
                }
				start = path;
			}
            /*
			if (!isRoomConnected[i])
			{
				isRoomConnected[i] = true;
				int connectRoom = Random.Range(0, isRoomConnected.Length);
				isRoomConnected[connectRoom] = true;
			}*/
        }
    }

	void FixRooms()
	{
		List<(int,int)> nodes = new List<(int,int)>();
        for(int i = 0; i < rooms.Count; i++)
		{
            if (rooms[i].Count < 12)
			{
				for(int l = 0; l < rooms[i].Count; l++)
				{
					grid[rooms[i][l].Item1, rooms[i][l].Item2] = LevelTile.wall;
					//OpenRooms(i, l);
                }
			}
			//else
			//{
			Debug.Log(i);
			nodes.Add(rooms[i][0]);
			//}
		}
		Debug.Log("Rooms " + rooms.Count);
		Debug.Log("nodes" + nodes.Count);
		OpenRooms(nodes);
    }

    void FindRooms()
	{
		int roomCounter = 2;
        for (int x = 0; x < levelWidth - 1; x++)
        {
            for (int z = 0; z < levelHeight - 1; z++)
            {
                if (grid[x, z] == LevelTile.floor)
                {
					/*
                    if (!IsTileLoaded(x, z))
                    {
						FloodFill(x, z, roomCounter);
                    }*/
					if (gridValues[x][z] == 0)
					{
						//Debug.Log("X/Z is " + x + "/" + z);
						FloodFill(x, z, roomCounter);
						roomCounter++;
                        rooms.Add(new List<(int,int)>());
                    }
                }
            }
        }
        for (int x = 0; x < levelWidth - 1; x++)
        {
            for (int z = 0; z < levelHeight - 1; z++)
            {
                if (gridValues[x][z] >= 2)
					rooms[(gridValues[x][z]) - 2].Add((x, z));
            }
        }
        Debug.Log(roomCounter);
    }

    private void FloodFill(int x, int z, int roomCounter)
    {
        if (x > 0 && x < levelWidth && z > 0 && z < levelHeight)
		{
			if (gridValues[x][z] == 0)
			{
				gridValues[x][z] = roomCounter;
				FloodFill(x + 1, z + 1, roomCounter);
				FloodFill(x + 1, z,     roomCounter);
				FloodFill(x + 1, z - 1, roomCounter);
				FloodFill(x,     z + 1, roomCounter);
                FloodFill(x,     z - 1, roomCounter);
                FloodFill(x - 1, z + 1, roomCounter);
                FloodFill(x - 1, z,     roomCounter);
                FloodFill(x - 1, z - 1, roomCounter);
            }
		}
    }

	void CreateWalls() {
		int iterations = 0;
		do{
			//create wall at position of every Walker
			foreach (RandomWalker walker in walkers){
				grid[(int)walker.pos.x,(int)walker.pos.z] = LevelTile.wall;
				gridValues[(int)walker.pos.x][(int)walker.pos.z] = 1;
            }

			//chance: destroy Walker
			int numberChecks = walkers.Count;
			for (int i = 0; i < numberChecks; i++) {
				if (Random.value < chanceWalkerDestoy && walkers.Count > 1){
					walkers.RemoveAt(i);
					break;
				}
			}

			//chance: Walker pick new direction
			for (int i = 0; i < walkers.Count; i++) {
				if (Random.value < chanceWalkerChangeDir){
					RandomWalker thisWalker = walkers[i];
					thisWalker.dir = RandomDirection();
					walkers[i] = thisWalker;
				}
			}

			//chance: spawn new Walker
			numberChecks = walkers.Count;
			for (int i = 0; i < numberChecks; i++){
				if (Random.value < chanceWalkerSpawn && walkers.Count < maxWalkers) {
					RandomWalker walker = new RandomWalker();
					walker.dir = RandomDirection();
					walker.pos = walkers[i].pos;
					walkers.Add(walker);
				}
			}

			//move Walkers
			for (int i = 0; i < walkers.Count; i++){
				RandomWalker walker = walkers[i];
				walker.pos.x += walker.dir.x;
				walker.pos.z += walker.dir.y;
				walkers[i] = walker;				
			}

			//avoid boarder of grid
			for (int i =0; i < walkers.Count; i++){
				RandomWalker walker = walkers[i];
				walker.pos.x = Mathf.Clamp(walker.pos.x, 1, levelWidth-2);
				walker.pos.z = Mathf.Clamp(walker.pos.z, 1, levelHeight-2);
				walkers[i] = walker;
			}

			//check to exit loop
			if ((float)NumberOfWalls() / (float)grid.Length > percentToFill){
				break;
			}
			iterations++;
		} while(iterations < iterationSteps);

		//Create floor collider
		level.transform.localScale = new Vector3(levelWidth, 0, levelHeight);
		level.transform.position = new Vector3(levelWidth / 2, -1, levelHeight / 2);
		//level.transform.Rotate(90, 0, 0);

		//Create border Walls
		for (int i = 0; i < levelWidth; i++)
		{
			if(i == 0)
			{
				for (int l = 0; l < levelHeight; l++)
				{
					grid[i, l] = LevelTile.wall;
					gridValues[i][l] = 1;
				}
			}
			if(i == levelWidth-1)
			{
                for (int l = 0; l < levelHeight; l++)
                {
                    grid[i, l] = LevelTile.wall;
                    gridValues[i][l] = 1;
                }
            }
			grid[i, 0] = LevelTile.wall;
			gridValues[i][0] = 1;
            grid[i, levelHeight-1] = LevelTile.wall;
			gridValues[i][levelHeight - 1] = 1;
		}
	}

	void CreateLights()
	{
		for (int x = 0; x < levelWidth - 1; x++)
		{
			for (int z = 0; z < levelHeight - 1; z++)
			{
				if (grid[x, z] == LevelTile.floor)
				{
					/* Attempt to randomly generate light positions
					if (!nearbyLight(x, z))
					{
						grid[x, z] = LevelTile.light;
					}
					*/
					grid[x, z] = LevelTile.light;
				}
                if (z + 5 < levelHeight)
                    z += 5;
            }
            if (x + 5 < levelWidth)
                x += 5;
        }
	}

	void CreateEnemySpawns()
	{
        for (int x = 0; x < levelWidth - 1; x++)
        {
            for (int z = 0; z < levelHeight - 1; z++)
            {
                if (grid[x, z] == LevelTile.floor)
                {
					if (enemySpawnPoints.Count < maxEnemySpawns)
					{
						if (Random.Range(0, 100) > 98)
						{
							enemySpawnPoints.Add(new Vector2(x, z));
						}
					}
                }
				if (enemySpawnPoints.Count == maxEnemySpawns)
					break;
            }
            if (enemySpawnPoints.Count == maxEnemySpawns)
                break;
        }
    }

    private bool nearbyLight(int x, int z)
    {
        int xMin;
		if (x <= 5)
		{
			xMin = -x;
		}
		else
			xMin = -5;
        int zMin;
		if (z <= 5)
			zMin = -z;
		else
			zMin = -5;
        int xIterator = x;
        int zIterator = z;
        for (xIterator = (xIterator - xMin); xIterator <= lightRange; xIterator++)
        {
            for (zIterator = (zIterator - zMin); zIterator <= lightRange; zIterator++)
            {
                if (grid[xIterator, zIterator] == LevelTile.light)
                {
                    Debug.Log("true");
                    return true;
                }
            }
        }
        Debug.Log("false");
        return false;
    }

    void CreateFloors(){
		for (int x = 0; x < levelWidth-1; x++) {
			for (int z = 0; z < levelHeight-1; z++) {
				if (grid[x,z] == LevelTile.empty) {
					grid[x, z] = LevelTile.floor;
					/*
					if (grid[x,z+1] == LevelTile.empty) {
						grid[x,z+1] = LevelTile.floor;
					}

					if (grid[x,z-1] == LevelTile.empty) {
						grid[x,z-1] = LevelTile.floor;
					}
					if (grid[x+1,z] == LevelTile.empty) {
						grid[x+1,z] = LevelTile.floor;
					}
					if (grid[x-1,z] == LevelTile.empty) {
						grid[x-1,z] = LevelTile.floor;
					}

                    if (grid[x - 1, z - 1] == LevelTile.empty) {
                        grid[x - 1, z - 1] = LevelTile.floor;
                    }
                    if (grid[x - 1, z + 1] == LevelTile.empty) {
                        grid[x - 1, z + 1] = LevelTile.floor;
                    }
                    if (grid[x + 1, z + 1] == LevelTile.empty) {
                        grid[x + 1, z + 1] = LevelTile.floor;
                    }
                    if (grid[x + 1, z - 1] == LevelTile.empty) {
                        grid[x + 1, z - 1] = LevelTile.floor;
                    }*/
                }
            }
		}
	}



	void CreateBottomWalls() {
		for (int x = 0; x < levelWidth; x++) {
			for (int z = 1; z < levelHeight; z++) {
				if (grid[x, z] == LevelTile.floor && grid[x, z - 1] == LevelTile.wall) {
                    grid[x, z] = LevelTile.bottomWall;
                }
            }
		}
	}

	void SpawnLevel(){
		for (int x = 0; x < levelWidth; x++) {
			for (int z = 0; z < levelHeight; z++) {
				switch (grid[x, z])
				{
					case LevelTile.empty:
						break;
					case LevelTile.floor:
						GameObject floor = floorTiles[Random.Range(0, floorTiles.Length)];
						Spawn(new Vector3(x, 0, z), Quaternion.LookRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 0)), floor);
						break;
					case LevelTile.wall:
						GameObject wall = wallTiles[Random.Range(0, wallTiles.Length)];
						Spawn(new Vector3(x, 1, z), Quaternion.LookRotation(new Vector3(0, -90, 0), new Vector3(0, 0, 0)), wall);
						break;
					case LevelTile.bottomWall:
						GameObject bottomWall = bottomWallTiles[Random.Range(0, bottomWallTiles.Length)];
						Spawn(new Vector3(x, 1, z), Quaternion.LookRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 0)), bottomWall);
						break;
					case LevelTile.light:
						GameObject light = lights[Random.Range(0, lights.Length)];
						Spawn(new Vector3(x, 1, z), Quaternion.LookRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 0)), light);
						break;
					case LevelTile.shop:
						if(x == 0)
                            shop.transform.rotation = Quaternion.LookRotation(Vector3.back);
						else
                            shop.transform.rotation = Quaternion.LookRotation(Vector3.forward);
                        shop.transform.position = new Vector3(x * 2.5f, 0, z * 2.5f);
                        shop.transform.rotation = Quaternion.LookRotation(Vector3.back);
						Debug.Log("Spawning shop");
                        break;
				}
			}
		}
	}    

	Vector2 RandomDirection(){
		int choice = Mathf.FloorToInt(Random.value * 3.99f);
		switch (choice){
			case 0:
				return Vector2.up;
			case 1:
				return Vector2.down;
			case 2:
				return Vector2.right;
			default:
				return Vector2.left;
		}
	}

	int NumberOfWalls() {
		int count = 0;
		foreach (LevelTile space in grid){
			if (space == LevelTile.wall){
				count++;
			}
		}
		return count;
	}

    void SpawnPlayer() {
		Vector3 pos = new Vector3(Mathf.RoundToInt(levelWidth / 2.0f), 0.52f,
										Mathf.RoundToInt(levelHeight / 2.0f));
		GameObject playerObj = GameObject.Find("Player");
		playerObj.transform.position = pos;
		playerObj.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
		//CinemachineVirtualCamera vCam = virtualCamera.GetComponent<CinemachineVirtualCamera>();
        //vCam.m_Follow = playerObj.transform;
		//vCam.LookAt = playerObj.transform;
		//vCam.transform.parent = playerObj.transform;
	}

    public void SpawnExit() {

        Vector3 playerPos = new Vector3(Mathf.RoundToInt(levelWidth/ 2.0f),0,
                                Mathf.RoundToInt(levelHeight/ 2.0f));
        Vector3 exitPos = playerPos;
        float exitDistance = 0f;

		int x = Random.Range(1, levelWidth);
		int z = Random.Range(1, levelHeight);
		while (grid[x,z] != LevelTile.floor && grid[x,z+1] != LevelTile.wall)
		{
            x = Random.Range(1, levelWidth);
            z = Random.Range(1, levelHeight);
        }

        Spawn(new Vector3(exitPos.x, 0.001f, exitPos.z), Quaternion.LookRotation(Vector3.down), exit);
    }  
	
	public void SpawnEnemies()
	{
		for(int i = 0; i < enemySpawnPoints.Count; i++)
		{
			Debug.Log("Index is " + i);
			int index = Random.Range(0, enemyTypes.Count());
			Spawn(new Vector3(enemySpawnPoints[i].x, 0, enemySpawnPoints[i].y), Quaternion.identity, enemyTypes[index]);
		}
	}

	public void SpawnChest()
	{
		int spawnX = (int)(Random.Range(1, levelWidth) * 2.5);
		int spawnZ = (int)(Random.Range(1, levelHeight) * 2.5);
		if (grid[spawnX, spawnZ] == LevelTile.floor)
		{
			GameObject spawnedChest = Instantiate(chest);
			currentChests++;
			spawnedChest.transform.position = new Vector3(spawnX*2, 0, spawnZ*2);
            Ray ray = new Ray(spawnedChest.transform.position, -Vector3.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                spawnedChest.transform.position = hit.point;
            }
			spawnedChest.name = "Chest";
        }
		else
			SpawnChest();
	}

	void Spawn(Vector3 pos, Quaternion rot, GameObject toSpawn) {
        float scale = 2.5f;
		Vector3 spawnScaled = new Vector3(pos.x * scale, pos.y, pos.z * scale);
        Instantiate(toSpawn, spawnScaled, Quaternion.LookRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 0)));
		if(toSpawn.tag == "Obstacle")
		{
			//toSpawn.GetComponent<NavMeshObstacle>().Bake
		}
	}

	void BuildSurfaces()
	{
		/*foreach (GameObject floor in GameObject.FindGameObjectsWithTag("Floor"))
		{
			NavMeshSurface i = new NavMeshSurface();
			
			surfaces.Add(i);
		}*/
	}

	void BuildNavMesh()
	{
		/*for(int i = 0; i < surfaces.Count; i++)
		{
			surfaces[i].BuildNavMesh();
		}*/
		level.GetComponent<NavMeshSurface>().BuildNavMesh();
		level.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }
}
