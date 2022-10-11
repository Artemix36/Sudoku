﻿using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;

namespace Sudoku
{
    class SudokuState
    {
        int[,] state = new int[9, 9];
        protected void SetState(int[,] newState)
        {
            int[,] result = new int[9, 9];
            Array.Copy(newState, result, 81);
            state = result;
        }
        protected int[,] getState()
        {
            return state;
        }

        int[,] Save = new int[9, 9];
        protected void SetSave(int[,] newSave)
        {
            int[,] result = new int[9, 9];
            Array.Copy(newSave, result, 81);
            Save = result;
        }
        protected int[,] getSave()
        {
            return Save;
        }

        int[,] Winner = new int[9, 9];
        protected void SetWinner(int[,] newWinner)
        {
            int[,] result = new int[9, 9];
            Array.Copy(newWinner, result, 81);
            Winner = result;
        }
        protected int[,] getWinner()
        {
            return Winner;
        }
    }

    class SudokuMap : SudokuState
    {
        const int n = 3;
        private string path = "./records.txt";
        private int level = 0;
        private bool is_active = false;
        private Stopwatch stopwatch = Stopwatch.StartNew();
        private string elapsedTime = "";
        protected void Timer_Start()
        {
            stopwatch.Start();
        }
        protected string Timer_Stop()
        {
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            return elapsedTime;
        }
        protected void Record_Write(string name)
        {
            StreamWriter SW = new StreamWriter(path, true, System.Text.Encoding.Default);
            SW.WriteLine(name + " " + elapsedTime);
            SW.Close();
        }
        protected List<string> Record_Read()
        {
            List<string> result = new List<string>();
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }
            return result;
        }
        protected int[,] Room()
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

        protected void LevelChoosing(int new_level)
        {
            level = new_level;
            is_active = false;

        }
        protected int GetLevel()
        {
            return level;
        }
        protected bool GetIsActive()
        {
            return is_active;
        }

        protected void CreateRoom()
        {
            stopwatch = Stopwatch.StartNew();
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
            elapsedTime = "";
        }
        protected void GetCell(int x, int y, int num)
        {
            int[,] transferCell = getState();
            transferCell[x - 1, y - 1] = num;
            SetState(transferCell);
        }
        protected bool InputValidation(int x, int y)
        {
            int[,] validationcheck = getSave();

            bool Validation;
            if (validationcheck[x - 1, y - 1] != 0)
            {

                Validation = false;
                return (Validation);

            }
            else
            {
                Validation = true;
                return (Validation);
            }
        }

