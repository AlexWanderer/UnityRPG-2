﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/*
 * Global:
 * MyVariableBanana
 * 
 * Local:
 * myVariableBanana
 * 
 * Private:
 * _MyVariableBanana
 * 
 */

public class TileMapData : MonoBehaviour {
	private GameManager gm;
	private int _SizeX;
	private int _SizeZ;
	private Tile[,] _MapData;
	private List<Room> _Rooms;
	private int _MyMaxFails = 10;
	private int _MyMaxRooms = 25;
	private Tile floor, wall, stone, unknown;

	public int MaxFails {
		get {
			return _MyMaxFails;
		}
		set {
			_MyMaxFails = value;
		}
	}
	public int MaxRooms {
		get {
			return _MyMaxRooms;
		}
		set {
			_MyMaxRooms = value;
		}
	}

	public Tile[,] MapData {
		get {
			return _MapData;
		}
		set {
			_MapData = value;
		}
	}
	public List<Room> Rooms {
		get {
			return _Rooms;
		}
		set {
			_Rooms = value;
		}
	}

	void Awake (){
		gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}

	void Start(){
		floor = new Tile(1,Tile.Kind.Floor,true,"Floor",0);
		wall = new Tile(2,Tile.Kind.Wall,false,"Wall",1);
		stone = new Tile(3,Tile.Kind.Stone,false,"Stone",2);
		unknown = new Tile(0,Tile.Kind.Unknown,false,"Unknown",99);
	}

	public void CreateTileMapData(int sizeX, int sizeZ) {
		this._SizeX = sizeX;
		this._SizeZ = sizeZ;

		_MapData = new Tile[sizeX,sizeZ];

		for(int x=0;x<sizeX;x++) {
			for(int y=0;y<sizeZ;y++) {
				_MapData[x,y] = stone;
			}
		}

		_Rooms = new List<Room>();
		Room r;
		
		while(_Rooms.Count < MaxRooms) {
			int rsx = Random.Range(4,14);
			int rsy = Random.Range(4,10);
			
			r = new Room();
			r.left = Random.Range(0, _SizeX - rsx);
			r.top = Random.Range(0, _SizeZ - rsy);
			r.width = rsx;
			r.height = rsy;
			
			if(!RoomCollides(r)) {
				_Rooms.Add (r);
			}else {
				MaxFails--;
				if(MaxFails <=0)
					break;
			}		
		}

		foreach(Room rr in _Rooms) {
			MakeRoom(rr);
		}

		for(int i=0; i < _Rooms.Count; i++) {
			if(!_Rooms[i].isConnected) {
				int j = Random.Range(1, _Rooms.Count);
				MakeCorridor(_Rooms[i], _Rooms[(i + j) % _Rooms.Count ]);
			}
		}
		foreach(Room rr in _Rooms) {
			MakeDoors(rr);
		}

		MakeWalls();
	}

	bool RoomCollides(Room r) {
		foreach(Room r2 in _Rooms) {
			if(r.CollidesWith(r2)) {
				return true;
			}
		}	
	return false;
	}

	void MakeRoom(Room r) {	
		for(int x=0; x < r.width; x++) {
			for(int z=0; z < r.height; z++){
				if(x==0 || x == r.width-1 || z==0 || z == r.height-1 || x<0 || z< 0) {
					_MapData[r.left+x,r.top+z] = wall;
				}
				else {
					_MapData[r.left+x,r.top+z] = floor;
				}
			}
		}	
	}
	
	void MakeCorridor(Room r1, Room r2) {
		int x = r1.centerX;
		int z = r1.centerZ;
		
		while( x != r2.centerX ) {
			_MapData[x,z] = floor;
			x += x < r2.centerX ? 1 : -1;
		}
		while( z != r2.centerZ ) {
			_MapData[x,z] = floor;
			z += z < r2.centerZ ? 1 : -1;
		}
		r1.isConnected = true;
		r2.isConnected = true;	
	}
	
	void MakeWalls() {
		for(int x=0; x< _SizeX;x++) {
			for(int z=0; z< _SizeZ;z++) {
				if(_MapData[x,z]==stone && HasAdjacentFloor(x,z)) {
					_MapData[x,z]= wall;
				}
			}
		}
	}

	void MakeDoors(Room r){
/* Check floor tiles outside of room
 * using HasAdjacentFloor
 * Place door where there are 4 Adjacent floor tile
 */
		for(int x=0; x < r.width; x++) {
			for(int z=0; z < r.height; z++){
				if(HasAdjacentFloor(x,z)){
					_MapData[x,z] = unknown;
				}
			}
		}
	}
	
	bool HasAdjacentFloor(int x, int z) {
		if( x > 0 && _MapData[x-1,z] == floor )
			return true;
		if( x < _SizeX-1 && _MapData[x+1,z] == floor )
			return true;
		if( z > 0 && _MapData[x,z-1] == floor )
			return true;
		if( z < _SizeZ-1 && _MapData[x,z+1] == floor )
			return true;
		if( x > 0 && z > 0 && _MapData[x-1,z-1] == floor )
			return true;
		if( x < _SizeX - 1 && z > 0 && _MapData[x+1,z-1] == floor )
			return true;
		if( x > 0 && z < _SizeZ-1 && _MapData[x-1,z+1] == floor )
			return true;
		if( x < _SizeX-1 && z < _SizeZ-1 && _MapData[x+1,z+1] == floor )
			return true;

		return false;
	}
}