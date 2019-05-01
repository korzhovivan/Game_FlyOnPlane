using System;
using static System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace hw1
{
    class Program
    {
        class CarEventArgs
        {
            //Сообщение о автомобиле
            public string Message { get; }

            public CarEventArgs(string message)
            {
                Message = message;
            }
        }

        private delegate void CarStateHandler(object sender, CarEventArgs e);

        private static void ShowInfoToConsole(object sender, CarEventArgs args)
        {
            Clear();
            WriteLine(args.Message);
        }

        class Weather
        {
            public int Value { set; get; }
            public Weather() { }
            public void New()
            {
                Random rand = new Random();
                Value = rand.Next(-200, 200);
            }
        }

        class Supervisor
        {
            Weather weather = new Weather();
           
            public string Name { get; set; }
            public int Recomendation { get; set; }

            public Supervisor() { }

            public Supervisor(string name)
            {
                Name = name;
            }
            public void UpdateWeather()
            {
                weather.New();
                Recomendation = weather.Value;
            }
        }
        
        class Pilot
        {

            // Состояния
            public bool Started { get; set; } = false; // Начал полет
            public bool MissonComplete { get; set; } = false; // Набрано 1000 скорость
            public bool Crash { get; set; } = false; // Разбился
            public bool Unsuitable { get; set; } = false; // Непригоден
            

            public bool MissionFail { get; set; } = false; //если болле 1000 разница

            public int Fine { get; set; }// штрафы
           
            //Высота и скорость
            public int Height { set; get; }  // 0-10.000 (21 положение по высоте)
            public int Speed { set; get; }// 0-1.000 (21 положение по скорости)

            public int RecomendHight { get; set; }   //Рукомендуемая высота

            public virtual event CarStateHandler EventCrash;
            public virtual event CarStateHandler EventUnsuitable;

            public Supervisor First = new Supervisor("Ivan");
            public Supervisor Second = new Supervisor("Vadym");

            public Pilot() {
                Height = 0;
                Speed = 0;
            }
            

            public void DrawPlaneInfo()// Отрисовка графика
            {
                int j= 21;
                Console.Clear();
                WriteLine("Высота\n");

                j = 21 - (Height / 250);
                if (Height == 0)
                {
                    j = 21;
                }

                

                for (int i = 1; i < 22; i++)
                {
                    if (j == i)
                    {
                        Write(5250 - (i * 250) + "\t");
                       
                        Write("|");
                        if (Speed != 0)
                        {
                            for (int s = 0; s < Speed/50; s++)
                            {
                                Write(" ");
                            }
                        }
                        WriteLine("#");
                    }
                    else
                    {
                        WriteLine(5250 - (i * 250) + "\t" + "|");
                    }
                   

                }
                Write("\t");
                for (int i = 0; i < 22; i++)
                {

                    Write("-");
                }
                if (Speed <=1000 && Speed >=0)
                {
                    Write("| Cкорость");
                }
                else
                {
                    Write("| Скорость (Недопустимая)");
                }
                Console.WriteLine("\n");
                
            }   
            public void Fly() // Управление самолетом
            {
                
                ConsoleKeyInfo key = ReadKey();
                // Без шифта (просто стрелки)
                if (key.Key == ConsoleKey.Tab)
                {
                    
                }

                switch (key.Key)
                {
                    case ConsoleKey.RightArrow:
                        Speed += 50;
                        break;
                    case ConsoleKey.LeftArrow:
                        Speed -= 50;
                        break;
                    case ConsoleKey.UpArrow:
                        Height += 250;
                        break;
                    case ConsoleKey.DownArrow:
                        Height -= 250;
                        break;
                    default:
                        break;
                }
                // Шифт + стрелки
                if (key.Modifiers == ConsoleModifiers.Shift && key.Key == ConsoleKey.RightArrow)
                {
                    Speed += 100;
                }
                if (key.Modifiers == ConsoleModifiers.Shift && key.Key == ConsoleKey.LeftArrow)
                {
                    Speed -= 100;
                }
                if (key.Modifiers == ConsoleModifiers.Shift && key.Key == ConsoleKey.UpArrow)
                {
                    Height += 500;
                }
                if (key.Modifiers == ConsoleModifiers.Shift && key.Key == ConsoleKey.DownArrow)
                {
                    Height -= 500;
                }
                //Выполненое задание
                if (Speed==500)
                {
                    this.MissonComplete = true;
                }
                // Самолет разбился
                if (Speed > 500)
                {
                    this.Crash = true;
                    EventCrash(this, new CarEventArgs($"Your Speed was more than 500"));
                }
                // Нельзя взлететь без скорости
                if (Speed == 0)
                {
                    Height = 0;
                }
                //выход за пределы
                if (Height > 5000)
                {
                    Height = 5000;
                }
                if (Height <0)
                {
                    Height = 0;
                }
                if (Speed < 0)
                {
                    Speed = 0;
                }
                if (MissonComplete && Speed == 0)
                {
                    Clear();
                    WriteLine("You a good flyer");
                    System.Threading.Thread.Sleep(3000);
                    Environment.Exit(0);
                }
            }
            public void UpdateRecomendation()
            {
                First.UpdateWeather();
                Second.UpdateWeather();
                if (Height == 0)
                {
                    RecomendHight = 0;
                }
                else
                {
                    RecomendHight = 7 * Speed - ((First.Recomendation + Second.Recomendation) / 2);
                }
                
            } // Обновение рекомендаци погоды
            public void FindFines()
            {
                int difference = System.Math.Abs(Height - RecomendHight); 

                if (difference>= 300 && difference <=600)
                {
                    this.Fine += 25;
                }
                if (difference >= 600 && difference <= 1000)
                {
                    this.Fine += 50;
                }
                if (difference > 1000)
                {
                    this.Crash = true;
                    EventCrash(this, new CarEventArgs("You dont hear supervisors"));
                }
                if (this.Fine > 1000)
                {
                    this.Unsuitable = true;
                    EventUnsuitable(this, new CarEventArgs("Fines more than 1000 score"));
                }
                if (!this.MissonComplete && (this.Height == 0 || this.Speed == 0) && this.Started)
                {
                    this.Crash = true;
                    EventCrash(this, new CarEventArgs("You came without get 500 speed. Mission Fail"));
                }
            }
            
            public void ShowDetails()
            {
                WriteLine("\nSpeed: " + this.Speed);
                WriteLine("Height: " + this.Height + "\n");
            }
        }

        static void Main(string[] args)
        {
            Pilot pilot = new Pilot();

            pilot.EventCrash += ShowInfoToConsole;
            pilot.EventUnsuitable += ShowInfoToConsole;

            WriteLine("Ваша задача набрать скорость 500 для выполнения задания и приземлится");
            WriteLine("Нажмите клавишу для нача ла\n");
            WriteLine("Your task is to pick up speed 500 to complete the task and land");
            WriteLine("Press key to start");
            ReadKey();


            while (true)
            {
                pilot.DrawPlaneInfo();
                pilot.ShowDetails();
                WriteLine(pilot.First.Name + ", " + pilot.Second.Name  + " рекомендуют высоту: " + pilot.RecomendHight);
                WriteLine("Штраф: " + pilot.Fine);
                pilot.Fly();
                pilot.UpdateRecomendation();
                pilot.FindFines();

                if (pilot.Unsuitable)// Непригоден
                {
                    break;
                }
                if (pilot.Crash) // Разбился
                {
                    break;
                }
            }
            ReadKey();
        }
    }   
}