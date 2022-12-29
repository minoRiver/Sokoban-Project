﻿using System;

namespace KMH_Sokoban
{
    public enum MapSpaceType
    {
        Pass            // 지나가는 곳..
        , DontPass      // 못지나가는 곳..
        , PlayerStand   // 플레이어가 있는 곳..
        , BoxStand      // 박스가 있는 곳..
    }

    class Program
    {
        static void Main()
        {
            // ------------------------------------------- 초기화(객체 생성 및 초기화).. -------------------------------------------

            #region 상수 초기화
            // ============================================================================================================================================
            // 상수 초기화..
            // ============================================================================================================================================

            // 초기 세팅 관련 상수 설정..
            const bool CURSOR_VISIBLE = false;                      // 커서를 숨긴다..
            const string TITLE_NAME = "Welcome To Liverpool";       // 타이틀을 설정한다..
            ConsoleColor BACKGROUND_COLOR = ConsoleColor.DarkRed;   // Background 색을 설정한다..
            ConsoleColor FOREGROUND_COLOR = ConsoleColor.White;     // 글꼴색을 설정한다..

            // 맵 사이즈 관련 상수 설정..
            const int MAP_WIDTH = 25;
            const int MAP_HEIGHT = 18;
            const int MAP_RANGE_MIN_X = 1;
            const int MAP_RANGE_MIN_Y = 1;
            const int MAP_RANGE_MAX_X = MAP_RANGE_MIN_X + MAP_WIDTH;
            const int MAP_RANGE_MAX_Y = MAP_RANGE_MIN_Y + MAP_HEIGHT;

            // 플레이어 관련 상수 설정..
            const int INITIAL_PLAYER_X = 1;
            const int INITIAL_PLAYER_Y = 1;
            const char PLAYER_IMAGE = 'P';

            // 박스 관련 상수 설정..
            const char BOX_IMAGE = 'B';
            const ConsoleColor BOX_COLOR = ConsoleColor.Yellow;

            // 벽 관련 상수 설정..
            const char WALL_IMAGE = 'W';
            const ConsoleColor WALL_COLOR = ConsoleColor.Black;

            // 골인 지점 관련 상수..
            const char GOALIN_IMAGE = 'G';
            const ConsoleColor GOALIN_COLOR = ConsoleColor.Blue;
            #endregion


            #region 변수 초기화
            // ============================================================================================================================================
            // 변수 초기화..
            // ============================================================================================================================================

            // 플레이어 관련 변수 설정..
            int playerX = INITIAL_PLAYER_X, playerY = INITIAL_PLAYER_Y;
            int prevPlayerX = playerX, prevPlayerY = playerY;

            // 박스 관련 변수 설정..
            const int BOX_COUNT = 3;
            int[] boxesX = new int[BOX_COUNT] { 5, 7, 9 };
            int[] boxesY = new int[BOX_COUNT] { 5, 7, 9 };
            int[] prevBoxesX = new int[BOX_COUNT];
            int[] prevBoxesY = new int[BOX_COUNT];

            // 맵 관련 변수 설정..
            // 맵의 각 위치들의 데이터를 저장하는 룩업 테이블..
            MapSpaceType[,] mapDatas = new MapSpaceType[MAP_HEIGHT + 1, MAP_WIDTH + 1];
            // 벽 관련 변수 설정..
            const int WALL_COUNT = 3;
            int[] wallsX = new int[WALL_COUNT] { 12, 7, 9 };
            int[] wallsY = new int[WALL_COUNT] { 4, 5, 8 };

            // 골인 지점 관련 변수 설정..
            const int GOALIN_COUNT = BOX_COUNT;
            int[] goalInsX = new int[GOALIN_COUNT] { 7, 2, 5 };
            int[] goalInsY = new int[GOALIN_COUNT] { 10, 3, 15 };
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
            mapDatas[playerY, playerX] = MapSpaceType.PlayerStand;
            for (int i = 0; i < BOX_COUNT; ++i)
                mapDatas[boxesY[i], boxesX[i]] = MapSpaceType.BoxStand;

            // 맵 데이터에 벽 위치 저장..
            for (int i = 0; i < WALL_COUNT; ++i)
                mapDatas[wallsY[i], wallsX[i]] = MapSpaceType.DontPass;

            // 맵 외곽 통과 못하는 곳으로 설정..
            for (int i = 0; i <= MAP_WIDTH; ++i)
            {
                mapDatas[0, i] = MapSpaceType.DontPass;
                mapDatas[MAP_HEIGHT, i] = MapSpaceType.DontPass;
            }
            for (int i = 0; i <= MAP_HEIGHT; ++i)
            {
                mapDatas[i, 0] = MapSpaceType.DontPass;
                mapDatas[i, MAP_WIDTH] = MapSpaceType.DontPass;
            }
            #endregion

            while (true)
            {
                #region Render
                // ------------------------------------------------------------------ Render.. ------------------------------------------------------------------
                // 이전 프레임 지우기..
                Console.Clear();

                // 플레이어 출력하기..
                Console.SetCursorPosition(playerX, playerY);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(PLAYER_IMAGE);

                // 골인 지점 출력하기..
                for (int i = 0; i < GOALIN_COUNT; ++i)
                {
                    Console.SetCursorPosition(goalInsX[i], goalInsY[i]);
                    Console.ForegroundColor = GOALIN_COLOR;
                    Console.Write(GOALIN_IMAGE);
                }

                // 박스 출력하기..
                for (int i = 0; i < BOX_COUNT; ++i)
                {
                    Console.SetCursorPosition(boxesX[i], boxesY[i]);
                    Console.ForegroundColor = BOX_COLOR;
                    Console.Write(BOX_IMAGE);
                }

                // 벽 출력하기..
                for( int i = 0; i < WALL_COUNT; ++i)
                {
                    Console.SetCursorPosition(wallsX[i], wallsY[i]);
                    Console.ForegroundColor = WALL_COLOR;
                    Console.Write(WALL_IMAGE);
                }

                // 맵 출력하기..
                Console.ForegroundColor = ConsoleColor.White;
                for (int i = MAP_RANGE_MIN_X - 1; i < MAP_RANGE_MAX_X; ++i)
                {
                    Console.SetCursorPosition(i, MAP_RANGE_MIN_Y - 1);
                    Console.Write('-');
                    Console.SetCursorPosition(i, MAP_RANGE_MAX_Y - 1);
                    Console.Write('-');
                }
                for (int i = MAP_RANGE_MIN_Y - 1; i < MAP_RANGE_MAX_Y; ++i)
                {
                    Console.SetCursorPosition(MAP_RANGE_MIN_X - 1, i);
                    Console.Write('I');
                    Console.SetCursorPosition(MAP_RANGE_MAX_X - 1, i);
                    Console.Write('I');
                }
                #endregion

                // --------------------------------------------------------------- ProcessInput.. ---------------------------------------------------------------
                // 입력한 키 가져오기..
                ConsoleKey inputKey = Console.ReadKey().Key;

                // ------------------------------------------------------------------ Update.. ------------------------------------------------------------------
                // ================================= Player Update.. =================================
                // 플레이어 이전위치 갱신..
                prevPlayerX = playerX;
                prevPlayerY = playerY;

                // 1. 이동 입력 값 처리..
                if (inputKey == ConsoleKey.RightArrow || inputKey == ConsoleKey.LeftArrow)
                    playerX += (int)inputKey - 38;
                if (inputKey == ConsoleKey.DownArrow || inputKey == ConsoleKey.UpArrow)
                    playerY += (int)inputKey - 39;

                // ================================= Box Update.. =================================
                // 박스 업데이트..
                for (int i = 0; i < BOX_COUNT; ++i)
                {
                    // 박스 이전위치 갱신..
                    prevBoxesX[i] = boxesX[i];
                    prevBoxesY[i] = boxesY[i];

                    if (playerX == boxesX[i] && playerY == boxesY[i])   // 플레이어와 박스가 같을 때..
                    {
                        // 박스가 이동할 위치를 계산( 현재위치 - 이전위치 = 이동할 방향 )..
                        int boxMoveDirX = playerX - prevPlayerX;
                        int boxMoveDirY = playerY - prevPlayerY;

                        // 박스 현재위치 갱신..
                        boxesX[i] += boxMoveDirX;
                        boxesY[i] += boxMoveDirY;
                    }
                }

                // ================================= Player Box Update가 끝난 후 Overlap 처리??.. =================================
                // 박스가 특정 물체와 겹쳤다( 박스 or 벽 or 맵 외곽 )..
                for (int i = 0; i < BOX_COUNT; ++i)
                {
                    MapSpaceType curStandSpaceType = mapDatas[boxesY[i], boxesX[i]];

                    // 박스가 현재 위치에 다른 물체가 있을 때는 이전 위치로 이동..
                    if (MapSpaceType.Pass == curStandSpaceType)
                        continue;

                    boxesX[i] = prevBoxesX[i];
                    boxesY[i] = prevBoxesY[i];
                }

                // 플레이어가 특정 물체와 겹쳤다( 박스 or 벽 등등 )..
                MapSpaceType overlapSpaceType = mapDatas[playerY, playerX];
                if (overlapSpaceType == MapSpaceType.DontPass)     // 벽이랑 겹쳐있다( 못지나감 )..
                {
                    playerX = prevPlayerX;
                    playerY = prevPlayerY;
                }
                else if (overlapSpaceType == MapSpaceType.BoxStand)  // 박스랑 겹쳐있다..
                {
                    // 만약 플레이어와 겹쳐있는 박스 찾고 있다면 이전 위치로..
                    for (int i = 0; i < BOX_COUNT; ++i)
                    {
                        if (boxesX[i] == playerX && boxesY[i] == playerY)
                        {
                            playerX = prevPlayerX;
                            playerY = prevPlayerY;
                        }
                    }
                }

                // 골인 지점과 박스 위치가 몇개나 같은지 비교하는 곳..
                int goalInBoxCount = 0;
                for( int i = 0; i < GOALIN_COUNT; ++i)
                {
                    for( int j = 0; j < BOX_COUNT; ++j)
                    {
                        if(goalInsX[i] == boxesX[j] && goalInsY[i] == boxesY[j])
                        {
                            ++goalInBoxCount;
                            break;
                        }
                    }
                }

                // 현재 골인지점과 박스위치가 전부 같다면( GG )..
                if(goalInBoxCount == GOALIN_COUNT)
                    break;

                // ================================= 전체적인 위치의 갱신이 끝났다면 맵에 데이터 저장.. =================================
                mapDatas[prevPlayerY, prevPlayerX] = MapSpaceType.Pass;
                mapDatas[playerY, playerX] = MapSpaceType.PlayerStand;
                for (int i = 0; i < BOX_COUNT; ++i)
                {
                    mapDatas[prevBoxesY[i], prevBoxesX[i]] = MapSpaceType.Pass;
                    mapDatas[boxesY[i], boxesX[i]] = MapSpaceType.BoxStand;
                }
            }

            // 게임 클리어 했으니까 메시지 띄우기..
            Console.Clear();

            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNNNNXXXXXNNNNNNNNNXXXXXXXNNNNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWWWWNXXNXXXXXXXXXXXXXXNXXXXXXXNNXXNWWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWWWWWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWWMMMMWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMWK0KNWNWNWNNWNNWNWNWNNNNWNWNWNNNNWWWNWNNNNNNNNWNWWNWNK0KNWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXNKKXXOkkKXKXKXKKKOKXKXKXKKXKXKXKXKKKKXKXKXKXKKKKXKXKXKKNKkkOXXKKXNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKNXKXXK00KNXNXXXXXKXNXNXNXXXXXXNXNXXXXXXNXNXNXXXXNXNXXXXWX000XXKXK0XMMWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXNWWWWWWWWWWWWWWWWWWWWWMWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWNXWMMMWWWNWWWNXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKXNXXXXXXXXXKXXXXXXXXKXXXWXXXXXKXKXXXXXXKXKXXXXXXKXKXXXXXXXXKXKXWNXXWWXXXKXKXXNXKXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXWMWKXKKKKXKXKXKXKKXKXKXKKWKKXKXKXKXKKKKXKXKXKXKKKKXKXKXKKKKXKXKXWMWXXXKXKKKKXKNMWXXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNNNWWWMMWWWWWWWWWWWWWNXNWWWWWWWWWWWNXNWWWNNWWWWWWWNNNNWWWWWWWWWWWWWWWWMMWWNNXNWWWWWWMWWNNNNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXNNNNNNNXNNXNXNXNWMWXXXXNNXNNNWWMNXXNXNWXKXNWNNNNWKO0XNXNNNNNNXWWNNNNXNNNNXNNKXWMWXNNNNNNXXKXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXWMNKKKKKKKKKKKKKKKNXKXWMXKKKKKKNNXXNWMKKX0XXKNKKKXN0OKWXKXKKKXWXNXKKKKKKKKK0NWWNKXXKK0KKKKKWWXXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNWMMMWNNNNNNNNNNNNNXNNXWMWMWNNNNXXNNNWMMMWNXNWWNWNNNWNXXNMWNNNNNWMMMWNNNNNNNNNNWMMMWNNNNNNNNNNMMMWNNWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXXWWNWNWNWWNWNWNWNWMWXKNWNWNNNNMWNXXNNWWNWNWNXNWNNWNWNWNK0XWWNNNWNWNWMW00WWNWNNWNWNWNWNNNNMWNWWNWNNNNMXOKWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0KKXXKXKXKXKKKKXKXKXNKXWMNKXKKXKNXKNWXOKXKXKKKKKXKKXKXKNXOOXMKKKKXKXKXWK00KNKKKXXKXKXKXKXKKWWXXWKXKKXXX000XMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXXWXXXXNXNXXXXXXNXNXKXWMMMWXXXXXKXNWMMNKXNXNX0XWXXXXXXNXNX0XWMXXXXNXNXNNXWWXNXXXXXXNXNXNXXXXWMWWWXXXXXXXXMNXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNNWWWWWWWWWWWWMMMWXXNWWWWWWMMMWXNNWWWWWWNWMMNXNWWWWWWWWWWNXXNWWWWWWWWWWWWWWMWXXWWWWWWWWWWWWWWWWWWWMNXMNXNWWMMXKWMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKKXXXXXXXXXXXXXWWNKXWXXXXXKNMWXXNNXXXXXXX0KWN0KXXKXXXXXXXNKO0NNXXKXXXXKXXXKNWXOOXWKXXXXXXKXXXXXXXXKWN0K0KNXNMN0OKWMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKKNKXKKKKXKXKXKKNXXWMWKKKKXKXXXNWMNKXKKKKX0KXKXXKXKXKKKKKXX0OKMXKXKXKKKKXKXKXNKNNKXKXKKKKXKXKXKKKKXKNMNOKWXKNNKXWKXMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWNWWWWWWWWWWWWWNNNWMMWWWWWWXXNNWWMMWWWWWWWWWNNNWWWWWWWWWWNWNXXNMWWWWWWWWWWWWWWWWMMWNNWWWWWWWWWWWWWWWWWWWNNWWNNWWWWWNNMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKKXNNXNNNNNXNNNXKXNNNNXNMWNXXNNNNNNNNNNXNNNXOKXNNNNNNNNXNNNN0OKWNNNNNNNXNNNNNNNNXNWNOONWNNNNNNNNNNNNNNNNNN0XK0NNNXNN0OXMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWK0XKKKKKKKKXKKK00XKKKKK0XNXXNMNKKKKKKKKKKXK0KKXKKKKKKKKK0XKKKOOXWKKKKKKXKXKKKKKKKKXN0XKKNKKKKKKKKXKXKKKKKKWXOOXWKXKKKKX0NMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXNWNNNNNNNNNNNNKNMNNNNNNNXNMMMWNNNNNNNNNNNXXNMWNNNNNNNNNNNNNXKNWMNNNNNNNNNNNNNNNNNWNNMWNNNNNNNNNNNNNNNNNNNWWNNMWNNNNNWMNNMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKXWNNWNWNWNWNWNKNWWWWNWXXNWNWWWWNWNWWWWWMX0KNWXNWNWNWWWNWWNWN0KWWNWWWNWNWWNWWWNWWWWNWNWMN00WWNWWWNWWWWNWWWNNWNNXKKXNNNMMXOXMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXO0XWXKKKXKXKXXK0KKKXKXXK0KKXKXKKXKXKXKXKXN0OKWXOKXKXKXKKKKXKX0O0KXKXKXKKKKXKXKXKXKKXKXKNWK00KNKXKXXKXKXKXKXKKXKKkxkkKKKWN000NMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWNWWMXXXXNXNXXX0XWXXXXNK0NNXNXXXXXXXXNXNXNXKKXMNKXXXNXXXXXXNXNK0KXXXNXNXXXXXXNXNXXXXXXNXNNXWNXNXXXXXXXXNXNXXXXXXNKXNKNXXWXNWXXMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNKXWWWWWWWWWWWNXXNWWWWMMWNNWWWWWWWWWWWWWWMMWXKXWWWMWWWWWWWWWWWWWWXKWWWWWWWWWWWWWWWWWWWWWWWWWWMMNKNMWWMWWWWWWWWWWWWWWWNNWNWWWMMWKNMWXXWMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNOOKWXXXXXXXKXN0kKWXXXKNNKKXNXXXXXKXXXXXXXWWXOOKWXXMXKXXXXXNKXXXXXKOKXKXXXXKXXXXXXXXXXXXXXXXXXWN0O0NXXNXXXXXXXXXXXXXXOkkOOKXKWWKO0NMNNWMMMMMMMMMMMMMMMM");
            Console.WriteLine("WMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMKO0NMKKKKXKXKXKO0XMKKXKX0XNKNKKKKXKXKKKKKKNWKO0NWKXWXKXKXKXXKXKXKKKOKKKKKKKKXKXKXKKKKKKXKXKKKKNXKWKXXKNKKKKKKXKKKKKKNO0KOOKKKNXKWXKWMMNNMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXNWMWWWNWWWWNXXNWWWWWWNNWWWWWWWWWWWWWWWWWWNXXNMMWWWWWWWWWWWWWNWNWXKWNNWWWWWNWWNWWWWWWWWWWWWWWWWWMWNNWWWWWWWWWWWWWWWWWWWWWWWWWWWMWNNWMWWMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXOOXWWXNNNNXWWKO0NWNXWWKKNNNNNNNNNNNNNNNNNWWKO0NWNNNNNNNNNNNNNNNNNNK0XNXNNNNNNNNNNNNNNNNNNNNNNNNXWWXO0NMNXNNNNNNNNNNNNXK0OKKXNNNNNNK0WMMMMNXWMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0OKWWX0XKKKKWXOOKWWK0XKKXKKKKKKKKXKXKKKKKKWX0OKWXKXKKKKKKKKKKKXKXKKKO0XKKKKKKXKXKKKKKKXKXKXKKKKK0NX0XKKWX0XKKXKXKKKKKKNkk0kkKKKXKKKO0WMMMMWXNMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXXNMMWNNNNNNWNKXNMWNNXKNWNNNNNNNNNNNNNNNNNWNKXNMNNNNNNNNNNNNNNNNNNNXKNNNNNNNNNNNNNNNNNNNNNNNNNNNNWNNWWNWWNNNNNNNNNXNNNWXNWNXNNNNNNNXXWMMMMMMMMMWWMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKKXMMMWNWNWMWX0XWWWWNNKKWWWWNWWWWWWNWWWNWWNWX0XWWWWNWWWWWWWNWNWWWWWWK0XWWXXWNNWWWNWWWNWWWWWWWWWWNWWNWNNXNNWWNWMMWNXNWNWMKKNWNWWWWWWWNKXMMMMMMMMMNXXWMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKO0NMMMNKXKXWXOOKMXXNKX00XXKXKXKKXKXKXKXKXXKXOO0XKKXKXXKXKXKKXKXKXKXXk0K000KXKXXKXKXKXKXXKXKXKXKXKKXKX0OKKOKXKNWNXXWMWNXNKOKXKXKKXXXKX0XMMMMMMMMMMWXXXWMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNK0XWMMMNXXXXWX0KNMNXNXXK0XXXNXXXXXXNXXXXXXXXN00KXXXNXXXXXXXXXXXNXXXXXOKWKKNXXXXXXXXXXXXXXXXXXXXXXXXXXN0XWNKXNXXXXWWMMMMWWX0XXXXXXXXNXXKXMMMMMMMMMMMMWXXNMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKXWMMMWXXWWWWXXWWWWWWWWXKWWWWWWWWWWWWWWWWWWWXKXWWWWWWWWWWWWWWWWWWWWWWXXMMNNMNXWWWWWWWWWWWWWWWWWWWWWWWWWWWMMWXXWWWMMMMMWXNKKWWWWWWWWWWNXNMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXOOKMMMMWK0XXXX0OXXKXXXXXKOKXXXXXXXXXXXXXXXXXK00KXXXXXXXXXXXXXXXXXXXXXXKKWMX0K0KNXXXXXXXXXXXXXXXXXXXXXNXXXXWNXXWWWWWWMWWK0KO0XXXXXXXXXXX0XMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKO0NMMMMWKOKXKK0OKKKXKKKKKO0XKKKKXKXKXKKKKX00XX00XKKKKXKKXKKKKKKXKXKKKK00WMWXOKWXKXKXKKKKKKXKXKKKKKKXKXKKK0XXWWWWWMWWMWXKNXkOKXKKKKKKKKK0XMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXXNMMMMMWXXWNNNKXWWWWWWWWXKNWWWWWWWWWWWWWWNXNNXKNWWWWWWWWWWWWWWWWWWWWWWKKWMMMNNWWNNNWWWWWWWWWWWWWWWWWWWWNNNWMMMWWWMWWWNNWWWKXWWNWXNWWWWNKXMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0OKWMMMMMWKKNNNN0KNNNNNNNNK0XNNNNNNNNNNNNWNKXMXO0KNNNNNNNXNNNNNNNNNNNNNNX0NMMMWKKNKXNNNNNNNNNNNNNNNNNNNNNKXWMWWMWWMWWNKXNWXNK0NNNN0ONNNNX0XMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMKOOXMMMMMMWKOKKKK0OKKKKKXKKKO0XKKKKXKXKXKXN0KWMXO0OKKKXKKKOKKKKKXKXKKKKKKK0NMMMMN0kKWKKKKXKXKKKKKKKKXKXKKKKWWMWWMWWMWN0KXKNKK0O0KKK0OX0XX0OXMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKXWMMMMMMWXKNNNNKKNNNNNNNNXKXNNNNNNNNNNNNNXWWMXKKXNNNNNNXKNNNNNNNNNNNNNNXXWMMMMWNXNMNNNNNNNNNNNNNNNNNNNKXWWWMWWMWWMWNXWNNWNNXKNNNNXNWNWNXKNMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWX0XWMMMMMMWKKWWNWKKWWNWWWWWXKNWWWWWWWWNWMNXWWXNXKWWNWWNNX00KNNWWWWNWWNNXKXNWMMMMMMWXXWMMMMMMMMWWWNWMMWNXNWMMMMMMMMMMXXWWWWNWWKKNWNWXXMMMWXXWMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKOKWMMMMMMWKOXXKXK0XXKXKXKKK0KXKXKKXKXKXN0KWNKKOkKKO00OKK00KNXXKKXKXKX0kkk0WMMMMMMMWNXXWMMMMMMXKXKNWNXXWWWWWWWMWWWWN0XWKXXKNKk0KXKX0KWWWWWNXXNMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWK0KWMMMMMMWK0XXXXK0XXXNXXXXK0KNXXXXXXNXXKXWXKXX00NNXNNXNNXXXNXXXXXXNXNKKNKXMMMMMMMMMMWNXXXXXXXKXXXXXXWMMWWWWWWMWWMNKXWWXXNXNX0KXXXXKKWWWWWMWNXNMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWX0XWMMMMMMWXXWWWWXXWWWWWWWWNNMNXNMWXKK0XXXWWWMMXKNWXXMWWMWNNWWWWWWWWWNNNWWMMMMMMWNXXXXNWX0KXXXNWWNNWWMMMMMMMMMMMMMWKXWWWWWWWWKKWWWWKKMMMMMMMMWWWMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKOKWMMMMMMWK0XXXXK0XXXXXXXXKKNKO0NNOkOOXK0WMMMMK0KX0KMXKXKO0XXXXXXXXKkkk0NMMMMMMMMMMMMMMWNXXNMNXX0XWWWWMWWMWWMWWWWN0XXXXXXXXXO0XXXXKKWMMMMWWWMX0NMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0OKWMMMMMMWKOKKKKKOKKKXKKKKKKKKNKKK00KXK00NNNWWKO0K0KMWKkKNKKKKXKXKX0OKO0WMMMMMMMMMMMMMMMMMNXXK00XWWMWWMWWMWWMWWWWXOKKKXKXKKKkO0KKK00WWNNNNWWWXKNMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWX0XWMMMMMMWXXWWWNKXWNWWNXXNNXNWMWWNWNNWNX0KXXXXXKKK00NWWXNMWWWWWNWWWWWWWWMMMMMMMMMMMMMMMMMMMWNNNNWWWMWWWWWMWWWWWWWWXKNNXNWWWNXXNNWWNXX0xOKXWMWWXNMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKOKWMMMMMMWKKNNNN00NNXNXO0NWX00KWNXNNXNWXOXMMMMMNXXXXNWN0OKNMWNNNNNNWXKWMMMMMMMMMMMMMMMWWWNXNWXKWWMWWMWWWWWMWWWWWMWNKKKKNNNWKKNXXKXNXXX0KWWMWWWKKMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0OKWMMMMMMWKOKKKK0OKX0XX00NNOkk0X0k0kOK0OOXMMMMMMMMMMMW0KKO0NWKKKKXKXKXMMMMMMMMMMMMMMWWNXXXXNX0NWWWWWWWWMWWMWWWWWWWWKOOXNKXX0XWXKKKNWWMMMWWMWWWKKMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKXWMMMMMMMXKNNXNKKNNXNWNXNWXK0KX000O0XNXKKXXKXXWMMWWWNNWWWNNWNXNNNNXNMMMMMMMMMMMMMMWNNWMMMMWXNMWWMWWMWWMWWMWWMWWMWXXNNXXNNNNMWNNWMMMMMMMWWMWWWXXNNNNWMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKKNWMMMMMMMWKXXKNXKWWWWWWWX00KKKNXXXXXXNWNNWKKNNWWXO000NNWWWWMWXXWXXWMMMMMMMMMMMMMMMMMMMMMMWXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXXXXXXNWMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0OKWMMMMMMMMX0OXWK0XXKXXXXOkxO00NMMMMMX0K0XXOXX00XX0KXXKKOxONMMN000XMMMMMMMMMMMMMMMMMMMMMMWK0NWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMN0XMMMMMWNWWMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWK0XWMMMMMMMNXXXXXK0XXXXXXXKX0KWKXMMMMMNXNXXNKKKOKNK00KKKK00XMMMMN0XMMMMMMMMMMMMMMMMMMMMMMMNKNWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWWWWWKXMMMMMMMWNWMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMMMMMMMMMWWMMWNKXWWWWWWWWWNNNXKNMMMMMNXNNXWNKKNWMWXXNWNWMWXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMMMMMMMMWXKWMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW00XXXXXXXXXXXXKk0WMMMMWK00KWNXXNWWNXXNK0WMWK0WMMMMMMMMMMMMMMMMMMMMMMMMMMMX0XMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWWWWWWMWWWMN0XWMWWMWWWWX0NMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKOKKKXKKKKK0Ok000KNWMWNK00KKKKXNNXXWNXXNMMWKKWMMMMMMMMMMMMMMMMMMMMMMMMMMMX0XWWWMWWMWWMWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWN0XWWWWWWWWWWKONMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXWNWWWWNWWNXKXXXXNWMWXKXK0KXXXXNXXWNNWWWWWNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKXWWWMWWMWWMWWWWWMWWMWWMWWWWWWWWMWWMWWWWWWWMWWWWNWWWWWWWWWMWXXWMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKKNNNNNNNNMXOKWMWKXMWXXX000KWWKXMWXXWWXXWWWXXWWWWWWWWWMMMMMWX00KWWX00XWWXXXNMMMMMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMMXKWMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKOKXKXKKKKX0XKKWKKWWKKNXKKKXNKKWWKKNWK0NWNK0NWNNNNNNNWWWWWWWKOk0NW0Ok0WXOKNWWMMMMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMX0NMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKNNXNNNNNNNWNXXXWWNXNWXXXXXXXWMNXNWNXNMNXXNMWWWWWWWWWWWWWWWWWWWMMWWWWMWNWWNXNMMMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWWXNMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKXXXXKWWWWWWNNN0KMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0NMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0KWMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0KKKK0XKXKXX0KX0OXWWWMMMWWWWWWMMMMMMMMMMMMMMMMMMMMMMWWMMMMMMMMMWWWWWWWWWWWWWX0XWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWWWWWWWWWWWWWWWWMWNKO0XMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNXNXKXXXXXX0OOKK0KXNMWMNXXXXWMMMMMMMMMMMMMMMMMMMMMMWWMMMMMMMMMWWWWWWWWWWWWWX0XWMWWMWWMWWMWWMWWMWWMWWMWWWWWWWWWWWWWWWWWWWWMWWWWWWWXXWNKWMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMMMWXKNN0KNXKXKKXNNXXWMWNNNNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWMWNWMMMNXNMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0KKkOX0ONNXK0NWXKXWMWWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0XMMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWMMWWWWN0KWWWWW0KWMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWWMMNO0K0KOONNXXXNMMWXXXXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0XWMWWMWWMWWMWWMWWWWWMWWWWWWWWWWWWWWWWWWWWWWWWWWWNKKWMWWWW0KMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMMMMMWWNXXNMWNXXNKKKXWWWMWWMMMMWNXXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWXKKKXXWNKXXXNNWWMWWMWWMWWMWWMWWMWMWWMMWMMWMMWWMWWWNNWMMWWMNNWMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXXXNWWKXMMMMMWKO0XKx0XXXNXXWMMMMMMMWXXXNWMMMMMMMMMMWWNNNNNNNNNWMMWWWWWWMMMMMMMMMMWXXNX000KWX000KNXXNWMMWMMWMMWMMMMMMMMMMWMMWWMWMMWWMMMMWWMWWMWKXMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXK0KNK0NMMMMMWK0Kk0000O0XK0WMMMMMMMWXK0KNMMWMMMMMMMN00XXXXXXXXWMWXXXXXNWMMMMMMMWXXNWMNKKKXNNKKKXWMWXXNWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWNNKXWMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMMWXNWMWXNWXXXXXKXNWKNXXXNNNNNWMMMMMMMWXNWMMWNXNWMMMWWWNWMWWMMMMMMMMMMMMMNNWMMMMMNXNWMMWXXXXXXXXXXXNMMMWXNWWWWWWWMWWMWWMWWMWWMWWMWWMWWMWWMWWMWNXNMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXWMMMMMMMNNMWWK0KKKXXNK0NXO0XNWWNWMMMMMMMMMMMMWXXNMMMWXXXNWMNNWNXXXNMMMMMMMMWXKXXWNXKKNWXKXXWNXKKNWNKKXWWXXNXXWMWWMMMMMMMMMMMMMMMMMMMMMMMMMMWNNNNWMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXWMMMMMMMMKKWMN0XMMMMMX000000XKKK0NMMMMMMMMMMMMMWNXXWMWWX0OKWMMWWWWWWMMWWWXKXN0kk0NNKKKXNXKKKNNKKKXWXKKXNNXKNWNXXNWWWWWWWWWWWWMWWWWWWWMWWMMWXKXWK0WMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXWMMMMMMMMMMX0XNXNWMMMMMNXN0KX0XXNXXWMMMMMMMMMMMMMMMWNXNWNXXNWMMMWXXXXXNMX0XXKKXKOOONNKKKXNXKXXNXKXKXNXKXXNNXXNWMWNXNWWWWWWWWWWWWWMMWWWWWWWMMX0KXNXXWMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWWMMMMMMMMMMMMMWX0XMMMMMMWWMMWXXWX0XWWWWMMMMMMMMMMMMMMMMWWNKXWMMMMMMMNNNNXXNWNXXXWNXKXNWNNXNWWNXXNWNNNXNWNXXNWWNXXWWNXXNNWMMMMMMMMMMMMMMMWMMMMMMMMMWNWMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0KMMMMMMMMMMMMMMMMKxKMMMMMX0NMMMX0K0x0XKKXWMMMMMMMMMMMMMMMMMWXXXXWMMWNXXWNXKXNWXXXXWNXXKNWNKKXNWXKXXWNXXXNWXXXXWNXXXNWXXNWXKXWMWWMWWMWWMWWMWWMWWMWWMWKKWMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKKWMMMMMMMMMMMMMMMM0x0NXNNXXNMMMMWKOK00KXNKNMMMMMMMMMMMMMMMMMMMMWXXNNXXWMWNXXKXNXXXXNNXXXXNXXXXNNXXXXNXXXXNNXXXXNNXXXNNXXXWMWXKNNWMWWMWWMWWMWWMWWWWWWKKNMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXWMMMMMMMMMMMMMMMMWXKXXKXNNWWNWMMMWNXNKXWMWWWMMMMMMMMMMMMMMMMMMMMMWNNWWMMMWWWWWWWXXKXXNWWWWWWXKXXKXNNNWWWWWWWWWWWWWWWWMWWWMMWMWNNNWWWMWWMWWMWWMWWWNXXXWMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0WMMMMMMMMMMMMMMMMMXKNMMMMMMWKKWMMMX0X00WMXOKWMMMMMMMMMMMMMMMMMMWXXNXXXNWNXXXWMXKXXXXXNWWNNMWNXXXXNWWXXNWMWNXXXWWXXXNWNXXXWWXXXNWXKXWMMWMMWMMMMMWXXXXXNWMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMK0WMMMMMMMMMMMMMMMMMN0XMMMMMMMK0WMMMWKkONMN0X0KWMMMMMMMMMMMMMMMMMX0XNXXXNNXXXXNX0XMMWMMMMWNNWMMMMMMMMMMWXKNWXXXXNNXXXXNNXXXNNXXXXWMWXKXWWWWWWWWNXXXWMMWXXXWMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMWXXWWXKXXXKKNWMMMMWKXWWNNMNNWWMMMMMMMMMMMMMMWNXNWNXXXNNNXXXNXNWNNWWWWMMMMMMMWWWMMMMMMMWXXXXXXNNXXXXNNXXXNNXXXXWMMMWXNWWWNXNWXXWMMMMMMWXNWMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMMNX00KKKKKKKXXXWMMMMWXXWMMMMWKXMMMMMMMMMMWNXXKKNXXKNWNXXXWMWXXXXXXXXXNMMMMMMWNXNMMMMMMMMWNXNMMMNXXXNWXKXXWNXKXNWNKNNXKXKXWWMMMMMMMMMMMNKNMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMK0WMMMMMMMMMMMMMMMMMMMK0WMMMMMWWX0XMMMMMWNXXWMWKO0XMMMMMMMWNXXWMX0XXKKXWXKKXNWKKWMMMMMWNXXNMMMMMMNXXNMMMMMMMWNXXWWNXKKNWXKKXNNXXKXWNKXNXKX0KMMMMMMMMMMMMMX0NMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMMMWKKWMMMMMMWXKWMMMMMMMWNXNXKWXKWMMMMMWXXWMMMX0KXKKXNXKKKNXKWMMMMMMMMWNXXWMMMMMMNXXWMMMMMMMMNXXXXXKXNXKXXNNXXXXNXKXXXXX0XMMMMMMMMMMMMMX0NMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXWMMMMMMMMMMMMWWMMMMWXXWMMMMMWNKKXXXXWWMMMMWWWXNWMMMMMMWNWMMMMWNNNXNNWWNNWMMXXMWXXXNWMMWWWWWNNWMMMMMWWMMMMMMMMMWWNXWMMMWNNNNWWNXNWWNNNNWWNNWMMMMMMMMMMMMNXWMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0WMMMMMMMMMMMWXXWMMMMWNXXWMMMX0KXXXXNWWXNMMMMMWXXNMMMMN0XMMMMMWXKXXKXWWNXK0X0KN0KXXNWXXWMMMMWXXXWMMMMMMMMMMMMMMMMWXXXWMWXXKXWNXXXNWXXXXWWKKWMMMMMMMMMMMW0KMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0WMMMMMMMMMMMMWNNMMMMMMWXKXNX0XWMMMMMMMNNWMMMMMMNXXNMW0KWMMMMMMX0KXXXNWNXXKN000KWMMMMWNWMMMMMMWXXNWWMMMMMMMMMMMMMMMWXKXNXXXXNXXXKNNXXXXNK0NMMMMMMMMMMMWKKWMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMWNMMMMMWNXXXXXXXNNWMMMWNWMMMMMMMNNWNNWMMMMMMMWNWWNNWWWWMMMNKNWMWWMMMMMMWWWMMMMWNWMMMMMMMMMMMMMMMMMMWNNNNWWWWWWWWWWWWWNNWMMMMMMMMMMMMNNWMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMXXMMMMMMMMMNXXXXXKKNMWWXKNMMMMMMMNXNMMMMMMMMMMWNXXWMMMMMMNKNMMMN0XMMMMMWNKXWMMMMMMMMMMMMMMMMMMMMMMMMMWNKXWMMNXNWNXNWNKNMMMMMMMMMMMMNKWMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0WMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0XMMMX0XMMMMMMW0KWMMMMMMMMWNXXNMMMMMMMW0KWMMMW00WWMMMMMWXXXWMMMMMMMMMMMMMMMMMMMMMMMMMWXKXWNXXNXXNN0KWMMMMMMMMMMMN0XWMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMMMWNWMWWNWWMMMMWWNXXWMMWXXXXXXXNMXXWMMMMMMMWNXXNMMMMMMMMMXXWMMMMNXNNWWWMMMMMWXNWMMMMMMMMMMMMMMMMMMMMMMMMMMWXNNXXNXXNXXWMMMMMMMMMMMWXXWMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMMMMWXXXXXXXXXXXXXXXXXXXXXXXXKKNWWNKNMMMMMMWXNWWMMMMMMMMMMMNXNWMMMMMWWMWXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMMMNNMMMMMMMMMMMMMNNMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMK0WMMMMMMMMMMMMMMMMWNXXWWWMMMMW0ONMMMMMWWWWWWNOOWMW0KWMMMMWXXXWMMMMMMMMMMMMMMWXXNWMMMNXNWWXXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXNWN0XMMMMMMMMMMMMX0NMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMWXXWMMMMMMMMWKXMMMMMMMMMMMMWKXMMWXKNMMWNXNWMMMMMMMMMMMMMMMMMMWXXWMMMWWMMMWXXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWNKXWMMMMMMMMMMMNKNMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXNMMMMMMMMMMMMMMMNXNXXXXXXWWMMNXXXNWMWWXXXXXXXWMWWWWWWNNWWMMMMMMMMMMMMMMMMMMMMMMWWWNWMMMWXXXKXNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKXMMMMMMMMMMMMXXWMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMKKMMMMMMMMMMMMMMNKO0NNWWWNKKWMWXXXNWNKXNNWWNNKKWNXWWXKXWMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXNWW0OXNNNXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXXMMMMMMMMMMWWNNMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMKKMMMMMMMMMMMMMWKKK0NMMMMMKOKNXXXXXNXOKMMMMMWKKWK0XXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXNKKWMMWWXXXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWWMMMMMMMMMNWMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXMMMMMMMMMMMMMWNNNXNMMMMMWXXNNXNNXNK0XWMMMMWNNXNXNWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNXNWMMMMWWWNWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXWMMMMMMMMNNMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0NMMMMMMMMMMMMMMMX0NNNNNNNNNWNNNNNNWK00KNWNNNNNXNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXKNMMNXNNXXNWMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMKKWMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN0XMMMMMMMMMMMMMMMK0NWWWWWWWWWWWWWWWWX0O0WWWWWNKKWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWWMWNXXXXO0MWXKXWNK00NMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMK0WMMMMMMMMMMMMMMM");
            Console.WriteLine("WMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMNKXWNNNNWNNNWNNNNNNNK0XWNWNNNXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNWMMMWWWNNWNXNWWXXWMMMMMWNKXXXXWMMMMMMMMMMMMMMMWWMNNMMMMMMMXKWMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXKWMMMMMMMMMMMMMMMNXNNNWNNNNNNNNWNNNWWK0KNWMNXWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXNXKKXNXXMMMMWNXNNXXXWWXXXXNNXOKNWMMMMMMMMMMMMMMMMMMNKNMMMMMMXKWMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMK0WMMMMMMMMMMMMMMMKONNNWNWNWNNNNWNWNWN0O0NWWKKWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWNXXWWXXKXK0NMMWNXXWWXXXXNNXXXXWWKO0XMMMMMMMMMMMMMMMMMMMWNWMMMMMMK0WMMMMMMMMMMMMMMM");
            Console.WriteLine("WMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMKKWMMMMMMMMMMMMMMMXXNNNNWNWNNNNNNWNWMNKOKWN0KWMMMMMMMMMMMMMMMMMMMMMWNXXXXXNWMMMWNXXWMMNXXXXXKXNXXXWMMWXKXXNNXXXXWXKWXKWMMMMMMMMMMMMMMMMMMMMNNWMMMMK0WMMMMMMMMMMMMMMM");
            Console.WriteLine("WMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWXXMMMMMMMMMMMMMMMWXXNNNNNNNNNNNNNWNWMNXXNNXNWMMMMMMMMMMMMMMMMMMMMWWNXXXXKKKXXXNWNXNNWWNNNNWWNKKXNNNNNWWNNNWWWNWWNNNMWXNMMMMMMMMMMMMMMMMMMMMN0XMMMMNKNMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0KMMMMMMMMMMMMMMMN0KWNNWNNNNNNNNMWNWMN0XX0NMMMMMMMMMMMMMMMMMMMMWNKXMMMMWKXWXXXNWNXKXNWXKXXWNXKXNWXXKXWNXKKNWXKNWXXNMXOONMMMMMMMMMMMMMMMMMMMWXNMMMMN0XMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMW0KMMMMMMMMMMMMMMMN0XWWWWWWWWWWWWWWWWWN000XMMMMMMMMMMMMMMMMMMMMWKOKWMMMWK0NXXXXXNXXXKNNXXXXNXKXXXNXXXXNNKXKXNXKXXXKNN0XK0NMMMMMMMMMMMMMMMMMMMMWWMMMN0XMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWKXMMMMMMMMMMMMMMMWXNNNWNNNWNNNNNNWNXNWXXNMMMMMMMMMMMMMMMMMMMMMNNNWMMMMWNWWNXXXXNWNNNWWWWWWWWNWNWWNNWNWWNWNWWWNWWNNWWNWWXXWWWMMMMMMMMMMMMMMMMMWNWMMNKNMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMMXNWNNNNNNWNWNNNNNXWXXWWMMMMMMMMMMMMMMMMMMMNXNMMMMMNXXNXXXWNXXXNWXXXXWWXXXNWNXXXWWXXXXWNXXXNWXXNNXXNNXK0XMMMMMMMMMMMMMMMMMMMWNWMMNKXMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMX0NMMMMMMMMMMMMMMMMKXMWWWWWWWWWWWWWMMN0XMMMMMMMMMMMMMMMMMMMMWKKWMMMMWKKNXXXXNNXXXXNXXXXNNXXXXNNXXXNNXXXXNNXXXNNXXXXXXX0O00XMMMMMMMMMMMMMMMMMMMNKNMMN0XMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNKNMMMMMMMMMMMMMMMWNNNNNNNNWNNNWNNNNWXXMMMMMMMMMMMMMMMMMMMMMNXWMMMMMNXWWXXXXNNXXXNNXXXXNNXXXXNNXXXNNXXXXNNXXXNNXXXXXXNNNXKNMMMMMMMMMMMMMMMMMMMNXWMMNKNMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWMMMMMMMMMMMMMMMMMMMWWWWWWWWWWWWWWWMWWMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMWMMMMMMMMMMMMMMMMMMMMMMMMMWWMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
        }
    }
}