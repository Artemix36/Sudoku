﻿using System.Diagnostics.Metrics;
using System.Globalization;

namespace Sudoku
{
    class SudokuState
    {
        int[,] state = new int[9, 9];
        public void SetState(int[,] newState)
        {
            int[,] result =  new int[9, 9];
            Array.Copy(newState, result, 81);
            state = result;
        }
        public int[,] getState()
        {
            return state;
        }

        int[,] Save = new int[9, 9];
        public void SetSave(int[,] newSave)
        {
            int[,] result =  new int[9, 9];
            Array.Copy(newSave, result, 81);
            Save = result;
        }
        public int[,] getSave()
        {
            return Save;
        }

        int[,] Winner = new int[9, 9];
        public void SetWinner(int[,] newWinner)
        {
            int[,] result =  new int[9, 9];
            Array.Copy(newWinner, result, 81);
            Winner = result;
        }
        public int[,] getWinner()
        {
            return Winner;
        }
    }

    class SudokuMap : SudokuState
    {
        const int n = 3;
        private int level = 0;
        private bool is_active = false;

        public int[,] Room()
        {
            return base.getState();
        }

        private int[,] Transposition(int[,] map)
        {
            int[,] Tmap = new int[n * n, n * n];

            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    Tmap[i, j] = map[j, i];
                }
            }
            map = Tmap;
            return (map);
        }

        private int[,] Line(int[,] map, Random rnd)
        {
            int Block = rnd.Next(0, n - 1);
            int LineOne = rnd.Next(0, n);
            int LineTwo = rnd.Next(0, n);
            int Lo = Block * n + LineOne;
            int Lt = Block * n + LineTwo;
            for (int j = 0; j < n * n; j++)
                (map[Lo, j], map[Lt, j]) = (map[Lt, j], map[Lo, j]);
            return (map);
        }

        private int[,] Column(int[,] map, Random rnd)
        {
            int Block = rnd.Next(0, n - 1);
            int ColumnOne = rnd.Next(0, n);
            int ColumnTwo = rnd.Next(0, n);
            int Co = Block * n + ColumnOne;
            int Ct = Block * n + ColumnTwo;
            for (int i = 0; i < n * n; i++)
                (map[i, Co], map[i, Ct]) = (map[i, Co], map[i, Ct]);

            return (map);
        }

        private void Hide(ref int[,] map, Random rnd)
        {
            int chance = 0, null_check = 0, removed = 0;

            if (level == 0)
            {
                removed = 40;
            }
            if (level == 1)
            {
                removed = 54;
            }
            if (level == 2)
            {
                removed = 61;
            }

            while (removed != 0)
            {
                for (int i = 0; i < n * n; i++)
                {
                    for (int j = 0; j < n * n; j++)
                    {
                        chance = rnd.Next(0, 3);
                        if (chance == 0 && map[i, j] != 0 && removed > 0)
                        {
                            null_check = Null_Check(map, i, j);
                            if (null_check == 0)
                            {
                                map[i, j] = 0;
                                removed--;
                            }

                        }
                    }
                }
            }
        }

        private int Null_Check(int[,] map, int i, int j)
        {
            int line = 0, column = 0;
            for (int k = 0; k < n * n; k++)
            {
                if (map[i, k] == 0)
                {
                    line++;
                }
                if (map[k, j] == 0)
                {
                    column++;
                }
            }
            if (line == n * n - 1)
                return line;
            else if (column == n * n - 1)
                return column;
            else return 0;
        }

        public void LevelChoosing(int new_level)
        {
            level = new_level;
            is_active = false;

        }
        public int GetLevel()
        {
            return level;
        }
        public bool GetIsActive()
        {
            return is_active;
        }

        public void CreateRoom()
        {
            is_active = true;
            int[,] map = Room();
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    map[i, j] = ((i * n + i / n + j) % (n * n) + 1);
                }
            }
            Random rnd = new Random();

            int create = rnd.Next(n * n * n * n, n * n * n * n * n * n * n * n);
            for (int i = 0; i < create; i++)
            {
                map = Transposition(map);
                map = Line(map, rnd);
                map = Column(map, rnd);
            }
            SetWinner(map);
            Hide(ref map, rnd);
            SetSave(map);
            base.SetState(map);
        }
        public void GetCell(int x, int y, int num)
        {
            int [,] transferCell = getState();
            transferCell[x - 1, y - 1] = num;
            SetState(transferCell);
        }
        public bool InputValidation(int x, int y)
        {
            int[,] validationcheck = getSave();
            //int[,] validation = getState();
            bool Validation;
            if (validationcheck[x - 1, y - 1] != 0)
            {
                //if (validation[x - 1, y - 1] == 0)
                //{
                    //Validation = true;
                    //return (Validation);
                //}
                //else
                //{
                    Validation = false;
                    return (Validation);
                //}
            }
            else
            {
                Validation = true;
                return (Validation);
            }
        }

        public bool WinCheck()
        {
          int[,] WinPretendent = getState();
          int[,] WinVariant = getWinner();
          bool wincheck = false;
          foreach (int s1 in WinPretendent)
            foreach (int s2 in WinVariant) if(s1==s2) wincheck = true;
          return wincheck;
        }
        
        public int[,] DeveloperWin()
        {
            int[,] devel = getWinner();
            SetState(devel);
            return devel;
        }
    }

    class SudokuUi : SudokuMap
    {
        private void StartMenu()
        {
            bool startMenuActive = true;
            bool appActive = true;
            do
            {
                do
                {
                    try
                    {
                        Console.WriteLine("----МЕНЮ----");
                        if (base.GetIsActive()) Console.WriteLine("0. Продолжить игру");
                        Console.WriteLine("1. Начать игру");
                        Console.WriteLine("2. Режим сложности");
                        Console.WriteLine("3. Выход");
                        int input = Convert.ToInt32(Console.ReadLine());
                        if (input < 0 || input > 3)
                        {
                            throw new Exception("Ошибка");
                        }
                        else
                        {
                            if (input == 0)
                            {
                                startMenuActive = false;
                                break;
                            }
                            if (input == 1)
                            {
                                base.CreateRoom();
                                startMenuActive = false;
                                break;
                            }
                            if (input == 2)
                            {
                                ChangeDifficulty();
                            }
                            if (input == 3)
                            {
                                startMenuActive = false;
                                appActive = false;
                                break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Неверный формат ввода.");
                    }
                }
                while (startMenuActive);
                while (appActive)
                {
                    try
                    {
                        PrintSudoku();
                        Console.WriteLine("1. Ввести число");
                        Console.WriteLine("2. Завершить игру");
                        Console.WriteLine("3. В главное меню");
                        int input = Convert.ToInt32(Console.ReadLine());
                        if (input < 1 || (input > 4)) //&& input != 4))
                        {
                            throw new Exception("Ошибка");
                        }
                        else
                        {
                            if (input == 1)
                            {
                                InputCell();
                            }
                            if (input == 2)
                            {
                                if(WinCheck()){
                                    Console.WriteLine("GJ<MKCMSKI");
                                    break;
                                } else {
                                    Console.WriteLine("ОШИБКА");
                                }
                                
                            }
                            if (input == 3)
                            {
                                startMenuActive = true;
                                break;
                            }
                            if (input == 4)
                            {
                                
                                base.DeveloperWin();
                                Console.WriteLine("DEVELOPMENTMODE");
                                int[,] a = base.getWinner();
                                // Console.WriteLine(a);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Неверный формат ввода.");
                    }
                }
            }
            while (appActive);
        }
        private void PrintSudoku()
        {
            int n = 9;
            int[,] mas = base.Room();
            Console.Write(String.Format("{0,3}", '|'));
            for (int j = 1; j <= n; j++)
                Console.Write(String.Format("{0,2}", j));
            Console.WriteLine();
            Console.Write(String.Format("   {0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}", '-'));
            Console.WriteLine();
            for (int i = 0; i < n; i++)
            {
                Console.Write(String.Format("{0,0}{1,0} ", i + 1, '}'));
                for (int j = 0; j < n; j++)
                {
                    if (j != n - 1)
                    {
                        Console.Write(String.Format("{0,1}{1,1}", '|', mas[i, j] == 0 ? ' ' : mas[i, j].ToString()));
                    }
                    else
                    {
                        Console.Write(String.Format("{0,1}{1,1}{2,1}", '|', mas[i, j] == 0 ? ' ' : mas[i, j].ToString(), '|'));
                    }
                }
                Console.WriteLine();
            }
            Console.Write(String.Format("   {0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}", '-'));
            Console.WriteLine();
        }
        private void InputCell()
        {
        int x_coord;
        int y_coord;
        int value;
            do
            {
                try
                {
                    Console.WriteLine("Введите строку:");
                    int x = Convert.ToInt32(Console.ReadLine());
                    if (x < 1 || x > 9)
                    {
                        throw new Exception("Ошибка");
                    }
                    else
                    {
                        x_coord = x;
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Неверный формат ввода.");
                }
            }
            while (true);
            do
            {
                try
                {
                    Console.WriteLine("Введите столбец:");
                    int y = Convert.ToInt32(Console.ReadLine());
                    if (y < 1 || y > 9)
                    {
                        throw new Exception("Ошибка");
                    }
                    else
                    {
                        y_coord = y;
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Неверный формат ввода.");
                }
            }
            while (true);
            do
            {
                try
                {
                    Console.WriteLine("Введите число для выбранной клетки:");
                    int num = Convert.ToInt32(Console.ReadLine());
                    if (num < 1 || num > 9)
                    {
                        throw new Exception("Ошибка");
                    }
                    else
                    {
                        value = num;
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Неверный формат ввода.");
                }
            }
            while (true);
            try
            {
                if (base.InputValidation(x_coord, y_coord))
                {
                    base.GetCell(x_coord, y_coord, value);
                }
                else
                {
                    throw new Exception("Ошибка");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Вы ввели занятую ячейку\nНачните сначала");
                InputCell();
            }
        }


        private void ChangeDifficulty()
        {
            string[] difficultyArr = new string[3] { "Легкий", "Средниий", "Сложный" };
            int activeDifficulty = base.GetLevel();
            Console.WriteLine(String.Format("Текущий уровень сложности: {0,0}", difficultyArr[activeDifficulty]));
            do
            {
                try
                {
                    Console.WriteLine("Выберите уровень сложности:");
                    Console.WriteLine("1. Легкий");
                    Console.WriteLine("2. Средний");
                    Console.WriteLine("3. Сложный");
                    int x = Convert.ToInt32(Console.ReadLine());
                    if (x < 1 || x > 3)
                    {
                        throw new Exception("Ошибка");
                    }
                    else
                    {
                        if (activeDifficulty != x - 1)
                        {
                            base.LevelChoosing(x - 1);
                        }
                        Console.WriteLine("Сохранено!");
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Неверный формат ввода.");
                }
            }
            while (true);
        }

        static void Main()
        {
            SudokuUi map = new SudokuUi();
            map.StartMenu();
        }
    }
}
