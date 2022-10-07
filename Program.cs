namespace room
{
    class SudokuUi
    {   
        private void PrintSudoku()
        {
            int n = 9;
            int[,] mas = new int[n, n];
            Random rn = new Random();
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++)
                    mas[i, j] = rn.Next(9);
                }
            Console.Write(String.Format("{0,3}", '|'));
            for (int j = 1; j <= n; j++)
                    Console.Write(String.Format("{0,2}", j));
            Console.WriteLine();
            Console.Write(String.Format("   {0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}{0,0}", '-'));
            Console.WriteLine();
            for (int i = 0; i < n; i++)
            {
                Console.Write(String.Format("{0,0}{1,0} ",  i+1, '}'));
                for (int j = 0; j < n; j++){
                    if(j != n-1){
                    Console.Write(String.Format("{0,1}{1,1}",  '|', mas[i, j] == 0 ? ' ' : mas[i, j].ToString() ));
                    } else {
                        Console.Write(String.Format("{0,1}{1,1}{2,1}",  '|', mas[i, j] == 0 ? ' ' : mas[i, j].ToString(), '|'));
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
                    if (x<1 || x>9){
                      throw new Exception("Ошибка");
                    } else {
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
                    if (y<1 || y>9){
                      throw new Exception("Ошибка");
                    } else {
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
                    if (num<1 || num>9){
                      throw new Exception("Ошибка");
                    } else {
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
            string[] difficultyArr = new string[3] {"Легкий", "Средниий", "Сложный"};
            int activeDifficulty = 0;
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
                    if (x<1 || x>3){
                      throw new Exception("Ошибка");
                    } else {
                        if(activeDifficulty != x-1){
                            activeDifficulty = x-1;
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
                        if (true) Console.WriteLine("0. Продолжить игру");
                        Console.WriteLine("1. Начать игру");
                        Console.WriteLine("2. Режим сложности");
                        Console.WriteLine("3. Выход");
                        int input = Convert.ToInt32(Console.ReadLine());
                        if (input<0 || input>3){
                        throw new Exception("Ошибка");
                        } else {
                            if (input == 0){
                                startMenuActive = false;
                                break;
                            }
                            if (input == 1){
                                startMenuActive = false;
                                break;
                            }
                            if (input == 2){
                                map.ChangeDifficulty();
                            }
                            if (input == 3){
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
                while (appActive) {
                    try
                    {
                        map.PrintSudoku();
                        Console.WriteLine("1. Ввести число");
                        Console.WriteLine("2. В главное меню");
                        int input = Convert.ToInt32(Console.ReadLine());
                        if (input<1 || input>2){
                        throw new Exception("Ошибка");
                        } else {
                            if (input == 1){
                            map.InputCell();
                            }
                            if (input == 2){
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

// Метод/функция которая возращает текущий массив судоку [,]
// Метод/функция которая начинает новую игру
// Метод/функция которая будет возвращать есть ли игра которую можно продолжить (true если есть, false если нет и можно только начать новую игру)
// Метод/функция которая будет вводить данные в судоку, фронт будет отсылать массив вида:
// {
//  x: index
//  y: index
// }
// Функция смены уровня сложности, при смене уровня сложность на беке сбрасывать текущую игру (нельзя будет продолжить, только начать новую)