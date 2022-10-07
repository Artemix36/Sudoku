using System.Diagnostics.Metrics;
using System.Globalization;

namespace Sudoku
{
    class SudokuState
    {
        int[,] state = new int[9,9];
        public void SetState(int[,] newState)
        {
            state = newState;
        }
        public int[,] getState()
        {
            return state;
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

        private void Hide(ref int[,] map, Random rnd )
        {
            int chance = 0, null_check=0, removed=0;

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
            Hide(ref map, rnd);
            base.SetState(map);
        }
    }

    class SudokuUi : SudokuMap
    {
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
            do
            {
                try
                {
                    Console.WriteLine("Введите координату по горизонтали:");
                    int x = Convert.ToInt32(Console.ReadLine());
                    if (x < 1 || x > 9)
                    {
                        throw new Exception("Ошибка");
                    }
                    else
                    {
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
                    Console.WriteLine("Введите координату по вертикали:");
                    int y = Convert.ToInt32(Console.ReadLine());
                    if (y < 1 || y > 9)
                    {
                        throw new Exception("Ошибка");
                    }
                    else
                    {
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
            bool startMenuActive = true;
            bool appActive = true;
            do
            {
                do
                {
                    try
                    {
                        Console.WriteLine("----МЕНЮ----");
                        if ( map.GetIsActive() ) Console.WriteLine("0. Продолжить игру");
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
                                map.CreateRoom();
                                startMenuActive = false;
                                break;
                            }
                            if (input == 2)
                            {
                                map.ChangeDifficulty();
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
                        map.PrintSudoku();
                        Console.WriteLine("1. Ввести число");
                        Console.WriteLine("2. В главное меню");
                        int input = Convert.ToInt32(Console.ReadLine());
                        if (input < 1 || input > 2)
                        {
                            throw new Exception("Ошибка");
                        }
                        else
                        {
                            if (input == 1)
                            {
                                map.InputCell();
                            }
                            if (input == 2)
                            {
                                startMenuActive = true;
                                break;
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
    }
}

// Метод/функция которая будет вводить данные в судоку, фронт будет отсылать массив вида:
// {
//  x: число
//  y: число
//  num: число для ввода
// }