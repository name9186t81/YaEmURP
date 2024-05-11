//use Rule tiles from Packages/com.unity.2d.tilemap.extras



//using UnityEngine;
//using UnityEngine.Tilemaps;

//namespace YaEm.Core
//{
//	[RequireComponent(typeof(Tilemap)), DisallowMultipleComponent()]
//	public sealed class TileMapShaper : MonoBehaviour
//	{
//		private enum Neighbours : byte
//		{
//			None = 0,

//			UpperLeft = 1,
//			UpperCentral = 2,
//			UpperRight = 4,

//			CentralLeft = 8,
//			CentralRight = 16,

//			DownLeft = 32,
//			DownCentral = 64,
//			DownRight = 128,

//			SelfUpperLeft = DownCentral | CentralRight & ~CentralLeft & ~UpperCentral,
//			SelfUpperCentral = DownCentral | CentralRight | CentralLeft & ~UpperCentral,
//			SelfUpperRight = DownCentral & ~CentralRight | CentralLeft & ~UpperCentral,

//			SelfCentralLeft = DownCentral | CentralRight & ~CentralLeft | UpperCentral,
//			SelfCentralCentral = DownCentral | CentralRight | CentralLeft | UpperCentral,
//			SelfCentralRight = DownCentral & ~CentralRight | CentralLeft | UpperCentral,

//			SelfDownLeft = ~DownCentral & CentralRight & ~CentralLeft | UpperCentral,
//			SelfDownCentral = ~DownCentral & CentralRight | CentralLeft | UpperCentral,
//			SelfDownRight = ~DownCentral & ~CentralRight & CentralLeft | UpperCentral
//		}

//		[SerializeField] private Sprite _compared;

//		[Space]
//		[SerializeField] private Sprite _upperLeft;
//		[SerializeField] private Sprite _upperCentral;
//		[SerializeField] private Sprite _upperRight;

//		[Space]
//		[SerializeField] private Sprite _centralLeft;
//		[SerializeField] private Sprite _central;
//		[SerializeField] private Sprite _centralRight;

//		[Space]
//		[SerializeField] private Sprite _downLeft;
//		[SerializeField] private Sprite _downCentral;
//		[SerializeField] private Sprite _downRight;

//		[Space]
//		[SerializeField] private Sprite _up;
//		[SerializeField] private Sprite _left;
//		[SerializeField] private Sprite _right;
//		[SerializeField] private Sprite _down;

//		private Tilemap _tileMap;

//		private void Start()
//		{
//			_tileMap = GetComponent<Tilemap>();

//			_tileMap.CompressBounds();

//			Neighbours neighbours = Neighbours.None;
//			TileData data = new TileData();
//			TileData checkData = new TileData();

//			for (int y = _tileMap.cellBounds.yMin; y < _tileMap.cellBounds.yMax; y++)
//			{
//				for (int x = _tileMap.cellBounds.xMin; x < _tileMap.cellBounds.xMax; x++)
//				{
//					for (int z = _tileMap.cellBounds.zMin; z < _tileMap.cellBounds.zMax; z++)
//					{
//						Vector3Int pos = new Vector3Int(x, y, z);

//						var tile = _tileMap.GetTile<TileBase>(pos);

//						var emptyTile = tile == null;

//						if (!emptyTile)
//							tile.GetTileData(pos, _tileMap, ref data);
//						else
//							continue;

//						if (data.sprite != _compared) continue;

//						neighbours = Neighbours.None;
//						for(int dx = -1; dx < 2; dx++)
//						{
//							for(int dy = -1; dy < 2; dy++)
//							{
//								if (dx == 0 && dy == 0 ||
//									Mathf.Abs(dx) == Mathf.Abs(dy)) continue;

//								int offsetX = dx + x;
//								int offsetY = dy + y;
//								if (offsetX < _tileMap.cellBounds.xMin || offsetX > _tileMap.cellBounds.xMax ||
//									offsetY < _tileMap.cellBounds.yMin || offsetY > _tileMap.cellBounds.yMax) continue;

//								Vector3Int offsetPos = Vector3Int.right * offsetX + Vector3Int.up * offsetY;
//								var offsetTile = _tileMap.GetTile<TileBase>(offsetPos);

//								if (offsetTile == null) continue;
//								offsetTile.GetTileData(offsetPos, _tileMap, ref checkData);

//								if (checkData.sprite != _compared) continue;

//								neighbours |= (Neighbours)(1 << ((dx + 1) + (dy + 1) * 3 + ((dy > -1 && dx > -1) ? -1 : 0)));
//							}
//						}

//						if((neighbours & ~Neighbours.SelfUpperLeft) != 0)
//						{
//							checkData.sprite = _upperLeft;
//							continue;
//						}

//						if ((neighbours & ~Neighbours.SelfUpperCentral) != 0)
//						{
//							checkData.sprite = _upperCentral;
//							continue;
//						}

//						if ((neighbours & ~Neighbours.SelfUpperRight) != 0)
//						{
//							checkData.sprite = _upperRight;
//							continue;
//						}

//						if ((neighbours & ~Neighbours.CentralLeft) != 0)
//						{
//							checkData.sprite = _centralLeft;
//							continue;
//						}

//						if ((neighbours & ~Neighbours.SelfCentralCentral) != 0)
//						{
//							checkData.sprite = _central;
//							continue;
//						}

//						if ((neighbours & ~Neighbours.SelfCentralRight) != 0)
//						{
//							checkData.sprite = _centralRight;
//							continue;
//						}

//						if ((neighbours & ~Neighbours.SelfDownLeft) != 0)
//						{
//							checkData.sprite = _downLeft;
//							continue;
//						}

//						if ((neighbours & ~Neighbours.SelfDownCentral) != 0)
//						{
//							checkData.sprite = _downCentral;
//							continue;
//						}

//						if ((neighbours & ~Neighbours.SelfCentralRight) != 0)
//						{
//							checkData.sprite = _downRight;
//							continue;
//						}
//					}
//				}
//			}
//		}
//	}
//}