        protected bool WinCheck()
        {
            int[,] WinPretendent = getState();
            int[,] WinVariant = getWinner();
            bool wincheck = true;
            for (int i = 0; i < WinPretendent.GetLength(0); i++)
            {
                for (int j = 0; j < WinPretendent.GetLength(1); j++)
                {
                    if (WinPretendent[i, j] != WinVariant[i, j])
                    {
                        wincheck = false;
                        break;
                    }
                }
            }
            if (wincheck) is_active = false;
            return wincheck;
        }
        protected int[,] DeveloperWin()
        {
            int[,] devel = getWinner();
            SetState(devel);
            return devel;
        }
        protected enum Error
        {
            NO,
            One,
            Two,
            Three,
        }
        protected int ErrorCheck()
        {
            int error=0;
            int[,] map = getState();
            for (int X = 0; X < n * n; X++)
            {
                for (int Y = 0; Y < n * n; Y++)
                {
                    for (int j = 0; j < n * n; j++)
                    {
                        if ((map[X, j] == map[X, Y]) & (j!=Y) &(map[X, Y] > 0))
                        {
                            error = 1;
                            break;
                        }
                    }
                    for (int i = 0; i < n * n; i++)
                    {
                        if ((map[i, Y] == map[X, Y]) & (i != X) & (map[X, Y] > 0))
                        {
                            error = 2;
                            break;
                        }
                    }
                    if (X >= 0 & X <= 2 & error==0)
                    {
                        error = ErroeCheckSqere(X, Y);
                    }
                    if (X >= 3 & X <= 5 & error == 0)
                    {
                        error = ErroeCheckSqere(X, Y);
                    }
                    if (X >= 6 & X <= 8 & error == 0)
                    {
                        error = ErroeCheckSqere(X, Y);
                    }
                }
            }
            return error;

        }
        private int ErroeCheckSqere(int X, int Y)
        {
            int error = 0;
            int[,] map = getState();
            if (Y >= 0 && Y <= 2)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if ((map[i, j] == map[X, Y]) & (i != X) & (j != Y) & (map[X, Y] > 0))
                        {
                            error = 3;
                            break;
                        }
                    }
                    i = i + 6;
                }
            }
            if (Y >= 3 && Y <= 5)
            {
                for (int i = n; i < n * 2; i++)
                {
                    for (int j = n; j < n * 2; j++)
                    {
                        if ((map[i, j] == map[X, Y]) & (i != X) & (j != Y) & (map[X, Y] > 0))
                        {
                            error = 3;
                            break;
                        }
                    }
                    i = i + 6;
                }
            }
            if (Y >= 6 && Y <= 8)
            {
                for (int i = n * 2; i < n * 3; i++)
                {
                    for (int j = n * 2; j < n * 3; j++)
                    {
                        if ((map[i, j] == map[X, Y]) & (i != X) & (j != Y) & (map[X, Y] > 0))
                        {
                            error = 3;
                            break;
                        }
                    }
                    i = i + 6;
                }
            }
            return error;
        }
    }

    class SudokuUi : SudokuMap
    {
        private void StartMenu()
        {
            bool startMenuActive = true;
            bool appActive = true;
            bool youWinActive = false;
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
                        Console.WriteLine("3. Список лидеров");
                        Console.WriteLine("4. Выход");
                        int input = Convert.ToInt32(Console.ReadLine());
                        if (input < 0 || input > 4)
                        {
                            throw new Exception("Ошибка");
                        }
                        else
                        {
                            if (input == 0)
                            {
                                Timer_Start();
                                startMenuActive = false;
                                Console.Clear();
                                break;
                            }
                            if (input == 1)
                            {
                                base.CreateRoom();
                                startMenuActive = false;
                                Timer_Start();
                                Console.Clear();
                                break;
                            }
                            if (input == 2)
                            {
                                ChangeDifficulty();
                                Console.Clear();
                            }
                            if (input == 3)
                            {
                                Console.Clear();
                                foreach (string record in Record_Read())
                                {
                                    Console.WriteLine(record);
                                }
                                Console.WriteLine();
                            }
                            if (input == 4)
                            {
                                startMenuActive = false;
                                appActive = false;
                                break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Clear();
                        Console.WriteLine("Неверный формат ввода.");
                        Console.ForegroundColor = ConsoleColor.Black;
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
                        if (input < 1 || (input > 4))
                        {
                            throw new Exception("Ошибка");
                        }
                        else
                        {
                            if (input == 1)
                            {
                                InputCell();
                                Console.Clear();
                            }
                            if (input == 2)
                            {
                                if (WinCheck())
                                {
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Поздравляю, вы победили");
                                    Console.WriteLine("Ваше время: {0}", Timer_Stop());
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    youWinActive = true;
                                    break;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Clear();
                                    Console.WriteLine("Решение неверное, проверьте заполнены ли все клетки, нет ли одинаковых значений в каждом ряду и колонке");
                                    Console.ForegroundColor = ConsoleColor.Black;
                                }

                            }
                            if (input == 3)
                            {
                                Timer_Stop();
                                startMenuActive = true;
                                Console.Clear();
                                break;
                            }
                            if (input == 4)
                            {
                                Console.Clear();
                                base.DeveloperWin();
                                Console.WriteLine("DEVELOPMENTMODE");
                                int[,] a = base.getWinner();

                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Clear();
                        Console.WriteLine("Неверный формат ввода.");
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                }
                while (youWinActive)
                {

                    Console.WriteLine("Желаете ли вы отправить данные и занять место в сипке лидеров?");
                    Console.WriteLine("1. Да!");
                    Console.WriteLine("2. Нет, спасибо");
                    int input = Convert.ToInt32(Console.ReadLine());
                    if (input < 1 || (input > 2))
                    {
                        throw new Exception("Ошибка");
                    }
                    else
                    {
                        if (input == 1)
                        {
                            Console.WriteLine("Введите ваше имя");
                            string name = Console.ReadLine();
                            Record_Write(name);
                            youWinActive = false;
                            Console.Clear();
                            startMenuActive = true;
                            break;

                        }
                        if (input == 2)
                        {
                            Console.Clear();
                            youWinActive = false;
                            startMenuActive = true;
                            break;
                        }
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
            Error error = (Error)ErrorCheck();
            switch (error)
            {
                case Error.NO:
                    break;
                case Error.One:
                    Console.WriteLine("\nПодсказка: Одинаковые элементы в строке\n");
                    break;
                case Error.Two:
                    Console.WriteLine("\nПодсказка: Одинаковые элементы в столбце\n");
                    break;
                case Error.Three:
                    Console.WriteLine("\nПодсказка: Одинаковые элементы в квадрате 3 на 3\n");
                    break;
            }
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Неверный формат ввода.");
                    Console.ForegroundColor = ConsoleColor.Black;
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Неверный формат ввода.");
                    Console.ForegroundColor = ConsoleColor.Black;
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Неверный формат ввода.");
                    Console.ForegroundColor = ConsoleColor.Black;
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Вы ввели занятую ячейку\nНачните сначала");
                InputCell();
                Console.ForegroundColor = ConsoleColor.Black;
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Неверный формат ввода.");
                    Console.ForegroundColor = ConsoleColor.Black;
                }
            }
            while (true);
        }

        static void Main()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();
            SudokuUi map = new SudokuUi();
            map.StartMenu();
        }
    }
}