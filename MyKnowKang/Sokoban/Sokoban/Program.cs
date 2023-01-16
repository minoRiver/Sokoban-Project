﻿using Sokoban;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace KMH_Sokoban
{
	public enum MapSpaceType
	{
		Pass            // 지나가는 곳..
		, DontPass      // 못지나가는 곳..
		, PlayerStand   // 플레이어가 있는 곳..
		, BoxStand      // 박스가 있는 곳..
		, Portal		// 포탈이 있는 곳..
		, Item			// 아이템이 있는 곳..
		, Trap          // 트랩이 있는 곳..
		, Bomb			// 폭탄이 있는 곳..
		, Arrow			// 화살이 있는 곳..
	}

	public enum EndingType
	{
		None,
		Clear,
		Die
	}

	class Program
	{
		static void Main()
		{
			bool isWin11 = false;
			while ( true )
			{
				Console.Clear();
				Console.WriteLine( "전체화면으로 실행해주세요" );
				for ( int i = 0; i < 5; ++i ) Console.WriteLine( "Win11 인 경우 1번, 아닌 경우 2번을 눌러주세요" );
                ConsoleKey key = Console.ReadKey().Key;
				if ( key == ConsoleKey.D1 )
				{
					isWin11 = true;
					break;
				}
				else if ( key == ConsoleKey.D2 )
				{
					isWin11 = false;
					break;
				}
            }

			// ------------------------------------------- 초기화(객체 생성 및 초기화).. -------------------------------------------
			#region Initialize

			// ============================================================================================================================================
			// 상수 초기화..
			// ============================================================================================================================================
			#region 상수 초기화

			// 초기 세팅 관련 상수 설정..
			const bool CURSOR_VISIBLE = false;                      // 커서를 숨긴다..
			const string TITLE_NAME = "Welcome To Liverpool";       // 타이틀을 설정한다..
			const ConsoleColor BACKGROUND_COLOR = ConsoleColor.Black;   // Background 색을 설정한다..
			const ConsoleColor FOREGROUND_COLOR = ConsoleColor.White;     // 글꼴색을 설정한다..

			// Map 관련 상수 설정..
			const int MAP_WIDTH = 50;
			const int MAP_HEIGHT = 22;
			const int MAP_RANGE_MIN_X = 1;
			const int MAP_RANGE_MIN_Y = 1;
			const int MAP_RANGE_MAX_X = MAP_RANGE_MIN_X + MAP_WIDTH;
			const int MAP_RANGE_MAX_Y = MAP_RANGE_MIN_Y + MAP_HEIGHT;
			// Map 관련 초기 세팅..
            const ConsoleColor BORDERLINE_COLOR = ConsoleColor.DarkRed;
			const string BORDERLINE_IMAGE_WIN11 = "▦";
			const string BORDERLINE_IMAGE_WIN10 = "W";
			string initBorderLineImage = (isWin11) ? BORDERLINE_IMAGE_WIN11 : BORDERLINE_IMAGE_WIN10;

			// Player 관련 상수 설정..
			const string PLAYER_IMAGE_WIN11 = "♀";
			const string PLAYER_IMAGE_WIN10 = "P";
			const ConsoleColor PLAYER_COLOR = ConsoleColor.Blue;
			// Player 초기 세팅..
			const int INITIAL_PLAYER_X = 1;
			const int INITIAL_PLAYER_Y = 1;
			const int INITIAL_PLAYER_HP = 10;
			const int INITIAL_PLAYER_MP = 500;
			string initPlayerImage = (isWin11) ? PLAYER_IMAGE_WIN11 : PLAYER_IMAGE_WIN10;

			// Box 관련 상수 설정.. 
			const int BOX_COUNT = 5;
			const string BOX_IMAGE_WIN11 = "◎";
            const string BOX_IMAGE_WIN10 = "B";
            const ConsoleColor BOX_COLOR = ConsoleColor.Yellow;
			// Box 초기 세팅..
			int[] INIT_BOXES_X = new int[BOX_COUNT] { 5, 5, 5, 5, 10 };
			int[] INIT_BOXES_Y = new int[BOX_COUNT] { 2, 3, 4, 5, 7 };
			string initBoxImage = (isWin11) ? BOX_IMAGE_WIN11 : BOX_IMAGE_WIN10;

			// Wall 관련 상수 설정..
			const int WALL_COUNT = 15;
			const string WALL_IMAGE_WIN11 = "▣";
            const string WALL_IMAGE_WIN10 = "W";
            const ConsoleColor WALL_COLOR = ConsoleColor.DarkRed;
			// Wall 초기 세팅..
			int[] INIT_WALLS_X = new int[WALL_COUNT] { 3, 3, 3, 42, 42, 42, 42, 42, 43, 44, 45, 46, 47, 48, 49 };
			int[] INIT_WALLS_Y = new int[WALL_COUNT] { 8, 7, 9, 18, 19, 20, 21, 17, 17, 17, 17, 17, 17, 17, 17 };
			string initWallImage = (isWin11) ? WALL_IMAGE_WIN11 : WALL_IMAGE_WIN10;

			// Goal 지점 관련 상수..
			const int GOAL_COUNT = BOX_COUNT;
			const string GOAL_IMAGE_WIN11 = "∏";
            const string GOAL_IMAGE_WIN10 = "G";
            const ConsoleColor GOAL_COLOR = ConsoleColor.Gray;
			const ConsoleColor GOALIN_COLOR = ConsoleColor.DarkGray;
			// Goal 초기 세팅..
			int[] INIT_GOALS_X = new int[GOAL_COUNT] { 49, 49, 49, 49, 49 };
			int[] INIT_GOALS_Y = new int[GOAL_COUNT] { 18, 19, 20, 21, 7 };
			string initGoalImage = (isWin11) ? GOAL_IMAGE_WIN11 : GOAL_IMAGE_WIN10;

			// Portal 관련 상수 설정..
			const int PORTAL_GATE_COUNT = 2;	// 한 개의 포탈에 이동할 수 있는 게이트의 개수??( 입구 <-> 출구 로 이동하니까 2개 )
			const int PORTAL_COUNT = 8;
			const string PORTAL_IMAGE_WIN11 = "Ⅱ";
            const string PORTAL_IMAGE_WIN10 = "X";
			// Portal 초기 세팅..
			int[,] INIT_PORTALGATE_X = new int[PORTAL_COUNT, 2]
			{ { 35, 5 }, { 35, 5 }, { 35, 5 }, { 35, 5 }, { 4, 40 }, { 10, 20 }, { 20, 30 }, { 30, 10 } };
			int[,] INIT_PORTALGATE_Y = new int[PORTAL_COUNT, 2]
			{ { 2, 18 }, { 3, 19 }, { 4, 20 }, { 5, 21 }, { 6, 16 }, { 1, 12 }, { 1, 12 }, { 1, 12 } };
			string initPortalImage = (isWin11) ? PORTAL_IMAGE_WIN11 : PORTAL_IMAGE_WIN10;
			ConsoleColor[] INIT_PORTAL_COLOR = new ConsoleColor[PORTAL_COUNT]
			{
				ConsoleColor.Green, ConsoleColor.DarkMagenta, ConsoleColor.Gray, ConsoleColor.Blue, ConsoleColor.Yellow
				, ConsoleColor.Green, ConsoleColor.Gray, ConsoleColor.Blue
			};

			// Switch 관련 상수 설정..
			const int SWITCH_COUNT = 1;
			const ConsoleColor SWITCH_COLOR = ConsoleColor.DarkMagenta;
			const string SWITCH_IMAGE = "ㅣ";
			const string SWITCH_PUSH_IMAGE = "*";
			// Switch 초기 세팅..
			int[] INIT_SWITCHES_X = new int[SWITCH_COUNT] { 41 };
			int[] INIT_SWITCHES_Y = new int[SWITCH_COUNT] { 17 };
			int[] INIT_SWITCHESBUTTON_OFFSETX = new int[SWITCH_COUNT] { -1 };
			int[] INIT_SWITCHESBUTTON_OFFSETY = new int[SWITCH_COUNT] { 0 };
			// 스위치 누르거나 땔 때 열거나 닫는 벽 인덱스..
			int[][] INIT_OPENCLOSE_WALL_INDEX = new int[SWITCH_COUNT][];
			INIT_OPENCLOSE_WALL_INDEX[0] = new int[4] { 3, 4, 5, 6 };

			// Trap 관련 상수 설정..
			const int TRAP_COUNT = 7;
			const ConsoleColor TRAP_COLOR = ConsoleColor.DarkMagenta;
			const string TRAP_IMAGE_WIN11 = "▒";
			const string TRAP_IMAGE_WIN10 = "Y";
			// Trap 초기 세팅..
			string initTrapImage = (isWin11) ? TRAP_IMAGE_WIN11 : TRAP_IMAGE_WIN10;

			// Arrow 관련 상수 설정..
			const ConsoleColor ARROW_COLOR = ConsoleColor.White;
			const string ARROW_IMAGE_WIN11 = "→←↑↓";
			const string ARROW_IMAGE_WIN10 = "→←↑↓";
			// Arrow 초기 세팅..
			string initArrowImage = (isWin11) ? ARROW_IMAGE_WIN11 : ARROW_IMAGE_WIN10;

            // Item 관련 상수 설정..
            const int ITEM_COUNT = 4;
			ConsoleColor[] ITEM_COLOR = new ConsoleColor[Item.ITEM_TYPE_COUNT]
			{
				ConsoleColor.DarkCyan, ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Blue
			};
			string[] ITEM_IMAGE_WIN11 = new string[Item.ITEM_TYPE_COUNT]
			{
				"®", "┼", "☏", "☎"
            };
			string[] ITEM_IMAGE_WIN10 = new string[Item.ITEM_TYPE_COUNT]
			{
				"R", "E", "B", "B"
			};
			// Item 초기 세팅..
			string[] initItemImage = new string[Item.ITEM_TYPE_COUNT];
			for ( int index = 0; index < Item.ITEM_TYPE_COUNT; ++index )
			{
				string win11Image = ITEM_IMAGE_WIN11[index];
				string win10Image = ITEM_IMAGE_WIN10[index];

				initItemImage[index] = (isWin11) ? win11Image : win10Image;
			}
            #endregion

            // ============================================================================================================================================
            // 변수 초기화..
            // ============================================================================================================================================
            #region 변수 초기화
            // 플레이어 관련 변수 설정..
            Player player = new Player
			{
				X = INITIAL_PLAYER_X, Y = INITIAL_PLAYER_Y, PrevX = INITIAL_PLAYER_X, PrevY = INITIAL_PLAYER_Y,
				Image = initPlayerImage, Color = PLAYER_COLOR, MaxHp = INITIAL_PLAYER_HP, MaxMp = INITIAL_PLAYER_MP,
				CurHp = INITIAL_PLAYER_HP, CurMp = INITIAL_PLAYER_MP
			};
			int curPlayerMoveCount = 0;


			// 박스 관련 변수 설정..
            Box[] boxes = new Box[BOX_COUNT];
			for( int boxIndex = 0; boxIndex < BOX_COUNT; ++boxIndex )
			{
				boxes[boxIndex] = new Box
				{
					X = INIT_BOXES_X[boxIndex], Y = INIT_BOXES_Y[boxIndex], PrevX = INIT_BOXES_X[boxIndex], PrevY = INIT_BOXES_Y[boxIndex],
					Image = initBoxImage, Color = BOX_COLOR, CurState = Box.State.Idle, DirX = 0, DirY = 0
                };
			}


			// 맵 관련 변수 설정..
			// 맵의 각 위치들의 데이터를 저장하는 룩업 테이블..
			MapSpaceType[,] mapDatas = new MapSpaceType[MAP_HEIGHT + 1, MAP_WIDTH + 1];


            // 벽 관련 변수 설정..
			Wall[] walls = new Wall[WALL_COUNT];
			for( int wallIndex = 0; wallIndex < WALL_COUNT; ++wallIndex )
			{
				walls[wallIndex] = new Wall
				{
					X = INIT_WALLS_X[wallIndex], Y = INIT_WALLS_Y[wallIndex], 
					Image = initWallImage, Color = WALL_COLOR, IsActive = true, IsRender = true
				};
			}


			// 골인 지점 관련 변수 설정..
            Goal[] goals = new Goal[GOAL_COUNT];
			for( int goalIndex = 0; goalIndex < GOAL_COUNT; ++goalIndex )
			{
				goals[goalIndex] = new Goal
				{
					X = INIT_GOALS_X[goalIndex], Y = INIT_GOALS_Y[goalIndex], Image = initGoalImage, Color = GOAL_COLOR,
					GoalInColor = GOALIN_COLOR, isGoalIn = false
				};
			}


			// Portal 관련 변수 설정..
			Portal[] portals = new Portal[PORTAL_COUNT];
			for( int portalIndex = 0; portalIndex < PORTAL_COUNT; ++portalIndex )
			{
				portals[portalIndex] = new Portal
				{
					GatesX = new int[PORTAL_GATE_COUNT], GatesY = new int[PORTAL_GATE_COUNT], Image = initPortalImage, Color = INIT_PORTAL_COLOR[portalIndex]
				};

                for ( int gateIndex = 0; gateIndex < PORTAL_GATE_COUNT; ++gateIndex )
				{
					portals[portalIndex].GatesX[gateIndex] = INIT_PORTALGATE_X[portalIndex, gateIndex];
					portals[portalIndex].GatesY[gateIndex] = INIT_PORTALGATE_Y[portalIndex, gateIndex];
                }
			}


			// Switch 관련 변수 설정..
			Sokoban.Switch[] switches = new Sokoban.Switch[SWITCH_COUNT];
			for( int switchIndex = 0; switchIndex < SWITCH_COUNT; ++switchIndex )
			{
				int loopCount = INIT_OPENCLOSE_WALL_INDEX[switchIndex].Length;

				switches[switchIndex] = new Sokoban.Switch
				{
					X = INIT_SWITCHES_X[switchIndex], Y = INIT_SWITCHES_Y[switchIndex], ButtonOffsetX = INIT_SWITCHESBUTTON_OFFSETX[switchIndex],
					ButtonOffsetY = INIT_SWITCHESBUTTON_OFFSETY[switchIndex], IsHolding = false, OpenCloseWallIndex = new int[loopCount]
				};

				for ( int openCloseWallIndex = 0; openCloseWallIndex < loopCount; ++openCloseWallIndex )
				{
					switches[switchIndex].OpenCloseWallIndex[openCloseWallIndex] = INIT_OPENCLOSE_WALL_INDEX[switchIndex][openCloseWallIndex];
				}
			}


			// Trap 관련 변수 설정..
			Trap[] traps = new Trap[TRAP_COUNT];
			int trapIndexTemp = 0;
			traps[trapIndexTemp++] = new BombTrap
			{
				X = 15, Y = 10, Damage = 5, Image = initTrapImage, Color = TRAP_COLOR, BurstRange = 5
				, MyType = Trap.TrapType.Bomb
			};
            traps[trapIndexTemp++] = new BombTrap
            {
                X = 4, Y = 2, Damage = 5, Image = initTrapImage, Color = TRAP_COLOR, BurstRange = 5
                , MyType = Trap.TrapType.Bomb
            };
            traps[trapIndexTemp++] = new TriggerTrap
			{
				X = 15, Y = 7, Image = initTrapImage, Color = TRAP_COLOR, MyType = Trap.TrapType.Trigger
			};
            traps[trapIndexTemp++] = new TriggerTrap
            {
                X = 33, Y = 2, Image = initTrapImage, Color = TRAP_COLOR, MyType = Trap.TrapType.Trigger
            };
            traps[trapIndexTemp++] = new TriggerTrap
            {
                X = 33, Y = 3, Image = initTrapImage, Color = TRAP_COLOR, MyType = Trap.TrapType.Trigger
            };
            traps[trapIndexTemp++] = new TriggerTrap
            {
                X = 33, Y = 4, Image = initTrapImage, Color = TRAP_COLOR, MyType = Trap.TrapType.Trigger
            };
            traps[trapIndexTemp++] = new TriggerTrap
            {
                X = 33, Y = 5, Image = initTrapImage, Color = TRAP_COLOR, MyType = Trap.TrapType.Trigger
            };

			for( int trapIndex = 2; trapIndex < TRAP_COUNT; ++trapIndex )
			{
                ((TriggerTrap)traps[trapIndex]).CreateSpawnObjectArray( MAP_RANGE_MAX_Y - 1 );
                for ( int i = 0; i < MAP_RANGE_MAX_Y - 1; ++i )
                {
                    TriggerTrap curTrap = (TriggerTrap)traps[trapIndex];
                    curTrap.SpawnObjectsX[i] = MAP_RANGE_MAX_X - 2;
                    curTrap.SpawnObjectsY[i] = i + 1;
                    curTrap.SpawnObjectsDirX[i] = -1;
                    curTrap.SpawnObjectsDirY[i] = 0;
                }
            }

			// 한 개 테스트용..
			//((TriggerTrap)traps[1]).CreateSpawnObjectArray( 1 );
			//for ( int i = 0; i < 1; ++i )
			//{
			//    TriggerTrap curTrap = (TriggerTrap)traps[1];
			//    curTrap.SpawnObjectsX[i] = MAP_RANGE_MAX_X - 1;
			//    curTrap.SpawnObjectsY[i] = 15;
			//    curTrap.SpawnObjectsDirX[i] = -1;
			//    curTrap.SpawnObjectsDirY[i] = 0;
			//}


			// Arrow 관련 변수 설정..
			List<Arrow> arrows = new List<Arrow>();
			List<Arrow> removeArrows = new List<Arrow>();


            // Item 관련 변수 설정..
            Item[] items = new Item[ITEM_COUNT]
			{
				new Item { X = 10, Y = 10, Effect = 0, Duration = 10, type = Item.Type.ReverseMove, isActive = true },
				new Item { X = 10, Y = 2, Effect = 1, Duration = 1, type = Item.Type.EasterEgg, isActive = true },
				new Item { X = 10, Y = 6, Effect = 5, Duration = 1, type = Item.Type.HPPosion, isActive = true },
				new Item { X = 20, Y = 7, Effect = 5, Duration = 1, type = Item.Type.MPPosion, isActive = true }
			};
			// 현재 타입에 따라 Image 와 Color 결정..
			for( int itemIndex = 0; itemIndex < ITEM_COUNT; ++itemIndex )
			{
				int itemType = (int)(items[itemIndex].type);
				items[itemIndex].Image = initItemImage[itemType];
				items[itemIndex].Color = ITEM_COLOR[itemType];
			}
			// 플레이어가 사용중인 아이템 관련..
			int[] playerActiveItemIndex = new int[ITEM_COUNT];
			int activeItemCount = 0;

			#region 기타 변/상수 설정
			// 타이머 관련..
			const int FRAME_PER_SECOND = 20;
			double frameInterval = 1.0 / FRAME_PER_SECOND;
			double elaspedTime = 0.0;
			double runTime = 0.0;
			double prevRunTime = 0.0;
			double gameOverTime = 0.0;
			Stopwatch stopwatch = new Stopwatch();
			// 렌더 관련( 그릴지 말지 )..
			bool isSkipRender = false;
			bool isConsoleClear = false;
			#endregion

			EndingType endingType = EndingType.None;

            #endregion

            #region 시작 전 초기 작업
            // Console 초기 세팅..
            Console.ResetColor();                           // 컬러를 초기화한다..
			Console.CursorVisible = CURSOR_VISIBLE;         // 커서를 숨긴다..
			Console.Title = TITLE_NAME;                     // 타이틀을 설정한다..
			Console.BackgroundColor = BACKGROUND_COLOR;     // Background 색을 설정한다..
			Console.ForegroundColor = FOREGROUND_COLOR;     // 글꼴색을 설정한다..
			Console.Clear();                                // 출력된 모든 내용을 지운다..


			// 시작 전에 맵 데이터에 플레이어 박스 위치 저장..
			mapDatas[player.Y, player.X] = MapSpaceType.PlayerStand;
			for ( int i = 0; i < BOX_COUNT; ++i )
			{
                mapDatas[boxes[i].Y, boxes[i].X] = MapSpaceType.BoxStand;
            }

			// 맵 데이터에 벽 위치 저장..
			for ( int i = 0; i < WALL_COUNT; ++i )
			{
				if ( walls[i].IsActive )
				{
                    mapDatas[walls[i].Y, walls[i].X] = MapSpaceType.DontPass;
                }
				else
				{
                    mapDatas[walls[i].Y, walls[i].X] = MapSpaceType.Pass;
                }
			}

			// 맵 데이터에 포탈 위치 저장..
			for ( int portalIndex = 0; portalIndex < PORTAL_COUNT; ++portalIndex )
			{
				for( int gateIndex = 0; gateIndex < PORTAL_GATE_COUNT; ++gateIndex )
				{
					int gateX = portals[portalIndex].GatesX[gateIndex];
					int gateY = portals[portalIndex].GatesY[gateIndex];
                    mapDatas[gateY, gateX] = MapSpaceType.Portal;
                }
			}

			// 맵 데이터에 스위치 위치 저장..
			for ( int switchIndex = 0; switchIndex < SWITCH_COUNT; ++switchIndex )
			{
				int switchX = switches[switchIndex].X;
				int switchY = switches[switchIndex].Y;

				mapDatas[switchY, switchX] = MapSpaceType.DontPass;
            }

			// 맵 데이터에 Trap 위치 저장..
			for ( int switchIndex = 0; switchIndex < TRAP_COUNT; ++switchIndex )
			{
				int trapX = traps[switchIndex].X;
				int trapY = traps[switchIndex].Y;

				mapDatas[trapY, trapX] = MapSpaceType.Trap;
			}

			// 맵 데이터에 Item 위치 저장..
			for ( int switchIndex = 0; switchIndex < SWITCH_COUNT; ++switchIndex )
			{
				int itemX = items[switchIndex].X;
				int itemY = items[switchIndex].Y;

				mapDatas[itemY, itemX] = MapSpaceType.Item;
			}

			// 맵 외곽 통과 못하는 곳으로 설정..
			for ( int i = 0; i <= MAP_WIDTH; ++i )
			{
				mapDatas[0, i] = MapSpaceType.DontPass;
				mapDatas[MAP_HEIGHT, i] = MapSpaceType.DontPass;
			}
			for ( int i = 0; i <= MAP_HEIGHT; ++i )
			{
				mapDatas[i, 0] = MapSpaceType.DontPass;
				mapDatas[i, MAP_WIDTH] = MapSpaceType.DontPass;
			}

			// 설명용 텍스트 설정..
			LinkedList<KeyValuePair<int, string>> logMessage = new LinkedList<KeyValuePair<int, string>>();
			const int logStartX = 60;
			const int logStartY = 0;

			logMessage.AddLast( new KeyValuePair<int, string>( 2, "============== 설명 ==============" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, $"{initPlayerImage} : 플레이어" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, $"{initBoxImage} : 박스" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, $"{initWallImage} : 벽" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, $"{initGoalImage} : 골" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, $"{SWITCH_IMAGE} : 스위치, {SWITCH_PUSH_IMAGE} : 버튼" ) );

			logMessage.AddLast( new KeyValuePair<int, string>( 2, "" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 2, "============ 아이템 설명 ============" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, $"반대로 움직임(10턴) : {initItemImage[(int)(Item.Type.ReverseMove)]}" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, $"이스터 에그 : {initItemImage[(int)(Item.Type.EasterEgg)]}" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, $"HP 포션 : {initItemImage[(int)(Item.Type.HPPosion)]}" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, $"MP 포션 : {initItemImage[(int)(Item.Type.MPPosion)]}" ) );

			logMessage.AddLast( new KeyValuePair<int, string>( 2, "" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 2, "======== 방향키 ========" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, "↑ ← ↓ → : 이동" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, "SpaceBar : 박스 잡기" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, "A : 박스 차기" ) );
			logMessage.AddLast( new KeyValuePair<int, string>( 1, "" ) );

			// 플레이어 정보 텍스트 설정..
			StringBuilder playerStateLog = new StringBuilder();
			const int playerStateLogStartX = 0;
			const int playerStateLogStartY = MAP_RANGE_MAX_Y + 2;
			// 플레이어 에러 텍스트 설정..
			StringBuilder playerErrorLog = new StringBuilder();
			// 야매로 플레이어 정보 출력한 곳 지워줄 텍스트 설정..
			StringBuilder clearPlayerStateLog = new StringBuilder();
			clearPlayerStateLog.AppendLine( "                               " );
			clearPlayerStateLog.AppendLine( "                               " );
			clearPlayerStateLog.AppendLine( "                               " );
			clearPlayerStateLog.AppendLine( "                               " );
			clearPlayerStateLog.AppendLine( "                               " );

			// 타이머 스타트..
			stopwatch.Start();
			#endregion
			#endregion

			#region GameLoop
			while ( true )
			{
				// 실행 시간 계산..
				elaspedTime += runTime - prevRunTime;
				prevRunTime = runTime;
				runTime = stopwatch.Elapsed.TotalMilliseconds * 0.001;

				logMessage.RemoveLast();
				logMessage.AddLast( new KeyValuePair<int, string>( 1, $"실행 시간 : {runTime:F3}" ) );

				playerStateLog.Clear();
				playerStateLog.AppendLine( "========== Player State ==========" );

				if ( elaspedTime >= frameInterval )
				{
					elaspedTime = 0.0;

					#region ProcessInput
					// --------------------------------------------------------------- ProcessInput.. ---------------------------------------------------------------
					// 입력한 키 가져오기..
					ConsoleKey inputKey = ConsoleKey.NoName;
					if ( Console.KeyAvailable )
					{
						inputKey = Console.ReadKey().Key;
						isSkipRender = false;
					}
					#endregion

					if( EndingType.None != endingType )
					{
						inputKey = ConsoleKey.NoName;
					}

                    Update( inputKey );

                    // =========================================== Check Game Clear.. =========================================== //
                    // 골인 지점과 박스 위치가 몇개나 같은지 비교하는 곳..
                    int goalBoxCount = 0;
					for ( int i = 0; i < GOAL_COUNT; ++i )
					{
						goals[i].isGoalIn = false;

						for ( int j = 0; j < BOX_COUNT; ++j )
						{
							if ( goals[i].X == boxes[j].X && goals[i].Y == boxes[j].Y )
							{
								++goalBoxCount;
								goals[i].isGoalIn = true;

								break;
							}
						}
					}

					if(EndingType.None == endingType )
					{
                        // 현재 골인지점과 박스위치가 전부 같다면( GG )..
                        if ( goalBoxCount == GOAL_COUNT )
                        {
                            endingType = EndingType.Clear;
                            gameOverTime = runTime;
                        }

                        if ( player.CurHp <= 0 )
                        {
                            endingType = EndingType.Die;
                            gameOverTime = runTime;
                        }
                    }
					else
					{
                        if ( runTime - gameOverTime > 1 )
                        {
                            break;
                        }
                    }

                    Render();
				}
			}
			#endregion

			#region Clear Message
			// 게임 클리어 했으니까 메시지 띄우기..
			Console.Clear();
			Console.SetCursorPosition( 0, 0 );

			switch(endingType)
			{
				case EndingType.Clear:
                    Console.ForegroundColor = FOREGROUND_COLOR;
                    Console.WriteLine( "게임을 클리어하셨습니다!!!!!\n" );
                    Console.WriteLine( "L을 입력 시 그림 나옴" );
                    Console.WriteLine( "글꼴을 돋움채로 하시고 전체화면으로 보시는 걸 추천합니다." );

                    ConsoleKeyInfo KeyInfo = Console.ReadKey();

                    #region Ending 아스키 코드

                    if ( KeyInfo.Key == ConsoleKey.L )
                    {
                        Console.Clear();
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.SetCursorPosition( 0, 0 );
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNNNNXXXXXNNNNNNNNNXXXXXXXNNNNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWWWWNXXNXXXXXXXXXXXXXXNXXXXXXXNNXXNWWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWWWWWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWWMMMMWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMWK0KNWNWNWNNWNNWNWNWNNNNWNWNWNNNNWWWNWNNNNNNNNWNWWNWNK0KNWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXNKKXXOkkKXKXKXKKKOKXKXKXKKXKXKXKXKKKKXKXKXKXKKKKXKXKXKKNKkkOXXKKXNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKNXKXXK00KNXNXXXXXKXNXNXNXXXXXXNXNXXXXXXNXNXNXXXXNXNXXXXWX000XXKXK0XMMWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXNWWWWWWWWWWWWWWWWWWWWWMWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWNXWMMMWWWNWWWNXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKXNXXXXXXXXXKXXXXXXXXKXXXWXXXXXKXKXXXXXXKXKXXXXXXKXKXXXXXXXXKXKXWNXXWWXXXKXKXXNXKXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXWMWKXKKKKXKXKXKXKKXKXKXKKWKKXKXKXKXKKKKXKXKXKXKKKKXKXKXKKKKXKXKXWMWXXXKXKKKKXKNMWXXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNNNWWWMMWWWWWWWWWWWWWNXNWWWWWWWWWWWNXNWWWNNWWWWWWWNNNNWWWWWWWWWWWWWWWWMMWWNNXNWWWWWWMWWNNNNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXNNNNNNNXNNXNXNXNWMWXXXXNNXNNNWWMNXXNXNWXKXNWNNNNWKO0XNXNNNNNNXWWNNNNXNNNNXNNKXWMWXNNNNNNXXKXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXWMNKKKKKKKKKKKKKKKNXKXWMXKKKKKKNNXXNWMKKX0XXKNKKKXN0OKWXKXKKKXWXNXKKKKKKKKK0NWWNKXXKK0KKKKKWWXXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNWMMMWNNNNNNNNNNNNNXNNXWMWMWNNNNXXNNNWMMMWNXNWWNWNNNWNXXNMWNNNNNWMMMWNNNNNNNNNNWMMMWNNNNNNNNNNMMMWNNWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXXWWNWNWNWWNWNWNWNWMWXKNWNWNNNNMWNXXNNWWNWNWNXNWNNWNWNWNK0XWWNNNWNWNWMW00WWNWNNWNWNWNWNNNNMWNWWNWNNNNMXOKWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0KKXXKXKXKXKKKKXKXKXNKXWMNKXKKXKNXKNWXOKXKXKKKKKXKKXKXKNXOOXMKKKKXKXKXWK00KNKKKXXKXKXKXKXKKWWXXWKXKKXXX000XMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXXWXXXXNXNXXXXXXNXNXKXWMMMWXXXXXKXNWMMNKXNXNX0XWXXXXXXNXNX0XWMXXXXNXNXNNXWWXNXXXXXXNXNXNXXXXWMWWWXXXXXXXXMNXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNNWWWWWWWWWWWWMMMWXXNWWWWWWMMMWXNNWWWWWWNWMMNXNWWWWWWWWWWNXXNWWWWWWWWWWWWWWMWXXWWWWWWWWWWWWWWWWWWWMNXMNXNWWMMXKWMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKKXXXXXXXXXXXXXWWNKXWXXXXXKNMWXXNNXXXXXXX0KWN0KXXKXXXXXXXNKO0NNXXKXXXXKXXXKNWXOOXWKXXXXXXKXXXXXXXXKWN0K0KNXNMN0OKWMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKKNKXKKKKXKXKXKKNXXWMWKKKKXKXXXNWMNKXKKKKX0KXKXXKXKXKKKKKXX0OKMXKXKXKKKKXKXKXNKNNKXKXKKKKXKXKXKKKKXKNMNOKWXKNNKXWKXMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWNWWWWWWWWWWWWWNNNWMMWWWWWWXXNNWWMMWWWWWWWWWNNNWWWWWWWWWWNWNXXNMWWWWWWWWWWWWWWWWMMWNNWWWWWWWWWWWWWWWWWWWNNWWNNWWWWWNNMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKKXNNXNNNNNXNNNXKXNNNNXNMWNXXNNNNNNNNNNXNNNXOKXNNNNNNNNXNNNN0OKWNNNNNNNXNNNNNNNNXNWNOONWNNNNNNNNNNNNNNNNNN0XK0NNNXNN0OXMMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWK0XKKKKKKKKXKKK00XKKKKK0XNXXNMNKKKKKKKKKKXK0KKXKKKKKKKKK0XKKKOOXWKKKKKKXKXKKKKKKKKXN0XKKNKKKKKKKKXKXKKKKKKWXOOXWKXKKKKX0NMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXNWNNNNNNNNNNNNKNMNNNNNNNXNMMMWNNNNNNNNNNNXXNMWNNNNNNNNNNNNNXKNWMNNNNNNNNNNNNNNNNNWNNMWNNNNNNNNNNNNNNNNNNNWWNNMWNNNNNWMNNMMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKXWNNWNWNWNWNWNKNWWWWNWXXNWNWWWWNWNWWWWWMX0KNWXNWNWNWWWNWWNWN0KWWNWWWNWNWWNWWWNWWWWNWNWMN00WWNWWWNWWWWNWWWNNWNNXKKXNNNMMXOXMMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXO0XWXKKKXKXKXXK0KKKXKXXK0KKXKXKKXKXKXKXKXN0OKWXOKXKXKXKKKKXKX0O0KXKXKXKKKKXKXKXKXKKXKXKNWK00KNKXKXXKXKXKXKXKKXKKkxkkKKKWN000NMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWNWWMXXXXNXNXXX0XWXXXXNK0NNXNXXXXXXXXNXNXNXKKXMNKXXXNXXXXXXNXNK0KXXXNXNXXXXXXNXNXXXXXXNXNNXWNXNXXXXXXXXNXNXXXXXXNKXNKNXXWXNWXXMMMMMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNKXWWWWWWWWWWWNXXNWWWWMMWNNWWWWWWWWWWWWWWMMWXKXWWWMWWWWWWWWWWWWWWXKWWWWWWWWWWWWWWWWWWWWWWWWWWMMNKNMWWMWWWWWWWWWWWWWWWNNWNWWWMMWKNMWXXWMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNOOKWXXXXXXXKXN0kKWXXXKNNKKXNXXXXXKXXXXXXXWWXOOKWXXMXKXXXXXNKXXXXXKOKXKXXXXKXXXXXXXXXXXXXXXXXXWN0O0NXXNXXXXXXXXXXXXXXOkkOOKXKWWKO0NMNNWMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "WMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMKO0NMKKKKXKXKXKO0XMKKXKX0XNKNKKKKXKXKKKKKKNWKO0NWKXWXKXKXKXXKXKXKKKOKKKKKKKKXKXKXKKKKKKXKXKKKKNXKWKXXKNKKKKKKXKKKKKKNO0KOOKKKNXKWXKWMMNNMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXNWMWWWNWWWWNXXNWWWWWWNNWWWWWWWWWWWWWWWWWWNXXNMMWWWWWWWWWWWWWNWNWXKWNNWWWWWNWWNWWWWWWWWWWWWWWWWWMWNNWWWWWWWWWWWWWWWWWWWWWWWWWWWMWNNWMWWMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXOOXWWXNNNNXWWKO0NWNXWWKKNNNNNNNNNNNNNNNNNWWKO0NWNNNNNNNNNNNNNNNNNNK0XNXNNNNNNNNNNNNNNNNNNNNNNNNXWWXO0NMNXNNNNNNNNNNNNXK0OKKXNNNNNNK0WMMMMNXWMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0OKWWX0XKKKKWXOOKWWK0XKKXKKKKKKKKXKXKKKKKKWX0OKWXKXKKKKKKKKKKKXKXKKKO0XKKKKKKXKXKKKKKKXKXKXKKKKK0NX0XKKWX0XKKXKXKKKKKKNkk0kkKKKXKKKO0WMMMMWXNMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXXNMMWNNNNNNWNKXNMWNNXKNWNNNNNNNNNNNNNNNNNWNKXNMNNNNNNNNNNNNNNNNNNNXKNNNNNNNNNNNNNNNNNNNNNNNNNNNNWNNWWNWWNNNNNNNNNXNNNWXNWNXNNNNNNNXXWMMMMMMMMMWWMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKKXMMMWNWNWMWX0XWWWWNNKKWWWWNWWWWWWNWWWNWWNWX0XWWWWNWWWWWWWNWNWWWWWWK0XWWXXWNNWWWNWWWNWWWWWWWWWWNWWNWNNXNNWWNWMMWNXNWNWMKKNWNWWWWWWWNKXMMMMMMMMMNXXWMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKO0NMMMNKXKXWXOOKMXXNKX00XXKXKXKKXKXKXKXKXXKXOO0XKKXKXXKXKXKKXKXKXKXXk0K000KXKXXKXKXKXKXXKXKXKXKXKKXKX0OKKOKXKNWNXXWMWNXNKOKXKXKKXXXKX0XMMMMMMMMMMWXXXWMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNK0XWMMMNXXXXWX0KNMNXNXXK0XXXNXXXXXXNXXXXXXXXN00KXXXNXXXXXXXXXXXNXXXXXOKWKKNXXXXXXXXXXXXXXXXXXXXXXXXXXN0XWNKXNXXXXWWMMMMWWX0XXXXXXXXNXXKXMMMMMMMMMMMMWXXNMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKXWMMMWXXWWWWXXWWWWWWWWXKWWWWWWWWWWWWWWWWWWWXKXWWWWWWWWWWWWWWWWWWWWWWXXMMNNMNXWWWWWWWWWWWWWWWWWWWWWWWWWWWMMWXXWWWMMMMMWXNKKWWWWWWWWWWNXNMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXOOKMMMMWK0XXXX0OXXKXXXXXKOKXXXXXXXXXXXXXXXXXK00KXXXXXXXXXXXXXXXXXXXXXXKKWMX0K0KNXXXXXXXXXXXXXXXXXXXXXNXXXXWNXXWWWWWWMWWK0KO0XXXXXXXXXXX0XMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKO0NMMMMWKOKXKK0OKKKXKKKKKO0XKKKKXKXKXKKKKX00XX00XKKKKXKKXKKKKKKXKXKKKK00WMWXOKWXKXKXKKKKKKXKXKKKKKKXKXKKK0XXWWWWWMWWMWXKNXkOKXKKKKKKKKK0XMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXXNMMMMMWXXWNNNKXWWWWWWWWXKNWWWWWWWWWWWWWWNXNNXKNWWWWWWWWWWWWWWWWWWWWWWKKWMMMNNWWNNNWWWWWWWWWWWWWWWWWWWWNNNWMMMWWWMWWWNNWWWKXWWNWXNWWWWNKXMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0OKWMMMMMWKKNNNN0KNNNNNNNNK0XNNNNNNNNNNNNWNKXMXO0KNNNNNNNXNNNNNNNNNNNNNNX0NMMMWKKNKXNNNNNNNNNNNNNNNNNNNNNKXWMWWMWWMWWNKXNWXNK0NNNN0ONNNNX0XMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMKOOXMMMMMMWKOKKKK0OKKKKKXKKKO0XKKKKXKXKXKXN0KWMXO0OKKKXKKKOKKKKKXKXKKKKKKK0NMMMMN0kKWKKKKXKXKKKKKKKKXKXKKKKWWMWWMWWMWN0KXKNKK0O0KKK0OX0XX0OXMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKXWMMMMMMWXKNNNNKKNNNNNNNNXKXNNNNNNNNNNNNNXWWMXKKXNNNNNNXKNNNNNNNNNNNNNNXXWMMMMWNXNMNNNNNNNNNNNNNNNNNNNKXWWWMWWMWWMWNXWNNWNNXKNNNNXNWNWNXKNMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWX0XWMMMMMMWKKWWNWKKWWNWWWWWXKNWWWWWWWWNWMNXWWXNXKWWNWWNNX00KNNWWWWNWWNNXKXNWMMMMMMWXXWMMMMMMMMWWWNWMMWNXNWMMMMMMMMMMXXWWWWNWWKKNWNWXXMMMWXXWMMMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKOKWMMMMMMWKOXXKXK0XXKXKXKKK0KXKXKKXKXKXN0KWNKKOkKKO00OKK00KNXXKKXKXKX0kkk0WMMMMMMMWNXXWMMMMMMXKXKNWNXXWWWWWWWMWWWWN0XWKXXKNKk0KXKX0KWWWWWNXXNMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWK0KWMMMMMMWK0XXXXK0XXXNXXXXK0KNXXXXXXNXXKXWXKXX00NNXNNXNNXXXNXXXXXXNXNKKNKXMMMMMMMMMMWNXXXXXXXKXXXXXXWMMWWWWWWMWWMNKXWWXXNXNX0KXXXXKKWWWWWMWNXNMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWX0XWMMMMMMWXXWWWWXXWWWWWWWWNNMNXNMWXKK0XXXWWWMMXKNWXXMWWMWNNWWWWWWWWWNNNWWMMMMMMWNXXXXNWX0KXXXNWWNNWWMMMMMMMMMMMMMWKXWWWWWWWWKKWWWWKKMMMMMMMMWWWMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKOKWMMMMMMWK0XXXXK0XXXXXXXXKKNKO0NNOkOOXK0WMMMMK0KX0KMXKXKO0XXXXXXXXKkkk0NMMMMMMMMMMMMMMWNXXNMNXX0XWWWWMWWMWWMWWWWN0XXXXXXXXXO0XXXXKKWMMMMWWWMX0NMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0OKWMMMMMMWKOKKKKKOKKKXKKKKKKKKNKKK00KXK00NNNWWKO0K0KMWKkKNKKKKXKXKX0OKO0WMMMMMMMMMMMMMMMMMNXXK00XWWMWWMWWMWWMWWWWXOKKKXKXKKKkO0KKK00WWNNNNWWWXKNMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWX0XWMMMMMMWXXWWWNKXWNWWNXXNNXNWMWWNWNNWNX0KXXXXXKKK00NWWXNMWWWWWNWWWWWWWWMMMMMMMMMMMMMMMMMMMWNNNNWWWMWWWWWMWWWWWWWWXKNNXNWWWNXXNNWWNXX0xOKXWMWWXNMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKOKWMMMMMMWKKNNNN00NNXNXO0NWX00KWNXNNXNWXOXMMMMMNXXXXNWN0OKNMWNNNNNNWXKWMMMMMMMMMMMMMMMWWWNXNWXKWWMWWMWWWWWMWWWWWMWNKKKKNNNWKKNXXKXNXXX0KWWMWWWKKMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0OKWMMMMMMWKOKKKK0OKX0XX00NNOkk0X0k0kOK0OOXMMMMMMMMMMMW0KKO0NWKKKKXKXKXMMMMMMMMMMMMMMWWNXXXXNX0NWWWWWWWWMWWMWWWWWWWWKOOXNKXX0XWXKKKNWWMMMWWMWWWKKMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKXWMMMMMMMXKNNXNKKNNXNWNXNWXK0KX000O0XNXKKXXKXXWMMWWWNNWWWNNWNXNNNNXNMMMMMMMMMMMMMMWNNWMMMMWXNMWWMWWMWWMWWMWWMWWMWXXNNXXNNNNMWNNWMMMMMMMWWMWWWXXNNNNWMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKKNWMMMMMMMWKXXKNXKWWWWWWWX00KKKNXXXXXXNWNNWKKNNWWXO000NNWWWWMWXXWXXWMMMMMMMMMMMMMMMMMMMMMMWXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXXXXXXNWMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0OKWMMMMMMMMX0OXWK0XXKXXXXOkxO00NMMMMMX0K0XXOXX00XX0KXXKKOxONMMN000XMMMMMMMMMMMMMMMMMMMMMMWK0NWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMN0XMMMMMWNWWMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWK0XWMMMMMMMNXXXXXK0XXXXXXXKX0KWKXMMMMMNXNXXNKKKOKNK00KKKK00XMMMMN0XMMMMMMMMMMMMMMMMMMMMMMMNKNWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWWWWWKXMMMMMMMWNWMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMMMMMMMMMWWMMWNKXWWWWWWWWWNNNXKNMMMMMNXNNXWNKKNWMWXXNWNWMWXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMMMMMMMMWXKWMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW00XXXXXXXXXXXXKk0WMMMMWK00KWNXXNWWNXXNK0WMWK0WMMMMMMMMMMMMMMMMMMMMMMMMMMMX0XMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWWWWWWMWWWMN0XWMWWMWWWWX0NMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKOKKKXKKKKK0Ok000KNWMWNK00KKKKXNNXXWNXXNMMWKKWMMMMMMMMMMMMMMMMMMMMMMMMMMMX0XWWWMWWMWWMWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWN0XWWWWWWWWWWKONMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXWNWWWWNWWNXKXXXXNWMWXKXK0KXXXXNXXWNNWWWWWNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKXWWWMWWMWWMWWWWWMWWMWWMWWWWWWWWMWWMWWWWWWWMWWWWNWWWWWWWWWMWXXWMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKKNNNNNNNNMXOKWMWKXMWXXX000KWWKXMWXXWWXXWWWXXWWWWWWWWWMMMMMWX00KWWX00XWWXXXNMMMMMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMMXKWMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKOKXKXKKKKX0XKKWKKWWKKNXKKKXNKKWWKKNWK0NWNK0NWNNNNNNNWWWWWWWKOk0NW0Ok0WXOKNWWMMMMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMX0NMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKNNXNNNNNNNWNXXXWWNXNWXXXXXXXWMNXNWNXNMNXXNMWWWWWWWWWWWWWWWWWWWMMWWWWMWNWWNXNMMMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWWXNMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKXXXXKWWWWWWNNN0KMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0NMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0KWMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0KKKK0XKXKXX0KX0OXWWWMMMWWWWWWMMMMMMMMMMMMMMMMMMMMMMWWMMMMMMMMMWWWWWWWWWWWWWX0XWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWWWWWWWWWWWWWWWWMWNKO0XMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNXNXKXXXXXX0OOKK0KXNMWMNXXXXWMMMMMMMMMMMMMMMMMMMMMMWWMMMMMMMMMWWWWWWWWWWWWWX0XWMWWMWWMWWMWWMWWMWWMWWMWWWWWWWWWWWWWWWWWWWWMWWWWWWWXXWNKWMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMMMWXKNN0KNXKXKKXNNXXWMWNNNNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWMWNWMMMNXNMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0KKkOX0ONNXK0NWXKXWMWWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0XMMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWMMWWWWN0KWWWWW0KWMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWWMMNO0K0KOONNXXXNMMWXXXXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0XWMWWMWWMWWMWWMWWWWWMWWWWWWWWWWWWWWWWWWWWWWWWWWWNKKWMWWWW0KMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMMMMMWWNXXNMWNXXNKKKXWWWMWWMMMMWNXXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWXKKKXXWNKXXXNNWWMWWMWWMWWMWWMWWMWMWWMMWMMWMMWWMWWWNNWMMWWMNNWMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXXXNWWKXMMMMMWKO0XKx0XXXNXXWMMMMMMMWXXXNWMMMMMMMMMMWWNNNNNNNNNWMMWWWWWWMMMMMMMMMMWXXNX000KWX000KNXXNWMMWMMWMMWMMMMMMMMMMWMMWWMWMMWWMMMMWWMWWMWKXMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXK0KNK0NMMMMMWK0Kk0000O0XK0WMMMMMMMWXK0KNMMWMMMMMMMN00XXXXXXXXWMWXXXXXNWMMMMMMMWXXNWMNKKKXNNKKKXWMWXXNWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWNNKXWMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMMWXNWMWXNWXXXXXKXNWKNXXXNNNNNWMMMMMMMWXNWMMWNXNWMMMWWWNWMWWMMMMMMMMMMMMMNNWMMMMMNXNWMMWXXXXXXXXXXXNMMMWXNWWWWWWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWNXNMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXWMMMMMMMNNMWWK0KKKXXNK0NXO0XNWWNWMMMMMMMMMMMMWXXNMMMWXXXNWMNNWNXXXNMMMMMMMMWXKXXWNXKKNWXKXXWNXKKNWNKKXWWXXNXXWMWWMMMMMMMMMMMMMMMMMMMMMMMMMMWNNNNWMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXWMMMMMMMMKKWMN0XMMMMMX000000XKKK0NMMMMMMMMMMMMMWNXXWMWWX0OKWMMWWWWWWMMWWWXKXN0kk0NNKKKXNXKKKNNKKKXWXKKXNNXKNWNXXNWWWWWWWWWWWWMWWWWWWWMWWMMWXKXWK0WMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXWMMMMMMMMMMX0XNXNWMMMMMNXN0KX0XXNXXWMMMMMMMMMMMMMMMWNXNWNXXNWMMMWXXXXXNMX0XXKKXKOOONNKKKXNXKXXNXKXKXNXKXXNNXXNWMWNXNWWWWWWWWWWWWWMMWWWWWWWMMX0KXNXXWMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWWMMMMMMMMMMMMMWX0XMMMMMMWWMMWXXWX0XWWWWMMMMMMMMMMMMMMMMWWNKXWMMMMMMMNNNNXXNWNXXXWNXKXNWNNXNWWNXXNWNNNXNWNXXNWWNXXWWNXXNNWMMMMMMMMMMMMMMMWMMMMMMMMMWNWMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0KMMMMMMMMMMMMMMMMKxKMMMMMX0NMMMX0K0x0XKKXWMMMMMMMMMMMMMMMMMWXXXXWMMWNXXWNXKXNWXXXXWNXXKNWNKKXNWXKXXWNXXXNWXXXXWNXXXNWXXNWXKXWMWWMWWMWWMWWMWWMWWMWWMWKKWMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKKWMMMMMMMMMMMMMMMM0x0NXNNXXNMMMMWKOK00KXNKNMMMMMMMMMMMMMMMMMMMMWXXNNXXWMWNXXKXNXXXXNNXXXXNXXXXNNXXXXNXXXXNNXXXXNNXXXNNXXXWMWXKNNWMWWMWWMWWMWWMWWWWWWKKNMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXWMMMMMMMMMMMMMMMMWXKXXKXNNWWNWMMMWNXNKXWMWWWMMMMMMMMMMMMMMMMMMMMMWNNWWMMMWWWWWWWXXKXXNWWWWWWXKXXKXNNNWWWWWWWWWWWWWWWWMWWWMMWMWNNNWWWMWWMWWMWWMWWWNXXXWMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0WMMMMMMMMMMMMMMMMMXKNMMMMMMWKKWMMMX0X00WMXOKWMMMMMMMMMMMMMMMMMMWXXNXXXNWNXXXWMXKXXXXXNWWNNMWNXXXXNWWXXNWMWNXXXWWXXXNWNXXXWWXXXNWXKXWMMWMMWMMMMMWXXXXXNWMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMK0WMMMMMMMMMMMMMMMMMN0XMMMMMMMK0WMMMWKkONMN0X0KWMMMMMMMMMMMMMMMMMX0XNXXXNNXXXXNX0XMMWMMMMWNNWMMMMMMMMMMWXKNWXXXXNNXXXXNNXXXNNXXXXWMWXKXWWWWWWWWNXXXWMMWXXXWMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMWXXWWXKXXXKKNWMMMMWKXWWNNMNNWWMMMMMMMMMMMMMMWNXNWNXXXNNNXXXNXNWNNWWWWMMMMMMMWWWMMMMMMMWXXXXXXNNXXXXNNXXXNNXXXXWMMMWXNWWWNXNWXXWMMMMMMWXNWMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMMNX00KKKKKKKXXXWMMMMWXXWMMMMWKXMMMMMMMMMMWNXXKKNXXKNWNXXXWMWXXXXXXXXXNMMMMMMWNXNMMMMMMMMWNXNMMMNXXXNWXKXXWNXKXNWNKNNXKXKXWWMMMMMMMMMMMNKNMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMK0WMMMMMMMMMMMMMMMMMMMK0WMMMMMWWX0XMMMMMWNXXWMWKO0XMMMMMMMWNXXWMX0XXKKXWXKKXNWKKWMMMMMWNXXNMMMMMMNXXNMMMMMMMWNXXWWNXKKNWXKKXNNXXKXWNKXNXKX0KMMMMMMMMMMMMMX0NMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMMMWKKWMMMMMMWXKWMMMMMMMWNXNXKWXKWMMMMMWXXWMMMX0KXKKXNXKKKNXKWMMMMMMMMWNXXWMMMMMMNXXWMMMMMMMMNXXXXXKXNXKXXNNXXXXNXKXXXXX0XMMMMMMMMMMMMMX0NMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXWMMMMMMMMMMMMWWMMMMWXXWMMMMMWNKKXXXXWWMMMMWWWXNWMMMMMMWNWMMMMWNNNXNNWWNNWMMXXMWXXXNWMMWWWWWNNWMMMMMWWMMMMMMMMMWWNXWMMMWNNNNWWNXNWWNNNNWWNNWMMMMMMMMMMMMNXWMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0WMMMMMMMMMMMWXXWMMMMWNXXWMMMX0KXXXXNWWXNMMMMMWXXNMMMMN0XMMMMMWXKXXKXWWNXK0X0KN0KXXNWXXWMMMMWXXXWMMMMMMMMMMMMMMMMWXXXWMWXXKXWNXXXNWXXXXWWKKWMMMMMMMMMMMW0KMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0WMMMMMMMMMMMMWNNMMMMMMWXKXNX0XWMMMMMMMNNWMMMMMMNXXNMW0KWMMMMMMX0KXXXNWNXXKN000KWMMMMWNWMMMMMMWXXNWWMMMMMMMMMMMMMMMWXKXNXXXXNXXXKNNXXXXNK0NMMMMMMMMMMMWKKWMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMWNMMMMMWNXXXXXXXNNWMMMWNWMMMMMMMNNWNNWMMMMMMMWNWWNNWWWWMMMNKNWMWWMMMMMMWWWMMMMWNWMMMMMMMMMMMMMMMMMMWNNNNWWWWWWWWWWWWWNNWMMMMMMMMMMMMNNWMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMXXMMMMMMMMMNXXXXXKKNMWWXKNMMMMMMMNXNMMMMMMMMMMWNXXWMMMMMMNKNMMMN0XMMMMMWNKXWMMMMMMMMMMMMMMMMMMMMMMMMMWNKXWMMNXNWNXNWNKNMMMMMMMMMMMMNKWMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0WMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0XMMMX0XMMMMMMW0KWMMMMMMMMWNXXNMMMMMMMW0KWMMMW00WWMMMMMWXXXWMMMMMMMMMMMMMMMMMMMMMMMMMWXKXWNXXNXXNN0KWMMMMMMMMMMMN0XWMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMMWNWMWWNWWMMMMWWNXXWMMWXXXXXXXNMXXWMMMMMMMWNXXNMMMMMMMMMXXWMMMMNXNNWWWMMMMMWXNWMMMMMMMMMMMMMMMMMMMMMMMMMMWXNNXXNXXNXXWMMMMMMMMMMMWXXWMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMMMMWXXXXXXXXXXXXXXXXXXXXXXXXKKNWWNKNMMMMMMWXNWWMMMMMMMMMMMNXNWMMMMMWWMWXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMMMNNMMMMMMMMMMMMMNNMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMK0WMMMMMMMMMMMMMMMMWNXXWWWMMMMW0ONMMMMMWWWWWWNOOWMW0KWMMMMWXXXWMMMMMMMMMMMMMMWXXNWMMMNXNWWXXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXNWN0XMMMMMMMMMMMMX0NMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMWXXWMMMMMMMMWKXMMMMMMMMMMMMWKXMMWXKNMMWNXNWMMMMMMMMMMMMMMMMMMWXXWMMMWWMMMWXXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWNKXWMMMMMMMMMMMNKNMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXNMMMMMMMMMMMMMMMNXNXXXXXXWWMMNXXXNWMWWXXXXXXXWMWWWWWWNNWWMMMMMMMMMMMMMMMMMMMMMMWWWNWMMMWXXXKXNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKXMMMMMMMMMMMMXXWMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMKKMMMMMMMMMMMMMMNKO0NNWWWNKKWMWXXXNWNKXNNWWNNKKWNXWWXKXWMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXNWW0OXNNNXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXXMMMMMMMMMMWWNNMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMKKMMMMMMMMMMMMMWKKK0NMMMMMKOKNXXXXXNXOKMMMMMWKKWK0XXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXNKKWMMWWXXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWMMMMMMMMMNWMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXMMMMMMMMMMMMMWNNNXNMMMMMWXXNNXNNXNK0XWMMMMWNNXNXNWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXNWMMMMWWWNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXWMMMMMMMMNNMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0NMMMMMMMMMMMMMMMX0NNNNNNNNNWNNNNNNWK00KNWNNNNNXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKNMMNXNNXXNWMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMKKWMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0XMMMMMMMMMMMMMMMK0NWWWWWWWWWWWWWWWWX0O0WWWWWNKKWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMWNXXXXO0MWXKXWNK00NMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMK0WMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "WMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMNKXWNNNNWNNNWNNNNNNNK0XWNWNNNXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNWMMMWWWNNWNXNWWXXWMMMMMWNKXXXXWMMMMMMMMMMMMMMMWWMNNMMMMMMMXKWMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMNXNNNWNNNNNNNNWNNNWWK0KNWMNXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXNXKKXNXXMMMMWNXNNXXXWWXXXXNNXOKNWMMMMMMMMMMMMMMMMMMNKNMMMMMMXKWMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMK0WMMMMMMMMMMMMMMMKONNNWNWNWNNNNWNWNWN0O0NWWKKWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXWWXXKXK0NMMWNXXWWXXXXNNXXXXWWKO0XMMMMMMMMMMMMMMMMMMMWNWMMMMMMK0WMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "WMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMKKWMMMMMMMMMMMMMMMXXNNNNWNWNNNNNNWNWMNKOKWN0KWMMMMMMMMMMMMMMMMMMMMMWNXXXXXNWMMMWNXXWMMNXXXXXKXNXXXWMMWXKXXNNXXXXWXKWXKWMMMMMMMMMMMMMMMMMMMMNNWMMMMK0WMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "WMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXMMMMMMMMMMMMMMMWXXNNNNNNNNNNNNNWNWMNXXNNXNWMMMMMMMMMMMMMMMMMMMMWWNXXXXKKKXXXNWNXNNWWNNNNWWNKKXNNNNNWWNNNWWWNWWNNNMWXNMMMMMMMMMMMMMMMMMMMMN0XMMMMNKNMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0KMMMMMMMMMMMMMMMN0KWNNWNNNNNNNNMWNWMN0XX0NMMMMMMMMMMMMMMMMMMMMWNKXMMMMWKXWXXXNWNXKXNWXKXXWNXKXNWXXKXWNXKKNWXKNWXXNMXOONMMMMMMMMMMMMMMMMMMMWXNMMMMN0XMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0KMMMMMMMMMMMMMMMN0XWWWWWWWWWWWWWWWWWN000XMMMMMMMMMMMMMMMMMMMMWKOKWMMMWK0NXXXXXNXXXKNNXXXXNXKXXXNXXXXNNKXKXNXKXXXKNN0XK0NMMMMMMMMMMMMMMMMMMMMWWMMMN0XMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKXMMMMMMMMMMMMMMMWXNNNWNNNWNNNNNNWNXNWXXNMMMMMMMMMMMMMMMMMMMMMNNNWMMMMWNWWNXXXXNWNNNWWWWWWWWNWNWWNNWNWWNWNWWWNWWNNWWNWWXXWWWMMMMMMMMMMMMMMMMMWNWMMNKNMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMMXNWNNNNNNWNWNNNNNXWXXWWMMMMMMMMMMMMMMMMMMMNXNMMMMMNXXNXXXWNXXXNWXXXXWWXXXNWNXXXWWXXXXWNXXXNWXXNNXXNNXK0XMMMMMMMMMMMMMMMMMMMWNWMMNKXMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0NMMMMMMMMMMMMMMMMKXMWWWWWWWWWWWWWMMN0XMMMMMMMMMMMMMMMMMMMMWKKWMMMMWKKNXXXXNNXXXXNXXXXNNXXXXNNXXXNNXXXXNNXXXNNXXXXXXX0O00XMMMMMMMMMMMMMMMMMMMNKNMMN0XMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMWNNNNNNNNWNNNWNNNNWXXMMMMMMMMMMMMMMMMMMMMMNXWMMMMMNXWWXXXXNNXXXNNXXXXNNXXXXNNXXXNNXXXXNNXXXNNXXXXXXNNNXKNMMMMMMMMMMMMMMMMMMMNXWMMNKNMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWMMMMMMMMMMMMMMMMMMMWWWWWWWWWWWWWWWMWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWMMMMMMMMMMMMMMMMMMMMMMMMMWWMMMMMMMMMMMMMMMM" );
                        Console.WriteLine( "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM" );
                    }

					#endregion

					break;
				case EndingType.Die:
                    Console.ForegroundColor = FOREGROUND_COLOR;
					for( int i = 0; i < 20; ++i )
					{
						for ( int j = 0; j < 20; ++j )
							Console.Write( "유다희" );

                        Console.WriteLine( "" );
                    }

					break;
			}

			#endregion

			#region Render Function
			void Render()
			{
				// 이전 프레임 지우기..
				if( isConsoleClear )
				{
					Console.Clear();
					isConsoleClear = false;
				}

				// Log Render..
				int logYOffset = 0;
				foreach ( var message in logMessage )
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.SetCursorPosition( logStartX, logStartY + logYOffset );
					Console.Write( message.Value );

					logYOffset += message.Key;
				}

				if ( isSkipRender ) // Render 를 스킵해야 한다면( 무한 깜빡임 방지 )..
				    return;

				isSkipRender = true;

				// Player State Render..
				playerStateLog.AppendLine( $"HP : {player.CurHp} / {player.MaxHp}" );
				playerStateLog.AppendLine( $"MP : {player.CurMp} / {player.MaxMp}" );

				Console.SetCursorPosition( playerStateLogStartX, playerStateLogStartY );
				Console.Write( clearPlayerStateLog );
				Console.SetCursorPosition( playerStateLogStartX, playerStateLogStartY );
				Console.WriteLine( playerStateLog );
				Console.Write( playerErrorLog );
				playerErrorLog.Clear();

				// Render Switch..
				for ( int switchIndex = 0; switchIndex < SWITCH_COUNT; ++switchIndex )
				{
					int posX = switches[switchIndex].X;
					int posY = switches[switchIndex].Y;

					RenderObject( posX, posY, SWITCH_IMAGE, SWITCH_COLOR );

					posX += switches[switchIndex].ButtonOffsetX;
					posY += switches[switchIndex].ButtonOffsetY;

					RenderObject( posX, posY, SWITCH_PUSH_IMAGE, SWITCH_COLOR );
				}

				// Render Portal..
				for ( int portalIndex = 0; portalIndex < PORTAL_COUNT; ++portalIndex )
				{
					for ( int gateIndex = 0; gateIndex < PORTAL_GATE_COUNT; ++gateIndex )
					{
						int portalGateX = portals[portalIndex].GatesX[gateIndex];
						int portalGateY = portals[portalIndex].GatesY[gateIndex];

						RenderObject( portalGateX, portalGateY, portals[portalIndex].Image, portals[portalIndex].Color );
					}
				}

				// Render Wall..
				for ( int i = 0; i < WALL_COUNT; ++i )
				{
					if ( true == walls[i].IsActive )
					{
						RenderObject( walls[i].X, walls[i].Y, walls[i].Image, walls[i].Color );
					}
					else
					{
						RenderObject( walls[i].X, walls[i].Y, " ", ConsoleColor.Black );
					}
				}


				// Render Arrow..
				foreach ( var arrow in arrows )
				{
                    RenderObject( arrow.PrevX, arrow.PrevY, " ", ConsoleColor.Black );
                    RenderObject( arrow.X, arrow.Y, arrow.Image, arrow.Color );
				}

				foreach( var arrow in removeArrows )
				{
					RenderObject( arrow.PrevX, arrow.PrevY, " ", ConsoleColor.Black );
                }
				removeArrows.Clear();


                // Render Trap..
                for ( int trapIndex = 0; trapIndex < TRAP_COUNT; ++trapIndex )
				{
					if ( traps[trapIndex].IsActive )
					{
						switch ( traps[trapIndex].MyType )
						{
							case Trap.TrapType.Bomb:
								// 폭발 범위만큼 그린다..
								BombTrap curTrap = (BombTrap)traps[trapIndex];

								int loopStart = -curTrap.curBurstRange;
								int loopEnd = curTrap.curBurstRange;

								for ( int offsetX = loopStart; offsetX < loopEnd; ++offsetX )
								{
									int trapX = traps[trapIndex].X + offsetX;
									int trapY = traps[trapIndex].Y - curTrap.curBurstRange;

									RenderObject( trapX, trapY, traps[trapIndex].Image, traps[trapIndex].Color );

									trapY = traps[trapIndex].Y + curTrap.curBurstRange;

									RenderObject( trapX, trapY, traps[trapIndex].Image, traps[trapIndex].Color );
								}

								for ( int offsetY = loopStart; offsetY < loopEnd; ++offsetY )
								{
									int trapX = traps[trapIndex].X - curTrap.curBurstRange;
									int trapY = traps[trapIndex].Y - offsetY;

									RenderObject( trapX, trapY, traps[trapIndex].Image, traps[trapIndex].Color );

									trapX = traps[trapIndex].X + curTrap.curBurstRange;

									RenderObject( trapX, trapY, traps[trapIndex].Image, traps[trapIndex].Color );
								}

								break;
							case Trap.TrapType.Trigger:

								break;
						}
					}
				}

				// Render Item..
				for ( int itemIndex = 0; itemIndex < ITEM_COUNT; ++itemIndex )
				{
					int itemX = items[itemIndex].X;
					int itemY = items[itemIndex].Y;
					string itemImage = items[itemIndex].Image;
					ConsoleColor itemColor = items[itemIndex].Color;

                    if ( true == items[itemIndex].isActive )
                    {
                    	RenderObject( itemX, itemY, itemImage, ConsoleColor.Blue );
                    }
                    else
                    {
                    	RenderObject( itemX, itemY, " ", ConsoleColor.Black );
                    }
                }

				// Render Box..
				for ( int i = 0; i < BOX_COUNT; ++i )
				{
					RenderObject( boxes[i].PrevX, boxes[i].PrevY, " ", ConsoleColor.Black );
					RenderObject( boxes[i].X, boxes[i].Y, boxes[i].Image, boxes[i].Color );
				}

				// Render Player..
				if ( MapSpaceType.Pass == mapDatas[player.PrevY, player.PrevX] )
					RenderObject( player.PrevX, player.PrevY, " ", ConsoleColor.Black );
				RenderObject( player.X, player.Y, player.Image, player.Color );

				// Render Goal..
				for ( int i = 0; i < GOAL_COUNT; ++i )
				{
					if ( goals[i].isGoalIn )
					{
						RenderObject( goals[i].X, goals[i].Y, goals[i].Image, goals[i].GoalInColor );
					}
					else
					{
						RenderObject( goals[i].X, goals[i].Y, goals[i].Image, goals[i].Color );
					}
				}

				// Render Map BorderLine..
				Console.ForegroundColor = BORDERLINE_COLOR;
				for ( int i = MAP_RANGE_MIN_X - 1; i < MAP_RANGE_MAX_X; ++i )
				{
					RenderObject( i, MAP_RANGE_MIN_Y - 1, initBorderLineImage, BORDERLINE_COLOR );
					RenderObject( i, MAP_RANGE_MAX_Y - 1, initBorderLineImage, BORDERLINE_COLOR );
				}
				for ( int i = MAP_RANGE_MIN_Y - 1; i < MAP_RANGE_MAX_Y; ++i )
				{
					RenderObject( MAP_RANGE_MIN_X - 1, i, initBorderLineImage, BORDERLINE_COLOR );
					RenderObject( MAP_RANGE_MAX_X - 1, i, initBorderLineImage, BORDERLINE_COLOR );
				}
			}

			void RenderObject(int x, int y, string image, ConsoleColor color)
			{
				if ( MAP_RANGE_MIN_X - 1 > x || x > MAP_RANGE_MAX_X ||
					MAP_RANGE_MIN_Y - 1 > y || y > MAP_RANGE_MAX_Y )
					return;

				ConsoleColor prevColor = Console.ForegroundColor;

                Console.ForegroundColor = color;
                Console.SetCursorPosition( x, y );
				Console.Write( image );

				Console.ForegroundColor = prevColor;
			}

			void 교수님죄송합니다()
			{
				Console.Clear();
				Console.WriteLine( "이스터에그 약 3초뒤에 나옵니다. 전체화면 추천" );
				Console.WriteLine( "이스터에그는 아무키나 누르시면 나가집니다. 전체화면 추천" );
				Console.WriteLine( "교수님 늘 좋은 수업 해주셔서 감사합니다." );
				Console.WriteLine( "감사한 마음을 담았습니다." );

				Thread.Sleep( 3000 );

				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.DarkGreen;

                Console.Clear();
                Console.SetCursorPosition( 0, 0 );

                #region 교수님 아스키아트
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!*=@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@~;;!**==***!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!*!!*======*$===#==*!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@=***=$=====$#=$$$=$=*==!*=!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@===#$#=$#$=*$#$$###$@$#$#$=*;;@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@=$*$==$$$=$$#@##@########$##*!;;;;@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@=$$$==$=$*$$=#$$$#####@##@@@##$$=!:;;@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@**$$$=**=#$$$==$####$#######@@@@$##=!:;@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@=$##$#!$=====$$$#######@###@#@#@@#@#$;;;@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@**=!#=!$=$$=$$=$$$$######@@###$#@@@@@@@#*!:@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@**=$$***=$=$$==**=*!**==$$$###$#@@@@@@@@$==;;@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;*=*=$!=!$$$$=**!!;;:::;;!**==$$$@@@@@@@@@$$$;@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*!**$==$##$$==*!;:~~~~~~-~~~~:;!*$##@@@@@@@$=*~@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*=!=$!=#$$$$$==!;~~---,,,,,,,,,,-~;!*$@@@@@@@@#*~@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;!***$$*$$$$$$=*;~---,,............,~:!=$#@@@@@#$;@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*=$==#$#$=$$$$*;:~--,,,.............,,-:;*$#@@@@$;@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;;!$#=$$$$$$$$=*:~--,,,,................,-~;*=#@@#*-@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*=$$$=$$$$$$=$*!~--,,,,,........ ..   ....,-~;=$@#$!;@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*$$=$=$$$$$=$=*;---,,,,,.......        ......-:*$#$*~@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!*$$$=$$=$$$$=*!~---,,,,,.......  .      ......,~;##$!~@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*==#$=$=$$===*!:----,,,,,,.....   .         ....,~*@#*;@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*$$$$$=$$=$=*!;~----,,,,,.......    .        ....,:#@=!@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@=##$$=$$$=$=!;~---------,........  ..           ..-*@$*~@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!$#$$$$$=$==*;:~~~~~~~:;:~~,,.......  . .         ..;##=:@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!$##$$#==$=*!;~~~~~:;;:;;;:::~~-,...   ... ..      .:##=;@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@==$#$$##=$==*;~~~~~~~~~~~~:~;!;;;~,...  ..   . .     ~##$;@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@$$$#$$#$===*;~~~~~~~------,,-~:;;:~-,.  .            ,##=!@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@.............@@ ... ........@@@............@@@@@@@@@@@.......@....@@@@@@@@@@@....@.......@@@@..... @@@@@@@@@@@=$##$$#$$$*!:~~~~~~~~-----,..,-~::::-........        .$#$*@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@.............@@.............@@@............@@@@@@@@@@.............@@@@@@@@@..............@@@....... @@@@@@@@@@$$####$$$=*;~~~~~~~~~~~~~--,...-~:::~-.........       *#$=;@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@.............@@.............@@@............@@@@@@@@@ .............@@@@@@@@@..............@@.........@@@@@@@@@@=#####$$$=!:~~~--~~~:~::::~,...,-~~:~--,.,-----,.     -#$*@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@.....@....@@@@@.............@@@............@@@@@@@@@.....@.........@@@@@@@@ .............@ ......... @@@@@@@@@$###$$$#$=;:~~----~:;;!**==*:-,,,-~~---,,-::;;;:~,  . ,##*@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@.............@@..............@@............@@@@@@@@@.....@.........@@@@@@@@@.............@.....@.... @@@@@@@@@$@###$#$$!::~--,,,-~~;!*!=$$$=;------,,,,~:~::;;;;-.  ,@#*@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@;............@@..............@@............@@@@@@@@@ ..............@@@@@@@@ .............@ .........@@@@@@@@@@#@@#$$$$=;::~--,,,,,,--~~;=$*;*;----,...,-~~~---~~,.. ,@#*@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@.............@@..............@@............@@@@@@@@@@.............@@@@@@@@@..............@@@@@......@@@@@@@@@!#@@##$$=!;:~~--,,,,..,,--,,--~:*-,,,....,---,,.....,,.,@#;@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@.............@@.............@@@............@@@@@@@@@@@.......@....@@@@@@@@@..............@@@@@.....@@@@@@@@@@$###$$*=*;::~~---,,.....,--~-,,.,.,,,..  .---,,...  .,,-@$;@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@.............@@.............@@@............@@@@@@@@@@@....@@@@....@@@@@@@@@..............@@@@.....@@@@@@@@@@@=~,-;:;**;::~~--,,,........,,,....,,,.. ..,-~:~,.  ....;@;@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@............@@.............@@@@@@@....@@@@@@@@@@@@@@@....@@@@....@@@@@@@@@..............@@@@.... @@@@@@@@@@~-~!;~:;!!;::~~~-,,,,...   ...   ..,,,..  .,;=$$$!- ....*#;@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@............@@.............@@@@@@@....@@@@@@@@@@@@@@@....@@@@....@@@@@@@@@@.............@@@@,...@@@@@@@@@@@ ~;*!:;;;;;::~~~-,,,,...  .    ...,,,,.   ..-:*$$=*:.,..==~@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@.....@@.............@@..............@@@@@@@@@@............@@@@@@@@@@.............@@@@....@@@@@@@@@@@~;;!!:;;;;;::~~--,,,,,.... . ....,,,,,.   ..,--~;:!*~,..$*-@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@.....@@@............@@..............@@@@@@@@@@............@@@@@@@@@@@@.. @.......@@@@....@@@@@@@@@@@::;;!;;;;;;:::~---,,,,....  . ...,,,,,....  .,--,..,!-..#!@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@.....@@@@@@@@@@@....@@..............@@@@@@@@@@............@@@@@@@@@@@@@@@@.......@@@@....@@@@@@@@@@@;;~~:;;;;;;;:~~~--,,,,... .. ....,--,,..     ..,-,. ,-..*;@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@.......@@@@@@@@@@@@@@@@@@@;::;$=::;;;::~~~---,,,....  ....,--,,,..      .....  . .!@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;:;*#!~::;;::~~~~---,,,.......,,,,,,,,,.  . .   . ..   .@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@~:!*$~:~:;;:~~~~-----,,,.....,,,,,,,,--,..   ..        .@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@~:!**::~;;;:~~~------,,,,...,,,----~:~:~,. ..          .@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@::;;!!::;;::~~~-------,,,...,,~::;=$*;;;~-...     .    ,@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:::~:!::;;:~~~~--------,,,,,..~;;!***!!*=;, .    ..    .@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:::::::;:;:~~~~--------,,,,.,,,-:~~~:!=$#=:...  ..  .. .@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;:::::;;:::~~~~~-------,,,.,,,,,,,,,,-~;;;:,.... .     ,@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-;:~:;;;:::~~~~~-------,,,,,,,,,,,,.....~:~.....      .-@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:;:;;;;;;:~~~~-------,,,,,,,,,,,,..  .  . .,.... ... .@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@,~;=*:!;;:~~~~-------,,,,,,,,,,,,,. ....   ..........,@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-::!*:;;;:~~~------,,,,,,-----~:;:~,..  .. .........,-@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-*::;!~!;;:~~~~~----,,,-,---~:!!!!!;:--,.   ........,,~@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!#~:;;~!;;:~~~~~-----,,,--~:!**==*!;::::~.   ....,,,,-;@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:##-:;;~!;;:~~~~~-----,,--~;*=$$$$***;;;;;- . ...,,,,--@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;=@#,:::~!;;::~~~~~----,----:;!;**=*-~!**!!:......,,,,-~@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!$@$,::;-!;;::~~~~-----------~~~---~, ~===*;,....,,,,,~~@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!=#@$-::-,:!;;::~~~~-------,,,----,..~.  ==*!-....,,,,-:@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@~@@:;$#@=-:--~:;!;::~~~~-~---------~~-----:.,,:*!:,...,,,-~@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@.~*#*@;;;*$@@!~~,.~;:!;;::~~~~~~~~~-----~~~~~:~!;...-~~,..,,--~;@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ @.-;=#$,~:!!$#@@;~,,,,;;:!;:::~~~~~~~~~-------~~;;!*~.::.,,,,,,--:@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@   @@@@@@@ - .,,,, -!##@-~;=**=#@@;,,.,,~::!!;::~~~~~~~~~~---------~!!;~;;.,,,,---~:@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ .... .-~--,,,,,-,,.. -!$#@@*;=***=$#@@~..,,,-~::!!;:::~~~--~-------,,,,-~:~~~!,,----~~;:@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@,,........,,~~---~:!:~,...~$#@@@@*==!**=$#@#:...,--~::!!;;::~~~----------,,,,-~:~-~*~,--~~::;@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@~.,-.  ..,,,,--~;;:-~:!;~...-;=$#@@@@#****!*=$$@#;~...,-~::;*!!;;::~~------,-,,,,,-~~~-~!:~~~:::!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-,,.,~;~,,.,-~,~***!;$$;~-,:*=###@@@@@@=!***===$#@#!;,...,~:::!*!!;;::~-------,,,,,,--~~-~~:~~::;;@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@,,,...,---!*~--~;*!;*$!=####$#$####@@@@@@#***=====$#@@*;:,..,-:::;!**!!;;:~~~----,,,--------~~~~::;!:@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@,-,..,----~--;$=~~;!;!=$*=##########@@@@@@@========$$#@@*;;:,,,,-::;;=****!;:::~~~~----~~-~-----~~::!;-,,-@@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-,,..,-~::~~~!:;*$$~~!!!*=!$##########@@@@@@@!!*!==*=$$#@#*;;;:-,,-~:;;!===***!;:::::::~~~:~~-----~::;!------@@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@~,..,-::::!*!*=$;=$$#*~;*$$!=#########@@@@@@@***!;==*=$##@#*!;;;:--,-~:;;!$===***!;;;;;::::::::~~-~:;!*,~-,,~~-@@@@@@@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@~~-..,-;;:!*==$=======$$#$==$#*$##$#######@@@@@#!*=!;===$$$#@#=!;;;!:~---~:;;*$$====****!!!;;;;;;;::::;!*~,:::!$=*!:~,,~@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-,..,-~:;*;==$=$$$$==$=$$$$####=$##########@@@@@*!*=**====$$#@#=!;;;!!;:~-~::;;*#$$=======***!!!!*!;;;!**:,,~~:*@@##$*:-,@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ --,.,,~:;:====$$$$$$=$$$$$$$=$###=$####$####@@@@@$**=$$$$===$$##$=!;;;!!!!;~-~::;!*$##$=$$$$$===********==:,. -~:=##@##$=!;@@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@~-..,.,-~!:!*==$$$$$$$$$#$$$$$$$$####=$####$###@@@@@@**==$$$$==$$$$$==*;;;;!!!!!:~:;:;!*$###$$$$$$=$$$=$==$$=:,...-~:=####$####$@@@@@@@@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@~,.,,,,-~:!!!!$$$$$$$$$$$$$$#$$###$###@$$#####$##@@@@@=**=====**==$$====*;;;;!!!*!!!!;;;!!*$####$$$$$$$$$$$$=*:-,.-~-~~:;!=#$#@#$=*;@@;~;@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@,.,,---~~::;*=$==$$$$$$$$$$#####$#####@@$######$##@@@@@*========$$$$$===**!!;;;!**!!!**!!!***$$$####$$$$$$$$=*:---:~--:::~:;$#@##$==*:@~:~: @@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ----~~::;;;**$=$=$$$$$#####$#############$########@@@@@#===$$$$$=$$$$#==$=**!;;;!!!!*!***!*****=$$$$$##$#$$=**;~:;;~~~~:~::~;=#@@##$$=!@~~~~@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;::~:!;:****=$=$$$=$$#$$####$############@$#####$#@@@@@@==$$$$$$$#$$$$$==$=**!;;!;;;;!***********=$$$##$$==***;;;::-~--~~~~:::!$###$$$$!-:- @@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*!;;::;;*====$=$$$$$$#$$$$$$#$##############@########@@@@@#==$$$$$$$$$$$$#$=$***!!;!;;;;;!************=$$======*!!;:~~-----~~::::;*@@####$!-@~~@@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!;;;;;!!=$$$=$$$$$$$$$#$$$$$$############@@###########@@@@@$==$=$$$$$###@@@@##*!!!;;;;;;;;;!*!*====*****========***;:~~~~~--~:;::;!!=$$###$*:-~,~@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;;:::::*$$==$$$$$$$$$$$$$$$$########$########@@########@@@@@====$$###@@@@@@@@@#=*!;;;;;;;;;::!!!!*====$===========!:~~~~~---~~:;::;!**=####**:~--~@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:::::;*==$$$$$$$$$$$$$$$$$$$###########$#####@@########@@@@@#*====#@@@@@@@@@@###$**!;;;:;;;;:::!!;!**===========*;::~~~~~~---~~~~:::;==*###$$=~---~ @@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:;;:;;!*==$=$$$$$$$$#$#$#$$$$$$$########$#######@########@@@@@==$$$@@@@@@@#@@@$=#@$*!!;;;::;;;;:::;!!**======**;;;;!;:~~~~------~~~~~::;!#$$$##$*;;: @@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@~;;;;!!!*=$$$$$$$$$$#$$#$$#$##$$$$################@#######@@@@@#####@###@@####@@#$@@$!!!;;;;;;;;;;;:~;!!**=**!;;!;;;;!!;:~~~~-----~~~~:;::*$$##@@#*=!!~@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!;;!!!!*=$$$$$$$$$$$$#####$#####$$$################@########@@@@#########@@###@@@@@@#$**!;;;;;;;;;!;;::!!**;::!!!!;;;;;;;::~~~~~-~~~~~::;:;!$###@##$$$;;@@@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!!!!**=*=$$$$$$$#$$$$#####$####$$#############$#####@######@@@@@####$#################$**!;;;;;;;;;;;;;;:::~~~:;;!!!!;;;;;::~~~~~~~~~~~~;*!!!##@@#$$$$*;: :@@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;!!**==$$$$$$$$$$$$$$$#####$$$######$$############@###@######@@@@@##$$$######@##$$#####@$*!!!!;;;;;;;;;;;;::~~~~:;;;!!!;;;;::::~~~~~~~~~~~!!!!=@@@#$##$$*!:-;;@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*!***$$$#$$$$$$$$$$$$$###$#########################@###@######@@@@@##$$###$$############@=**!!;;;;;;;::;;;::;::::::;!!;;!!;;:::::~~~~~~~~~:!!!!#@@###@$$=**;~~-;@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@**==$$$$$$#$$$$$$#$$$$$############################@@##@@#####@@@@@####$###$$$#####$#$###=****!;;;;;;;;:;;::::::::::;!!;;!!;;;::::~~~~~~~:~:!!=$@@##@@#$$===*:~~,@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*==$$$$$$$###$$$####$$$########$####################@@##@######@@@@@#####$##$$###$##$#$##=*****!!;;;;;;;:;;::::::::::;;;;;:;;;:::::~~~~~~~::;!!=@@@@@###$$=$$*!~--@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@**=$$$$$$#####$$$####$$$#####################$$######@@##@#####@@@@@@@#######$$$##$#$$$$#=******!;;;;;;;;;:::::~::;::::;;;;::;;:::::~~~~~~::;;*!$@@@@###$=====!*;~~;@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*==$$$$$$####$$$$###$#################$#####$$$######@@@@@#####@@@@@########$$$$##$##$$$*!*!**!!;:;;;;::;:::::~~~:::;:;;;;;;::::::::~~~::~~:;*$#@@@######@@@#==!::: @@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*=$$$$$$$$$###$$###############$$#####$$$####$########@##@@#####@@@@@########$$$$$$$#@$$#!*!!!*!;;:::;;;:::::::::::::::;;;;;::::::::::~~::~~:!$@@@@@##@#####$***=!~~ @@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@$$$$$$$$$$$$#############################$####$########@##@@####@@@@#@##########$$$$$$####$!*!!;;;;:::;::::::::::::::::;;::;;::~:::~:::::::::;;#@@@@#@@#####$====*!~~@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@$$$$$$$==$==$$####$$############$########################@##@@####@##@###########$$$$$$$#@#$#=!!;;;;;:::;::::::::::::::::;:::::::::::::::::::;;*=#@@@@@@###@@#=$#$$=*-@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@@$$#$$$$$=$$=$$############################$$$###########@##@@@####@#############$$$$$$$$##$$##*;;;;;::::;:::::::::::;;:::;;::::::;::::::::::;;*##@@@@@@##@@#$=$##$==*~~;@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@@$$$$$$===*===$##$#############$$##########$$##########@#@@@#@@@####@#############$$$=$$$$$#####$!;;;;;:::::::~~:::;:;;;;;;;;;;::;;;!;!;;;;;::;;=$$@@@@@##@@###@@@#$$**:,;@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@@##$$$$$==*==$$$################$$#########$$##########@@@@@##@@@###@##############$$=$==$$$###$$$#!;;;;:::::::~::;::::;;;;;;;;:::;!*=**!!!!;;;!*===#@@@#@@@##@@@@#$$$$*:-@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@##$$$$=$$=**=$$$###############$$##########$$###########@@#####@@#########$#$#######$$$====$=$$$#$$$$;;;;;:~~::~::;::::;:;;;;;;::::;!*=**!!!!!!**=$#$@@@@@@@#@@@@###$$$==!; @@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@##$$$$$====$===$################$$###########$############@@#####@@####$$$#$$$$$#######$$$========$$=$$*:;;::~~:::::;::::::::;;;;::::;;!*!!!!!!!!=$#@@@@@@@@##@@@@######$$*! @@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@###$$$=$$$*=*=$$##################$##############################@#$$$$$$$======$$$#$$$$$====**=***===*!;:;;;:::::;!;:::::::;;;;;;:::;!!!!!!!****$@@@@@@@@@@@@@@######$$$$*!@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@$$$$$$===*=$=$$$#################$$################################$$$$$$$=****===$$$$$$=$===**!*****!***;;;;::::;;;::;;:;;;!!;:;;::;;!!!!!!!!!!*$@@@@@@@@#@@@@#######$$$$*!@@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@$$$$==$==$$==$$$##################$$$########################$$####$$$$$$$==*!!!!**===========*!*!!!*!;!*!!:;;:::;;;;::;;;;;;;!;:;::;;;!!!!!!!!!!*$=#@@@@@#@@@#########$$$$**~@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@#$==$$=$$=*=$=$##############$$##$$################################$$=$$$$==**!!!!!*****=====******!!!!!!!!!;::::;;;;;;;;;;;;::;;::::;;!!!!!!!!!!=**=#@@#@#@@@######$$$#$#$$**:~@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@$$$====$===*$$##############$$#$#$$################################$$=======**!!!!!!*!!**********!!!!;;;;;;!!:::;;;;;;;::;;;;::::;;::;!!!!!!!!***$=**==###@@@@@@#########$$$*=!@@@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@=======$=***=$$$################$$$#################################$=======*****!!!!!!!!!****!*!!;!!;;;;;:;;!;::;;;;:;::::;;;;::;:;::;!!!!!******==!**=#####@###############===-~@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@=$====*===*==$$$##################$##################################$=**********!!!!!;;;;!!!!!!!!!!!;;;;;::;;!;:;;:::::::::;;;;::::;;;;;!*!!!*****==!!==$###=$#############$$*==~: @@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@*==$=*===$=*===$$######################################################$=***==***!!;!!;;;;;;;;;;;;;;!!;;;::::::;;!:;!;;::::::;:::;:::;;;;;;!!;;;!*!!!**!;=$*==#$*###########$$$$*$=:;;@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@@!*===$====*$#==$$#######################################################$==****!!!!!!!;;;;;::::;;;;;;;;;::::::::;;;:;;;:::::::::;:::::;;:;;;!!;;;;!!;!;!!!$==**$==#@##########$$=*$$!:@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@@*=$$=*==$=#$*===$#######################################################$$==*!!!!!;!!!;;::::::::::::;;;;:::::::::;;;;;;!;:::::::::::;::;;;;;;;;;;;;;;;;;!!***==*#==#####$$$$$$$$==*$$* @@@@" );
				Console.WriteLine( @"@@@@@@@@@@@@!!===*==$==***=$$$#######################################################$$**!!!;;;;;;;;:::::::::::::;;;;:::::::::;;!;;!!:::::;:::::::::::;;;;;;;;;;;;;;;!!!;*=*=#$######$$$$====$=*$$;;@@@@" );
				Console.WriteLine( @"@@@@@@@@@@@=$$$$#@*!===***=$$$############################################@@########$$=**!;;:;;;;;;:::::::::::::;;;;:;;:::::::;!;;;!!:::~:;;::~~::~:::::;;;;;;;;;;!!!!;;;!==$#######@##@###$====$#!:;@@@" );
				Console.WriteLine( @"@@@@@@@@@@$==$==**=***!;*=$$$############################################@@########$$$=!!;:::;;;;:::::::::::::::;;;;;:::::::::;;;:;;;::::::;::~::~~:::::;;;;;;;;!!!!!!;;;!**$##@@@@@@@@@@@@@#$*$#$*:@@@@" );
				Console.WriteLine( @"@@@@@@@@@$======*!!!==*!*==$$###########################################@@@########$$$=!;;::;;;;::::::::::::::::::;;;:;:::::;;;;;:;;;;::~:::::~~:::~::::;;;;;;;;;;!!!!;;!!!=@@@@@@@@@@@@@@###$=$##$~:@@@" );
				Console.WriteLine( @"@@@@@@@@$#===*!!!*!;*$#*:=#$$#@@@##########@###########################@@@@########$$$=*!;;;;;;;;;:::::::::::::::;;;:;;:::::;;;;;::;;;::::::::::~~::::::;;;;;;;;;!!;;!;;!!!##@@@@@####@@##@#$=$##$$~@@@@" );
				Console.WriteLine( @"@@@@@@@@=!*$=**===;:=$###$$####@@@#@@@@@#@@@###########################@@@@#######$=$$$=*!!!;;;;::::::::::::::::::;::;;;:;;;::;;;::;;;;::::::::::~::::::;;;;;!!!;*!;!!;!!!=################$$=$###$;@@@@" );
				Console.WriteLine( @"@@@@@@@!$=:;$=$$$##$!*==#$#$###@@@@@@@@#@@############################@@@@@#######$$$#$==**!;;;;:::::;:;;;::::;:::::;;;;;;;::;:::::::;;::;;::;:::::::::;;;!!!!!!*=$=!!;*!*=##############$$$$$$####!@@@@" );
				Console.WriteLine( @"@@@@@@~!!!!:!!!!!*$###$$=$######@@@@@@##@###@##########################@@@#############$==**!;;;;;;::;;;;;;;;;;;;:::;;;;:;;:;;::::::::::::;;;;:::::::;;;;;;!***==$#=*!!*!*$@@@#@########$$##$#=####=@@@@" );
				Console.WriteLine( @"@@@@@!;:;:~!$=$$$$*!=$#$########@@@@@@@##############################@@@@##############$$==**!;;;;;;;!;!;;;;;;;;;;;;;;;;;;;;;;:::::~~~::::;;;;;;;:;;;;;;;;;!***==$=$=***$#@@@#########$$$###$$=$##$*@@@@" );
				Console.WriteLine( @"@@@@:;;!;=$*;!!!==#@*;##$########@@#@@############################@##@@@@########$$#####$===**!!!!!!!*!!!;;;;;;;;;;;;;;;;;;;;;;;;:::~~::;;;;;;;;;;;;;;;;;;!!**==$=$#$=*$@@@@#########$$####$$##=$$$*@@@@" );
				Console.WriteLine( @"@@@!;:~~~:;**:::;*=$@#=$#########@#@###########################@#@###@@@@########$$$####$===**********!!!;;;;;;;;;;;;;;;;;;;:;;;;:;:::::;:;;;;;;::;;;;;;;!*****=$$$#$$#@@@@@#######$$######$$$#=$$$=@@@@" );
				Console.WriteLine( @"@@;~~--~~;==*!!;::!=$#@$######################################@@@#$$#@@@@#######$=$$$#$$$=====*********!!!!!!!;;;::;:::;;;;:::;;;;:;;::;;;;;:;;;;:;;!;;;!******=$###@@@@@@@###############$$$$#==$==~@@@" );
				Console.WriteLine( @"@@:;=#=**::;;!*;;;!*###@####################################@@@@#*$#@@@@########$==$$$$$=======****=******!!!;;;;;::::::;;:;;;;;;;;;;;;;;;;;;;;;;;;!!!;*==$==**$#@@@@@@@@@@############@#$$$$$$$*===:@@@" );
				Console.WriteLine( @"@#=!!==*;:-~:;$#$$!;*###@################@################@@@@@#$=*@@@@@########==*==$$$=======***********!!!!;;;:::::::;;;;;;;;;;;;;;;;;!;;!!;!!!*==**=$=$=*==#@@@@@@@@@########$######$$#$$$$#=**$:@@@" );
				Console.WriteLine( @"@#;--!*=*==**;~!=$$$==@@@@###############################@@@@@#===*@@@@@#######$=====$$======***********!!!!;;;:::::::::;;;;;;;;;;;!!!;;!!;;;;;;!!*====###$$=$#@@@@@@@@##########$$#####$$$$$$$$$*==:@@@" );
				Console.WriteLine( @"@*;*!;;::-~~:;!;**=$###@@@@############################@@@@@@#=*!*$@@@@########$====$$$=*====********!!!;!!;;;;:::::::::;;;;;;;;;;;!!!!!!!!*!;;!**=$$$#@@@@###@@@@@@@@##@#@@@@####=$$###$$$$$$===**$!@@@" );
				Console.WriteLine( @"!--~:~~:;****!:!$$*=$#@#@@@@#########################@@@@@@@@@@*:!@#@@@########$$$===$$$=======***!!!!!!!!;;:;:::::::::::;::;;;;;;;!!!!*!!***!***=#@###@@@#####@@@@@@@@@@@@@@@@@@#$==$$$$$$======*;$*@@@" );
				Console.WriteLine( @"---~;;:;::;:;$=!;==$=####@@@##################@@####@@@@@@@@@@@;;*@@@@########$$==$$$$$$$=***===*!!;;;;;;!;;:::;:::::::::::::::;;;;!!!*=!***=$$$$$#@@@#@@@@@@@@@@@@@@@@@@@@@@@@@@##$====*=========:$*@@@" );
				Console.WriteLine( @",----~---~~~~~:;=$=$$$###@@@@#################@#@####@@@#@@@@@@@!$@@@@########$=$$$=$$$$=******!!!;;;:;;;;;::::::::::::::::::;;;;;;!!!****==$$$####@@@@@@@@@@@@@@@@@@@@@@@@@@@#@##@###$$$=======$=;==@@@" );
				Console.WriteLine( @"*$*!$*!;;--:;;~~;$$$$$####@@@@################@@#@@@@@#=@@@@@@@@~$@@@@##$#####$$$$==$$===*****!!!;;:::;;;;::::::::::::::::::;;;;:;;;*******$#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#####@@@@@###########$==!==:@@" );
				Console.WriteLine( @"*####@@##@$$!~:!==$$$#$###@@@@@@#####$$########@@@@@$=@@@@@@@@@@:#@@@@########=$$$$$=========**!!;::::;;;;;::::::::::::::::;;;;;;;!*========$@@@@@@@@@@@@@@@@@@@@@@@@@#########@@@@@@@@@@@#####@@#=!$@@@" );
				Console.WriteLine( @"$$#######@@@@@#*~=#=$$####@@@@@@@@#@####@@@@###@@##=@@@@@@@@@@@@!@@@@@#######$=$$$===**=======**!;;:::;;;;;:::::::::::::::;;;;;:;;!**=====$$$@@@@@@@@@@@@@@@@@@@@@@@@@####@#######@@@####@@@######$!=@@@" );
				Console.WriteLine( @"$$=$=$$==**!!*#@#==##$$####@@@@@@@@@####@@@@@@##*@@@@@@@@@@@@@@!$@@@@########$$$$$=======$$=====!;;;:::;;;;:::::::::::::::;;;;:;;!**!***=$###@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#@##$$$$###########$#$;!@@@" );
				Console.WriteLine( @"#$##$==$==#@#*!=###$#######@@@@@@@@@@@@@@@@@$;@@@@@@@@@@@@@@@@~=#@@@@########=$$$$=======$===$$=*;;::::::;;;::::::::::::::;;;;:;!!******=$###@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@##$=====$$#$$$$$$$!*@@@" );
				Console.WriteLine( @"#$$$=$$$$===$#@$~*##$###@##@@@@@##@###@@##@@!~@@@@@@@@@@@@@@@@!##@@@@@#$$###$$$$$$$====$$$======*!;;;;:::;:;;::::::::::::;;;;;;!*!!****==$$##@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#####@@#$$$*!!$$$==*=;*@@@" );
				Console.WriteLine( @"#$*;;:~--~:!==$#@==##$####@@@@######@@@##@@*@@@@@@@@@@@@@@@@@!$@@@@@########$$$$$$$$====$====$$=*;;;;::::::;;;;;::;;:;;;;:;;;;;***!***=*==$$#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@######@@@@@@###$*!!!=$$$*;@@@" );
				Console.WriteLine( @"=*~-,,-.,,,,-~=$$@#=#####@@@@@#########@@@$@@@@@@@@@@@@@@@@@:=#@@@@########$$$$$$$$$=====*===$==*!!;;;:;::;;;;;;:;;;;;;;;;;;;;!!!!!!!!!!==$$#@@@@@@@@@@@@@@@@@@@@#@@@@@@#####@@@@@@@@@@@@@@#$!;;!**$!:@@" );
				Console.WriteLine( @"#~:~:~~:~~,--~~!#$#@$####@@@@@@@@@##@#@#@#@@@@@@@@@@@@@@@@@@*$@@@#@@#######$$$$$$$$$$==========**!!;;;::;::;;;;;;;;;;;;;;;;;;;;!!!;!!;!!*$$$#@@@@@@@@@@@#@@@@@@@@$@@@@##@###@@@@@@@@@@@@@@@@#$*;!!*$!~@@" );
				Console.WriteLine( @":~:!:~~----~::~~;#$#@$###@@@@@@@@@@@@@#@*@@@@@@@@@@@@@@@@@@-$#@@@@@@######=$#$$$$$$=$$========*!!!;:;;;;;:;;;;;;;;;;;!;;;;;;;;!!!;!!;;;!==$$@@@@@@@@@@@@@@@@@@@#@~@@@@@@@@#@@@@@@@@@@@@@@@@@@@#*;**== @@" );
				Console.WriteLine( @";~;:~~--~~~:::;;~=##@####@@@@@@@@@@@@#@$@@@@@@@@@@@@@@@@@@@!$@@@@@@######$=$$$$$$$$$$$$$==$$==***!;;;;;;;;;;;;;;;;;;!!;!!!;;;!!!!!!!;;**=$$#@@@@@@@@@@@#@@@@@@@@=-$@@@@@@#@@@@@@@@@#$$###@@@@@@#!;!==;@@" );
				Console.WriteLine( @":::~;;!;~~-~~;~;;:###@###@@@@@@@@@@@@@$@@@@@@@@@@@@@@@@@@@:##@@@@@@######=$$$$##$$$$###$$$$#$$=***!;;;;;;;;;;;;;;;;;;;!!!!;;;;!!!*****==$##@@@@@@@@@@@@#@@@@@@@#!@$@@@@@@@@@@@@@#$$$=*@##$$#@@@@#*;=$~@@" );
				Console.WriteLine( @"!!;::;~~~-~~--~~:;$#######@@@@@@@@@@##=@@@@@@@@@@@@@@@@@@@*@@@@@@@@#####==$$$$####$$#######$$$$=**!;;;;;!!;;;;;;;;;;;;!!;;;!!!!;!***==$@@@@@@@@@@@@@@@#@@@@@@@@!@@@;@@@@@@@@##$######$*@@@####@@#@#!=@@@" );
				Console.WriteLine( @"!!*;:::~-:-~-:::~;####@###@@@@@@@@@#@#@@@@@@@@@@@@@@@@@@@@$@@@@@@@@$###$=$$$$$$############$$$$=**!;;;!;;;;;;;;;;;;;;;!;;;!!;;!*!!***=$@@@@@@@@@@@@@@@#@@@@@@@$@@@@@;@@######@@@@##@@#$*#@@@@@#@@@##*@@@" );
				Console.WriteLine( @"!**;!*;!==*!;~::;:$###@####@@@@@@@#@#@@@@@@@@@@@@@@@@@@@@**@@@@@@@#####=$$$$$##############$$$$$=*!;;;!!;;;;;;;;;;;;;;;!;!!!!!****====#@@@@@@@@@@@@@@#@@@#@@@#:@@@@==@@@##@#$$=@@@@@@@@#==@@@@@@@@@## @@" );
				Console.WriteLine( @"===###=;:~:;;*!::;$###@###@@@@@@@@@#@@@@@@@@@@@@@@@@@@@@@@=@@@@#@@####$$$#####$###@@@@@####$$###$=*!;;;!!;;;;;!;;;;;;;;;;!!*!========$@@@@@@@@@@@@@@##@@@@@@@*@@@#@=;~----~!$@#==$$$#####$=#@@@@@@@@@~@@" );
				Console.WriteLine( @"=$#$~..    @  !*;:=###@##$*$#=$@@@@@@@@@@@@@@@@@@@@@@@@@@@$@@@@@@#####=$#########@@@###########$==*!!;;!!;!;;!;;;;;;;;;;!!!****===$##@@@@@@@@@@@@@@@$@@@##@@#@@@@#;,----,,,..~$@@!:!;;;*==$$#@@@@@@@@:;@" );
				Console.WriteLine( @"@@:,,...     . ~=;$###@@@*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#@@@#@@####=##########@@@@@######@@##$===!;!!;;;;;!!;;;;;;;;;;;;***!**=$#@@@@@@@##@@@@@@@##@@@#@@@#@@@$----------,,,.-$@#~*$=*;!$*==@@@@@@@;;@" );
				Console.WriteLine( @"@:.......    .  !!=##@@@$ @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!:#@@#@@####=$#####@@@@#@@@@@@#@##@###$$==*!!*!!!;;;;;;;;;;;;;;!!!****!*=###@@@@@@@@@@@@@@##@@@##@@#@@;!~~-------------,,!@@!~$$$!;!=**#@@@@@!@@" );
				Console.WriteLine( @"~:,.... .   . ..,#$###@@*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!;:@@@##@###=$#####@@@@@@@@#@@@@@@###$$====**!!!!!;;;;;;;;;;;;!!!!!*****==$##@@@@@@@@@@@@@@##@@@##@@:@;:~~~~----~---------.:#@$-!#$$!;*!!#@@@@=@@" );
				Console.WriteLine( @"; ,..,-,-,., ....$###@#:@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@!;!@@######=$####@@@@@@@@@@@@@@@@@##$$$$$$=***!!!!;!!;;;;;;;;;!;!!!*=***==$#$#@@@@@@@@@@@@@#@@@##@@=,~:~~~~~~~~~~~~~~~~----,-@@@,;##$*:;:;#@@@=@@" );
				Console.WriteLine( @"   .,-~:~:::~~,..~@##*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@;;*@###$$$==#####@@@@@@@@@@@@@@@@##$$$$$$$*!**!!!;;!;;;;!!!;;!!!!!******==$#$#@@@@@@@@@@@@#@@@###@@;::~~~~~~~~~~~~~~~~~~~~-~,*@@$-;$==*;;~;$@@=@@" );
				Console.WriteLine( @"  :-~:::~~~:~:~~,.##@#= @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ =@$==$$*=#@@@@@@@@@@@@@@@@@@@####$$$==$$*!****!;;;;;;!!*!!!!!!!****=*==$$#$$@@@@@@@@@@@@#@@###@@=~:~~~~~~~~~~~~~~~~~~~~~~~-:@@@;-*==*!;::!#@=@@" );
				Console.WriteLine( @" ~;:::::~:::::~:~.$@@@@$=@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:==$$$=!*$#@@@@@@@@@@@@@@@@@#######$$=====!!**!!!;!;;;***!!!!!!!****==*=$$###@@@@@@@@@@@#@@@###@@::~~~~~~~~~~~~~~~~~~~~~~::~~@@@!~;=*!!;:~;$@=@@" );
				Console.WriteLine( @";;;;;;::~::::::~:-;@@@#@@=*@@@@@@@@@@@@@@@@@@@@@@@@@@@@*!:$#=*!=##@@@@@@@@@@@@@@@@#####@@@##$=====*!**!!;;;;;!*!!!;!!;!!****=**=###@@@@@@@@@@@@@@@@####@*~~~~~~~~~~~~~~~~~~~~~~~~:;::@@@*;:!$=!;**;*@!@@" );
				Console.WriteLine( @" ~;;~::~::~~::~::--@@@@##@@* @@@@@@@@@@@@@@@@@@@@@@@@@@@;!$*;*$#@#@@@@@@@@@@@@@@@####@@@@@##$$$$==****!;;;;;;!*!!!;;;;!!**====*$###@@@@@@@@@@@@@@@@##$@#~~~~~~~~~~~~~~~~~~~~~~~~~:;;!@@#=!;!*!*$@#!!@~@@" );
				Console.WriteLine( @" ~;;:::~:~:~~~~:~~.$@#@@@#@@$!*@@@@@@@@@@@@@@@@@@@@@@@@;!:;;=#@@@@@@@@@@@@@@@@@@###@@@@@@@###$$$=====*!;;;;;;!!!!!!;;!!!***===$$$#@@@@@@@@@@@@@@@@##@#@~--~~~~~~~~~~~--~~~~~~~~~~:!;=@@$==!**$@@@@;!$@@@" );
				Console.WriteLine( @"@;;::::~~~:~~~~~:-,;@##@@@#@@#$$*!@@@@@@@@@@@@@@@@@@@@@;:,*$#@@@@@@@@@@@@@@@@@@@@@@@@@###@###$$$======!;;;;;!!;;!**!!!******==$$$#@@@@@@@@@@@@@@@####@;----~~-~~~~~~~----~~~~~~~~:!=@@#$$=*=#@@##@!;!@@@" );
				Console.WriteLine( @" ;~:::::~~:~:~~~~-,,@@##@@@$#@@$##;@ @@@@@@@@@@@@@@@@@@ ~*@#@@@@@@@@@@@@@@@@@@@@@@@@@@@##@####$$$=====*!;;!!!!!!!**!!********===$#@@@@@@@@@@@@@@###@#*------~~~~~~~~----~~~~~~~-~:*@@##$$=$@@#@###=!;@@@" );
				Console.WriteLine( @"@;~:~~:::~~:~~~~---.;@@@$@@@##@@#=@#~@@@@@@@@@@@@@@@@@;;$@@@@@@@@@@@#@####@@@@@@@@@@@@@######$##$$=====*!!*!!!!!!*!!!!*******===##@@@@@@@@@@@@@#####=~~---~~~~::~~----~-~~~~~~~~-:#@@#$$#$$$@##@@@=*!:*@" );
				Console.WriteLine( @"@ ;;~~:~~~:~~::~~---.#@@@$@@@#$@@@$@@$;@@@@@@@@@@@@@@@@!#@@####@###@@#@@@@@@@@@@@@@@@@@#########$$$=====*!*!!****!!;!!!*******=$#@@@@@@@@@@@@@@####$-~~~~~::::::~~----~~~~~~~~~~,;@@@#$##$#=#@#@@@$*=~;@" );
				Console.WriteLine( @"@;;~~:::~:::~::::~~~--@@@@$@@@#$@@@=#@#=!@@@@@@@@@@@@@@;$$====$#@@@@@@@@@@@@@@@@@@@@###########$#$$==*====*!!****!!!!!!!!!!***=#@@@@@@@@@@@@@@@@###~~~~~~:::::::~~~~~~~~~~~~~~~~-~@@##$##$@$=@#@@@#== @@" );
				Console.WriteLine( @"@ ;;:~::~~~~~:::::::~.=@@@@$@@@@=@@@$$@@$;@@@@@@@@@@@@@;#$$###@@@@@@@@@@@@@@@@@@@@@@############$$=========******!!!!!*******=##@@@@@@@@@@@@@@@@@@!-~~~::::::::::::::::~~-~-~~~:--@@###$##@$=$@@###$= @@" );
				Console.WriteLine( @"@ ;~::~::::::::::::::~~@@@@@=@@@@=#@@#=@@@=:@@@@@@@@@@@*@@@@@@@@@@@@@@@@@@@@@@@@@@@##############$==$$$==$$==***!!******====$$###@@@@@@@@@@@@@@@#$-~~~~::::::::::::::;::~~~~-~~~--@@###@#@@@#*@@@@#$$@@@" );
				Console.WriteLine( @"@@ ~:~~:~::::::~::::::-*#@@@@$@@@@$@@@#=@@@@$;@@@@@@@@@==$@@@@@@@@@@@@@@@@@@@@@@@@@##############$$$$$$$$$$$===*!****=====$$=$$##@@@@@@@@@@@@@@@@;~::~:::::::::::::;;;;:~~~~~~~~-,#@##@##@@@@#$@@@#$$;@@" );
				Console.WriteLine( @"@@ *:~~:~~:::::~~:~--:::@#@@@@#@@@@$@@@@*#@@@$=@@@@@@@$!=$@@@@@@@@@@@@@@@@@@@@@@@@@@@@@###$$$##########$$$$$===**====$====$$$$$##@@@@@@@@@@@@@@@#~:::~::::::;;:::::;;:;::~:::~-~--#@@@@###@@@@=#@@@$$@@@" );
				Console.WriteLine( @"@@ ;;:::~~:~~~~~~:~~,,:~$@#@@@@#@@@@$@@@@=$@@@@!:@@@@@ !==##@##@@@@@@@@@@@@@@@@@@@@@@@@@####$$$#########$$$$===========$==$$$#$#@@@@@@@@@@@@@@@#=~:::~~~~:::;;::::::::;:::::;:~~--$@@@##@#@@@@#=@@@=*@@@" );
				Console.WriteLine( @"@@@;;::~:::~~~~~:::~~,~:;@@#@@@@#@@@@$@@@@$=@@@@$;@@@@!===$#######@@@@@@@@@@@@@@@@@@@@@####$$$#############$$===$$$=$$$$$$$$$$##@@@@@@@@@@@@@@@#~~~::~~~~:::;!::::;::;;;::::;::~--=@@@###@@@@@@##@@$!@@@" );
				Console.WriteLine( @"@@@ ~:::::~~:~::::~~~~-:~@@@#@@@@#@@@@#@@@@#=@@@@#!:!~**$####@@@@@@@@@@@@@@@@@@@@@@@@@#########@###########$=$$$$$$$$###$$$#####@@@@@@@@@@@@@@@$-~~:~~~~~~::!!:::~!!:;;:::::::::~-#@@@#@$@@@@@@@#@#=:@@@" );
				Console.WriteLine( @"@@@ ~;;;:::::::::~~~::~:~$@@#@@@@@#@@@@@@@@@@=@@@@@!~~!=$#@@@@@@@@@@@@@@@@@@@@@@@@@@@@######@#########$##$#$$$$$####$######@@###@@@@@@@@@@@@@@#:~~~;~~~:~~~:=!:::;;;:;;:::::::::~-@@@@#@$#@#@@@@@@#=@@@@" );
				Console.WriteLine( @"@@@ ~;:;::~::~~::~:~:::::*@@@#@@@@@#@@@@@@@@@@*@@@@@$$@$####@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@##############$$$$###########@@@@@@@@@@@@@@@@@@@@$-~~:;~~~::~~~$!:-*;:::!;:::::;;::~-@@@@@@$@@@@@@@@@#*@@@@" );
				Console.WriteLine( @"@@@@;:;:::~~::~:~:::::;;::@@@@#@@@@@#@@@@@@@@@@=#@@@@#!!=$####@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@####@####################@###@@@@@@@@@@@@@@@@@@@@!-~~:!~~:~:~~~$!~~:;;:;*::::::;;:~~-@@@#@#$@@@@@@@@@*;@@@@" );
				Console.WriteLine( @"@@@@;;::::::::~~~:::::;;:~#@@@@@@@@@@#@@@@@@@@@@=#@@@@@=#@@@@###@#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@###$###############@@@@@@@@@@@@@@@@@@@@@@@@$-~~:;!~~~::~~~#;-:::::*!:::::;;:::~~@@@@@##@#@@@@@@$!@@@@@" );
				Console.WriteLine( @"@@@@;~:~~:::::~~~~::::!!:~=@@@@@#@@@@@@@@@@@@@@@@=$@@@@@=$@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@##$$$##$$#######$$#@@@@@@@@@@@@@@@@@@@@@@#@:-~~;!!~:::;:~:$-::::::*!:::~~=!::~-~@@@@@#@@#@@@@##:~@@@@@" );
				Console.WriteLine( @"@@@@;~-~~:~;::::::::::!!;:!@@@@@@@@@@@@@@@@@@@@@@@#*@@@@@$!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@##$$$$##$$#########@@@@@@@@@@@@@@@@@@@@@@#!,-~:;*!~:::;::*~~:::::~=!::~~~$!:~~-;@@@@@$@@@@@@@=:~~-@@@@" );
				Console.WriteLine( @"@@@@ ~;;~::;;:::::::::*!;::@@@@@@@@@@@@@@@@@@@@@@@@#=@@@@@###@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@##$$$$###########@@@@@@@@@@@@@@@@@@@@@@@@=,-~~:;*;:::;;:;;~::;:::~=;:::~:=;::~-!@@@@@@@@@@@@*~~~::@@@@" );
				Console.WriteLine( @"@@@@ ;;;::;:;:;::;::::!!;:~$@@@@@@@@@@@@@@@@@@@@@@@@#=@@@@@#$$#@@@@@@###@@@@@@@@@@@@@@@@@@@@@@@#@@###$$$$#######@##@@@@@@@@@@@@@@@@@@@@@@#!,-~::;*=;~:;;;;;;~:;!;:::*;:::~;=;~~~-*@@@@@@@@@#=--~~~::@@@@" );
				Console.WriteLine( @"@@@@@;*:::;;;;;;;;;:;:!*;::=@@@@@@@@@@@@@@@@@@@@@@@@@@*@@@@@@####@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#######$#######@###@@@@@@@@@@@@@@@@@@@@!,,~~::;!==!~:::;;;;~;*!;:~:*;:::~!=:~~:-=@@@@@@@#@@=!~-~~~~@@@@" );
				Console.WriteLine( @"@@@@@;*::;:;;;;;;;:;::**;;:$@@@@@@@@@@@@@@@@@@@@@@@@@#@*#@@@@@@@##@#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#####$$########@#@###@@@@@@@@@@@@@@@@!--~~:;;;**$*~:::::;:;*!;;;~;*;:::~*=;~~~-=@@@@@@@@@@$=*:!*!;@@@@" );
				Console.WriteLine( @"@@@@@@*;;;::;;;;;:;;:;**;;:#@@@@@@@@@@@@@@@@@@@@@@@@@@@@=#@@@@@@@@#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@##############@@###@@@@@@@@@@@@@@@@@@!-~::;;!*=$@!~:::::;;*!;;;;:!!::::~*=::::-*@@@@@@@@@#$$=*~;;;@@@@" );
				Console.WriteLine( @"@@@@@;;;;;:!;;;;;;;;:;**!;;@@@@@##@@@@@@##@@@@@@@#@@@@@@@#@@@@@@@@@#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#########$###@@@##@@#@@@@@@@@@@@@@@#=-::;!!**#@@*~~::::;$*!;;;;:!!:;;:~==::::-*@@@@@@@@@##$==!~:;@@@@" );
				Console.WriteLine( @"@@@@@@;;!!**!*;:;!;;;!!*;;;*@#*=*=!*@@$!=**@**$#$*@@@@@@@*=@!#@!@@@:#===$*@@@@@@@@@@@@@@@@@@@@@@@@@############@#########@@@@@@@@@@@=!=#$;;!*$#@@@@*~~:::~*@*!;;;::!!;;:::==;:::~*@@@@@@@@@#$$$==!~:@@@@" );
				Console.WriteLine( @"@@@@@@@;!!*!*;!;!!:;;**!;!;;@#==*=*=@@$*===@##*#$**==$@@!#*@*#@;@@@:*#:##*@@@@@@@@@@@@@@@@@@@@@@@@@@@@#@@#################@@@@@@@@@!:!=$@@@@@@@####!~~~:::##!;;;;;:*!;::::==;:::~=@@@@@@@#@##$$$$=!:@@@@" );
				Console.WriteLine( @"@@@@@@@*;!*!=;;;;;;;;***;!!=*@!=***=##**!*=====#$#$$$$@@*@$#*#@***=;@#~@#;#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@##############@@@@@@@@@@@;!*=$$##@@@##$#@!-~~~~:@$!;:::::=!;;;;~==;:::-$@@@@@@@####$$$$$=!;@@@" );
				Console.WriteLine( @"@@@@@@ *!;**=;!!*!:::=***!*@##*#****@@#=*$###@@#$=$$$$@@=#=@*#@@===!@!$*#*@@@@@@@@@#@@@@@@@@@@@@@@@@@@@@@@@############@@@@@@@@@@#@$!*=$$###@#$$##@!~~~~~~@$;;::::;=!;;;:~#=;:::-#@@@@@#@####$$$$$$=;@@@" );
				Console.WriteLine( @"@@@@@@@*;;**=!*;!!::!$*!:~;@@@@=****@@$=$##@==$##=@@@@@@@=$@*#@@=@@;=$@$$=@@@@@@@@@####@@@@@@@@@@@@@@@@@@@@@###########@@@@@@@@@###@$==$$###$$$#@@@*~:::::@$;:::::!=;;::::@=;:::~@@@@@@#####$$$$$$$$=@@@" );
				Console.WriteLine( @"@@@@@@@;;;!*$:;;;!::;**!!;!*==$$=*=$@@$=$$#@@@@@@#####@@@@@@$@@@*=*!@@@@#$@@@@@@@@@@@@@@@@#@@#@@@@@@@@@@@@@@@########@@@@@@@@########$$##$$$$###@@@$~:~~:;@$;:::::*!;:::~;@$;::~~@@@#@####$$$$$$$$$$$;@@" );
				Console.WriteLine( @"@@@@@@@;~;;*#:;;:::::=*;;:;@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@###@@@@@@@@@@@@@@@@#####@@@@@@@###@####$$$##$$$$###@@@@#-~~~~;@=::::::=!::::~*@=;:~~~@@@#####$$$$$$$$$$$$#=@" );
				Console.WriteLine( @"@@@@@@@ ~:;*#~;;;::;:$*!;!;@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@###@@@@@@@@@@@@@@@@#####@@##@####@####$##$$##$$$######@@~~---*@=::::::$;::::~=@=:~~~~@@@#####$$$$$$$$$$$=#$@" );
				Console.WriteLine( @"@@@@@@@ ~:;=$~::;::::$*;!;!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#@@@@@@@@@@@@@@@@@#####$###@@###$$$$$##$###$$########@*~~--$@=:::;::$!::::~=@=:~~~~@@######$$$$$$$$$$$$=#@" );
				Console.WriteLine( @"@@@@@@@ ;;;=$::;;;;::#*!!;*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#######@@@##$$$$###$$##############@!~-;@@=:;;::;#;:::::=#=::~~:@##########$$$$$$$$$=@*" );
				Console.WriteLine( @"@@@@@@@@;;!=*:~;;;;;;#=*!!=@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@######@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#$$$$$$$#$$$###############$=@#@*:::::;@!:::::*@=~~--!#$##$$$####$$$$$$$$$=@*" );
				Console.WriteLine( @"@@@@@@@  ~-!*~;;;;;!*#$;,,-~~~~~~:*@@@@@@@@@@@@@@@@@@@@@@@@@@*      ;=@@@@@@@@@@@@@@@@@@@@@@@;~-~~~~~~~~~=@@@@@@@@@@~~~~~~~~~$########$!-~-----~~-;=######@*~::::;@*~:::~$:    .::-$$$$$$####$$$$$$$$=$=" );
				Console.WriteLine( @"@@@@@@@@   :*~~;;;;;!*=           -$@@@@@@@@@@@@@@@@@@@@@@@@@!       ~@@@@@@@@@@@@@@@@@@@@@@#. .         :@@@@@@@@@#        ,$######$$#~      .   -;$$####@!::;;:;@*~~~~:#.    .:!!*#$$$$$###$$$$$$$$$==" );
				Console.WriteLine( @"@@@@@@@@ ~-~=~:::;:;;!=           :=@@@@@@@@@@@@@@@@@@@@@@@@@-       .#@@@@@@#@@@@@@@@@@@@@@@-,:;:;;:!;;~;@@@@@@@@@$        .=#####$$$#,      ,   -;##$###@=:;;;;;@!~~~~~$     ~!~-~$$$$$$###$$$$$$$$$$=" );
				Console.WriteLine( @"@@@@@@@@  ; =~~::;:;;!*           ~$#@@@@@@@@@@@@@@@@@@@@@@@$     .-,~@@@@@@@@@@@@@@@@@@@@@@@!-;;~::-.,--,@@@@@@@@@*         *#####$$$$        ,  .;#######$:;::;!@!~::~:*     ,-, .=$$$$$$##$$$$$$$$##*" );
				Console.WriteLine( @"@@@@@@@@  ~ --:~:;:;!##            !#@@@@@@@@@@@@@@@@@@@@@@@!    .,::###@@@@@@@@#@@@@@@@@@@@@$$*!====-   ,#@@@@@@@@;         !########=      ..,,, ~#$#$$$##::::;*@;:~::;;          *$$$$$$$$$$$$$$$$#@=" );
				Console.WriteLine( @"@@@@@@@@    ,~~~;::!@##.           !@@@@@@@@@@@@@@@@@@@@@@@@~    .. .;@@@@@@@@@@@@@@@#@@@@@@@=~,.-!-;.   .$@@@@@@@@;.        ~#######$*      -.,:;:*$$$#$$$@!:~~~=@::~~:;-          ;#$$$$$$$$$$$$$$$#@*" );
				Console.WriteLine( @"@@@@@@@@@@  ,-:~:~:=@#@            !@@@@@@@@@@@@@@@@@@@@@@@@:     .. .*@@@@@@@@@@@@@@@@@@@@@@#$$$#$$~     =@@@@@@@@:;.       ,########;      -~:!!!$$####$$#$:::~#@~~~~~:           ~$$$$$$$$$$$$$$$$#@$" );
				Console.WriteLine( @"@@@@@@@@@    -:~~:~$@#@.           !@@@@@@@@@@@@@@@@@@@@@@@#.     ..,;@@@@@@@@@@@@@@@@@@@@@@@#;-:$=!*;-   *@@@@@@##.;~        $#######;      .~:*~;#####$$$##=:~*@@;~---~           .=$$$$$$$$$$$$$$$#@@" );
				Console.WriteLine( @"@@@@@@@@@    ,::::-#@@@            !@@@@@@@@@@@@@@@@@@@@@@@!   ~~-..,-*=*@@@@@@@@@@@@@@@@@@@@@:!*=**!-:.,.;@@@@@@@= .*,...    =###$$#@:       -::-:##########@##@@@#~~~--            !$$$$$$$$$$$$$$$#@@" );
				Console.WriteLine( @"@@@@@@@@@     ::~:~#@@#.           !@@@@@@@@@@@@@@@@@@@@@@@-           .~@@@@@@@@@@@@@@@@@@@@@=~,,-,,;-   -@@@@@@@!  ~~...    !$$#####-        .!**###########@@####$;~#:            ~$$$$$$$$$$$$$$##@#" );
				Console.WriteLine( @"@@@@@@@@@@    ~~~~~#@@#            !@@@@@@@@@@@@@@@@@@@@@@#             .#@@@@@@@@@@@@@@@@@@@@#;~-..,*; - .@@@@@@@:  .~: .    ;#######.    .,..,~$$#######$###########@$             :$$$$$$$$$$$$$$$#@=" );
				Console.WriteLine( @"@@@@@@@@@@    -~~~:@@@#            !@@@@@@@@@@@@@@@@@@@@@@$              *@@@@@@@@@@@@@@@@@@@@#;--.:!*, .  $@@@@@@;    ~-.    :#$$###=     ...  .;$####################;             .$$$$$$$$$$$$$$$#@;" );
				Console.WriteLine( @"@@@@@@@@@@     -~-;@@@#.           !@@@@@@@@@@@@@@@@@@@@@@=              :@@@@@@@@@@@@@@@@@@@@@=:!-::*-.   =@@@@@@~ .  .:.    ,######*            $#####$########$$$$$#,              *$$$$$$$$$$$$$$#$~" );
				Console.WriteLine( @"@@@@@@@@@@     ~--*@@@#.           !@@@@@@@@@@@@@@@@@@@@@@~              :@@@@@@@@@@@@@@@@@@@@@#==,..~     !@@@@@@.     ,~     $####@;           .$##$$$$########$$$$##.   .          -$$$$$$$$$$$$$$@*;" );
				Console.WriteLine( @"@@@@@@@@@@     ,~-@@@@#            !@@@@@@@@@@@@@@@@@@@@@#.              .@@@@@@@@@@@@@@@@@@@@@$*;,,.,..   ;@@@@@$       ..    =###@@~           ~##$$$$$$#######$$$$#=.-~.:          .$$$$$$$$$$$$$#@::" );
				Console.WriteLine( @"@@@@@@@@@@@ ~  :*#@@@@#.           !@@@@@@@@@@@@@@@@@@@@@=                =@@@@@@@@@@@@@@@@@@@@#=$~,-,     ;@@@@@!  ..    .,   !#@@@@~           ;#$$$$$$$$$$$###$$$##!.,,~,           =$$$$$$$$$$$$#;-;" );
				Console.WriteLine( @"@@@@@@@@@@@ ~  -@#@@@@#            !@@@@@@@@@@@@@@@@@@@@@;                ~@@@@@@@@@@@@@@@@@@@@##$=$=-     :@@@@@~   ,  .  ,-  :@@@@@;           ;$$$$$$$$$$$$###$$$##~~-,,,    .      !$$$$$$$$$$$$;:!=" );
				Console.WriteLine( @"@@@@@@@@@@@     =#@@@@#            !@@@@@@@@@@@@@@@@@@@@@!                -@@@@@@@@@@@@@@@@@@@@#$*~~:,~;:;:!@@@@@, -  ..,., :. ~@@@@@,           !$=====$$$$$$$###$$$=:~~:-... .       :$$$$$$$$$$$$;;;:" );
				Console.WriteLine( @"@@@@@@@@@@@ ;-~-;#@@@@#.           !@@@@@@@@@@@@@@@@@@@@@,                :#@@@@@@@@@@@@@@@@@@@#=;~~-..-.. .#@@@#,..,  .,.-..:,-@@@@$            *=======$$$$$$$#$$$$* .               ,$$$$$$$$$$$#;:!;" );
				Console.WriteLine( @"@@@@@@@@@@@@ @  ~@#@@@#.           !@@@@@@@@@@@@@@@@@@@@#~..,~-~,          =@@@@@@@@@@@@@@@@@@@@$$$#$. .    =@@@$., ..  ,.,...~-@@@@*            ****=====$$$$$$$$$$$;..       ,,       *$$$$$$$$$$#$*;~" );
				Console.WriteLine( @"@@@@@@@@@@@ ;~!,-@@@@@@            !@@@@@@@@@@@@@@@@@@@@;   ,,:-.          ;@@@@@@@@@@@@@@@@@@@@--...  ,    !@@@=.~--.  ..,,~,.-$@##:           .******====$$$$$$$$$$:                  ;$$$$$$$$$$$;*=*" );
				Console.WriteLine( @"@@@@@@@@@@@@  ,. $@@@@@            !@@@@@@@@@@@@@@@@@@@@;~-,,.~:,.  .      ,@@@@@@@@@@@@@@@@@@@@:,... ..    ~@@@*. ., ,...,,:- -=@@#-           -!!!!***===$$$$$$$$$$,  . ,             :$$$$$$$$$#=!;:*" );
				Console.WriteLine( @"@@@@@@@@@@@@     ;@@@@@            !@@@@@@@@@@@@@@@@@@@@~-~;~-:~-~ -.   ,::-#@@@@@@@@@@@@@@@@@@@!..,, .     ~@@@!..    .   .-:: ;@@#.          .:!!!!!***====$$$$$$$=,..                -$$$$$$$$$=!;*!!" );
				Console.WriteLine( @"@@@@@@@@@@@@ ;;~.:@@@@@            !@@@@@@@@@@@@@@@@@@@$.... ..        -!##=#@@@@@@@@@@@@@@@@@@@$-.,. .     :@@@-:      . .,::!~,@@#           .;;;;!!!**=====$$$$$$*:~. ..              =$$$$$$$=;:~~::" );
				Console.WriteLine( @"@@@@@@@@@@@@     @$@@@@            !@@@@@@@@@@@@@@@@@@@!,, ..    ,..~  ~:;$*$@@@@@@@@@@@@@@@@@@#$ .,. -     ,@@#~!-.       ,:;!*:@@$           -;;;;!!!***====$$$$$$!;~-,...             ;$$$$$==****!!;" );
				Console.WriteLine( @"@@@@@@@@@@@@   ,..*@@@@            !@@@@@@@@@@@@@@@@@@@-       .-.  .  :!;-:*@@@@@@@@@@@@@@@@@@@#..,.        #@#;-;~      ..~;;!!#@$,         -~;;;;;!!!**====$$$$$$~-,..     . .,,..    ~$=$$===*******" );
				Console.WriteLine( @"@@@@@@@@@@@@@  ,. ~@@@@            !@@@@@@@@@@@@@@@@@@@, .    ,,..,    ~;!=$$@@@@@@@@@@@@@@@@@@@@~..         =@=;~:;- ,   ..-;!!*#@=!.       .~:;;;!!!!!***===$$$$$=,.,,,,,,,,~~:-::~.   ,$======*!**!!*" );
				Console.WriteLine( @"@@@@@@@@@@@@@     ,$@@@            !@@@@@@@@@@@@@@@@@@$:,..         .--~!$$$##@@@@@@@@@@@@@@@@@@#;      .    ;@*~~,;~  .. , ,~;!!$@=;;       ~~:;;;!!!!****===$$$$$;.  .                  *======;;;;*=;" );
				Console.WriteLine( @"@@@@@@@@@@@@@  ,.  *@@@            !@@@@@@@@@@@@@@@@@@=:;:-.   .... .~----:;;*@@@@@@@@@@@@@@@@@@@$.          -@*:,,~;:.      :::**@*!;,      ~.:;;;;!!*****===$$$$$,.                     ;==****!;!***:" );
				Console.WriteLine( @"@@@@@@@@@@@@@@     -@@@            !@@@@@@@@@@@@@@@@@@!!;;:-,,,,,            :@@@@@@@@@@@@@@@@@@@$. .,       .@~~~.,~;-  ,   ~;;!*#;:.      .~,:;;;;;!*!***====$$$=.                      ,=**;;!!;;!;:;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@     ;@@@            !@@@@@@@@@@@@@@@@@#:;;!:~~,,-.,~  ..      -@@@@@@@@@@@@@@@@@@@@..-         #.,.  .~-- - -~;!;;;#!:       ,-~;;;;;;!!!!**=====$$*.          -.           **!;;;!!;!:;=" );
				Console.WriteLine( @"@@@@@@@@@@@@@@      $@@.           !@@@@@@@@@@@@@@@@@#!:...,.---,:=:;;;;:-   ,#@@@@@@@@@@@@@@@@@@@.           =  ,  ~:,-. -.-**!;!=~.       -,-;;;;;;!!!**====$$$$;~,         !-           ;!;;;!!:::~:~" );
				Console.WriteLine( @"@@@@@@@@@@@@@@ ~!;;;*@@;~,         !@@@@@@@@@@@@@@@@@*;:~...,. ..!=,    .     ~@@@@@@@@@@@@@@@@@@@-           :   .  :- :;-~!****-~         ~;;;;;;;;;!!**===$$$$$-:.         =~           ~!!!!!*;::;:;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@  -,-,:@@~~.         !@@@@@@@@@@@@@@@@@;           =$.         .:@@@@@@@@@@@@@@@@@@@; -:        .    ;,.:.~:!~~:**!.          ::;;;;;;;;!!**====$$$=.          ,#!           -!!!!;;:::!;;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@      -#@.           !@@@@@@@@@@@@@@@@#.          .$#,         -;@@@@@@@@@@@@@@@@@@@*                ,!:~, .!!~;!**.          :::;;;;;;;!!**====$$$*,.         ~#$.           !!*!*=*!;;::" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@      =@.           !@@@@@@@@@@@@@@@@*           :##-         ;*$@@@@@@@@@@@@@@@@@@#.                :;~-,  !;-~-            .~;;;;;;;;!!*====$$$$;-:         :$#~           :!*!!*!!!;;:" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@      ~@            !@@@@@@@@@@@@@@@@~           !##!         ~!=@@@@@@@@@@@@@@@@@@@,                ~!;:;--!*.            . .-;;;;;;!!!**====$$$$:::::,      !$$;           ~;;;;!*;;~:;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@      ,#       ,- -:=@@@@@@@@@@@@@@@@~           *##$          :!@@@@@@@@@@@@@@@@@@@~                 ~;;:;*:-            ..,.-;;;;;;!!!**====$$$=,.. .      .=$$;      ..,,,~!!;!!=**;;;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@       =.     .,~~-,*@@@@@@@@@@@@@@@@,           $###:          !#@@@@@@@@@@@@@@@@@@:                  ,.--,              .,.,:!!;;;;!!!**====$$$*,..        ~$$$*      .,,..,;!*!;;:!!!:" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@       ;.    ..  ..~=@@@@@@@@@@@@@@@=           .####:         ,;$@@@@@@@@@@@@@@@@@@;                                     ,,-,;!!!!!!!!!*=====$$$-  .        ;$$$$.           :!*=!;;;*:~" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@       ..       . .;$@@@@@@@@@@@@@@@~           :####:        .*!!@@@@@@@@@@@@@@@@@@*                                     -~~-;**!!!!!!***===$$=$,-.. ,,,.,. !$$$$:           -!!=!!;*!;;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@              .,~!;$@@@@@@@@@@@@@@@.           =####*        .:=!@@@@@@@@@@@@@@@@@@$                                     -:::**!!;!!!!**===$$$==~::~::-::::.=$$$$!           ,!!***!;;!!" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@          ,....,,-;$@@@@@@@@@@@@@@$           -#####$.        -$$@@@@@@@@@@@@@@@@@@@::;:;~:--                            ,~::*!!!;;;;!**===$$$$!..      ...,$$$$$*           .!**$=*::::" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@                   !@@@@@@@@@@@@@@*           ,=$$##@:       ,!$=$@@@@@@@@@@@@@@@@@@~,, .,-~,           -                -~~;!!!!;;;!!!*===$$$$-.          ~$$$=$=            ;**=$=***;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@                   !@@@@@@@@@@@@@@!           -=!;!~:-       ;*;:;@@@@@@@@@@@@@@@@@@!                   *                 ,,;!;!**!!!****==$$$=~~.,..,,    !$$==$$,           ~!!!***;!;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@                   !@@@@@@@@@@@@@@,           ;*;;;;;:      .*~  :@@@@@@@@@@@@@@@@@@*                  -#-                .,;!!!**!!***=====$$=!~,         =$$==$$:           .!!!!*!;::" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@                  !@@@@@@@@@@@@@$            *****=**      ,*,-~,#@@@@@@@@@@@@@@@@@=                  ;@;                -~!!!!**!!****====$$=!~-,       .$$$$$$$*            !;!;==*!;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@                  !@@@@@@@@@@@@@!           ,;;!!***=.     -*=**!*@@@@@@@@@@@@@@@@@#                  ;@,               ..-!***!!!!!***====$$*:.,.       ,$$$$$$$=            :!!!;:~~~" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@                  !@@@@@@@@@@@@@:          .-;~--;;!!,     .-:~~~~@@@@@@@@@@@@@@@@@@.                 ;@:               ..,****!!!!!****====$!::-        ,;;;;;;;:            ~**;;::~!" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@                  !#@@@@@@@@@@@@-               -:         ,;;:;-.#@@@@@@@@@@@@@@@@@-                 *@*               ..-****!!!!!**===$==*-:,,                             ,!!:!;*;:" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@                  !#@@@@@@@@@@@$                -;         ~!!*!~-$@@@@@@@@@@@@@@@@@;                 $@$             .. ,:*!!!!!****===$$$=*:;~,                              ;!*!!;~~" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@                  !#@@@@@@@@@@@;            ....,~         !****!.!@@@@@@@@@@@@@@@@@=                ,@@@-           .,,,.;***!!!**===$$$$$=*:~-.....                          ;*=**!:!" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@                 !#####@@@@@@@-        .. ...,,::         -!*!!!~-@@@@@@@@@@@@@@@@@#.               :@@@:                !***!**==$$$$$$$==;;:~,.                             ~*!;:*!!" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@                 ;####@@@@@@@@~                ,,          :!;;;:~#@@@@@@@@@@@@@@@@@-              .*@@@:               .******====$$$$$$==~:,,..                             -=**$===" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@                 ;###$##@@@@@$.                            ,!;!;:;$@@@@@@@@@@@@@@@#@~     ~ ~~,..::!#@@@;               ,******===$$$$$$$$!~~--.                              .=$$$$$=" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@                 ;$$==#$#@@@@*                             ~;!;!!;=@@@@@@@@@@@@@@@@@-     ,---,  .-;#@@@*               -!!***====$$$$$$$$:                                    !$$****" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@                 ;=====$##@@@-                            ,;;~~**!*@@@@@@@@@@@@@@@@@:   ,,-:~,,.-~:,#@@@$               ~!!***=====$$=$$$$,.                                   -!:~;;;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@       ..,,,,...!$$==*=$##@#                             .~;~::::;#@@@@@@@@@@@@@@@#*,,.,,~:;;;!:- ~@@@@@,             .:!!!!**====$==$$$=-~.                                 .,==**!;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@      :;;;!~~   :*===#$#$##=                               ~;;:~  *@@@@@@@@@@@@@@###!;;;-  ~~.-,-.*@@@@@:         .:~.;!!!;!**====$==$$$!:-.                                 .-;:;*:;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@     :!;-~-  .  -;!***===##!                              .-:;;::-;@@@@@@@@@@@@@####-~,-,    -. .,*@@@#@!    ,, .~-:;:~!!;;!!**=====$$$$:~,                                   ,:;::::" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@    -*!~-,-~,, .-:;!!*====$~                                ..... :@##@#@@@@@#@####@:- .,,    .  .#@@@@#*   .,-,,,~...~!!;!!!**===$$$$$$:-..                                  ,;;:~::" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@    !*,  .  .-,-;***=**====,.                                     .#####@@@@@###$##@!   ...      -@@@@@#=        --,.,~!;;!!!!**==$$$$$* . .                                   ~:;!!!" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@    ,- ,. .   ,-:!*!!!!!==*                                        *#$####@@####$$$$;  .  ,... ..-@@@@##$    .   -:~~!!;!!!!!!=*===$$$#! .  . ,,                           .    :!**=" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@                -;;;;*!!;!~                                        -====$$$$$###$$=$!   .  .  ,-~;@@@@@##,   ..::~~-~~;;!!!!*===$$$$$$#~  .. .                             .    ,-~::" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@              . ~:!*!*=!;!,.                                       .;**!***!*$=#$$===   ..,.,.~;;=@@@@@##:,,...,,--   ;!!!**==$$$$$$$$=. ..  .                                  .-~::" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@             . ~;!;:!***!.-           !;::;=!;!*!**=$*             ;;*=!!!**$*$$====.   ,-,-~:;!$@@@@@##!       ..   !!!****=$$$$###$*             ::::!*!;;!*!***;       ,... .-~~:" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@               ~:;!!*!;;;            -*::;!;:;!!*$#=#*.            ;:;;::;!!$=*$===$~.     . ,,~#@@@@@##=.          .!!!*****=$#####$: ..         -*=#$$$$$==#$$==#~      ,-    ,,~~" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@               ~!**==**!~            ~!:!;:::;;;:!=**=:            ~!;!$$=;!==;====$!;:,-~-~~--~@@@@@@@##-          -!!*=****=$####$=-,.,,        ;$*==$$=$=*==$=*=~      -~.   ..~~" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@               :**==*==*,            ~:!;*==***!;:!!!;:            .*!*===!**=*!$==$!~.     .  ~@@@@@@@#@~          :***=====$$$##$=!.  ..  ....,,**!!*****!;!!!!:;-      ~-     .~~" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@         .  ...~!!**$==*             =#=$#####$*==*!*==,            *=$===***==;!$=$*:~        ;@@@@@@@@@-  -- -~.  !**======$$$$$==:.,--,, ,,-..,:;;!*;;:;;!*!:;;*;      -~.,. .,::" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@          ...,.:!*=====!            .$#####@=$=*!!!!=$=,            :;*$#$!!**$=;*$$$~         ;@@@@@@@@@; .-:~,.- .========$$$$#$$$:            -;;;!;!;:;;;!:!;:;:      ...,,  ,~;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@        .,-~-,;=$$$$==:            ~=#=##$##$**!!!***;,            ~$:=$==***=$*;$$=~.        *@@@@@@@@@*..   .~- ,=====$$$$$$#=!*=-            :===!!!==*=**!*=*;!,      .~.,..,:;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@     , ,~,.,~~-;===$=**.            !=!;!!!!!!*=!!!!;:;:            .=$!**$***!=$!!==~,..      $@@@@@@@@@#  .  .;: -=====$$$$$$#*==*             ::;;!*:::;!!*;:~:;!~       ..,.,,;;" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@  ~,,-...,--.;*=====*     .. .    ==:~::;;;;;:::;!*!!!.            :-:~~;$****$=!!$====~    .@@@@@@####@, ,.~::- ~=$$$$$$$$$#$###=            .;::!;:;;!;**!:!;**!;        ,,-,~::" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@  ~::-~. . ,.;$$#==$*~;;!!;;!~-~ .==;;;:::*!---~;:-~:=~   -, ;~    ~==!;!#*****==;**~..     ~@@@@@@@####:,,,,-;- !$$$$$$$$$#=;!;=~            ~;:~::;$==;~;!===*!:;        ,:~-;;*" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@ ;!;::~~-,,,*#$#==#;.,.., ..,,,.~$=*$$!!=$=;;::!::;;;~  ,-~--~-   ,!::!:;#****=#=;*        !@@@#####$$$:     .  =$$$$$$$$$#$$$=!.            ;:;:;=:!!***=$***=$*$~        ;~~;=*" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@ -,..,,.    !##$*==-            *=**:!**:*!-~-~~:;!::: ....,-;-   .;;!*;:*$!***=#*;.       *######$$$$$;       .$$$$$$$$$#=:;!!=             =#!=*;::;;;!;*:~!*===!        .:~-;*" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@            ;=$$===.           -*****$$;;!~~:::*!;;**!. ,,  ,::   -**!!!=:$=!***$#!.       *$###$$$$=$$=       :#$$$$$$$##!::;;:            .*=#*:::~~!;:;!*;;!;!!!            .=" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@;;::;;:~:;!$@!=$$#*::;:~-:;;;:~=***!=*;=!*;*;;!!!:=;=:~:::=**;;;::!;!!;!=*$=!*=*$$~~-~~:~~*$$#$*=$$=***~~:;;;:!##$$#$##@!!!;*!:,.--.,,-,,-~;;;:!!;;*$#$;:*=!!!!!;;-----:-,-~~--:" );
				Console.WriteLine( @"@@@@@@@@@@@@@@@@@@@@@@@@*=$##$=$##@##;=$#$#@##=*=@@@=******=;;*!;=$!==*!=$!$$$#$*****$**!!!!!*=*!!!$=!***@*!!**=**=@=$$=$==****=*!#!:$$;*#=;$#;;!!*!*;~;!:=;::*:!*;!:~;!:!;:*!!=;;;!!!;;!;!!;!:!!=!!!!**" );

                #endregion

                Console.WriteLine( "이스터에그는 아무키나 누르시면 나가집니다." );

                Console.SetCursorPosition( 0, 0 );

				Console.ReadKey();

				Console.BackgroundColor = BACKGROUND_COLOR;
				Console.ForegroundColor = FOREGROUND_COLOR;
				Console.Clear();
                Console.SetCursorPosition( 0, 0 );
                Console.CursorVisible = false;

				isSkipRender = false;
			}
            #endregion

			void Update( ConsoleKey inputKey )
			{
				#region Update
				// ------------------------------------------------------------------ Update.. ------------------------------------------------------------------
				// ================================= Player Update.. =================================
				UpdatePlayer( ref player, inputKey );

				// ================================= Item Update.. =================================
				#region Item Update

				for ( int index = 0; index < activeItemCount; ++index )
				{
					int itemIndex = playerActiveItemIndex[index];

					Item.Type curItemType = items[itemIndex].type;

					if ( Item.Type.END == curItemType )
					{
						continue;
					}

					switch( curItemType )
					{
						case Item.Type.ReverseMove:
							// 현재 프레임에 플레이어가 움직인 거리 계산..
							int moveDirX = player.X - player.PrevX;
							int moveDirY = player.Y - player.PrevY;

							if( 0 != moveDirX || moveDirY != 0 )	// 움직임이 있는 경우에만 실행..
							{
								// 그 반대 방향으로 움직이게 함..
								player.X = player.PrevX - moveDirX;
								player.Y = player.PrevY - moveDirY;
							}
							else
							{
								++items[itemIndex].Duration;
							}
							break;

						case Item.Type.EasterEgg:
							교수님죄송합니다();
							break;

						case Item.Type.HPPosion:
							player.CurHp = Math.Min( player.MaxHp, player.CurHp + items[itemIndex].Effect );
							break;

						case Item.Type.MPPosion:
							player.CurMp = Math.Min( player.MaxMp, player.CurMp + items[itemIndex].Effect );
							break;
					}

					--items[itemIndex].Duration;

					if ( 0 == items[itemIndex].Duration )
					{
						items[itemIndex].type = Item.Type.END;
					}

					isSkipRender = false;
				}

				#endregion

				#region Box Update
				// ================================= Box Update.. =================================
				// 박스 업데이트..
				for ( int i = 0; i < BOX_COUNT; ++i )
				{
					// 박스 이전위치 갱신..
					boxes[i].PrevX = boxes[i].X;
					boxes[i].PrevY = boxes[i].Y;

					switch ( boxes[i].CurState )
					{
						case Box.State.Idle:
							if ( IsCollision( player.X, player.Y, boxes[i].X, boxes[i].Y ) )   // 플레이어와 박스가 같을 때..
							{
								// 박스가 이동할 위치를 계산( 현재위치 - 이전위치 = 이동할 방향 )..
								int boxMoveDirX = player.X - player.PrevX;
								int boxMoveDirY = player.Y - player.PrevY;

								// 박스 현재위치 갱신.. 
								boxes[i].X += boxMoveDirX;
								boxes[i].Y += boxMoveDirY;

								break;
							}

							break;
						case Box.State.Move:
							boxes[i].X -= boxes[i].DirX;
							boxes[i].Y -= boxes[i].DirY;

							isSkipRender = false;

							break;

						case Box.State.GrabByPlayer:
						{
							boxes[i].PrevX = boxes[i].X;
							boxes[i].PrevY = boxes[i].Y;

							// 박스가 이동할 위치를 계산( 현재위치 - 이전위치 = 이동할 방향 )..
							int boxMoveDirX = player.X - player.PrevX;
							int boxMoveDirY = player.Y - player.PrevY;

							// 박스 현재위치 갱신.. 
							boxes[i].X += boxMoveDirX;
							boxes[i].Y += boxMoveDirY;
						}

						break;
					}
				}
                #endregion

                #region Trap Update

                for ( int trapIndex = 0; trapIndex < TRAP_COUNT; ++trapIndex )
                {
                    if ( traps[trapIndex].IsBurst )
                    {
                        isSkipRender = false;
                        isConsoleClear = true;

						switch(traps[trapIndex].MyType)
						{
							case Trap.TrapType.Bomb:
							{
                                BombTrap curTrap = (BombTrap)traps[trapIndex];

                                if ( curTrap.curBurstRange >= curTrap.BurstRange )
                                {
                                    curTrap.IsActive = false;
                                    curTrap.IsBurst = false;

									continue;
                                }

                                // 플레이어에게 아직 데미지를 주지 못했다면 검사..
                                if ( false == curTrap.IsPlayerHit )
                                {
                                    if ( IsInRange( player.X, player.Y, traps[trapIndex].X, traps[trapIndex].Y, curTrap.curBurstRange ) )
                                    {
                                        player.CurHp -= curTrap.Damage;
                                        curTrap.IsPlayerHit = true;
                                    }
                                }

                                ++curTrap.curBurstRange;
                            }

                                break;

							case Trap.TrapType.Trigger:
							{
                                TriggerTrap curTrap = (TriggerTrap)traps[trapIndex];

                                curTrap.IsActive = false;
                                curTrap.IsBurst = false;

                                for( int spawnIndex = 0; spawnIndex < curTrap.SpawnObjectCount; ++spawnIndex )
								{
									int x = curTrap.SpawnObjectsX[spawnIndex];
									int y = curTrap.SpawnObjectsY[spawnIndex];
									int dirX = curTrap.SpawnObjectsDirX[spawnIndex];
									int dirY = curTrap.SpawnObjectsDirY[spawnIndex];

									Arrow arrow = new Arrow
									{
										X = x, Y = y, PrevX = x, PrevY = y, DirX = dirX, DirY = dirY,
										Image = initArrowImage, Color = ARROW_COLOR, IsActive = true, Damage = 5
									};

									arrow.ComputeImage();

                                    arrows.Add( arrow );
								}
                            }

                                break;
						}
                    }
                }

				#endregion

				foreach ( var arrow in arrows )
				{
					arrow.PrevX = arrow.X;
					arrow.PrevY = arrow.Y;

                    arrow.X += arrow.DirX;
					arrow.Y += arrow.DirY;

					isSkipRender = false;
                }

                // ================================= Collision Update.. =================================
                #region Box Collision
                // 박스가 특정 물체와 겹쳤다( 박스 or 벽 or 맵 외곽 )..
                for ( int i = 0; i < BOX_COUNT; ++i )
				{
                    MapSpaceType curStandSpaceType = mapDatas[boxes[i].Y, boxes[i].X];

					// 박스가 현재 위치에 다른 물체가 있을 때는 이전 위치로 이동..
					switch ( curStandSpaceType )
					{
						case MapSpaceType.DontPass:
							boxes[i].X = boxes[i].PrevX;
							boxes[i].Y = boxes[i].PrevY;

							boxes[i].CurState = Box.State.Idle;

							break;
						case MapSpaceType.BoxStand:
							for ( int curBoxIdx = 0; curBoxIdx < BOX_COUNT; ++curBoxIdx )
							{
								if ( curBoxIdx == i )
									continue;
								if ( boxes[i].X != boxes[curBoxIdx].X || boxes[i].Y != boxes[curBoxIdx].Y )
									continue;

								boxes[i].X = boxes[i].PrevX;
								boxes[i].Y = boxes[i].PrevY;

								boxes[i].CurState = Box.State.Idle;

								break;
							}

							break;
						case MapSpaceType.PlayerStand:
							if ( Box.State.GrabByPlayer != boxes[i].CurState )
							{
								boxes[i].X = boxes[i].PrevX;
								boxes[i].Y = boxes[i].PrevY;

								boxes[i].CurState = Box.State.Idle;
							}

							break;
						case MapSpaceType.Portal:
							// 포탈 다른 게이트로 이동..
							PushPortal( portals, ref boxes[i].X, ref boxes[i].Y, boxes[i].PrevX, boxes[i].PrevY );

							// 현재 플레이어에게 잡혀있는 상태라면 기본 상태로 변경..
							if ( Box.State.GrabByPlayer == boxes[i].CurState )
								boxes[i].CurState = Box.State.Idle;

							i--;    // 이동한 지점에 다른 오브젝트가 있는지 검사하려고..

							break; 
						case MapSpaceType.Trap:
                            for ( int trapIndex = 0; trapIndex < TRAP_COUNT; ++trapIndex )
                            {
                                if ( traps[trapIndex].IsDestroy )
                                {
                                    continue;
                                }

                                if ( IsCollision( boxes[i].X, boxes[i].Y, traps[trapIndex].X, traps[trapIndex].Y ) )
                                {
                                    traps[trapIndex].IsBurst = true;
                                    traps[trapIndex].IsActive = true;
                                    traps[trapIndex].IsDestroy = true;

                                    mapDatas[traps[trapIndex].Y, traps[trapIndex].X] = MapSpaceType.Pass;

                                    break;
                                }
                            }

							break;
						case MapSpaceType.Arrow:
							boxes[i].CurState = Box.State.Idle;

                            break;
					}
				}
				#endregion

				#region Player Collision
				// 플레이어가 특정 물체와 겹쳤다( 박스 or 벽 등등 )..
				MapSpaceType overlapSpaceType = mapDatas[player.Y, player.X];	// 여기서 받아옵니다..

				// 여기서 검사..
				switch ( overlapSpaceType )
				{
					case MapSpaceType.DontPass:
						player.X = player.PrevX;
						player.Y = player.PrevY;

						break;
					case MapSpaceType.BoxStand:
						for ( int boxIdx = 0; boxIdx < BOX_COUNT; ++boxIdx )
						{
							int boxX = boxes[boxIdx].X;
							int boxY = boxes[boxIdx].Y;

							// 만약 플레이어와 박스의 위치가 같을 때 이전 위치로..
							// 맵 데이터가 갱신이 안되있는 상태기 때문에 이 검사를 하는 것..
							if ( player.X == boxX && player.Y == boxY )
							{
								player.X = player.PrevX;
								player.Y = player.PrevY;
							}
						}

						break;
					case MapSpaceType.Portal:
						PushPortal( portals, ref player.X, ref player.Y, player.PrevX, player.PrevY );

						break;

					case MapSpaceType.Item:
						for ( int itemIndex = 0; itemIndex < ITEM_COUNT; ++itemIndex )
						{
							if( IsCollision( player.X, player.Y, items[itemIndex].X, items[itemIndex].Y ))
							{
								playerActiveItemIndex[activeItemCount] = itemIndex;
								++activeItemCount;
								// 밟은 아이템은 필드에서 제거( 안보이게 함 )..
								items[itemIndex].isActive = false;

								break;
							}
						}
						break;

					case MapSpaceType.Trap:
						for ( int trapIndex = 0; trapIndex < TRAP_COUNT; ++trapIndex )
						{
							if ( traps[trapIndex].IsDestroy )
							{
								continue;
							}

							if ( IsCollision( player.X, player.Y, traps[trapIndex].X, traps[trapIndex].Y ) )
							{
								traps[trapIndex].IsBurst = true;
								traps[trapIndex].IsActive = true;
								traps[trapIndex].IsDestroy = true;

                                mapDatas[traps[trapIndex].Y, traps[trapIndex].X] = MapSpaceType.Pass;

								break;
							}
						}

						break;
				}
                #endregion

				#region Switch Collision
				for ( int switchIdx = 0; switchIdx < SWITCH_COUNT; ++switchIdx )
				{
					int switchButtonX = switches[switchIdx].X + switches[switchIdx].ButtonOffsetX;
					int switchButtonY = switches[switchIdx].Y + switches[switchIdx].ButtonOffsetY;

					MapSpaceType curPushPosSpaceType = mapDatas[switchButtonY, switchButtonX];
					if ( MapSpaceType.Pass != curPushPosSpaceType )
					{
						if ( false == switches[switchIdx].IsHolding )
						{
							switches[switchIdx].IsHolding = true;

							int loopCount = switches[switchIdx].OpenCloseWallIndex.Length;
							for ( int loopIndex = 0; loopIndex < loopCount; ++loopIndex )
							{
								int wallIndex = switches[switchIdx].OpenCloseWallIndex[loopIndex];

								walls[wallIndex].IsActive = false;
								walls[wallIndex].IsRender = false;
                            }

							isSkipRender = false;
						}
					}
					else
					{
						if ( true == switches[switchIdx].IsHolding )
						{
							switches[switchIdx].IsHolding = false;

							int loopCount = switches[switchIdx].OpenCloseWallIndex.Length;
							for ( int loopIndex = 0; loopIndex < loopCount; ++loopIndex )
							{
								int wallIndex = switches[switchIdx].OpenCloseWallIndex[loopIndex];

								walls[wallIndex].IsActive = true;
                                walls[wallIndex].IsRender = true;
                            }

							isSkipRender = false;
						}
					}
				}
                #endregion

				for( int arrowIndex = 0; arrowIndex < arrows.Count; )
				{
                    MapSpaceType curSpaceType = mapDatas[arrows[arrowIndex].Y, arrows[arrowIndex].X];

                    switch ( curSpaceType )
                    {
                        case MapSpaceType.PlayerStand:
                            player.CurHp -= arrows[arrowIndex].Damage;
							removeArrows.Add( arrows[arrowIndex] );
                            arrows.Remove( arrows[arrowIndex] );

                            break;

                        case MapSpaceType.DontPass:
                            removeArrows.Add( arrows[arrowIndex] );
                            arrows.Remove( arrows[arrowIndex] );
							break;

                        case MapSpaceType.BoxStand:
                            removeArrows.Add( arrows[arrowIndex] );
                            arrows.Remove( arrows[arrowIndex] );

                            break;

						default:
							++arrowIndex;

                            break;
                    }
                }

				foreach ( var arrow in removeArrows )
				{
					mapDatas[arrow.PrevY, arrow.PrevX] = MapSpaceType.Pass;
				}

                // =========================================== Map Update.. =========================================== //
                #region Map Update
                // 플레이어 정보 갱신..
                mapDatas[player.PrevY, player.PrevX] = MapSpaceType.Pass;
                mapDatas[player.Y, player.X] = MapSpaceType.PlayerStand;

                // Box 정보 갱신..
                for ( int i = 0; i < BOX_COUNT; ++i )
				{
					mapDatas[boxes[i].PrevY, boxes[i].PrevX] = MapSpaceType.Pass;
					mapDatas[boxes[i].Y, boxes[i].X] = MapSpaceType.BoxStand;
				}

				// Portal 정보 갱신..
				for ( int portalIdx = 0; portalIdx < PORTAL_COUNT; ++portalIdx )
				{
					for ( int gateIndex = 0; gateIndex < PORTAL_GATE_COUNT; ++gateIndex )
					{
						int curPortalX = portals[portalIdx].GatesX[gateIndex];
						int curPortalY = portals[portalIdx].GatesY[gateIndex];
						mapDatas[curPortalY, curPortalX] = MapSpaceType.Portal;
					}
				}

				// 벽 정보 갱신..
				for ( int wallIdx = 0; wallIdx < WALL_COUNT; ++wallIdx )
				{
					int wallX = walls[wallIdx].X;
					int wallY = walls[wallIdx].Y;

					if ( true == walls[wallIdx].IsActive )
					{
						mapDatas[wallY, wallX] = MapSpaceType.DontPass;
					}
					else
					{
						mapDatas[wallY, wallX] = MapSpaceType.Pass;
					}
				}

				// Switch 정보 갱신..
				for ( int switchIndex = 0; switchIndex < SWITCH_COUNT; ++switchIndex )
				{
					mapDatas[switches[switchIndex].Y, switches[switchIndex].X] = MapSpaceType.DontPass;
				}

				// Item 정보 갱신..
				for( int itemIndex = 0; itemIndex < ITEM_COUNT; ++itemIndex )
				{
					if( true == items[itemIndex].isActive )
					{
						mapDatas[items[itemIndex].Y, items[itemIndex].X] = MapSpaceType.Item;
					}
					else
					{
						mapDatas[items[itemIndex].Y, items[itemIndex].X] = MapSpaceType.Pass;
					}
				}

				// Arrow 정보 갱신..
				for ( int arrowIndex = 0; arrowIndex < arrows.Count; ++arrowIndex )
				{
					mapDatas[arrows[arrowIndex].PrevY, arrows[arrowIndex].PrevX] = MapSpaceType.Pass;
					mapDatas[arrows[arrowIndex].Y, arrows[arrowIndex].X] = MapSpaceType.Arrow;
				}

                // Trap 정보 갱신..
                for ( int trapIndex = 0; trapIndex < TRAP_COUNT; ++trapIndex )
                {
					if ( traps[trapIndex].IsDestroy )
					{
                        mapDatas[traps[trapIndex].Y, traps[trapIndex].X] = MapSpaceType.Pass;
                    }
					else
					{
                        mapDatas[traps[trapIndex].Y, traps[trapIndex].X] = MapSpaceType.Trap;
                    }
                }

                #endregion

                #endregion
            }

			void UpdatePlayer( ref Player player, ConsoleKey inputKey )
			{
				// 플레이어 이전위치 갱신..
				player.PrevX = player.X;
				player.PrevY = player.Y;

				int moveX = 0;
				int moveY = 0;

				switch ( inputKey )
				{
					case ConsoleKey.RightArrow:
					case ConsoleKey.LeftArrow:
						moveX += (int)inputKey - 38;
						player.actionKind = Player.ActionKind.Move;

						break;
					case ConsoleKey.DownArrow:
					case ConsoleKey.UpArrow:
						moveY += (int)inputKey - 39;
						player.actionKind = Player.ActionKind.Move;

						break;
					case ConsoleKey.Spacebar:
						player.actionKind = Player.ActionKind.Grab;

						break;
					case ConsoleKey.A:
						player.actionKind = Player.ActionKind.Kick;

						break;

					default:
						return;
				}

				// 현재 MP가 행동 시 필요한 MP 보다 부족하다면..
				if ( player.CurMp < Player.ACTION_USE_MP[(int)player.actionKind] )
				{
					playerErrorLog.AppendLine( "마나가 부족합니다." );
					return;
				}

				// 행동에 필요한 MP 만큼 빼주기..
				player.CurMp -= Player.ACTION_USE_MP[(int)player.actionKind];

				// 행동 종류에 따라 해야할 행동 진행..
				switch (player.actionKind)
				{
					case Player.ActionKind.Move:
						player.X += moveX;
						player.Y += moveY;
						++curPlayerMoveCount;

						break;
					case Player.ActionKind.Grab:
						for ( int i = 0; i < BOX_COUNT; ++i )
						{
							int boxX = boxes[i].X;
							int boxY = boxes[i].Y;
							Box.State boxState = boxes[i].CurState;

							int xDist = Math.Abs( player.X - boxX );
							int yDist = Math.Abs( player.Y - boxY );

							if ( 1 == xDist + yDist )
							{
								if ( Box.State.GrabByPlayer == boxState )
									boxState = Box.State.Idle;
								else
									boxState = Box.State.GrabByPlayer;

								boxes[i].CurState = boxState;
							}
						}

						break;
					case Player.ActionKind.Kick:
						for ( int i = 0; i < BOX_COUNT; ++i )
						{
							int boxX = boxes[i].X;
							int boxY = boxes[i].Y;

							int xDist = Math.Abs( player.X - boxX );
							int yDist = Math.Abs( player.Y - boxY );

							if ( 1 == xDist + yDist )
							{
								int curBoxIndex = i;

								// 박스 상태 변경 및 그와 관련된 값 설정..
								boxes[curBoxIndex].CurState = Box.State.Move;

								int dirX = player.X - boxX;
								int dirY = player.Y - boxY;

								boxes[curBoxIndex].DirX = dirX;
								boxes[curBoxIndex].DirY = dirY;
							}
						}

						break;
				}
			}

			// 충돌 했는가..
			bool IsCollision( int x, int y, int x2, int y2 )
			{
				if( x == x2 && y == y2)
				{
					return true;
				}

				return false;
			}

			// 범위 안에 있는가..
			bool IsInRange( int x, int y, int x2, int y2, int range )
			{
				int xDistance = Math.Abs(x - x2);
				int yDistance = Math.Abs(y - y2);

				if ( xDistance + yDistance <= range )
				{
					return true;
				}

				return false;
			}

			// 포탈 밟았을 때 다른 게이트로 이동시키는 기능..
			void PushPortal(in Portal[] portals, ref int curPosX, ref int curPosY, int prevPosX, int prevPosY )
			{
				bool isFindPortal = false;

				for ( int portalIdx = 0; portalIdx < PORTAL_COUNT; ++portalIdx )
				{
					for ( int gateIndex = 0; gateIndex < PORTAL_GATE_COUNT; ++gateIndex )
					{
						int curPortalX = portals[portalIdx].GatesX[gateIndex];
						int curPortalY = portals[portalIdx].GatesY[gateIndex];

						if ( IsCollision( curPortalX, curPortalY, curPosX, curPosY ) )
						{
							// 다른 포탈 게이트 위치 계산..
							ComputeOtherPortalGatePosition( portals[portalIdx], gateIndex, out curPortalX, out curPortalY );

							// 다른 포탈 게이트 이동 시 현재 이동한 방향으로 1칸 이동한 위치에 순간이동시키기..
							int dirX = curPosX - prevPosX;
							int dirY = curPosY - prevPosY;

							int teleportPosX = curPortalX + dirX;
							int teleportPosY = curPortalY + dirY;

							// 현재 포탈 이동한 지점에 다른 오브젝트가 있는지 검사..
							MapSpaceType curPosSpaceType = mapDatas[teleportPosY, teleportPosX];
							if ( MapSpaceType.Pass == curPosSpaceType || MapSpaceType.Item == curPosSpaceType )
							{
								curPosX = teleportPosX;
								curPosY = teleportPosY;
							}
							else
							{
								curPosX = prevPosX;
								curPosY = prevPosY;
							}

							isFindPortal = true;

							break;
						}
					}

                    if ( isFindPortal )
                    {
                        break;
                    }
                }
			}

			// Portal 현재 서 있는 게이트가 아닌 다른 게이트의 위치를 계산..
			void ComputeOtherPortalGatePosition( in Portal portal, int ignoreIndex, out int gateX, out int gateY )
			{
				Random random = new Random();

                int curGateX = -1;
                int curGateY = -1;
                int curIndex = ignoreIndex;
				while ( curIndex == ignoreIndex )
				{
					curIndex = random.Next( PORTAL_GATE_COUNT );

                    curGateX = portal.GatesX[curIndex];
                    curGateY = portal.GatesY[curIndex];
                }

                gateX = curGateX;
                gateY = curGateY;
			}
        }
    }
}